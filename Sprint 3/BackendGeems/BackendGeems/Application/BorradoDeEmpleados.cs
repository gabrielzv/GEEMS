namespace BackendGeems.Application
{
    public class BorradoDeEmpleados
    {
        private readonly IEmpleadoRepo _EmpleadoRepo;
        private readonly IReporteService _reporteService;

        public BorradoDeEmpleados(IEmpleadoRepo EmpleadoRepo, IReporteService reporteService)
        {
            _EmpleadoRepo = EmpleadoRepo;
            _reporteService = reporteService;
        }

        public string BorrarEmpleado(string Cedula)
        {
            try
            {
                bool tienePagos = _EmpleadoRepo.VerificarRelacionEmpleadoPlanilla(Cedula);
                string NombreEmpleado = _EmpleadoRepo.ObtenerNombreEmpleadoPorCedula(Cedula);
                string CorreoEmpleado = _EmpleadoRepo.ObtenerCorreoEmpleadoPorCedula(Cedula);
                string NombreEmpresa = _EmpleadoRepo.ObtenerNombreEmpresaPorCedula(Cedula);

                string mensaje = $@"
                    Estimado(a) {NombreEmpleado},

                    Le informamos que su registro como empleado en la empresa {NombreEmpresa} ha sido eliminado del sistema por motivos administrativos.

                    Si considera que esta acción es un error o necesita mayor información, por favor comuníquese con el departamento de Recursos Humanos a la brevedad posible.

                    Agradecemos su comprensión.

                    Atentamente,  
                    Equipo de Administración  
                    {NombreEmpresa}
                    ";
                string asunto = "Notificación de Eliminación de Registro de Empleado";
                _EmpleadoRepo.BorrarTimesheetEmpleado(Cedula);

                if (tienePagos)
                {
                    _EmpleadoRepo.BorrarLogicoEmpleado(Cedula);
                    string message = "Borrado logico completado";
                    _reporteService.EnviarCorreoAsync(CorreoEmpleado, asunto, mensaje);
                    return message;
                }
                else
                {
                    _EmpleadoRepo.BorrarPermanenteEmpleado(Cedula);
                    string message = "Borrado Permanente completado";
                    _reporteService.EnviarCorreoAsync(CorreoEmpleado, asunto, mensaje);
                    return message;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public bool UsuarioActivo(string cedula)
        {
            bool resultado = _EmpleadoRepo.UsuarioEstaBorrado(cedula);
            if (resultado)
            {
                return false;
            }
            return true;
        }
    }
}
