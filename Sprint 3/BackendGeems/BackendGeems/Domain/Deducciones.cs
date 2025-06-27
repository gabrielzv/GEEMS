namespace BackendGeems.Domain
{
    public class Deduccion
    {
        public Guid Id { get; set; }
        public Guid IdPago { get; set; }
        public string TipoDeduccion { get; set; } = string.Empty;
        public Guid? IdBeneficio { get; set; }
        public double Monto { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}