namespace BackendGeems.Domain
{
    public class ResultadoPago
    {
        public double SalarioBruto { get; set; }
        public double TotalDeducciones { get; set; }
        public double ImpuestoRenta { get; set; }
        public double SEM { get; set; }
        public double IVM { get; set; }
        public double BancoPopular { get; set; }
        public bool EsSegundaQuincena { get; set; } = false;
        public List<DeduccionVoluntaria> DeduccionesVoluntarias { get; set; } = new();
    }
   
    public class DeduccionVoluntaria
    {
        public Guid Id { get; set; }
        public double Monto { get; set; }
        public string Nombre { get; set; }
        public bool esAPI { get; set; }
        public string NombreDeAPI { get; set; } 
        public bool esPorcentual { get; set; } 
    }

}
