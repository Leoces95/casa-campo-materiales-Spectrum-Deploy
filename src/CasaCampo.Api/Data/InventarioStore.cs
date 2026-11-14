using CasaCampo.Api.Models;

namespace CasaCampo.Api.Data;

/// <summary>
/// Almacén en memoria del inventario de la obra. Se reinicia con cada despliegue,
/// lo cual es suficiente para la práctica (el foco es CI/CD) y evita manejar
/// cadenas de conexión o secretos de una base de datos.
/// </summary>
public class InventarioStore
{
    private readonly object _lock = new();
    private readonly List<Material> _materiales = new();
    private readonly List<Movimiento> _movimientos = new();
    private int _siguienteMaterial;
    private int _siguienteMovimiento;

    public InventarioStore()
    {
        // Materiales iniciales de la vivienda de campo (precios de referencia en COP).
        Sembrar("Bloque de cemento 10x20x40", CategoriaMaterial.Mamposteria, "unidad", 2_800m, 600, 200, "Bloquera El Progreso");
        Sembrar("Ladrillo tolete", CategoriaMaterial.Mamposteria, "unidad", 900m, 1_000, 300, "Chircal La Vega");
        Sembrar("Tabla de pino 3 m", CategoriaMaterial.Madera, "unidad", 18_000m, 40, 15, "Aserrío del pueblo");
        Sembrar("Vara de guadua 6 m", CategoriaMaterial.Madera, "unidad", 12_000m, 25, 10, "Finca La Esperanza");
        Sembrar("Lámina de zinc ondulada 3 m", CategoriaMaterial.Cubierta, "lámina", 38_000m, 12, 20, "Ferretería Central");
        Sembrar("Teja de barro", CategoriaMaterial.Cubierta, "unidad", 1_500m, 300, 100, "Chircal La Vega");
        Sembrar("Clavos de 3 pulgadas", CategoriaMaterial.Ferreteria, "libra", 6_500m, 8, 10, "Ferretería Central");
        Sembrar("Alambre negro", CategoriaMaterial.Ferreteria, "kg", 9_000m, 5, 3, "Ferretería Central");
        Sembrar("Arena de río", CategoriaMaterial.Agregados, "m3", 85_000m, 3, 2, "Volquetas San José");
        Sembrar("Balastro", CategoriaMaterial.Agregados, "m3", 70_000m, 1.5m, 2, "Volquetas San José");
        Sembrar("Cemento gris 50 kg", CategoriaMaterial.Aglomerantes, "bulto", 32_000m, 15, 10, "Ferretería Central");
    }

    public IReadOnlyList<Material> ListarMateriales()
    {
        lock (_lock) return _materiales.OrderBy(m => m.Id).ToList();
    }

    public Material? BuscarMaterial(int id)
    {
        lock (_lock) return _materiales.FirstOrDefault(m => m.Id == id);
    }

    public bool ExisteNombre(string nombre, int? excluirId = null)
    {
        lock (_lock)
        {
            return _materiales.Any(m => m.Id != excluirId &&
                string.Equals(m.Nombre, nombre.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }

    public Material AgregarMaterial(Material material)
    {
        lock (_lock)
        {
            material.Id = ++_siguienteMaterial;
            _materiales.Add(material);
            return material;
        }
    }

    public bool TieneMovimientos(int materialId)
    {
        lock (_lock) return _movimientos.Any(m => m.MaterialId == materialId);
    }

    public bool EliminarMaterial(int id)
    {
        lock (_lock) return _materiales.RemoveAll(m => m.Id == id) > 0;
    }

    public IReadOnlyList<Movimiento> ListarMovimientos()
    {
        lock (_lock) return _movimientos.OrderByDescending(m => m.Id).ToList();
    }

    /// <summary>
    /// Registra una entrada o salida y actualiza el stock. Devuelve null si es una
    /// salida mayor que la existencia disponible.
    /// </summary>
    public Movimiento? RegistrarMovimiento(Material material, TipoMovimiento tipo, decimal cantidad, string? destino, string? nota)
    {
        lock (_lock)
        {
            if (tipo == TipoMovimiento.Salida && cantidad > material.Stock)
            {
                return null;
            }

            material.Stock += tipo == TipoMovimiento.Entrada ? cantidad : -cantidad;
            var movimiento = new Movimiento(++_siguienteMovimiento, material.Id, material.Nombre, tipo,
                cantidad, DateTime.UtcNow, destino?.Trim(), nota?.Trim());
            _movimientos.Add(movimiento);
            return movimiento;
        }
    }

    private void Sembrar(string nombre, CategoriaMaterial categoria, string unidad, decimal precio,
        decimal stock, decimal stockMinimo, string proveedor)
    {
        AgregarMaterial(new Material
        {
            Nombre = nombre,
            Categoria = categoria,
            Unidad = unidad,
            PrecioUnitario = precio,
            Stock = stock,
            StockMinimo = stockMinimo,
            Proveedor = proveedor
        });
    }
}
