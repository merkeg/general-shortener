# general-shortener

This application is to shorten links, text (with markdown support) and files into short and easy to read `slugs`
Configuration will be set with environment variables:

### Adding new content

There will be an open endpoint called `%BASE_URL%/new`
With an standard post request you can add new content (More about it on %BASE_URL%/docs)

The body of the post request looks like this (json and form-data are accepted):

```json
{
	"password": "%AUTHENTICATION_PASSWORD%",
	"type": "file | url | text",
	"value": "for url and text",
	"file": "Sent via form-data for type file"
}
```

You can delete an entry with an delete request to the specified slug you want to delete. `DELETE: %BASE_URL%/:slug` or with the given deletion url

### Environment variables

| Variable                | Default value | Description                                          | Optional |
| ----------------------- | ------------- | ---------------------------------------------------- | -------- |
| SERVER_PORT             | 5000          |                                                      | y        |
| SERVER_BASE_URL         |               | The base url the server is running on                | n        |
|                         |               |                                                      |          |
| AUTHENTICATION_PASSWORD |               | The password to upload new entries                   | n        |
|                         |               |                                                      |          |
| REDIS_HOST              |               |                                                      | n        |
| REDIS_PORT              |               |                                                      | n        |
| REDIS_PASSWORD          |               |                                                      | n        |
|                         |               |                                                      |          |
| STORAGE_DRIVER          |               | currently, only `local` is supported.                | n        |
|                         |               |                                                      |          |
| STORAGE_LOCAL_DIR       |               | Location of the directory to save the uploaded files | n\*      |
|                         |               |                                                      |          |
| HTTP_404_REDIRECT       |               | Set to redirect if slug not found                    | y        |
| HTTP_BASE_REDIRECT      |               | Set to redirect on url base `/`                      | y        |
| HTTP_DELETION_REDIRECT  |               | Set to redirect on entry deletion                    | y        |

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
  general-shortener:
    image: ghcr.io/merkeg/general-shortener/general-shortener:{VERSION}
    restart: always
    environment:
      - SERVER_BASE_URL=https://example.com
      - AUTHENTICATION_PASSWORD=PASSWORD
      - REDIS_HOST=redis
      - REDIS_PORT=6379
      - REDIS_PASSWORD=redis
      - STORAGE_DRIVER=local
      - STORAGE_LOCAL_DIR=/screenshots
    depends_on:
      - redis
volumes:
  redis:
```

### Endpoints

| Endpoint               | Method | Description                                           | Data                             | Protected |
| ---------------------- | ------ | ----------------------------------------------------- | -------------------------------- | --------- |
| `/new`                 | POST   | Create a new entry                                    | `type`, `value`, `slug?`         | Yes       |
| `/:slug`               | GET    | Get the entry                                         |                                  |           |
|                        |        |                                                       |                                  |           |
| `/:slug`               | DELETE | Deletes an entry                                      |                                  | Yes       |
| `/:slug/:deletionCode` | GET    | Deletes an entry withoud password and special request |                                  |           |
|                        |        |                                                       |                                  |           |
| `/:slug/info`          | GET    | Information about the entry                           |                                  | Yes       |
|                        |        |                                                       |                                  |           |
| `/list`                | GET    |                                                       | `offest?`, `amount?`, `pattern?` | Yes       |

Protected endpoints need a `password` set in either query, body or bearer.
