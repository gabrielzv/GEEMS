using BackendGeems.Application;
using BackendGeems.Domain;

namespace UnitTests;

public class CalculosDeduccionesTest
{
    [Test]
    public void CalcularDeducciones_SalarioBrutoDeUnMillon_DeberiaRetornarMontosCorrectos()
    {
        // Arrange
        var calculadora = new CalculadoraDeducciones();
        decimal salarioBruto = 1_000_000m;
        decimal totalEsperado = Math.Round(
            salarioBruto * 0.0025m + // Banco Popular
            salarioBruto * 0.05m +   // Cosas familiares
            salarioBruto * 0.005m +  // IMAS
            salarioBruto * 0.015m +  // INA
            salarioBruto * 0.03m +   // FCL
            salarioBruto * 0.005m +  // Pensiones 
            salarioBruto * 0.01m     // INS
        , 2);

        // Act
        var resultado = calculadora.Calcular(salarioBruto);

        // Assert
        Assert.That(resultado.TotalDeducciones, Is.EqualTo(totalEsperado));
        Assert.That(resultado.SalarioNeto, Is.EqualTo(Math.Round(salarioBruto - totalEsperado, 2)));
        Assert.That(resultado.Deducciones.Count, Is.EqualTo(7));
    }
}
