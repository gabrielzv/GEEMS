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
        public void  Post(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFin)
        {       
            _GenerarPago.GenerarPagoEmpleado(idEmpleado, idPlanilla, fechaInicio, fechaFin);

        }

      
    }
}
