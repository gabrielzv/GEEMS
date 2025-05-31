using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BackendGeems.Controllers
{
    [Route("api/national-register")]
    [ApiController]
    public class RegistroController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RegistroController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("validate/{legalId}")]
        public async Task<IActionResult> ValidateLegalId(string legalId)
        {
            try
            {
                using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
                conn.Open();

                string query = @"
                    SELECT 
                        metodo, 
                        url_completa, 
                        nombre_key_header, 
                        valor_key_header 
                    FROM API_guardadas 
                    WHERE nombre_api = 'Registro Nacional'";

                using SqlCommand cmd = new(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return NotFound("API Registro Nacional no configurada");

                string url = reader["url_completa"].ToString();
                string keyName = reader["nombre_key_header"]?.ToString();
                string keyValue = reader["valor_key_header"]?.ToString();

                if (string.IsNullOrEmpty(keyName) || string.IsNullOrEmpty(keyValue))
                    return BadRequest("Configuración de API incompleta");

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add(keyName, keyValue);

                var response = await httpClient.PostAsJsonAsync(url, new { legalId });

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Error al llamar al Registro Nacional");

                return Content(await response.Content.ReadAsStringAsync(), "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno", error = ex.Message });
            }
        }
    }
}