# Maresa — API de Registro de Pedidos

API REST en .NET 10 que registra pedidos, valida al cliente contra un servicio externo,
persiste la información en SQL Server dentro de una transacción y registra auditoría del
proceso.

## Arquitectura

Solución organizada en 4 capas (`src/Maresa/Maresa.slnx`):

- **`Maresa.Domain`** — Entidades (`PedidoCabecera`, `PedidoDetalle`, `LogAuditoria`) y
  excepciones de dominio.
- **`Maresa.Application`** — DTOs, validación de entrada, interfaces y `PedidoService`
  (orquestación transaccional).
- **`Maresa.Infrastructure`** — EF Core + SQL Server (`DbContext`, migraciones,
  repositorios) y el cliente HTTP de validación externa.
- **`Maresa.API`** — Controllers, middleware de errores y configuración.
- **`Maresa.Application.Tests`** — Pruebas unitarias de `PedidoService`.

## Requisitos previos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express o cualquier instancia accesible)

## Configuración

La cadena de conexión está en `src/Maresa/Maresa.API/appsettings.json`
(`ConnectionStrings:DefaultConnection`). Por defecto apunta a LocalDB:

```
Server=(localdb)\MSSQLLocalDB;Database=MaresaDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

Ajustala si usás otra instancia de SQL Server.

## Base de datos

Hay dos formas de crear la base de datos y sus tablas (son equivalentes):

**Opción A — Script SQL** (no requiere el SDK de .NET ni el tooling de EF):

```
sqlcmd -S "(localdb)\MSSQLLocalDB" -i db/Maresa.sql
```

El script (`db/Maresa.sql`, exportado con `dotnet ef migrations script --idempotent`) crea
la base `MaresaDb` si no existe y aplica el esquema completo.

**Opción B — Migraciones de EF Core:**

```
cd src/Maresa
dotnet tool restore
dotnet tool run dotnet-ef database update --project Maresa.Infrastructure --startup-project Maresa.API
```

## Ejecutar la API

```
cd src/Maresa/Maresa.API
dotnet run
```

La API queda disponible en `http://localhost:5292`. En entorno `Development` se expone el
documento OpenAPI en `/openapi/v1.json` y una UI interactiva en `/scalar/v1`.

Podés probar los distintos casos (éxito, cliente inválido, datos inválidos) con el archivo
`src/Maresa/Maresa.API/Maresa.API.http`.

### Ejemplo de request

```
POST http://localhost:5292/api/pedidos
Content-Type: application/json

{
  "clienteId": 1,
  "usuario": "usuario.prueba",
  "items": [
    { "productoId": 1, "cantidad": 2, "precio": 10 },
    { "productoId": 2, "cantidad": 1, "precio": 20 }
  ]
}
```

El servicio de validación externo (`https://jsonplaceholder.typicode.com/users/{id}`) solo
tiene usuarios con `id` entre 1 y 10; cualquier otro `clienteId` se trata como cliente
inválido.

## Pruebas unitarias

```
cd src/Maresa
dotnet test Maresa.Application.Tests/Maresa.Application.Tests.csproj
```

## Logging

- Consola (siempre).
- Archivo de texto en `src/Maresa/Maresa.API/logs/maresa.log` (se crea en tiempo de
  ejecución, no versionado).
- Auditoría de negocio (`LogAuditoria`) en la tabla `LogsAuditoria`, independiente del log
  técnico anterior.
