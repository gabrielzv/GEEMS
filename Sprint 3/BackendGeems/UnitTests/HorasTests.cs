using BackendGeems.API;
using BackendGeems.Application;
using BackendGeems.Infraestructure;
using BackendGeems.Domain;
using BackendGeems.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace UnitTests
{
    public class Tests
    {
        private IConfiguration _configuration;
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void InvalidDateTest()
        {
            // Esta persona se inserta como datos de prueba usando el DUMMYFEED
            string guidString = "00000000-0000-0000-0000-000000000003";
            Guid guidTest = Guid.Parse(guidString);
            DateTime fechaTest = new DateTime(2025, 4, 20); // Año, mes, día
            IHorasRepo repo = new HorasRepo();
            IQueryHoras queryHoras = new QueryHoras(repo);
            var controller = new HorasController(queryHoras);

            bool response = controller.ValidDate(fechaTest, guidTest);

            // El test pasa si la fecha es inválida (por eso esperamos false)
            Assert.IsFalse(response, "Se esperaba que la fecha fuera inválida, pero se devolvió true.");
        }
        [Test]
        public void InvalidHoursTest()
        {
            // Esta persona se inserta como datos de prueba usando el DUMMYFEED
            string guidString = "00000000-0000-0000-0000-000000000003";

            Guid guidTest = Guid.Parse(guidString);
            DateTime fechaTest = new DateTime(2022, 4, 20); // Año, mes, día
            IHorasRepo repo = new HorasRepo();
            IQueryHoras queryHoras = new QueryHoras(repo);
            var controller = new HorasController(queryHoras);
            int horas = 161;
            bool response = controller.ValidHours(fechaTest, guidTest, horas);

            // El test pasa si la fecha es inválida (por eso esperamos false)
            Assert.IsFalse(response, "Se esperaba que las horas fueran inválidas, pero se devolvió true.");
        }
        [Test]
        public void ValidRegisterEditTest()
        {
            // Esta persona se inserta como datos de prueba usando el DUMMYFEED
            string guidString = "00000000-0000-0000-0000-000000000003";

            var registroAnterior = new Registro
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000007"),
                NumHoras = 160,
                Fecha = new DateTime(2025, 4, 20, 0, 0, 0),
                Estado = "Aprobado",
                IdEmpleado = Guid.Parse("00000000-0000-0000-0000-000000000003")
            };

            var registro = new Registro
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000007"),
                NumHoras = 7,
                Fecha = new DateTime(2025, 4, 20, 0, 0, 0),
                Estado = "Aprobado",
                IdEmpleado = Guid.Parse("00000000-0000-0000-0000-000000000003")
            };


            IHorasRepo repo = new HorasRepo();
            IQueryHoras queryHoras = new QueryHoras(repo);
            var controller = new HorasController(queryHoras);

            controller.EditRegister(registro, registro.Id);

            var registroActualizado = controller.GetRegister(registro.Id);

            Assert.AreEqual(registroActualizado.NumHoras, 7);

            // Volver al estado anterior para no afectar las pruebas
            controller.EditRegister(registroAnterior, registro.Id);
        }
    }
}