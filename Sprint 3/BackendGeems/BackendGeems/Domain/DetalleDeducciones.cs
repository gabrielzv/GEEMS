namespace BackendGeems.Domain
{
    public class DetalleDeduccion
    {
        public string Nombre { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal Monto { get; set; }
    }

    public class ResultadoDeducciones
    {
        public decimal SalarioBruto { get; set; }
        public List<DetalleDeduccion> Deducciones { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal SalarioNeto { get; set; }
    }
}
