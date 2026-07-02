# AI Workflow Report

## Overview

This document describes how Claude (Anthropic) was used as a primary AI assistant throughout the development of the Claims Module assessment. It covers prompting strategy, what AI contributed, where it failed, how mistakes were corrected, and what was done independently by the engineer.

**Total effort:** ~5 days of active engineering work across 7 calendar days (2 days lost to scheduling), averaging 5–6 hours per day.

**AI tool used:** Claude Sonnet (claude.ai web interface), single long conversation spanning the entire project.

---

## How AI Was Used

### Role Division

The collaboration followed a clear division of responsibilities:

**Engineer (human) owned:**
- All architectural decisions — layer structure, dependency rules, what patterns to apply and which to skip
- Feature scoping — what to implement vs what to defer
- Domain analysis — reading the FRS, identifying business rules, deciding what matters
- Critical review of every generated output before committing
- Handler-level logic fixes — most command handlers required manual corrections after generation
- All queries and filters in repositories — written or heavily revised by hand to avoid unnecessary token consumption
- Decision to skip Domain Events entirely (not required by spec, AI initially included them)
- Decision to simplify `IUnitOfWork` to a single `SaveChangesAsync` method
- Frontend architecture decisions — Standalone components, Signals over NgRx, feature-based structure

**AI (Claude) contributed:**
- Implementation of boilerplate — entity configurations, repository implementations, MediatR handlers structure
- Writing out long configuration files — `ClaimsDbContext`, EF Core Fluent API configurations
- MediatR pipeline behaviors (Logging, Validation, Transaction)
- FluentValidation validators
- Angular service layer — all API service files
- Angular model interfaces — TypeScript DTOs matching backend shapes
- Docker Compose, GitHub Actions workflow
- Documentation — ARCHITECTURE.md and this file

---

## Prompting Strategy

### Context Loading

The conversation started with a full analysis session — both spec files (`DICEUS_Fullstack_Technical_Assessment.md` and `Claims_Module_Candidate_Specification.md`) were shared and Claude produced a summary report with risk analysis before any code was written. This front-loaded context proved valuable throughout — Claude could reference spec sections without re-reading.

### Incremental Development

Rather than asking for large chunks at once, the workflow was incremental:
- One entity at a time in Domain layer
- One interface at a time in Application layer
- One handler at a time, with explicit review before moving on

This prevented the common failure mode of AI generating interconnected code with subtle inconsistencies across files.

### Architecture Enforcement

Key architectural rules were stated explicitly and reinforced throughout:
- "Domain has zero external dependencies"
- "No Domain Events — not required by spec"
- "IUnitOfWork has only SaveChangesAsync"
- "Primary constructors everywhere, no private fields"
- "Sealed classes by default"
- "AsNoTracking on read-only queries only"
- "AuditLogService lives in Application, not Infrastructure"

When Claude violated these rules (which happened regularly), the correction was immediate and explicit.

---

## AI Mistakes and Corrections

### Domain Events — Removed Entirely

**What AI did:** Generated `IDomainEvent` interface with MediatR dependency, added domain event collection to `BaseEntity`, raised events from `Claim.Create()` and status transitions.

**Problem:** Domain Events introduced a MediatR dependency into the Domain layer — violating the core rule that Domain has zero external dependencies. Additionally, the spec never required Domain Events; Claude added them as a "best practice" pattern without being asked.

**Fix:** Engineer identified the violation, AI confirmed and removed all Domain Events code — `IDomainEvent`, `_domainEvents` collection from `BaseEntity`, all `AddDomainEvent()` calls. Result: Domain layer has zero NuGet dependencies.

### `IUnitOfWork` Over-Engineering

**What AI did:** Initially proposed `IUnitOfWork` containing all repositories (`uow.Claims`, `uow.Reserves`, `uow.Parties`, etc.) — a common pattern but unnecessary here.

**Problem:** This couples every handler to the full repository surface through a single interface, reduces DI clarity, and adds indirection without benefit when repositories can simply be injected directly.

**Fix:** Engineer rejected the pattern. `IUnitOfWork` was reduced to a single method: `Task<int> SaveChangesAsync(CancellationToken ct)`. All repositories injected directly into handlers via DI.

### `PerformContext` in Hangfire Job Constructor

**What AI did:** Added `PerformContext` as a primary constructor parameter on `GlPostingJob`, then registered the job with `services.AddScoped<GlPostingJob>()`.

**Problem:** DI container cannot resolve `PerformContext` — Hangfire injects it only as a method parameter at execution time, not through the DI container. The app crashed on startup with `Unable to resolve service for type 'Hangfire.Server.PerformContext'`.

**Fix:** Moved `PerformContext` from constructor to method parameter `ExecuteAsync(..., PerformContext performContext)`. Removed `services.AddScoped<GlPostingJob>()` — Hangfire resolves jobs through its own activator.

### Manual `RowVersion` Comparison

**What AI did:** Generated `if (!claim.RowVer.SequenceEqual(request.RowVersion)) throw new ConcurrencyException(...)` in multiple command handlers.

**Problem:** This duplicates EF Core's built-in optimistic concurrency mechanism. EF Core already performs the `WHERE RowVer = @originalValue` check in the generated `UPDATE` statement. The manual check runs before EF Core's check and gives a false sense of safety without real atomicity.

**Fix:** Engineer identified the duplication. All manual `SequenceEqual` checks were removed. `SetOriginalRowVersion(entity, rowVersion)` method was added to repositories — sets `OriginalValue` on the EF Core entry so EF Core performs the concurrency check correctly during `SaveChangesAsync`. `DbUpdateConcurrencyException` → 409 in `ExceptionHandlingMiddleware`.

### `byte[] RowVersion` in HTTP Contracts

**What AI did:** Used `byte[] RowVersion` in request records (commands and API request DTOs).

**Problem:** ASP.NET Core model binding fails to deserialize base64 strings into `byte[]` inside JSON request bodies — produces `"The command field is required"` error. Discovered during Swagger testing.

**Fix:** All request records changed to `string RowVersion` (base64). Controllers call `Convert.FromBase64String(request.RowVersion)` before passing to commands. `ClaimDetailDto` and `ReserveComponentDto` return `string rowVer` (base64) — consistent contract end-to-end.

### `[AsParameters]` on GET Endpoint

**What AI did:** Used `[AsParameters] ListClaimsRequest request` on `GET /api/claims` endpoint.

**Problem:** Swagger UI incorrectly treated the parameter as a request body, producing `TypeError: Failed to execute 'fetch' on 'Window': Request with GET/HEAD method cannot have body`.

**Fix:** Replaced with explicit `[FromQuery]` parameters on the method signature — verbose but unambiguous and Swagger-compatible.

### `[FromBody] SomeCommand` Directly in Controllers

**What AI did:** Initially bound MediatR commands directly as `[FromBody]` parameters in controllers (e.g., `[FromBody] CreateClaimCommand command`).

**Problem:** ASP.NET Core model binding produces `"The command field is required"` for MediatR record commands, especially when they contain `byte[]` fields or complex nested types.

**Fix:** Separate `*Request` record for each endpoint. Controller manually maps request to command. `OrganisationId` taken from JWT via `ICurrentUserService`, never from request body.

### `Microsoft.AspNetCore.OpenApi` + Swashbuckle Conflict

**What AI did:** Suggested using both `builder.Services.AddOpenApi()` (.NET 9 built-in) and `Swashbuckle.AspNetCore` simultaneously.

**Problem:** Both packages pull different major versions of `Microsoft.OpenApi`, causing `ReflectionTypeLoadException` at startup — the entire app fails to start.

**Fix:** Removed `Microsoft.AspNetCore.OpenApi` entirely. Pinned `Swashbuckle.AspNetCore` to version `6.9.0` which is compatible with `Microsoft.OpenApi 1.x`. Since the spec explicitly requires Swagger, Swashbuckle was the correct choice.

### Azure `TestUsers` Configuration

**What AI did:** Suggested storing `TestUsers` as a single JSON array string in Azure App Service Application Settings.

**Problem:** Azure App Service does not parse JSON arrays in Application Settings. The `configuration.GetSection("TestUsers").Get<List<TestUser>>()` call returns null, causing all login attempts to fail with 401.

**Fix:** Each test user expanded into individual Azure Application Settings using `__` (double underscore) notation: `TestUsers__0__Email`, `TestUsers__0__Password`, etc. — 5 settings per user × 3 users = 15 settings total.

### `Guid.Empty` in `IdempotencyKey`

**What AI did:** Called `reserve.AddTransaction(...)` before `SaveChangesAsync`, generating `IdempotencyKey = "Reserve:00000000-0000-0000-0000-000000000000:Change:1"` because `reserve.Id` was `Guid.Empty` until saved to DB.

**Problem:** `NEWSEQUENTIALID()` is generated by SQL Server on insert — the entity's `Id` is `Guid.Empty` in C# until `SaveChangesAsync` completes. The unique constraint on `IdempotencyKey` caused `SqlException: Cannot insert duplicate key row` on second reserve creation.

**Fix:** `ClaimReserveComponent` generates its own `Id` via `Guid.NewGuid()` in the `Create()` factory method, using `SetId()` helper in `BaseEntity`. `ValueGeneratedNever()` in EF Core configuration for this entity only. All other entities continue using `NEWSEQUENTIALID()`. This allows a single atomic `SaveChangesAsync` with a correct `IdempotencyKey`.

### `forkJoin` for Sequential Party Creation in FNOL Form

**What AI did:** Used `forkJoin([...partyRequests, reserveRequest])` to send all `AddParty` requests in parallel in the FNOL form submission.

**Problem:** `AddParty` command uses optimistic concurrency via `RowVersion`. Parallel requests all carry the same `RowVersion` — the second request arrives after the first has already updated `RowVer`, causing a 409 Conflict.

**Fix:** Changed to `reduce`-based sequential chain where each `AddParty` response carries the updated `rowVer` which is passed to the next request. `CreateClaimResult` extended with `RowVer` field. `AddPartyResult` extended with `ClaimRowVer`. Zero extra GET requests needed.

---

## What Was Done Without AI

- Reading and interpreting the full FRS specification
- Initial risk analysis and scope decisions
- All architectural decisions (layer structure, dependency rules, patterns to use/skip)
- Decision to use feature-based Application structure instead of type-based
- Decision to extract `ClaimStateMachine` as a separate static class
- Decision to separate `IClaimAuditLogRepository` (read) from `IClaimAuditLogWriter` (write)
- Decision to move `AuditLogService` from Infrastructure to Application layer
- Manual corrections to most command handlers after AI generation
- All repository query methods — written or reviewed line by line
- Frontend architecture decisions (Signals, Standalone, feature routes)
- Azure infrastructure setup and troubleshooting
- Identifying and fixing all runtime bugs discovered during Swagger testing

---

## Observations on AI-Assisted Development

**What AI does well:**
- Boilerplate generation with correct structure when given precise context
- Remembering conventions when explicitly stated ("primary constructors", "sealed classes")
- Generating consistent TypeScript models from backend DTO descriptions
- Writing documentation when given accurate context to work from

**Where AI requires active supervision:**
- Pattern application without asking — AI adds Domain Events, Outbox pattern, complex abstractions that weren't requested and aren't needed
- Context drift in long conversations — rules stated early get forgotten, requiring re-enforcement
- Azure-specific configuration — AI knows general .NET configuration patterns but not Azure App Service's specific quirks (`__` notation, JSON array limitations)
- Concurrency and atomicity — AI often generates plausible-looking but subtly incorrect concurrent code that requires careful review
- HTTP contract design — model binding edge cases, Swagger compatibility issues are consistently missed

**Key lesson:** AI is most effective as an implementation accelerator when the engineer maintains tight architectural control. The value is in removing typing work, not in making design decisions. Every significant decision in this codebase was made by the engineer; AI implemented those decisions.
