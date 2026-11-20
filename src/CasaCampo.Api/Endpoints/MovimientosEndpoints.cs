using CasaCampo.Api.Data;
using CasaCampo.Api.Models;

namespace CasaCampo.Api.Endpoints;

/// <summary>Datos de una entrada o salida. Destino: parte de la obra (ej. "Muro cocina", "Techo").</summary>
public record MovimientoRequest(decimal Cantidad, string? Destino, string? Nota);

/// <summary>Entradas y salidas de bodega.</summary>
public static class MovimientosEndpoints
{
    public static void MapMovimientos(this WebApplication app)
    {
        app.MapPost("/api/materiales/{id:int}/entradas", (int id, MovimientoRequest request, InventarioStore store) =>
            Registrar(id, TipoMovimiento.Entrada, request, store)).WithTags("Movimientos");

        app.MapPost("/api/materiales/{id:int}/salidas", (int id, MovimientoRequest request, InventarioStore store) =>
            Registrar(id, TipoMovimiento.Salida, request, store)).WithTags("Movimientos");

        // Historial, con filtros opcionales: ?materialId=5&tipo=Salida
        app.MapGet("/api/movimientos", (InventarioStore store, int? materialId, TipoMovimiento? tipo) =>
        {
            var movimientos = store.ListarMovimientos().AsEnumerable();
            if (materialId is not null) movimientos = movimientos.Where(m => m.MaterialId == materialId);
            if (tipo is not null) movimientos = movimientos.Where(m => m.Tipo == tipo);
            return Results.Ok(movimientos);
        }).WithTags("Movimientos");
    }

    private static IResult Registrar(int id, TipoMovimiento tipo, MovimientoRequest request, InventarioStore store)
    {
        var material = store.BuscarMaterial(id);
        if (material is null) return Results.NotFound();

        if (request.Cantidad <= 0)
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["cantidad"] = ["La cantidad debe ser mayor que cero"]
            });

        if (tipo == TipoMovimiento.Salida && string.IsNullOrWhiteSpace(request.Destino))
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["destino"] = ["Indique en qué parte de la obra se usa el material"]
            });

        var movimiento = store.RegistrarMovimiento(material, tipo, request.Cantidad, request.Destino, request.Nota);
        if (movimiento is null)
            return Results.Conflict(new
            {
                error = $"Stock insuficiente de {material.Nombre}: hay {material.Stock} {material.Unidad}"
            });

        return Results.Created($"/api/movimientos?materialId={id}", new { movimiento, stockActual = material.Stock });
    }
}
