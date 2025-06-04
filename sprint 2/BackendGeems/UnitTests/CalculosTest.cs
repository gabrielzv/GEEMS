using BackendGeems.API;
using BackendGeems.Application;
using BackendGeems.Infraestructure;

namespace UnitTests;

public class CalculosTest
{

    GEEMSRepo _repoInfrastructure;
    [SetUp]
    public void Setup()
    {
        _repoInfrastructure = new GEEMSRepo();
    }

    [Test]
    public void CalcularImpuestoRenta_DeberiaRetornarImpuestoCorrecto()
    {
        // Arrange
        var repo = new GEEMSRepo();
        int ingresoMensual = 1352000;
        decimal impuestoEsperado = 43000; 

        // Act
        decimal impuesto = repo.CalcularImpuestoRenta(ingresoMensual);

        // Assert
        Assert.That(impuesto, Is.EqualTo(impuestoEsperado));
    }

   
    [Test]
        public void GenerarPagoEmpleado_FechaInicioPosteriorAFechaFinal_DeberiaLanzarExcepcion()
        {
            // Arrange
            var repo = new GEEMSRepo();
            Guid idEmpleado = Guid.NewGuid();
            Guid idPlanilla = Guid.NewGuid();
            DateTime fechaInicio = new DateTime(2024, 7, 1);
            DateTime fechaFinal = new DateTime(2024, 6, 30);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                repo.GenerarPagoEmpleado(idEmpleado, idPlanilla, fechaInicio, fechaFinal)
            );
        }
    [Test]
    public void GenerarPagoEmpleado_EmpleadoInexistente_DeberiaLanzarExcepcion()
    {
        // Arrange
        var repo = new GEEMSRepo();
        Guid idEmpleado = Guid.NewGuid();
        Guid idPlanilla = Guid.NewGuid();
        DateTime fechaInicio = new DateTime(2024, 5, 1);
        DateTime fechaFinal = new DateTime(2024, 6, 30);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            repo.GenerarPagoEmpleado(idEmpleado, idPlanilla, fechaInicio, fechaFinal)
        );
    }
}
