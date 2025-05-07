// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Data.SqlClient;
// using System;

// namespace BackendGeems.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class SetBeneficioPorEmpleadoController : ControllerBase
//     {
//         private readonly IConfiguration _configuration;

//         public SetBeneficioPorEmpleadoController(IConfiguration configuration)
//         {
//             _configuration = configuration;
//         }

//         [HttpPost("matricularBeneficio")]
//         public IActionResult MatricularBeneficio([FromBody] dynamic data)
//         {
//             string beneficioId = "E4915603-8DF1-4014-B400-08190B37D91B";
//             string empleadoId = "26257399-F0F1-4E8A-9939-2C79D89C0E28";

//             if (string.IsNullOrWhiteSpace(beneficioId) || string.IsNullOrWhiteSpace(empleadoId))
//             {
//                 return BadRequest("El ID del beneficio y el ID del empleado son obligatorios.");
//             }

//             try
//             {
//                 using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//                 {
//                     connection.Open();
//                     var query = "INSERT INTO BeneficiosEmpleado (IdEmpleado, IdBeneficio) VALUES (@EmpleadoId, @BeneficioId)";
//                     using (var command = new SqlCommand(query, connection))
//                     {
//                         command.Parameters.AddWithValue("@EmpleadoId", empleadoId);
//                         command.Parameters.AddWithValue("@BeneficioId", beneficioId);

//                         command.ExecuteNonQuery();
//                     }
//                 }

//                 return Ok("Beneficio matriculado exitosamente.");
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, $"Error al matricular el beneficio: {ex.Message}");
//             }
//         }
//     }
// }