import express from "express";
import { promisify } from "util";
import { markdownParser, redisInstance, s3Instance } from "../app";
import { SlugData } from "./EntryController";

export const handleLink = async (request: express.Request, response: express.Response) => {
	const getAsync = promisify(redisInstance.get).bind(redisInstance);

	const slug = <string>request.params.slug;
	var slugData: SlugData | undefined = JSON.parse(await getAsync(slug));

	if (slugData == undefined) {
		response.status(404).json({
			message: "Unknown slug",
		});
		return;
	}

	if (slugData.type == "url") {
		response.redirect(slugData.value);
	} else if (slugData.type == "text") {
		response.send(process.env.TEXT_WRAPPER.replace("{{content}}", markdownParser.render(slugData.value)));
	} else if (slugData.type == "file") {
		var params = {
			Bucket: process.env.S3_BUCKET,
			Key: slugData.value,
		};
		const noDownload = ["image/png", "image/jpeg", "image/jpg", "application/pdf", "audio/mpeg", "video/x-matroska", "video/mp4"];
		const videos = ["video/x-matroska", "video/mp4"];

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

				// TODO: Videos don't work, need workaround
				// if (videos.includes(headers["content-type"]) && request.headers.range) {
				// 	var range = request.headers.range;
				// 	var bytes = range.replace(/bytes=/, "").split("-");
				// 	var start = parseInt(bytes[0], 10);
				// 	var total = parseInt(headers["content-length"]);
				// 	var end = bytes[1] ? parseInt(bytes[1], 10) : total - 1;
				// 	response.header("Content-Range", "bytes " + start + "-" + end + "/" + total);
				// }
			})
			.createReadStream()
			.pipe(response);
	} else {
		response.status(500).json({ message: "Internal Server error" });
	}
};
