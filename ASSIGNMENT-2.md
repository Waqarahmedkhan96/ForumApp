# Assignment 2 – Command Line Interface

## What was added
- Server/CLI console app
- DI root in Program.cs
- UI in CLI/UI/CliApp.cs (async menu: users, posts, comments)
- Refs to Entities, RepositoryContracts, InMemoryRepositories

## How to run
dotnet run --project "Server/CLI"

## Notes
- Repositories created once in Program.cs and injected into UI
- Async all the way; minimal validation; easy to extend
