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
        private readonly GeneralRepo _repoInfrastructure;
        private readonly PagoRepo _pagoInfrastructure;
        private readonly GestorPagosService _gestorPagosService;
        private readonly IQueryPago _queryPago;
        private readonly IGenerarPago _GenerarPago;

        public PagosController(IQueryPago queryPago, IGenerarPago generarPago, GestorPagosService gestorPagosService)
        {
            _queryPago = queryPago;

            _repoInfrastructure = new GeneralRepo();
            _pagoInfrastructure = new PagoRepo();
            _GenerarPago = generarPago;
            _gestorPagosService = gestorPagosService;
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
                var empleados = _repoInfrastructure.ObtenerEmpleadosPorEmpresa(nombreEmpresa);
               
                int pagosGenerados = 0;
                foreach (var empleado in empleados)
                {
                    try
                    {
                        // Obtener salario bruto antes de intentar generar el pago
                        double salarioBruto = _pagoInfrastructure.ObtenerSalarioBruto(empleado.Id, fechaInicio, fechaFinal);
                        if (salarioBruto <= 0)
                        {
                            // Saltar empleados sin horas o salario
                            continue;
                        }
                        _gestorPagosService.GenerarPagoEmpleado(empleado.Id, idPlanilla, fechaInicio, fechaFinal);
                        pagosGenerados++;
                    }
                    catch (Exception ex)
                    {
                        // Opcional: loguear el error, pero no detener el proceso
                        
                        continue;
                    }
                }
                if (pagosGenerados == 0)
                {
                    return BadRequest(new { message = "No hay empleados con horas registradas para esta planilla." });
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
