namespace BackendGeems.Domain
{
    public class Planilla
    {
        public Guid Id { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
        public Guid IdPayroll { get; set; }
        }
}