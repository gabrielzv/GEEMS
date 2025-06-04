using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using BackendGeems.Domain;
using System;

namespace BackendGeems.Controllers
{
    [Route("api/Empleado")]
    [ApiController]
    public class EmpleadoController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public EmpleadoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // POST: api/Empleado/{cedula}
        [HttpPost("actualizar/{cedula}")]
        public IActionResult ActualizarEmpleado(string cedula, [FromBody] Empleado empleado)
        {
            if (empleado == null)
            {
                return BadRequest(new { message = "El objeto Empleado es nulo." });
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                UPDATE Empleado
                SET Tipo = @Tipo,
                    SalarioBruto = @SalarioBruto,
                    FechaIngreso = @FechaIngreso,
                    Contrato = @Contrato
                WHERE CedulaPersona = @CedulaPersona";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Tipo", empleado.Tipo);
                        command.Parameters.AddWithValue("@SalarioBruto", empleado.SalarioBruto);
                        command.Parameters.AddWithValue("@FechaIngreso", empleado.FechaIngreso);
                        command.Parameters.AddWithValue("@Contrato", empleado.Contrato);
                        command.Parameters.AddWithValue("@CedulaPersona", cedula);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Datos de empleado actualizados correctamente." });
                        }
                        else
                        {
                            return NotFound(new { message = "No se encontró un empleado con la cédula proporcionada." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el empleado: " + ex.Message });
            }
        }

    }
}
