using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace BackendGeems.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public EmpresaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetTodasLasEmpresas()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                conn.Open();

                string query = @"
                    SELECT 
                        e.CedulaJuridica,
                        e.Nombre,
                        e.Descripcion,
                        e.Telefono,
                        e.Provincia,
                        e.Canton,
                        e.Distrito,
                        e.Senas,
                        e.Correo,
                        ISNULL(p.NombrePila + ' ' + p.Apellido1 + ' ' + p.Apellido2, 'Sin asignar') AS NombreDueno
                    FROM Empresa e
                    LEFT JOIN DuenoEmpresa d ON e.CedulaJuridica = d.CedulaEmpresa
                    LEFT JOIN Persona p ON d.CedulaPersona = p.Cedula;";

                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                List<object> empresas = new();

                while (reader.Read())
                {
                    empresas.Add(new
                    {
                        cedulaJuridica = reader["CedulaJuridica"],
                        nombre = reader["Nombre"].ToString(),
                        descripcion = reader["Descripcion"],
                        telefono = reader["Telefono"],
                        provincia = reader["Provincia"],
                        canton = reader["Canton"],
                        distrito = reader["Distrito"],
                        senas = reader["Senas"],
                        correo = reader["Correo"],
                        dueno = reader["NombreDueno"]
                    });
                }

                return Ok(empresas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno", error = ex.Message });
            }
        }
    }
}
