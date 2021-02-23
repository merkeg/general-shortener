import express from "express";
import { Body, Controller, Get, Path, Post, Query, Request, Response, Route, Tags } from "tsoa";
import fetch from "node-fetch";
import { RedisClient } from "redis";
import { redisInstance } from "../app";
import { verify, VerifyErrors } from "jsonwebtoken";
import { Verify } from "crypto";

export interface StandardJWT {
	uid?: string;
	type: string;
	iat?: number;
	exp?: number;
	iss?: string;
	sub?: string;
	aud: string;
}

@Route("/")
@Tags("Authorization")
export class AuthorizationController extends Controller {
	@Get("login")
	public async handleLogin(@Request() req: express.Request) {
		var uri = process.env.OAUTH2_CONSENT_URI;
		uri += `?client_id=${process.env.OAUTH2_CLIENT_ID}`;
		uri += `&redirect_uri=${encodeURIComponent(process.env.SERVER_BASE_URL)}/callback`;
		uri += `&scope=${process.env.OAUTH2_SCOPES}`;
		uri += `&response_type=code`;
		uri += `&response_mode=query`;
		req.res.redirect(uri);
	}

	@Get("callback")
	public async handleCallback(@Query("code") code: string, @Request() req: express.Request) {
		var response = await fetch(process.env.OAUTH2_TOKEN_URI, {
			method: "POST",
			headers: {
				"Content-Type": "application/json",
				Authorization: "Bearer " + new Buffer(process.env.OAUTH2_CLIENT_ID + ":" + process.env.OAUTH2_CLIENT_SECRET).toString("base64"),
			},
			body: JSON.stringify({
				grant_type: "authorization_code",
				authorization_code: code,
			}),
		});

		var json: { id_token: string; access_token: string; refresh_token: string } = (await response.json()).message;
		if (response.ok) {
			try {
				var token: StandardJWT = <StandardJWT>verify(json.id_token, process.env.OAUTH2_CERT, {
					algorithms: ["RS256"],
					issuer: process.env.OAUTH2_ISSUER,
					audience: process.env.OAUTH2_CLIENT_ID,
				});
				redisInstance.set(token.sub, json.refresh_token);
				req.res.cookie("id", json.id_token, {
					httpOnly: true,
					domain: process.env.SERVER_BASE_URL,
				});

				this.setStatus(200);

				return {
					message: "OK",
				};
			} catch (err) {
				this.setStatus(500);

				console.log("Error", err);
				return {
					message: "Invalid id_token",
				};
			}
		} else {
			this.setStatus(500);
			return {
				message: "Invalid or expired code",
			};
		}
	}
}
