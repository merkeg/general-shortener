import * as dotenv from "dotenv";
import path from "path";
import fs from "fs";
import { strict as assert } from "assert";
import { AssertionError } from "assert";

export const loadAndCheckEnvironment = () => {
	if (process.env.ENVIRONMENT_FILE) {
		dotenv.config({ path: process.env.ENVIRONMENT_FILE.replace("%BASE_DIR%", process.env.BASEDIR) });
	}

	if (process.env.OAUTH2_CERT_FILE) {
		process.env.OAUTH2_CERT = fs.readFileSync(String(process.env.OAUTH2_CERT_FILE).replace("%BASE_DIR%", process.env.BASEDIR)).toString();
	}

	process.env.TEXT_WRAPPER = fs.readFileSync(path.join(process.env.BASEDIR, "wrapper.html")).toString();

	try {
		if (!process.env.AUTHENTICATION_PASSWORD) {
			assert(process.env.OAUTH2_ISSUER, "OAUTH2_ISSUER must be set");
			assert(process.env.OAUTH2_CONSENT_URI, "OAUTH2_CONSENT_URI must be set");
			assert(process.env.OAUTH2_CERT, "OAUTH2_CERT must be set");
			assert(process.env.SERVER_BASE_URL, "SERVER_BASE_URL must be set");
			assert(process.env.OAUTH2_PROFILE_URI, "OAUTH2_PROFILE_URI must be set");
			assert(process.env.OAUTH2_PROFILE_ID, "OAUTH2_PROFILE_ID must be set");
			assert(process.env.OAUTH2_SCOPES, "OAUTH2_SCOPES must be set");
			assert(process.env.OAUTH2_CLIENT_ID, "OAUTH2_CLIENT_ID must be set");
			assert(process.env.OAUTH2_CLIENT_SECRET, "OAUTH2_CLIENT_SECRET must be set");
		} else if (process.env.AUTHENTICATION_MODE == "password") {
			assert(process.env.AUTHENTICATION_PASSWORD, "AUTHENTICATION_PASSWORD must be set");
		}
		if (process.env.STORAGE_DRIVER == "local") {
			assert(process.env.STORAGE_LOCAL_DIR, "STORAGE_LOCAL_DIR must be set");
		} else {
			assert(process.env.STORAGE_S3_ACCESS_KEY, "STORAGE_S3_ACCESS_KEY must be set");
			assert(process.env.STORAGE_S3_SECRET_KEY, "STORAGE_S3_SECRET_KEY must be set");
			assert(process.env.STORAGE_S3_ENDPOINT, "STORAGE_S3_ENDPOINT must be set");
			assert(process.env.STORAGE_S3_BUCKET, "STORAGE_S3_BUCKET must be set");
		}

		assert(process.env.REDIS_HOST, "REDIS_HOST must be set");
		assert(process.env.REDIS_PORT, "REDIS_PORT must be set");
		assert(process.env.REDIS_PASSWORD, "REDIS_PASSWORD must be set");
	} catch (err) {
		console.error(`Error starting process: ${err.message}`);
		process.exit(1);
	}
};
