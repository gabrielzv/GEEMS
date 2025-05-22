using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BackendGeems.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckDupeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CheckDupeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("username/{username}")]
        public IActionResult IsUsernameDuplicate(string username)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                conn.Open();

                string query = "SELECT COUNT(*) FROM Usuario WHERE Username = @Username";
                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                int count = (int)cmd.ExecuteScalar();
                return Ok(count > 0); // Devuelve true si el username ya existe
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno", error = ex.Message });
            }
        }

        [HttpGet("correo/{correo}")]
        public IActionResult IsCorreoDuplicate(string correo)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                conn.Open();

                string query = "SELECT COUNT(*) FROM Persona WHERE Correo = @Correo";
                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Correo", correo);

                int count = (int)cmd.ExecuteScalar();
                return Ok(count > 0); // Devuelve true si el correo ya existe
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno", error = ex.Message });
            }
        }

        [HttpGet("cedula/{cedula}")]
        public IActionResult IsCedulaDuplicate(int cedula)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                conn.Open();

                string query = "SELECT COUNT(*) FROM Persona WHERE Cedula = @Cedula";
                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Cedula", cedula);

                int count = (int)cmd.ExecuteScalar();
                return Ok(count > 0); // Devuelve true si la cédula ya existe
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno", error = ex.Message });
            }
        }
    }
}