using BackendGeems.Domain;
using static BackendGeems.Infraestructure.GeneralRepo;
namespace BackendGeems.Application
{
    public interface IGeneralRepo
    {
        public List<Empleado> ObtenerEmpleadosPorEmpresa(string nombreEmpresa);
        public List<Planilla> ObtenerPlanillasPorEmpresa(string nombreEmpresa);

        public Planilla ObtenerPlanillaPorId(Guid id);
    }

}
