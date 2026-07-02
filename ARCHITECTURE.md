# Claims Module — FNOL & Reserve Management
## Backend Architecture Document

> **Stack:** .NET 9 · ASP.NET Core · Clean Architecture · CQRS (MediatR) · EF Core 9 · SQL Server 2022 · Hangfire · Azure Blob Storage / Local FS · Swashbuckle

> Документ відображає актуальний стан реалізованого бекенду (станом на завершення Application + API шарів, перед стартом фронтенду).

---

## 1. Структура репозиторію

```
claims-module/
│
├── src/
│   ├── claims-frontend/             # Angular 18 (standalone, no NgModules)
│   ├── ClaimsModule.Domain/
│   ├── ClaimsModule.Application/
│   ├── ClaimsModule.Infrastructure/
│   ├── ClaimsModule.Persistence/
│   └── ClaimsModule.API/
│
├── tests/
│   ├── ClaimsModule.Domain.Tests/
│   ├── ClaimsModule.Application.Tests/
│   └── ClaimsModule.Integration.Tests/
│
├── .github/
│   └── workflows/
│       ├── backend-ci.yml
│       └── deploy-azure.yml
│
├── .env                              # секрети, не в репо
├── .env.example                      # шаблон, в репо
├── docker-compose.yml                # SQL Server для локалки
├── ClaimsModule.sln
├── global.json
├── README.md
├── ARCHITECTURE.md                   # цей файл
└── AI-WORKFLOW.md
```

Усі .NET проекти живуть в `src/` поруч з Angular проектом. Domain events як патерн свідомо не використовуються — у ТЗ немає вимоги до event-driven моделі, тому домен лишається мінімальним: entities, enums, exceptions, constants, state machines.

---

## 2. Шарова архітектура

```
ClaimsModule.API
    DI: Application + Persistence + Infrastructure
        ClaimsModule.Application
            залежить лише від інтерфейсів власних + Domain
                ClaimsModule.Domain (нуль залежностей)
        ClaimsModule.Persistence   -> реалізує інтерфейси Application
        ClaimsModule.Infrastructure -> реалізує інтерфейси Application
```

**Правило залежностей:** `Domain` не залежить ні від чого. `Application` залежить лише від `Domain`. `Persistence` і `Infrastructure` залежать від `Application` + `Domain` і реалізують інтерфейси визначені в `Application`. `API` залежить від усіх трьох і відповідає лише за DI wiring, HTTP-біндинг та middleware.

Жодних Domain Events, жодного MediatR у Domain — `Domain` не має жодної NuGet залежності окрім BCL.

---

## 3. Domain Layer

```
Domain/
├── Common/
│   └── BaseEntity.cs              # Id, audit columns, soft delete — нічого більше
├── Constants/
│   ├── AuditEventTypes.cs
│   ├── DocumentTypes.cs
│   ├── PerilCategories.cs
│   ├── ReserveThresholds.cs       # $10K / $100K / $10M пороги — single source of truth
│   └── UserRoles.cs
├── Entities/
│   ├── Claim.cs                   # Aggregate Root
│   ├── LossEvent.cs               # створюється виключно через Claim.Create(...)
│   ├── ClaimParty.cs
│   ├── ClaimRiskObject.cs
│   ├── ClaimDocument.cs
│   ├── ClaimReserveComponent.cs   # Aggregate Root, власний RowVer
│   ├── ReserveHistory.cs          # append-only, без BaseEntity
│   ├── ClaimAuditLog.cs           # immutable, без BaseEntity
│   ├── CauseOfLossCode.cs         # reference, без BaseEntity
│   ├── Policy.cs                  # simulated, без BaseEntity
│   └── ClaimSequence.cs           # технічна, для атомарної генерації ClaimNumber
├── Enums/                         # 12 enum-ів, кожен в окремому файлі
├── Exceptions/
│   ├── DomainException.cs         # -> 422
│   └── NotFoundException.cs       # -> 404
└── StateMachines/
    └── ClaimStateMachine.cs       # єдине джерело правди для переходів статусів
```

### Ключові рішення

**`Claim` як єдина точка створення `LossEvent`.** `Claim.Create(...)` приймає всі дані для loss event і створює його всередині фабричного методу — `LossEvent` ніколи не створюється напряму через свій власний `Create` ззовні агрегату. Це гарантує що Claim без LossEvent неможливий.

**`ClaimReserveComponent` — другий Aggregate Root**, не дочірня сутність `Claim`. Має власний `RowVer`. Транзакції (`ReserveHistory`) створюються і шукаються виключно через методи агрегату (`AddTransaction`, `ApproveTransaction`, `RejectTransaction`) — handler ніколи не звертається до `_transactions` напряму.

**`ClaimReserveComponent.Id` генерується в C#-коді (`Guid.NewGuid()`)**, а не через `NEWSEQUENTIALID()` в БД (на відміну від решти entities). Причина: `IdempotencyKey` транзакції (`Reserve:{Id}:Change:{Sequence}`) формується одразу при створенні резерву в одному атомарному `SaveChangesAsync`, без проміжного збереження заради отримання Id з БД.

**`ClaimStateMachine`** — статичний клас, єдине джерело правди для валідних переходів статусу. Використовується і в `Claim.TransitionTo` (валідація) і в `GetClaimStatuses` query (довідкові дані для фронту). Без дублювання логіки.

**`ReserveHistory` і `ClaimAuditLog`** свідомо не наслідують `BaseEntity` — обидва append-only, не мають `UpdatedAt`/`IsDeleted`.

**Жодних навігаційних властивостей "заради зручності".** `Claim.AuditLogs` свідомо прибрано — аудит лог читається виключно через `IClaimAuditLogRepository`, ніколи через `Include` на агрегаті (занадто важкий запит).

---

## 4. Application Layer — Feature-Based структура

На відміну від початкового плану (шар по типам: всі Commands в одній папці, всі Queries в іншій), застосована **feature-based структура**: кожна фіча має власну папку з Commands/Queries, а кожна команда/запит — власну підпапку з командою, handler-ом і валідатором поруч.

```
Application/
├── Common/
│   ├── Behaviors/
│   │   ├── LoggingBehavior.cs
│   │   ├── ValidationBehavior.cs
│   │   └── TransactionBehavior.cs   # обгортає лише *Command (за конвенцією назви типу)
│   ├── Exceptions/
│   │   ├── ValidationException.cs
│   │   └── UnauthorizedException.cs # currentUser.UserId == null
│   └── Interfaces/                  # усі порти: репозиторії, UoW, сервіси
│
├── Services/
│   └── AuditLogService.cs           # живе тут, НЕ в Infrastructure — нуль зовнішніх залежностей
│
├── Claims/
│   ├── Dtos/                        # DTO що шаряться між кількома queries
│   │   ├── ClaimPartyDto.cs
│   │   ├── ClaimRiskObjectDto.cs
│   │   ├── ClaimDocumentDto.cs
│   │   └── ClaimMappingProfile.cs   # AutoMapper Profile для всього фіче-розділу Claims
│   ├── Commands/
│   │   ├── CreateClaim/
│   │   ├── TransitionClaimStatus/
│   │   ├── AssignHandler/
│   │   ├── UpdateNotes/
│   │   ├── AddParty/
│   │   ├── RemoveParty/
│   │   ├── AddRiskObject/
│   │   └── UploadDocument/
│   └── Queries/
│       ├── ListClaims/
│       ├── GetClaimDetail/
│       ├── GetClaimDocuments/       # DownloadUrl генерується вручну, без AutoMapper
│       └── GetClaimAuditLog/
│
├── Reserves/
│   ├── Dtos/
│   ├── Commands/
│   │   ├── OpenReserve/
│   │   ├── AdjustReserve/
│   │   ├── ApproveReserve/
│   │   ├── RejectReserve/
│   │   └── OverrideAggregateLimit/  # BR-R-05, окрема двокрокова операція
│   └── Queries/
│       └── ListReserves/
│
├── Policies/
│   ├── Dtos/
│   └── Queries/
│       ├── SearchPolicies/
│       └── GetPolicyById/
│
└── Reference/
    └── Queries/
        ├── GetCauseOfLossCodes/
        └── GetClaimStatuses/         # без БД — генерується з ClaimStateMachine in-memory
```

### MediatR Pipeline Behaviors

```
Request -> LoggingBehavior -> ValidationBehavior -> TransactionBehavior -> Handler
```

`TransactionBehavior` визначає Command vs Query **за конвенцією назви типу** (`typeof(TRequest).Name.EndsWith("Command")`), без маркерних інтерфейсів `ICommand`/`IQuery`.

### AutoMapper — використовується вибірково

AutoMapper застосовується тільки там, де маппінг простий (entity -> DTO один-в-один, без side-effects). Якщо потрібна async-операція (генерація SAS URL) або трансформація яка не виражається через expression tree (JSON-десеріалізація `Policy.CoverageTypes`) — маппінг робиться вручну прямо в handler. Спроба прогнати `JsonSerializer.Deserialize` через `ForMember(...MapFrom(...))` провалилась через обмеження expression trees (`An expression tree may not contain a call or invocation that uses optional arguments`) — після цього було свідомо вирішено не форсувати AutoMapper там, де він не пасує природно.

### Concurrency control

`RowVersion` (`byte[]`, base64 у JSON) передається клієнтом для кожної команди що модифікує вже існуючий агрегат (`Claim` або `ClaimReserveComponent`). У handler це виглядає так:

```csharp
var claim = await claimRepository.GetByIdAsync(request.ClaimId, ct)
    ?? throw new NotFoundException(nameof(Claim), request.ClaimId);

claimRepository.SetOriginalRowVersion(claim, request.RowVersion);
```

`SetOriginalRowVersion` встановлює `OriginalValue` на EF Core entry — реальну перевірку конфлікту виконує сам EF Core при `SaveChangesAsync()` (кидає `DbUpdateConcurrencyException` -> 409). **Ручний `SequenceEqual` порівняння версій в коді handler-а свідомо не використовується** — це дублювало б роботу EF Core і давало хибне відчуття безпеки без реальної атомарності.

Команди що **створюють** нову сутність (`OpenReserve`, `AddParty`, `CreateClaim`) — без `RowVersion`. Команди що **модифікують** існуючу (`AdjustReserve`, `ApproveReserve`, `RemoveParty`, `TransitionClaimStatus`) — з `RowVersion`.

### Self-authorization guard

`ICurrentUserService` повертає `Guid? UserId`, `string? Role`, `Guid? OrganisationId` — усе nullable. Жодних exceptions всередині сервісу. Кожен handler сам вирішує що робити з відсутнім юзером:

```csharp
var userId = currentUser.UserId ?? throw new UnauthorizedException();
```

RBAC (перевірка ролі) **не** робиться в handler-ах — це відповідальність API шару (`[Authorize(Roles = ...)]`). Handler довіряє що якщо запит дійшов — авторизація вже відбулась. Виняток — бізнес-правила що завʼязані на ролі опосередковано (BR-R-01 authority level, BR-R-03 self-approval) — вони лишаються в Application, бо це доменна логіка, не RBAC.

---

## 5. Persistence Layer

```
Persistence/
├── ClaimsDbContext.cs
├── ClaimsDbContextFactory.cs        # IDesignTimeDbContextFactory — для `dotnet ef migrations`
├── UnitOfWork.cs                    # ЛИШЕ SaveChangesAsync, нічого більше
├── Configurations/                  # 11 IEntityTypeConfiguration, +BaseEntityConfiguration<T>
├── Repositories/                    # по одному репо на сутність
├── Services/
│   └── ClaimNumberGenerator.cs      # MERGE statement, атомарна генерація CLM-YYYY-NNNNNNN
└── DependencyInjection.cs
```

### `IUnitOfWork` — мінімальний

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

Початково планувалось тримати всі репозиторії всередині UoW (`uow.Claims`, `uow.Reserves`, ...) — від цього свідомо відмовились: репозиторії інжектуються в handler напряму через DI, UoW відповідає виключно за збереження змін одного `DbContext`.

### Генерація ClaimNumber

Атомарний `MERGE` на таблиці `ClaimSequences` (composite PK: `OrganisationId + Year`):

```sql
MERGE ClaimSequences AS target
USING (SELECT @orgId AS OrganisationId, @year AS Year) AS source
    ON target.OrganisationId = source.OrganisationId AND target.Year = source.Year
WHEN MATCHED THEN UPDATE SET LastSequence = target.LastSequence + 1
WHEN NOT MATCHED THEN INSERT (...) VALUES (..., 1);
```

Формат: `CLM-{Year}-{LastSequence:D7}` -> `CLM-2026-0000142`.

### Generated ChangeSequence для ReserveHistory

Окремий метод `IReserveRepository.GetNextChangeSequenceAsync` бере `MAX(ChangeSequence) + 1` атомарно в БД (не в пам'яті через `.Count`), плюс **unique constraint** `(ReserveComponentId, ChangeSequence)` на рівні БД як остання лінія захисту проти конкурентного запису.

### Read vs Write розділення на рівні методів репозиторію

- `GetByIdAsync` — **без** `AsNoTracking()`, бо може використовуватись і в Command, і в Query
- `ListByClaimAsync`, агрегатні запити (`SumAsync`, `CountAsync`) — **завжди** з `AsNoTracking()`

### `IClaimAuditLogRepository` vs `IClaimAuditLogWriter`

Розділені свідомо: `IClaimAuditLogRepository` — лише читання (для `GetClaimAuditLog` query), `IClaimAuditLogWriter` — лише `AddAsync` (використовується винятково `AuditLogService`). Обидва реалізуються одним класом `ClaimAuditLogRepository` в Persistence, але контракти лишаються розділеними — handler ніколи не отримує можливості писати в audit log напряму, обходячи `AuditLogService`.

---

## 6. Infrastructure Layer

```
Infrastructure/
├── Jobs/
│   ├── GlPostingJob.cs              # PerformContext — параметр методу, НЕ конструктора
│   └── SlaMonitoringJob.cs          # recurring, кожні 15 хв
├── Services/
│   ├── AzureBlobStorageService.cs
│   ├── LocalFileSystemStorageService.cs
│   ├── GlPostingJobScheduler.cs
│   └── MockGlPostingJobScheduler.cs # Development environment — Hangfire dashboard не обов'язковий локально
└── DependencyInjection.cs
```

`AuditLogService` тут **відсутній** — переїхав в `Application/Services/`, бо не має жодної зовнішньої залежності (тільки інтерфейси з `Application.Common.Interfaces`).

### Чому `PerformContext` — параметр методу, а не конструктора

Перша версія `GlPostingJob` приймала `PerformContext` через primary constructor — DI контейнер не міг його зарезолвити (`Unable to resolve service for type 'Hangfire.Server.PerformContext'`), бо Hangfire підставляє `PerformContext` лише як аргумент виконуваного методу, не через звичайний DI. Виправлено:

```csharp
public async Task ExecuteAsync(
    Guid reserveHistoryId,
    string idempotencyKey,
    PerformContext performContext)   // інжектується Hangfire-ом автоматично при виконанні
```

### GL Posting — known limitation (задокументовано в коді)

Якщо `SaveChangesAsync` після апруву резерву пройшов успішно, але виклик `glPostingJobScheduler.EnqueueGlPosting(...)` після нього впав — транзакція вже `Approved` в БД, але GL job не поставлений в чергу. Для production рекомендований Outbox pattern; в рамках цього ассесменту — try/catch з логуванням, прийнято як свідомий компроміс:

```csharp
await unitOfWork.SaveChangesAsync(ct);

try
{
    glPostingJobScheduler.EnqueueGlPosting(transaction.Id, transaction.IdempotencyKey);
}
catch (Exception ex)
{
    // NOTE: known limitation, see AI-WORKFLOW.md / commit history
    logger.LogError(ex, "Failed to enqueue GL posting job for transaction {TransactionId}.", transaction.Id);
}
```

---

## 7. API Layer

```
API/
├── Controllers/
│   ├── AuthController.cs            # mock JWT, TestUsers з appsettings
│   ├── Claims/
│   │   ├── ClaimsController.cs
│   │   └── *Request.cs              # окремі request DTO для кожного POST/PUT — НЕ command напряму
│   ├── Reserves/
│   │   ├── ReservesController.cs
│   │   └── *Request.cs
│   ├── Policies/
│   │   └── PoliciesController.cs
│   └── Reference/
│       └── ReferenceController.cs
├── Middleware/
│   ├── ExceptionHandlingMiddleware.cs
│   └── CorrelationIdMiddleware.cs
├── Services/
│   └── CurrentUserService.cs        # читає JWT claims через IHttpContextAccessor, все nullable
└── Program.cs
```

### Чому request DTO окремо від Command

`[FromBody] CreateClaimCommand command` напряму в сигнатурі контролера **не працює** — ASP.NET Core модель-біндинг кидає `"The command field is required"` на record-командах MediatR (особливо помітно з `byte[]` полями типу `RowVersion`). Рішення — окремий `*Request` record на кожен ендпоінт, контролер вручну мапить його в Command:

```csharp
[HttpPost]
public async Task<IActionResult> CreateClaim(
    [FromBody] CreateClaimRequest request,
    CancellationToken ct = default)
{
    var organisationId = currentUser.OrganisationId ?? throw new UnauthorizedException();

    var result = await sender.Send(new CreateClaimCommand(
        OrganisationId: organisationId,
        PolicyId: request.PolicyId,
        ...
    ), ct);

    return CreatedAtAction(nameof(GetClaimDetail), new { id = result.ClaimId }, result);
}
```

`OrganisationId` ніколи не приходить в тілі запиту від клієнта — завжди береться з JWT через `ICurrentUserService`.

### `RowVersion` в HTTP — `string`, не `byte[]`

Аналогічна проблема моделбіндингу виникла з `byte[] RowVersion` в request records — ASP.NET Core не зміг коректно задесеріалізувати base64-рядок в `byte[]` всередині record body. Усі `*Request` records використовують `string RowVersion` (base64), контролер конвертує вручну при виклику команди:

```csharp
RowVersion: Convert.FromBase64String(request.RowVersion)
```

### `ListClaims` — explicit `[FromQuery]`, не `[AsParameters]`

`[AsParameters]` на record-параметрі викликав помилку Swagger UI (`Request with GET/HEAD method cannot have body`) — Swagger намагався відправити query-record як body. Виправлено явним переліком `[FromQuery]` параметрів у сигнатурі методу контролера.

### Swagger / Swashbuckle

`Microsoft.AspNetCore.OpenApi` (вбудований в .NET 9 `AddOpenApi()`) і `Swashbuckle.AspNetCore` **конфліктують** — обидва тягнуть різні мажорні версії `Microsoft.OpenApi`, що призводить до `ReflectionTypeLoadException` під час старту. Оскільки ТЗ explicit вимагає Swagger — `Microsoft.AspNetCore.OpenApi` прибраний повністю, залишений лише `Swashbuckle.AspNetCore` (зафіксована версія `6.9.0`, що сумісна з `Microsoft.OpenApi 1.x`).

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { ... });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { ... });
});
```

### Middleware pipeline (порядок критичний)

```
ExceptionHandlingMiddleware
    -> CorrelationIdMiddleware
    -> Swagger (Development)
    -> HttpsRedirection
    -> Authentication
    -> Authorization
    -> Controllers
```

### Exception -> HTTP mapping

| Exception | HTTP |
|---|---|
| `ValidationException` (FluentValidation, агреговано) | 400 |
| `UnauthorizedException` | 401 |
| `NotFoundException` | 404 |
| `DomainException` | 422 |
| `ConcurrencyException` / `DbUpdateConcurrencyException` | 409 |
| необроблений `Exception` | 500 |

Кожен `ProblemDetails` містить `correlationId` з `HttpContext.Items["CorrelationId"]` — той самий id що йде в логи через `ExceptionHandlingMiddleware`, дозволяє наскрізно відслідкувати один інцидент від логу до відповіді клієнту.

### Mock Authentication

`AuthController.Login(email, password)` звіряє проти `TestUsers` секції в `appsettings.Development.json` (не в репо), генерує JWT через `SecurityTokenDescriptor` з claims `sub`, `role`, `organisation_id`. Три тестові акаунти: Handler / Supervisor / Manager, всі в одній `OrganisationId`.

---

## 8. Локальна розробка — фактичний flow

```bash
# 1. SQL Server
docker-compose up -d

# 2. Міграції (design-time factory читає з appsettings.Development.json)
dotnet ef migrations add InitialSchema \
  --project src/ClaimsModule.Persistence \
  --startup-project src/ClaimsModule.API

dotnet ef database update \
  --project src/ClaimsModule.Persistence \
  --startup-project src/ClaimsModule.API

# 3. API
dotnet run --project src/ClaimsModule.API
# Swagger: https://localhost:5001/swagger
# Hangfire Dashboard: https://localhost:5001/hangfire
```

`appsettings.Development.json` (не в репо) повинен містити: `ConnectionStrings:DefaultConnection`, `Jwt:Secret/Issuer/Audience/ExpiryMinutes`, `TestUsers[]`, `Storage:Provider=LocalFileSystem` (для локалки без Azure).

---

## 9. Що протестовано вручну (Swagger, manual QA pass)

- Login -> JWT з усіма ролями
- `CreateClaim` (з і без `PolicyId`) -> `LossEvent` створюється атомарно разом з Claim
- `GetClaimDetail` -> повний агрегат, `RowVer` в base64
- `TransitionClaimStatus` Draft -> Open, з `RowVersion`-конфліктом при застарілій версії -> 409
- `AddParty` -> party з'являється в `GetClaimDetail.parties`
- `AddRiskObject`
- `OpenReserve` amount <= $10K -> авто-апрув, GL job enqueued (mock у Development)
- `OpenReserve` amount > $10K -> `PendingApproval`, видно в `ListReserves`
- `ApproveReserve` (роль Supervisor) -> 204, статус `Approved`, GL job enqueued
- `GetClaimAuditLog` -> всі події присутні з правильним `EventType`

---

## 10. Відхилення від початкового плану

| Початковий план | Що зроблено фактично | Причина |
|---|---|---|
| Domain Events + MediatR notifications | Прибрано повністю | Не вимагається ТЗ, зайва складність для assessment scope |
| `IUnitOfWork` з усіма репозиторіями всередині (`uow.Claims`, `uow.Reserves`) | `IUnitOfWork` лише з `SaveChangesAsync` | Репозиторії — окремі DI-залежності, чистіший SRP |
| Структура Application по типам (всі Commands окремо, всі Queries окремо) | Feature-based (Claims/Reserves/Policies/Reference, кожен з власними Commands+Queries+Dtos) | Читабельність, легше орієнтуватись по фічі |
| `RowVersion` перевіряється вручну в handler (`SequenceEqual`) | Прибрано, покладено на EF Core `OriginalValue` + `DbUpdateConcurrencyException` | Уникнення дублювання логіки конкурентності |
| `Microsoft.AspNetCore.OpenApi` + `Swashbuckle` одночасно | Лишився тільки `Swashbuckle 6.9.0` | Конфлікт версій `Microsoft.OpenApi`, `ReflectionTypeLoadException` |
| `[FromBody] SomeCommand` напряму в контролері | Окремий `*Request` record на кожен ендпоінт | ASP.NET Core model-binding не дружить з MediatR records напряму |
| `byte[] RowVersion` в HTTP контракті | `string RowVersion` (base64), конвертація в контролері | Проблеми моделбіндингу `byte[]` в JSON body |
| GL posting — гарантована атомарність через Outbox | try/catch з логуванням, задокументований known limitation | Поза розумним scope для assessment timeline |
