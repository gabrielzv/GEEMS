using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BackendGeems.Controllers
{
    [Route("api/insurance")]
    [ApiController]
    public class InsuranceController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public InsuranceController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateInsurance([FromBody] InsuranceCalculationRequest request)
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
                    WHERE nombre_api = 'MediSeguro'";

                using SqlCommand cmd = new(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return NotFound("API de seguros médicos no configurada");

                string url = reader["url_completa"].ToString();
                string keyName = reader["nombre_key_header"]?.ToString();
                string keyValue = reader["valor_key_header"]?.ToString();

                if (string.IsNullOrEmpty(keyName) || string.IsNullOrEmpty(keyValue))
                    return BadRequest("Configuración de API incompleta");

                var payload = new {
                    fechaNacimiento = request.FechaNacimiento,
                    genero = request.Genero,
                    cantidadDependientes = request.CantidadDependientes
                };

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add(keyName, keyValue);

                var response = await httpClient.PostAsJsonAsync(url, payload);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Error al calcular seguro médico");

                return Content(await response.Content.ReadAsStringAsync(), "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Error interno al calcular seguro", 
                    error = ex.Message 
                });
            }
        }
    }

    public class InsuranceCalculationRequest
    {
        public string FechaNacimiento { get; set; } // Formato: "YYYY-MM-DD"
        public string Genero { get; set; } // "masculino" o "femenino"
        public int CantidadDependientes { get; set; }
    }
}