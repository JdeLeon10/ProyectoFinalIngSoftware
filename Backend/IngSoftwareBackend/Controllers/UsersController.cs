using IngSoftwareBackend.Data;
using IngSoftwareBackend.DTOs;
using IngSoftwareBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IngSoftwareBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        public readonly BancaEnLineaDbContext _context;

        public UsersController(BancaEnLineaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/allusers")]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsers()
        {
            var users = await _context.Usuarios.Select(u => new UsuarioDto
            {
                IdUsuario = u.IdUsuario,
                IdRol = u.IdRol,
                Nombres = u.Nombres,
                Apellidos = u.Apellidos,
                Dpi = u.Dpi,
                Email = u.Email,
                Telefono = u.Telefono,
                Direccion = u.Direccion,
                Estado = u.Estado,
                FechaCreacion = u.FechaCreacion
            }).ToListAsync();

            return Ok(users);
        }

        [HttpGet]
        [Route("user/{id}")]
        public async Task<ActionResult<UsuarioDto>> GetUserById(int id)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(ToDto(user));
        }

        [HttpPost]
        [Route("/createuser")]
        public async Task<ActionResult<UsuarioDto>> CreateUser(UsuarioDto usuarioDto)
        {
            var validation = await ValidateUser(usuarioDto);
            if (validation != null)
            {
                return validation;
            }

            if (await _context.Usuarios.AnyAsync(u => u.Dpi == usuarioDto.Dpi.Trim()))
            {
                return BadRequest("Ya existe un usuario con el mismo DPI.");
            }
            if (await _context.Usuarios.AnyAsync(u => u.Email == usuarioDto.Email.Trim()))
            {
                return BadRequest("Ya existe un usuario con el mismo email.");
            }

            var user = new Usuario
            {
                IdRol = usuarioDto.IdRol,
                Nombres = usuarioDto.Nombres.Trim(),
                Apellidos = usuarioDto.Apellidos.Trim(),
                Dpi = usuarioDto.Dpi.Trim(),
                Email = usuarioDto.Email.Trim(),
                Telefono = usuarioDto.Telefono,
                Direccion = usuarioDto.Direccion,
                Estado = "Activo"
            };

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserById), new { id = user.IdUsuario }, ToDto(user));
        }

        [HttpPut]
        [Route("edituser/{id}")]
        public async Task<ActionResult<UsuarioDto>> UpdateUser(int id, UsuarioDto usuarioDto)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);
            if (user == null)
            {
                return NotFound();
            }

            var validation = await ValidateUser(usuarioDto);
            if (validation != null)
            {
                return validation;
            }

            if (await _context.Usuarios.AnyAsync(u => u.IdUsuario != id && u.Dpi == usuarioDto.Dpi.Trim()))
            {
                return BadRequest("Ya existe un usuario con el mismo DPI.");
            }
            if (await _context.Usuarios.AnyAsync(u => u.IdUsuario != id && u.Email == usuarioDto.Email.Trim()))
            {
                return BadRequest("Ya existe un usuario con el mismo email.");
            }

            user.IdRol = usuarioDto.IdRol;
            user.Nombres = usuarioDto.Nombres.Trim();
            user.Apellidos = usuarioDto.Apellidos.Trim();
            user.Dpi = usuarioDto.Dpi.Trim();
            user.Email = usuarioDto.Email.Trim();
            user.Telefono = usuarioDto.Telefono;
            user.Direccion = usuarioDto.Direccion;

            await _context.SaveChangesAsync();

            return Ok(ToDto(user));
        }

        [HttpPut]
        [Route("toggleuser/{id}")]
        public async Task<ActionResult<UsuarioDto>> ToggleUser(int id)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);
            if (user == null)
            {
                return NotFound();
            }

            user.Estado = user.Estado == "Activo" ? "Inactivo" : "Activo";
            await _context.SaveChangesAsync();

            return Ok(ToDto(user));
        }

        [HttpDelete]
        [Route("deleteuser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Estado == "Activo")
            {
                return BadRequest("No se puede eliminar un usuario activo.");
            }

            _context.Usuarios.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<ActionResult?> ValidateUser(UsuarioDto usuarioDto)
        {
            if (!await _context.Rols.AnyAsync(r => r.IdRol == usuarioDto.IdRol))
            {
                return BadRequest("El rol indicado no existe.");
            }
            if (string.IsNullOrWhiteSpace(usuarioDto.Nombres) || string.IsNullOrWhiteSpace(usuarioDto.Apellidos) || string.IsNullOrWhiteSpace(usuarioDto.Dpi) || string.IsNullOrWhiteSpace(usuarioDto.Email))
            {
                return BadRequest("Nombres, apellidos, DPI y email son requeridos.");
            }

            return null;
        }

        private static UsuarioDto ToDto(Usuario user)
        {
            return new UsuarioDto
            {
                IdUsuario = user.IdUsuario,
                IdRol = user.IdRol,
                Nombres = user.Nombres,
                Apellidos = user.Apellidos,
                Dpi = user.Dpi,
                Email = user.Email,
                Telefono = user.Telefono,
                Direccion = user.Direccion,
                Estado = user.Estado,
                FechaCreacion = user.FechaCreacion
            };
        }
    }
}
