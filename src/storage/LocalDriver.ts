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
	const noDownload = ["image/*", "video/*", "application/pdf"];

	const stats = await stat(join(process.env.STORAGE_LOCAL_DIR, filename));

	response.setHeader("Accept-Ranges", "bytes");
	response.setHeader("Content-Length", stats.size);
	response.setHeader("Content-Type", slugData.mime);

	let download = true;
	for (let i = 0; i < noDownload.length; i++) {
		if (slugData.mime.match(noDownload[i])) {
			download = false;
		}
	}
	if (download) {
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
