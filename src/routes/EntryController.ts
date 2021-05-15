import express from "express";
import { unlinkSync } from "fs";
import { join } from "path";
import path from "path";
import { Body, Controller, Delete, Get, Path, Post, Request, Route, Security, Tags } from "tsoa";
import { uid } from "uid/secure";
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
}

export interface EntryDeletionParams {
	password: string;
}
export interface WrappedLocals {
	slug: string;
}

@Route("/")
@Tags("Application")
export class NewEntryController extends Controller {
	@Security("a")
	@Post("new")
	public async createNewEntry(@Body() params: NewEntryParams, @Request() req: express.Request) {
		const existsAsync = promisify(redisInstance.exists).bind(redisInstance);

		var slug = params.slug || uid(parseInt(process.env.SLUG_LEN || "7"));
		if (!params.slug) {
			while (await existsAsync(slug)) {
				slug = uid(parseInt(process.env.SLUG_LEN || "7"));
			}
		}

		if (params.type == "file") {
			params.value = slug + path.extname(req.file.originalname);
		}
		const slugData: SlugData = {
			type: params.type,
			value: params.value,
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
			return {
				message: {
					url: process.env.SERVER_BASE_URL + "/" + slug,
				},
			};
		} else {
			this.setStatus(200);
			return {
				message: {
					url: process.env.SERVER_BASE_URL + "/" + slug,
				},
			};
		}
	}

	@Security("a")
	@Delete("{slug}")
	public async deleteEntry(slug: string, @Body() params: EntryDeletionParams, @Request() req: express.Request) {
		const existsAsync = promisify(redisInstance.exists).bind(redisInstance);
		const getAsync = promisify(redisInstance.get).bind(redisInstance);

		if (await existsAsync(slug)) {
			var slugData: SlugData = JSON.parse(await getAsync(slug));

			if (slugData.type == "file") {
				var path = join(process.env.STORAGE_LOCAL_DIR, slugData.value);
				unlinkSync(path);
			}

			redisInstance.DEL(slug);

			return {
				message: "Slug deleted",
			};
		} else {
			this.setStatus(404);
			return {
				message: "Slug does not exist.",
			};
		}
	}
}
