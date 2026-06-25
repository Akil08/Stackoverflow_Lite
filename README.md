# StackOverflowLite

StackOverflowLite is a .NET 8 clean-architecture Web API inspired by Q&A platforms. It supports authentication, questions, answers, voting, accepted answers, reputation scoring, Redis-backed view tracking, and Redis-backed question list caching.

## Project Overview

- `src/StackOverflowLite.Domain`
  - Core entities and enums.
- `src/StackOverflowLite.Application`
  - CQRS handlers, validators, DTOs, common interfaces/models.
- `src/StackOverflowLite.Infrastructure`
  - EF Core persistence, Identity/JWT setup, Redis services, background sync service.
- `src/StackOverflowLite.API`
  - Controllers, middleware, Swagger, API host.

## Run With Docker Compose

From the solution root:

```bash
docker compose up --build
```

API will be available on:

- `http://localhost:8080`
- Swagger (development): `http://localhost:8080/swagger`

Services started by compose:

- API (`aspnet` app)
- PostgreSQL (`postgres:16-alpine`)
- Redis (`redis:7-alpine`)

## Run Locally Without Docker

1. Ensure dependencies are running and reachable:
- PostgreSQL
- Redis

2. Set environment variables (or use `appsettings.json`/user secrets).

3. Run the API project:

```bash
dotnet run --project src/StackOverflowLite.API
```

Default local URL is determined by ASP.NET settings (commonly `https://localhost:xxxx` and `http://localhost:xxxx`).

## Required Environment Variables

These are the key configuration values used by the API:

- `ConnectionStrings__DefaultConnection`
- `Redis__ConnectionString`
- `JwtSettings__Secret`
- `JwtSettings__Issuer`
- `JwtSettings__Audience`
- `JwtSettings__ExpiryDays`

Example values:

- `ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=stackoverflowlite;Username=postgres;Password=postgres`
- `Redis__ConnectionString=localhost:6379`
- `JwtSettings__Secret=your-secret-key-here-min-32-chars`
- `JwtSettings__Issuer=StackOverflowLite`
- `JwtSettings__Audience=StackOverflowLite`
- `JwtSettings__ExpiryDays=7`

## API Endpoint Groups

### Auth

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/auth/profile` (authorized)

### Questions

- `GET /api/questions`
- `GET /api/questions/{id}`
- `POST /api/questions` (authorized)
- `PUT /api/questions/{id}` (authorized)
- `DELETE /api/questions/{id}` (authorized)

### Answers

- `GET /api/questions/{questionId}/answers`
- `POST /api/questions/{questionId}/answers` (authorized)
- `PUT /api/answers/{id}` (authorized)
- `DELETE /api/answers/{id}` (authorized)
- `PUT /api/questions/{questionId}/accept` (authorized)

### Votes

- `POST /api/votes` (authorized)

## Notes

- Swagger is enabled only in development.
- CORS is configured to allow all origins in development.
- Question view counts are incremented in Redis and periodically synced to PostgreSQL.
- Question list responses are cached in Redis and invalidated on question create/update/delete.
