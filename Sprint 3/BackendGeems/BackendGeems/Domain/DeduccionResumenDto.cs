namespace BackendGeems.Domain
{
    public class DeduccionResumenDto
    {
        public string Nombre { get; set; }
        public double Total { get; set; }
        public bool EsBeneficio { get; set; } // true si es beneficio, false si es deducción obligatoria
    }
}