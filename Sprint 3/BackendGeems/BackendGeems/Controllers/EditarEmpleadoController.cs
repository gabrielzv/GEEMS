using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using BackendGeems.Domain;

namespace BackendGeems.Domain
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public EmpleadosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("editarEmpleado")]
        public IActionResult EditarEmpleado([FromBody] EmpleadoUpdateDto empleado)
        {
            // Validar que el objeto recibido no sea nulo y que tenga una cédula válida
            if (empleado == null || empleado.CedulaPersona == 0)
            {
                return BadRequest(new { message = "Datos del empleado inválidos." });
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();

                    string updateQuery = @"
                        UPDATE Empleado
                        SET 
                            Contrato = @Contrato,
                            NumHorasTrabajadas = @NumHorasTrabajadas,
                            Genero = @Genero,
                            EstadoLaboral = @EstadoLaboral,
                            SalarioBruto = @SalarioBruto,
                            Tipo = @Tipo,
                            FechaIngreso = @FechaIngreso,
                            NombreEmpresa = @NombreEmpresa
                        WHERE CedulaPersona = @CedulaPersona";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@CedulaPersona", empleado.CedulaPersona);
                        cmd.Parameters.AddWithValue("@Contrato", empleado.Contrato ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@NumHorasTrabajadas", empleado.NumHorasTrabajadas);
                        cmd.Parameters.AddWithValue("@Genero", empleado.Genero ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@EstadoLaboral", empleado.EstadoLaboral ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SalarioBruto", empleado.SalarioBruto);
                        cmd.Parameters.AddWithValue("@Tipo", empleado.Tipo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaIngreso", empleado.FechaIngreso ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@NombreEmpresa", empleado.NombreEmpresa ?? (object)DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Empleado actualizado exitosamente" });
                        }
                        else
                        {
                            return NotFound(new { message = "Empleado no encontrado." });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                string errorMessage = ex.Number switch
                {
                    547 => "Error de constraints: Valores no permitidos según las reglas de la base de datos",
                    _ => "Error en la base de datos al actualizar el empleado"
                };

                return StatusCode(500, new
                {
                    message = errorMessage,
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error interno al actualizar el empleado",
                    error = ex.Message
                });
            }
        }
    }
}
