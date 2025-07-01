using BackendGeems.API;
using BackendGeems.Application;
using BackendGeems.Infraestructure;

namespace UnitTests;

public class CalculosTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CalcularImpuestoRenta_DeberiaRetornarImpuestoCorrecto()
    {
        // Arrange
        var repo = new PagoRepo();
    
        int ingresoMensual = 1352000;
        double impuestoEsperado = 43000; 

        // Act
        double impuesto = repo.CalcularImpuestoRenta(ingresoMensual);

        // Assert
        Assert.That(impuesto, Is.EqualTo(impuestoEsperado));
    }

   
    [Test]
        public void GenerarPagoEmpleado_FechaInicioPosteriorAFechaFinal_DeberiaLanzarExcepcion()
        {
            // Arrange
            var repo = new PagoRepo();
        var EmpleadoRepo = new EmpleadoRepo();
        var servicioDeCalculo = new ServicioCalculoPago(repo);
            
            var gestorPagos = new GestorPagosService(repo,servicioDeCalculo,EmpleadoRepo);
            Guid idEmpleado = Guid.NewGuid();
            Guid idPlanilla = Guid.NewGuid();
            DateTime fechaInicio = new DateTime(2024, 7, 1);
            DateTime fechaFinal = new DateTime(2024, 6, 30);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                gestorPagos.GenerarPagoEmpleado(idEmpleado, idPlanilla, fechaInicio, fechaFinal)
            );
        }
    [Test]
    public void GenerarPagoEmpleado_EmpleadoInexistente_DeberiaLanzarExcepcion()
    {
        // Arrange
        var repo = new PagoRepo();
        var EmpleadoRepo = new EmpleadoRepo();
        var servicioDeCalculo = new ServicioCalculoPago(repo);
        var gestorPagos = new GestorPagosService(repo, servicioDeCalculo, EmpleadoRepo);
        Guid idEmpleado = Guid.NewGuid();
        Guid idPlanilla = Guid.NewGuid();
        DateTime fechaInicio = new DateTime(2024, 5, 1);
        DateTime fechaFinal = new DateTime(2024, 6, 30);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            gestorPagos.GenerarPagoEmpleado(idEmpleado, idPlanilla, fechaInicio, fechaFinal)
        );
    }
}
