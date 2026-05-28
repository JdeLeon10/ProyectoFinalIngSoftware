using System;
using System.Collections.Generic;

namespace IngSoftwareBackend.Models;

public partial class CuotaPrestamo
{
    public int IdCuota { get; set; }

    public int IdPrestamo { get; set; }

    public int NumeroCuota { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public decimal CapitalProgramado { get; set; }

    public decimal InteresProgramado { get; set; }

    public decimal MoraProgramada { get; set; }

    public decimal MontoTotalProgramado { get; set; }

    public decimal CapitalPagado { get; set; }

    public decimal InteresPagado { get; set; }

    public decimal MoraPagada { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Prestamo IdPrestamoNavigation { get; set; } = null!;

    public virtual ICollection<PagoPrestamo> PagoPrestamos { get; set; } = new List<PagoPrestamo>();
}
