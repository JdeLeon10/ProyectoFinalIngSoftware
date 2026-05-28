using IngSoftwareBackend.Data;
using IngSoftwareBackend.DTOs;
using IngSoftwareBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IngSoftwareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeneficiariosController : Controller
    {
        public readonly BancaEnLineaDbContext _context;

        public BeneficiariosController(BancaEnLineaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/allbeneficiarios")]
        public async Task<ActionResult<IEnumerable<BeneficiarioDto>>> GetBeneficiarios()
        {
            var beneficiarios = await _context.Beneficiarios.Select(b => new BeneficiarioDto
            {
                IdBeneficiario = b.IdBeneficiario,
                IdUsuarioOrigen = b.IdUsuarioOrigen,
                IdCuentaDestino = b.IdCuentaDestino,
                Alias = b.Alias,
                Estado = b.Estado,
                FechaAgregado = b.FechaAgregado
            }).ToListAsync();

            return Ok(beneficiarios);
        }

        [HttpGet]
        [Route("beneficiario/{id}")]
        public async Task<ActionResult<BeneficiarioDto>> GetBeneficiarioById(int id)
        {
            var beneficiario = await _context.Beneficiarios.FirstOrDefaultAsync(b => b.IdBeneficiario == id);

            if (beneficiario == null)
            {
                return NotFound();
            }

            return Ok(ToDto(beneficiario));
        }

        [HttpPost]
        [Route("/createbeneficiario")]
        public async Task<ActionResult<BeneficiarioDto>> CreateBeneficiario(BeneficiarioDto beneficiarioDto)
        {
            var validation = await ValidateBeneficiario(beneficiarioDto);
            if (validation != null)
            {
                return validation;
            }

            if (await _context.Beneficiarios.AnyAsync(b => b.IdUsuarioOrigen == beneficiarioDto.IdUsuarioOrigen && b.IdCuentaDestino == beneficiarioDto.IdCuentaDestino))
            {
                return BadRequest("Ya existe este beneficiario para el usuario indicado.");
            }

            var beneficiario = new Beneficiario
            {
                IdUsuarioOrigen = beneficiarioDto.IdUsuarioOrigen,
                IdCuentaDestino = beneficiarioDto.IdCuentaDestino,
                Alias = beneficiarioDto.Alias.Trim(),
                Estado = "Activo"
            };

            _context.Beneficiarios.Add(beneficiario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBeneficiarioById), new { id = beneficiario.IdBeneficiario }, ToDto(beneficiario));
        }

        [HttpPut]
        [Route("editbeneficiario/{id}")]
        public async Task<ActionResult<BeneficiarioDto>> UpdateBeneficiario(int id, BeneficiarioDto beneficiarioDto)
        {
            var beneficiario = await _context.Beneficiarios.FirstOrDefaultAsync(b => b.IdBeneficiario == id);

            if (beneficiario == null)
            {
                return NotFound();
            }

            var validation = await ValidateBeneficiario(beneficiarioDto);
            if (validation != null)
            {
                return validation;
            }

            if (await _context.Beneficiarios.AnyAsync(b => b.IdBeneficiario != id && b.IdUsuarioOrigen == beneficiarioDto.IdUsuarioOrigen && b.IdCuentaDestino == beneficiarioDto.IdCuentaDestino))
            {
                return BadRequest("Ya existe este beneficiario para el usuario indicado.");
            }

            beneficiario.IdUsuarioOrigen = beneficiarioDto.IdUsuarioOrigen;
            beneficiario.IdCuentaDestino = beneficiarioDto.IdCuentaDestino;
            beneficiario.Alias = beneficiarioDto.Alias.Trim();

            await _context.SaveChangesAsync();

            return Ok(ToDto(beneficiario));
        }

        [HttpPut]
        [Route("togglebeneficiario/{id}")]
        public async Task<ActionResult<BeneficiarioDto>> ToggleBeneficiario(int id)
        {
            var beneficiario = await _context.Beneficiarios.FirstOrDefaultAsync(b => b.IdBeneficiario == id);

            if (beneficiario == null)
            {
                return NotFound();
            }

            beneficiario.Estado = beneficiario.Estado == "Activo" ? "Inactivo" : "Activo";
            await _context.SaveChangesAsync();

            return Ok(ToDto(beneficiario));
        }

        [HttpDelete]
        [Route("deletebeneficiario/{id}")]
        public async Task<IActionResult> DeleteBeneficiario(int id)
        {
            var beneficiario = await _context.Beneficiarios.FirstOrDefaultAsync(b => b.IdBeneficiario == id);

            if (beneficiario == null)
            {
                return NotFound();
            }
            if (beneficiario.Estado == "Activo")
            {
                return BadRequest("No se puede eliminar un beneficiario activo.");
            }

            _context.Beneficiarios.Remove(beneficiario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<ActionResult?> ValidateBeneficiario(BeneficiarioDto beneficiarioDto)
        {
            if (!await _context.Usuarios.AnyAsync(u => u.IdUsuario == beneficiarioDto.IdUsuarioOrigen))
            {
                return BadRequest("El usuario origen no existe.");
            }
            if (!await _context.Cuenta.AnyAsync(c => c.IdCuenta == beneficiarioDto.IdCuentaDestino))
            {
                return BadRequest("La cuenta destino no existe.");
            }
            if (string.IsNullOrWhiteSpace(beneficiarioDto.Alias))
            {
                return BadRequest("El alias es requerido.");
            }

            return null;
        }

        private static BeneficiarioDto ToDto(Beneficiario beneficiario)
        {
            return new BeneficiarioDto
            {
                IdBeneficiario = beneficiario.IdBeneficiario,
                IdUsuarioOrigen = beneficiario.IdUsuarioOrigen,
                IdCuentaDestino = beneficiario.IdCuentaDestino,
                Alias = beneficiario.Alias,
                Estado = beneficiario.Estado,
                FechaAgregado = beneficiario.FechaAgregado
            };
        }
    }
}
