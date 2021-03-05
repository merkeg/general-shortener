import { loadAndCheckEnvironment } from "./lib/env/env";

import express, { NextFunction } from "express";
import bodyParser from "body-parser";
import { RegisterRoutes } from "../build/routes";
import swaggerUi from "swagger-ui-express";
import { RedisClient } from "redis";
import AWS from "aws-sdk";
import { ValidateError } from "tsoa";
import { JsonWebTokenError } from "jsonwebtoken";
import multer from "multer";
import { errorHandler } from "./lib/ErrorHandler";
import markdownit from "markdown-it/lib";
import { getLanguage, highlight, highlightAll, initHighlighting } from "highlight.js";
import { handleSlug } from "./routes/Slug";
const cookieParser = require("cookie-parser");

process.env.BASEDIR = __dirname;
loadAndCheckEnvironment();

export const app = express();

/**
 * EXPRESS ROUTES
 */

app.use(
	bodyParser.urlencoded({
		extended: true,
	})
);

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));
app.use(cookieParser());
app.post("/new", multer().single("file"));

app.get("/", (req, res) => {
	if (process.env.HTTP_BASE_REDIRECT) {
		res.redirect(process.env.HTTP_BASE_REDIRECT);
	} else {
		res.status(200).send("Hello world");
	}
});

app.use("/docs", swaggerUi.serve, async (_req: express.Request, res: express.Response) => {
	return res.send(swaggerUi.generateHTML(await import("../build/swagger.json")));
});

app.get("/:slug", handleSlug);

RegisterRoutes(app);

/**
 * EXPRESS
 */

const port = process.env.SERVER_PORT || 5000;
app.listen(port, () => console.log(`Application listening at http://localhost:${port}`));

/**
 * REDIS
 */
export const redisInstance: RedisClient = require("redis").createClient({
	host: process.env.REDIS_HOST,
	port: process.env.REDIS_PORT,
	password: process.env.REDIS_PASSWORD,
});

redisInstance.on("connect", () => {
	console.log("[Redis]", "Connection to database established");
});

redisInstance.on("error", (err: Error) => {
	console.log("[Redis]", "Error with redis: " + err.message);
});

/**
 * S3
 */
export var s3Instance: AWS.S3 = undefined;
if (process.env.STORAGE_DRIVER == "s3") {
	s3Instance = new AWS.S3({
		accessKeyId: process.env.STORAGE_S3_ACCESS_KEY,
		secretAccessKey: process.env.STORAGE_S3_SECRET_KEY,
		endpoint: process.env.STORAGE_S3_ENDPOINT,
		s3ForcePathStyle: true,
		signatureVersion: "v4",
	});

	checkBucket();
	async function checkBucket() {
		var params = {
			Bucket: process.env.STORAGE_S3_BUCKET,
		};
		try {
			await s3Instance.headBucket(params).promise();
		} catch (e) {
			s3Instance.createBucket(params, (err, data) => {
				if (err) console.log(err.message);
			});
		}
	}
}

app.use(errorHandler);

/**
 * MARKDOWN
 */

highlightAll();
export const markdownParser = markdownit({
	highlight: function (str, lang) {
		if (lang && getLanguage(lang)) {
			try {
				return highlight(lang, str).value;
			} catch (__) {}
		}

		return "";
	},
	html: true,
	typographer: true,
	linkify: true,
});
