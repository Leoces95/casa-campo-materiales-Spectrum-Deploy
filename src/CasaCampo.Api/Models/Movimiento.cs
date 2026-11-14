namespace CasaCampo.Api.Models;

public enum TipoMovimiento
{
    Entrada, // compra o llegada de material a la bodega
    Salida   // material entregado a una parte de la obra
}

/// <summary>Registro de una entrada o salida de material.</summary>
public record Movimiento(
    int Id,
    int MaterialId,
    string Material,
    TipoMovimiento Tipo,
    decimal Cantidad,
    DateTime Fecha,
    string? Destino,
    string? Nota);
