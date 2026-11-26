# Facility API

ASP.NET Core service for clinic offices, healthcare services, and medical specializations. It stores them in PostgreSQL.

Persistence uses Dapper. Each entity declares its own id. Each entity has its own repository. The service is implemented.

Target framework: **.NET 10** (`net10.0`). Nullable reference types and implicit usings are enabled in every project.

## Solution layout

```
FacilityApi.slnx
FacilityApi.Api/              HTTP host
FacilityApi.Application/      use cases, DTOs, validation
FacilityApi.Domain/           entities and persistence contracts
FacilityApi.Infrastructure/   Dapper, PostgreSQL, repositories, SQL scripts
```

`FacilityApi.Domain`, `FacilityApi.Application`, and `FacilityApi.Infrastructure` are class libraries. Do not set `OutputType` to `Exe`. The API project is the only executable.

Dependency direction:

```
Api ──► Application ──► Domain
 │
 └──► Infrastructure ──► Domain
```

Application does not reference Infrastructure. Infrastructure does not reference Application. Domain and Application do not reference Dapper or Npgsql.

## How this service differs from Profiles API

- PostgreSQL access goes through Dapper and Npgsql. The schema lives in SQL scripts under Infrastructure. There is no DbContext, no EF configuration, no interceptor, and no EF migration.
- `OfficeEntity`, `ServiceEntity`, and `SpecializationEntity` each declare `Id`. There is no `BaseEntity` and no `SoftDeletableEntity`. These tables have no audit columns.
- `IOfficeRepository`, `IServiceRepository`, and `ISpecializationRepository` are separate interfaces. `OfficeRepository`, `ServiceRepository`, and `SpecializationRepository` are separate classes. They do not inherit a shared repository type. There is no unit of work and no `SaveChanges`.
- Facility does not call the Auth API and does not register users. It validates the same Keycloak JWT that Profiles uses.

## Projects

### FacilityApi.Domain

Business model and persistence contracts. No reference to the other projects. No NuGet packages.

| Folder | Contents |
| --- | --- |
| `Entities` | `OfficeEntity`, `ServiceEntity`, `SpecializationEntity` |
| `Exceptions` | `AppException`, `ConflictException`, `InvalidFormatException`, `ServerException` |
| `Interfaces` | `IOfficeRepository`, `IServiceRepository`, `ISpecializationRepository`, `IDbTransactionScope`. Repository operations match Profiles `IGenericRepository` and are declared separately for each entity |

Entities do not inherit a base type, and they do not keep navigation properties.

`OfficeEntity` has `Id` and `Title`. `SpecializationEntity` has `Id`, `Title`, and `Description`. `ServiceEntity` has `Id`, `Title`, `Description`, `Price` (`decimal`), `IsActive`, and `DoctorId`. `Id` and `DoctorId` are `Guid`. `DoctorId` is the Profiles `Doctors.Id`, not the account id. This database has no foreign key to that table.

Profiles stores `OfficeId` on admin, receptionist, and doctor, and `SpecializationId` on doctor. Those rows stay in Profiles.

Each repository interface repeats the Profiles `IGenericRepository` operations for its own entity. `CountAsync`, `GetAsync`, and `GetAllAsync` take an `Expression` predicate. Mutations are `AddAsync`, `AddRangeAsync`, `UpdateAsync`, `UpdateRangeAsync`, `RemoveAsync`, and `RemoveRangeAsync`. The interfaces do not share a base interface.

### FacilityApi.Application

Use cases and the types controllers exchange with clients. References Domain only.

| Folder | Contents |
| --- | --- |
| `Services` | `OfficeService`, `ServiceService`, `SpecializationService` |
| `Interfaces` | `IOfficeService`, `IServiceService`, `ISpecializationService` |
| `DTOs` | Create, update, filter, and response records, plus `PaginationParamsDto` (`PageNumber` default 0, `PageSize` default 10) |
| `Validators` | FluentValidation validators, including pagination rules |
| `Mappings` | Entity ↔ DTO extension methods |
| `Helpers` | `PaginationHelper` |

`DependencyInjection.AddApplicationServices` registers validators from this assembly, SharpGrip automatic MVC validation, and the scoped services.

The application service assigns `Guid.NewGuid()` before insert. A missing office, service, or specialization throws `KeyNotFoundException`. Delete removes the row.

Update, delete, and the range forms of those operations call the repository twice (load, then write), so they call `IDbTransactionScope.BeginAsync`. The method commits after both calls succeed and rolls back when one throws. Create and create-range call the repository once and do not open a transaction. There is no unit of work. Repositories reuse the open transaction connection instead of opening a second one.

Write methods catch unexpected exceptions, roll back when a transaction is open, and throw `ServerException`. `KeyNotFoundException` is thrown in the `try` block and is not caught. `AppException` is not caught either.

There is no password hasher and no identity client.

### FacilityApi.Infrastructure

PostgreSQL persistence. References Domain only.

| Folder | Contents |
| --- | --- |
| `Data` | `DatabaseOptions` bound from `ConnectionStrings`, and `NpgsqlConnectionFactory` |
| `Repositories` | `OfficeRepository`, `ServiceRepository`, `SpecializationRepository`, and `SqlPredicateBuilder` |
| `Scripts` | `001_init.sql` |

`NpgsqlConnectionFactory` creates and opens an `NpgsqlConnection`. It stays in Infrastructure. Domain interfaces do not mention connections.

Each repository owns its SQL text. `SqlPredicateBuilder` turns the repository `Expression` into a `WHERE` clause. It supports `string.ToLower()` and `string.Contains()` (`ILIKE`). Each method opens its own connection, runs the command, and disposes the connection. Insert, update, and delete run immediately. Repositories do not share a base class.

`DependencyInjection.AddInfrastructure` requires `ConnectionStrings:FacilityDb` and registers the connection factory and the three repositories.

`001_init.sql` creates:

| Table | Columns |
| --- | --- |
| `Services` | `Id uuid` primary key, `Title text` not null, `Description text` not null, `Price numeric` not null, `IsActive boolean` not null default true, `DoctorId uuid` not null |
| `Specializations` | `Id uuid` primary key, `Title text` not null, `Description text` not null |
| `Offices` | `Id uuid` primary key, `Title text` not null |

Column names match the C# property names (`Id`, `Title`, `Description`, `Price`, `IsActive`, `DoctorId`).

### FacilityApi.Api

The executable web project. References Application and Infrastructure.

| Folder | Contents |
| --- | --- |
| `Controllers` | `OfficeController`, `ServiceController`, `SpecializationController`. Routes are `/{controller}` |
| `Authorization` | Named role policies in `Policies`. Fallback policy requires an authenticated user |
| `Extensions` | Service registration, Keycloak JWT authentication, Swagger |
| `Middlewares` | `GlobalExceptionHandler` (`application/problem+json`) |
| `Options` | `KeycloakOptions` |

Each controller exposes get by id, paged list, create, update, delete, and the batch forms of create, update, and delete. List and get are anonymous. Create, update, delete, and batch actions require the `Admin` role. List actions take the entity filter and `PaginationParamsDto` from the query string.

Authentication is JWT Bearer against the Keycloak realm. The access token is read from the `accessToken` cookie when that cookie is present, otherwise from the `Authorization` header. Realm roles are copied onto `role` claims. Audience check accepts `aud` or `azp` equal to `Keycloak:Audience`. Role names match Profiles: `Admin`, `Receptionist`, `Doctor`, `Patient`.

Swagger UI is enabled in Development at `/swagger`. HTTPS redirection runs outside Development.

Serilog writes to the console and to MongoDB through the `MongoDBBson` sink. The database is `logs` and the collection is `facility`. Startup is Information. Each call writes a Serilog request line with method, path, status, and elapsed time, plus one HTTP log with headers, query, duration, and the request and response bodies. Authorization and cookie headers are redacted. Each public service method logs its outcome. `GlobalExceptionHandler` writes warnings and errors to the same collection. Compose sets `Serilog__WriteTo__0__Args__databaseUrl` from `x-mongo-logs-url`, using `MONGO_USER`, `MONGO_PASSWORD`, and `MONGO_DB`.

`GlobalExceptionHandler` maps argument and format errors and `ConflictException` to 400, `UnauthorizedAccessException` to 401, `KeyNotFoundException` to 404, and anything else to 500. 500 responses hide the exception message.

## NuGet packages

Use these versions. Shared packages match Profiles API.

| Project | Package | Version |
| --- | --- | --- |
| Application | FluentValidation | 12.2.0 |
| Application | FluentValidation.DependencyInjectionExtensions | 12.2.0 |
| Application | Microsoft.Extensions.Logging.Abstractions | 11.0.0 |
| Application | SharpGrip.FluentValidation.AutoValidation.Mvc | 2.0.0 |
| Infrastructure | Dapper | 2.1.89 |
| Infrastructure | Npgsql | 10.0.3 |
| Infrastructure | Microsoft.Extensions.Configuration.Abstractions | 10.0.14 |
| Infrastructure | Microsoft.Extensions.Configuration.UserSecrets | 10.0.14 |
| Infrastructure | Microsoft.Extensions.Options.ConfigurationExtensions | 10.0.14 |
| Api | Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.14 |
| Api | Microsoft.AspNetCore.OpenApi | 10.0.12 |
| Api | Serilog.AspNetCore | 11.0.0 |
| Api | Serilog.Sinks.MongoDB | 8.0.0 |
| Api | Swashbuckle.AspNetCore | 10.2.3 |

Domain has no package references.

## Configuration

`FacilityApi.Api/appsettings.json` holds non-secret defaults:

| Key | Purpose |
| --- | --- |
| `Keycloak:BaseUrl` | Keycloak base URL. Default `http://localhost:8080` |
| `Keycloak:Realm` | Realm used as the JWT authority. `appsettings.json` defaults to `myrealm`. Compose uses `KEYCLOAK_REALM` (`inno-realm` in `.env.example`) |
| `Keycloak:Audience` | Expected token audience / authorized party. Default `dotnet-auth-service` |
| `Keycloak:RequireHttpsMetadata` | JWT metadata HTTPS requirement. Default `false` for local HTTP |

`ConnectionStrings:FacilityDb` is required at startup and is not in `appsettings.json`. Set it with user secrets on the API project (the process loads that project's secrets):

```powershell
dotnet user-secrets set "ConnectionStrings:FacilityDb" "Host=localhost;Port=5434;Database=facility;Username=postgres;Password=postgres" --project FacilityApi.Api
```

Port `5434` is the Compose host port for `facility-db`. Port `5432` is SonarQube. The database name and password are `FACILITY_DB`, `FACILITY_DB_USER`, and `FACILITY_DB_PASSWORD` in `InnoInfrastructure/.env`. The API user-secrets id is `a27f01fb-1d5b-45e7-be2c-d9161fd0afa8`.

## Setup and commands

From this directory (`FacilityApi`):

```powershell
dotnet restore FacilityApi.slnx
dotnet build FacilityApi.slnx
dotnet run --project FacilityApi.Api --launch-profile http
```

The `http` profile listens on `http://localhost:5035`. Swagger: `http://localhost:5035/swagger`. The `https` profile also listens on `https://localhost:7224`.

Apply the schema with `psql` (there is no `dotnet ef` command in this service):

```powershell
psql "host=localhost port=5434 dbname=facility user=postgres" -f FacilityApi.Infrastructure/Scripts/001_init.sql
```

Compose runs that script only when the `facility-db` volume is empty. This repository has a `Dockerfile` and no Compose file. Start the shared stack from `InnoInfrastructure`.

Keycloak realm roles must match the Profiles role names: `Admin`, `Receptionist`, `Doctor`, `Patient`. `DoctorId` on a service is the Profiles doctor profile id, not the account id.
