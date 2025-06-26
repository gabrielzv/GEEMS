using BackendGeems.Domain;
namespace BackendGeems.Application
{
    public class QueryEmpresa : IQueryEmpresa
    {
        private readonly IEmpresaRepo _empresaRepo;
        
        public QueryEmpresa(IEmpresaRepo empresaRepo)
        {
            _empresaRepo = empresaRepo;
        }
        public void EliminarEmpresa(string cedula)
        {
            Empresa empresa = _empresaRepo.GetEmpresa(cedula);
            if (empresa == null)
            {
                return;
            }
            Console.WriteLine($"Cédula Jurídica: {empresa.CedulaJuridica}");
            Console.WriteLine($"Nombre: {empresa.Nombre}");
            Console.WriteLine($"Descripción: {empresa.Descripcion}");
            Console.WriteLine($"Teléfono: {empresa.Telefono}");
            Console.WriteLine($"Correo: {empresa.Correo}");
            Console.WriteLine($"Provincia: {empresa.Provincia}");
            Console.WriteLine($"Cantón: {empresa.Canton}");
            Console.WriteLine($"Distrito: {empresa.Distrito}");
            Console.WriteLine($"Señas: {empresa.Senas}");
            Console.WriteLine($"Modalidad de Pago: {empresa.ModalidadPago}");
            Console.WriteLine($"Máx. Beneficios por Empleado: {empresa.MaxBeneficiosXEmpleado}");
            Console.WriteLine($"¿Está Borrado?: {empresa.EstaBorrado}");
        }
    }
}
