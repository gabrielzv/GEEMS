using BackendGeems.Domain;
using static BackendGeems.Infraestructure.PagoRepo;
namespace BackendGeems.Application
{
    public interface IPagoRepo
    {
        
        List<Pago> ObtenerPagos(DateTime fechaInicio, DateTime fechaFinal);
        double ObtenerSalarioBruto(Guid idEmpleado, DateTime fechaInicio, DateTime fechaFinal);
        void GenerarPagoEmpleado(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal);
        void InsertDeduccion(Guid idPago, string tipo, Guid? idBeneficio, double monto,string nombreBeneficio);
        double CalcularImpuestoRenta(double salarioBruto);
        public string ObtenerTipoContratoEmpleado(Guid idEmpleado);
        public bool ExisteEmpleado(Guid idEmpleado);
        public double ObtenerSalarioEmpleado(Guid idEmpleado);
      
    }

}
