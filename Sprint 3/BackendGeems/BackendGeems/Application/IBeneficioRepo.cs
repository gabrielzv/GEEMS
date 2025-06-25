using BackendGeems.Domain;
namespace BackendGeems.Application
{
    public interface IBeneficioRepo
    {
        void CrearBeneficio(Beneficio beneficio);
        void EditarBeneficio(Beneficio beneficio);
        Beneficio GetBeneficio(Guid id);
    }
}