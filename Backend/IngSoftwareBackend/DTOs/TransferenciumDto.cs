namespace IngSoftwareBackend.DTOs;

public class TransferenciumDto
{
    public int IdTransferencia { get; set; }

    public int IdCuentaOrigen { get; set; }

    public int IdCuentaDestino { get; set; }

    public decimal Monto { get; set; }

    public string? Descripcion { get; set; }

    public DateTime FechaTransferencia { get; set; }

    public string Estado { get; set; } = null!;

    public string Referencia { get; set; } = null!;
}
