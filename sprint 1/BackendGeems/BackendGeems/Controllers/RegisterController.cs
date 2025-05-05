using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using BackendGeems.Models;

namespace BackendGeems.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RegisterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Endpoint para registrar una Persona
        [HttpPost("persona")]
        public IActionResult RegistrarPersona([FromBody] Persona persona)
        {
            if (persona == null)
            {
                return BadRequest(new { message = "Todos los campos de la persona deben ser completados." });
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string personaQuery = "INSERT INTO Persona (Cedula, Direccion, Telefono, NombrePila, Apellido1, Apellido2, Correo) " +
                                          "VALUES (@Cedula, @Direccion, @Telefono, @NombrePila, @Apellido1, @Apellido2, @Correo)";
                    SqlCommand personaCmd = new SqlCommand(personaQuery, connection);
                    personaCmd.Parameters.AddWithValue("@Cedula", persona.Cedula);
                    personaCmd.Parameters.AddWithValue("@Direccion", persona.Direccion);
                    personaCmd.Parameters.AddWithValue("@Telefono", persona.Telefono);
                    personaCmd.Parameters.AddWithValue("@NombrePila", persona.NombrePila);
                    personaCmd.Parameters.AddWithValue("@Apellido1", persona.Apellido1);
                    personaCmd.Parameters.AddWithValue("@Apellido2", persona.Apellido2);
                    personaCmd.Parameters.AddWithValue("@Correo", persona.Correo);
                    personaCmd.ExecuteNonQuery();

                    return Ok(new { message = "Persona registrada exitosamente." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al registrar la persona: " + ex.Message });
            }
        }

        // Endpoint para registrar un Usuario
        [HttpPost("usuario")]
        public IActionResult RegistrarUsuario([FromBody] Usuario usuario)
        {
            if (usuario == null)
            {
                return BadRequest(new { message = "Todos los campos del usuario deben ser completados." });
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string usuarioQuery = "INSERT INTO Usuario (Id, Username, Contrasena, Tipo, CedulaPersona) " +
                                          "VALUES (@Id, @Username, @Contrasena, @Tipo, @CedulaPersona)";
                    SqlCommand usuarioCmd = new SqlCommand(usuarioQuery, connection);
                    usuarioCmd.Parameters.AddWithValue("@Id", usuario.Id);
                    usuarioCmd.Parameters.AddWithValue("@Username", usuario.Username);
                    usuarioCmd.Parameters.AddWithValue("@Contrasena", usuario.Contrasena);
                    usuarioCmd.Parameters.AddWithValue("@Tipo", usuario.Tipo);
                    usuarioCmd.Parameters.AddWithValue("@CedulaPersona", usuario.CedulaPersona);
                    usuarioCmd.ExecuteNonQuery();

                    return Ok(new { message = "Usuario registrado exitosamente." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al registrar el usuario: " + ex.Message });
            }
        }

        // Endpoint para registrar un DuenoEmpresa
        [HttpPost("duenoempresa")]
        public IActionResult RegistrarDuenoEmpresa([FromBody] DuenoEmpresa duenoEmpresa)
        {
            if (duenoEmpresa == null)
            {
                return BadRequest(new { message = "Todos los campos del dueño de la empresa deben ser completados." });
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string duenoEmpresaQuery = "INSERT INTO DuenoEmpresa (Id, CedulaEmpresa, CedulaPersona) " +
                                               "VALUES (@Id, @CedulaEmpresa, @CedulaPersona)";
                    SqlCommand duenoEmpresaCmd = new SqlCommand(duenoEmpresaQuery, connection);
                    duenoEmpresaCmd.Parameters.AddWithValue("@Id", duenoEmpresa.Id);
                    duenoEmpresaCmd.Parameters.AddWithValue("@CedulaEmpresa", duenoEmpresa.CedulaEmpresa);
                    duenoEmpresaCmd.Parameters.AddWithValue("@CedulaPersona", duenoEmpresa.CedulaPersona);
                    duenoEmpresaCmd.ExecuteNonQuery();

                    return Ok(new { message = "Dueño de empresa registrado exitosamente." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al registrar el dueño de la empresa: " + ex.Message });
            }
        }
    }
}
