using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]
public class EmpresaController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public EmpresaController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("{cedula}")]
    public IActionResult GetEmpresa(int cedula)
    {
        Console.WriteLine($"Cédula recibida: {cedula}");

        try
        {
            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();

            // Obtener datos de la empresa que pertenece al dueño
            string queryEmpresa = @"
                SELECT 
                    e.CedulaJuridica,
                    e.Nombre,
                    e.Descripcion,
                    e.Telefono,
                    e.Provincia,
                    e.Canton,
                    e.Distrito,
                    e.Senas,
                    e.Correo
                FROM Persona p
                JOIN DuenoEmpresa d ON p.Cedula = d.CedulaPersona
                JOIN Empresa e ON d.CedulaEmpresa = e.CedulaJuridica
                WHERE p.Cedula = @Cedula;";

            using SqlCommand cmdEmpresa = new SqlCommand(queryEmpresa, conn);
            cmdEmpresa.Parameters.AddWithValue("@Cedula", cedula);

            using SqlDataReader readerEmpresa = cmdEmpresa.ExecuteReader();
            if (!readerEmpresa.Read())
                return NotFound(new { message = "Empresa no encontrada para el dueño indicado." });

            // Guardamos los datos de la empresa
            var empresa = new
            {
                cedulaJuridica = readerEmpresa["CedulaJuridica"],
                nombre = readerEmpresa["Nombre"].ToString(),
                descripcion = readerEmpresa["Descripcion"],
                telefono = readerEmpresa["Telefono"],
                provincia = readerEmpresa["Provincia"],
                canton = readerEmpresa["Canton"],
                distrito = readerEmpresa["Distrito"],
                senas = readerEmpresa["Senas"],
                correo = readerEmpresa["Correo"]
            };

            string nombreEmpresa = empresa.nombre;
            readerEmpresa.Close(); // Cerramos el lector antes de iniciar otro

            // Obtener empleados que pertenecen a la empresa
            string queryEmpleados = @"
                SELECT 
                    p.Cedula,
                    p.NombrePila + ' ' + p.Apellido1 + ' ' + p.Apellido2 AS NombreCompleto
                FROM Empleado em
                JOIN Persona p ON em.CedulaPersona = p.Cedula
                WHERE em.NombreEmpresa = @NombreEmpresa;";

            using SqlCommand cmdEmpleados = new SqlCommand(queryEmpleados, conn);
            cmdEmpleados.Parameters.AddWithValue("@NombreEmpresa", nombreEmpresa);

            using SqlDataReader readerEmpleados = cmdEmpleados.ExecuteReader();
            List<object> empleados = new();

            while (readerEmpleados.Read())
            {
                empleados.Add(new
                {
                    cedula = readerEmpleados["Cedula"],
                    nombre = readerEmpleados["NombreCompleto"]
                });
            }

            // Devolvemos todo junto
            return Ok(new
            {
                empresa,
                empleados
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno", error = ex.Message });
        }
    }


    [HttpGet("por-cedula-juridica/{cedulaJuridica}")]
    public IActionResult GetEmpresaPorCedulaJuridica(string cedulaJuridica)
    {
        Console.WriteLine($"Cédula jurídica recibida: {cedulaJuridica}");

        try
        {
            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();

            // Obtener datos directamente desde la tabla Empresa usando la cédula jurídica
            string queryEmpresa = @"
            SELECT 
                e.CedulaJuridica,
                e.Nombre,
                e.Descripcion,
                e.Telefono,
                e.Provincia,
                e.Canton,
                e.Distrito,
                e.Senas,
                e.Correo
            FROM Empresa e
            WHERE e.CedulaJuridica = @CedulaJuridica;";

            using SqlCommand cmdEmpresa = new SqlCommand(queryEmpresa, conn);
            cmdEmpresa.Parameters.AddWithValue("@CedulaJuridica", cedulaJuridica);

            using SqlDataReader readerEmpresa = cmdEmpresa.ExecuteReader();
            if (!readerEmpresa.Read())
                return NotFound(new { message = "Empresa no encontrada con la cédula jurídica indicada." });

            var empresa = new
            {
                cedulaJuridica = readerEmpresa["CedulaJuridica"],
                nombre = readerEmpresa["Nombre"].ToString(),
                descripcion = readerEmpresa["Descripcion"],
                telefono = readerEmpresa["Telefono"],
                provincia = readerEmpresa["Provincia"],
                canton = readerEmpresa["Canton"],
                distrito = readerEmpresa["Distrito"],
                senas = readerEmpresa["Senas"],
                correo = readerEmpresa["Correo"]
            };

            string nombreEmpresa = empresa.nombre;
            readerEmpresa.Close();

            // Obtener empleados de la empresa por nombre
            string queryEmpleados = @"
            SELECT 
                p.Cedula,
                p.NombrePila + ' ' + p.Apellido1 + ' ' + p.Apellido2 AS NombreCompleto
            FROM Empleado em
            JOIN Persona p ON em.CedulaPersona = p.Cedula
            WHERE em.NombreEmpresa = @NombreEmpresa;";

            using SqlCommand cmdEmpleados = new SqlCommand(queryEmpleados, conn);
            cmdEmpleados.Parameters.AddWithValue("@NombreEmpresa", nombreEmpresa);

            using SqlDataReader readerEmpleados = cmdEmpleados.ExecuteReader();
            List<object> empleados = new();

            while (readerEmpleados.Read())
            {
                empleados.Add(new
                {
                    cedula = readerEmpleados["Cedula"],
                    nombre = readerEmpleados["NombreCompleto"]
                });
            }

            return Ok(new
            {
                empresa,
                empleados
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno", error = ex.Message });
        }
    }

    [HttpGet("cedula-juridica/{nombreEmpresa}")]
    public IActionResult GetCedulaJuridicaPorNombreEmpresa(string nombreEmpresa)
    {
        Console.WriteLine($"Nombre de la empresa recibido: {nombreEmpresa}");

        try
        {
            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();

            // Se obtiene la cédula jurídica de la empresa por su nombre
            string query = @"
                SELECT CedulaJuridica
                FROM Empresa
                WHERE Nombre = @NombreEmpresa;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@NombreEmpresa", nombreEmpresa);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return NotFound(new { message = "No se encontró una empresa con el nombre proporcionado." });
            }

            var cedulaJuridica = reader["CedulaJuridica"].ToString();

            return Ok(new { cedulaJuridica });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error al obtener la cédula jurídica: {ex.Message}");
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

}
