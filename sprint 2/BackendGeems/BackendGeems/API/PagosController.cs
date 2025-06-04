using BackendGeems.Application;
using BackendGeems.Controllers;
using BackendGeems.Domain;
using BackendGeems.Infraestructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BackendGeems.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        private readonly GEEMSRepo _repoInfrastructure;
        private readonly GEEMSPagoRepo _pagoInfrastructure;
        private readonly IQueryPago _queryPago;
        private readonly IGenerarPago _GenerarPago;

        public PagosController(IQueryPago queryPago, IGenerarPago generarPago)
        {
            _queryPago = queryPago;

            _repoInfrastructure = new GEEMSRepo();
            _pagoInfrastructure = new GEEMSPagoRepo();
            _GenerarPago = generarPago;
        }
        [HttpGet]
        public List<Pago> Get(DateTime fechaInicio, DateTime fechaFin)
        {
            var pagos = _queryPago.ObtenerPagos(fechaInicio, fechaFin);
            return pagos;
        }
        [HttpPost]
        public void Post(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFin)
        {
            _GenerarPago.GenerarPagoEmpleado(idEmpleado, idPlanilla, fechaInicio, fechaFin);

        }

        [HttpPost("generarPagosEmpresa")]
        public IActionResult GenerarPagosEmpresa([FromQuery] string nombreEmpresa, [FromQuery] Guid idPlanilla, [FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFinal)
        {
            try
            {
                var nombreEmpleado = "";
                var empleados = _repoInfrastructure.ObtenerEmpleadosPorEmpresa(nombreEmpresa);
                foreach (var empleado in empleados)
                {
                    try
                    {
                         nombreEmpleado = _pagoInfrastructure.GetNombreEmpleadoPorCedula(empleado.CedulaPersona.ToString());
                        _pagoInfrastructure.GenerarPagoEmpleado(empleado.Id, idPlanilla, fechaInicio, fechaFinal);
                        Console.WriteLine("generando Para:" + nombreEmpleado);
                    }
                    catch (Exception ex)
                    {
                        // Manejar el error para cada empleado individualmente
                        return StatusCode(500, new { message = $"\nError al generar pago para el empleado {nombreEmpleado}: {ex.Message}\n" });
                    }
                }
                return Ok(new { message = "Pagos generados para todos los empleados." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("resumenPlanilla")]
        public IActionResult ResumenPlanilla([FromQuery] string nombreEmpresa, [FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                using (var connection = new SqlConnection(_repoInfrastructure.CadenaConexion))
                {
                    var cmd = new SqlCommand("SELECT * FROM dbo.fnResumenPlanilla(@nombreEmpresa, @fechaInicio, @fechaFin)", connection);
                    cmd.Parameters.AddWithValue("@nombreEmpresa", nombreEmpresa);
                    cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@fechaFin", fechaFin);

                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Ok(new
                            {
                                totalBruto = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0),
                                totalNeto = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1),
                                totalDeducciones = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2)
                            });
                        }
                        else
                        {
                            return Ok(new { totalBruto = 0, totalNeto = 0, totalDeducciones = 0 });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el resumen de planilla: " + ex.ToString() });
            }
        }
    }
}
