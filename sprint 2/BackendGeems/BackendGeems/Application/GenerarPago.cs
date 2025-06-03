using BackendGeems.Domain;  


namespace BackendGeems.Application
{
    public class GenerarPago : IGenerarPago
    {
        private readonly IGEEMSRepo _repoInfrastructure;
        public GenerarPago(IGEEMSRepo repo)
        {
            _repoInfrastructure = repo;
        }
        public void GenerarPagoEmpleado(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal)
        {
            _repoInfrastructure.GenerarPagoEmpleado(idEmpleado, idPlanilla, fechaInicio, fechaFinal);
        }
        public void InsertDeduccion(Guid idPago, string tipo, Guid? idBeneficio, int monto)
        {
            _repoInfrastructure.InsertDeduccion(idPago, tipo, idBeneficio, monto);
        }

    }
}
