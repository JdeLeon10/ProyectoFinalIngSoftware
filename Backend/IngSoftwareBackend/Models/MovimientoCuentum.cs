using System;
using System.Collections.Generic;

namespace IngSoftwareBackend.Models;

public partial class MovimientoCuentum
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

    public virtual Cuenta IdCuentaNavigation { get; set; } = null!;
}
