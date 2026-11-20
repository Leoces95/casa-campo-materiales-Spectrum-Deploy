using CasaCampo.Api.Data;

namespace CasaCampo.Api.Endpoints;

/// <summary>Reportes para el maestro de obra: qué comprar y cuánto vale la bodega.</summary>
public static class ReportesEndpoints
{
    public static void MapReportes(this WebApplication app)
    {
        var grupo = app.MapGroup("/api/reportes").WithTags("Reportes");

        // Materiales en o por debajo del mínimo, con la cantidad sugerida a comprar.
        grupo.MapGet("/stock-bajo", (InventarioStore store) =>
        {
            var pendientes = store.ListarMateriales()
                .Where(m => m.StockBajo)
                .Select(m => new
                {
                    m.Id,
                    m.Nombre,
                    m.Unidad,
                    m.Stock,
                    m.StockMinimo,
                    sugeridoComprar = m.StockMinimo * 2 - m.Stock,
                    costoEstimado = (m.StockMinimo * 2 - m.Stock) * m.PrecioUnitario,
                    m.Proveedor
                })
                .OrderBy(m => m.Nombre);
            return Results.Ok(pendientes);
        });

        // Valor del inventario total y por categoría.
        grupo.MapGet("/resumen", (InventarioStore store) =>
        {
            var materiales = store.ListarMateriales();
            var movimientos = store.ListarMovimientos();
            return Results.Ok(new
            {
                totalMateriales = materiales.Count,
                valorInventario = materiales.Sum(m => m.ValorEnBodega),
                materialesConStockBajo = materiales.Count(m => m.StockBajo),
                totalEntradas = movimientos.Count(m => m.Tipo == Models.TipoMovimiento.Entrada),
                totalSalidas = movimientos.Count(m => m.Tipo == Models.TipoMovimiento.Salida),
                porCategoria = materiales
                    .GroupBy(m => m.Categoria)
                    .Select(g => new { categoria = g.Key, materiales = g.Count(), valor = g.Sum(m => m.ValorEnBodega) })
                    .OrderByDescending(g => g.valor)
            });
        });
    }
}
