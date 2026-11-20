using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CasaCampo.Api.Tests;

public class MovimientosTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _cliente;

    public MovimientosTests(WebApplicationFactory<Program> fabrica) => _cliente = fabrica.CreateClient();

    [Fact]
    public async Task Entrada_AumentaElStock()
    {
        var material = await _cliente.CrearMaterialAsync(stockInicial: 10);

        var respuesta = await _cliente.PostAsJsonAsync($"/api/materiales/{material.Id}/entradas",
            new { cantidad = 15, nota = "Compra en la ferretería del pueblo" });
        var actual = await (await _cliente.GetAsync($"/api/materiales/{material.Id}")).LeerAsync<MaterialDto>();

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        Assert.Equal(25, actual.Stock);
    }

    [Fact]
    public async Task Salida_DisminuyeElStock()
    {
        var material = await _cliente.CrearMaterialAsync(stockInicial: 10);

        var respuesta = await _cliente.PostAsJsonAsync($"/api/materiales/{material.Id}/salidas",
            new { cantidad = 4, destino = "Muro de la cocina" });
        var actual = await (await _cliente.GetAsync($"/api/materiales/{material.Id}")).LeerAsync<MaterialDto>();

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        Assert.Equal(6, actual.Stock);
    }

    [Fact]
    public async Task Salida_MayorQueElStock_DevuelveConflict()
    {
        var material = await _cliente.CrearMaterialAsync(stockInicial: 3);

        var respuesta = await _cliente.PostAsJsonAsync($"/api/materiales/{material.Id}/salidas",
            new { cantidad = 5, destino = "Techo" });

        Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
    }

    [Fact]
    public async Task Salida_SinDestino_DevuelveBadRequest()
    {
        var material = await _cliente.CrearMaterialAsync(stockInicial: 3);

        var respuesta = await _cliente.PostAsJsonAsync($"/api/materiales/{material.Id}/salidas", new { cantidad = 1 });

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task CantidadCero_DevuelveBadRequest()
    {
        var material = await _cliente.CrearMaterialAsync();

        var respuesta = await _cliente.PostAsJsonAsync($"/api/materiales/{material.Id}/entradas", new { cantidad = 0 });

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task Eliminar_MaterialConMovimientos_DevuelveConflict()
    {
        var material = await _cliente.CrearMaterialAsync();
        await _cliente.PostAsJsonAsync($"/api/materiales/{material.Id}/entradas", new { cantidad = 1 });

        var respuesta = await _cliente.DeleteAsync($"/api/materiales/{material.Id}");

        Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
    }

    [Fact]
    public async Task Historial_FiltraPorMaterial()
    {
        var material = await _cliente.CrearMaterialAsync(stockInicial: 20);
        await _cliente.PostAsJsonAsync($"/api/materiales/{material.Id}/entradas", new { cantidad = 5 });
        await _cliente.PostAsJsonAsync($"/api/materiales/{material.Id}/salidas", new { cantidad = 2, destino = "Baño" });

        var historial = await (await _cliente.GetAsync($"/api/movimientos?materialId={material.Id}"))
            .LeerAsync<List<MovimientoDto>>();

        Assert.Equal(2, historial.Count);
        Assert.All(historial, m => Assert.Equal(material.Id, m.MaterialId));
    }

    private record MovimientoDto(int Id, int MaterialId, string Tipo, decimal Cantidad, string? Destino);
}
