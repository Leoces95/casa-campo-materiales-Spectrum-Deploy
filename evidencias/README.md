# Evidencias

Capturas del proceso, numeradas en el orden en que ocurrieron.

## Pruebas y ejecución local (`pruebas/`)

| Archivo | Qué muestra |
|---|---|
| `01-pruebas-materiales-9-verde.png` | Primeras 9 pruebas del CRUD de materiales en verde (commit 4) |
| `02-estructura-proyecto-pruebas.png` | Estructura del proyecto de pruebas |
| `03-pruebas-19-verde.png` | Las 19 pruebas en verde (commit 8) |
| `04-pr-1-movimientos-reportes.png` | Pull request #1 con las pruebas de movimientos y reportes |
| `05-api-local-respuesta-json.png` | Respuesta JSON de la API ejecutada en local |
| `06-api-local-consola.png` | Consola de la API en local |

## Azure y entrega continua (CD)

| Archivo | Qué muestra |
|---|---|
| `01-app-service-antes-del-despliegue.png` | App Service recién creado, antes del primer despliegue |
| `02-app-service-resumen.png` | Resumen del App Service: Linux, .NET 10, plan F1, Brazil South y URL |
| `03-app-service-variable-roll-forward.png` | Variable `DOTNET_ROLL_FORWARD=Major` en el App Service |
| `04-service-connection.png` | Service connection `sc-azure-casacampo` (sin datos sensibles) |
| `05-primera-cd-manual-20261120.jpeg` | Primera ejecución de CasaCampo-CD, lanzada al crear la pipeline |
| `06-log-despliegue-run-20261120-1.png` | Log del primer despliegue |
| `07-cd-automatica-tras-ci-v1-1-1.png` | CD disparada automáticamente por la CI del commit 16 (versión 1.1.0) |
| `08-historial-runs-cd.png` | Historial de ejecuciones de CasaCampo-CD |
| `09-log-despliegue-run-20261122-2.png` | Log del despliegue del cambio 2.0.0 |
| `10-app-publica-reporte-stock-bajo.png` | Aplicación publicada respondiendo el reporte de stock bajo |

## Conexión GitHub ↔ Azure DevOps

| Archivo | Qué muestra |
|---|---|
| `11-conexion-github-azure-devops.png` | Service connections del proyecto: `github.com_Leoces95` (app Azure Pipelines) y `sc-azure-casacampo` |
| `12-permisos-conexion-github.png` | Permisos sobre `github.com_Leoces95`: Paulina y Mariana como administradoras para crear sus pipelines |

## Integración continua (CI)

| Archivo | Qué muestra |
|---|---|
| `13-historial-runs-ci.png` | Historial de CasaCampo-CI: cada commit de código en main disparó la CI («Individual CI») con su hash |
| `14-run-ci-commit-17.png` | Run #20261122.2 del commit `a3acdca` (versión 2.0.0): Individual CI, 100 % de pruebas y 1 artefacto publicado |
| `15-artefacto-drop.png` | Artefacto `drop` con `CasaCampo.Api.zip` (67 KB), el paquete que consume la CD |
| `16-tests-ci.png` | Pestaña Tests de esa run: 19 pruebas superadas, 0 fallidas |

## Entrega continua: historial y corrección

| Archivo | Qué muestra |
|---|---|
| `17-historial-runs-cd.png` | Historial de CasaCampo-CD: cada despliegue disparado automáticamente por una run de CI |
| `18-cd-verde-disparada-por-ci.png` | CasaCampo-CD en verde, disparada automáticamente por la CI `ci/20261123.1` (commit `33a36705`), con la espera y los reintentos en la comprobación de salud |
