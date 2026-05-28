namespace IngSoftwareBackend.DTOs;

public class CuentaDto
{
    public int IdCuenta { get; set; }

    public int IdUsuario { get; set; }

    public string NumeroCuenta { get; set; } = null!;

    public string TipoCuenta { get; set; } = null!;

    public decimal SaldoActual { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime FechaApertura { get; set; }
}
