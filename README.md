# MediPoint

![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?logo=microsoftsqlserver&logoColor=white)
![MongoDB](https://img.shields.io/badge/MongoDB-47A248?logo=mongodb&logoColor=white)
![Architecture](https://img.shields.io/badge/Clean_Architecture-CQRS-2496ED)

**Smart Healthcare Appointment System** — a role-based REST API for booking and managing medical appointments, built with **.NET 10 / ASP.NET Core** using Clean Architecture and CQRS.

Patients search for doctors by specialty and book, cancel, and review appointments; doctors manage their schedules, complete visits, and issue prescriptions; admins manage doctors and register patients. Authentication is JWT-based with per-role refresh tokens. Relational data lives in **SQL Server**; medical records, prescriptions' medicines, and lab results live in **MongoDB**.

> Academic project (Task 2). The original brief targeted Spring Boot/Java; this implementation maps those concepts onto the .NET ecosystem (MediatR ≈ application services, FluentValidation ≈ Bean Validation, EF Core ≈ JPA/Hibernate).

---

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Data Model](#data-model)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Seeded Accounts](#seeded-accounts)
- [API Reference](#api-reference)
- [Authentication & Security](#authentication--security)
- [Error Handling](#error-handling)
- [Testing](#testing)
- [Roadmap / Known Gaps](#roadmap--known-gaps)

---

## Features

### Patient
- Register (via Admin), log in, and refresh tokens
- Search available doctors by specialty (soft-removed doctors are excluded)
- Book an open appointment slot
- Cancel an own appointment (with optional reason)
- Update own profile details
- View own medical records

### Doctor
- Log in and refresh tokens
- View today's appointments
- Create appointment slots (with overlap detection)
- Mark a confirmed appointment as completed
- Add prescriptions (medicines + lab results), persisted to MongoDB

### Admin
- Log in and refresh tokens
- Add a new doctor
- Update a doctor's profile
- Remove a doctor (soft delete — preserves history)
- Register a new patient

---

## Tech Stack

| Concern | Technology |
|---|---|
| Runtime / Framework | .NET 10, ASP.NET Core Web API |
| Language | C# (nullable + implicit usings enabled) |
| Mediator / CQRS | MediatR 14.2.0 |
| Validation | FluentValidation 12.1.1 (pipeline behavior) |
| Object mapping | Mapster 10.0.11 |
| Password hashing | BCrypt.Net-Next 4.2.0 |
| Relational data | Entity Framework Core 10.0.10 + SQL Server |
| Document data | MongoDB.Driver 3.10.0 |
| Auth | JWT Bearer (Microsoft.AspNetCore.Authentication.JwtBearer 10.0.10) |
| API docs / UI | OpenAPI + Scalar 2.16.17 |
| Tests | xUnit 2.9.3 (scaffolded) |

---

## Architecture

The solution follows **Clean Architecture** with four projects and a strict inward dependency rule (`Api → Application → Domain`, `Infrastructure → Application → Domain`):

```
┌──────────────────────────────────────────────────────────┐
│  MediPoint.Api            Controllers, JWT auth, DI,       │
│                           global exception handling         │
├──────────────────────────────────────────────────────────┤
│  MediPoint.Application    CQRS features (Command/Query +    │
│                           Handler + Validator + DTOs),      │
│                           IAppDbContext abstraction         │
├──────────────────────────────────────────────────────────┤
│  MediPoint.Infrastructure EF Core DbContext, Mongo          │
│                           services, JWT provider, seeding   │
├──────────────────────────────────────────────────────────┤
│  MediPoint.Domain         Entities, enums (no dependencies) │
└──────────────────────────────────────────────────────────┘
```

### CQRS vertical slices
Each feature is a self-contained folder under `Features/{Role}/{Feature}/` containing:
- a **Command/Query** record (`IRequest<TResponse>`),
- a **Handler** (`IRequestHandler`, primary constructor taking `IAppDbContext`),
- an optional **FluentValidation** validator,
- feature-specific **DTOs**.

Handlers and validators are **auto-discovered** by assembly scan in `Program.cs` — adding a feature requires no manual DI registration.

### Request pipeline
```
HTTP request
   → Controller action  (reads caller id from JWT claims)
   → mediator.Send(Command/Query)
   → ValidationBehavior  (runs FluentValidation; 400 on failure)
   → Handler  (business logic via IAppDbContext / Mongo services)
   → Mapster  (entity → DTO)
   → Ok(dto)
Exceptions → GlobalExceptionHandler → RFC 7807 ProblemDetails
```

---

## Project Structure

```
MediPoint/
├── MediPoint.slnx                     # Solution (XML format)
├── MediPoint.postman_collection.json  # ApiDog / Postman collection
├── request.http                       # Ad-hoc HTTP requests
├── src/
│   ├── MediPoint.Api/                 # Web API host
│   │   ├── Controllers/               # Admin, Doctor, Patient
│   │   ├── Exceptions/                # GlobalExceptionHandler
│   │   ├── Program.cs                 # DI, auth, pipeline, startup seeding
│   │   └── appsettings.json
│   ├── MediPoint.Application/
│   │   ├── Common/                    # IAppDbContext, behaviors, exceptions, services
│   │   └── Features/
│   │       ├── Admins/                # Login, RefreshToken, AdminAddsDoctor,
│   │       │                          #   UpdateDoctor, RemoveDoctor, RegisterPatient
│   │       ├── Doctors/               # Login, RefreshToken, AddAppointment,
│   │       │                          #   AppointmentsQuery, AddPrescription, CompleteAppointment
│   │       └── Patients/              # Login, RefreshPatientToken, FindDoctors,
│   │                                  #   BookAppointment, CancelAppointment,
│   │                                  #   UpdateDetails, GetRecords
│   ├── MediPoint.Infrastructure/
│   │   ├── Data/                      # AppDbContext + EF configurations + migrations
│   │   ├── MongoData/                 # MongoDbContext + document services
│   │   └── Common/                    # JwtTokenServiceProvider, UsersDataSeed
│   └── MediPoint.Domain/
│       └── Entities/                  # Users, Appointments, Prescriptions,
│                                      #   MedicalRecords, RefreshToken + enums
└── tests/
    └── MediPoint.Tests/               # xUnit test project (scaffolded)
```

---

## Data Model

### SQL Server (Entity Framework Core)
| Entity | Notes |
|---|---|
| `Admin`, `Doctor`, `Patient` | Inherit `BaseUser` (FirstName, LastName, Email, PasswordHash, PhoneNumber, DateOfBirth, Gender) |
| `Appointment` | Date, Duration, `Status` (Pending/Confirmed/Completed/Cancelled), Reason, Notes, CancellationReason, FKs to Doctor + Patient |
| `Prescription` | Linked to Doctor/Appointment (Doctor FK is **Restrict**) |
| `AdminRefreshToken`, `DoctorRefreshToken`, `PatientRefreshToken` | Per-role refresh tokens with `ExpiresAt` |

`Doctor` uses a soft-delete flag (`IsAvailable`) rather than row deletion, which preserves appointment and prescription history.

### MongoDB (document store)
These types are explicitly **ignored** by EF Core (`modelBuilder.Ignore<T>()`) and stored as documents:
| Document | Collection |
|---|---|
| `MedicalRecord` (PatientId, DoctorId, Diagnosis, Notes, Treatment, CreatedAt) | `MedicalRecords` |
| `Medicine` | `Medicine` |
| `LabResult` | `LabResults` |

---

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- **SQL Server** reachable at `Server=.` (LocalDB, Express, or full) — used via `Trusted_Connection`
- **MongoDB** running at `mongodb://localhost:27017`

### Run
```bash
# from the repository root
dotnet restore
dotnet build

# run the API (uses the https launch profile)
dotnet run --project src/MediPoint.Api
```

On startup the app **automatically applies EF Core migrations** and **seeds** demo admins, doctors, patients, and appointments (only if the tables are empty).

The API is served at **https://localhost:7213**:
- Scalar API explorer → https://localhost:7213/scalar/v1
- OpenAPI document → https://localhost:7213/openapi/v1.json

> The dev server is HTTPS-only with a self-signed certificate. When using an external client (ApiDog/Postman/curl), disable TLS verification or trust the dev cert (`dotnet dev-certs https --trust`).

---

## Configuration

Configuration lives in [src/MediPoint.Api/appsettings.json](src/MediPoint.Api/appsettings.json):

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MediPoint;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "MediPoint": {                                  // MongoDB settings
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "MediPoint",
    "MedicalRecordsCollectionName": "MedicalRecords",
    "LabResultsCollectionName": "LabResults",
    "MedicineCollectionName": "Medicine"
  },
  "JwtSettings": {
    "Issuer": "localhost",
    "Audience": "localhost",
    "TokenExpirationInMinutes": 200,
    "SecretKey": "<use a strong secret; keep it out of source control>"
  }
}
```

> **Security note:** the repo ships with a demo `JwtSettings:SecretKey` for local convenience. For anything beyond local development, move it to [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) or environment variables and rotate it.

---

## Seeded Accounts

The seeder creates ready-to-use accounts (password is BCrypt-hashed at seed time):

| Role | Email | Password |
|---|---|---|
| Admin | `admin@medipoint.com` | `Admin@123` |
| Admin | `sarah.manager@medipoint.com` | `Admin@123` |
| Doctor | `dr.smith@medipoint.com` (Cardiology) | `Doctor@123` |
| Doctor | `dr.johnson@medipoint.com` (Pediatrics) | `Doctor@123` |
| Doctor | `dr.williams@medipoint.com` (Orthopedics) | `Doctor@123` |
| Doctor | `dr.brown@medipoint.com` (Dermatology) | `Doctor@123` |
| Doctor | `dr.davis@medipoint.com` (Neurology) | `Doctor@123` |
| Patient | `alice.wilson@email.com` | `Patient@123` |
| Patient | `robert.martinez@email.com` … +4 more | `Patient@123` |

A set of sample appointments (various statuses) is also seeded.

---

## API Reference

Base URL: `https://localhost:7213`. All non-auth endpoints require an `Authorization: Bearer <token>` header for the matching role. Enums are serialized as **strings** (`"Male"`, `"Confirmed"`).

### Patient — `/patients`
| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/patients/login` | — | Authenticate; returns access + refresh tokens |
| POST | `/patients/refresh-token` | — | Exchange `{ refreshToken }` for a new token pair |
| GET | `/patients/search-doctors/{speciality}` | Patient | List available doctors (+ their slots) by specialty |
| POST | `/patients/book-appointment/{appointmentId}` | Patient | Book an open slot |
| POST | `/patients/cancel-appointment/{appointmentId}` | Patient | Cancel own appointment (`{ cancellationReason? }`) |
| POST | `/patients/update-details` | Patient | Update own profile (no email/password change) |
| GET | `/patients/get-medical-records` | Patient | Retrieve own medical records (Mongo) |

### Doctor — `/api/Doctor`
| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/Doctor/login` | — | Authenticate |
| POST | `/api/Doctor/refreshToken` | — | Raw JSON-string body: `"<refreshToken>"` |
| GET | `/api/Doctor/get-Appointments-today` | Doctor | Today's appointments for the doctor |
| POST | `/api/Doctor/add-appointment` | Doctor | Create a slot (409 on time overlap) |
| POST | `/api/Doctor/add-prescription` | Doctor | Add prescription + lab result |
| POST | `/api/Doctor/complete-appointment/{appointmentId}` | Doctor | Mark a confirmed appointment completed (`{ notes? }`) |

### Admin — `/api/Admin`
| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/Admin/login` | — | Authenticate |
| POST | `/api/Admin/refreshtoken` | — | Raw JSON-string body: `"<refreshToken>"` |
| POST | `/api/Admin/Add-doctor` | Admin | Create a doctor |
| POST | `/api/Admin/update-doctor/{doctorId}` | Admin | Update a doctor's profile (no email/password change) |
| POST | `/api/Admin/remove-doctor/{doctorId}` | Admin | Soft-remove a doctor (`IsAvailable = false`) |
| POST | `/api/Admin/register-patient` | Admin | Register a new patient |

---

## Authentication & Security

- **JWT Bearer** tokens carry the user id (`sub`/`NameIdentifier`) and `role` claim; `[Authorize(Roles = "...")]` gates each endpoint.
- Token lifetime and signing are configured under `JwtSettings`; validation checks issuer, audience, lifetime (zero clock skew), and signing key.
- **Refresh tokens** are stored per role with an `ExpiresAt` and are rejected when missing or expired (401).
- **Passwords** are hashed with BCrypt and never returned in responses or exception messages.
- **Login** returns a generic `401 Invalid email or password` to avoid revealing which emails exist.
- **Ownership checks**: patients may only cancel their own appointments and doctors only complete their own (a mismatch returns `404` to avoid leaking existence).
- **Profile updates** (patient details / doctor update) change profile fields only — email and password are never mutated by these flows.

---

## Error Handling

A single `IExceptionHandler` (`GlobalExceptionHandler`) maps domain exceptions to RFC 7807 `ProblemDetails`:

| Exception | HTTP status |
|---|---|
| `ValidationException` (FluentValidation) | `400 Bad Request` |
| `UnauthorizedException` | `401 Unauthorized` |
| `NotFoundException` | `404 Not Found` |
| `ConflictException` | `409 Conflict` |
| _(unhandled)_ | `500 Internal Server Error` |

Every thrown exception in the codebase is a typed domain exception — there are no raw `throw new Exception(...)` that would leak as a 500.

---

## Testing

### API collection (ApiDog / Postman)
Import [MediPoint.postman_collection.json](MediPoint.postman_collection.json) — a Postman v2.1 collection (ApiDog-compatible) with **19 requests** across Admin / Doctor / Patient folders:
- Login requests **auto-save** the JWT and refresh token into collection variables; authenticated requests send the bearer token automatically.
- The *Search Doctors* request auto-captures a bookable `appointmentId`.
- Seeded credentials are pre-filled; see the collection description for the recommended run order.

### Unit tests
```bash
dotnet test
```
The `MediPoint.Tests` xUnit project is scaffolded and wired into the solution; substantive test cases are not yet implemented.

---

## Roadmap / Known Gaps

- [ ] Unit/integration test coverage (project scaffolded only)
- [ ] Chatbot / role-based AI assistant (in the original brief; not implemented)
- [ ] AOP-style logging behavior (currently inline `ILogger` only)
- [ ] Caching for doctor lookups
- [ ] Move the JWT secret out of `appsettings.json`

---

_Built with .NET 10 · Clean Architecture · CQRS_
