namespace BackendGeems.Application
{
    public class BorradoDeEmpleados
    {
        private readonly IGeneralRepo _generalRepo;

        public BorradoDeEmpleados(IGeneralRepo generalRepo)
        {
            _generalRepo = generalRepo;
        }

        public void BorrarEmpleado(string Cedula)
        {
            try
            {
                
                bool tienePagos = _generalRepo.VerificarRelacionEmpleadoPlanilla(Cedula);

                if (tienePagos)
                {
                    _generalRepo.BorrarLogicoEmpleado(Cedula);
                }
                else
                {
                    _generalRepo.BorrarPermanenteEmpleado(Cedula);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
    }
}
