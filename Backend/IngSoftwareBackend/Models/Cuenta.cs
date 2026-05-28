using System;
using System.Collections.Generic;

namespace IngSoftwareBackend.Models;

public partial class Cuenta
{
    public int IdCuenta { get; set; }

    public int IdUsuario { get; set; }

    public string NumeroCuenta { get; set; } = null!;

    public string TipoCuenta { get; set; } = null!;

    public decimal SaldoActual { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime FechaApertura { get; set; }

    public virtual ICollection<Beneficiario> Beneficiarios { get; set; } = new List<Beneficiario>();

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<MovimientoCuentum> MovimientoCuenta { get; set; } = new List<MovimientoCuentum>();

    public virtual ICollection<PagoPrestamo> PagoPrestamos { get; set; } = new List<PagoPrestamo>();

    public virtual ICollection<SolicitudPrestamo> SolicitudPrestamos { get; set; } = new List<SolicitudPrestamo>();

    public virtual ICollection<Transferencium> TransferenciumIdCuentaDestinoNavigations { get; set; } = new List<Transferencium>();

    public virtual ICollection<Transferencium> TransferenciumIdCuentaOrigenNavigations { get; set; } = new List<Transferencium>();
}
