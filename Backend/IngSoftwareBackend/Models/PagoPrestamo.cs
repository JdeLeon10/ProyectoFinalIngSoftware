using System;
using System.Collections.Generic;

namespace IngSoftwareBackend.Models;

public partial class PagoPrestamo
{
    public int IdPagoPrestamo { get; set; }

    public int IdPrestamo { get; set; }

    public int IdCuota { get; set; }

    public int IdCuentaOrigen { get; set; }

    public decimal MontoPagado { get; set; }

    public decimal CapitalAbonado { get; set; }

    public decimal InteresAbonado { get; set; }

    public decimal MoraAbonada { get; set; }

    public DateTime FechaPago { get; set; }

    public string Estado { get; set; } = null!;

    public int RegistradoPor { get; set; }

    public virtual Cuenta IdCuentaOrigenNavigation { get; set; } = null!;

    public virtual CuotaPrestamo IdCuotaNavigation { get; set; } = null!;

    public virtual Prestamo IdPrestamoNavigation { get; set; } = null!;

    public virtual Usuario RegistradoPorNavigation { get; set; } = null!;
}
