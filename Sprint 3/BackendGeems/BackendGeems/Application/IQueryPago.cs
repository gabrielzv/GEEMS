using BackendGeems.Domain;


namespace BackendGeems.Application

{
    public interface IQueryPago
    {
        List<Pago> ObtenerPagos(DateTime fechaInicio, DateTime fechaFinal);
    }
}
