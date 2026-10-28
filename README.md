# Order & Inventory Management System

REST API for managing products and orders. Placing an order checks stock and reduces it inside one database transaction.

**Stack:** ASP.NET Core 8 Web API, Entity Framework Core, SQL Server, Docker, GitHub Actions

## Features
- Products CRUD
- Create orders with stock validation and transactional stock deduction
- Swagger UI, health check endpoint, CORS enabled
- Docker Compose setup (API + SQL Server)
- CI build on every push

## Run with Docker
```bash
docker compose up --build
```
Open http://localhost:5000/swagger

## Run locally
1. Start SQL Server (or use the `db` service from docker-compose).
2. Update the connection string in `api/appsettings.json`.
3. `cd api && dotnet run`

## API
| Method | Endpoint | Description |
|---|---|---|
| GET/POST | /api/products | List / create products |
| GET/PUT/DELETE | /api/products/{id} | Read / update / delete |
| GET/POST | /api/orders | List / place orders |

Sample order:
```json
{ "customerName": "Ravi", "items": [ { "productId": 1, "quantity": 2 } ] }
```

## Roadmap
- [ ] JWT authentication and roles
- [ ] Angular frontend (AG Grid, dashboard)
- [ ] Unit tests (xUnit)
- [ ] Razorpay test-mode payments
- [ ] Live deployment
