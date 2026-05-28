namespace IngSoftwareBackend.DTOs;

public class MovimientoCuentumDto
{
    public int IdMovimiento { get; set; }

    public int IdCuenta { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public decimal Monto { get; set; }

    public decimal SaldoAnterior { get; set; }

    public decimal SaldoPosterior { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public string? ReferenciaTipo { get; set; }

    public int? ReferenciaId { get; set; }

    public string? Descripcion { get; set; }
}
