using BackendGeems.Domain;
using static BackendGeems.Infraestructure.GeneralRepo;
namespace BackendGeems.Application
{
        public interface IGeneralRepo
        {
                public List<Empleado> ObtenerEmpleadosPorEmpresa(string nombreEmpresa);
                public List<Planilla> ObtenerPlanillasPorEmpresa(string nombreEmpresa);

                public Planilla ObtenerPlanillaPorId(Guid id);
                public bool VerificarRelacionEmpleadoPlanilla(string cedula);
                public void BorrarLogicoEmpleado(string cedula);
                public void BorrarPermanenteEmpleado(string cedula);
                public bool UsuarioEstaBorrado(string cedula);
        }


}
