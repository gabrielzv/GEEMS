using BackendGeems.Domain;

namespace BackendGeems.Application
{
    public class QueryPago : IQueryPago
    {
        private readonly IPagoRepo _repoInfrastructure;
        public QueryPago(IPagoRepo repo)
        {
            _repoInfrastructure = repo;
        }
        public List<Pago> ObtenerPagos(DateTime fechaInicio, DateTime fechaFinal)
        {
           var pagos = _repoInfrastructure.ObtenerPagos(fechaInicio, fechaFinal);
            return pagos;
        }
        public List<PagoyDeducciones> ObtenerPagosPorEmpleadoyPeriodo(string cedulaEmpleado, DateTime fechaInicio, DateTime fechaFinal)
        {
            var pagos = _repoInfrastructure.ObtenerPagosPorEmpleadoyPeriodo(cedulaEmpleado, fechaInicio, fechaFinal);
            return pagos;
        }
    }
    
}

