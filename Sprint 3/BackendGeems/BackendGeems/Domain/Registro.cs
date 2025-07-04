namespace BackendGeems.Domain
{
    public class Registro
    {
        public Guid Id { get; set; }
        public int NumHoras { get; set; }
        public DateTime Fecha { get; set; } 
        public string Estado { get; set; }
        public Guid IdEmpleado { get; set; }
    }
}