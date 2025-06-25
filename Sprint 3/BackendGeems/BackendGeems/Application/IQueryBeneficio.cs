using BackendGeems.Domain;
namespace BackendGeems.Application
{
    public interface IQueryBeneficio
    {
        void CrearBeneficio(Beneficio beneficio);
        void EditarBeneficio(Beneficio beneficio);
        Beneficio GetBeneficio(Guid id);
    }
}