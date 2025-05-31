using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net.Http.Headers;


namespace BackendGeems.Controllers
{
    [Route("api/association")]
    [ApiController]
    public class AssociationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AssociationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateAssociationFee([FromBody] AssociationCalculationRequest request)
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
                    WHERE nombre_api = 'Asociacion Calculator'";

                using SqlCommand cmd = new(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return NotFound("API de asociación no configurada");

                string url = reader["url_completa"].ToString();
                string keyName = reader["nombre_key_header"]?.ToString();
                string keyValue = reader["valor_key_header"]?.ToString();

                if (string.IsNullOrEmpty(keyName) || string.IsNullOrEmpty(keyValue))
                    return BadRequest("Configuración de API incompleta");

                var payload = new {
                    associationName = request.AssociationName,
                    employeeSalary = request.EmployeeSalary
                };

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add(keyName, keyValue);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await httpClient.PostAsJsonAsync(url, payload);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Error al calcular cuota de asociación");

                return Content(await response.Content.ReadAsStringAsync(), "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Error interno al calcular cuota", 
                    error = ex.Message 
                });
            }
        }
    }

    public class AssociationCalculationRequest
    {
        public string AssociationName { get; set; }
        public decimal EmployeeSalary { get; set; }
    }
}