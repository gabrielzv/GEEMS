using BackendGeems.Domain;
using static BackendGeems.Infraestructure.EmpresaRepo;
namespace BackendGeems.Application
{
    public interface IEmpresaRepo
    {
        Empresa GetEmpresa(string cedula);
        List<Empleado> GetEmpleados(string cedula);
        void BorradoLogico(string cedula);
        bool GetEstadoEliminadoEmpresaEmpleado(int cedulaPersona);
        bool GetEstadoEliminadoEmpresaDueno(int cedulaPersona);
        string GetTipo(int cedulaPersona);
    }
}
