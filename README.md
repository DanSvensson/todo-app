# To-Do Task Manager

A small full-stack to-do application built as a take-home exercise.

- **Backend:** .NET (Minimal API), EF Core (InMemory), Swagger
- **Frontend:** Vue 3 + Composition API, TypeScript, Vite

## Repo Layout

```text
/
├── backend/   # .NET API (solution + projects)
└── frontend/  # Vue app
```

## Running the Backend API

From the repo root:
```
cd backend
dotnet restore
dotnet run
```
The API will print the listening URL in the console (example: http://localhost:5218).

## Running the Frontend (Vue)

From the repo root:
```
cd frontend
npm install
npm run dev
```
Vite will print the app URL (example: http://localhost:5173).

## Frontend → Backend configuration

Create frontend/.env:
```
VITE_API_BASE_URL=http://localhost:5040
```
## Features (Production MVP)

Create / list / toggle complete / delete tasks
Filter tasks: All / Open / Completed
Priority indicator (Normal / High / Critical)
Due date display + overdue styling
Basic error handling + empty state UX

## API Endpoints

GET /api/tasks
Optional query: ?completed=true|false
GET /api/tasks/{id}
POST /api/tasks
PUT /api/tasks/{id}
PATCH /api/tasks/{id}/toggle
DELETE /api/tasks/{id}

## Architecture & Code Organization

### Backend

Minimal API endpoints are kept thin; business logic lives in a service layer.
EF Core provides a simple persistence abstraction. For the take-home I used InMemory for fast setup.

### Frontend

Vue UI is built with Composition API.
HTTP calls are isolated in an API client module to keep components focused on UI/state.
UI state uses a single source of truth (allTasks) with computed filters for display.

## Trade-offs & Assumptions

Persistence: EF Core InMemory was chosen to keep the project lightweight and fast to run. In a production MVP I would switch to SQLite + migrations for durability and predictable behavior across restarts.
Authentication/Authorization: intentionally omitted for scope; would be required for multi-user use.
Paging/Search: omitted due to expected small dataset; would add server-side paging and search once the dataset grows.
Concurrency: assumes single-user/local use. For multi-user, introduce optimistic concurrency (row version) and more robust conflict handling.
Validation: basic validation is implemented (e.g., title required). For production, I’d add consistent validation responses (ProblemDetails) and stronger DTO validation.

## What I would implement next

SQLite persistence with migrations
Server-side pagination and sorting (due date, priority)
Edit task UI (PUT) + richer validation messages
Observability: structured logging, tracing, health checks
CI workflow (build + tests + lint)
