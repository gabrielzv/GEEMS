using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

[Route("api/[controller]")]
[ApiController]
public class SetEmpresaController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public SetEmpresaController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public class EmpresaModel
    {
        public string CedulaJuridica { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }
        public string Senas { get; set; }
        public string ModalidadPago { get; set; }
    }

    [HttpPost("crearEmpresa")]
    public IActionResult CrearEmpresa([FromBody] EmpresaModel empresa)
    {
        if (empresa == null)
        {
            return BadRequest(new { message = "Datos de la empresa inválidos." });
        }

        try
        {
            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();

            string insertQuery = @"
                INSERT INTO Empresa (
                    CedulaJuridica,
                    Nombre,
                    Descripcion,
                    Telefono,
                    Correo,
                    Provincia,
                    Canton,
                    Distrito,
                    Senas,
                    ModalidadPago
                )
                VALUES (
                    @CedulaJuridica,
                    @Nombre,
                    @Descripcion,
                    @Telefono,
                    @Correo,
                    @Provincia,
                    @Canton,
                    @Distrito,
                    @Senas,
                    @ModalidadPago
                );";

            using SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@CedulaJuridica", empresa.CedulaJuridica);
            cmd.Parameters.AddWithValue("@Nombre", empresa.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", empresa.Descripcion);
            cmd.Parameters.AddWithValue("@Telefono", empresa.Telefono);
            cmd.Parameters.AddWithValue("@Correo", empresa.Correo);
            cmd.Parameters.AddWithValue("@Provincia", empresa.Provincia);
            cmd.Parameters.AddWithValue("@Canton", empresa.Canton);
            cmd.Parameters.AddWithValue("@Distrito", empresa.Distrito);
            cmd.Parameters.AddWithValue("@Senas", empresa.Senas);
            cmd.Parameters.AddWithValue("@ModalidadPago", empresa.ModalidadPago);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return Ok(new { message = "Empresa creada exitosamente", empresa.CedulaJuridica });
            }
            else
            {
                return StatusCode(500, new { message = "No se pudo insertar la empresa." });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al insertar empresa", error = ex.Message });
        }
    }
    // Metodo para editar una empresa ya registrada
    [HttpPost("editarEmpresa")]
    public IActionResult EditarEmpresa([FromBody] EmpresaModel empresa)
    {
        if (empresa == null || string.IsNullOrEmpty(empresa.CedulaJuridica))
        {
            return BadRequest(new { message = "Datos de la empresa inválidos." });
        }

        try
        {
            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();

            string updateQuery = @"
                UPDATE Empresa
                SET 
                    Nombre = @Nombre,
                    Descripcion = @Descripcion,
                    Telefono = @Telefono,
                    Correo = @Correo,
                    Provincia = @Provincia,
                    Canton = @Canton,
                    Distrito = @Distrito,
                    Senas = @Senas,
                    ModalidadPago = @ModalidadPago
                WHERE CedulaJuridica = @CedulaJuridica;";

            using SqlCommand cmd = new SqlCommand(updateQuery, conn);
            cmd.Parameters.AddWithValue("@CedulaJuridica", empresa.CedulaJuridica);
            cmd.Parameters.AddWithValue("@Nombre", empresa.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", empresa.Descripcion);
            cmd.Parameters.AddWithValue("@Telefono", empresa.Telefono);
            cmd.Parameters.AddWithValue("@Correo", empresa.Correo);
            cmd.Parameters.AddWithValue("@Provincia", empresa.Provincia);
            cmd.Parameters.AddWithValue("@Canton", empresa.Canton);
            cmd.Parameters.AddWithValue("@Distrito", empresa.Distrito);
            cmd.Parameters.AddWithValue("@Senas", empresa.Senas);
            cmd.Parameters.AddWithValue("@ModalidadPago", empresa.ModalidadPago);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return Ok(new { message = "Empresa actualizada exitosamente" });
            }
            else
            {
                return NotFound(new { message = "Empresa no encontrada o no se realizaron cambios." });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al actualizar la empresa", error = ex.Message });
        }
    }
}
