import { S3Parameters } from "aws-sdk/clients/quicksight";
import express from "express";
import multer from "multer";
import multerS3 from "multer-s3";
import path from "path";
import { Body, Controller, Get, Path, Post, Request, Route, Security, Tags } from "tsoa";
import { uid } from "uid/secure";
import { markdownParser, redisInstance, s3Instance } from "../app";
import { fileFilter, WrappedLocals } from "../ImageUploader";
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
	password?: string;

	/**
	 * If mode is oauth2, you can give over here
	 */
	id?: string;
}

export interface SlugData {
	type: "url" | "file" | "text";
	value: string;
	mime?: string;
	size?: number;
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

			const params = {
				Body: req.file.buffer,
				Bucket: process.env.S3_BUCKET,
				ContentType: req.file.mimetype,
				Key: slug + path.extname(req.file.originalname),
			};
			await s3Instance.upload(params).promise();

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
}
