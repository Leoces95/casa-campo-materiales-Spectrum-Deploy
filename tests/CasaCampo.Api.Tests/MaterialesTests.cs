using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CasaCampo.Api.Tests;

public class MaterialesTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _cliente;

    public MaterialesTests(WebApplicationFactory<Program> fabrica) => _cliente = fabrica.CreateClient();

    [Fact]
    public async Task Health_RespondeOk()
    {
        var respuesta = await _cliente.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.Contains("ok", await respuesta.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Listar_IncluyeMaterialesSemilla()
    {
        var materiales = await (await _cliente.GetAsync("/api/materiales")).LeerAsync<List<MaterialDto>>();

        Assert.Contains(materiales, m => m.Nombre == "Balastro");
        Assert.Contains(materiales, m => m.Nombre.StartsWith("Lámina de zinc"));
        Assert.Contains(materiales, m => m.Nombre.StartsWith("Bloque de cemento"));
    }

    [Fact]
    public async Task Listar_FiltraPorCategoria()
    {
        var madera = await (await _cliente.GetAsync("/api/materiales?categoria=Madera")).LeerAsync<List<MaterialDto>>();

        Assert.NotEmpty(madera);
        Assert.All(madera, m => Assert.Equal("Madera", m.Categoria));
    }

    [Fact]
    public async Task Crear_LuegoConsultarPorId()
    {
        var creado = await _cliente.CrearMaterialAsync(stockInicial: 50);

        var consultado = await (await _cliente.GetAsync($"/api/materiales/{creado.Id}")).LeerAsync<MaterialDto>();

        Assert.Equal(creado.Nombre, consultado.Nombre);
        Assert.Equal(50, consultado.Stock);
    }

    [Fact]
    public async Task Crear_SinNombre_DevuelveBadRequest()
    {
        var respuesta = await _cliente.PostAsJsonAsync("/api/materiales", new
        {
            nombre = "",
            categoria = "Madera",
            unidad = "unidad",
            precioUnitario = 1000
        });

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task Crear_NombreRepetido_DevuelveConflict()
    {
        var respuesta = await _cliente.PostAsJsonAsync("/api/materiales", new
        {
            nombre = "balastro",
            categoria = "Agregados",
            unidad = "m3",
            precioUnitario = 70000
        });

        Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
    }

    [Fact]
    public async Task Actualizar_CambiaPrecioPeroNoElStock()
    {
        var creado = await _cliente.CrearMaterialAsync(stockInicial: 30);

        var respuesta = await _cliente.PutAsJsonAsync($"/api/materiales/{creado.Id}", new
        {
            nombre = creado.Nombre,
            categoria = "Ferreteria",
            unidad = "caja",
            precioUnitario = 2500,
            stockInicial = 999,
            stockMinimo = 5
        });
        var actualizado = await respuesta.LeerAsync<MaterialDto>();

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.Equal(2500, actualizado.PrecioUnitario);
        Assert.Equal("caja", actualizado.Unidad);
        Assert.Equal(30, actualizado.Stock);
    }

    [Fact]
    public async Task Eliminar_MaterialSinMovimientos_DevuelveNoContent()
    {
        var creado = await _cliente.CrearMaterialAsync();

        var respuesta = await _cliente.DeleteAsync($"/api/materiales/{creado.Id}");
        var consulta = await _cliente.GetAsync($"/api/materiales/{creado.Id}");

        Assert.Equal(HttpStatusCode.NoContent, respuesta.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, consulta.StatusCode);
    }

    [Fact]
    public async Task MaterialInexistente_DevuelveNotFound()
    {
        var respuesta = await _cliente.GetAsync("/api/materiales/99999");

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }
}
