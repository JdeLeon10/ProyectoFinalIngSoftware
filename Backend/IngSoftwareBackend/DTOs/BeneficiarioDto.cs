namespace IngSoftwareBackend.DTOs;

public class BeneficiarioDto
{
    public int IdBeneficiario { get; set; }

    public int IdUsuarioOrigen { get; set; }

    public int IdCuentaDestino { get; set; }

    public string Alias { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public DateTime FechaAgregado { get; set; }
}
