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
            List<Empleado> empleados = _empresaRepo.GetEmpleados(cedula);
            bool hayPagos = HayPagos(empleados);
            EliminarBeneficios(cedula);
            EliminarEmpleados(empleados);
            if (hayPagos)
            {
                _empresaRepo.BorradoFisico(cedula);
            }
            else
            {
                _empresaRepo.BorradoLogico(cedula);
            }
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
            foreach (var beneficioObj in listaBeneficios)
            {
                // Usa reflexión para obtener la propiedad Id del objeto anónimo
                var idProp = beneficioObj.GetType().GetProperty("Id");
                if (idProp != null)
                {
                    var id = idProp.GetValue(beneficioObj)?.ToString();
                    if (!string.IsNullOrEmpty(id))
                    {
                        _beneficioQuery.EliminarBeneficio(id);
                    }
                }
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
            try
            {
                if (tipo == "Empleado")
                {
                    estado = _empresaRepo.GetEstadoEliminadoEmpresaEmpleado(cedulaPersona);
                }
                else if (tipo == "DuenoEmpresa")
                {
                    estado = _empresaRepo.GetEstadoEliminadoEmpresaDueno(cedulaPersona);
                }
                return estado;
            }
            catch (Exception ex)
            {
                return true; // Empresa no existe ya
            }
        }
        public bool GetEstadoEliminadoEmpresa(string nombreEmpresa)
        {
            return _empresaRepo.GetEstadoEliminadoEmpresa(nombreEmpresa);
        }
    }
}
