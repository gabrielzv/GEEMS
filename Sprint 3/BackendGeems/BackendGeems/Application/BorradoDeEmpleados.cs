namespace BackendGeems.Application
{
    public class BorradoDeEmpleados
    {
        private readonly IGeneralRepo _generalRepo;

        public BorradoDeEmpleados(IGeneralRepo generalRepo)
        {
            _generalRepo = generalRepo;
        }

        public string BorrarEmpleado(string Cedula)
        {
            try
            {
                
                bool tienePagos = _generalRepo.VerificarRelacionEmpleadoPlanilla(Cedula);

                if (tienePagos)
                {
                    _generalRepo.BorrarLogicoEmpleado(Cedula);
                    string message = "Borrado logico completado";
                    return message;
                }
                else
                {
                    _generalRepo.BorrarPermanenteEmpleado(Cedula);
                    string message = "Borrado Permanente completado";
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
            bool resultado = _generalRepo.UsuarioEstaBorrado(cedula);
            if (resultado)
            {
                return false;
            }
            return true;
        }
    }
}
