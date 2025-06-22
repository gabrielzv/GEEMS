using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace BackendGeems.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetEmployeeBenefitsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GetEmployeeBenefitsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("{IdEmpleado}")]
        public IActionResult GetEmployeeBenefits(string IdEmpleado)
        {
            if (string.IsNullOrWhiteSpace(IdEmpleado))
            {
                return BadRequest(new { message = "El ID del empleado es obligatorio y debe ser válido." });
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT b.Nombre AS NombreBeneficio, b.Descripcion, b.Costo, b.Frecuencia
                    FROM BeneficiosEmpleado be
                    JOIN Beneficio b ON be.IdBeneficio = b.Id
                    WHERE be.IdEmpleado = @IdEmpleado;";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdEmpleado", IdEmpleado);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                List<object> beneficiosEmpleado = new List<object>();

                while (reader.Read())
                {
                    beneficiosEmpleado.Add(new
                    {
                        Nombre = reader["NombreBeneficio"].ToString(),
                        Descripcion = reader["Descripcion"].ToString(),
                        Costo = Convert.ToInt32(reader["Costo"]),
                        Frecuencia = reader["Frecuencia"].ToString(),
                    });
                }

                if (beneficiosEmpleado.Count > 0)
                {
                    return Ok(beneficiosEmpleado);
                }
                else
                {
                    return NotFound(new { message = "No se encontraron beneficios matriculados." });
                }
            }
        }
    }
}