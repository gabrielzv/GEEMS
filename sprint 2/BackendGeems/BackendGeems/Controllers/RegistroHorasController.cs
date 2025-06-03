using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace BackendGeems.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistroHorasController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RegistroHorasController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("por-empresa/{cedulaJuridica}")]
        public IActionResult GetRegistrosPorEmpresa(string cedulaJuridica)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                conn.Open();

                string query = @"
                    SELECT 
                        r.Id,
                        p.NombrePila + ' ' + p.Apellido1 AS NombreEmpleado,
                        u.Username AS Usuario,
                        r.NumHoras AS Horas,
                        r.Fecha AS FechaSolicitud,
                        r.Estado
                    FROM Registro r
                    INNER JOIN Empleado e ON r.IdEmpleado = e.Id
                    INNER JOIN Persona p ON e.CedulaPersona = p.Cedula
                    INNER JOIN Usuario u ON p.Cedula = u.CedulaPersona
                    INNER JOIN Empresa emp ON e.NombreEmpresa = emp.Nombre
                    WHERE emp.CedulaJuridica = @CedulaJuridica
                    ORDER BY r.Fecha DESC";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CedulaJuridica", cedulaJuridica);
                
                using SqlDataReader reader = cmd.ExecuteReader();

                List<object> registros = new();

                while (reader.Read())
                {
                    registros.Add(new
                    {
                        id = reader["Id"],
                        nombreEmpleado = reader["NombreEmpleado"].ToString(),
                        usuario = reader["Usuario"].ToString(),
                        horas = reader["Horas"] != DBNull.Value ? (int?)reader["Horas"] : null,
                        fechaSolicitud = reader["FechaSolicitud"],
                        estado = reader["Estado"].ToString()
                    });
                }

                return Ok(registros);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener registros de horas", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEstadoRegistro(Guid id, [FromBody] dynamic request)
        {
            try
            {
                string estado = request.estado;

                using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                conn.Open();

                string query = @"
                    UPDATE Registro 
                    SET Estado = @Estado 
                    WHERE Id = @Id";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Estado", estado);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return NotFound(new { message = "Registro no encontrado" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el registro", error = ex.Message });
            }
        }
    }
}