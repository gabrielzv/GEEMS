using BackendGeems.Domain;


namespace BackendGeems.Application

{
    public interface ISalarioBruto
    {
        double ObtenerSalarioBruto(Guid idEmpleado, DateTime fechaInicio, DateTime fechaFinal);
    }
}
