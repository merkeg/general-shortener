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
		assert(process.env.AUTHENTICATION_PASSWORD, "AUTHENTICATION_PASSWORD must be set");
		if (process.env.STORAGE_DRIVER == "local") {
			assert(process.env.STORAGE_LOCAL_DIR, "STORAGE_LOCAL_DIR must be set");
		}

		assert(process.env.REDIS_HOST, "REDIS_HOST must be set");
		assert(process.env.REDIS_PORT, "REDIS_PORT must be set");
		assert(process.env.REDIS_PASSWORD, "REDIS_PASSWORD must be set");
	} catch (err) {
		console.error(`Error starting process: ${err.message}`);
		process.exit(1);
	}
};
