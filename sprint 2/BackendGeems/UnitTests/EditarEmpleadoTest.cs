using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using BackendGeems.Controllers;
using BackendGeems.Domain;

namespace UnitTests
{
    public class EmpleadosTest
    {
        private Mock<IConfiguration> _configMock;
        private EmpleadosController _controller;

        [SetUp]
        public void Setup()
        {
            _configMock = new Mock<IConfiguration>();
            _controller = new EmpleadosController(_configMock.Object);
        }

        // Prueba 1: Datos válidos
        [Test]
        public void EditarEmpleado_DatosValidos_RetornaOk()
        {
            var empleado = new EmpleadoUpdateDto
            {
                CedulaPersona = 303030303,
                Contrato = "Tiempo Completo",
                NumHorasTrabajadas = 40,
                Genero = "M",
                EstadoLaboral = "Activo",
                SalarioBruto = 1500000,
                Tipo = "Colaborador",
                FechaIngreso = "2023-01-15",
                NombreEmpresa = "GEEMS Solutions"
            };

            var result = _controller.EditarEmpleado(empleado);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        // Prueba 2: Cédula inválida o ausente
        [Test]
        public void EditarEmpleado_CedulaInvalida_RetornaBadRequest()
        {
            var empleado = new EmpleadoUpdateDto
            {
                CedulaPersona = 0, // Cédula inválida
                Contrato = "Tiempo Completo",
                NumHorasTrabajadas = 40,
                Genero = "M",
                EstadoLaboral = "Activo",
                SalarioBruto = 1500000,
                Tipo = "Colaborador",
                FechaIngreso = "2023-01-15",
                NombreEmpresa = "GEEMS Solutions"
            };

            var result = _controller.EditarEmpleado(empleado);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        // Prueba 3: Simulación de error en la base de datos
        [Test]
        public void EditarEmpleado_ErrorBaseDatos_RetornaStatus500()
        {
            var configMockError = new Mock<IConfiguration>();
            var controllerError = new EmpleadosController(configMockError.Object);

            var empleado = new EmpleadoUpdateDto
            {
                CedulaPersona = 303030303,
                Contrato = "Tiempo Completo",
                NumHorasTrabajadas = 40,
                Genero = "M",
                EstadoLaboral = "Activo",
                SalarioBruto = 1500000,
                Tipo = "Colaborador",
                FechaIngreso = "2023-01-15",
                NombreEmpresa = "GEEMS Solutions"
            };

            // Simular un error en la base de datos creando un mock que falle
            configMockError.Setup(c => c.GetConnectionString(It.IsAny<string>())).Throws(new System.Exception("Error en la base de datos"));

            var result = controllerError.EditarEmpleado(empleado);

            Assert.IsInstanceOf<ObjectResult>(result);

            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }
    }
}
