namespace BackendGeems.Domain
{
    public class Beneficio
    {
        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }
        public int Costo { get; set; }
        public int TiempoMinimo { get; set; }
        public required string Frecuencia { get; set; }
        public required string CedulaJuridica { get; set; }
        public List<string>? ContratosElegibles { get; set; }
    }
}