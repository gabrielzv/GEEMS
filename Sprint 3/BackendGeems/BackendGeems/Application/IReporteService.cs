namespace BackendGeems.Application
{
    public interface IReporteService
    {
        Task EnviarReportePorCorreoAsync(string correo, byte[] archivo, string nombreArchivo, string nombreUsuario);
    }
}
