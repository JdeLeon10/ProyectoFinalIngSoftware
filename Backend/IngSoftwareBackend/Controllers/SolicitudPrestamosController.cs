using IngSoftwareBackend.Data;
using IngSoftwareBackend.DTOs;
using IngSoftwareBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IngSoftwareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudPrestamosController : Controller
    {
        public readonly BancaEnLineaDbContext _context;

        public SolicitudPrestamosController(BancaEnLineaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/allsolicitudprestamos")]
        public async Task<ActionResult<IEnumerable<SolicitudPrestamoDto>>> GetSolicitudPrestamos()
        {
            var solicitudes = await _context.SolicitudPrestamos.Select(s => new SolicitudPrestamoDto
            {
                IdSolicitudPrestamo = s.IdSolicitudPrestamo,
                IdUsuario = s.IdUsuario,
                IdCuentaDesembolso = s.IdCuentaDesembolso,
                MontoSolicitado = s.MontoSolicitado,
                PlazoMeses = s.PlazoMeses,
                DestinoPrestamo = s.DestinoPrestamo,
                Estado = s.Estado,
                FechaSolicitud = s.FechaSolicitud,
                Observaciones = s.Observaciones,
                AprobadoPor = s.AprobadoPor,
                FechaResolucion = s.FechaResolucion
            }).ToListAsync();

            return Ok(solicitudes);
        }

        [HttpGet]
        [Route("solicitudprestamo/{id}")]
        public async Task<ActionResult<SolicitudPrestamoDto>> GetSolicitudPrestamoById(int id)
        {
            var solicitud = await _context.SolicitudPrestamos.FirstOrDefaultAsync(s => s.IdSolicitudPrestamo == id);
            return solicitud == null ? NotFound() : Ok(ToDto(solicitud));
        }

        [HttpPost]
        [Route("/createsolicitudprestamo")]
        public async Task<ActionResult<SolicitudPrestamoDto>> CreateSolicitudPrestamo(SolicitudPrestamoDto solicitudDto)
        {
            var validation = await ValidateSolicitud(solicitudDto);
            if (validation != null) return validation;

            var solicitud = new SolicitudPrestamo
            {
                IdUsuario = solicitudDto.IdUsuario,
                IdCuentaDesembolso = solicitudDto.IdCuentaDesembolso,
                MontoSolicitado = solicitudDto.MontoSolicitado,
                PlazoMeses = solicitudDto.PlazoMeses,
                DestinoPrestamo = solicitudDto.DestinoPrestamo.Trim(),
                Estado = "Pendiente",
                Observaciones = solicitudDto.Observaciones,
                AprobadoPor = solicitudDto.AprobadoPor,
                FechaResolucion = solicitudDto.FechaResolucion
            };

            _context.SolicitudPrestamos.Add(solicitud);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSolicitudPrestamoById), new { id = solicitud.IdSolicitudPrestamo }, ToDto(solicitud));
        }

        [HttpPut]
        [Route("editsolicitudprestamo/{id}")]
        public async Task<ActionResult<SolicitudPrestamoDto>> UpdateSolicitudPrestamo(int id, SolicitudPrestamoDto solicitudDto)
        {
            var solicitud = await _context.SolicitudPrestamos.FirstOrDefaultAsync(s => s.IdSolicitudPrestamo == id);
            if (solicitud == null) return NotFound();

            var validation = await ValidateSolicitud(solicitudDto);
            if (validation != null) return validation;

            solicitud.IdUsuario = solicitudDto.IdUsuario;
            solicitud.IdCuentaDesembolso = solicitudDto.IdCuentaDesembolso;
            solicitud.MontoSolicitado = solicitudDto.MontoSolicitado;
            solicitud.PlazoMeses = solicitudDto.PlazoMeses;
            solicitud.DestinoPrestamo = solicitudDto.DestinoPrestamo.Trim();
            solicitud.Observaciones = solicitudDto.Observaciones;
            solicitud.AprobadoPor = solicitudDto.AprobadoPor;
            solicitud.FechaResolucion = solicitudDto.FechaResolucion;

            await _context.SaveChangesAsync();
            return Ok(ToDto(solicitud));
        }

        [HttpPut]
        [Route("togglesolicitudprestamo/{id}")]
        public async Task<ActionResult<SolicitudPrestamoDto>> ToggleSolicitudPrestamo(int id)
        {
            var solicitud = await _context.SolicitudPrestamos.FirstOrDefaultAsync(s => s.IdSolicitudPrestamo == id);
            if (solicitud == null) return NotFound();

            solicitud.Estado = solicitud.Estado == "Pendiente" ? "Cancelada" : "Pendiente";
            await _context.SaveChangesAsync();

            return Ok(ToDto(solicitud));
        }

        [HttpDelete]
        [Route("deletesolicitudprestamo/{id}")]
        public async Task<IActionResult> DeleteSolicitudPrestamo(int id)
        {
            var solicitud = await _context.SolicitudPrestamos.FirstOrDefaultAsync(s => s.IdSolicitudPrestamo == id);
            if (solicitud == null) return NotFound();
            if (solicitud.Estado == "Pendiente") return BadRequest("No se puede eliminar una solicitud pendiente.");

            _context.SolicitudPrestamos.Remove(solicitud);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<ActionResult?> ValidateSolicitud(SolicitudPrestamoDto solicitudDto)
        {
            if (!await _context.Usuarios.AnyAsync(u => u.IdUsuario == solicitudDto.IdUsuario)) return BadRequest("El usuario indicado no existe.");
            if (!await _context.Cuenta.AnyAsync(c => c.IdCuenta == solicitudDto.IdCuentaDesembolso)) return BadRequest("La cuenta de desembolso no existe.");
            if (solicitudDto.AprobadoPor.HasValue && !await _context.Usuarios.AnyAsync(u => u.IdUsuario == solicitudDto.AprobadoPor.Value)) return BadRequest("El usuario aprobador no existe.");
            if (solicitudDto.MontoSolicitado <= 0) return BadRequest("El monto solicitado debe ser mayor a cero.");
            if (solicitudDto.PlazoMeses <= 0) return BadRequest("El plazo debe ser mayor a cero.");
            if (string.IsNullOrWhiteSpace(solicitudDto.DestinoPrestamo)) return BadRequest("El destino del prestamo es requerido.");

            return null;
        }

        private static SolicitudPrestamoDto ToDto(SolicitudPrestamo solicitud)
        {
            return new SolicitudPrestamoDto
            {
                IdSolicitudPrestamo = solicitud.IdSolicitudPrestamo,
                IdUsuario = solicitud.IdUsuario,
                IdCuentaDesembolso = solicitud.IdCuentaDesembolso,
                MontoSolicitado = solicitud.MontoSolicitado,
                PlazoMeses = solicitud.PlazoMeses,
                DestinoPrestamo = solicitud.DestinoPrestamo,
                Estado = solicitud.Estado,
                FechaSolicitud = solicitud.FechaSolicitud,
                Observaciones = solicitud.Observaciones,
                AprobadoPor = solicitud.AprobadoPor,
                FechaResolucion = solicitud.FechaResolucion
            };
        }
    }
}
