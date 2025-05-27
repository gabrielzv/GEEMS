namespace BackendGeems.Application
{
    public class SalarioBruto : ISalarioBruto
    {
        private readonly IGEEMSRepo _repoInfrastructure;
        public SalarioBruto(IGEEMSRepo repo)
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
