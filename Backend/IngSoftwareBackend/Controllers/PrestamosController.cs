using IngSoftwareBackend.Data;
using IngSoftwareBackend.DTOs;
using IngSoftwareBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IngSoftwareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrestamosController : Controller
    {
        public readonly BancaEnLineaDbContext _context;

        public PrestamosController(BancaEnLineaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/allprestamos")]
        public async Task<ActionResult<IEnumerable<PrestamoDto>>> GetPrestamos()
        {
            var prestamos = await _context.Prestamos.Select(p => new PrestamoDto
            {
                IdPrestamo = p.IdPrestamo,
                IdSolicitudPrestamo = p.IdSolicitudPrestamo,
                IdUsuario = p.IdUsuario,
                MontoAprobado = p.MontoAprobado,
                TasaInteres = p.TasaInteres,
                PlazoMeses = p.PlazoMeses,
                SaldoPendiente = p.SaldoPendiente,
                FechaDesembolso = p.FechaDesembolso,
                Estado = p.Estado
            }).ToListAsync();

            return Ok(prestamos);
        }

        [HttpGet]
        [Route("prestamo/{id}")]
        public async Task<ActionResult<PrestamoDto>> GetPrestamoById(int id)
        {
            var prestamo = await _context.Prestamos.FirstOrDefaultAsync(p => p.IdPrestamo == id);
            return prestamo == null ? NotFound() : Ok(ToDto(prestamo));
        }

        [HttpPost]
        [Route("/createprestamo")]
        public async Task<ActionResult<PrestamoDto>> CreatePrestamo(PrestamoDto prestamoDto)
        {
            var validation = await ValidatePrestamo(prestamoDto);
            if (validation != null) return validation;

            if (await _context.Prestamos.AnyAsync(p => p.IdSolicitudPrestamo == prestamoDto.IdSolicitudPrestamo))
            {
                return BadRequest("Ya existe un prestamo para esta solicitud.");
            }

            var prestamo = new Prestamo
            {
                IdSolicitudPrestamo = prestamoDto.IdSolicitudPrestamo,
                IdUsuario = prestamoDto.IdUsuario,
                MontoAprobado = prestamoDto.MontoAprobado,
                TasaInteres = prestamoDto.TasaInteres,
                PlazoMeses = prestamoDto.PlazoMeses,
                SaldoPendiente = prestamoDto.SaldoPendiente,
                FechaDesembolso = prestamoDto.FechaDesembolso,
                Estado = "Aprobado"
            };

            _context.Prestamos.Add(prestamo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPrestamoById), new { id = prestamo.IdPrestamo }, ToDto(prestamo));
        }

        [HttpPut]
        [Route("editprestamo/{id}")]
        public async Task<ActionResult<PrestamoDto>> UpdatePrestamo(int id, PrestamoDto prestamoDto)
        {
            var prestamo = await _context.Prestamos.FirstOrDefaultAsync(p => p.IdPrestamo == id);
            if (prestamo == null) return NotFound();

            var validation = await ValidatePrestamo(prestamoDto);
            if (validation != null) return validation;

            if (await _context.Prestamos.AnyAsync(p => p.IdPrestamo != id && p.IdSolicitudPrestamo == prestamoDto.IdSolicitudPrestamo))
            {
                return BadRequest("Ya existe un prestamo para esta solicitud.");
            }

            prestamo.IdSolicitudPrestamo = prestamoDto.IdSolicitudPrestamo;
            prestamo.IdUsuario = prestamoDto.IdUsuario;
            prestamo.MontoAprobado = prestamoDto.MontoAprobado;
            prestamo.TasaInteres = prestamoDto.TasaInteres;
            prestamo.PlazoMeses = prestamoDto.PlazoMeses;
            prestamo.SaldoPendiente = prestamoDto.SaldoPendiente;
            prestamo.FechaDesembolso = prestamoDto.FechaDesembolso;

            await _context.SaveChangesAsync();
            return Ok(ToDto(prestamo));
        }

        [HttpPut]
        [Route("toggleprestamo/{id}")]
        public async Task<ActionResult<PrestamoDto>> TogglePrestamo(int id)
        {
            var prestamo = await _context.Prestamos.FirstOrDefaultAsync(p => p.IdPrestamo == id);
            if (prestamo == null) return NotFound();

            prestamo.Estado = prestamo.Estado == "Aprobado" ? "Cancelado" : "Aprobado";
            await _context.SaveChangesAsync();

            return Ok(ToDto(prestamo));
        }

        [HttpDelete]
        [Route("deleteprestamo/{id}")]
        public async Task<IActionResult> DeletePrestamo(int id)
        {
            var prestamo = await _context.Prestamos.FirstOrDefaultAsync(p => p.IdPrestamo == id);
            if (prestamo == null) return NotFound();
            if (prestamo.Estado == "Aprobado") return BadRequest("No se puede eliminar un prestamo aprobado.");

            _context.Prestamos.Remove(prestamo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<ActionResult?> ValidatePrestamo(PrestamoDto prestamoDto)
        {
            if (!await _context.SolicitudPrestamos.AnyAsync(s => s.IdSolicitudPrestamo == prestamoDto.IdSolicitudPrestamo)) return BadRequest("La solicitud indicada no existe.");
            if (!await _context.Usuarios.AnyAsync(u => u.IdUsuario == prestamoDto.IdUsuario)) return BadRequest("El usuario indicado no existe.");
            if (prestamoDto.MontoAprobado <= 0) return BadRequest("El monto aprobado debe ser mayor a cero.");
            if (prestamoDto.TasaInteres < 0) return BadRequest("La tasa de interes no puede ser negativa.");
            if (prestamoDto.PlazoMeses <= 0) return BadRequest("El plazo debe ser mayor a cero.");
            if (prestamoDto.SaldoPendiente < 0) return BadRequest("El saldo pendiente no puede ser negativo.");

            return null;
        }

        private static PrestamoDto ToDto(Prestamo prestamo)
        {
            return new PrestamoDto
            {
                IdPrestamo = prestamo.IdPrestamo,
                IdSolicitudPrestamo = prestamo.IdSolicitudPrestamo,
                IdUsuario = prestamo.IdUsuario,
                MontoAprobado = prestamo.MontoAprobado,
                TasaInteres = prestamo.TasaInteres,
                PlazoMeses = prestamo.PlazoMeses,
                SaldoPendiente = prestamo.SaldoPendiente,
                FechaDesembolso = prestamo.FechaDesembolso,
                Estado = prestamo.Estado
            };
        }
    }
}
