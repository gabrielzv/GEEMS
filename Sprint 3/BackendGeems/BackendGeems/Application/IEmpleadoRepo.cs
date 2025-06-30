using BackendGeems.Domain;

namespace BackendGeems.Application
{
    public interface IEmpleadoRepo
    {
       
        public bool VerificarRelacionEmpleadoPlanilla(string cedula);
        public void BorrarLogicoEmpleado(string cedula);
        public void BorrarPermanenteEmpleado(string cedula);
        public bool UsuarioEstaBorrado(string cedula);
        public void BorrarTimesheetEmpleado(string cedula);
        public string ObtenerNombreEmpleadoPorCedula(string cedula);
        public string ObtenerCorreoEmpleadoPorCedula(string cedula);

        public string ObtenerNombreEmpresaPorCedula(string cedula);
        public bool UsuarioEstaBorradoId(Guid IdEmpleado);
    }


}
