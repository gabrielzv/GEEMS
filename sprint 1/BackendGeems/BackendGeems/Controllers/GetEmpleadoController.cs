using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackendGeems.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetEmpleadoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public GetEmpleadoController( IConfiguration configuration)
        {
            _configuration = configuration;
        }
       
        [HttpGet("{cedula}")]
        public IActionResult getEmpleado(int cedula)
        {
            Console.WriteLine($"Cedula recibida: {cedula}");
            try
            {
                using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                conn.Open();

                string query = "SELECT * FROM Empleado WHERE CedulaPersona = @cedula";
                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@cedula", cedula);
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return Ok(new
                    {
                        id = reader["Id"],
                        cedulaPersona = reader["CedulaPersona"],
                        contrato = reader["Contrato"],
                        numHorasTrabajadas = reader["NumHorasTrabajadas"],
                        genero = reader["Genero"],
                        estadoLaboral = reader["EstadoLaboral"],
                        salarioBruto = reader["SalarioBruto"],
                        tipo = reader["Tipo"],
                        fechaIngreso = reader["FechaIngreso"],
                        nombreEmpresa = reader["NombreEmpresa"]


                    });
                }
                return NotFound(new { message = "Empleado No encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno", error = ex.Message });

            }
        }

        
    }
}
