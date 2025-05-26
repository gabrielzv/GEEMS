using BackendGeems.Application;
using BackendGeems.Domain;

namespace BackendGeems.Infraestructure
{
    public class QueryPago : IQueryPago
    {
        private readonly IGEEMSRepo _repoInfrastructure;
        public QueryPago(IGEEMSRepo repo)
        {
            _repoInfrastructure = repo;
        }
        public List<Pago> ObtenerPagos(DateTime fechaInicio, DateTime fechaFinal)
        {
           var pagos = _repoInfrastructure.ObtenerPagos(fechaInicio, fechaFinal);
            return pagos;
        }
    }
    
}

