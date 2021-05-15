declare global {
	namespace NodeJS {
		interface ProcessEnv {
			NODE_ENV: "development" | "production";
			BASEDIR: string;

			SERVER_PORT: string;
			SERVER_BASE_URL: string;

			ENVIRONMENT_FILE: string;

			AUTHENTICATION_PASSWORD: string;

			STORAGE_DRIVER: "local";

			STORAGE_LOCAL_DIR: string;

			REDIS_HOST: string;
			REDIS_PORT: string;
			REDIS_PASSWORD: string;
			SLUG_LEN: string;

			TEXT_WRAPPER: string;
			HTTP_404_REDIRECT: string;
			HTTP_BASE_REDIRECT: string;
		}
	}
}

export {};
