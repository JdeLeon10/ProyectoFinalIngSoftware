using IngSoftwareBackend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IngSoftwareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : Controller
    {
        public readonly BancaEnLineaDbContext _context;

        public ReportesController(BancaEnLineaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/allreportes")]
        public async Task<ActionResult> GetReportes()
        {
            var reporte = new
            {
                Usuarios = await _context.Usuarios.CountAsync(),
                Cuentas = await _context.Cuenta.CountAsync(),
                Transferencias = await _context.Transferencia.CountAsync(),
                SolicitudesPrestamo = await _context.SolicitudPrestamos.CountAsync(),
                Prestamos = await _context.Prestamos.CountAsync(),
                Pagos = await _context.PagoPrestamos.CountAsync(),
                SaldoTotalCuentas = await _context.Cuenta.SumAsync(c => c.SaldoActual),
                TotalTransferido = await _context.Transferencia.SumAsync(t => t.Monto),
                TotalPagadoPrestamos = await _context.PagoPrestamos.SumAsync(p => p.MontoPagado)
            };

            return Ok(reporte);
        }

        [HttpGet]
        [Route("reporte/{id}")]
        public async Task<ActionResult> GetReporteById(string id)
        {
            switch (id.ToLower())
            {
                case "usuarios":
                    return Ok(new { Total = await _context.Usuarios.CountAsync(), Activos = await _context.Usuarios.CountAsync(u => u.Estado == "Activo") });
                case "cuentas":
                    return Ok(new { Total = await _context.Cuenta.CountAsync(), Activas = await _context.Cuenta.CountAsync(c => c.Estado == "Activa") });
                case "transferencias":
                    return Ok(new { Total = await _context.Transferencia.CountAsync(), Monto = await _context.Transferencia.SumAsync(t => t.Monto) });
                case "solicitudes":
                    return Ok(new { Total = await _context.SolicitudPrestamos.CountAsync(), Pendientes = await _context.SolicitudPrestamos.CountAsync(s => s.Estado == "Pendiente") });
                case "prestamos":
                    return Ok(new { Total = await _context.Prestamos.CountAsync(), SaldoPendiente = await _context.Prestamos.SumAsync(p => p.SaldoPendiente) });
                case "pagos":
                    return Ok(new { Total = await _context.PagoPrestamos.CountAsync(), Monto = await _context.PagoPrestamos.SumAsync(p => p.MontoPagado) });
                default:
                    return NotFound();
            }
        }

        [HttpPost]
        [Route("/createreporte")]
        public IActionResult CreateReporte()
        {
            return BadRequest("Los reportes son de solo consulta.");
        }

        [HttpPut]
        [Route("editreporte/{id}")]
        public IActionResult UpdateReporte(string id)
        {
            return BadRequest("Los reportes son de solo consulta.");
        }

        [HttpPut]
        [Route("togglereporte/{id}")]
        public IActionResult ToggleReporte(string id)
        {
            return BadRequest("Los reportes son de solo consulta.");
        }

        [HttpDelete]
        [Route("deletereporte/{id}")]
        public IActionResult DeleteReporte(string id)
        {
            return BadRequest("Los reportes son de solo consulta.");
        }
    }
}
