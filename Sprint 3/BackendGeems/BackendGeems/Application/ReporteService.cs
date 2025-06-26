using BackendGeems.Application;
using BackendGeems.Domain;
using BackendGeems.Infraestructure;


namespace BackendGeems.Application
{
    public class ReporteService
    {
        private readonly CorreoSender _correoSender;

        public ReporteService(CorreoSender correoSender)
        {
            _correoSender = correoSender;
        }

        public async Task EnviarReportePorCorreoAsync(string correoDestino, byte[] archivo, string nombreArchivo, string? nombreUsuario)
        {
            await _correoSender.EnviarCorreoConAdjuntoAsync(
                correoDestino,
                archivo,
                nombreArchivo,
                nombreUsuario
            );

        }
    }
}
