namespace IngSoftwareBackend.DTOs;

public class PrestamoDto
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
}
