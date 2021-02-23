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
}

@Route("/")
@Tags("Application")
export class NewEntryController extends Controller {
	@Security("a")
	@Post("new")
	public async createNewEntry(@Body() params: NewEntryParams, @Request() req: express.Request) {
		const existsAsync = promisify(redisInstance.exists).bind(redisInstance);

		if (params.slug) {
			if (await existsAsync(params.slug)) {
				this.setStatus(400);
				return {
					message: "Slug does already exist",
				};
			}
		}

		var slug = params.slug || uid(parseInt(process.env.SLUG_LEN || "7"));

		while (await existsAsync(slug)) {
			slug = uid(parseInt(process.env.SLUG_LEN || "7"));
		}
		if (params.type == "file") {
			params.value = slug + path.extname(req.file.originalname);
		}
		redisInstance.set(slug, JSON.stringify({ type: params.type, value: params.value }));

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
