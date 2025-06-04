using BackendGeems.Domain;
using static BackendGeems.Infraestructure.GEEMSRepo;
namespace BackendGeems.Application
{
    public interface IGEEMSRepo
    {



        Registro GetRegister(Guid Id);
        void EditRegister(Registro editing, Guid oldId);

        List<Registro> ObtenerRegistros(Guid idEmpleado);
        void InsertRegister(Registro registro);
        public int GetMonthHours(Guid idEmpleado, DateTime fecha);
        public List<Empleado> ObtenerEmpleadosPorEmpresa(string nombreEmpresa);
        public List<PlanillaDTO> ObtenerPlanillasPorEmpresa(string nombreEmpresa);

    }

}
