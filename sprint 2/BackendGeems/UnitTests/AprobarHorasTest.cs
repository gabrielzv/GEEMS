using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Threading.Tasks;
using MiProyecto.Controllers;
using MiProyecto.DTOs;

namespace UnitTests
{
    public class RegistroControllerTests
    {
        private Mock<IConfiguration> _configMock;
        private RegistroController _controller;

        [SetUp]
        public void Setup()
        {
            _configMock = new Mock<IConfiguration>();
            _controller = new RegistroController(_configMock.Object);
        }

        // Test 1: Request nulo - Se espera BadRequest
        [Test]
        public async Task ActualizarEstadoRegistro_RequestNulo_RetornaBadRequest()
        {
            // Act
            var result = await _controller.ActualizarEstadoRegistro(null) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("La petición no puede ser nula.", result.Value);
        }
    }
}
