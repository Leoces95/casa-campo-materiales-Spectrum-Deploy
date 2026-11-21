# Plan de trabajo y distribución del equipo

**Objetivo:** demostrar que un cambio en `main` viaja solo de GitHub → CI (build, pruebas, artefacto) → CD → Azure App Service, y que cada integrante tiene un aporte verificable.

## Roles

| Integrante | Rol | Responsable de |
|---|---|---|
| Leonel Martinez (creador del repo) | Gestión de GitHub y documentación | Crear el repo, invitar al equipo y a oalarconpe, README, plan, memoria PDF |
| Daniel Zapata | Desarrollo de la aplicación | Proyecto API, modelos, CRUD de materiales, entradas y salidas |
| Paulina Vargas | Configuración de CI | Pipeline de CI, conexión GitHub ↔ Azure DevOps, artefacto, solución de fallos |
| Mariana Suaza | Configuración de CD y Azure | Reportes, App Service, service connection, pipeline de CD, validación final |
| Sebastian Ciro | Pruebas, evidencias y videos | Pruebas xUnit, pruebas manuales, capturas, preparación y edición de videos |

## Plan de commits (3 por integrante)

Cada integrante hace sus commits **desde su propia cuenta de GitHub**. En cada commit la solución compila y las pruebas pasan.

| # | Integrante | Mensaje | Archivos |
|---|---|---|---|
| 1 | Leonel Martinez | `chore: estructura inicial del repositorio` | `.gitignore`, `global.json`, `README.md`, `evidencias/README.md` |
| 2 | Daniel Zapata | `feat: proyecto API con modelos de materiales y datos semilla` | `CasaCampo.sln`, `src/CasaCampo.Api/` (Program, Models, Data) |
| 3 | Daniel Zapata | `feat: CRUD de materiales de construcción` | `Endpoints/MaterialesEndpoints.cs`, `Program.cs` |
| 4 | Sebastian Ciro | `test: pruebas xUnit del CRUD de materiales` | `tests/`, `CasaCampo.sln` |
| 5 | Paulina Vargas | `ci: pipeline de CI con restore, build y pruebas en main` | `pipelines/azure-pipelines-ci.yml` |
| 6 | Daniel Zapata | `feat: entradas y salidas de bodega con control de stock` | `Endpoints/MovimientosEndpoints.cs`, `Program.cs` |
| 7 | Mariana Suaza | `feat: reportes de stock bajo y resumen del inventario` | `Endpoints/ReportesEndpoints.cs`, `Program.cs` |
| 8 | Sebastian Ciro | `test: pruebas de movimientos y reportes` | `tests/.../MovimientosTests.cs`, `ReportesTests.cs` |
| 9 | Paulina Vargas | `ci: publicar artefacto drop para la CD` | `pipelines/azure-pipelines-ci.yml` |
| 10 | Leonel Martinez | `docs: plan de trabajo y distribución del equipo` | `docs/plan-de-trabajo.md` |
| 11 | Mariana Suaza | `cd: pipeline que despliega el artefacto de CI en App Service` | `pipelines/azure-pipelines-cd.yml` |
| 12 | Sebastian Ciro | `docs: pruebas manuales de la API` | `docs/pruebas-manuales.http` |
| 13 | Paulina Vargas | `docs: registro de incidencias de CI/CD` | `docs/incidencias.md` |
| 14 | Mariana Suaza | `docs: configuración de Azure App Service y manejo de secretos` | `docs/azure-app-service.md` |
| 15 | Leonel Martinez | `docs: README completo con aplicación, ejecución y flujo CI/CD` | `README.md`, `docs/plan-de-trabajo.md` |

**Commits en vivo y de cierre** (se hacen a mano, después de los anteriores):

| # | Integrante | Mensaje | Cuándo |
|---|---|---|---|
| 16 | Daniel Zapata | `feat: versión 1.1.0` (cambia `AppVersion` en `Program.cs`) | En vivo, Video 1 (CI) |
| 17 | Mariana Suaza | `feat: versión 2.0.0 con cal hidratada en el inventario` | En vivo, Video 2 (CD) |
| 18 | Sebastian Ciro | `docs: evidencias de CI y CD` | Después de grabar |
| 19 | Leonel Martinez | `docs: README final con URL pública, ejecuciones y videos` | Cierre |

Para trabajar sin conflictos: `git pull` antes de empezar, cambiar solo los archivos propios y hacer `git push` a `main`, siempre en el orden de la tabla.

## Cuándo se configura Azure (cronograma)

No todos los commits necesitan Azure. Los commits 1 a 4 son código y se prueban en local. Azure DevOps entra con el commit 5 y Azure App Service con el commit 11. Desde el commit 5, cada push que toque código o pipelines dispara la CI sola. Desde el commit 11, cada CI exitosa dispara la CD y publica en Azure. **Antes de grabar, los 15 commits deben estar subidos y CI → CD → URL funcionando.** En los videos solo se hacen en vivo los commits 16 y 17.

| Momento | Qué se configura | Quién |
|---|---|---|
| Hoy, antes o justo después del commit 1 | Leonel crea la organización `spectrum-deploy-itm` y el proyecto privado **Spectrum-Deploy** en Azure DevOps (https://dev.azure.com/spectrum-deploy-itm) e invita al equipo. Al crearla la vincula a su suscripción Azure for Students (*billing*), y con eso se activa sola la capa gratuita de paralelismo: 1 parallel job Microsoft-hosted, 1.800 min/mes. Ya no existe el formulario de solicitud. Paulina lo verifica en *Organization settings → Pipelines → Parallel jobs*. La suscripción Azure for Students es la de Leonel: le asigna a Mariana el rol **Propietario (Owner)** en el grupo de recursos `rg-casacampo` (Control de acceso IAM). | Leonel Martinez y Paulina Vargas |
| Commits 1 a 4 | Nada de Azure. Repo en GitHub, colaboradores (incluido oalarconpe), código y `dotnet test` en local. | Cada autor |
| Después del commit 5 | Azure DevOps → Pipelines → New pipeline → GitHub → autorizar → elegir el repo → Existing YAML → `/pipelines/azure-pipelines-ci.yml` → Run. Renombrar la pipeline a **CasaCampo-CI**. Desde aquí los commits 6, 7, 8, 9 y 11 disparan la CI solos (primera evidencia de CI). | Paulina Vargas |
| Entre los commits 9 y 11 | Portal de Azure (suscripción de Leonel, grupo `rg-casacampo`): Web App Linux `casacampo-spectrum-deploy` con pila .NET 10 (el portal ya no ofrece .NET 8) y la variable `DOTNET_ROLL_FORWARD=Major` para ejecutar la API de .NET 8. Azure DevOps: service connection `sc-azure-casacampo` y environment `production` (ver `docs/azure-app-service.md`). Poner el nombre y la URL reales en el YAML de CD antes de subirlo. | Mariana Suaza |
| Después del commit 11 | Azure DevOps → New pipeline → Existing YAML → `/pipelines/azure-pipelines-cd.yml`. Renombrarla **CasaCampo-CD**. Ejecutarla una vez a mano y pulsar *Permit* en los permisos. Desde aquí cada CI exitosa en main despliega sola. | Mariana Suaza |
| Commits 12 a 15 | Solo documentación: no disparan la CI (exclusión de rutas a propósito). Completar datos reales de Azure en los documentos. | Cada autor |
| Antes de grabar | Ensayo completo: abrir la URL pública, probar los endpoints con `docs/pruebas-manuales.http` y confirmar que la última run de CD terminó en verde. La URL debe mostrar la versión 1.0.0. | Todo el equipo |
| Grabación Video 1 | Commit 16 en vivo (versión 1.1.0) → la CI arranca sola. | Daniel Zapata |
| Grabación Video 2 | Commit 17 en vivo (versión 2.0.0 + cal hidratada) → CI → CD → URL pública. | Mariana Suaza |
| Cierre | Commit 18 (evidencias) y commit 19 (README final con URL, ejecuciones y videos). Memoria PDF y entrega en Teams. | Sebastian Ciro y Leonel Martinez |

## Aportes en Azure DevOps y Azure (no son commits)

La configuración de Azure DevOps y del portal de Azure no queda en GitHub. Su evidencia son las capturas en `evidencias/`, el historial de ejecuciones de Azure DevOps (que muestra quién creó y ejecutó cada pipeline), los videos y la memoria PDF.

| Integrante | Tareas en Azure DevOps / Azure | Evidencia |
|---|---|---|
| Leonel Martinez | Crear la organización y el proyecto **Spectrum-Deploy** en Azure DevOps, invitar al equipo y autorizar la conexión GitHub ↔ Azure Pipelines (como dueño del repo) | Capturas del proyecto, de sus miembros y de la conexión con GitHub |
| Daniel Zapata | Probar la API sobre la URL pública. Configurar Health check `/health` en App Service → Monitoring. Revisar Log stream si algo falla | Capturas de los endpoints en Azure y del Health check |
| Paulina Vargas | Verificar la capa gratuita de paralelismo (Parallel jobs). Crear y renombrar **CasaCampo-CI**. Revisar ejecuciones y resolver fallos | Capturas de la pipeline, de las runs con su commit y del artefacto drop |
| Mariana Suaza | Crear el App Service, la service connection `sc-azure-casacampo`, el environment `production` y **CasaCampo-CD**. Autorizar los permisos | Capturas del recurso, la service connection, el environment y el despliegue |
| Sebastian Ciro | Revisar la pestaña Tests de cada run y el historial de despliegues del environment. Tomar y organizar todas las capturas | Carpeta `evidencias/` (commit 18) y los videos |

## Memoria técnica PDF

| Sección | Redacta |
|---|---|
| Portada, equipo y distribución del trabajo | Leonel Martinez |
| Descripción de la aplicación | Daniel Zapata |
| Pruebas automatizadas y manuales | Sebastian Ciro |
| Configuración de CI (YAML, ejecuciones, artefacto) | Paulina Vargas |
| Configuración de CD y Azure (recurso, service connection, despliegue) | Mariana Suaza |
| Evidencias y trazabilidad commit → CI → CD → URL | Sebastian Ciro |
| Problemas encontrados y solución | Todos (desde `docs/incidencias.md`) |
| Enlaces: repo, videos, URL pública | Leonel Martinez |
