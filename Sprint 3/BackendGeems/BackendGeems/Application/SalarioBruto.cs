namespace BackendGeems.Application
{
    public class SalarioBruto : ISalarioBruto
    {
        private readonly IPagoRepo _repoInfrastructure;
        public SalarioBruto(IPagoRepo repo)
        {
            _repoInfrastructure = repo;
        }
        public double ObtenerSalarioBruto(Guid idEmpleado, DateTime fechaInicio, DateTime fechaFinal)
        {
            var salarioBruto = _repoInfrastructure.ObtenerSalarioBruto(idEmpleado, fechaInicio, fechaFinal);
            return salarioBruto;
        }
    }
}
