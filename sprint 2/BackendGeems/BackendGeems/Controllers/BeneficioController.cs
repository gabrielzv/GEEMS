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
                beneficio.TiempoMinimo <= 0 ||
                string.IsNullOrWhiteSpace(beneficio.Frecuencia) ||
                string.IsNullOrWhiteSpace(beneficio.CedulaJuridica))
            {
                return BadRequest("Todos los campos son obligatorios y deben ser válidos.");
            }
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    // Se verifica si el nombre del beneficio ya existe para esa empresa
                    Guid beneficioId;
                    var checkQuery = "SELECT Id FROM Beneficio WHERE Nombre = @Nombre AND CedulaJuridica = @CedulaJuridica";
                    using (var checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Nombre", beneficio.Nombre);
                        checkCommand.Parameters.AddWithValue("@CedulaJuridica", beneficio.CedulaJuridica);

                        var result = checkCommand.ExecuteScalar();
                        if (result != null)
                        {
                            return BadRequest("Ya existe un beneficio con el mismo nombre para esta empresa.");
                        }
                        else
                        {
                            // Se hace la inserción del nuevo beneficio
                            beneficioId = Guid.NewGuid();
                            var insertQuery = "INSERT INTO Beneficio (Id, Costo, TiempoMinimoEnEmpresa, Frecuencia, Descripcion, Nombre, CedulaJuridica) " +
                                              "VALUES (@Id, @Costo, @TiempoMinimo, @Frecuencia, @Descripcion, @Nombre, @CedulaJuridica)";
                            using (var insertCommand = new SqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@Id", beneficioId);
                                insertCommand.Parameters.AddWithValue("@Costo", beneficio.Costo);
                                insertCommand.Parameters.AddWithValue("@TiempoMinimo", beneficio.TiempoMinimo);
                                insertCommand.Parameters.AddWithValue("@Frecuencia", beneficio.Frecuencia);
                                insertCommand.Parameters.AddWithValue("@Descripcion", beneficio.Descripcion);
                                insertCommand.Parameters.AddWithValue("@Nombre", beneficio.Nombre);
                                insertCommand.Parameters.AddWithValue("@CedulaJuridica", beneficio.CedulaJuridica);

                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }

                    // Se insertan los contratos elegibles seleccionados
                    if (beneficio.ContratosElegibles != null && beneficio.ContratosElegibles.Count > 0)
                    {
                        var contratoQuery = "INSERT INTO BeneficioContratoElegible (IdBeneficio, ContratoEmpleado) VALUES (@IdBeneficio, @ContratoEmpleado)";
                        foreach (var contrato in beneficio.ContratosElegibles)
                        {
                            using (var contratoCommand = new SqlCommand(contratoQuery, connection))
                            {
                                contratoCommand.Parameters.AddWithValue("@IdBeneficio", beneficioId);
                                contratoCommand.Parameters.AddWithValue("@ContratoEmpleado", contrato);
                                contratoCommand.ExecuteNonQuery();
                            }
                        }
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