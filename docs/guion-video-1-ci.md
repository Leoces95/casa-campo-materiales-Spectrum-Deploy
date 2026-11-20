# Guion – Video 1: Integración Continua (CI)

**Duración objetivo:** 8 a 10 minutos.
**Reglas:** hablan los 5 integrantes, cada uno se presenta con su nombre completo y su rol antes de su parte, y se muestran pantallas reales de GitHub y Azure DevOps (no solo diapositivas).

**Antes de grabar:**
- La pipeline **CasaCampo-CI** ya existe en Azure DevOps y tiene al menos una ejecución exitosa.
- Pestañas abiertas: README en GitHub, historial de commits, Azure DevOps → Pipelines, VS Code.
- El commit 16 (versión 1.1.0) **no** está hecho todavía: se hace en vivo en la escena 6.

| Escena | Tiempo | Habla | Pantalla | Qué hace y qué dice |
|---|---|---|---|---|
| 1. Presentación | 0:00–1:00 | Leonel Martinez | README en GitHub | «Somos el equipo Spectrum-Deploy. Construimos una API en .NET 8 para controlar los materiales de construcción de una casa de campo: bloques, madera, zinc, clavos, arena, balastro. El objetivo es mostrar cómo un commit en main activa sola la CI en Azure DevOps y genera el artefacto que después despliega la CD.» Muestra la tabla de integrantes. |
| 2. Repositorio | 1:00–2:00 | Leonel Martinez | GitHub: rama main, Commits, Settings → Collaborators | Muestra que se trabaja en main, los 3 commits de cada integrante y que oalarconpe tiene acceso. |
| 3. Aplicación | 2:00–3:30 | Daniel Zapata | VS Code, terminal y navegador | Explica materiales, entradas/salidas y reportes. Ejecuta `dotnet run --project src/CasaCampo.Api`, abre `/api/materiales?categoria=Cubierta` y registra una salida de bloques para «Muro de la cocina» con `docs/pruebas-manuales.http`. |
| 4. Pruebas | 3:30–4:15 | Sebastian Ciro | Carpeta `tests/` y terminal | Explica qué validan las pruebas (CRUD, stock insuficiente, reportes). Ejecuta `dotnet test`: 19 correctas. «Si una prueba falla, la CI falla y no hay artefacto.» |
| 5. Pipeline YAML | 4:15–5:30 | Paulina Vargas | `pipelines/azure-pipelines-ci.yml` y la pipeline en Azure DevOps | Explica el trigger en main, cada paso (SDK, restore, build, test, publish zip, artefacto drop), la conexión con GitHub y por qué los cambios que solo tocan docs no disparan la CI. |
| 6. Cambio controlado | 5:30–6:30 | Daniel Zapata | VS Code → GitHub | Cambia `AppVersion` de `1.0.0` a `1.1.0` en `Program.cs`, hace el commit `feat: versión 1.1.0` y push. Muestra el hash del commit en GitHub. **No toca Azure DevOps.** |
| 7. Activación automática | 6:30–8:00 | Paulina Vargas | Azure DevOps → CasaCampo-CI → Runs | Muestra la ejecución nueva iniciada sola con el mismo hash. Recorre cada paso en verde y la pestaña Tests con las 19 pruebas. |
| 8. Artefacto | 8:00–8:45 | Mariana Suaza | Run → «1 published» → drop | Muestra `CasaCampo.Api.zip` y explica que la CD lo descarga y despliega sin recompilar. |
| 9. Cierre | 8:45–9:30 | Leonel Martinez | `evidencias/` y `docs/incidencias.md` | Resume el flujo y cuenta un problema real que tuvieron y cómo lo resolvieron. |

**Grabación:** OBS Studio o grabación de una reunión de Teams. Si la ejecución tarda, corten la espera en la edición sin ocultar el hash del commit. Subir a YouTube como «No listado».
