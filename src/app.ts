import { loadAndCheckEnvironment } from "./lib/env/env";

import express, { NextFunction } from "express";
import bodyParser from "body-parser";
import { RegisterRoutes } from "../build/routes";
import swaggerUi from "swagger-ui-express";
import { RedisClient } from "redis";
import multer from "multer";
import { errorHandler } from "./lib/ErrorHandler";
import markdownit from "markdown-it/lib";
import { getLanguage, highlight, highlightAll, initHighlighting } from "highlight.js";
import { handleSlug } from "./routes/Slug";
import cors from "cors";

process.env.BASEDIR = __dirname;
loadAndCheckEnvironment();

export const app = express();

/**
 * CORS Protection
 */

// let origins = process.env.CORS?.split(",");
// let apiCors = cors({
//     origin: (origin, callback) => {
//         if (origins.indexOf(origin) !== -1) {
//             callback(null, true);
//         } else {
//             callback(new Error("Not allowed by CORS"));
//         }
//     },
// });

// app.delete("/:slug", apiCors);
// app.get("/:slug/info", apiCors);
// app.get("/list", apiCors);

app.use(cors());
/**
 * EXPRESS ROUTES
 */
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

app.post("/new", multer().single("file"));

app.get("/", (req, res) => {
    if (process.env.HTTP_BASE_REDIRECT) {
        res.redirect(process.env.HTTP_BASE_REDIRECT);
    } else {
        res.status(200).send("API OK");
    }
});

app.use("/docs", swaggerUi.serve, async (_req: express.Request, res: express.Response) => {
    return res.send(swaggerUi.generateHTML(await import("../build/swagger.json")));
});

RegisterRoutes(app);
app.get("/:slug", handleSlug);

/**
 * EXPRESS
 */

const port = process.env.SERVER_PORT || 5000;
app.listen(port, () => console.log(`Application listening at http://localhost:${port}`));

/**
 * REDIS
 */
export const redisInstance: RedisClient = require("redis").createClient({
    host: process.env.REDIS_HOST,
    port: process.env.REDIS_PORT,
    password: process.env.REDIS_PASSWORD,
});

redisInstance.on("connect", () => {
    console.log("[Redis]", "Connection to database established");
});

redisInstance.on("error", (err: Error) => {
    console.log("[Redis]", "Error with redis: " + err.message);
});

app.use(errorHandler);

/**
 * MARKDOWN
 */

//highlightAll();
export const markdownParser = markdownit({
    highlight: function (str, lang) {
        if (lang && getLanguage(lang)) {
            try {
                return highlight(lang, str).value;
            } catch (__) {}
        }

        return "";
    },
    html: true,
    typographer: true,
    linkify: true,
});
