
using BackendGeems.Domain;
using BackendGeems.Infraestructure;
namespace BackendGeems.Application
{
    public class GestorPagosService
    {
        private readonly IPagoRepo _pagoRepo;
        private readonly ServicioCalculoPago _servicioCalculo;

        public GestorPagosService(IPagoRepo pagoRepo, ServicioCalculoPago servicioCalculo)
        {
            _pagoRepo = pagoRepo;
            _servicioCalculo = servicioCalculo;
        }

        public void GenerarPagoEmpleado(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal)
        {
            try
            {
               
                _pagoRepo.BorrarPagoExistente(idEmpleado, idPlanilla, fechaInicio, fechaFinal);

                if (idEmpleado == Guid.Empty || idPlanilla == Guid.Empty)
                {
                    throw new ArgumentException("\nId de empleado o planilla no puede ser vacío.");
                }
                else if (fechaInicio >= fechaFinal)
                {
                    throw new ArgumentException("\nLa fecha de inicio debe ser anterior a la fecha final.");
                }
                if (!_pagoRepo.ExisteEmpleado(idEmpleado))
                {
                    throw new ArgumentException("\nEl empleado no existe.");
                }

                TimeSpan duracion = fechaFinal - fechaInicio;
                bool esQuincenal = duracion.TotalDays <= 16;
                if (!esQuincenal)
                {
                    Console.WriteLine("Mensual");

                    GenerarPagoMensual(idEmpleado, idPlanilla, fechaInicio, fechaFinal);

                }
                else
                {
                    Console.WriteLine("Quincenal");
                    GenerarPagoQuincenal(idEmpleado, idPlanilla, fechaInicio, fechaFinal);

                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }   

        public void GenerarPagoMensual(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal)
        {
            try
            {
                var resultado = _servicioCalculo.CalcularPagoMensual(idEmpleado, fechaInicio, fechaFinal);

                Guid idPago = Guid.NewGuid();

                _pagoRepo.InsertPago(idPago, idEmpleado, idPlanilla, fechaInicio, fechaFinal, resultado.SalarioBruto, resultado.SalarioBruto - resultado.TotalDeducciones);

                _pagoRepo.InsertDeduccion(idPago, "Obligatoria", null, resultado.ImpuestoRenta, "Impuesto De Renta");
                _pagoRepo.InsertDeduccion(idPago, "Obligatoria", null, resultado.SEM, "SEM");
                _pagoRepo.InsertDeduccion(idPago, "Obligatoria", null, resultado.IVM, "IVM");
                _pagoRepo.InsertDeduccion(idPago, "Obligatoria", null, resultado.BancoPopular, "Banco Popular");

                foreach (var deduccion in resultado.DeduccionesVoluntarias)
                {
                    _pagoRepo.InsertDeduccion(idPago, "Voluntaria", deduccion.Id, deduccion.Monto, deduccion.Nombre);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en GenerarPagoMensual: " + ex.Message);
            }
        }

        public void GenerarPagoQuincenal(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal)
        {
            try
            {
                var resultado = _servicioCalculo.CalcularPagoQuincenal(idEmpleado, fechaInicio, fechaFinal);

                Guid idPago = Guid.NewGuid();

                _pagoRepo.InsertPago(idPago, idEmpleado, idPlanilla, fechaInicio, fechaFinal, resultado.SalarioBruto, resultado.SalarioBruto - resultado.TotalDeducciones);

                _pagoRepo.InsertDeduccion(idPago, "Obligatoria", null, resultado.ImpuestoRenta, "Impuesto de Renta Quincenal");
                _pagoRepo.InsertDeduccion(idPago, "Obligatoria", null, resultado.SEM, "SEM");
                _pagoRepo.InsertDeduccion(idPago, "Obligatoria", null, resultado.IVM, "IVM");
                _pagoRepo.InsertDeduccion(idPago, "Obligatoria", null, resultado.BancoPopular, "Banco Popular");

                if (resultado.EsSegundaQuincena)
                {
                    foreach (var deduccion in resultado.DeduccionesVoluntarias)
                    {
                        _pagoRepo.InsertDeduccion(idPago, "Voluntaria", deduccion.Id, deduccion.Monto, deduccion.Nombre);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en GenerarPagoQuincenal: " + ex.Message);
            }
        }
    }
}
