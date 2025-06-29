namespace BackendGeems.Domain
{
    public interface ICalculadoraDeducciones
    {
        ResultadoDeducciones Calcular(decimal salarioBruto);
    }
}
