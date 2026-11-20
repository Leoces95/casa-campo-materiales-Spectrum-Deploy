# Guion – Video 2: Entrega Continua (CD)

**Duración objetivo:** 8 a 10 minutos.
**Clave:** seguir **un mismo cambio** (commit 17) desde GitHub → CI → CD → URL pública.

**Antes de grabar:**
- App Service creado, service connection `sc-azure-casacampo` y pipeline **CasaCampo-CD** con al menos un despliegue exitoso.
- La URL pública muestra la versión 1.1.0 (la del Video 1) y 11 materiales.
- El commit 17 **no** está hecho todavía: se hace en vivo en la escena 5.

**Cambio del commit 17** (Mariana Suaza):
1. En `src/CasaCampo.Api/Program.cs`: `const string AppVersion = "2.0.0";`
2. En `src/CasaCampo.Api/Data/InventarioStore.cs`, debajo del cemento:
   `Sembrar("Cal hidratada 25 kg", CategoriaMaterial.Aglomerantes, "bulto", 18_000m, 6, 4, "Ferretería Central");`
3. Commit `feat: versión 2.0.0 con cal hidratada en el inventario` y push.

| Escena | Tiempo | Habla | Pantalla | Qué hace y qué dice |
|---|---|---|---|---|
| 1. Presentación | 0:00–0:45 | Leonel Martinez | README | Equipo, objetivo del video y el flujo Código → CI → Artefacto → CD → Azure. |
| 2. Recurso en Azure | 0:45–2:00 | Mariana Suaza | Portal de Azure → App Service | Grupo de recursos, Linux, .NET 8, plan, región y URL. Abre la URL: versión 1.1.0 y 11 materiales. |
| 3. Conexión y secretos | 2:00–2:45 | Mariana Suaza | Azure DevOps → Project settings → Service connections | La autenticación vive en Azure DevOps (workload identity federation); el repositorio no tiene credenciales. |
| 4. Pipeline CD | 2:45–4:00 | Paulina Vargas | `pipelines/azure-pipelines-cd.yml` | Explica `resources.pipelines` (se dispara cuando la CI termina en main), la descarga del artefacto drop, el despliegue y la comprobación final. Si el environment `production` tiene aprobación manual, explica su efecto. |
| 5. Cambio visible | 4:00–5:00 | Mariana Suaza | VS Code → GitHub | Hace el cambio del commit 17 y push. Muestra el hash en GitHub. |
| 6. CI automática | 5:00–6:00 | Paulina Vargas | Azure DevOps → CasaCampo-CI | Ejecución nueva con ese mismo hash, en verde, artefacto drop publicado. |
| 7. CD automática | 6:00–7:30 | Sebastian Ciro | Azure DevOps → CasaCampo-CD | La CD arrancó sola. Muestra el paso «Trazabilidad del artefacto» (run de CI y commit), el log del despliegue y la comprobación de salud. |
| 8. Verificación | 7:30–8:45 | Daniel Zapata | Navegador o `docs/pruebas-manuales.http` con la URL pública | `/` muestra 2.0.0; `/api/materiales?buscar=cal` muestra la cal hidratada; registra una salida de zinc para «Techo» y muestra `/api/reportes/stock-bajo`. |
| 9. Cierre | 8:45–9:30 | Sebastian Ciro | `docs/incidencias.md` y `evidencias/` | Errores encontrados, cómo se resolvieron y resumen final. |
