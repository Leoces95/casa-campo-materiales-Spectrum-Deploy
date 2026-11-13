# Evidencias

Capturas legibles (PNG) nombradas con número y descripción, p. ej. `01-ci-trigger-commit.png`.

## ci/
1. Repositorio en GitHub con la rama main e historial de commits.
2. Usuario oalarconpe como colaborador con acceso aceptado.
3. Pipeline CasaCampo-CI conectada al repositorio de GitHub.
4. Ejecución disparada por el commit (se ve «Triggered by» y el hash).
5. Pasos de la ejecución en verde.
6. Pestaña Tests con las pruebas superadas.
7. Artefacto drop publicado.

## cd/
1. App Service en el portal (Linux, .NET 8, plan, región, URL).
2. Service connection en Azure DevOps (sin mostrar datos sensibles).
3. Pipeline CasaCampo-CD disparada por la CI (mismo commit).
4. Paso «Trazabilidad del artefacto» con el run de CI y el commit.
5. Log del despliegue exitoso.
6. URL pública mostrando la versión nueva y /health respondiendo ok.
