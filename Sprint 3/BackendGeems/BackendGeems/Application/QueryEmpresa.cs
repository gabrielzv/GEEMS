using BackendGeems.Domain;
using BackendGeems.Infraestructure;
using System.Reflection.Metadata.Ecma335;
namespace BackendGeems.Application
{
    public class QueryEmpresa : IQueryEmpresa
    {
        private readonly IEmpresaRepo _empresaRepo;
        private readonly IPagoRepo _pagoRepo;
        public QueryEmpresa(IEmpresaRepo empresaRepo, IPagoRepo pagoRepo)
        {
            _pagoRepo = pagoRepo;
            _empresaRepo = empresaRepo;
        }
        public void EliminarEmpresa(string cedula)
        {
            Empresa empresa = _empresaRepo.GetEmpresa(cedula);
            if (empresa == null)
            {
                return;
            }
            Console.WriteLine($"Eliminando empresa: {empresa.Nombre}");
            List<Empleado> empleados = _empresaRepo.GetEmpleados(cedula);
            int totalPagos = 0;

            foreach (var empleado in empleados)
            {
                totalPagos += _pagoRepo.ContarPagos(empleado.Id);
            }

            Console.WriteLine($"Total de pagos para la empresa {empresa.Nombre}: {totalPagos}");
            if (totalPagos > 0)
            {
                Console.WriteLine("Borrado fisico");
            }
            else
            {
                _empresaRepo.BorradoLogico(cedula);
                Console.WriteLine("Borrado logico");
            }

        }
        public bool GetEstadoEliminadoEmpresaPersona(int cedulaPersona)
        {
            string tipo = _empresaRepo.GetTipo(cedulaPersona);
            bool estado = false;
            if( tipo == "Empleado")
            {
                estado = _empresaRepo.GetEstadoEliminadoEmpresaEmpleado(cedulaPersona);
            }
            else if( tipo == "DuenoEmpresa" )
            {
                estado = _empresaRepo.GetEstadoEliminadoEmpresaDueno(cedulaPersona);
            }
            return estado;
        }
        public bool GetEstadoEliminadoEmpresa(string nombreEmpresa)
        {
            return _empresaRepo.GetEstadoEliminadoEmpresa(nombreEmpresa);
        }
    }
}
