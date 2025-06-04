using System;

namespace MiProyecto.DTOs
{
    public class UpdateEstadoRegistroRequest
    {
        public Guid IdRegistro { get; set; }
        public int OpcionEstado { get; set; }
    }
}
