namespace BackendGeems.Models
{
    public class Usuario
    {
        public Guid Id { get; set; } 
        public string Username { get; set; }
        public string Contrasena { get; set; }
        public string Tipo { get; set; }
        public int CedulaPersona { get; set; }
        public string CorreoPersona { get; set; }
    }
}
