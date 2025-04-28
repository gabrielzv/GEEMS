using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendGeems.Models;
using BackendGeems.Data;
using Microsoft.AspNetCore.Identity.Data;

namespace BackendGeems.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Usuario request)
        {
            if (string.IsNullOrEmpty(request.NombreUsuario) || string.IsNullOrEmpty(request.Contrasena))
            {
                return BadRequest(new { message = "Debe completar todos los campos" });
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == request.NombreUsuario);

            if (usuario == null || usuario.Contrasena != request.Contrasena)
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            return Ok(new { message = "Inicio de sesión exitoso" });
        }
    }
}
