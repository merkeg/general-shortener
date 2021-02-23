declare global {
	namespace NodeJS {
		interface ProcessEnv {
			NODE_ENV: "development" | "production";
			BASEDIR: string;

			SERVER_PORT: string;
			SERVER_BASE_URL: string;

			ENVIRONMENT_FILE: string;

			OAUTH2_CERT_FILE: string;
			OAUTH2_CERT: string;
			OAUTH2_ISSUER: string;
			OAUTH2_CONSENT_URI: string;
			OAUTH2_TOKEN_URI: string;
			OAUTH2_PROFILE_URI: string;
			OAUTH2_PROFILE_ID: string;
			OAUTH2_SCOPES: string;
			OAUTH2_CLIENT_ID: string;
			OAUTH2_CLIENT_SECRET: string;

			AUTHENTICATION_PASSWORD: string;

			S3_ACCESS_KEY: string;
			S3_SECRET_KEY: string;
			S3_ENDPOINT: string;
			S3_BUCKET: string;

			REDIS_HOST: string;
			REDIS_PORT: string;
			REDIS_PASSWORD: string;
			SLUG_LEN: string;

			TEXT_WRAPPER: string;
		}
	}
}

export {};
