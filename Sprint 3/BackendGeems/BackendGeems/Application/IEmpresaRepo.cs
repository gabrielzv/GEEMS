using BackendGeems.Domain;
using static BackendGeems.Infraestructure.EmpresaRepo;
namespace BackendGeems.Application
{
    public interface IEmpresaRepo
    {
        Empresa GetEmpresa(string cedula);
    }
}
