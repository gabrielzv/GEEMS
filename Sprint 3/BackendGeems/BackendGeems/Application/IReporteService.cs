namespace BackendGeems.Application
{
    public interface IReporteService
    {
        Task EnviarReportePorCorreoAsync(string correo, byte[] archivo, string nombreArchivo, string nombreUsuario);
        Task EnviarCorreoAsync(string destinatario, string asunto, string mensaje);
    }
}
