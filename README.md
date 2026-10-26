# SmartERP foundation

This repository currently contains only the main solution projects and their dependency references. Business entities, repository contracts, services, database contexts, controllers, Docker configuration, and ERP modules will be added during the implementation roadmap.

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

No business implementation has been added yet.
