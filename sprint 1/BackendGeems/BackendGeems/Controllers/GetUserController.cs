using Microsoft.AspNetCore.Mvc;
using BackendGeems.Models;
using Microsoft.Data.SqlClient;


namespace BackendGeems.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetUserController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GetUserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("getUser/{username}")]
        public IActionResult GetUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { message = "El nombre de usuario es obligatorio." });
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Usuario WHERE Username = @username";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {

                    Usuario usuario = new Usuario
                    {
                        Id = Guid.Parse(reader["Id"].ToString()),
                        Username = reader["Username"].ToString(),
                        Contrasena = reader["Contrasena"].ToString(),
                        Tipo = reader["Tipo"].ToString(),
                        CedulaPersona = Convert.ToInt32(reader["CedulaPersona"]),
                    };

                    return Ok(usuario);
                }
                else
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }
            }
        }
    }
}

