using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
    public IActionResult Login([FromBody] Usuario login)
    {
        var user = _context.Usuarios.FirstOrDefault(u =>
            u.NombreUsuario == login.NombreUsuario &&
            u.Contrasena == login.Contrasena);

        if (user == null)
            return Unauthorized(new { success = false, message = "Credenciales incorrectas" });

        return Ok(new { success = true, message = "Login exitoso" });
    }
}
