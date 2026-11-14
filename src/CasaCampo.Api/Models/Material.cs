namespace CasaCampo.Api.Models;

/// <summary>Grupos de materiales de la obra.</summary>
public enum CategoriaMaterial
{
    Mamposteria,   // bloques, ladrillos
    Madera,        // tablas, varas, guadua
    Cubierta,      // zinc, tejas (techo)
    Ferreteria,    // clavos, alambre
    Agregados,     // arena, balastro
    Aglomerantes   // cemento, cal
}

/// <summary>Material de construcción con su existencia en bodega.</summary>
public class Material
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public CategoriaMaterial Categoria { get; set; }
    public string Unidad { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public decimal Stock { get; set; }
    public decimal StockMinimo { get; set; }
    public string? Proveedor { get; set; }

    public bool StockBajo => Stock <= StockMinimo;
    public decimal ValorEnBodega => Stock * PrecioUnitario;
}
