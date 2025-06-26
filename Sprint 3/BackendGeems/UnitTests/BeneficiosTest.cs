using NUnit.Framework;
using Moq;
using BackendGeems.Controllers;
using BackendGeems.Application;
using BackendGeems.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace UnitTests
{
    public class BeneficiosTest
    {
        private Mock<IQueryBeneficio> _mockQueryBeneficio;
        private BeneficioController _controller;

        [SetUp]
        public void Setup()
        {
            _mockQueryBeneficio = new Mock<IQueryBeneficio>();
            _controller = new BeneficioController(_mockQueryBeneficio.Object);
        }

        [Test]
        public void CrearBeneficio_DatosValidos_RetornaOk()
        {
            // Arrange
            var beneficio = new Beneficio
            {
                Nombre = "Seguro Médico",
                Descripcion = "Cobertura médica básica",
                Costo = 1000,
                TiempoMinimo = 0,
                Frecuencia = "Mensual",
                CedulaJuridica = "123456789",
                ContratosElegibles = new List<string> { "Tiempo Completo" },
                NombreDeAPI = "MediSeguro",
                EsApi = true,
                EsPorcentual = false
            };

            // Act
            var result = _controller.CrearBeneficio(beneficio);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void EditarBeneficio_DatosValidos_RetornaOk()
        {
            // Arrange
            var beneficio = new Beneficio
            {
                Nombre = "Seguro Médico",
                Descripcion = "Cobertura médica básica",
                Costo = 1000,
                TiempoMinimo = 0,
                Frecuencia = "Mensual",
                CedulaJuridica = "123456789",
                ContratosElegibles = new List<string> { "Tiempo Completo" },
                NombreDeAPI = "MediSeguro",
                EsApi = true,
                EsPorcentual = false
            };

            // Act
            var result = _controller.EditarBeneficio(beneficio);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void GetBeneficio_Existente_RetornaOk()
        {
            // Arrange
            var beneficio = new Beneficio {
                Nombre = "Seguro Médico",
                Descripcion = "Cobertura médica básica",
                Costo = 1000,
                TiempoMinimo = 0,
                Frecuencia = "Mensual",
                CedulaJuridica = "123456789",
                ContratosElegibles = new List<string> { "Tiempo Completo" },
                NombreDeAPI = "MediSeguro",
                EsApi = true,
                EsPorcentual = false
            };
            _mockQueryBeneficio.Setup(q => q.GetBeneficio(It.IsAny<System.Guid>())).Returns(beneficio);

            // Act
            var result = _controller.GetBeneficio(System.Guid.NewGuid());

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void GetBeneficio_NoExistente_RetornaNotFound()
        {
            // Arrange
            _mockQueryBeneficio.Setup(q => q.GetBeneficio(It.IsAny<System.Guid>())).Returns((Beneficio)null);

            // Act
            var result = _controller.GetBeneficio(System.Guid.NewGuid());

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public void GetBeneficio_EsPorcentual_True_RetornaOkConPorcentual()
        {
            // Arrange
            var beneficio = new Beneficio
            {
                Nombre = "Deducción porcentual prueba",
                Descripcion = "Deducción porcentual descripción prueba",
                Costo = 3,
                TiempoMinimo = 0,
                Frecuencia = "Mensual",
                CedulaJuridica = "123456789",
                ContratosElegibles = new List<string> { "Tiempo Completo", "Medio Tiempo", "Servicios Profesionales", "Por Horas" },
                NombreDeAPI = "BeneficioNormal",
                EsApi = false,
                EsPorcentual = true
            };
            _mockQueryBeneficio.Setup(q => q.GetBeneficio(It.IsAny<System.Guid>())).Returns(beneficio);

            // Act
            var result = _controller.GetBeneficio(System.Guid.NewGuid()) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var beneficioResult = result.Value as Beneficio;
            Assert.IsNotNull(beneficioResult);
            Assert.IsTrue(beneficioResult.EsPorcentual);
        }

        [Test]
        public void GetBeneficio_EsPorcentual_False_RetornaOkConRegular()
        {
            // Arrange
            var beneficio = new Beneficio
            {
                Nombre = "Deducción regular prueba",
                Descripcion = "Deducción regular descripción prueba",
                Costo = 10000,
                TiempoMinimo = 0,
                Frecuencia = "Mensual",
                CedulaJuridica = "123456789",
                ContratosElegibles = new List<string> { "Tiempo Completo", "Medio Tiempo", "Servicios Profesionales", "Por Horas" },
                NombreDeAPI = "BeneficioNormal",
                EsApi = false,
                EsPorcentual = false
            };
            _mockQueryBeneficio.Setup(q => q.GetBeneficio(It.IsAny<System.Guid>())).Returns(beneficio);

            // Act
            var result = _controller.GetBeneficio(System.Guid.NewGuid()) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var beneficioResult = result.Value as Beneficio;
            Assert.IsNotNull(beneficioResult);
            Assert.IsFalse(beneficioResult.EsPorcentual);
        }
    }
}