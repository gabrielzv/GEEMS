using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using BackendGeems.Application;
using BackendGeems.Domain;
using BackendGeems.Infraestructure;

namespace BackendGeems.Infraestructure
{
    public class CorreoSender
    {
        private readonly IConfiguration _config;

        public CorreoSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarCorreoConAdjuntoAsync(string destinatario, byte[] contenido, string nombreArchivo, string? nombreUsuario = null)
        {
            var remitente = _config["Correo:Remitente"];
            var contraseña = _config["Correo:Password"];

            string saludo = !string.IsNullOrWhiteSpace(nombreUsuario)
            ? $"Hola {nombreUsuario},\n\n": "";

            var mensaje = new MailMessage(remitente, destinatario)
            {
                Subject = "Reporte",
                Body = saludo + "Aqui esta el reporte solicitado.",
            };

            mensaje.Attachments.Add(new Attachment(new MemoryStream(contenido), nombreArchivo, "text/csv"));

            using var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(remitente, contraseña),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mensaje);
        }
    }
}
