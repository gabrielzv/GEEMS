using BackendGeems.Domain;

namespace BackendGeems.Application
{
    public class QueryBeneficio : IQueryBeneficio
    {
        private readonly IBeneficioRepo _repoBeneficio;

        public QueryBeneficio(IBeneficioRepo repo)
        {
            _repoBeneficio = repo;
        }

        public void CrearBeneficio(Beneficio beneficio)
        {
            _repoBeneficio.CrearBeneficio(beneficio);
        }

        public void EditarBeneficio(Beneficio beneficio)
        {
            _repoBeneficio.EditarBeneficio(beneficio);
        }

        public Beneficio GetBeneficio(Guid id)
        {
            return _repoBeneficio.GetBeneficio(id);
        }

        public List<object> GetCompanyBenefits(string CedulaJuridica)
        {
            return _repoBeneficio.GetCompanyBenefits(CedulaJuridica);
        }

        public List<object> GetBenefitsEmployeeContract(string CedulaJuridica, string IdEmpleado)
        {
            return _repoBeneficio.GetBenefitsEmployeeContract(CedulaJuridica, IdEmpleado);
        }

        public List<object> GetEmployeeBenefits(string IdEmpleado)
        {
            return _repoBeneficio.GetEmployeeBenefits(IdEmpleado);
        }
    }
}