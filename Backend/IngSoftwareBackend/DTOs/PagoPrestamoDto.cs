namespace IngSoftwareBackend.DTOs;

public class PagoPrestamoDto
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
}
