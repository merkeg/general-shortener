import express from "express";
import { SlugData } from "../routes/EntryController";
import { localhandleFileDownload, localhandleUpload } from "./LocalDriver";
import { s3handleFileDownload, s3handleUpload } from "./S3Driver";

export async function handleUpload(file: Express.Multer.File, filename: string) {
	switch (process.env.STORAGE_DRIVER) {
		case "local":
			await localhandleUpload(file, filename);
			break;
		case "s3":
			await s3handleUpload(file, filename);
			break;
		default:
			break;
	}
}

export async function handleFileDownload(filename: string, slugData: SlugData, req: express.Request, res: express.Response) {
	switch (process.env.STORAGE_DRIVER) {
		case "local":
			await localhandleFileDownload(filename, slugData, req, res);
			break;
		case "s3":
			await s3handleFileDownload(filename, slugData, req, res);
			break;
		default:
			break;
	}
}
