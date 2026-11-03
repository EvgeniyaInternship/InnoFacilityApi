# Facility API

ASP.NET Core service for clinic offices, healthcare services, and medical specializations. It stores them in PostgreSQL.

This document is the structure to implement. It follows Profiles API. Persistence uses Dapper. Each entity declares its own id. Each entity has its own repository.

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
| `Interfaces` | `IOfficeRepository`, `IServiceRepository`, `ISpecializationRepository`. Same operations as Profiles `IGenericRepository`, declared separately for each entity |

Remove the current `BaseEntity`. Entities do not inherit it, and they do not keep navigation properties.

`OfficeEntity` has `Id`, `Title`, and `ServiceId`. `SpecializationEntity` has `Id` and `Title`. `ServiceEntity` has `Id`, `Price` (`decimal`), and `IsActive`. `Id` and `ServiceId` are `Guid`, the same type Profiles already stores on `OfficeId` and `SpecializationId`.

`ServiceId` on an office is the only foreign key inside this database. Profiles stores `OfficeId` on admin, receptionist, and doctor, and `SpecializationId` on doctor. Profiles doctor and the appointments service store `ServiceId`. Those rows stay in the other databases. Facility does not store their ids.

Each repository interface repeats the Profiles `IGenericRepository` operations for its own entity: `CountAsync`, `GetAsync`, and `GetAllAsync` take an `Expression` predicate, and `GetAsync` and `GetAllAsync` take `isTracked`. Mutations are `Add`, `AddRange`, `Update`, `UpdateRange`, `Remove`, and `RemoveRange`. The interfaces do not share a base interface.

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

The application service assigns `Guid.NewGuid()` before insert. Creating or updating an office whose `ServiceId` does not exist throws `KeyNotFoundException`. Deleting a service that still has offices throws `ConflictException`. `IsActive` is a column on the service. Delete removes the row.

There is no password hasher and no identity client.

### FacilityApi.Infrastructure

PostgreSQL persistence. References Domain only.

| Folder | Contents |
| --- | --- |
| `Data` | `DatabaseOptions` bound from `ConnectionStrings`, and `NpgsqlConnectionFactory` |
| `Repositories` | `OfficeRepository`, `ServiceRepository`, `SpecializationRepository` |
| `Scripts` | `001_init.sql` |

`NpgsqlConnectionFactory` creates and opens an `NpgsqlConnection`. It stays in Infrastructure. Domain interfaces do not mention connections.

Each repository owns its SQL text. Each method opens its own connection, runs the command, and disposes the connection. Insert, update, and delete run immediately. Repositories do not share a base class.

`DependencyInjection.AddInfrastructure` requires `ConnectionStrings:FacilityDb` and registers the connection factory and the three repositories.

`001_init.sql` creates:

| Table | Columns |
| --- | --- |
| `Services` | `Id uuid` primary key, `Price numeric` not null, `IsActive boolean` not null default true |
| `Specializations` | `Id uuid` primary key, `Title text` not null |
| `Offices` | `Id uuid` primary key, `Title text` not null, `ServiceId uuid` not null references `Services(Id)` on delete restrict, index on `ServiceId` |

Column names match the C# property names, same as the Profiles tables (`Id`, `Title`, `ServiceId`, `Price`, `IsActive`).

### FacilityApi.Api

The executable web project. References Application and Infrastructure.

| Folder | Contents |
| --- | --- |
| `Controllers` | `OfficeController`, `ServiceController`, `SpecializationController`. Routes are `/{controller}` |
| `Authorization` | Named role policies in `Policies`. Fallback policy requires an authenticated user |
| `Extensions` | Service registration, Keycloak JWT authentication, Swagger |
| `Middlewares` | `GlobalExceptionHandler` (`application/problem+json`) |
| `Options` | `KeycloakOptions` |

Each controller exposes get by id, paged list, create, update, and delete. List and get require an authenticated user. Create, update, and delete require the `Admin` role. List actions take the entity filter and `PaginationParamsDto` from the query string.

Authentication is JWT Bearer against the Keycloak realm. The access token is read from the `accessToken` cookie when that cookie is present, otherwise from the `Authorization` header. Realm roles are copied onto `role` claims. Audience check accepts `aud` or `azp` equal to `Keycloak:Audience`. Role names match Profiles: `Admin`, `Receptionist`, `Doctor`, `Patient`.

Swagger UI is enabled in Development at `/swagger`. HTTPS redirection runs outside Development.

`GlobalExceptionHandler` maps argument and format errors and `ConflictException` to 400, `UnauthorizedAccessException` to 401, `KeyNotFoundException` to 404, and anything else to 500. 500 responses hide the exception message.

## NuGet packages

Use these versions. Shared packages match Profiles API.

| Project | Package | Version |
| --- | --- | --- |
| Application | FluentValidation | 12.2.0 |
| Application | FluentValidation.DependencyInjectionExtensions | 12.2.0 |
| Application | SharpGrip.FluentValidation.AutoValidation.Mvc | 2.0.0 |
| Infrastructure | Dapper | 2.1.89 |
| Infrastructure | Npgsql | 10.0.3 |
| Infrastructure | Microsoft.Extensions.Configuration.Abstractions | 10.0.14 |
| Infrastructure | Microsoft.Extensions.Configuration.UserSecrets | 10.0.14 |
| Infrastructure | Microsoft.Extensions.Options.ConfigurationExtensions | 10.0.14 |
| Api | Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.14 |
| Api | Microsoft.AspNetCore.OpenApi | 10.0.12 |
| Api | Swashbuckle.AspNetCore | 10.2.3 |

Domain has no package references.

## Configuration

`FacilityApi.Api/appsettings.json` holds non-secret defaults:

| Key | Purpose |
| --- | --- |
| `Keycloak:BaseUrl` | Keycloak base URL. Default `http://localhost:8080` |
| `Keycloak:Realm` | Realm used as the JWT authority. Default `myrealm` |
| `Keycloak:Audience` | Expected token audience / authorized party. Default `dotnet-auth-service` |
| `Keycloak:RequireHttpsMetadata` | JWT metadata HTTPS requirement. Default `false` for local HTTP |

`ConnectionStrings:FacilityDb` is required at startup and is not in `appsettings.json`. Set it with user secrets on the API project (the process loads that project's secrets):

```powershell
dotnet user-secrets set "ConnectionStrings:FacilityDb" "Host=localhost;Port=5432;Database=facility;Username=postgres;Password=postgres" --project FacilityApi.Api
```

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
psql "host=localhost port=5432 dbname=facility user=postgres" -f FacilityApi.Infrastructure/Scripts/001_init.sql
```

Keycloak realm roles must match the Profiles role names: `Admin`, `Receptionist`, `Doctor`, `Patient`.
