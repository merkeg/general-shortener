import express from "express";
import { SlugData } from "../routes/EntryController";
import { localhandleFileDownload, localhandleUpload } from "./LocalDriver";

export async function handleUpload(file: Express.Multer.File, filename: string) {
	switch (process.env.STORAGE_DRIVER) {
		case "local":
			await localhandleUpload(file, filename);
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
		default:
			break;
	}
}
