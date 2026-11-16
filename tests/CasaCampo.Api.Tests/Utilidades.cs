using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CasaCampo.Api.Tests;

/// <summary>DTOs y ayudas compartidas por las pruebas.</summary>
public static class Utilidades
{
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static async Task<T> LeerAsync<T>(this HttpResponseMessage respuesta) =>
        (await respuesta.Content.ReadFromJsonAsync<T>(Json))!;

    /// <summary>Crea un material nuevo con un nombre único y lo devuelve.</summary>
    public static async Task<MaterialDto> CrearMaterialAsync(this HttpClient cliente, decimal stockInicial = 10, decimal stockMinimo = 2)
    {
        var respuesta = await cliente.PostAsJsonAsync("/api/materiales", new
        {
            nombre = $"Material de prueba {Guid.NewGuid():N}",
            categoria = "Ferreteria",
            unidad = "unidad",
            precioUnitario = 1000,
            stockInicial,
            stockMinimo
        });
        respuesta.EnsureSuccessStatusCode();
        return await respuesta.LeerAsync<MaterialDto>();
    }
}

public record MaterialDto(int Id, string Nombre, string Categoria, string Unidad, decimal PrecioUnitario,
    decimal Stock, decimal StockMinimo, string? Proveedor, bool StockBajo, decimal ValorEnBodega);
