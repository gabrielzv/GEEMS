using BackendGeems.Domain;
using static BackendGeems.Infraestructure.GEEMSRepo;
namespace BackendGeems.Application
{
    public interface IGEEMSRepo
    {
        public List<Empleado> ObtenerEmpleadosPorEmpresa(string nombreEmpresa);
        public List<Planilla> ObtenerPlanillasPorEmpresa(string nombreEmpresa);
    }

}
