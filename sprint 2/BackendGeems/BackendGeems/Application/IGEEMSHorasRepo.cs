using BackendGeems.Domain;
namespace BackendGeems.Application
{
    public interface IGEEMSHorasRepo
    {
        Registro GetRegister(Guid Id);
        void EditRegister(Registro editing, Guid oldId);
        List<Registro> ObtenerRegistros(Guid idEmpleado);
        void InsertRegister(Registro registro);
        public int GetMonthHours(Guid idEmpleado, DateTime fecha);
    }
}
