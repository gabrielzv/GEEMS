namespace BackendGeems.Application
{
    public class EnviarReporteRequest
    {
        public IFormFile Archivo { get; set; }
        public string Correo { get; set; }
         public string? NombreUsuario { get; set; }
    }
}
