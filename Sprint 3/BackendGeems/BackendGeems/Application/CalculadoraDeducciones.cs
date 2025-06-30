using BackendGeems.Domain;

namespace BackendGeems.Application
{
    public class CalculadoraDeducciones : ICalculadoraDeducciones
    {
        public ResultadoDeducciones Calcular(decimal salarioBruto)
        {
            var lista = new List<DetalleDeduccion>
            {
                //new DetalleDeduccion { Nombre = "SEM (Enfermedad/Maternidad)", Porcentaje = 0.0550m },
                //new DetalleDeduccion { Nombre = "IVM (Invalidez, Vejez y Muerte)", Porcentaje = 0.0417m },
                new DetalleDeduccion { Nombre = "Cuota Patronal Banco Popular", Porcentaje = 0.0025m },
                new DetalleDeduccion { Nombre = "Asignaciones Familiares", Porcentaje = 0.05m },
                new DetalleDeduccion { Nombre = "IMAS", Porcentaje = 0.005m },
                new DetalleDeduccion { Nombre = "INA", Porcentaje = 0.015m },
                //new DetalleDeduccion { Nombre = "Aporte Banco Popular", Porcentaje = 0.01m },
                new DetalleDeduccion { Nombre = "FCL - Fondo de Capitalización Laboral", Porcentaje = 0.03m },
                new DetalleDeduccion { Nombre = "Fondo de Pensiones Complementarias", Porcentaje = 0.005m },
                new DetalleDeduccion { Nombre = "INS", Porcentaje = 0.01m }
            };

            foreach (var item in lista)
            {
                item.Monto = Math.Round(salarioBruto * item.Porcentaje, 2);
            }

            var total = lista.Sum(x => x.Monto);

            return new ResultadoDeducciones
            {
                SalarioBruto = salarioBruto,
                Deducciones = lista,
                TotalDeducciones = total,
                SalarioNeto = Math.Round(salarioBruto - total, 2)
            };
        }
    }
}
