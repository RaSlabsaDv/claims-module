# Claims Module — FNOL & Reserve Management

> Greenfield vertical slice implementation of an insurance Claims Management module.
> Built as part of DICEUS Engineering Excellence Programme technical assessment.

## Live Demo

| Resource | URL |
|---|---|
| Backend API (Swagger) | `https://claims-module-api-gacch3brgxedc9br.westeurope-01.azurewebsites.net/swagger` |
| Frontend | TBD (Azure Static Web Apps) |

## Test Credentials

| Role | Email | Password |
|---|---|---|
| Handler | handler@test.com | Handler123! |
| Supervisor | supervisor@test.com | Supervisor123! |
| Manager | manager@test.com | Manager123! |

## Tech Stack

**Backend:** .NET 9 · ASP.NET Core · Clean Architecture · CQRS (MediatR) · EF Core 9 · SQL Server / Azure SQL · Hangfire · Azure Blob Storage

**Frontend:** Angular 21 · Angular Material · Signals · Standalone Components

**Infrastructure:** Azure App Service · Azure SQL · GitHub Actions CI/CD

## Architecture

See [ARCHITECTURE.md](./ARCHITECTURE.md) for full backend architecture documentation including:
- Clean Architecture layering and dependency rules
- CQRS flow with MediatR pipeline behaviors
- Domain model design decisions
- Concurrency control strategy
- Known limitations and tradeoffs

## Project Structure

```
claims-module/
├── src/
│   ├── claims-frontend/             # Angular 21 SPA
│   ├── ClaimsModule.Domain/         # Entities, enums, exceptions, state machines
│   ├── ClaimsModule.Application/    # CQRS commands/queries, validators, DTOs
│   ├── ClaimsModule.Infrastructure/ # Hangfire jobs, Azure Blob Storage
│   ├── ClaimsModule.Persistence/    # EF Core DbContext, repositories, migrations
│   └── ClaimsModule.API/            # Controllers, middleware, DI wiring
├── .github/workflows/
│   └── backend-ci.yml               # CI/CD pipeline
├── docker-compose.yml               # Local SQL Server
└── ARCHITECTURE.md
```

## Local Development

### Prerequisites

- .NET 9 SDK
- Node.js 20+
- Docker Desktop

### Backend

```bash
# Start SQL Server
docker-compose up -d

# Apply migrations
dotnet ef database update \
  --project src/ClaimsModule.Persistence \
  --startup-project src/ClaimsModule.API

# Run API
dotnet run --project src/ClaimsModule.API

# Swagger: https://localhost:5001/swagger
# Hangfire: https://localhost:5001/hangfire
```

Create `src/ClaimsModule.API/appsettings.Development.json` (not in repo):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ClaimsModule;User Id=sa;Password=Claims_Dev_2026!;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Secret": "your-secret-min-32-characters-long",
    "Issuer": "claims-module",
    "Audience": "claims-module-client",
    "ExpiryMinutes": "60"
  },
  "TestUsers": [
    {
      "Email": "handler@test.com",
      "Password": "Handler123!",
      "Role": "Handler",
      "UserId": "00000000-0000-0000-0000-000000000001",
      "OrganisationId": "00000000-0000-0000-0000-000000000001"
    },
    {
      "Email": "supervisor@test.com",
      "Password": "Supervisor123!",
      "Role": "Supervisor",
      "UserId": "00000000-0000-0000-0000-000000000002",
      "OrganisationId": "00000000-0000-0000-0000-000000000001"
    },
    {
      "Email": "manager@test.com",
      "Password": "Manager123!",
      "Role": "Manager",
      "UserId": "00000000-0000-0000-0000-000000000003",
      "OrganisationId": "00000000-0000-0000-0000-000000000001"
    }
  ],
  "Storage": {
    "Provider": "LocalFileSystem",
    "LocalFileSystem": {
      "BasePath": "/uploads",
      "BaseUrl": "http://localhost:5001"
    }
  }
}
```

### Frontend

```bash
cd src/claims-frontend
npm install
ng serve

# App: http://localhost:4200
```

## Implemented Features

### Backend (fully implemented and tested)

- **FNOL** — Create claim with LossEvent, automatic ClaimNumber generation (`CLM-2026-NNNNNNN`)
- **Claim lifecycle** — Status transitions with state machine validation (Draft → Open → UnderInvestigation → PendingPayment → Closed)
- **Party management** — Add/remove parties with Claimant guard
- **Risk objects** — Add risk objects to claim
- **Reserve management** — Open/adjust reserves with 3-tier approval ($10K auto / $100K Supervisor / $10M Manager)
- **GL Posting simulation** — Hangfire background job with idempotency key
- **SLA monitoring** — Recurring Hangfire job (every 15 min), breach detection with 24h deduplication
- **Document upload** — Azure Blob Storage / Local FS with compensating transaction
- **Audit log** — Immutable append-only log, 16 event types
- **Optimistic concurrency** — RowVersion on aggregate roots, 409 on conflict
- **Soft delete + tenant isolation** — Global EF Core query filters

### Frontend (partially implemented)

- Auth flow (Login, JWT storage, interceptors, guard)
- Claims list with server-side pagination and filtering
- Shared components (StatusBadge, LoadingSpinner, ConfirmDialog)
- FNOL form — multi-step wizard (in progress)
- API service layer — fully typed HTTP services for all endpoints

## CI/CD

GitHub Actions workflow (`.github/workflows/backend-ci.yml`) triggers on push to `main`:
1. Restore → Build → Publish
2. Deploy to Azure App Service

Database migrations are applied manually via `dotnet ef database update` pointing at Azure SQL.

## AI Workflow

See [AI-WORKFLOW.md](./AI-WORKFLOW.md) for detailed documentation of AI-assisted development process, including prompting strategies, AI mistakes and corrections, and architectural decisions made independently.
