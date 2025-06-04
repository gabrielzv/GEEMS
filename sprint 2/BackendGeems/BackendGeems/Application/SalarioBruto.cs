namespace BackendGeems.Application
{
    public class SalarioBruto : ISalarioBruto
    {
        private readonly IGEEMSPagoRepo _repoInfrastructure;
        public SalarioBruto(IGEEMSPagoRepo repo)
        {
            _repoInfrastructure = repo;
        }
        public int ObtenerSalarioBruto(Guid idEmpleado, DateTime fechaInicio, DateTime fechaFinal)
        {
            var salarioBruto = _repoInfrastructure.ObtenerSalarioBruto(idEmpleado, fechaInicio, fechaFinal);
            return salarioBruto;
        }
    }
}
