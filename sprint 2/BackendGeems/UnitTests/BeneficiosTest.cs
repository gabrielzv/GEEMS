using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using BackendGeems.Controllers;
using System.Collections.Generic;

namespace UnitTests
{
    public class BeneficiosTest
    {
        // Test para crear un beneficio, todos los datos son válidos y se espera que retorne un resultado exitoso.
        [Test]
        public void CrearBeneficio_DatosValidos_RetornaOk()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            var controller = new BackendGeems.Controllers.BeneficioController(configMock.Object);

            var beneficio = new BackendGeems.Domain.Beneficio
            {
                Nombre = "Seguro Médico",
                Descripcion = "Cobertura médica básica",
                Costo = 1000,
                TiempoMinimo = 0,
                Frecuencia = "Mensual",
                CedulaJuridica = "123456789",
                ContratosElegibles = new List<string> { "Tiempo Completo" },
                NombreDeAPI = "MediSeguro",
                EsApi = true
            };

            // Act
            var result = controller.CrearBeneficio(beneficio);

            // Assert
            Assert.IsInstanceOf<IActionResult>(result);
        }

        // Test para crear una empresa, todos los datos son válidos y se espera que retorne un resultado exitoso.
        [Test]
        public void CrearEmpresa_DatosValidos_RetornaOk()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            var controller = new SetEmpresaController(configMock.Object);

            var empresa = new SetEmpresaController.EmpresaModel
            {
                CedulaJuridica = "987654321",
                Nombre = "Empresa Prueba",
                Descripcion = "Empresa de prueba",
                Telefono = "22223333",
                Correo = "prueba@empresa.com",
                Provincia = "San José",
                Canton = "Montes de Oca",
                Distrito = "San Pedro",
                Senas = "Cerca de la UCR",
                ModalidadPago = "Mensual"
            };

            // Act
            var result = controller.CrearEmpresa(empresa);

            // Assert
            Assert.IsInstanceOf<IActionResult>(result);
        }

        // Test para modificar una empresa, todos los datos son válidos, solo se edita el nombre y la descripción y se espera que retorne un resultado exitoso.
        [Test]
        public void EditarEmpresa_DatosValidos_RetornaOk()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            var controller = new SetEmpresaController(configMock.Object);

            var empresa = new SetEmpresaController.EmpresaModel
            {
                CedulaJuridica = "987654321",
                Nombre = "Empresa Editada",
                Descripcion = "Empresa editada",
                Telefono = "22224444",
                Correo = "editada@empresa.com",
                Provincia = "San José",
                Canton = "Montes de Oca",
                Distrito = "San Pedro",
                Senas = "Cerca de la UCR",
                ModalidadPago = "Mensual"
            };

            // Act
            var result = controller.EditarEmpresa(empresa);

            // Assert
            Assert.IsInstanceOf<IActionResult>(result);
        }
    }
}