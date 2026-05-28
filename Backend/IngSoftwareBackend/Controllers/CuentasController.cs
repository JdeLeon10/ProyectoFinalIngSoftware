using IngSoftwareBackend.Data;
using IngSoftwareBackend.DTOs;
using IngSoftwareBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IngSoftwareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : Controller
    {
        public readonly BancaEnLineaDbContext _context;

        public CuentasController(BancaEnLineaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/allcuentas")]
        public async Task<ActionResult<IEnumerable<CuentaDto>>> GetCuentas()
        {
            var cuentas = await _context.Cuenta.Select(c => new CuentaDto
            {
                IdCuenta = c.IdCuenta,
                IdUsuario = c.IdUsuario,
                NumeroCuenta = c.NumeroCuenta,
                TipoCuenta = c.TipoCuenta,
                SaldoActual = c.SaldoActual,
                Estado = c.Estado,
                FechaApertura = c.FechaApertura
            }).ToListAsync();

            return Ok(cuentas);
        }

        [HttpGet]
        [Route("cuenta/{id}")]
        public async Task<ActionResult<CuentaDto>> GetCuentaById(int id)
        {
            var cuenta = await _context.Cuenta.FirstOrDefaultAsync(c => c.IdCuenta == id);
            return cuenta == null ? NotFound() : Ok(ToDto(cuenta));
        }

        [HttpPost]
        [Route("/createcuenta")]
        public async Task<ActionResult<CuentaDto>> CreateCuenta(CuentaDto cuentaDto)
        {
            var validation = await ValidateCuenta(cuentaDto);
            if (validation != null) return validation;

            if (await _context.Cuenta.AnyAsync(c => c.NumeroCuenta == cuentaDto.NumeroCuenta.Trim()))
            {
                return BadRequest("Ya existe una cuenta con el mismo numero.");
            }

            var cuenta = new Cuenta
            {
                IdUsuario = cuentaDto.IdUsuario,
                NumeroCuenta = cuentaDto.NumeroCuenta.Trim(),
                TipoCuenta = cuentaDto.TipoCuenta.Trim(),
                SaldoActual = cuentaDto.SaldoActual,
                Estado = "Activa"
            };

            _context.Cuenta.Add(cuenta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCuentaById), new { id = cuenta.IdCuenta }, ToDto(cuenta));
        }

        [HttpPut]
        [Route("editcuenta/{id}")]
        public async Task<ActionResult<CuentaDto>> UpdateCuenta(int id, CuentaDto cuentaDto)
        {
            var cuenta = await _context.Cuenta.FirstOrDefaultAsync(c => c.IdCuenta == id);
            if (cuenta == null) return NotFound();

            var validation = await ValidateCuenta(cuentaDto);
            if (validation != null) return validation;

            if (await _context.Cuenta.AnyAsync(c => c.IdCuenta != id && c.NumeroCuenta == cuentaDto.NumeroCuenta.Trim()))
            {
                return BadRequest("Ya existe una cuenta con el mismo numero.");
            }

            cuenta.IdUsuario = cuentaDto.IdUsuario;
            cuenta.NumeroCuenta = cuentaDto.NumeroCuenta.Trim();
            cuenta.TipoCuenta = cuentaDto.TipoCuenta.Trim();
            cuenta.SaldoActual = cuentaDto.SaldoActual;

            await _context.SaveChangesAsync();
            return Ok(ToDto(cuenta));
        }

        [HttpPut]
        [Route("togglecuenta/{id}")]
        public async Task<ActionResult<CuentaDto>> ToggleCuenta(int id)
        {
            var cuenta = await _context.Cuenta.FirstOrDefaultAsync(c => c.IdCuenta == id);
            if (cuenta == null) return NotFound();

            cuenta.Estado = cuenta.Estado == "Activa" ? "Inactiva" : "Activa";
            await _context.SaveChangesAsync();

            return Ok(ToDto(cuenta));
        }

        [HttpDelete]
        [Route("deletecuenta/{id}")]
        public async Task<IActionResult> DeleteCuenta(int id)
        {
            var cuenta = await _context.Cuenta.FirstOrDefaultAsync(c => c.IdCuenta == id);
            if (cuenta == null) return NotFound();
            if (cuenta.Estado == "Activa") return BadRequest("No se puede eliminar una cuenta activa.");

            _context.Cuenta.Remove(cuenta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<ActionResult?> ValidateCuenta(CuentaDto cuentaDto)
        {
            if (!await _context.Usuarios.AnyAsync(u => u.IdUsuario == cuentaDto.IdUsuario))
            {
                return BadRequest("El usuario indicado no existe.");
            }
            if (string.IsNullOrWhiteSpace(cuentaDto.NumeroCuenta) || string.IsNullOrWhiteSpace(cuentaDto.TipoCuenta))
            {
                return BadRequest("Numero de cuenta y tipo de cuenta son requeridos.");
            }

            return null;
        }

        private static CuentaDto ToDto(Cuenta cuenta)
        {
            return new CuentaDto
            {
                IdCuenta = cuenta.IdCuenta,
                IdUsuario = cuenta.IdUsuario,
                NumeroCuenta = cuenta.NumeroCuenta,
                TipoCuenta = cuenta.TipoCuenta,
                SaldoActual = cuenta.SaldoActual,
                Estado = cuenta.Estado,
                FechaApertura = cuenta.FechaApertura
            };
        }
    }
}
