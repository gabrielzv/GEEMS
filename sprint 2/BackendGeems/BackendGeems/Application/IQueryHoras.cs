using BackendGeems.Domain;
namespace BackendGeems.Application
{
    public interface IQueryHoras
    {
        bool ValidDate(DateTime date, Guid employeeId);
        void InsertRegister(Registro inserting);
    }
}
