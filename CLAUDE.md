# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Test Commands

```bash
# Build the solution
dotnet build ConcurReporting.sln

# Run all tests
dotnet test ConcurExpense.Tests/ConcurExpense.Tests.csproj

# Run a single test by name
dotnet test ConcurExpense.Tests/ConcurExpense.Tests.csproj --filter "FullyQualifiedName~GetReports_ReturnsMappedItems"

# Pack the NuGet package
dotnet pack ConcurExpense/ConcurExpense.csproj -c Release
```

## Architecture

This is a single-assembly .NET 8 class library (`ConcurExpense`) with a companion test project (`ConcurExpense.Tests`).

### Request Flow

`IConcurExpenseClient` (public) → `ConcurExpenseClient` (internal) → `FetchAllPagesAsync` → `GetPageAsync` → HTTP

- `ConcurExpenseClient` holds all resilience logic: rate-limit spacing (`EnforceRateLimitAsync` with a `SemaphoreSlim`), 5xx retries (up to 3), and 429 handling (escalating call interval, failing after 5 consecutive).
- The `Delay` property on `ConcurExpenseClient` is `internal` and overridable — tests replace it with a no-op to avoid real waits while still exercising retry counting logic.
- `IConcurTokenService` / `ConcurTokenService` handles OAuth2 `refresh_token` exchange and caches the token until expiry.
- Two named `HttpClient`s are registered: `ConcurAuth` (for token requests) and `ConcurApi` (for expense API calls).

### Internal vs Public

- `ConcurExpenseClient` is `internal`; `InternalsVisibleTo` exposes it to both `ConcurExpense.Tests` and `DynamicProxyGenAssembly2` (for Moq).
- All DTO types (`ReportDto`, `EntryDto`, `ItemizationDto`, `AllocationDto`, `CustomFieldDto`, `JourneyDto`, `ExpenseBaseDto`) are `public record` types. `ApiModels.cs` contains internal deserialization models with `ToDto()` mappers.

### Testing Approach

Tests instantiate `ConcurExpenseClient` directly (not via DI) using `ClientFactory.Create()`, which wires up a `FakeHttpMessageHandler` (a queue of pre-baked HTTP responses), a `Mock<IConcurTokenService>`, and a no-op `Delay`. Tests verify behavior through request counts and parsed query strings.

## Git Workflow

- Every feature/change goes on its own branch: `git checkout -b feature/<name>`
- Commit changes to the branch
- Ask before merging to main
