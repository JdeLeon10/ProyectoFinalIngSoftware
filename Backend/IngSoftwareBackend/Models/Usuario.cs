using System;
using System.Collections.Generic;

namespace IngSoftwareBackend.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int IdRol { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Dpi { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<Beneficiario> Beneficiarios { get; set; } = new List<Beneficiario>();

    public virtual ICollection<Cuenta> Cuenta { get; set; } = new List<Cuenta>();

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<PagoPrestamo> PagoPrestamos { get; set; } = new List<PagoPrestamo>();

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

    public virtual ICollection<SolicitudPrestamo> SolicitudPrestamoAprobadoPorNavigations { get; set; } = new List<SolicitudPrestamo>();

    public virtual ICollection<SolicitudPrestamo> SolicitudPrestamoIdUsuarioNavigations { get; set; } = new List<SolicitudPrestamo>();
}
