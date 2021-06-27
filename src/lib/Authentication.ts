import express from "express";
import { AuthenticationError } from "./ErrorHandler";

export async function expressAuthentication(request: express.Request, securityName: string, scopes: string[]): Promise<any> {
    if (request.body.password != undefined) {
        return handleAuthBody(request, securityName, scopes);
    }

    if (request.query.password != undefined) {
        return handleAuthQuery(request, securityName, scopes);
    }

    if (request.headers.authorization != undefined) {
        return handleAuthToken(request, securityName, scopes);
    }

    return Promise.reject(new AuthenticationError("Not authenticated"));
}

async function handleAuthQuery(request: express.Request, securityName: string, scopes: string[]) {
    if (request.query.password != undefined) {
        if (request.query.password == process.env.AUTHENTICATION_PASSWORD) {
            return Promise.resolve(true);
        }
        return Promise.reject(new AuthenticationError("Invalid Token"));
    }
    return Promise.reject(new AuthenticationError("Token not present"));
}

async function handleAuthBody(request: express.Request, securityName: string, scopes: string[]) {
    if (request.body.password != undefined) {
        if (request.body.password == process.env.AUTHENTICATION_PASSWORD) {
            return Promise.resolve(true);
        }
        return Promise.reject(new AuthenticationError("Invalid Token"));
    }
    return Promise.reject(new AuthenticationError("Token not present"));
}

async function handleAuthToken(request: express.Request, securityName: string, scopes: string[]) {
    const bearer = request.headers.authorization.split(" ");
    const bearerToken = bearer[1];
    if (bearerToken == process.env.AUTHENTICATION_PASSWORD) {
        return Promise.resolve(true);
    }
    return Promise.reject(new AuthenticationError("Invalid Token"));
}
