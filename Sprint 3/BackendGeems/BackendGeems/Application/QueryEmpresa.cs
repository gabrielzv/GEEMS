using BackendGeems.Domain;
using BackendGeems.Infraestructure;
using System.Reflection.Metadata.Ecma335;
namespace BackendGeems.Application
{
    public class QueryEmpresa : IQueryEmpresa
    {
        private readonly IEmpresaRepo _empresaRepo;
        private readonly IPagoRepo _pagoRepo;
        private readonly QueryBeneficio _beneficioQuery;
        private readonly BorradoDeEmpleados _borradoDeEmpleados;
        public QueryEmpresa(IEmpresaRepo empresaRepo, IPagoRepo pagoRepo, IBeneficioRepo beneficioRepo, IEmpleadoRepo empleadoRepo, IReporteService reporteService)
        {
            _beneficioQuery = new QueryBeneficio(beneficioRepo);
            _borradoDeEmpleados = new BorradoDeEmpleados(empleadoRepo, reporteService);
            _pagoRepo = pagoRepo;
            _empresaRepo = empresaRepo;
        }
        public void EliminarEmpresa(string cedula)
        {
            Empresa empresa = _empresaRepo.GetEmpresa(cedula);
            List<Empleado> empleados = _empresaRepo.GetEmpleados(cedula);
            if (HayPagos(empleados))
            {
                _empresaRepo.BorradoFisico(cedula);
            }
            else
            {
                _empresaRepo.BorradoLogico(cedula);
            }
            EliminarBeneficios(cedula);
            EliminarEmpleados(empleados);
        }
        private bool HayPagos(List<Empleado> empleados)
        {
            int totalPagos = 0;
            foreach (var empleado in empleados)
            {
                totalPagos += _pagoRepo.ContarPagos(empleado.Id);
            }
            return totalPagos > 0;
        }
        private void EliminarBeneficios(string cedula)
        {
            List<object> listaBeneficios = _beneficioQuery.GetCompanyBenefits(cedula);
            foreach (Beneficio beneficio in listaBeneficios)
            {
                _beneficioQuery.EliminarBeneficio(beneficio.Id);
            }
        }
        private void EliminarEmpleados(List<Empleado> empleados)
        {
            foreach (Empleado empleado in empleados)
            {
                _borradoDeEmpleados.BorrarEmpleado((empleado.CedulaPersona).ToString());
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
