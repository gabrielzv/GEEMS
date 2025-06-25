using BackendGeems.Domain;  


namespace BackendGeems.Application
{
    public class GenerarPago : IGenerarPago
    {
        private readonly IPagoRepo _repoInfrastructure;
        private readonly GestorPagosService _gestorPagosService;
        public GenerarPago(IPagoRepo repo, GestorPagosService gestorPagosService)
        {
            _repoInfrastructure = repo;
            _gestorPagosService = gestorPagosService;
        }
        public void GenerarPagoEmpleado(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal)
        {
            _gestorPagosService.GenerarPagoEmpleado(idEmpleado, idPlanilla, fechaInicio, fechaFinal);
        }
        public void InsertDeduccion(Guid idPago, string tipo, Guid? idBeneficio, double monto, string nombreBeneficio)
        {
            _repoInfrastructure.InsertDeduccion(idPago, tipo, idBeneficio, monto,nombreBeneficio);
        }

    }
}
