using BackendGeems.Domain;


namespace BackendGeems.Application

{
    public interface ISalarioBruto
    {
        int ObtenerSalarioBruto(Guid idEmpleado, DateTime fechaInicio, DateTime fechaFinal);
    }
}
