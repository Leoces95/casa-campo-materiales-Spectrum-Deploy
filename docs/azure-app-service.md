# Azure App Service y manejo de secretos

## Recurso

| Parámetro | Valor |
|---|---|
| Suscripción | Azure for Students (cuenta de Leonel Martinez) |
| Grupo de recursos | `rg-casacampo` |
| Nombre de la Web App | `casacampo-spectrum-deploy` |
| Publicar | Código |
| Pila del entorno de ejecución | .NET 10 (LTS). El portal ya no ofrece .NET 8; la API es .NET 8 y corre con roll-forward |
| Sistema operativo | Linux |
| Plan de App Service y SKU | `asp-casacampo`, F1 gratuito (Linux) |
| Región | Brazil South |
| URL pública | https://casacampo-spectrum-deploy.azurewebsites.net |

El formulario del portal (Crear un recurso → Aplicación web) falló porque el rol de Mariana es Propietario solo sobre `rg-casacampo` y el portal lanza la implementación a nivel de suscripción. El recurso se creó con Azure CLI desde Cloud Shell, con comandos que trabajan dentro del grupo de recursos:

```bash
az account set --subscription <id-suscripcion-leonel>
az appservice plan create --name asp-casacampo --resource-group rg-casacampo --sku F1 --is-linux --location brazilsouth
az webapp create --name casacampo-spectrum-deploy --resource-group rg-casacampo --plan asp-casacampo --runtime DOTNETCORE:10.0
az webapp config appsettings set --name casacampo-spectrum-deploy --resource-group rg-casacampo --settings DOTNET_ROLL_FORWARD=Major
```

No necesita base de datos. Tiene una sola variable de entorno, `DOTNET_ROLL_FORWARD=Major`, que no es secreta: permite ejecutar la API de .NET 8 sobre el runtime .NET 10 del App Service. Por la misma razón, el YAML de CD usa `runtimeStack: DOTNETCORE|10.0`.

## Conexión de Azure DevOps con Azure

1. Azure DevOps → Project settings → Service connections → New → **Azure Resource Manager**.
2. Tipo de identidad: **App registration (automatic)** con credencial **Workload identity federation**.
3. Ámbito: la suscripción y el grupo de recursos `rg-casacampo`.
4. Nombre: `sc-azure-casacampo` (el mismo que usa `pipelines/azure-pipelines-cd.yml`).
5. Pipelines → Environments → New environment → `production`, sin aprobación manual para que el flujo CI → CD sea automático.

## Medidas para no exponer secretos

- El repositorio no guarda tokens, contraseñas, perfiles de publicación (`*.PublishSettings`) ni cadenas de conexión.
- La CD se autentica con la service connection, que vive en Azure DevOps y usa federación de identidad: no hay contraseña que se pueda filtrar.
- El nombre de la service connection y el de la Web App que aparecen en el YAML no son secretos.
- Si en el futuro la app necesitara un dato sensible, iría en App Service → Variables de entorno o en una variable secreta de la pipeline, nunca en `appsettings.json`.
- El `.gitignore` excluye `bin/`, `obj/`, archivos `.user` y perfiles de publicación.
