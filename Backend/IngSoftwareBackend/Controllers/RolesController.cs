using IngSoftwareBackend.Data;
using IngSoftwareBackend.DTOs;
using IngSoftwareBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IngSoftwareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : Controller
    {
        public readonly BancaEnLineaDbContext _context;

        public RolesController(BancaEnLineaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/allroles")]
        public async Task<ActionResult<IEnumerable<RolDto>>> GetRoles()
        {
            var roles = await _context.Rols.
                Select(r => new RolDto
            {
                IdRol = r.IdRol,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion,
                Activo = r.Activo
            }).ToListAsync();

            return Ok(roles);
        }

        [HttpGet]
        [Route("rol/{id}")]
        public async Task<ActionResult<RolDto>> GetRoleById(int id)
        {
            var role = await _context.Rols.
                FirstOrDefaultAsync(r => r.IdRol == id);

            if (role == null)
            {
                return NotFound();
            }   

            var roleDto = new RolDto
            {
                IdRol = role.IdRol,
                Nombre = role.Nombre,
                Descripcion = role.Descripcion,
                Activo = role.Activo
            };

            return Ok(roleDto);
        }

        [HttpPost]
        [Route("/createrol")]
        public async Task<ActionResult<RolDto>> CreateRole(RolDto rolDto)
        {
            var nombre = rolDto.Nombre?.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest("El nombre del rol es requerido.");
            }

            var existeRol = await _context.Rols
                .AnyAsync(r => r.Nombre == nombre);

            if (existeRol)
            {
                return BadRequest("Ya existe un rol con el mismo nombre.");
            }

            var rol = new Rol
            {
                Nombre = nombre,
                Descripcion = rolDto.Descripcion
            };

            _context.Rols.Add(rol);
            await _context.SaveChangesAsync();

            var nuevoRolDto = new RolDto
            {
                IdRol = rol.IdRol,
                Nombre = rol.Nombre,
                Descripcion = rol.Descripcion,
                Activo = rol.Activo
            };

            return CreatedAtAction(nameof(GetRoleById), new { id = rol.IdRol }, nuevoRolDto);
        }

        [HttpPut]
        [Route("editrol/{id}")]
        public async Task<ActionResult<RolDto>> UpdateRole(int id, RolDto rolDto)
        {
            var role = await _context.Rols.FirstOrDefaultAsync(r => r.IdRol == id);

            if (role == null)
            {
                return NotFound();
            }

            var nombre = rolDto.Nombre?.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest("El nombre del rol es requerido.");
            }

            var existeRol = await _context.Rols
                .AnyAsync(r => r.IdRol != id && r.Nombre == nombre);

            if (existeRol)
            {
                return BadRequest("Ya existe un rol con el mismo nombre.");
            }

            role.Nombre = nombre;
            role.Descripcion = rolDto.Descripcion;

            await _context.SaveChangesAsync();

            var roleDto = new RolDto
            {
                IdRol = role.IdRol,
                Nombre = role.Nombre,
                Descripcion = role.Descripcion,
                Activo = role.Activo
            };

            return Ok(roleDto);
        }

        [HttpPut]
        [Route("togglerol/{id}")]
        public async Task<ActionResult<RolDto>> ToggleRole(int id)
        {
            var role = await _context.Rols.FirstOrDefaultAsync(r => r.IdRol == id);

            if (role == null)
            {
                return NotFound();
            }

            role.Activo = !role.Activo;
            await _context.SaveChangesAsync();

            var roleDto = new RolDto
            {
                IdRol = role.IdRol,
                Nombre = role.Nombre,
                Descripcion = role.Descripcion,
                Activo = role.Activo
            };

            return Ok(roleDto);
        }

        [HttpDelete]
        [Route("deleterol/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Rols.FirstOrDefaultAsync(r => r.IdRol == id);

            if (role == null)
            {
                return NotFound();
            }

            if (role.Activo)
            {
                return BadRequest("No se puede eliminar un rol activo.");
            }

            _context.Rols.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
