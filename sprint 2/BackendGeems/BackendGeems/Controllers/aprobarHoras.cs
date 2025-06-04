using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using MiProyecto.DTOs;

namespace MiProyecto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistroController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RegistroController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("actualizar-estado")]
        public async Task<IActionResult> ActualizarEstadoRegistro([FromBody] UpdateEstadoRegistroRequest request)
        {
            if (request == null)
                return BadRequest("La petición no puede ser nula.");

            string resultado;
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ActualizarEstadoRegistro", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        cmd.Parameters.Add(new SqlParameter("@IdRegistro", SqlDbType.UniqueIdentifier)
                        {
                            Value = request.IdRegistro
                        });
                        cmd.Parameters.Add(new SqlParameter("@OpcionEstado", SqlDbType.Int)
                        {
                            Value = request.OpcionEstado
                        });

                        // Parámetro de salida
                        var outputResult = new SqlParameter("@Resultado", SqlDbType.VarChar, 50)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputResult);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        resultado = outputResult.Value != null ? outputResult.Value.ToString() : "No se obtuvo resultado";
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al ejecutar el SP: {ex.Message}");
            }

            return Ok(new { mensaje = resultado });
        }
    }
}
