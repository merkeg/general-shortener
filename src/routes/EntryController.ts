import express, { query } from "express";
import { unlinkSync } from "fs";
import { join } from "path";
import path from "path";
import { Body, Controller, Delete, Get, Path, Post, Query, Request, Response, Route, Security, Tags } from "tsoa";
import { markdownParser, redisInstance } from "../app";
import { handleUpload } from "../storage/StorageDrivers";
const { promisify } = require("util");

export interface NewEntryParams {
	/**
	 * What type the new entry is
	 */
	type: "url" | "file" | "text";

	/**
	 * The value for the new entry
	 */
	value?: string;

	/**
	 * Wish slug to use
	 */
	slug?: string;

	/**
	 * If mode is password, you can give over here
	 */
	password: string;
}

export interface SlugData {
	type: "url" | "file" | "text";
	value: string;
	mime?: string;
	size?: number;
	deletionCode?: string;
}

export interface EntryDeletionParams {
	password: string;
}
export interface WrappedLocals {
	slug: string;
}

export interface EntryListParams {
	offset?: number;
	amount?: number;
}

@Route("/")
@Tags("Application")
export class NewEntryController extends Controller {
	@Security("a")
	@Post("new")
	public async createNewEntry(@Body() params: NewEntryParams, @Request() req: express.Request) {
		const existsAsync = promisify(redisInstance.exists).bind(redisInstance);

		var slug = params.slug || this.makeid(parseInt(process.env.SLUG_LEN || "7"));
		if (!params.slug) {
			while (await existsAsync(slug)) {
				slug = this.makeid(parseInt(process.env.SLUG_LEN || "7"));
			}
		}

		if (params.type == "file") {
			params.value = slug + path.extname(req.file.originalname);
		}
		const slugData: SlugData = {
			type: params.type,
			value: params.value,
			deletionCode: this.makeid(10),
		};

		if (req.file) {
			slugData.mime = req.file.mimetype;
			slugData.size = req.file.size;
		}

		redisInstance.set(slug, JSON.stringify(slugData));

		if (params.type == "file") {
			var locals = <WrappedLocals>req.res.locals;
			locals.slug = slug;

			await handleUpload(req.file, slug + path.extname(req.file.originalname));
		}

		this.setStatus(200);
		return {
			message: {
				url: process.env.SERVER_BASE_URL + "/" + slug,
				deletionUrl: process.env.SERVER_BASE_URL + "/" + slug + "/" + slugData.deletionCode,
			},
		};
	}

	@Security("a")
	@Get("list")
	public async getList(@Query() offset: number = 0, @Query() amount: number = 100, @Query() pattern: string = "*") {
		const scanAsync = promisify(redisInstance.scan).bind(redisInstance);
		const getAsync = promisify(redisInstance.get).bind(redisInstance);

		var data = await scanAsync(offset, "MATCH", pattern, "COUNT", amount);
		var newOffset: number = data[0];
		var keys: string[] = data[1] || [];
		var out = [];
		for (var i = 0; i < keys.length; i++) {
			var slugData: SlugData;
			try {
				slugData = JSON.parse(await getAsync(keys[i]));
			} catch (e) {
				continue;
			}
			out.push({
				slug: keys[i],
				type: slugData.type,
				mime: slugData.mime,
				size: slugData.size,
				deletionCode: slugData.deletionCode,
			});
		}
		return {
			message: {
				newOffset: newOffset,
				data: out,
			},
		};
	}

	@Security("a")
	@Get("{slug}/info")
	public async getEntryInfo(slug: string, @Request() req: express.Request) {
		const existsAsync = promisify(redisInstance.exists).bind(redisInstance);
		const getAsync = promisify(redisInstance.get).bind(redisInstance);
		if (await existsAsync(slug)) {
			var slugData: SlugData = JSON.parse(await getAsync(slug));
			return {
				message: {
					slug: slug,
					type: slugData.type,
					mime: slugData.mime,
					size: slugData.size,
					deletionCode: slugData.deletionCode,
				},
			};
		}

		this.setStatus(404);
		return {
			message: "Entry does not exist.",
		};
	}

	@Get("{slug}/{deletionCode}")
	public async getDeleteEntry(slug: string, deletionCode: string, @Request() req: express.Request) {
		return this.delete(slug, deletionCode, false, req);
	}

	@Security("a")
	@Delete("{slug}")
	public async deleteEntry(slug: string, @Request() req: express.Request) {
		return this.delete(slug, null, true, req);
	}

	public async delete(slug: string, deletionCode: string, force: boolean, req: express.Request) {
		const existsAsync = promisify(redisInstance.exists).bind(redisInstance);
		const getAsync = promisify(redisInstance.get).bind(redisInstance);

		if (await existsAsync(slug)) {
			var slugData: SlugData = JSON.parse(await getAsync(slug));

			if (!force) {
				if (deletionCode !== slugData.deletionCode) {
					this.setStatus(403);
					return {
						message: "Wrong deletion code.",
					};
				}
			}

			if (slugData.type == "file") {
				var path = join(process.env.STORAGE_LOCAL_DIR, slugData.value);
				unlinkSync(path);
			}

			redisInstance.DEL(slug);

			if (process.env.HTTP_DELETION_REDIRECT) {
				req.res.redirect(process.env.HTTP_DELETION_REDIRECT);
				return;
			} else {
				this.setStatus(404);
				return {
					message: "Entry does not exist.",
				};
			}
		} else {
			if (process.env.HTTP_404_REDIRECT) {
				req.res.redirect(process.env.HTTP_404_REDIRECT);
				return;
			} else {
				this.setStatus(404);
				return {
					message: "Entry does not exist.",
				};
			}
		}
	}

	private makeid(length) {
		var result = [];
		var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		var charactersLength = characters.length;
		for (var i = 0; i < length; i++) {
			result.push(characters.charAt(Math.floor(Math.random() * charactersLength)));
		}
		return result.join("");
	}
}
