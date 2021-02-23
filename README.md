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

### Example configuration

```
version: '3.0'
services:
  redis:
    image: redis
    restart: always
    volumes:
      - "redis:/data"
    command: redis-server --requirepass redis
  s3:
    image: minio/minio:latest
    restart: always
    volumes:
      - "s3:/data"
    environment:
      - MINIO_ROOT_USER=minio
      - MINIO_ROOT_PASSWORD=miniopassword
    command: server /data
  general-shortener:
    image: docker.pkg.github.com/merkeg/merkeg-general-storage/merkeg-general-storage:0.0.1.1
    restart: always
    environment:
      - SERVER_BASE_URL=https://example.com
      - AUTHENTICATION_PASSWORD=PASSWORD
      - REDIS_HOST=redis
      - REDIS_PORT=6379
      - REDIS_PASSWORD=redis
      - S3_ACCESS_KEY=minio
      - S3_SECRET_KEY=miniopassword
      - S3_ENDPOINT=http://s3:9000
      - S3_BUCKET=slugs
    depends_on:
      - redis
      - s3
volumes:
  redis:
  s3:
```
