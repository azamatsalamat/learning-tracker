# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

LearningTracker is an ASP.NET Core Web API (.NET 8.0) application for tracking learning progress and managing professional profiles. The application features resume/CV parsing capabilities and uses PostgreSQL for data persistence.

## Technology Stack

- **.NET 8.0** with C# (nullable reference types enabled)
- **ASP.NET Core Web API** with controllers
- **Entity Framework Core 9.0** with PostgreSQL (Npgsql)
- **MediatR** for CQRS pattern implementation
- **FluentValidation** for request validation
- **CSharpFunctionalExtensions** for Result pattern
- **JWT Bearer Authentication** for auth
- **iText7** for PDF text extraction
- **Swagger/OpenAPI** for API documentation

## Development Commands

### Build and Run
```bash
# Build the solution
dotnet build

# Run the API (from solution root)
dotnet run --project LearningTracker.Api

# Watch mode (hot reload)
dotnet watch --project LearningTracker.Api
```

### Database
```bash
# Create a new migration
dotnet ef migrations add <MigrationName> --project LearningTracker.Api

# Apply migrations
dotnet ef database update --project LearningTracker.Api

# Remove last migration
dotnet ef migrations remove --project LearningTracker.Api
```

### Testing
```bash
# Run all tests (once test project is added)
dotnet test
```

## Architecture

### Vertical Slice Architecture

The codebase follows **Vertical Slice Architecture** where features are organized by business capability rather than technical layers. Each feature contains all layers (API, business logic, data access) within its folder.

**Structure:**
- `Features/` - Feature slices (e.g., Users, Profiles)
  - Each feature contains its own controllers, commands/queries, entities, value objects, and enums
  - Features are self-contained and independent

**Key Pattern:**
Each feature use case is typically structured as:
```
FeatureName/
├── FeatureController.cs        # API endpoints
├── SomeCommand.cs              # MediatR command/query with:
│   ├── Command/Query record    # Request DTO
│   ├── Validator class         # FluentValidation rules
│   └── Handler class           # Business logic
├── Entities/                   # Domain entities for this feature
├── ValueObjects/               # Value objects
└── Enums/                      # Enums specific to feature
```

### CQRS with MediatR

All business operations use MediatR commands and queries:
- Commands/queries are defined as static classes with nested types (Command, Validator, Handler)
- Each handler implements `IRequestHandler<TRequest, TResponse>`
- All handlers return `Result` or `Result<T>` from CSharpFunctionalExtensions

Example pattern (see `Features/Users/Register.cs`):
```csharp
public static class OperationName
{
    public record Command(...) : IRequest<Result<T>>;

    public class Validator : AbstractValidator<Command> { }

    internal sealed class Handler : IRequestHandler<Command, Result<T>> { }
}
```

### Pipeline Behaviors

`Behaviors/ValidationBehavior.cs` - MediatR pipeline behavior that automatically validates all requests using FluentValidation before handlers execute. Validation failures are returned as `Result.Failure`.

### Shared Infrastructure

- `Shared/LearningTrackerControllerBase.cs` - Base controller with `HandleResult()` methods to convert `Result`/`Result<T>` to appropriate HTTP responses
- `Services/` - Cross-cutting services:
  - `TokenProvider` - JWT token generation
  - `TextExtractorService` - File text extraction with strategy pattern
  - `TextExtractors/PdfTextExtractor` - PDF-specific implementation
- `Database/LearningTrackerDbContext.cs` - EF Core context that auto-applies entity configurations from assembly
- `Database/EntityConfigurations/` - EF Core IEntityTypeConfiguration implementations
- `Configurations/` - Configuration POCOs (e.g., AuthOptions)

### Domain Concepts

Domain entities and value objects use namespaces like `LearningTracker.Domain.Entities` and `LearningTracker.Domain.ValueObjects` but physically reside within their feature folders. This provides logical domain grouping while maintaining vertical slice organization.

Profile entity is central to the resume parsing feature - it contains structured CV/resume data including experiences, education, certifications, projects, awards, publications, skills, and languages.

## Configuration

Copy `appsettings.example.json` to `appsettings.Development.json` and configure:
- `Auth:Key` - JWT signing key (required for authentication)
- `Auth:Issuer` and `Auth:Audience` - JWT token validation
- `Auth:AccessTokenLifetime` - Token expiration (format: HH:MM:SS)
- `ConnectionStrings:DefaultConnection` - PostgreSQL connection string

## Current Development Focus

The repository is on branch `feature/cv-parsing` implementing resume/CV parsing functionality. The `ParseResume` command handler in `Features/Profiles/ParseResume.cs` is not yet implemented (throws NotImplementedException).

## Conventions

- Use **Result pattern** - all command/query handlers return `Result` or `Result<T>`, never throw exceptions for business logic failures
- Use **sealed handlers** - mark handler classes as `sealed` for performance
- Use **protected setters** on entities - domain entities use `protected set` for properties and require constructor initialization
- Use **nullable reference types** - properly annotate nullability (project has `<Nullable>enable</Nullable>`)
- Use **array types** for collections in entities (e.g., `string[] Skills`) rather than List<T>
- Entity configurations belong in `Database/EntityConfigurations/` and implement `IEntityTypeConfiguration<T>`
