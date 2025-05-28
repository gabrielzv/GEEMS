using BackendGeems.Domain;
namespace BackendGeems.Application
{
    public interface IGEEMSRepo
    {
        bool calcularPago(string fechaInicio, string fechaFinal);
        List<Pago> ObtenerPagos(DateTime fechaInicio, DateTime fechaFinal);
        int ObtenerSalarioBruto(Guid idEmpleado, DateTime fechaInicio, DateTime fechaFinal);
        void GenerarPagoEmpleado(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal);
        void InsertDeduccion(Guid idPago, string tipo, Guid? idBeneficio, int monto);
    }
}
