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
    }
}