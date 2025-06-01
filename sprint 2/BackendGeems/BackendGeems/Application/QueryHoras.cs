using BackendGeems.Domain;

namespace BackendGeems.Application
{
    public class QueryHoras : IQueryHoras
    {
        private readonly IGEEMSRepo _repoInfrastructure;
        public QueryHoras(IGEEMSRepo repo)
        {
            _repoInfrastructure = repo;
        }
        public bool ValidDate(DateTime date, Guid employeeId)
        {
            List<Registro> registrosEmpleado = _repoInfrastructure.ObtenerRegistros(employeeId);
            bool valid = true;
            foreach (var registro in registrosEmpleado)
            {
                if (registro.Fecha.Date == date.Date)
                {
                    valid = false;
                }
            }
            return valid;
        }
        public void InsertRegister(Registro inserting)
        {
            inserting.Id = Guid.NewGuid();
            _repoInfrastructure.InsertRegister(inserting);
        }
        public Registro GetRegister(Guid Id)
        {
            return _repoInfrastructure.GetRegister(Id);
        }
        public void EditRegister(Registro editing, Guid oldId)
        {
            _repoInfrastructure.EditRegister(editing, oldId);
        }
    }
}
