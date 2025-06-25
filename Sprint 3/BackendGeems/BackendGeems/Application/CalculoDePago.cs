using BackendGeems.Domain;
using BackendGeems.Infraestructure;

namespace BackendGeems.Application
{
    public class ServicioCalculoPago
    {
        private readonly IPagoRepo _pagoRepo;

        public ServicioCalculoPago(IPagoRepo pagorRepo)
        {
            _pagoRepo = pagorRepo ?? throw new ArgumentNullException(nameof(pagorRepo));
        }

        public ResultadoPago CalcularPagoMensual(Guid idEmpleado, DateTime fechaInicio, DateTime fechaFinal)
        {
            string tipoContrato = _pagoRepo.ObtenerTipoContratoEmpleado(idEmpleado);
            double salarioBruto = _pagoRepo.ObtenerSalarioBruto(idEmpleado, fechaInicio, fechaFinal);

            if (salarioBruto == -1) throw new Exception("Contrato o salario inválido.");
            if (salarioBruto == -2) throw new Exception("No hay horas aceptadas.");

            if (tipoContrato != "Por Horas" && salarioBruto > 0)
                salarioBruto = _pagoRepo.ObtenerSalarioEmpleado(idEmpleado);

            var resultado = CalcularDeducciones(idEmpleado, salarioBruto, tipoContrato, esQuincenal: false);
            resultado.SalarioBruto = salarioBruto;

            return resultado;
        }

        public ResultadoPago CalcularPagoQuincenal(Guid idEmpleado, DateTime fechaInicio, DateTime fechaFinal)
        {
            string tipoContrato = _pagoRepo.ObtenerTipoContratoEmpleado(idEmpleado);
            double salarioBrutoMensual = _pagoRepo.ObtenerSalarioBruto(idEmpleado, fechaInicio, fechaFinal);
            double salarioBrutoQuincenal;

            if (salarioBrutoMensual == -1) throw new Exception("Contrato o salario inválido.");
            if (salarioBrutoMensual == -2) throw new Exception("No hay horas aceptadas.");

            if (tipoContrato != "Por Horas")
            {
                salarioBrutoMensual = _pagoRepo.ObtenerSalarioEmpleado(idEmpleado);
                salarioBrutoQuincenal = salarioBrutoMensual / 2;
            }
            else
            {
                salarioBrutoQuincenal = salarioBrutoMensual;
            }

            var resultado = CalcularDeducciones(idEmpleado, salarioBrutoMensual, tipoContrato, esQuincenal: true);
            resultado.SalarioBruto = salarioBrutoQuincenal;
            resultado.EsSegundaQuincena = fechaFinal.Day > 15;

            return resultado;
        }

        private ResultadoPago CalcularDeducciones(Guid idEmpleado, double salarioBase, string tipoContrato, bool esQuincenal)
        {
            double impuestoRenta = 0, sem = 0, ivm = 0, bancopopular = 0;
            double total = 0;

            if (tipoContrato == "Medio Tiempo" || tipoContrato == "Tiempo Completo" || tipoContrato == "Por Horas")
            {
                impuestoRenta = _pagoRepo.CalcularImpuestoRenta(salarioBase) / (esQuincenal ? 2 : 1);
                sem = (salarioBase * 0.0550) / (esQuincenal ? 2 : 1);
                ivm = (salarioBase * 0.0417) / (esQuincenal ? 2 : 1);
                bancopopular = (salarioBase * 0.01) / (esQuincenal ? 2 : 1);
                total = impuestoRenta + sem + ivm + bancopopular;
            }

            var beneficios = _pagoRepo.ObtenerDeduccionesVoluntarias(idEmpleado);
            var voluntarias = new List<DeduccionVoluntaria>();

            foreach (var b in beneficios)
            {
                double monto = 0;
                if (b.esAPI)
                {
                    monto = _pagoRepo.ObtenerMontoAPI(idEmpleado, b.NombreDeAPI, salarioBase);
                }else if (b.esPorcentual)
                {
                    monto = salarioBase * b.Monto;
                }
                else
                {
                    monto = b.Monto;
                }

                    monto = esQuincenal ? monto / 2 : monto;

                total += monto;

                if (total > salarioBase)
                    throw new Exception("Las deducciones no pueden exceder el salario bruto.");

                voluntarias.Add(new DeduccionVoluntaria { Id = b.Id, Monto = monto, Nombre = b.Nombre });
            }

            return new ResultadoPago
            {
                DeduccionesVoluntarias = voluntarias,
                ImpuestoRenta = impuestoRenta,
                SEM = sem,
                IVM = ivm,
                BancoPopular = bancopopular,
                TotalDeducciones = total
            };
        }
    }
}
