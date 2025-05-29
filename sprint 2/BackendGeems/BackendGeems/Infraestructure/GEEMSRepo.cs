using BackendGeems.Application;
using BackendGeems.Domain;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlClient;


namespace BackendGeems.Infraestructure
{
    public class GEEMSRepo : IGEEMSRepo
    {
        private SqlConnection _conexion;
        private string _cadenaConexion;

        public GEEMSRepo()
        {
            var builder = WebApplication.CreateBuilder();
            _cadenaConexion = builder.Configuration.GetConnectionString("DefaultConnection");
            _conexion = new SqlConnection(_cadenaConexion);
        }

        private DataTable CrearTablaConsulta(SqlCommand comando)
        {
            SqlDataAdapter adaptador = new SqlDataAdapter(comando);
            DataTable tablaConsulta = new DataTable();
            _conexion.Open();
            adaptador.Fill(tablaConsulta);
            _conexion.Close();
            return tablaConsulta;
        }

        public bool calcularPago(string fechaInicio, string fechaFinal)
        {
            SqlCommand comando = new SqlCommand("calcularPago", _conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@fechaInicio", fechaInicio);
            comando.Parameters.AddWithValue("@fechaFinal", fechaFinal);
            _conexion.Open();
            int filasAfectadas = comando.ExecuteNonQuery();
            _conexion.Close();
            return filasAfectadas > 0;
        }

        public List<Pago> ObtenerPagos(DateTime fechaInicio, DateTime fechaFinal)
        {
            string query = @"SELECT * FROM Pago p WHERE p.FechaInicio >= @fechaInicio 
                     AND p.FechaFinal <= @fechaFinal";
            SqlCommand comando = new SqlCommand(query, _conexion);
            comando.Parameters.AddWithValue("@fechaInicio", fechaInicio);
            comando.Parameters.AddWithValue("@fechaFinal", fechaFinal);

            DataTable tablaConsulta = CrearTablaConsulta(comando);

            List<Pago> pagos = new List<Pago>();
            foreach (DataRow fila in tablaConsulta.Rows)
            {
                Pago pago = new Pago
                {
                    Id = fila["Id"] == DBNull.Value ? Guid.Empty : Guid.Parse(fila["Id"].ToString()),
                    FechaRealizada = fila["FechaRealizada"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(fila["FechaRealizada"]),
                    MontoPago = fila["MontoPago"] == DBNull.Value ? 0m : Convert.ToDecimal(fila["MontoPago"]),
                    IdEmpleado = fila["IdEmpleado"] == DBNull.Value ? Guid.Empty : Guid.Parse(fila["IdEmpleado"].ToString()),
                    IdPayroll = fila["IdPayroll"] == DBNull.Value ? Guid.Empty : Guid.Parse(fila["IdPayroll"].ToString()),
                    IdPlanilla = fila["IdPlanilla"] == DBNull.Value ? Guid.Empty : Guid.Parse(fila["IdPlanilla"].ToString()),
                    MontoBruto = fila["MontoBruto"] == DBNull.Value ? 0m : Convert.ToDecimal(fila["MontoBruto"]),
                    FechaInicio = fila["FechaInicio"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(fila["FechaInicio"]),
                    FechaFinal = fila["FechaFinal"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(fila["FechaFinal"])
                };
                pagos.Add(pago);
            }
            return pagos;
        }

        public int ObtenerSalarioBruto(Guid idEmpleado, DateTime fechaInicio, DateTime fechaFinal)
        {
            int salarioBruto = 0;

            using (SqlCommand comando = new SqlCommand("CalcularSalarioBruto", _conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                comando.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                comando.Parameters.AddWithValue("@FechaFinal", fechaFinal);

                SqlParameter outputParam = new SqlParameter("@SalarioBruto", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                comando.Parameters.Add(outputParam);

                try
                {
                    _conexion.Open();
                    comando.ExecuteNonQuery();

                    if (outputParam.Value != DBNull.Value)
                        salarioBruto = (int)outputParam.Value;
                    else
                        throw new Exception("El procedimiento no retornó un valor de salario bruto.");
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al calcular salario bruto: " + ex.Message);
                }
                finally
                {
                    _conexion.Close();
                }
            }

            return salarioBruto;
        }
        public int CalcularImpuestoRenta(int ingresoMensual)
        {
            int impuesto = 0;

            if (ingresoMensual <= 922000)
            {
                impuesto = 0;
            }
            else if (ingresoMensual <= 1352000)
            {
                impuesto = (int)((ingresoMensual - 922000) * 0.10);
            }
            else if (ingresoMensual <= 2373000)
            {
                impuesto = (int)((1352000 - 922000) * 0.10 + (ingresoMensual - 1352000) * 0.15);
            }
            else if (ingresoMensual <= 4745000)
            {
                impuesto = (int)((1352000 - 922000) * 0.10 + (2373000 - 1352000) * 0.15 + (ingresoMensual - 2373000) * 0.20);
            }
            else
            {
                impuesto = (int)((1352000 - 922000) * 0.10 + (2373000 - 1352000) * 0.15 + (4745000 - 2373000) * 0.20 + (ingresoMensual - 4745000) * 0.25);
            }

            return impuesto;
        }
        public void GenerarPagoEmpleado(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal)
        {
            int salarioBruto = ObtenerSalarioBruto(idEmpleado, fechaInicio, fechaFinal);
            Console.WriteLine("aca1");

           
            TimeSpan duracion = fechaFinal - fechaInicio;
            bool esQuincenal = duracion.TotalDays <= 16;

            int salarioMensualEstimado = esQuincenal ? salarioBruto * 2 : salarioBruto;

            
            int impuestoRentaMensual = CalcularImpuestoRenta(salarioMensualEstimado);

            
            int impuestoRenta = esQuincenal ? impuestoRentaMensual / 2 : impuestoRentaMensual;

            //TODO: Deducciones APIs

            int seguro = (int)(salarioBruto * 0.1067); // 10.67%
            int totalDeducciones = impuestoRenta + seguro;

            // Inicializar lista de deducciones voluntarias
            List<(Guid idBeneficio, int monto)> deduccionesVoluntarias = new();

            string queryBeneficios = @"
SELECT b.Id, b.Costo
FROM BeneficiosEmpleado be
JOIN Beneficio b ON be.IdBeneficio = b.Id
WHERE be.IdEmpleado = @IdEmpleado";

            using (SqlCommand cmd = new SqlCommand(queryBeneficios, _conexion))
            {
                cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                DataTable dt = CrearTablaConsulta(cmd);

                foreach (DataRow row in dt.Rows)
                {
                    Guid idBeneficio = Guid.Parse(row["Id"].ToString());
                    int monto = Convert.ToInt32(row["Costo"]);
                    deduccionesVoluntarias.Add((idBeneficio, monto));
                    totalDeducciones += monto;
                }
            }

            // Ahora sí puedes calcular el MontoPago correctamente
            Guid idPago = Guid.NewGuid();

            string insertPagoQuery = @"INSERT INTO Pago (Id, IdEmpleado, IdPayroll, IdPlanilla, FechaInicio, FechaFinal, MontoBruto, MontoPago, FechaRealizada)
               VALUES (@Id, @IdEmpleado, @IdEmpleado, @IdPlanilla, @FechaInicio, @FechaFinal, @MontoBruto, @MontoPago, @FechaRealizada)";
            using (SqlCommand cmd = new SqlCommand(insertPagoQuery, _conexion))
            {
                cmd.Parameters.AddWithValue("@Id", idPago);
                cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                cmd.Parameters.AddWithValue("@IdPlanilla", idPlanilla);
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@FechaFinal", fechaFinal);
                cmd.Parameters.AddWithValue("@MontoBruto", salarioBruto);
                cmd.Parameters.AddWithValue("@MontoPago", salarioBruto - totalDeducciones); // ahora sí
                cmd.Parameters.AddWithValue("@FechaRealizada", DateTime.Now);

                _conexion.Open();
                cmd.ExecuteNonQuery();
                _conexion.Close();
            }

            // Insertar deducciones obligatorias
            InsertDeduccion(idPago, "Obligatoria", null, impuestoRenta);
            InsertDeduccion(idPago, "Obligatoria", null, seguro);

            // Insertar deducciones voluntarias
            foreach (var (idBeneficio, monto) in deduccionesVoluntarias)
            {
                InsertDeduccion(idPago, "Voluntaria", idBeneficio, monto);
            }

            Console.WriteLine("aca2");
        }

        public void InsertDeduccion(Guid idPago, string tipo, Guid? idBeneficio, int monto)
        {
            string insertQuery = @"INSERT INTO Deducciones (Id, IdPago, TipoDeduccion, IdBeneficio, Monto)
                           VALUES (@Id, @IdPago, @TipoDeduccion, @IdBeneficio, @Monto)";
            using (SqlCommand cmd = new SqlCommand(insertQuery, _conexion))
            {
                cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("@IdPago", idPago);
                cmd.Parameters.AddWithValue("@TipoDeduccion", tipo);
                cmd.Parameters.AddWithValue("@IdBeneficio", (object?)idBeneficio ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Monto", monto);

                _conexion.Open();
                cmd.ExecuteNonQuery();
                _conexion.Close();
            }
        }
    }
}
