using System;
using System.Collections.Generic;

namespace IngSoftwareBackend.Models;

public partial class Prestamo
{
    public int IdPrestamo { get; set; }

    public int IdSolicitudPrestamo { get; set; }

    public int IdUsuario { get; set; }

    public decimal MontoAprobado { get; set; }

    public decimal TasaInteres { get; set; }

    public int PlazoMeses { get; set; }

    public decimal SaldoPendiente { get; set; }

    public DateTime? FechaDesembolso { get; set; }

    public string Estado { get; set; } = null!;

    public virtual ICollection<CuotaPrestamo> CuotaPrestamos { get; set; } = new List<CuotaPrestamo>();

    public virtual SolicitudPrestamo IdSolicitudPrestamoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<PagoPrestamo> PagoPrestamos { get; set; } = new List<PagoPrestamo>();
}
