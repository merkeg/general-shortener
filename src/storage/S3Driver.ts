import express from "express";
import { s3Instance } from "../app";
import { SlugData } from "../routes/EntryController";

export async function s3handleUpload(file: Express.Multer.File, filename: string) {
	const params = {
		Body: file.buffer,
		Bucket: process.env.STORAGE_S3_BUCKET,
		ContentType: file.mimetype,
		Key: filename,
	};
	await s3Instance.upload(params).promise();
}

export async function s3handleFileDownload(filename: string, slugData: SlugData, request: express.Request, response: express.Response) {
	const noDownload = ["image/png", "image/jpeg", "image/jpg", "application/pdf", "video/x-matroska", "video/mp4", "audio/mp4", "audio/mpeg"];

	var params = {
		Bucket: process.env.STORAGE_S3_BUCKET,
		Key: filename,
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
		if (!noDownload.includes(slugData.mime)) {
			response.header("Content-disposition", "attachment; filename=" + slugData.value);
		}
		response.send(data.Body);
	});
}
