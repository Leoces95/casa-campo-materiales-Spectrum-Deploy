using System.Text.Json.Serialization;
using CasaCampo.Api.Data;
using CasaCampo.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<InventarioStore>();
builder.Services.ConfigureHttpJsonOptions(opciones =>
    opciones.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

// Versión visible de la aplicación. Se cambia en los "cambios controlados"
// de los videos para demostrar que CI/CD publica la nueva versión en Azure.
const string AppVersion = "1.0.0";

app.MapGet("/", () => Results.Ok(new
{
    app = "Casa de Campo - Control de Materiales",
    version = AppVersion,
    descripcion = "Registro y control de materiales para la construcción de una vivienda de campo",
    endpoints = new[]
    {
        "GET /health",
        "GET|POST /api/materiales",
        "GET|PUT|DELETE /api/materiales/{id}",
        "POST /api/materiales/{id}/entradas",
        "POST /api/materiales/{id}/salidas",
        "GET /api/movimientos"
    }
}));

app.MapGet("/health", () => Results.Ok(new { status = "ok", version = AppVersion }));

app.MapMateriales();
app.MapMovimientos();

app.Run();

// Permite que el proyecto de pruebas use WebApplicationFactory<Program>.
public partial class Program { }
