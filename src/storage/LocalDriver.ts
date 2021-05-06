import express from "express";
import { createReadStream, writeFileSync } from "fs";
import { stat } from "fs/promises";
import { join } from "path";
import { SlugData } from "../routes/EntryController";

export async function localhandleUpload(file: Express.Multer.File, filename: string) {
	writeFileSync(join(process.env.STORAGE_LOCAL_DIR, filename), file.buffer);
	return true;
}

export async function localhandleFileDownload(filename: string, slugData: SlugData, request: express.Request, response: express.Response) {
	const noDownload = ["image/png", "image/jpeg", "image/jpg", "image/gif", "application/pdf", "video/x-matroska", "video/mp4", "audio/mp4", "audio/mpeg", "video/avi"];

	const stats = await stat(join(process.env.STORAGE_LOCAL_DIR, filename));

	response.setHeader("Accept-Ranges", "bytes");
	response.setHeader("Content-Length", stats.size);
	response.setHeader("Content-Type", slugData.mime);
	if (!noDownload.includes(slugData.mime)) {
		response.header("Content-disposition", "attachment; filename=" + slugData.value);
	}

	var range = request.header("range");
	if (!request.header("range")) {
		range = `bytes=0-`;
	}

	var positions = range.replace(/bytes=/, "").split("-");
	var start = parseInt(positions[0], 10);
	var total = stats.size;
	var end = positions[1] ? parseInt(positions[1], 10) : total - 1;
	var chunksize = end - start + 1;

	response.setHeader("Content-Range", "bytes " + start + "-" + end + "/" + total);
	return createReadStream(join(process.env.STORAGE_LOCAL_DIR, filename)).pipe(response);
}
