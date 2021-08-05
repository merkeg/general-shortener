# general-shortener (Work in progress)
![Integration testing badge](https://github.com/merkeg/general-shortener/actions/workflows/dotnet_tests.yml/badge.svg)

This application is to shorten links, text (with markdown support) and files into short and easy to read slugs in which you can share it with your friends.



## Environment variables

| Environment variable      | Type   | Description                                  | Required |
|---------------------------|--------|----------------------------------------------|----------|
| MasterToken               | String | Mastertoken with full access to the api      | true     |
|                           |        |                                              |          |
| BaseUrl                   | String | Base Url                                     | true     |
|                           |        |                                              |          | 
| Storage__Path             | String | Path of where the files will be saved to     | true     | 
|                           |        |                                              |          | 
| MongoDB__ConnectionString | String | Connection string to connect to the database | true     |
| MongoDB__DatabaseName     | String | Name of the database in MongoDB              | false    | 
|                           |        |                                              |          | 
| HTTP__Redirect__NotFound  | String | Redirect on 404                              | false    |
| HTTP__Redirect__Root      | String | Redirect at Root                             | false    |
| HTTP__Redirect__Deletion  | String | Redirect at Deletion                         | false    |
Note: These are double underscores