using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using BackendGeems.Models;

namespace BackendGeems.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Usuario request)
        {
            if (string.IsNullOrEmpty(request.NombreUsuario) || string.IsNullOrEmpty(request.Contrasena))
            {
                return BadRequest(new { message = "Debe completar todos los campos" });
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            Usuario usuario = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Usuarios WHERE NombreUsuario = @NombreUsuario";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NombreUsuario", request.NombreUsuario);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            Id = (int)reader["Id"], 
                            NombreUsuario = reader["NombreUsuario"].ToString(),
                            Contrasena = reader["Contrasena"].ToString()
                        };
                    }
                }
            }

            if (usuario == null || usuario.Contrasena != request.Contrasena)
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            return Ok(new { message = "Inicio de sesión exitoso" });
        }
    }
}
