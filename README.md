# general-shortener

This application is to shorten links, texts (Markdown supported) and files into short and easy to read `slugs`
This image doesn't need any persistent storage. Everything is saved on s3 and redis. Configuration will be set with environment variables

### Environment variables

| Variable                | Default value | Description                           | Optional |
| ----------------------- | ------------- | ------------------------------------- | -------- |
| SERVER_PORT             | 5000          |                                       | y        |
| SERVER_BASE_URL         |               | The base url the server is running on | n        |
|                         |               |                                       |          |
| AUTHENTICATION_PASSWORD |               | The password to upload new things     | n        |
|                         |               |                                       |          |
| REDIS_HOST              |               |                                       | n        |
| REDIS_PORT              |               |                                       | n        |
| REDIS_PASSWORD          |               |                                       | n        |
|                         |               |                                       |          |
| S3_ACCESS_KEY           |               |                                       | n        |
| S3_SECRET_KEY           |               |                                       | n        |
| S3_ENDPOINT             |               | The endpoint s3 is running on         | n        |
| S3_BUCKET               |               | the bucket to save files on           | n        |
