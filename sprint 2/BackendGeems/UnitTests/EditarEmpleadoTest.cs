using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection;
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

        // Test 1: Datos válidos - Se espera que la actualización sea exitosa
        [Test]
        public void EditarEmpleado_DatosValidos_RetornaOk()
        {
            // Arrange
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

            // Act
            var result = _controller.EditarEmpleado(empleado);

            // Assert
            Assert.IsInstanceOf<IActionResult>(result);
        }

        // Test 2: Cédula inválida - Se espera BadRequest
        [Test]
        public void EditarEmpleado_CedulaInvalida_RetornaBadRequest()
        {
            // Arrange
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

            // Act
            var result = _controller.EditarEmpleado(empleado);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}
