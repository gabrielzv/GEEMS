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
                string.IsNullOrWhiteSpace(beneficio.Frecuencia) ||
                string.IsNullOrWhiteSpace(beneficio.CedulaJuridica) ||
                string.IsNullOrWhiteSpace(beneficio.NombreDeAPI)
                )
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
                            var insertQuery = "INSERT INTO Beneficio (Id, Costo, TiempoMinimoEnEmpresa, Frecuencia, Descripcion, Nombre, CedulaJuridica, NombreDeAPI, EsAPI) " +
                                              "VALUES (@Id, @Costo, @TiempoMinimo, @Frecuencia, @Descripcion, @Nombre, @CedulaJuridica, @NombreDeAPI, @EsApi)";
                            using (var insertCommand = new SqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@Id", beneficioId);
                                insertCommand.Parameters.AddWithValue("@Costo", beneficio.Costo);
                                insertCommand.Parameters.AddWithValue("@TiempoMinimo", beneficio.TiempoMinimo);
                                insertCommand.Parameters.AddWithValue("@Frecuencia", beneficio.Frecuencia);
                                insertCommand.Parameters.AddWithValue("@Descripcion", beneficio.Descripcion);
                                insertCommand.Parameters.AddWithValue("@Nombre", beneficio.Nombre);
                                insertCommand.Parameters.AddWithValue("@CedulaJuridica", beneficio.CedulaJuridica);
                                insertCommand.Parameters.AddWithValue("@NombreDeAPI", beneficio.NombreDeAPI);
                                insertCommand.Parameters.AddWithValue("@EsApi", beneficio.EsApi);

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
        // Método para poder modificar un beneficio ya registrado
        [HttpPost("editarBeneficio")]
        public IActionResult EditarBeneficio([FromBody] Beneficio beneficio)
        {
            if (string.IsNullOrWhiteSpace(beneficio.Nombre) ||
                string.IsNullOrWhiteSpace(beneficio.Descripcion) ||
                beneficio.Costo <= 0 ||
                beneficio.TiempoMinimo < 0 ||
                string.IsNullOrWhiteSpace(beneficio.Frecuencia) ||
                string.IsNullOrWhiteSpace(beneficio.CedulaJuridica) ||
                string.IsNullOrWhiteSpace(beneficio.NombreDeAPI))
            {
                return BadRequest("Todos los campos son obligatorios y deben ser válidos.");
            }
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    // Se verifica si el nombre del beneficio ya existe para esa empresa, excluyendo el propio beneficio
                    var checkQuery = @"
                        SELECT COUNT(*) FROM Beneficio
                        WHERE Nombre = @Nombre AND CedulaJuridica = @CedulaJuridica AND Id <> @Id";
                    using (var checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Nombre", beneficio.Nombre);
                        checkCommand.Parameters.AddWithValue("@CedulaJuridica", beneficio.CedulaJuridica);
                        checkCommand.Parameters.AddWithValue("@Id", beneficio.Id);

                        int count = (int)checkCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            return BadRequest("Ya existe un beneficio con el mismo nombre para esta empresa.");
                        }
                    }

                    // Actualizar el beneficio
                    var updateQuery = @"
                        UPDATE Beneficio
                        SET 
                            Nombre = @Nombre,
                            Descripcion = @Descripcion,
                            Costo = @Costo,
                            TiempoMinimoEnEmpresa = @TiempoMinimo,
                            Frecuencia = @Frecuencia,
                            NombreDeAPI = @NombreDeAPI,
                            EsAPI = @EsApi
                        WHERE Id = @Id";
                    using (var cmd = new SqlCommand(updateQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", beneficio.Id);
                        cmd.Parameters.AddWithValue("@Nombre", beneficio.Nombre);
                        cmd.Parameters.AddWithValue("@Descripcion", beneficio.Descripcion);
                        cmd.Parameters.AddWithValue("@Costo", beneficio.Costo);
                        cmd.Parameters.AddWithValue("@TiempoMinimo", beneficio.TiempoMinimo);
                        cmd.Parameters.AddWithValue("@Frecuencia", beneficio.Frecuencia);
                        cmd.Parameters.AddWithValue("@NombreDeAPI", beneficio.NombreDeAPI);
                        cmd.Parameters.AddWithValue("@EsApi", beneficio.EsApi);

                        cmd.ExecuteNonQuery();
                    }

                    // Se eliminan los contratos elegibles antiguos
                    var deleteQuery = "DELETE FROM BeneficioContratoElegible WHERE IdBeneficio = @IdBeneficio";
                    using (var deleteCommand = new SqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@IdBeneficio", beneficio.Id);
                        deleteCommand.ExecuteNonQuery();
                    }

                    // Se insertan contratos elegibles
                    if (beneficio.ContratosElegibles != null && beneficio.ContratosElegibles.Count > 0)
                    {
                        var contratoQuery = "INSERT INTO BeneficioContratoElegible (IdBeneficio, ContratoEmpleado) VALUES (@IdBeneficio, @ContratoEmpleado)";
                        foreach (var contrato in beneficio.ContratosElegibles)
                        {
                            using (var contratoCommand = new SqlCommand(contratoQuery, connection))
                            {
                                contratoCommand.Parameters.AddWithValue("@IdBeneficio", beneficio.Id);
                                contratoCommand.Parameters.AddWithValue("@ContratoEmpleado", contrato);
                                contratoCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }

                return Ok("Beneficio editado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al editar el beneficio: {ex.Message}");
            }
        }

        // Método para obtener un beneficio por su ID
        [HttpGet("{id}")]
        public IActionResult GetBeneficio(Guid id)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var query = @"SELECT Id, Costo, TiempoMinimoEnEmpresa, Frecuencia, Descripcion, Nombre, CedulaJuridica, NombreDeAPI, EsAPI
                                FROM Beneficio WHERE Id = @Id";
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var beneficio = new
                                {
                                    Id = reader["Id"],
                                    Costo = reader["Costo"],
                                    TiempoMinimo = reader["TiempoMinimoEnEmpresa"],
                                    Frecuencia = reader["Frecuencia"],
                                    Descripcion = reader["Descripcion"],
                                    Nombre = reader["Nombre"],
                                    CedulaJuridica = reader["CedulaJuridica"],
                                    NombreDeAPI = reader["NombreDeAPI"],
                                    EsApi = reader["EsAPI"]
                                };
                                return Ok(beneficio);
                            }
                            else
                            {
                                return NotFound("Beneficio no encontrado.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el beneficio: {ex.Message}");
            }
        }
        
    }
}