using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
            Console.WriteLine($"Nombre de usuario recibido: {request.Username}");
            Console.WriteLine($"Contraseña:{ request.Contrasena}");
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Contrasena))
            {
                return BadRequest(new { message = "Debe completar todos los campos" });
            }


            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Usuario WHERE Username = @username";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", request.Username);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string contrasenaDB = reader["Contrasena"].ToString();

                    if (request.Contrasena == contrasenaDB)
                    {
                        return Ok(new { message = "Inicio de sesión exitoso" });
                    }
                    else
                    {
                        return Unauthorized(new { message = "Usuario o Contraseña Contraseña incorrecta" });
                    }
                }
                else
                {
                    return Unauthorized(new { message = "Usuario o Contraseña Incorrectos" });
                }
            }
        }
    }
}
