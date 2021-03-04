import express from "express";
import { promisify } from "util";
import { markdownParser, redisInstance, s3Instance } from "../app";
import { SlugData } from "./EntryController";

export const handleLink = async (request: express.Request, response: express.Response) => {
	const getAsync = promisify(redisInstance.get).bind(redisInstance);

	const slug = <string>request.params.slug;
	var slugData: SlugData | undefined = JSON.parse(await getAsync(slug));

	if (slugData == undefined) {
		if (process.env.HTTP_404_REDIRECT) {
			response.redirect(process.env.HTTP_404_REDIRECT);
		} else {
			response.status(404).json({
				message: "Unknown slug",
			});
		}
		return;
	}

	if (slugData.type == "url") {
		response.redirect(slugData.value);
	} else if (slugData.type == "text") {
		response.send(process.env.TEXT_WRAPPER.replace("{{content}}", markdownParser.render(slugData.value)));
	} else if (slugData.type == "file") {
		const noDownload = ["image/png", "image/jpeg", "image/jpg", "application/pdf", "audio/mpeg"];
		const longMedia = ["video/x-matroska", "video/mp4", "audio/mp4", "audio/mpeg"];

		if (longMedia.includes(slugData.mime)) {
			handleAudioVideo(request, response, slugData);
			return;
		}

		var params = {
			Bucket: process.env.S3_BUCKET,
			Key: slugData.value,
		};

		s3Instance
			.getObject(params)
			.on("httpHeaders", async function (statusCode, headers) {
				if (statusCode == 404) {
					response.status(404).json({
						success: false,
						message: "File not found",
					});
				}

				response.header("Content-Length", headers["content-length"]);
				response.header("Content-Type", headers["content-type"]);
				if (!noDownload.includes(headers["content-type"])) {
					response.header("Content-disposition", "attachment; filename=" + slugData.value);
				}
			})
			.createReadStream()
			.pipe(response);
	} else {
		response.status(500).json({ message: "Internal Server error" });
	}
};

async function handleAudioVideo(request: express.Request, response: express.Response, slugData: SlugData) {
	var params = {
		Bucket: process.env.S3_BUCKET,
		Key: slugData.value,
		Range: undefined,
	};

	if (request.header("range")) {
		params.Range = request.header("range");
	} else {
		params.Range = `bytes=0-`;
	}

	s3Instance.getObject(params, (err, data) => {
		if (err) {
			return console.log(err);
		}
		response.status(206);
		if (data.ContentRange) {
			response.setHeader("Content-Range", data.ContentRange);
		}
		response.setHeader("Accept-Ranges", "bytes");
		response.setHeader("Content-Length", data.ContentLength);
		response.setHeader("Content-Type", slugData.mime);
		response.send(data.Body);
	});
}
