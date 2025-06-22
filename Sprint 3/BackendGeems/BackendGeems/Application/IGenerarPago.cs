using BackendGeems.Domain;

namespace BackendGeems.Application
{
    public interface IGenerarPago
    {
        void GenerarPagoEmpleado(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal);
        void InsertDeduccion(Guid idPago, string tipo, Guid? idBeneficio, int monto);
    }
}
