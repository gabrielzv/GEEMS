using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using BackendGeems.Domain;

namespace BackendGeems.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class BeneficioController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public BeneficioController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("crearBeneficio")]
        public IActionResult CrearBeneficio([FromBody] Beneficio beneficio)
        {
            if (string.IsNullOrWhiteSpace(beneficio.Nombre) || 
                string.IsNullOrWhiteSpace(beneficio.Descripcion) || 
                beneficio.Costo <= 0 || 
                beneficio.TiempoMinimo < 0 || 
                string.IsNullOrWhiteSpace(beneficio.CedulaJuridica))
            {
                return BadRequest("Todos los campos son obligatorios y deben ser válidos.");
            }
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    // Se verifica si el nombre del beneficio ya existe
                    var checkQuery = "SELECT COUNT(*) FROM Beneficio WHERE Nombre = @Nombre";
                    using (var checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Nombre", beneficio.Nombre);
                        int count = (int)checkCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            return BadRequest("El nombre del beneficio ya está en uso. Elige otro.");
                        }
                    }
                    // Se hace la inserción del nuevo beneficio
                    var query = "INSERT INTO Beneficio (Id, Costo, TiempoMinimoEnEmpresa, Descripcion, Nombre, CedulaJuridica) " +
                                "VALUES (NEWID(), @Costo, @TiempoMinimo, @Descripcion, @Nombre, @CedulaJuridica)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Costo", beneficio.Costo);
                        command.Parameters.AddWithValue("@TiempoMinimo", beneficio.TiempoMinimo);
                        command.Parameters.AddWithValue("@Descripcion", beneficio.Descripcion);
                        command.Parameters.AddWithValue("@Nombre", beneficio.Nombre);
                        command.Parameters.AddWithValue("@CedulaJuridica", beneficio.CedulaJuridica);
                        
                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Beneficio creado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el beneficio: {ex.Message}");
            }
        }
    }
}