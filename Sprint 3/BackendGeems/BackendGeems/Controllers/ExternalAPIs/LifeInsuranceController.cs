using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BackendGeems.Controllers
{
    [Route("api/life-insurance")]
    [ApiController]
    public class LifeInsuranceController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LifeInsuranceController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetPolicyInfo([FromQuery] string birthDate, [FromQuery] string sex)
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
                    WHERE nombre_api = 'Poliza Seguros'";

                using SqlCommand cmd = new(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return NotFound("API de seguros no configurada");

                string url = reader["url_completa"].ToString();
                string keyName = reader["nombre_key_header"]?.ToString();
                string keyValue = reader["valor_key_header"]?.ToString();

                if (string.IsNullOrEmpty(keyName) || string.IsNullOrEmpty(keyValue))
                    return BadRequest("Configuración de API incompleta");

                var queryParams = System.Web.HttpUtility.ParseQueryString(string.Empty);
                queryParams["date of birth"] = birthDate;
                queryParams["sex"] = sex;

                string fullUrl = $"{url}?{queryParams}";
                Console.WriteLine($"Consultando URL: {fullUrl} con header {keyName}: {keyValue}");
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add(keyName, keyValue);

                var response = await httpClient.GetAsync(fullUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al consultar pólizas: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    return StatusCode((int)response.StatusCode, "Error al consultar pólizas");
                }



                return Content(await response.Content.ReadAsStringAsync(), "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error interno al consultar pólizas",
                    error = ex.Message
                });
            }
        }
    }
}