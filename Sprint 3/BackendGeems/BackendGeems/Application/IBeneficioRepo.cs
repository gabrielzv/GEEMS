using BackendGeems.Domain;
namespace BackendGeems.Application
{
    public interface IBeneficioRepo
    {
        void CrearBeneficio(Beneficio beneficio);
        void EditarBeneficio(Beneficio beneficio);
        Beneficio GetBeneficio(Guid id);
        List<object> GetCompanyBenefits(string CedulaJuridica);
        List<object> GetBenefitsEmployeeContract(string CedulaJuridica, string IdEmpleado);
        List<object> GetEmployeeBenefits(string IdEmpleado);
        void MatricularBeneficio(BeneficiosEmpleado beneficioEmpleado);
    }
}