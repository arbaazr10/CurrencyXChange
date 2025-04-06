# CurrencyXChange
**CurrencyXChange** is a .NET Core Web API that provides currency conversion services powered by the Frankfurter API.
It includes JWT-based authentication and role-based authorization.

## Features
-  Convert currencies using live rates
-  Fetch the latest exchange rates for a given base currency
-  Retrieve historical exchange rate data with pagination
-  JWT Authentication with role-based access (`User`, `Admin`)
-------------------------------------------
## How to Run
-  Required Software: Visual Studio, Docker, SEQ Service Administrator
-  Can be browsed using browser on http://localhost:5064/swagger
-  JWT Token need to be generated 
-  User 1: username: user    password: password
-  User 2: username: admin   password: password
--------------------------------------------
## Technologies Used
- .NET 8
- HttpClient & IMemoryCache
- Polly (Resilience: Retry & Circuit Breaker)
- xUnit, Moq (Unit Testing)
- JWT Bearer Authentication
- Swagger for API documentation
- Serilog with Seq
- OpenTelemetry(Console)
--------------------------------------------
## 🛠️ Endpoints
All endpoints are under `/api/v1/currenctexchange`.

| Endpoint                                   | Method | Auth Role     | Description                                  |
|--------------------------------------------|--------|---------------|----------------------------------------------|
| `/api/v1/CurrenctExchange/latest`          | GET    | None          | Gets the latest rates for a base currency    |
| `/api/v1/CurrenctExchange/v1/convert`      | GET    | User, Admin   | Converts currency between two currencies     |
| `/api/v1/CurrenctExchange/v1/historical`   | GET    | Admin         | Retrieves historical rates with pagination   |

--------------------------------------------
## 🔐 Authentication

- JWT tokens are required for `/convert` and `/historical` endpoints.
- Login is handled via a separate authentication service (not shown here).
- Use the `Authorization: Bearer <token>` header in requests.

