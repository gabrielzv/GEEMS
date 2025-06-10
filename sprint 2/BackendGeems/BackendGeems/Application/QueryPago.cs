using BackendGeems.Domain;

namespace BackendGeems.Application
{
    public class QueryPago : IQueryPago
    {
        private readonly IGEEMSPagoRepo _repoInfrastructure;
        public QueryPago(IGEEMSPagoRepo repo)
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

