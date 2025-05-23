using BackendGeems.Domain;


namespace BackendGeems.Infraestructure


{
    public interface IGEEMSRepo
    {
        bool calcularPago(string fechaInicio, string fechaFinal);
        List<Pago> ObtenerPagos(DateTime fechaInicio, DateTime fechaFinal);

        int ObtenerSalarioBruto(Guid idEmpleado, DateTime fechaInicio, DateTime fechaFinal);

    }
}
