import express from "express";
import multer from "multer";
import multerS3 from "multer-s3";
import path from "path";
import { s3Instance } from "./app";

export const fileFilter = (req, file, cb) => {
	if (file.mimetype === "image/jpeg" || file.mimetype === "image/png") {
		cb(null, true);
	} else {
		cb(new Error("Invalid file type, only PNG is allowed!"), false);
	}
};

export interface WrappedLocals {
	slug: string;
}
