using System;
using System.Collections.Generic;

namespace IngSoftwareBackend.Models;

public partial class Beneficiario
{
    public int IdBeneficiario { get; set; }

    public int IdUsuarioOrigen { get; set; }

    public int IdCuentaDestino { get; set; }

    public string Alias { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public DateTime FechaAgregado { get; set; }

    public virtual Cuenta IdCuentaDestinoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioOrigenNavigation { get; set; } = null!;
}
