# Copilot instructions for EventDressRental

## Summary of the app
EventDressRental is a REST API backend for managing dress rentals (users, models, dresses, orders, and categories). It uses a three-layer architecture (Web API → Services → Repositories) with DTOs to isolate API contracts from EF Core entities.

## Tech stack
- C# / .NET 9 (ASP.NET Core Web API)
- Entity Framework Core 9 (SQL Server, database-first)
- AutoMapper for DTO ↔ entity mapping
- NLog for logging (file + email target)
- OpenAPI/Swagger via `Microsoft.AspNetCore.OpenApi` + `Swashbuckle.AspNetCore`
- xUnit + Moq (+ Moq.EntityFrameworkCore) for tests

## Project structure
- `WebApiShop/` – ASP.NET Core API project (controllers, middleware, DI, appsettings)
- `Services/` – business logic + service interfaces
- `Repositories/` – data access via EF Core `EventDressRentalContext`
- `Entities/` – EF Core entities (database-first; some files are auto-generated)
- `DTOs/` – request/response contracts (records)
- `Tests/` – unit tests for services and repositories

## Coding guidelines
- Prefer async APIs (`async/await`) everywhere, from controller → service → repository.
- Follow naming rules from `WebApiShop/.editorconfig`:
  - Types/methods: PascalCase
  - Parameters: camelCase
  - Private fields: `_camelCase`
- Keep controller actions thin; put logic in services.
- Add new data access methods to interface + implementation together.
- For new DTOs, update `Services/AutoMapping.cs` mappings.
- Avoid editing EF Core auto-generated files unless you’re regenerating from DB.
- Use `CreatedAtAction` for POSTs that create resources.

## Configuration and tooling
- Runtime config lives in `WebApiShop/appsettings*.json`.
- `WebApiShop/appsettings.json` contains base logging/host settings; connection strings live in `appsettings.Development.json`.
- Connection string key in use: `ConnectionStrings:Home` (SQL Server + Integrated Security).
- Local dev uses `ASPNETCORE_ENVIRONMENT=Development` (see `launchSettings.json`).

## Common workflows
### Build
```powershell
cd c:\Users\user1\Desktop\API

dotnet build .\EventDressRental.sln
```

### Run API
```powershell
cd c:\Users\user1\Desktop\API

dotnet run --project .\WebApiShop\EventDressRental.csproj
```

### Tests
```powershell
cd c:\Users\user1\Desktop\API

dotnet test .\Tests\Tests.csproj
```

## Existing resources
- Solution file: `EventDressRental.sln`
- Middleware: `WebApiShop/Middleware` (error handling + request rating)
- Logging: `WebApiShop/nlog.config`
- Repository layer instructions: `.github/repository-layer-instructions.md`

## Tips to avoid wasted time
- Ensure SQL Server and the `Event_dress_rental` database exist locally, or update `appsettings.Development.json` before running.
- Tests mock EF Core with Moq.EntityFrameworkCore; prefer that pattern for repository unit tests.
- If Swagger isn’t available, confirm `ASPNETCORE_ENVIRONMENT=Development`.
