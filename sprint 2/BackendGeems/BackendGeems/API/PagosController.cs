using BackendGeems.Application;
using BackendGeems.Domain;
using BackendGeems.Infraestructure;
using Microsoft.AspNetCore.Mvc;

namespace BackendGeems.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        private readonly GEEMSRepo _repoInfrastructure;
        private readonly IQueryPago _queryPago;
        private readonly IGenerarPago _GenerarPago;

        public PagosController(IQueryPago queryPago, IGenerarPago generarPago)
        {
            _queryPago = queryPago;

            _repoInfrastructure = new GEEMSRepo();
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
                var empleados = _repoInfrastructure.ObtenerEmpleadosPorEmpresa(nombreEmpresa);
                foreach (var empleado in empleados)
                {
                    _repoInfrastructure.GenerarPagoEmpleado(empleado.Id, idPlanilla, fechaInicio, fechaFinal);
                }
                return Ok(new { message = "Pagos generados para todos los empleados." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al generar pagos: " + ex.Message });
            }
        }
        
        [HttpGet("resumenPlanilla")]
        public IActionResult ResumenPlanilla([FromQuery] string nombreEmpresa, [FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var empleados = _repoInfrastructure.ObtenerEmpleadosPorEmpresa(nombreEmpresa);
                if (empleados == null || empleados.Count == 0)
                {
                    return Ok(new { totalBruto = 0, totalNeto = 0, totalDeducciones = 0 });
                }

                var pagos = _repoInfrastructure.ObtenerPagos(fechaInicio, fechaFin);

                decimal totalBruto = 0, totalNeto = 0, totalDeducciones = 0;

                foreach (var empleado in empleados)
                {
                    var pagosEmpleado = pagos.Where(p => p.IdEmpleado == empleado.Id);
                    foreach (var pago in pagosEmpleado)
                    {
                        totalBruto += pago.MontoBruto;
                        totalNeto += pago.MontoPago;
                        totalDeducciones += (pago.MontoBruto - pago.MontoPago);
                    }
                }

                return Ok(new
                {
                    totalBruto,
                    totalNeto,
                    totalDeducciones
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el resumen de planilla: " + ex.Message });
            }
        }
    }
}
