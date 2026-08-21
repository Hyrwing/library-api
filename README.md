# Library API

A clean architecture CRUD API for managing books and loans, built with .NET 10.

## Architecture

```
Domain → Application → Infrastructure → Api
```

- **Domain**: Entities (Book, Loan) with business rules and domain exceptions
- **Application**: Services, DTOs, FluentValidation validators, repository interfaces
- **Infrastructure**: EF Core (SQLite), repository implementations, 100-book seed data
- **Api**: Minimal API endpoints, validation filter, global error handling, correlation IDs

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Getting Started

```bash
# Clone the repository
git clone <your-repo-url>
cd LibraryApi

# Run the API (auto-applies migrations and seeds 100 books)
dotnet run --project src/LibraryApi.Api

# Swagger UI available at the URL shown in the console output, e.g.:
# http://localhost:5233/swagger
```

## Running Tests

```bash
# All tests (40 total: 25 unit + 15 integration)
dotnet test

# Unit tests only
dotnet test tests/LibraryApi.Domain.Tests

# Integration tests only
dotnet test tests/LibraryApi.Api.Tests
```

Tests use EF Core InMemory provider — no external database required.

## API Endpoints

### Books

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/books` | List books (filter: `search`, `title`, `author`, `genre`, `available`, `page`, `pageSize`) |
| GET | `/api/books/{id}` | Get book by ID |
| POST | `/api/books` | Create a book |
| PUT | `/api/books/{id}` | Update a book |
| DELETE | `/api/books/{id}` | Delete a book (fails if any loan history exists) |

### Loans

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/loans` | List loans (filter: `search`, `bookId`, `active`, `page`, `pageSize`) |
| GET | `/api/loans/{id}` | Get loan by ID |
| POST | `/api/loans` | Create a loan |
| POST | `/api/loans/{id}/return` | Return a loan |

## Domain Rules

1. **A book cannot be loaned twice** — if it already has an active loan (no return date), a new loan is rejected with `409 Conflict`
2. **Due date must be after loan date** — enforced in the `Loan` constructor
3. **A book can only be returned if it has an active loan** — returning an already-returned loan throws `409 Conflict`
4. **ISBN must be unique** — enforced via database unique index and service-level validation
5. **Books with loan history cannot be deleted** — prevents orphaned loan records

## Cross-Cutting Concerns

- **Validation**: FluentValidation via a reusable endpoint filter → `400` with field-level errors
- **Error handling**: Global exception handler → domain errors (`409`), not found (`404`), server errors (`500`)
- **Correlation IDs**: `X-Correlation-Id` header generated/propagated on all requests and responses
- **Structured logging**: Built-in `ILogger` with JSON output in production, console in development

## Design Decisions

| Decision | Choice | Why |
|----------|--------|-----|
| No MediatR | Direct service injection | Simpler to understand and explain; less overhead for a small project |
| SQLite | File-based database | Zero setup, cross-platform, supports EF Core migrations |
| EF InMemory for tests | No external DB dependency | Tests run fast and anywhere without setup |
| FluentValidation | Endpoint filter pattern | Consistent, testable, cross-cutting validation |
| Clean Architecture | 4 layers | Clear separation of concerns, testable business logic |

## Project Structure

```
LibraryApi/
├── src/
│   ├── LibraryApi.Domain/          # Entities, domain exceptions
│   ├── LibraryApi.Application/     # Services, DTOs, validators, interfaces
│   ├── LibraryApi.Infrastructure/  # EF Core, repositories, seed data, DI
│   └── LibraryApi.Api/             # Endpoints, middleware, Program.cs
└── tests/
    ├── LibraryApi.Domain.Tests/    # Unit tests (entities, validators, services)
    └── LibraryApi.Api.Tests/       # Integration tests (full HTTP pipeline)
```
