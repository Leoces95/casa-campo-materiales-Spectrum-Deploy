using CasaCampo.Api.Data;
using CasaCampo.Api.Models;

namespace CasaCampo.Api.Endpoints;

/// <summary>Datos para crear o actualizar un material.</summary>
public record MaterialRequest(
    string? Nombre,
    CategoriaMaterial? Categoria,
    string? Unidad,
    decimal PrecioUnitario,
    decimal StockInicial,
    decimal StockMinimo,
    string? Proveedor);

/// <summary>CRUD de materiales: /api/materiales</summary>
public static class MaterialesEndpoints
{
    public static void MapMateriales(this WebApplication app)
    {
        var grupo = app.MapGroup("/api/materiales").WithTags("Materiales");

        // Listar, con filtros opcionales: ?categoria=Madera&buscar=pino
        grupo.MapGet("/", (InventarioStore store, CategoriaMaterial? categoria, string? buscar) =>
        {
            var materiales = store.ListarMateriales().AsEnumerable();
            if (categoria is not null)
                materiales = materiales.Where(m => m.Categoria == categoria);
            if (!string.IsNullOrWhiteSpace(buscar))
                materiales = materiales.Where(m => m.Nombre.Contains(buscar.Trim(), StringComparison.OrdinalIgnoreCase));
            return Results.Ok(materiales);
        });

        grupo.MapGet("/{id:int}", (int id, InventarioStore store) =>
            store.BuscarMaterial(id) is { } material ? Results.Ok(material) : Results.NotFound());

        grupo.MapPost("/", (MaterialRequest request, InventarioStore store) =>
        {
            var errores = Validar(request, esNuevo: true);
            if (errores.Count > 0) return Results.ValidationProblem(errores);
            if (store.ExisteNombre(request.Nombre!))
                return Results.Conflict(new { error = $"Ya existe un material llamado '{request.Nombre!.Trim()}'" });

            var material = store.AgregarMaterial(new Material
            {
                Nombre = request.Nombre!.Trim(),
                Categoria = request.Categoria!.Value,
                Unidad = request.Unidad!.Trim(),
                PrecioUnitario = request.PrecioUnitario,
                Stock = request.StockInicial,
                StockMinimo = request.StockMinimo,
                Proveedor = request.Proveedor?.Trim()
            });
            return Results.Created($"/api/materiales/{material.Id}", material);
        });

        // Actualiza los datos del material. El stock NO se edita aquí:
        // solo cambia con entradas y salidas para que quede trazabilidad.
        grupo.MapPut("/{id:int}", (int id, MaterialRequest request, InventarioStore store) =>
        {
            var material = store.BuscarMaterial(id);
            if (material is null) return Results.NotFound();

            var errores = Validar(request, esNuevo: false);
            if (errores.Count > 0) return Results.ValidationProblem(errores);
            if (store.ExisteNombre(request.Nombre!, excluirId: id))
                return Results.Conflict(new { error = $"Ya existe un material llamado '{request.Nombre!.Trim()}'" });

            material.Nombre = request.Nombre!.Trim();
            material.Categoria = request.Categoria!.Value;
            material.Unidad = request.Unidad!.Trim();
            material.PrecioUnitario = request.PrecioUnitario;
            material.StockMinimo = request.StockMinimo;
            material.Proveedor = request.Proveedor?.Trim();
            return Results.Ok(material);
        });

        // No se elimina un material con movimientos: se perdería el historial de la obra.
        grupo.MapDelete("/{id:int}", (int id, InventarioStore store) =>
        {
            if (store.BuscarMaterial(id) is null) return Results.NotFound();
            if (store.TieneMovimientos(id))
                return Results.Conflict(new { error = "El material tiene movimientos registrados y no se puede eliminar" });

            store.EliminarMaterial(id);
            return Results.NoContent();
        });
    }

    private static Dictionary<string, string[]> Validar(MaterialRequest request, bool esNuevo)
    {
        var errores = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(request.Nombre))
            errores["nombre"] = ["El nombre es obligatorio"];
        if (request.Categoria is null)
            errores["categoria"] = ["La categoría es obligatoria (Mamposteria, Madera, Cubierta, Ferreteria, Agregados, Aglomerantes)"];
        if (string.IsNullOrWhiteSpace(request.Unidad))
            errores["unidad"] = ["La unidad es obligatoria (unidad, m3, bulto, kg, libra, lámina...)"];
        if (request.PrecioUnitario < 0)
            errores["precioUnitario"] = ["El precio no puede ser negativo"];
        if (request.StockMinimo < 0)
            errores["stockMinimo"] = ["El stock mínimo no puede ser negativo"];
        if (esNuevo && request.StockInicial < 0)
            errores["stockInicial"] = ["El stock inicial no puede ser negativo"];
        return errores;
    }
}
