using System.Reflection.Metadata;

namespace BackendGeems.Domain
{
    public class Pago
    {
        public Guid Id { get; set; }
        public DateTime FechaRealizada { get; set; }  
        public decimal MontoPago { get; set; }
        public Guid IdEmpleado { get; set; }
        public Guid IdPayroll { get; set; }
        public Guid IdPlanilla { get; set; }
        public decimal MontoBruto { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
    }
}
 