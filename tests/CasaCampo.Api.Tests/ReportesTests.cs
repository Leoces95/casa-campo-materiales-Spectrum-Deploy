using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CasaCampo.Api.Tests;

public class ReportesTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _cliente;

    public ReportesTests(WebApplicationFactory<Program> fabrica) => _cliente = fabrica.CreateClient();

    [Fact]
    public async Task StockBajo_IncluyeZincClavosYBalastro()
    {
        // En los datos semilla el zinc, los clavos y el balastro están por debajo del mínimo.
        var pendientes = await (await _cliente.GetAsync("/api/reportes/stock-bajo")).LeerAsync<List<StockBajoDto>>();

        Assert.Contains(pendientes, p => p.Nombre.StartsWith("Lámina de zinc"));
        Assert.Contains(pendientes, p => p.Nombre.StartsWith("Clavos"));
        Assert.Contains(pendientes, p => p.Nombre == "Balastro");
        Assert.All(pendientes, p => Assert.True(p.SugeridoComprar > 0));
    }

    [Fact]
    public async Task StockBajo_AparecenMaterialesQueBajanDelMinimo()
    {
        var material = await _cliente.CrearMaterialAsync(stockInicial: 10, stockMinimo: 5);
        await _cliente.PostAsJsonAsync($"/api/materiales/{material.Id}/salidas", new { cantidad = 6, destino = "Columnas" });

        var pendientes = await (await _cliente.GetAsync("/api/reportes/stock-bajo")).LeerAsync<List<StockBajoDto>>();

        Assert.Contains(pendientes, p => p.Id == material.Id);
    }

    [Fact]
    public async Task Resumen_CalculaValorDelInventario()
    {
        var resumen = await (await _cliente.GetAsync("/api/reportes/resumen")).LeerAsync<ResumenDto>();

        Assert.True(resumen.TotalMateriales >= 11);
        Assert.True(resumen.ValorInventario > 0);
        Assert.True(resumen.MaterialesConStockBajo >= 3);
        Assert.NotEmpty(resumen.PorCategoria);
    }

    private record StockBajoDto(int Id, string Nombre, decimal Stock, decimal StockMinimo, decimal SugeridoComprar);
    private record CategoriaDto(string Categoria, int Materiales, decimal Valor);
    private record ResumenDto(int TotalMateriales, decimal ValorInventario, int MaterialesConStockBajo, List<CategoriaDto> PorCategoria);
}
