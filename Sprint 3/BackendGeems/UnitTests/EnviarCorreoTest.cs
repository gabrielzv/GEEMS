using BackendGeems.Application;
using BackendGeems.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BackendGeems.Domain;


// probar la funcionalidad de enviar un reporte sin el archivo correspondiente
namespace BackendGeems.Tests
{
    public class ReporteServiceTests
    {
        [Test]
        public async Task EnviarReporte_ArchivoNulo_DeberiaRetornarBadRequest()
        {
            // Arrange
            var mockService = new Mock<IReporteService>();
            var controller = new ReporteController(mockService.Object);

            var request = new EnviarReporteRequest
            {
                Archivo = null,
                Correo = "test@correo.com",
                NombreUsuario = "usuario_prueba"
            };

            // Act
            var resultado = await controller.EnviarReporte(request);

            // Assert
            var badRequest = resultado as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual("Archivo no válido.", badRequest.Value);

            mockService.Verify(s => s.EnviarReportePorCorreoAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ), Times.Never);
        }

    }
}
