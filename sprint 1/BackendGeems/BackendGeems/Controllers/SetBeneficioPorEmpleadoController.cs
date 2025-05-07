using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;

namespace BackendGeems.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SetBeneficioPorEmpleadoController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SetBeneficioPorEmpleadoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class BeneficiosEmpleadoModel
        {
            public required string IdEmpleado { get; set; }
            public required string IdBeneficio { get; set; }
        }

        [HttpPost("matricularBeneficio")]
        public IActionResult MatricularBeneficio([FromBody] BeneficiosEmpleadoModel beneficiosEmpleado)
        {
            if (beneficiosEmpleado == null ||
            string.IsNullOrWhiteSpace(beneficiosEmpleado.IdEmpleado) ||
            string.IsNullOrWhiteSpace(beneficiosEmpleado.IdBeneficio))
            {
                return BadRequest("Los datos del beneficio y el empleado son obligatorios.");
            }

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = @"
                        INSERT INTO BeneficiosEmpleado (IdEmpleado, IdBeneficio)
                        VALUES (@IdEmpleado, @IdBeneficio);";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdEmpleado", beneficiosEmpleado.IdEmpleado);
                        command.Parameters.AddWithValue("@IdBeneficio", beneficiosEmpleado.IdBeneficio);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Beneficio matriculado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al matricular el beneficio: {ex.Message}");
            }
        }
        
    }
}