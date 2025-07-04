using BackendGeems.Domain;


namespace BackendGeems.Application

{
    public interface IQueryPago
    {
        List<Pago> ObtenerPagos(DateTime fechaInicio, DateTime fechaFinal);
        List<PagoyDeducciones> ObtenerPagosPorEmpleadoyPeriodo(string cedulaEmpleado, DateTime fechaInicio, DateTime fechaFinal);
    }
}
