namespace BackendGeems.Domain
{
    public class Empleado
    {
        public Guid Id { get; set; }
        public int CedulaPersona { get; set; }
        public string Contrato { get; set; }
        public int NumHorasTrabajadas { get; set; }
        public string Genero { get; set; }
        public string EstadoLaboral { get; set; }
        public int SalarioBruto { get; set; }
        public string Tipo { get; set; }
        public string FechaIngreso { get; set; }
        public string NombreEmpresa { get; set; }
    }

}
