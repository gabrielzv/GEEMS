using System.ComponentModel.DataAnnotations;
namespace BackendGeems.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NombreUsuario { get; set; }

        [Required]
        public string Contrasena { get; set; }
    }
}
