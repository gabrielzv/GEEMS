using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BackendGeems.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetDuenoEmpresaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public GetDuenoEmpresaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("{cedulaPersona}")]
        public IActionResult getDuenoEmpresa(int cedulaPersona)
        {
            Console.WriteLine($"Cédula recibida: {cedulaPersona}");
            try
            {
                using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                conn.Open();

                // Primera consulta: obtener datos del dueño
                string queryDueno = "SELECT * FROM DuenoEmpresa WHERE CedulaPersona = @cedulaPersona";
                using SqlCommand cmdDueno = new SqlCommand(queryDueno, conn);
                cmdDueno.Parameters.AddWithValue("@cedulaPersona", cedulaPersona);

                // Guardamos los valores necesarios antes de cerrar el reader
                Guid id = Guid.Empty;
                string cedulaEmpresa = null;
                int cedulaPersonaValue = 0;

                using (SqlDataReader readerDueno = cmdDueno.ExecuteReader())
                {
                    if (readerDueno.Read())
                    {
                        id = (Guid)readerDueno["Id"];
                        cedulaEmpresa = readerDueno["CedulaEmpresa"].ToString();
                        cedulaPersonaValue = Convert.ToInt32(readerDueno["CedulaPersona"]);
                    }
                    else
                    {
                        return NotFound(new { message = "Dueño de empresa no encontrado" });
                    }
                } // El reader se cierra automáticamente al salir del using

                // Segunda consulta: obtener nombre de la empresa
                string nombreEmpresa = null;
                string queryEmpresa = "SELECT Nombre FROM Empresa WHERE CedulaJuridica = @cedulaEmpresa";
                using (SqlCommand cmdEmpresa = new SqlCommand(queryEmpresa, conn))
                {
                    cmdEmpresa.Parameters.AddWithValue("@cedulaEmpresa", cedulaEmpresa);
                    using (SqlDataReader readerEmpresa = cmdEmpresa.ExecuteReader())
                    {
                        if (readerEmpresa.Read())
                        {
                            nombreEmpresa = readerEmpresa["Nombre"].ToString();
                        }
                    }
                }

                return Ok(new
                {
                    id = id,
                    cedulaEmpresa = cedulaEmpresa,
                    cedulaPersona = cedulaPersonaValue,
                    nombreEmpresa = nombreEmpresa
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno", error = ex.Message });
            }
        }
    }
}