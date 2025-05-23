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

        public PagosController(IQueryPago queryPago)
        {
            _queryPago = queryPago;
            _repoInfrastructure = new GEEMSRepo();

        }
        [HttpGet]
        public List<Pago> Get(DateTime fechaInicio, DateTime fechaFin)
        {
            var pagos = _queryPago.ObtenerPagos(fechaInicio,fechaFin);
            return pagos;
        }

      
    }
}
