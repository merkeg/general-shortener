# general-shortener (Work in progress)
![Integration testing badge](https://github.com/merkeg/general-shortener/actions/workflows/dotnet_tests.yml/badge.svg)

An application to shorten url's (redirects), files and text into short and easy to read links with a customized domain.



## Environment variables

| Environment variable      | Type    | Description                                         | Required |
|---------------------------|---------|-----------------------------------------------------|----------|
| MasterToken               | String  | Mastertoken with full access to the api             | true     |
|                           |         |                                                     |          |
| BaseUrl                   | String  | Base Url                                            | true     |
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

## Migration
### Data migration from redis model (>v1)
Populate all of the `Transfer__*` environment variables to the redis database that hosted your data, start the container and let it run for a short time. It will inform you about the state of the migration.

When the migration is done, stop the server and remove the `Transfer__*` environment variables as they are not needed anymore