# MediPoint React Frontend — Plan

## Context

MediPoint currently has no frontend — the API (`src/MediPoint.Api`) is consumed via Postman/`request.http`. We're building a React SPA to actually use the three role-based flows (Admin, Doctor, Patient) against the existing REST API. Scope per user request: React + React Router + Tailwind CSS only — no Redux/Zustand, no component library, no test framework for v1. TypeScript + Vite, `frontend/` at repo root. Auth token strategy: access token in memory (React context), refresh token in `localStorage`, silently re-hydrated on app load.

## API surface being wired up

Base API runs at `https://localhost:7213` (dev). Three role controllers + one shared controller, all returning `JwtTokenResponse { accessToken, refreshToken, expiresAt }` from login/refresh:

- **Admin** (`/api/Admin`): `login`, `refreshtoken` (raw JSON string body), `Add-doctor`, `update-doctor/{doctorId}`, `remove-doctor/{doctorId}`, `register-patient`
- **Doctor** (`/api/Doctor`): `login`, `refreshToken` (raw JSON string body), `get-Appointments-today`, `add-prescription`, `add-appointment`, `complete-appointment/{appointmentId}`
- **Patient** (`/patients`): `login`, `refresh-token` (`{ refreshToken }` object body), `search-doctors/{speciality}`, `book-appointment/{appointmentId}`, `get-medical-records`, `cancel-appointment/{appointmentId}`, `update-details`, `chat`
- **Users** (`/users`): `profile-image` (multipart upload, any authenticated role)

JWT carries `sub` (user id) + `role` claim (`Admin`/`Doctor`/`Patient`); ownership-mismatch and missing-resource cases return 404, invalid login always returns generic 401. Global exception handler returns RFC 7807 `ProblemDetails` on error — the frontend's API client needs to parse that shape for error messages.

## Folder structure

```
frontend/
  src/
    api/
      client.ts          # fetch wrapper: base URL, JSON handling, ProblemDetails error parsing, 401 → refresh → retry
      auth.ts             # login/refresh calls per role, token refresh orchestration
      admin.ts, doctor.ts, patient.ts, users.ts   # one file per controller, typed request/response functions
    types/
      auth.ts             # JwtTokenResponse, role union type
      admin.ts, doctor.ts, patient.ts             # DTO mirrors (DoctorDto, PatientDto, AppointmentResponse, MedicalRecordResponse, etc.)
    context/
      AuthContext.tsx     # holds { accessToken, role, userId, login(), logout() }, bootstraps from stored refresh token on mount
    routes/
      ProtectedRoute.tsx  # role-gated route wrapper, redirects to the right /login/:role if unauthenticated or wrong role
      router.tsx          # createBrowserRouter tree, grouped by role
    pages/
      auth/LoginPage.tsx           # single login page, role picked via tab/segmented control or /login/:role param
      admin/DashboardPage.tsx, AddDoctorPage.tsx, DoctorsListPage.tsx (update/remove), RegisterPatientPage.tsx
      doctor/TodayAppointmentsPage.tsx, AddAppointmentPage.tsx, AddPrescriptionPage.tsx
      patient/SearchDoctorsPage.tsx, MedicalRecordsPage.tsx, ChatPage.tsx, ProfilePage.tsx (update details + image upload)
    components/
      layout/AppShell.tsx, Sidebar.tsx, Navbar.tsx
      ui/  (Button, Input, Select, Card, Table, Badge, Spinner — small local primitives, Tailwind-styled, no external lib)
    App.tsx, main.tsx
  index.html, tailwind.config.ts, postcss.config.js, vite.config.ts, tsconfig.json, .env.example
```

## Key implementation pieces

1. **Scaffold**: `npm create vite@latest frontend -- --template react-ts`, then add Tailwind (`tailwindcss @tailwindcss/vite` or postcss route — use the current Tailwind v4 Vite plugin setup, simplest), React Router (`react-router-dom` v6/v7 with `createBrowserRouter`).

2. **API client (`api/client.ts`)**: thin wrapper around `fetch`. Reads base URL from `import.meta.env.VITE_API_URL`. On any 401 response (except from the login/refresh endpoints themselves), attempts one silent refresh via the role-appropriate refresh endpoint using the stored refresh token, then retries the original request once. On refresh failure, clears auth state and redirects to login. Central place to unwrap `ProblemDetails` (`{ title, status, detail }`) into a normalized `ApiError`.

3. **Auth (`context/AuthContext.tsx`)**: decodes `role`/`sub` out of the JWT (base64 decode payload, no signature check needed client-side) after login so the app knows which role's routes to render without a separate `/me` endpoint. Persists only the refresh token + role to `localStorage` (namespaced key per role since a user could in theory hold sessions for multiple roles, e.g. `medipoint_refresh_<role>`) — access token stays in React state only, re-acquired on mount via refresh if a stored refresh token exists.

4. **Routing**: top-level routes `/login`, then `/admin/*`, `/doctor/*`, `/patient/*`, each wrapped in `<ProtectedRoute role="Admin|Doctor|Patient">`. Root `/` redirects based on current role or to `/login`.

5. **Role screens** (mapped 1:1 to controller endpoints above):
   - Admin: doctors table (list comes from... note: there's no "list doctors" GET endpoint today, only add/update/remove/patient-search finds doctors by specialty from the Patient side — flag this gap; Admin dashboard will primarily be the three action forms (Add Doctor, Update Doctor by id, Remove Doctor by id) plus Register Patient, not a live doctor table, unless a list endpoint is added later).
   - Doctor: Today's Appointments (table from `get-Appointments-today`), Add Appointment (slot creation form), Complete Appointment (notes form from a row action), Add Prescription (medicines/lab results form tied to an appointment id).
   - Patient: Search Doctors by specialty → results with Book/Cancel actions, Medical Records list, Chat (simple message thread against `/patients/chat`), Update Details form, profile image upload via `/users/profile-image`.

6. **Styling**: Tailwind only, no UI kit. A handful of local primitives in `components/ui/` kept intentionally small (Button, Input, Select, Card, Badge, Spinner, Table) so every page composes from the same handful of styled elements instead of ad hoc classes everywhere.

## Out of scope / flagged for follow-up

- No admin "list all doctors" endpoint exists yet — Admin dashboard can't show a live table without either adding that endpoint or reusing patient search as a stopgap. Will surface this when building the Admin pages rather than guessing a solution now.
- No test framework set up per user's tool list (React Router + Tailwind "and that's enough") — can add Vitest/RTL later if wanted.
- CORS is currently `AllowAnyOrigin` in `Program.cs` — fine for local dev against `localhost:7213`, no change needed unless deploying cross-origin with credentials later.

## Verification

- `npm run dev` in `frontend/`, confirm Vite serves and Tailwind classes render (visual smoke check on Login page).
- Manual end-to-end pass per role against the running API (`dotnet run --project src/MediPoint.Api`): login → land on role dashboard → hit each wired action once (add doctor, book appointment, etc.) → confirm success and error (e.g. wrong password, 401) states render sensibly.
- Refresh flow: manually confirm that after the access token's short expiry, a subsequent action still succeeds (silent refresh-and-retry kicks in) without forcing re-login.
