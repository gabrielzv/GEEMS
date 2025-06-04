using BackendGeems.Domain;

namespace BackendGeems.Application
{
    public class QueryHoras : IQueryHoras
    {
        private readonly IGEEMSHorasRepo _repoHoras;
        public QueryHoras(IGEEMSHorasRepo repo)
        {
            _repoHoras = repo;
        }
        public bool ValidDate(DateTime date, Guid employeeId)
        {
            List<Registro> registrosEmpleado = _repoHoras.ObtenerRegistros(employeeId);
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
            _repoHoras.InsertRegister(inserting);
        }
        public Registro GetRegister(Guid Id)
        {
            return _repoHoras.GetRegister(Id);
        }
        public void EditRegister(Registro editing, Guid oldId)
        {
            _repoHoras.EditRegister(editing, oldId);
        }
        public bool ValidHours(DateTime date, Guid employeeId, int hours)
        {
            Console.WriteLine("Se entra a ValidHours Aplicacion");
            int workedHours = _repoHoras.GetMonthHours(employeeId, date);
            bool valid = true;
            if(workedHours + hours > 160)
            {
                valid = false;
            }
            return valid;
        }
    }
}
