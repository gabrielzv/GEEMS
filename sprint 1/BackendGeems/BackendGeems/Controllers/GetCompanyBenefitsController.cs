using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace BackendGeems.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetCompanyBenefitsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GetCompanyBenefitsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("{CedulaJuridica}")]
        public IActionResult GetCompanyBenefits(string CedulaJuridica)
        {
            if (string.IsNullOrWhiteSpace(CedulaJuridica))
            {
                return BadRequest(new { message = "El ID de la empresa es obligatorio y debe ser válido." });
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT b.Id, b.Nombre, b.Descripcion, b.Costo, b.TiempoMinimoEnEmpresa
                    FROM Beneficio b
                    INNER JOIN DuenoEmpresa de ON b.CedulaJuridica = de.CedulaEmpresa
                    WHERE de.CedulaEmpresa = @CedulaJuridica;";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CedulaJuridica", CedulaJuridica);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                List<object> beneficios = new List<object>();

                while (reader.Read())
                {
                    beneficios.Add(new
                    {
                        Id = reader["Id"].ToString(),
                        Nombre = reader["Nombre"].ToString(),
                        Descripcion = reader["Descripcion"].ToString(),
                        Costo = Convert.ToInt32(reader["Costo"]),
                        TiempoMinimoEnEmpresa = Convert.ToInt32(reader["TiempoMinimoEnEmpresa"])
                    });
                }

                if (beneficios.Count > 0)
                {
                    return Ok(beneficios);
                }
                else
                {
                    return NotFound(new { message = "No se encontraron beneficios para esta empresa." });
                }
            }
        }
    }
}