import express from "express";
import { AuthenticationError } from "./ErrorHandler";

export async function expressAuthentication(request: express.Request, securityName: string, scopes: string[]): Promise<any> {
	if (request.cookies.id != undefined) {
		return handleAuthCookie(request, securityName, scopes);
	}

	if (request.body.password != undefined) {
		return handleAuthBody(request, securityName, scopes);
	}

	if (request.query.password != undefined) {
		return handleAuthQuery(request, securityName, scopes);
	}

	return Promise.reject(new AuthenticationError("Not authenticated"));
}

async function handleAuthCookie(request: express.Request, securityName: string, scopes: string[]) {
	var id = request.cookies.id;
	return Promise.reject(new AuthenticationError("Not yet implemented"));
}

async function handleAuthQuery(request: express.Request, securityName: string, scopes: string[]) {
	if (request.query.password != undefined) {
		if (request.query.password == process.env.AUTHENTICATION_PASSWORD) {
			return Promise.resolve(true);
		}
		return Promise.reject(new AuthenticationError("Invalid Password"));
	}
	return Promise.reject(new AuthenticationError("Password not present"));
}

async function handleAuthBody(request: express.Request, securityName: string, scopes: string[]) {
	if (request.body.password != undefined) {
		if (request.body.password == process.env.AUTHENTICATION_PASSWORD) {
			return Promise.resolve(true);
		}
		return Promise.reject(new AuthenticationError("Invalid Password"));
	}
	return Promise.reject(new AuthenticationError("Password not present"));
}
