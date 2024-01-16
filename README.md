# ASP.NET CRUD APIs - Automate the boring stuff with sql2crud

## sql2crud pipeline
1. read metadata for all schemas & tables from your DB 
2. generate C# entities & DTOs for each table
3. generate application service interfaces with the usual CRUD operations
4. generate controllers with the usual CRUD endpoints
5. generate create table sql scripts

* all read/write configurations are handled by config.json (an example config.json can be found in the repo)

Ex. run cmd: `dotnet run "full_path_to_config.json"`
