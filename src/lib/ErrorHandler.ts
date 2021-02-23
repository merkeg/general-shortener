import express, { NextFunction } from "express";
import { JsonWebTokenError } from "jsonwebtoken";
import { ValidateError } from "tsoa";

export function errorHandler(err: unknown, req: express.Request, res: express.Response, next: NextFunction): Express.Response | void {
	if (err instanceof ValidateError) {
		return res.status(422).json({
			message: "Validation Failed",
			details: err?.fields,
		});
	}
	if (err instanceof JsonWebTokenError) {
		return res.status(422).json({
			message: "JSON web token invalid",
		});
	}

	if (err instanceof AuthenticationError) {
		return res.status(403).json({
			message: err.message,
		});
	}

	if (err instanceof Error) {
		return res.status(500).json({
			message: err.message,
		});
	}

	next();
}

export class AuthenticationError extends Error {}
