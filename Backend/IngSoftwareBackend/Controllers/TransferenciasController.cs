using IngSoftwareBackend.Data;
using IngSoftwareBackend.DTOs;
using IngSoftwareBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IngSoftwareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferenciasController : Controller
    {
        public readonly BancaEnLineaDbContext _context;

        public TransferenciasController(BancaEnLineaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/alltransferencias")]
        public async Task<ActionResult<IEnumerable<TransferenciumDto>>> GetTransferencias()
        {
            var transferencias = await _context.Transferencia.Select(t => new TransferenciumDto
            {
                IdTransferencia = t.IdTransferencia,
                IdCuentaOrigen = t.IdCuentaOrigen,
                IdCuentaDestino = t.IdCuentaDestino,
                Monto = t.Monto,
                Descripcion = t.Descripcion,
                FechaTransferencia = t.FechaTransferencia,
                Estado = t.Estado,
                Referencia = t.Referencia
            }).ToListAsync();

            return Ok(transferencias);
        }

        [HttpGet]
        [Route("transferencia/{id}")]
        public async Task<ActionResult<TransferenciumDto>> GetTransferenciaById(int id)
        {
            var transferencia = await _context.Transferencia.FirstOrDefaultAsync(t => t.IdTransferencia == id);
            return transferencia == null ? NotFound() : Ok(ToDto(transferencia));
        }

        [HttpPost]
        [Route("/createtransferencia")]
        public async Task<ActionResult<TransferenciumDto>> CreateTransferencia(TransferenciumDto transferenciaDto)
        {
            var validation = await ValidateTransferencia(transferenciaDto);
            if (validation != null) return validation;

            if (await _context.Transferencia.AnyAsync(t => t.Referencia == transferenciaDto.Referencia.Trim()))
            {
                return BadRequest("Ya existe una transferencia con la misma referencia.");
            }

            var transferencia = new Transferencium
            {
                IdCuentaOrigen = transferenciaDto.IdCuentaOrigen,
                IdCuentaDestino = transferenciaDto.IdCuentaDestino,
                Monto = transferenciaDto.Monto,
                Descripcion = transferenciaDto.Descripcion,
                Estado = "Completada",
                Referencia = transferenciaDto.Referencia.Trim()
            };

            _context.Transferencia.Add(transferencia);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransferenciaById), new { id = transferencia.IdTransferencia }, ToDto(transferencia));
        }

        [HttpPut]
        [Route("edittransferencia/{id}")]
        public async Task<ActionResult<TransferenciumDto>> UpdateTransferencia(int id, TransferenciumDto transferenciaDto)
        {
            var transferencia = await _context.Transferencia.FirstOrDefaultAsync(t => t.IdTransferencia == id);
            if (transferencia == null) return NotFound();

            var validation = await ValidateTransferencia(transferenciaDto);
            if (validation != null) return validation;

            if (await _context.Transferencia.AnyAsync(t => t.IdTransferencia != id && t.Referencia == transferenciaDto.Referencia.Trim()))
            {
                return BadRequest("Ya existe una transferencia con la misma referencia.");
            }

            transferencia.IdCuentaOrigen = transferenciaDto.IdCuentaOrigen;
            transferencia.IdCuentaDestino = transferenciaDto.IdCuentaDestino;
            transferencia.Monto = transferenciaDto.Monto;
            transferencia.Descripcion = transferenciaDto.Descripcion;
            transferencia.Referencia = transferenciaDto.Referencia.Trim();

            await _context.SaveChangesAsync();
            return Ok(ToDto(transferencia));
        }

        [HttpPut]
        [Route("toggletransferencia/{id}")]
        public async Task<ActionResult<TransferenciumDto>> ToggleTransferencia(int id)
        {
            var transferencia = await _context.Transferencia.FirstOrDefaultAsync(t => t.IdTransferencia == id);
            if (transferencia == null) return NotFound();

            transferencia.Estado = transferencia.Estado == "Completada" ? "Anulada" : "Completada";
            await _context.SaveChangesAsync();

            return Ok(ToDto(transferencia));
        }

        [HttpDelete]
        [Route("deletetransferencia/{id}")]
        public async Task<IActionResult> DeleteTransferencia(int id)
        {
            var transferencia = await _context.Transferencia.FirstOrDefaultAsync(t => t.IdTransferencia == id);
            if (transferencia == null) return NotFound();
            if (transferencia.Estado == "Completada") return BadRequest("No se puede eliminar una transferencia completada.");

            _context.Transferencia.Remove(transferencia);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<ActionResult?> ValidateTransferencia(TransferenciumDto transferenciaDto)
        {
            if (transferenciaDto.IdCuentaOrigen == transferenciaDto.IdCuentaDestino)
            {
                return BadRequest("La cuenta origen y destino no pueden ser la misma.");
            }
            if (!await _context.Cuenta.AnyAsync(c => c.IdCuenta == transferenciaDto.IdCuentaOrigen))
            {
                return BadRequest("La cuenta origen no existe.");
            }
            if (!await _context.Cuenta.AnyAsync(c => c.IdCuenta == transferenciaDto.IdCuentaDestino))
            {
                return BadRequest("La cuenta destino no existe.");
            }
            if (transferenciaDto.Monto <= 0)
            {
                return BadRequest("El monto debe ser mayor a cero.");
            }
            if (string.IsNullOrWhiteSpace(transferenciaDto.Referencia))
            {
                return BadRequest("La referencia es requerida.");
            }

            return null;
        }

        private static TransferenciumDto ToDto(Transferencium transferencia)
        {
            return new TransferenciumDto
            {
                IdTransferencia = transferencia.IdTransferencia,
                IdCuentaOrigen = transferencia.IdCuentaOrigen,
                IdCuentaDestino = transferencia.IdCuentaDestino,
                Monto = transferencia.Monto,
                Descripcion = transferencia.Descripcion,
                FechaTransferencia = transferencia.FechaTransferencia,
                Estado = transferencia.Estado,
                Referencia = transferencia.Referencia
            };
        }
    }
}
