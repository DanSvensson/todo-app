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
The API will print the listening URL in the console (example: http://localhost:5040).

## Running the Frontend (Vue)

From the repo root:
```
cd frontend
npm install
npm run dev
```
Vite will print the app URL (example: http://localhost:5173).
