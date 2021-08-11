# general-shortener
![Integration testing badge](https://github.com/merkeg/general-shortener/actions/workflows/dotnet_tests.yml/badge.svg)

An application to digest your data like url's, files or texts and saves it on disk or on database and gives you a shareable url to access the data.

## Usage

### Basic usage
To add new entries to your server, you always need to make a `POST` request to the `{base_url}/entries` endpoint.
- To access it you must add your token you set in the Headers `Authorization: Bearer {token}`
- The content of your body **must** be of type `multipart/form-data`
- If you want to send a file, the name of it in the form (not the file name) **must** be `file`

There are three things you can set in the form
- `type`: The type of the data you want to send and the server to parse (Either `file`, `text` or `url`). Required
- `value`: The data you want to send. It is required for the types `text` and `url`, but disallowed to set on type `file`
- `slug`: your custom identifier of the resource (or data), it is fully optional. If an entry with the specified slug already exists, it will be overwritten

> If you need more information about the currently existing endpoints and their request parameters, you can open the api documentation located at `{base_url}/docs`
### Using it with with [ShareX](https://getsharex.com) (recommended)
[Imgur Gallery demonstratring the options that need to be set](https://imgur.com/a/qgQJyeS)


## Installation

### Requirements
- Docker to run the application on
- A MongoDB instance, either hosted in docker as well or accessible from the application

### Docker stack
```
version: '3.1'
services:
  mongo:
    image: mongo
    restart: always
    environment:
      MONGO_INITDB_ROOT_USERNAME: mongo_username
      MONGO_INITDB_ROOT_PASSWORD: mongo_password
  general-shortener:
    image: ghcr.io/merkeg/general-shortener:{tag} # Either 'latest' or a specific tag
    restart: always
    ports:
      - 80:80 
    environment:
      MasterToken: {master_token} # Your own master token to access the endpoints with
      BaseUrl: {http://example.domain} # The base url in which the server resolves newly created entries
      Storage__Path: {path} # Storage location for files, use absolute paths please
      MongoDB__ConnectionString: mongodb://mongo_username:mongo_password@mongo:27017/
      MongoDB__DatabaseName: general_shortener
    
```

## Migration
### Data migration from redis model (below version 1)
Populate all of the `Transfer__*` environment variables to the redis database that hosted your data, start the container and let it run for a short time. It will inform you about the state of the migration.

When the migration is done, stop the server and remove the `Transfer__*` environment variables as they are not needed anymore

## Environment variables

| Environment variable      | Type    | Description                                         | Required |
|---------------------------|---------|-----------------------------------------------------|----------|
| MasterToken               | String  | Mastertoken with full access to the underlying api             | true     |
|                           |         |                                                     |          |
| BaseUrl                   | String  | Force base url or use the host the request was sent                                            | false     |
|                           |         |                                                     |          |
| Storage__Path             | String  | Path of where the files will be saved to            | true     |
|                           |         |                                                     |          |
| MongoDB__ConnectionString | String  | Connection string to connect to the database server | true     |
| MongoDB__DatabaseName     | String  | Name of the database in MongoDB                     | false    |
|                           |         |                                                     |          |
| HTTP__Redirect__NotFound  | String  | Redirect on 404                                     | false    |
| HTTP__Redirect__Root      | String  | Redirect at Root                                    | false    |
| HTTP__Redirect__Deletion  | String  | Redirect at Deletion                                | false    |
|                           |         |                                                     |          |
| Transfer__Transfer        | Boolean | Transfer data from old database to new database     | false    |
| Transfer__Redis__Host     | String  | Host address of the redis server                    | false    |
| Transfer__Redis__Port     | Integer | Port of the redis server                            | false    |
| Transfer__Redis__Password | String  | Password to connect to the redis server             | false    |


Note: These are double underscores