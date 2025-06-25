using NUnit.Framework;
using Moq;
using BackendGeems.Application;
using BackendGeems.Domain;
using System;
using System.Collections.Generic;

namespace BackendGeems.Tests
{
    public class ServicioCalculoPagoTests
    {
        private Mock<IPagoRepo> _mockPagoRepo;
        private ServicioCalculoPago _servicio;
        private Guid _empleadoId;
        private DateTime _fechaInicio;
        private DateTime _fechaFinal;

        [SetUp]
        public void Setup()
        {
            _mockPagoRepo = new Mock<IPagoRepo>();
            _servicio = new ServicioCalculoPago(_mockPagoRepo.Object);
            _empleadoId = Guid.NewGuid();
            _fechaInicio = new DateTime(2024, 6, 1);
            _fechaFinal = new DateTime(2024, 6, 30);
        }

        [Test]
        public void CalcularPagoMensual_TiempoCompleto_SinDeduccionesVoluntarias()
        {
            _mockPagoRepo.Setup(r => r.ObtenerTipoContratoEmpleado(_empleadoId)).Returns("Tiempo Completo");
            _mockPagoRepo.Setup(r => r.ObtenerSalarioBruto(_empleadoId, _fechaInicio, _fechaFinal)).Returns(2000);
            _mockPagoRepo.Setup(r => r.ObtenerSalarioEmpleado(_empleadoId)).Returns(2000);
            _mockPagoRepo.Setup(r => r.CalcularImpuestoRenta(2000)).Returns(100);
            _mockPagoRepo.Setup(r => r.ObtenerDeduccionesVoluntarias(_empleadoId)).Returns(new List<DeduccionVoluntaria>());

            var resultado = _servicio.CalcularPagoMensual(_empleadoId, _fechaInicio, _fechaFinal);

            Assert.AreEqual(2000, resultado.SalarioBruto);
            Assert.AreEqual(100, resultado.ImpuestoRenta);
            Assert.AreEqual(2000 * 0.055, resultado.SEM, 0.01);
            Assert.AreEqual(2000 * 0.0417, resultado.IVM, 0.01);
            Assert.AreEqual(2000 * 0.01, resultado.BancoPopular, 0.01);
            Assert.IsEmpty(resultado.DeduccionesVoluntarias);
        }

        [Test]
        public void CalcularPagoMensual_PorHoras_SinHorasAceptadas_LanzaExcepcion()
        {
            _mockPagoRepo.Setup(r => r.ObtenerTipoContratoEmpleado(_empleadoId)).Returns("Por Horas");
            _mockPagoRepo.Setup(r => r.ObtenerSalarioBruto(_empleadoId, _fechaInicio, _fechaFinal)).Returns(-2);

            var ex = Assert.Throws<Exception>(() =>
                _servicio.CalcularPagoMensual(_empleadoId, _fechaInicio, _fechaFinal));

            Assert.AreEqual("No hay horas aceptadas.", ex.Message);
        }

        [Test]
        public void CalcularPagoQuincenal_TiempoCompleto_DeduccionesDivididas()
        {
            _mockPagoRepo.Setup(r => r.ObtenerTipoContratoEmpleado(_empleadoId)).Returns("Tiempo Completo");
            _mockPagoRepo.Setup(r => r.ObtenerSalarioBruto(_empleadoId, _fechaInicio, _fechaFinal)).Returns(2000);
            _mockPagoRepo.Setup(r => r.ObtenerSalarioEmpleado(_empleadoId)).Returns(2000);
            _mockPagoRepo.Setup(r => r.CalcularImpuestoRenta(2000)).Returns(200);
            _mockPagoRepo.Setup(r => r.ObtenerDeduccionesVoluntarias(_empleadoId)).Returns(new List<DeduccionVoluntaria>());

            var resultado = _servicio.CalcularPagoQuincenal(_empleadoId, _fechaInicio, _fechaFinal);

            Assert.AreEqual(1000, resultado.SalarioBruto); 
            Assert.AreEqual(100, resultado.ImpuestoRenta); 
            Assert.IsTrue(resultado.EsSegundaQuincena);   
        }

        [Test]
        public void CalcularPagoMensual_DeduccionVoluntaria_Porcentual()
        {
            var beneficio = new DeduccionVoluntaria
            {
                Id = Guid.NewGuid(),
                Monto = 0.10, // 10%
                Nombre = "Plan Médico",
                esPorcentual = true
            };

            _mockPagoRepo.Setup(r => r.ObtenerTipoContratoEmpleado(_empleadoId)).Returns("Tiempo Completo");
            _mockPagoRepo.Setup(r => r.ObtenerSalarioBruto(_empleadoId, _fechaInicio, _fechaFinal)).Returns(1000);
            _mockPagoRepo.Setup(r => r.ObtenerSalarioEmpleado(_empleadoId)).Returns(1000);
            _mockPagoRepo.Setup(r => r.CalcularImpuestoRenta(1000)).Returns(0);
            _mockPagoRepo.Setup(r => r.ObtenerDeduccionesVoluntarias(_empleadoId)).Returns(new List<DeduccionVoluntaria> { beneficio });

            var resultado = _servicio.CalcularPagoMensual(_empleadoId, _fechaInicio, _fechaFinal);

            Assert.AreEqual(100, resultado.DeduccionesVoluntarias[0].Monto);
            Assert.AreEqual("Plan Médico", resultado.DeduccionesVoluntarias[0].Nombre);
        }
    }
}
