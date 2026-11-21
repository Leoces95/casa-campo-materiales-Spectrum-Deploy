# Registro de incidencias

Problemas que tuvimos durante la práctica y cómo los resolvimos.

| Fecha | Fase | Qué pasó | Por qué | Cómo se resolvió | Quién |
|---|---|---|---|---|---|
| 16/11/2026 | CI | Al crear la pipeline CasaCampo-CI salió el error «Unable to configure a service on the selected GitHub repository». | Paulina conectó GitHub con su propia cuenta. Esa conexión intenta crear un webhook en el repositorio y, como el repo es de la cuenta personal de Leonel, solo él puede hacerlo. | Leonel instaló la app Azure Pipelines en el repo, lo que creó la conexión `github.com_Leoces95`, y le dio permiso a Paulina en esa conexión. Con ella la pipeline se creó sin problema. | Paulina y Leonel |
| 19/11/2026 | GitHub | Al subir el commit 9, GitHub rechazó el push. | La copia local estaba desactualizada: no tenía los commits 6, 7 y 8 de los compañeros. | Se hizo `git pull`, que generó el commit «Merge branch 'main'», y después el push. Desde ahí hacemos `git pull` antes de cada commit. | Paulina |
| 20/11/2026 | CD | Al crear la pipeline CasaCampo-CD salió el mismo error del webhook. | La misma causa del 16/11. | Leonel le dio permiso a Mariana en la conexión `github.com_Leoces95`. | Mariana y Leonel |
| 20/11/2026 | Azure | Al crear el App Service, el portal solo ofrecía .NET 10 y 11, no .NET 8. | .NET 8 dejó de tener soporte el 10/11/2026. | Se creó el App Service con .NET 10, en la CD se puso `runtimeStack: DOTNETCORE\|10.0` y se agregó la variable `DOTNET_ROLL_FORWARD=Major` para que la API, hecha en .NET 8, corra sobre .NET 10. | Mariana |
| 20/11/2026 | Azure | El formulario del portal falló al crear el App Service. | Mariana es Propietaria solo del grupo `rg-casacampo`, y el portal intentaba crear el recurso a nivel de toda la suscripción. | Lo creó con Azure CLI desde Cloud Shell (ver `docs/azure-app-service.md`). | Mariana |
| 20/11/2026 | GitHub | En el pull request #2 se subieron por error los guiones de los videos. | No eran parte del repositorio. | Se borraron de la rama antes de aceptar el PR y se usó «Squash and merge», así en `main` solo quedó `docs/pruebas-manuales.http`. | Sebastian y Leonel |

## Lo que aprendimos

- Hacer `git pull` antes de copiar archivos o hacer un commit.
- En un repo de cuenta personal, la conexión con Azure DevOps se hace con la app Azure Pipelines del dueño, y a cada integrante se le da permiso en esa conexión.
- Revisar qué versiones ofrece Azure antes de elegir la versión del proyecto.
- No subir al repositorio nada que no haga parte de la entrega (guiones, notas, contraseñas).
