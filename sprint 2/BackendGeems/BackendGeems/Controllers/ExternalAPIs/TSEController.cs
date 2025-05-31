using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BackendGeems.Controllers
{
    [Route("api/tse")]
    [ApiController]
    public class TseController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("validar-cedula/{cedula}")]
        public IActionResult ValidarCedula(string cedula)
        {
            try
            {
                // 1. Obtener configuración de la API desde BD
                using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
                conn.Open();

                string query = @"
                    SELECT 
                        metodo, 
                        url_completa, 
                        nombre_key_header, 
                        valor_key_header 
                    FROM API_guardadas 
                    WHERE nombre_api = 'Validación TSE'";

                using SqlCommand cmd = new(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return NotFound("API TSE no configurada en el sistema");

                // 2. Validar valores antes de usarlos
                string metodo = reader["metodo"].ToString();
                string url = reader["url_completa"].ToString();
                string keyName = reader["nombre_key_header"]?.ToString();
                string keyValue = reader["valor_key_header"]?.ToString();

                if (string.IsNullOrEmpty(keyName) || string.IsNullOrEmpty(keyValue))
                    return BadRequest("Configuración de API incompleta");

                // 3. Llamar a la API externa
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add(keyName, keyValue);

                string apiUrl = $"{url}/{cedula}";
                var response = httpClient.GetAsync(apiUrl).Result;

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Error al llamar al TSE");

                // 4. Devolver respuesta
                return Content(response.Content.ReadAsStringAsync().Result, "application/json");
            }
            catch (SqlException sqlEx)
            {
                return StatusCode(500, new { message = "Error de base de datos", error = sqlEx.Message });
            }
            catch (HttpRequestException httpEx)
            {
                return StatusCode(502, new { message = "Error al conectar con API externa", error = httpEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno", error = ex.Message });
            }
        }
    }
}