using IngSoftwareBackend.Data;
using IngSoftwareBackend.DTOs;
using IngSoftwareBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IngSoftwareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : Controller
    {
        public readonly BancaEnLineaDbContext _context;

        public PagosController(BancaEnLineaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/allpagos")]
        public async Task<ActionResult<IEnumerable<PagoPrestamoDto>>> GetPagos()
        {
            var pagos = await _context.PagoPrestamos.Select(p => new PagoPrestamoDto
            {
                IdPagoPrestamo = p.IdPagoPrestamo,
                IdPrestamo = p.IdPrestamo,
                IdCuota = p.IdCuota,
                IdCuentaOrigen = p.IdCuentaOrigen,
                MontoPagado = p.MontoPagado,
                CapitalAbonado = p.CapitalAbonado,
                InteresAbonado = p.InteresAbonado,
                MoraAbonada = p.MoraAbonada,
                FechaPago = p.FechaPago,
                Estado = p.Estado,
                RegistradoPor = p.RegistradoPor
            }).ToListAsync();

            return Ok(pagos);
        }

        [HttpGet]
        [Route("pago/{id}")]
        public async Task<ActionResult<PagoPrestamoDto>> GetPagoById(int id)
        {
            var pago = await _context.PagoPrestamos.FirstOrDefaultAsync(p => p.IdPagoPrestamo == id);
            return pago == null ? NotFound() : Ok(ToDto(pago));
        }

        [HttpPost]
        [Route("/createpago")]
        public async Task<ActionResult<PagoPrestamoDto>> CreatePago(PagoPrestamoDto pagoDto)
        {
            var validation = await ValidatePago(pagoDto);
            if (validation != null) return validation;

            var pago = new PagoPrestamo
            {
                IdPrestamo = pagoDto.IdPrestamo,
                IdCuota = pagoDto.IdCuota,
                IdCuentaOrigen = pagoDto.IdCuentaOrigen,
                MontoPagado = pagoDto.MontoPagado,
                CapitalAbonado = pagoDto.CapitalAbonado,
                InteresAbonado = pagoDto.InteresAbonado,
                MoraAbonada = pagoDto.MoraAbonada,
                Estado = "Aplicado",
                RegistradoPor = pagoDto.RegistradoPor
            };

            _context.PagoPrestamos.Add(pago);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPagoById), new { id = pago.IdPagoPrestamo }, ToDto(pago));
        }

        [HttpPut]
        [Route("editpago/{id}")]
        public async Task<ActionResult<PagoPrestamoDto>> UpdatePago(int id, PagoPrestamoDto pagoDto)
        {
            var pago = await _context.PagoPrestamos.FirstOrDefaultAsync(p => p.IdPagoPrestamo == id);
            if (pago == null) return NotFound();

            var validation = await ValidatePago(pagoDto);
            if (validation != null) return validation;

            pago.IdPrestamo = pagoDto.IdPrestamo;
            pago.IdCuota = pagoDto.IdCuota;
            pago.IdCuentaOrigen = pagoDto.IdCuentaOrigen;
            pago.MontoPagado = pagoDto.MontoPagado;
            pago.CapitalAbonado = pagoDto.CapitalAbonado;
            pago.InteresAbonado = pagoDto.InteresAbonado;
            pago.MoraAbonada = pagoDto.MoraAbonada;
            pago.RegistradoPor = pagoDto.RegistradoPor;

            await _context.SaveChangesAsync();
            return Ok(ToDto(pago));
        }

        [HttpPut]
        [Route("togglepago/{id}")]
        public async Task<ActionResult<PagoPrestamoDto>> TogglePago(int id)
        {
            var pago = await _context.PagoPrestamos.FirstOrDefaultAsync(p => p.IdPagoPrestamo == id);
            if (pago == null) return NotFound();

            pago.Estado = pago.Estado == "Aplicado" ? "Anulado" : "Aplicado";
            await _context.SaveChangesAsync();

            return Ok(ToDto(pago));
        }

        [HttpDelete]
        [Route("deletepago/{id}")]
        public async Task<IActionResult> DeletePago(int id)
        {
            var pago = await _context.PagoPrestamos.FirstOrDefaultAsync(p => p.IdPagoPrestamo == id);
            if (pago == null) return NotFound();
            if (pago.Estado == "Aplicado") return BadRequest("No se puede eliminar un pago aplicado.");

            _context.PagoPrestamos.Remove(pago);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<ActionResult?> ValidatePago(PagoPrestamoDto pagoDto)
        {
            if (!await _context.Prestamos.AnyAsync(p => p.IdPrestamo == pagoDto.IdPrestamo)) return BadRequest("El prestamo indicado no existe.");
            if (!await _context.CuotaPrestamos.AnyAsync(c => c.IdCuota == pagoDto.IdCuota)) return BadRequest("La cuota indicada no existe.");
            if (!await _context.Cuenta.AnyAsync(c => c.IdCuenta == pagoDto.IdCuentaOrigen)) return BadRequest("La cuenta origen no existe.");
            if (!await _context.Usuarios.AnyAsync(u => u.IdUsuario == pagoDto.RegistradoPor)) return BadRequest("El usuario registrador no existe.");
            if (pagoDto.MontoPagado <= 0) return BadRequest("El monto pagado debe ser mayor a cero.");
            if (pagoDto.CapitalAbonado < 0 || pagoDto.InteresAbonado < 0 || pagoDto.MoraAbonada < 0) return BadRequest("Los abonos no pueden ser negativos.");

            return null;
        }

        private static PagoPrestamoDto ToDto(PagoPrestamo pago)
        {
            return new PagoPrestamoDto
            {
                IdPagoPrestamo = pago.IdPagoPrestamo,
                IdPrestamo = pago.IdPrestamo,
                IdCuota = pago.IdCuota,
                IdCuentaOrigen = pago.IdCuentaOrigen,
                MontoPagado = pago.MontoPagado,
                CapitalAbonado = pago.CapitalAbonado,
                InteresAbonado = pago.InteresAbonado,
                MoraAbonada = pago.MoraAbonada,
                FechaPago = pago.FechaPago,
                Estado = pago.Estado,
                RegistradoPor = pago.RegistradoPor
            };
        }
    }
}
