using BackendGeems.Domain;
using static BackendGeems.Infraestructure.PagoRepo;
namespace BackendGeems.Application
{
    public interface IPagoRepo
    {
        
        List<Pago> ObtenerPagos(DateTime fechaInicio, DateTime fechaFinal);
        List<Pago> ObtenerPagosPorEmpleado(Guid idEmpleado);
        double ObtenerSalarioBruto(Guid idEmpleado, DateTime fechaInicio, DateTime fechaFinal);
        
        void InsertDeduccion(Guid idPago, string tipo, Guid? idBeneficio, double monto,string nombreBeneficio);
        List<Deduccion> ObtenerDeduccionesPorPago(Guid idPago);
        double CalcularImpuestoRenta(double salarioBruto);
        public string ObtenerTipoContratoEmpleado(Guid idEmpleado);
        public void BorrarPagoExistente(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal);
        public bool ExisteEmpleado(Guid idEmpleado);
        public double ObtenerSalarioEmpleado(Guid idEmpleado);
        public double ObtenerMontoAPI(Guid idEmpleado, string nombreAPI, double salarioBruto);
        public List<DeduccionVoluntaria> ObtenerDeduccionesVoluntarias(Guid idEmpleado);
        public void InsertPago(Guid idPago, Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal, double montoBruto, double montoPago);
        public int ContarPagos(Guid idEmpleado);
        public void InactivarBeneficiosPendientesPorEmpresa(string nombreEmpresa);
        public List<PagoyDeducciones> ObtenerPagosPorEmpleadoyPeriodo(string cedulaEmpleado, DateTime fechaInicio, DateTime fechaFinal);
    }

}
