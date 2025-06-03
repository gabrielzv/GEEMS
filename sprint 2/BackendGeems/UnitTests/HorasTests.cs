using BackendGeems.API;
using BackendGeems.Application;
using BackendGeems.Infraestructure;
using BackendGeems.Domain;
using System;

namespace UnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void InvalidDateTest()
        {
            // Esta persona se inserta como datos de prueba usando el DUMMYFEED
            string guidString = "2727DAB0-0067-46E5-95B3-97564C6089F6";
            Guid guidTest = Guid.Parse(guidString);
            DateTime fechaTest = new DateTime(2025, 4, 20); // Año, mes, día
            IGEEMSRepo repo = new GEEMSRepo();
            IQueryHoras queryHoras = new QueryHoras(repo);
            var controller = new HorasController(queryHoras);

            bool response = controller.ValidDate(fechaTest, guidTest);

            // El test pasa si la fecha es inválida (por eso esperamos false)
            Assert.IsFalse(response, "Se esperaba que la fecha fuera inválida, pero se devolvió true.");
        }
    }
}