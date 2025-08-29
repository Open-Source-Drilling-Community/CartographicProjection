# Service

The Service project hosts the CartographicProjection HTTP API. It exposes endpoints to manage projections and conversion sets and serves an OpenAPI document and Swagger UI. Data persist in a local SQLite database under `home/CartographicProjection.db`.

## Purpose

- Expose REST endpoints for `CartographicProjection`, `CartographicConversionSet`, and `CartographicProjectionType`.
- Serve a merged OpenAPI schema at `CartographicProjection/api/swagger` and static assets from `wwwroot`.
- Persist data in SQLite; maintain and periodically clean the database.

## Install & Run

- Prerequisites: .NET SDK 8.0, Docker (optional), internet access to dependency services if configured.

- Local run (dev):
  - Restore tools for Swagger CLI: `dotnet tool restore`
  - Build and run: `dotnet run --project Service/Service.csproj`
  - Swagger UI: `http://localhost:8080/CartographicProjection/api/swagger`

- Configuration:
  - `GeodeticDatumHostURL`: base URL to the GeodeticDatum service (used by `APIUtils`).
    - Dev default: `Service/appsettings.Development.json:1`
    - Prod default: `Service/appsettings.Production.json:1`
  - Override via environment variable `GeodeticDatumHostURL` or in `appsettings.*.json`.

- Docker:
  - Build: `docker build -t norcedrillingcartographicprojectionservice -f Service/Dockerfile .`
  - Run: `docker run -p 8080:8080 -e GeodeticDatumHostURL=https://dev.digiwells.no/ -v %CD%/home:/app/../home norcedrillingcartographicprojectionservice`
    - On Linux/macOS, replace `%CD%` with `$(pwd)`.
    - Volume maps the SQLite DB to `home/CartographicProjection.db` in your workspace.

## Usage Examples

- Base path: `http://localhost:8080/CartographicProjection/api`
- Swagger UI: `/swagger`

- CartographicProjectionType (enumeration and prototypes):
  - `GET /CartographicProjectionType` → list of supported `ProjectionType` values
  - `GET /CartographicProjectionType/{id}` → prototype for a given type
  - `GET /CartographicProjectionType/HeavyData` → full list with flags

- CartographicProjection (CRUD):
  - `GET /CartographicProjection` → list of IDs
  - `GET /CartographicProjection/MetaInfo` → list of `MetaInfo`
  - `GET /CartographicProjection/{id}` → single item
  - `GET /CartographicProjection/LightData` / `HeavyData` → lightweight/heavy lists
  - `POST /CartographicProjection` → create
  - `PUT /CartographicProjection/{id}` → update
  - `DELETE /CartographicProjection/{id}` → delete

- CartographicConversionSet (CRUD + compute on add/update):
  - `GET /CartographicConversionSet` / `MetaInfo` / `{id}` / `LightData` / `HeavyData`
  - `POST /CartographicConversionSet` → add and compute
  - `PUT /CartographicConversionSet/{id}` → recompute and update
  - `DELETE /CartographicConversionSet/{id}`

- Minimal request payloads (schemas in `Model`):

```json
// CartographicProjection (minimal skeleton)
{
  "MetaInfo": { "ID": "c0ffee00-0000-0000-0000-000000000001" },
  "Name": "UTM 32N",
  "ProjectionType": "UTM",
  "Zone": 32,
  "IsSouth": false
}
```

```json
// CartographicConversionSet (minimal skeleton)
{
  "MetaInfo": { "ID": "c0ffee00-0000-0000-0000-000000000002" },
  "Name": "Demo conversion",
  "CartographicProjectionID": "c0ffee00-0000-0000-0000-000000000001",
  "CartographicCoordinateList": [
    { "Northing": 6740000.0, "Easting": 300000.0, "VerticalDepth": 0.0 }
  ]
}
```

Consult Swagger UI for the authoritative schema and examples.

## Dependencies

- Project: references `Model` (`Service/Service.csproj:1`) for conversion logic and data types.
- Packages: `Microsoft.Data.Sqlite`, `Microsoft.OpenApi`, `Swashbuckle.AspNetCore.SwaggerGen`/`SwaggerUI`.
- Tools: `swashbuckle.aspnetcore.cli` (via `.config/dotnet-tools.json:1`) to export OpenAPI during debug builds.

## Integration

- Model: Implements conversion algorithms (DotSpatial) and data types consumed by controllers.
- ModelSharedIn: Provides generated DTOs used inside `Model` (e.g., `GeodeticDatum`/`Spheroid`).
- ModelSharedOut: Generated from Service OpenAPI; places merged OpenAPI at `Service/wwwroot/json-schema/CartographicProjectionMergedModel.json` and produces a C# client for consumers.
- WebApp: Calls Service endpoints using the `ModelSharedOut` client; does not reference `Model` directly.

## Operations & Maintenance

- Database: stored in `home/CartographicProjection.db` (auto‑created). `DatabaseCleanerService` removes old records daily.
- Reverse proxy: `Program.cs` handles `X-Forwarded-Host` to generate correct Swagger `servers` URLs.

## Deployed Endpoints

- Swagger (dev): https://dev.digiwells.no/CartographicProjection/api/swagger
- Swagger (prod): https://app.digiwells.no/CartographicProjection/api/swagger
- Example API (dev): https://dev.digiwells.no/CartographicProjection/api/CartographicConversionSet
- Example API (prod): https://app.digiwells.no/CartographicProjection/api/CartographicConversionSet

## Source & Template

- Generated from NORCE Drilling and Well Modelling .NET template.
- Template repo: https://github.com/NORCE-DrillingAndWells/Templates
- Template docs: https://github.com/NORCE-DrillingAndWells/DrillingAndWells/wiki/.NET-Templates

## Funding

The current work has been funded by the Research Council of Norway and Industry partners in the SFI Digiwells (2020–2028) centre for research‑based innovation.

## Contributors

- Eric Cayeux — NORCE Energy Modelling and Automation
- Gilles Pelfrene — NORCE Energy Modelling and Automation
- Andrew Holsaeter — NORCE Energy Modelling and Automation
- Lucas Volpi — NORCE Energy Modelling and Automation

