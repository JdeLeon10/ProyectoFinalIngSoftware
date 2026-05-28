namespace IngSoftwareBackend.DTOs;

public class SolicitudPrestamoDto
{
    public int IdSolicitudPrestamo { get; set; }

    public int IdUsuario { get; set; }

    public int IdCuentaDesembolso { get; set; }

    public decimal MontoSolicitado { get; set; }

    public int PlazoMeses { get; set; }

    public string DestinoPrestamo { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public DateTime FechaSolicitud { get; set; }

    public string? Observaciones { get; set; }

    public int? AprobadoPor { get; set; }

    public DateTime? FechaResolucion { get; set; }
}
