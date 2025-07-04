using BackendGeems.Domain;

namespace BackendGeems.Application
{
    public interface IReporteRepo
    {
        List<SalarioPorContratoDto> ObtenerSalariosPorContrato(Guid idPlanilla);
        List<DeduccionResumenDto> ObtenerDeduccionesPorPlanilla(Guid idPlanilla);
    }
}
