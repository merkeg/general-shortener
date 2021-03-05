import express from "express";
import { promisify } from "util";
import { markdownParser, redisInstance, s3Instance } from "../app";
import { handleFileDownload } from "../storage/StorageDrivers";
import { SlugData } from "./EntryController";

export const handleSlug = async (request: express.Request, response: express.Response) => {
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
		return await handleFileDownload(slugData.value, slugData, request, response);
	} else {
		response.status(500).json({ message: "Internal Server error" });
	}
};
