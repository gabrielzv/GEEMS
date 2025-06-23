namespace BackendGeems.Domain
{
    public class Beneficio
    {
        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }
        public decimal Costo { get; set; }
        public int TiempoMinimo { get; set; }
        public required string Frecuencia { get; set; }
        public required string CedulaJuridica { get; set; }
        public List<string>? ContratosElegibles { get; set; }
        public required string NombreDeAPI { get; set; }
        public bool EsApi { get; set; } = true;
        public string? Id { get; set; }
        public bool EsPorcentual { get; set; } = false;
    }
}