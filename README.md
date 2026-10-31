# SmartERP

SmartERP is an ASP.NET Core microservices and Angular ERP platform. The current implementation includes Identity, company management, API Gateway integration, and a company-scoped Product Catalog.

## Dependency direction

Each microservice follows this dependency chain:

```text
API -> Services -> Repositories -> Entities
API -> DataAccess -> Repositories -> Entities
```

The projects remain separated for `Identity`, `Inventory`, and `Sales`. `SmartERP.Gateway` and `SmartERP.ServiceDefaults` are shared entry/configuration projects. The Angular application is under `src/frontend/Web/smart-erp-web` and will communicate only through the gateway.

## Main folders

```text
src/backend/
  APIs/
  Config/
  DataAccess/
  Entities/
  Repositories/
  Services/

src/frontend/
  Web/
```

## Implemented modules

- Identity: registration, confirmation, login, JWT/refresh sessions, roles and permissions, password recovery, Google authentication.
- Organization: company creation, editing, activation and current-company context.
- Product Catalog: products, categories, units of measure, brands, barcodes, product variants, pricing and tax configuration.
- Angular: bilingual English/Arabic authentication and administration UI with a responsive side menu.

## Local services

```text
Identity API      https://localhost:7054
Organization API  https://localhost:7043
Inventory API     https://localhost:7274
Gateway           https://localhost:7290
Angular           http://localhost:4200
```

Identity, Gateway, Organization and Inventory must use the same Base64 `Jwt:SigningKey` through .NET User Secrets. The Angular application communicates with services through the Gateway only.
