using BackendGeems.Domain;  


namespace BackendGeems.Application
{
    public class GenerarPago : IGenerarPago
    {
        private readonly IGEEMSPagoRepo _repoInfrastructure;
        public GenerarPago(IGEEMSPagoRepo repo)
        {
            _repoInfrastructure = repo;
        }
        public void GenerarPagoEmpleado(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal)
        {
            _repoInfrastructure.GenerarPagoEmpleado(idEmpleado, idPlanilla, fechaInicio, fechaFinal);
        }
        public void InsertDeduccion(Guid idPago, string tipo, Guid? idBeneficio, double monto, string nombreBeneficio)
        {
            _repoInfrastructure.InsertDeduccion(idPago, tipo, idBeneficio, monto,nombreBeneficio);
        }

    }
}
