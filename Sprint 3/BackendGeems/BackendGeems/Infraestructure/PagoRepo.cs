using BackendGeems.Application;
using BackendGeems.Controllers;
using BackendGeems.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;


namespace BackendGeems.Infraestructure
{
    public class PagoRepo : IPagoRepo
    {
        private SqlConnection _conexion;
     
        private string _cadenaConexion;
        
        public string CadenaConexion => _cadenaConexion;

        public PagoRepo()
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
                    MontoPago = fila["MontoPago"] == DBNull.Value ? 0 : Convert.ToDouble(fila["MontoPago"]),
                    IdEmpleado = fila["IdEmpleado"] == DBNull.Value ? Guid.Empty : Guid.Parse(fila["IdEmpleado"].ToString()),
                    IdPayroll = fila["IdPayroll"] == DBNull.Value ? Guid.Empty : Guid.Parse(fila["IdPayroll"].ToString()),
                    IdPlanilla = fila["IdPlanilla"] == DBNull.Value ? Guid.Empty : Guid.Parse(fila["IdPlanilla"].ToString()),
                    MontoBruto = fila["MontoBruto"] == DBNull.Value ? 0 : Convert.ToDouble(fila["MontoBruto"]),
                    FechaInicio = fila["FechaInicio"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(fila["FechaInicio"]),
                    FechaFinal = fila["FechaFinal"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(fila["FechaFinal"])
                };
                pagos.Add(pago);
            }
            return pagos;
        }

        public List<Pago> ObtenerPagosPorEmpleado(Guid idEmpleado)
        {
            var pagos = new List<Pago>();
            using (var connection = new SqlConnection(CadenaConexion))
            {
                var cmd = new SqlCommand("SELECT * FROM Pago     WHERE IdEmpleado = @idEmpleado", connection);
                cmd.Parameters.AddWithValue("@idEmpleado", idEmpleado);
                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pagos.Add(new Pago
                        {
                            Id = reader["Id"] != DBNull.Value ? reader.GetGuid(reader.GetOrdinal("Id")) : Guid.Empty,
                            FechaRealizada = reader["FechaRealizada"] != DBNull.Value ? reader.GetDateTime(reader.GetOrdinal("FechaRealizada")) : DateTime.MinValue,
                            MontoPago = reader["MontoPago"] != DBNull.Value ? Convert.ToDouble(reader["MontoPago"]) : 0,
                            IdEmpleado = reader["IdEmpleado"] != DBNull.Value ? reader.GetGuid(reader.GetOrdinal("IdEmpleado")) : Guid.Empty,
                            IdPayroll = reader["IdPayroll"] != DBNull.Value ? reader.GetGuid(reader.GetOrdinal("IdPayroll")) : Guid.Empty,
                            IdPlanilla = reader["IdPlanilla"] != DBNull.Value ? reader.GetGuid(reader.GetOrdinal("IdPlanilla")) : Guid.Empty,
                            MontoBruto = reader["MontoBruto"] != DBNull.Value ? Convert.ToDouble(reader["MontoBruto"]) : 0,
                            FechaInicio = reader["FechaInicio"] != DBNull.Value ? reader.GetDateTime(reader.GetOrdinal("FechaInicio")) : DateTime.MinValue,
                            FechaFinal = reader["FechaFinal"] != DBNull.Value ? reader.GetDateTime(reader.GetOrdinal("FechaFinal")) : DateTime.MinValue
                        });
                    }
                }
            }
            return pagos;
        }

        public double ObtenerSalarioBruto(Guid idEmpleado, DateTime fechaInicio, DateTime fechaFinal)
        {
            double salarioBruto = 0;

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

        public List<Deduccion> ObtenerDeduccionesPorPago(Guid idPago)
        {
            var deducciones = new List<Deduccion>();
            using (var connection = new SqlConnection(CadenaConexion))
            {
                var cmd = new SqlCommand("SELECT * FROM Deducciones WHERE IdPago = @idPago", connection);
                cmd.Parameters.AddWithValue("@idPago", idPago);
                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        deducciones.Add(new Deduccion
                        {
                            Id = reader["Id"] != DBNull.Value ? reader.GetGuid(reader.GetOrdinal("Id")) : Guid.Empty,
                            IdPago = reader["IdPago"] != DBNull.Value ? reader.GetGuid(reader.GetOrdinal("IdPago")) : Guid.Empty,
                            TipoDeduccion = reader["TipoDeduccion"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("TipoDeduccion")) : string.Empty,
                            IdBeneficio = reader["IdBeneficio"] != DBNull.Value ? (Guid?)reader.GetGuid(reader.GetOrdinal("IdBeneficio")) : null,
                            Monto = reader["Monto"] != DBNull.Value ? Convert.ToDouble(reader["Monto"]) : 0,
                            Nombre = reader["Nombre"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("Nombre")) : string.Empty
                        });
                    }
                }
            }
            return deducciones;
        }

        public double CalcularImpuestoRenta(double ingresoMensual)
        {
            Double impuesto = 0;
            var limite1 = 922000;
            var limite2 = 1352000;
            var limite3 = 2373000;
            var limite4 = 4745000;
            var porcentaje1 = 0.10;
            var porcentaje2 = 0.15;
            var porcentaje3 = 0.20;
            var porcentaje4 = 0.25;

            if (ingresoMensual <= limite1)
            {
                impuesto = 0;
            }
            else if (ingresoMensual <= limite2)
            {
                impuesto = (int)((ingresoMensual - limite1) * porcentaje1);
            }
            else if (ingresoMensual <= limite3)
            {
                impuesto = (int)((limite2 - limite1) * porcentaje1 + (ingresoMensual - limite2) * porcentaje2);
            }
            else if (ingresoMensual <= limite4)
            {
                impuesto = (int)((limite2 - limite1) * porcentaje1 + (limite3 - limite2) * porcentaje2 + (ingresoMensual - limite3) * porcentaje3);
            }
            else
            {
                impuesto = (int)((limite2 - limite1) * porcentaje1 + (limite3 - limite2) * porcentaje2 + (limite4 - limite3) * porcentaje3 + (ingresoMensual - limite4) * porcentaje4);
            }

            return impuesto;
        }
        private bool ExistePago(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal)
        {
            string query = @"SELECT COUNT(*) FROM Pago WHERE IdEmpleado = @IdEmpleado AND IdPlanilla = @IdPlanilla AND FechaInicio = @FechaInicio AND FechaFinal = @FechaFinal";
            using (SqlCommand cmd = new SqlCommand(query, _conexion))
            {
                cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                cmd.Parameters.AddWithValue("@IdPlanilla", idPlanilla);
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@FechaFinal", fechaFinal);
                _conexion.Open();
                int count = (int)cmd.ExecuteScalar();
                _conexion.Close();
                return count > 0;
            }
        }

        public string ObtenerTipoContratoEmpleado(Guid idEmpleado)
        {
            string tipoContrato = null;
            string query = "SELECT Contrato FROM Empleado WHERE Id = @IdEmpleado";

            using (SqlCommand comando = new SqlCommand(query, _conexion))
            {
                comando.Parameters.AddWithValue("@IdEmpleado", idEmpleado);

                try
                {
                    _conexion.Open();
                    var result = comando.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        tipoContrato = result.ToString();

                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener el tipo de contrato: " + ex.Message);
                }
                finally
                {
                    _conexion.Close();
                }
            }
            return tipoContrato;
        }

        public double ObtenerSalarioEmpleado(Guid idEmpleado)
        {
            double salario = 0;
            string query = "SELECT SalarioBruto FROM Empleado WHERE Id = @IdEmpleado";

            using (SqlCommand comando = new SqlCommand(query, _conexion))
            {
                comando.Parameters.AddWithValue("@IdEmpleado", idEmpleado);

                try
                {
                    _conexion.Open();
                    var result = comando.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        salario = Convert.ToDouble(result);
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener el salario del empleado: " + ex.Message);
                }
                finally
                {
                    _conexion.Close();
                }
            }
            return salario;
        }

        public void BorrarPagoExistente(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal)
        {
            if (ExistePago(idEmpleado, idPlanilla, fechaInicio, fechaFinal))
            {
                Console.WriteLine("Ya existe un pago para este empleado en el periodo especificado. Eliminando pago existente.");
                string queryPago = @"SELECT Id FROM Pago WHERE IdEmpleado = @IdEmpleado AND IdPlanilla = @IdPlanilla AND FechaInicio = @FechaInicio AND FechaFinal = @FechaFinal";
                Guid idPagoExistente = Guid.Empty;
                using (SqlCommand cmd = new SqlCommand(queryPago, _conexion))
                {
                    cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                    cmd.Parameters.AddWithValue("@IdPlanilla", idPlanilla);
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFinal", fechaFinal);
                    _conexion.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        idPagoExistente = Guid.Parse(result.ToString());
                    }
                    _conexion.Close();
                }

                if (idPagoExistente != Guid.Empty)
                {

                    string deleteDeducciones = "DELETE FROM Deducciones WHERE IdPago = @IdPago";
                    using (SqlCommand cmd = new SqlCommand(deleteDeducciones, _conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdPago", idPagoExistente);
                        _conexion.Open();
                        cmd.ExecuteNonQuery();
                        _conexion.Close();
                    }


                    string deletePago = "DELETE FROM Pago WHERE Id = @IdPago";
                    using (SqlCommand cmd = new SqlCommand(deletePago, _conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdPago", idPagoExistente);
                        _conexion.Open();
                        cmd.ExecuteNonQuery();
                        _conexion.Close();
                    }
                }
            }
        }

        public void InsertPago(Guid idPago, Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal, double montoBruto, double montoPago)
        {
            string insertPagoQuery = @"INSERT INTO Pago (Id, IdEmpleado, IdPayroll, IdPlanilla, FechaInicio, FechaFinal, MontoBruto, MontoPago, FechaRealizada)
                   VALUES (@Id, @IdEmpleado, @IdEmpleado, @IdPlanilla, @FechaInicio, @FechaFinal, @MontoBruto, @MontoPago, @FechaRealizada)";
            using (SqlCommand cmd = new SqlCommand(insertPagoQuery, _conexion))
            {
                cmd.Parameters.AddWithValue("@Id", idPago);
                cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                cmd.Parameters.AddWithValue("@IdPlanilla", idPlanilla);
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@FechaFinal", fechaFinal);
                cmd.Parameters.AddWithValue("@MontoBruto", montoBruto);
                cmd.Parameters.AddWithValue("@MontoPago", montoPago);
                cmd.Parameters.AddWithValue("@FechaRealizada", DateTime.Now);
                _conexion.Open();
                cmd.ExecuteNonQuery();
                _conexion.Close();
            }
        }
        
        public void InsertDeduccion(Guid idPago, string tipo, Guid? idBeneficio, double monto,string NombreBeneficio)
        {
            try
            {
                string insertQuery = @"INSERT INTO Deducciones (Id, IdPago, TipoDeduccion, IdBeneficio, Monto, Nombre)
                           VALUES (@Id, @IdPago, @TipoDeduccion, @IdBeneficio, @Monto, @Nombre)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, _conexion))
                {
                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@IdPago", idPago);
                    cmd.Parameters.AddWithValue("@TipoDeduccion", tipo);
                    cmd.Parameters.AddWithValue("@IdBeneficio", (object?)idBeneficio ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Monto", monto);
                    cmd.Parameters.AddWithValue("@Nombre", (object?)NombreBeneficio ?? DBNull.Value);

                    _conexion.Open();
                    cmd.ExecuteNonQuery();
                    _conexion.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar deducción: " + ex.Message);
            }
            finally
            {
                if (_conexion.State == ConnectionState.Open)
                {
                    _conexion.Close();
                }


            }
        }

        public bool ExisteEmpleado(Guid idEmpleado)
        {
            string query = "SELECT COUNT(*) FROM Empleado WHERE Id = @IdEmpleado";
            using (SqlCommand comando = new SqlCommand(query, _conexion))
            {
                comando.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                _conexion.Open();
                int count = (int)comando.ExecuteScalar();
                _conexion.Close();
                return count > 0;
            }
        }

        public string GetNombreEmpleadoPorCedula(string cedula)
        {
            string nombreCompleto = null;
            string query = "SELECT NombrePila, Apellido1, Apellido2 FROM Persona WHERE Cedula = @Cedula";

            using (SqlCommand comando = new SqlCommand(query, _conexion))
            {
                comando.Parameters.AddWithValue("@Cedula", cedula);
                try
                {
                    _conexion.Open();
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string nombre = reader["NombrePila"] != DBNull.Value ? reader["NombrePila"].ToString() : "";
                            string apellido1 = reader["Apellido1"] != DBNull.Value ? reader["Apellido1"].ToString() : "";
                            string apellido2 = reader["Apellido2"] != DBNull.Value ? reader["Apellido2"].ToString() : "";
                            nombreCompleto = $"{nombre} {apellido1} {apellido2}".Trim();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener el nombre completo del empleado por cédula: " + ex.Message);
                }
                finally
                {
                    _conexion.Close();
                }
            }
            return nombreCompleto;
        }


        public Empleado ObtenerEmpleado(Guid idEmpleado)
        {
            Empleado empleado = null;
            string query = @"SELECT Id, CedulaPersona, Contrato, NumHorasTrabajadas, Genero, EstadoLaboral, SalarioBruto, Tipo, FechaIngreso, NombreEmpresa, NumDependientes, fechaNacimiento 
                             FROM Empleado 
                             WHERE Id = @IdEmpleado";

            using (SqlCommand comando = new SqlCommand(query, _conexion))
            {
                comando.Parameters.AddWithValue("@IdEmpleado", idEmpleado);

                try
                {
                    _conexion.Open();
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            empleado = new Empleado
                            {
                                Id = reader["Id"] != DBNull.Value ? Guid.Parse(reader["Id"].ToString()) : Guid.Empty,
                                CedulaPersona = reader["CedulaPersona"] != DBNull.Value ? Convert.ToInt32(reader["CedulaPersona"]) : 0,
                                Contrato = reader["Contrato"] != DBNull.Value ? reader["Contrato"].ToString() : null,
                                NumHorasTrabajadas = reader["NumHorasTrabajadas"] != DBNull.Value ? Convert.ToInt32(reader["NumHorasTrabajadas"]) : 0,
                                Genero = reader["Genero"] != DBNull.Value ? reader["Genero"].ToString() : null,
                                EstadoLaboral = reader["EstadoLaboral"] != DBNull.Value ? reader["EstadoLaboral"].ToString() : null,
                                SalarioBruto = reader["SalarioBruto"] != DBNull.Value ? Convert.ToInt32(reader["SalarioBruto"]) : 0,
                                Tipo = reader["Tipo"] != DBNull.Value ? reader["Tipo"].ToString() : null,
                                FechaIngreso = reader["FechaIngreso"] != DBNull.Value ? reader["FechaIngreso"].ToString() : null,
                                NombreEmpresa = reader["NombreEmpresa"] != DBNull.Value ? reader["NombreEmpresa"].ToString() : null,
                                CantidadDependientes = reader["NumDependientes"] != DBNull.Value ? Convert.ToInt32(reader["NumDependientes"]) : 0,
                                fechaNacimiento = reader["fechaNacimiento"] != DBNull.Value ? Convert.ToDateTime(reader["fechaNacimiento"]) : DateTime.MinValue
                            };
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener el empleado: " + ex.Message);
                }
                finally
                {
                    _conexion.Close();
                }
            }
            return empleado;
        }
        public double ObtenerMontoAPI(Guid idEmpleado, string nombreAPI, double salarioBruto)
        {
            try
            {

                Empleado empleado = ObtenerEmpleado(idEmpleado);

                if (nombreAPI == "Asociacion Calculator")
                {

                    var nombreEmpresa = empleado.NombreEmpresa;
                    var builder = WebApplication.CreateBuilder();
                    var configuration = builder.Configuration;
                    var association = new AssociationController(configuration);
                    var request = new AssociationCalculationRequest
                    {
                        AssociationName = nombreEmpresa,
                        EmployeeSalary = salarioBruto
                    };


                    var response = association.CalculateAssociationFee(request).Result;


                    try
                    {

                        var contentResult = (ContentResult)response;


                        string jsonString = contentResult.Content;
                        Console.WriteLine(jsonString);


                        var json = JsonDocument.Parse(jsonString);
                        double rawAmount = json.RootElement.GetProperty("amountToCharge").GetDouble();
                        double amount = Convert.ToDouble(rawAmount);


                        return amount;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al procesar la respuesta de la API: " + ex.Message);
                        throw new Exception("Error al procesar la respuesta de la API: " + ex.Message);
                    }
                }

                else if (nombreAPI == "Poliza Seguros")
                {


                    var fechaNacimiento = empleado.fechaNacimiento;
                    var genero = empleado.Genero == "M" ? "Male" : "Female";

                    var builder = WebApplication.CreateBuilder();
                    var configuration = builder.Configuration;
                    var lifeInsuranceController = new LifeInsuranceController(configuration);

                    var response = lifeInsuranceController.GetPolicyInfo(fechaNacimiento.ToString("yyyy-MM-dd"), genero).Result;

                    try
                    {
                        // Convertir el resultado a ContentResult
                        var contentResult = (ContentResult)response;

                        // Obtener el contenido del JSON
                        string jsonString = contentResult.Content;



                        // Parsear JSON y extraer el valor
                        var json = JsonDocument.Parse(jsonString);
                        Double cost = json.RootElement.GetProperty("monthlyCost").GetInt32();

                        return cost;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al procesar la respuesta de la API: " + ex.Message);
                        return 0;
                    }
                }

                else if (nombreAPI == "MediSeguro")
                {


                    var genero = empleado.Genero == "M" ? "masculino" : "femenino";

                    var builder = WebApplication.CreateBuilder();
                    var configuration = builder.Configuration;
                    var mediSeguroController = new InsuranceController(configuration);
                    var request = new InsuranceCalculationRequest
                    {
                        FechaNacimiento = empleado.fechaNacimiento.ToString("yyyy-MM-dd"),
                        Genero = genero,
                        CantidadDependientes = empleado.CantidadDependientes
                    };

                    var response = mediSeguroController.CalculateInsurance(request).Result;

                    try
                    {
                        var contentResult = (ContentResult)response;
                        string content = contentResult.Content?.Trim();



                        if (double.TryParse(content, out double amount))
                        {
                            return amount;
                        }
                        else
                        {
                            throw new Exception("La respuesta no es un número válido.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al procesar la respuesta de la API: " + ex.Message);
                        throw new Exception("Error al procesar la respuesta de la API: " + ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return 0;
        }

        public List<DeduccionVoluntaria> ObtenerDeduccionesVoluntarias(Guid idEmpleado)
        {
            List<DeduccionVoluntaria> deduccionesVoluntarias = new List<DeduccionVoluntaria>();
            string query = @"
                SELECT b.Id, b.Costo, b.NombreDeAPI, b.EsAPI, b.Nombre, b.EsPorcentual, b.Estado
                FROM BeneficiosEmpleado be
                JOIN Beneficio b ON be.IdBeneficio = b.Id
                WHERE be.IdEmpleado = @IdEmpleado AND (b.Estado = 'Activo' OR b.Estado = 'PendienteBorrado')";
            using (SqlCommand comando = new SqlCommand(query, _conexion))
            {
                comando.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                try
                {
                    _conexion.Open();
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        
                        while (reader.Read())
                        {
                            string estado = reader["Estado"]?.ToString() ?? "";
                            Guid idBeneficio = reader["Id"] != DBNull.Value ? Guid.Parse(reader["Id"].ToString()) : Guid.Empty;

                            if (estado == "Activo" || estado == "PendienteBorrado")
                            {
                                DeduccionVoluntaria deduccion = new DeduccionVoluntaria
                                {
                                    Id = idBeneficio,
                                    Monto = reader["Costo"] != DBNull.Value ? Convert.ToDouble(reader["Costo"]) : 0,
                                    NombreDeAPI = reader["NombreDeAPI"]?.ToString() ?? string.Empty,
                                    esAPI = reader["EsAPI"] != DBNull.Value && Convert.ToBoolean(reader["EsAPI"]),
                                    Nombre = reader["Nombre"]?.ToString() ?? string.Empty,
                                    esPorcentual = reader["EsPorcentual"] != DBNull.Value && Convert.ToBoolean(reader["EsPorcentual"])
                                };
                                deduccionesVoluntarias.Add(deduccion);
                            }
                        }
                        reader.Close();
                      
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener las deducciones voluntarias: " + ex.Message);
                }
                finally
                {
                    _conexion.Close();
                }
            }
            return deduccionesVoluntarias;
        }
        public int ContarPagos(Guid idEmpleado)
        {
            int cantidadPagos = 0;
            string query = "SELECT COUNT(*) FROM Pago WHERE IdEmpleado = @IdEmpleado";

            using (SqlConnection conn = new SqlConnection(_cadenaConexion))
            {
                conn.Open();

                using (SqlCommand comando = new SqlCommand(query, conn))
                {
                    comando.Parameters.AddWithValue("@IdEmpleado", idEmpleado);

                    var result = comando.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        cantidadPagos = Convert.ToInt32(result);
                    }
                }
            }

            return cantidadPagos;
        }
        public void InactivarBeneficiosPendientesPorEmpresa(string nombreEmpresa)
        {
            
            string cedulaJuridica = null;
            string queryCedula = "SELECT CedulaJuridica FROM Empresa WHERE Nombre = @NombreEmpresa";
            using (SqlCommand cmdCedula = new SqlCommand(queryCedula, _conexion))
            {
                cmdCedula.Parameters.AddWithValue("@NombreEmpresa", nombreEmpresa);
                try
                {
                    _conexion.Open();
                    var result = cmdCedula.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        cedulaJuridica = result.ToString();
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener la cédula jurídica de la empresa: " + ex.Message);
                }
                finally
                {
                    _conexion.Close();
                }
            }

            if (string.IsNullOrEmpty(cedulaJuridica))
            {
                throw new Exception("No se encontró la cédula jurídica para la empresa especificada.");
            }

            
            string query = @"
                UPDATE Beneficio
                SET Estado = 'Inactivo', EstaBorrado = 1
                WHERE Estado = 'PendienteBorrado' AND CedulaJuridica = @CedulaJuridica";
            using (SqlCommand comando = new SqlCommand(query, _conexion))
            {
                comando.Parameters.AddWithValue("@CedulaJuridica", cedulaJuridica);
                try
                {
                    _conexion.Open();
                    comando.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al inactivar beneficios pendientes: " + ex.Message);
                }
                finally
                {
                    _conexion.Close();
                }
            }
        }


    }
}