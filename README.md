# Casa de Campo – Control de Materiales (CI/CD)

Práctica final de **Despliegue de Software**: Integración Continua y Entrega Continua con GitHub, Azure DevOps y Microsoft Azure.

## Objetivo de la práctica

Diseñar, implementar y demostrar un flujo automatizado en el que un cambio confirmado en `main`:

**Código → CI → Artefacto → CD → Aplicación en Azure**

1. **CI (50 %)**: Azure DevOps valida, compila, ejecuta las pruebas y publica un artefacto desplegable.
2. **CD (50 %)**: Azure DevOps toma ese mismo artefacto y lo despliega automáticamente en Azure App Service.

La aplicación es sencilla a propósito: el foco es la automatización, la verificación y la trazabilidad del despliegue.

## Equipo

**Nombre del equipo:** Spectrum-Deploy

| Integrante (nombre completo) | Rol | Aporte en el repositorio (commits) | Aporte en Azure DevOps / Azure |
|---|---|---|---|
| Leonel Antonio Martínez Silgado | Gestión de GitHub y documentación | Creación del repo y estructura inicial, plan de trabajo, README, acceso del docente, documento de evidencias técnicas | Proyecto Spectrum-Deploy en Azure DevOps, conexión GitHub ↔ Azure Pipelines |
| Daniel Zapata Ramírez | Desarrollo de la aplicación | Proyecto base, modelos y datos semilla, CRUD de materiales, entradas y salidas | Verificación de la API en la URL pública, Health check del App Service |
| María Paulina Vargas Lenis | Configuración de CI | Pipeline de CI (build y pruebas), artefacto drop, registro de incidencias | Creación y seguimiento de la pipeline CasaCampo-CI |
| Mariana Suaza Serna | Configuración de CD y Azure | Reportes de inventario, pipeline de CD, App Service y manejo de secretos | App Service, service connection, environment y pipeline CasaCampo-CD |
| Sebastián Ciro Medellín | Pruebas, evidencias y videos | Pruebas xUnit del CRUD, movimientos y reportes, pruebas manuales, evidencias, preparación y edición de videos | Revisión de pruebas en las runs, capturas de CI/CD y videos |

Detalle de tareas y commits: [docs/plan-de-trabajo.md](docs/plan-de-trabajo.md).

## Tecnología

- .NET 8 (ASP.NET Core Minimal API) y xUnit
- GitHub (repositorio, rama principal `main`)
- Azure DevOps Pipelines (CI y CD en YAML)
- Azure App Service (Linux, pila .NET 10; la API compilada en .NET 8 se ejecuta con `DOTNET_ROLL_FORWARD=Major`, ver [docs/azure-app-service.md](docs/azure-app-service.md))

## Aplicación

API REST para registrar y controlar los materiales de construcción de una **vivienda de campo estilo pueblo**. Lleva el inventario de la bodega de la obra, registra lo que entra (compras) y lo que sale hacia cada parte de la casa, y avisa qué materiales hay que volver a comprar.

Arranca con 11 materiales de ejemplo: bloque de cemento, ladrillo tolete, tabla de pino, vara de guadua, lámina de zinc, teja de barro, clavos, alambre, arena de río, balastro y cemento gris.

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/` | Nombre, versión y lista de endpoints |
| GET | `/health` | Comprobación de salud |
| GET | `/api/materiales?categoria=&buscar=` | Lista materiales; filtra por categoría (`Mamposteria`, `Madera`, `Cubierta`, `Ferreteria`, `Agregados`, `Aglomerantes`) o por nombre |
| GET | `/api/materiales/{id}` | Consulta un material |
| POST | `/api/materiales` | Crea un material (nombre único, con stock inicial) |
| PUT | `/api/materiales/{id}` | Actualiza datos y precio; el stock solo cambia con movimientos |
| DELETE | `/api/materiales/{id}` | Elimina un material sin movimientos |
| POST | `/api/materiales/{id}/entradas` | Registra una compra o llegada de material |
| POST | `/api/materiales/{id}/salidas` | Registra material usado en una parte de la obra (valida stock) |
| GET | `/api/movimientos?materialId=&tipo=` | Historial de entradas y salidas |
| GET | `/api/reportes/stock-bajo` | Materiales bajo el mínimo y cuánto comprar |
| GET | `/api/reportes/resumen` | Valor del inventario total y por categoría |

Los datos se guardan en memoria y se reinician con cada despliegue. Así no hace falta base de datos ni cadenas de conexión.

## Ejecución local

Requisito: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
dotnet restore
dotnet build
dotnet test                               # 19 pruebas automatizadas
dotnet run --project src/CasaCampo.Api    # http://localhost:5242
```

Peticiones de ejemplo: [docs/pruebas-manuales.http](docs/pruebas-manuales.http).

## Pruebas automatizadas

El proyecto `tests/CasaCampo.Api.Tests` (xUnit + WebApplicationFactory) prueba el CRUD de materiales, las entradas y salidas (incluida la validación de stock insuficiente) y los reportes. La CI las ejecuta en cada cambio: si una falla, no se genera artefacto y no hay despliegue.

## Flujo CI/CD

1. Un push a `main` dispara **CasaCampo-CI** ([pipelines/azure-pipelines-ci.yml](pipelines/azure-pipelines-ci.yml)): restaura, compila, ejecuta las pruebas y publica el artefacto **drop** (`CasaCampo.Api.zip`). Los cambios que solo tocan `docs/`, `evidencias/` o este README no disparan la CI.
2. Cuando la CI termina bien en `main`, se dispara **CasaCampo-CD** ([pipelines/azure-pipelines-cd.yml](pipelines/azure-pipelines-cd.yml)) mediante `resources.pipelines`.
3. La CD descarga el mismo artefacto drop sin recompilar. Registra el run y el commit de origen, lo despliega con `AzureWebApp@1` y comprueba `/health` y `/api/reportes/resumen` en la URL pública.

Configuración del App Service y manejo de secretos: [docs/azure-app-service.md](docs/azure-app-service.md).

## Enlaces y evidencias

- Repositorio: https://github.com/Leoces95/casa-campo-materiales-Spectrum-Deploy (rama `main`)
- URL pública: https://casacampo-spectrum-deploy.azurewebsites.net
- Ejecuciones de CI y CD (pipelines CasaCampo-CI y CasaCampo-CD): https://dev.azure.com/spectrum-deploy-itm/Spectrum-Deploy/_build
- Historial de despliegues (environment `production`): https://dev.azure.com/spectrum-deploy-itm/Spectrum-Deploy/_environments/1?view=deployments
- Video 1 – Integración Continua (CI): https://youtu.be/9q0rBqOoPFo
- Video 2 – Entrega Continua (CD): https://youtu.be/SygL0fX5UNE
- Capturas: [evidencias/](evidencias/)
- Incidencias y soluciones: [docs/incidencias.md](docs/incidencias.md)
