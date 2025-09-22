# GitHub Copilot Custom Instructions for .NET Engineer

## General Development

- Always target **.NET 8** and use **C# 12** features where appropriate.
- Adhere to **SOLID principles** and prefer **composition over inheritance**.
- Ensure code is **modular, testable, and maintainable**.

## Architecture & Structure

- Follow **clean separation of concerns**:
  - **Controllers** → handle HTTP requests, return responses.
  - **Services** → contain business logic.
  - **Repositories / Data Layer** → handle persistence with EF Core.
- Organize code into logical namespaces and folders:
  - `Controllers/`
  - `Services/`
  - `Models/`
  - `Infrastructure/`
  - `Data/`
- Use **dependency injection** for all services and repositories.

## Asynchronous Programming

- Use `async/await` for all I/O-bound operations.
- Avoid synchronous database or external service calls.

## Data Access

- Use **Entity Framework Core** with:
  - Strongly-typed `DbContext`
  - Entity configurations (via `IEntityTypeConfiguration<T>`)
  - Migrations for schema evolution
- Write queries using **LINQ** with proper async methods (`ToListAsync`, `FirstOrDefaultAsync`, etc.).

## Validation & Security

- Implement validation logic in **dedicated validator classes** (not in controllers).
- Apply **middleware** for centralized validation and exception handling.
- Never trust client input; enforce validation at the API boundary.
- Use **configuration constants** to avoid magic numbers/strings.

## API Development

- Always use `[ApiController]` for REST controllers.
- Add `[ProducesResponseType]` attributes to document response codes.
- Follow REST conventions (`GET`, `POST`, `PUT`, `DELETE`) properly.
- Return appropriate HTTP status codes.

## Documentation

- Write **XML documentation comments** for all public classes and methods.
- Use clear summaries, parameter descriptions, and return value explanations.

## Logging

- Use `ILogger<T>` for structured logging.
- Log key events at appropriate levels:
  - `Information` for normal operations.
  - `Warning` for recoverable issues.
  - `Error` for failures.

## DTOs & Mapping

- Use **DTOs** for request/response models.
- Keep mapping logic **out of controllers/services**.
- Use mapping libraries like **Mapster** for DTO ↔ entity conversions.

## Error Handling

- Implement **centralized exception handling middleware**.
- Never expose internal exception details to clients.
- Return structured error responses.

## Coding Standards

- Use **modern C# features**:
  - Target-typed `new()`
  - Nullable reference types
  - Pattern matching
  - `switch` expressions
- Avoid unnecessary complexity; keep methods short and readable.
- Follow **DRY (Don’t Repeat Yourself)** principle.

---

✅ **Copilot should always generate code following these rules.**
