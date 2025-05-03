// Controllers/PersonaController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


[Route("api/[controller]")]
[ApiController]
public class PersonaController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public PersonaController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("{cedula}")]
    public IActionResult GetPersona(int cedula)
    {
        Console.WriteLine($"cedula recibido: {cedula}");
        
        try
        {
            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();

            string query = "SELECT * FROM Persona WHERE Cedula = @Cedula";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Cedula", cedula);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return Ok(new
                {
                    fullName = $"{reader["NombrePila"]} {reader["Apellido1"]} {reader["Apellido2"]}",
                    email = reader["Correo"],
                    phone = reader["Telefono"],
                    address = reader["Direccion"]
                });
            }

            return NotFound(new { message = "Persona no encontrada" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno", error = ex.Message });
        }
    }
}
