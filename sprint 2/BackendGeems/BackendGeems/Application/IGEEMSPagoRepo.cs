using BackendGeems.Domain;
using static BackendGeems.Infraestructure.GEEMSPagoRepo;
namespace BackendGeems.Application
{
    public interface IGEEMSPagoRepo
    {
        
        List<Pago> ObtenerPagos(DateTime fechaInicio, DateTime fechaFinal);
        int ObtenerSalarioBruto(Guid idEmpleado, DateTime fechaInicio, DateTime fechaFinal);
        void GenerarPagoEmpleado(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal);
        void InsertDeduccion(Guid idPago, string tipo, Guid? idBeneficio, int monto);
        int CalcularImpuestoRenta(int salarioBruto);
        public string ObtenerTipoContratoEmpleado(Guid idEmpleado);
        public bool ExisteEmpleado(Guid idEmpleado);
        public int ObtenerSalarioEmpleado(Guid idEmpleado);
      
    }

}
