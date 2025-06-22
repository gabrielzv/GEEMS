﻿using BackendGeems.Application;
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

        public int ObtenerSalarioEmpleado(Guid idEmpleado)
        {
            int salario = 0;
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
                        salario = Convert.ToInt32(result);
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


        public void GenerarPagoEmpleado(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal)
        {
            try
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

                if (idEmpleado == Guid.Empty || idPlanilla == Guid.Empty)
                {
                    throw new ArgumentException("\nId de empleado o planilla no puede ser vacío.");
                }
                else if (fechaInicio >= fechaFinal)
                {
                    throw new ArgumentException("\nLa fecha de inicio debe ser anterior a la fecha final.");
                }
                if (!ExisteEmpleado(idEmpleado))
                {
                    throw new ArgumentException("\nEl empleado no existe.");
                }

                TimeSpan duracion = fechaFinal - fechaInicio;
                bool esQuincenal = duracion.TotalDays <= 16;
                if (!esQuincenal)
                {
                    Console.WriteLine("Mensual");

                    GenerarPagoEmpleadoMensual(idEmpleado, idPlanilla, fechaInicio, fechaFinal);

                }
                else
                {
                    Console.WriteLine("Quincenal");
                    GenerarPagoEmpleadoQuincenal(idEmpleado, idPlanilla, fechaInicio, fechaFinal);

                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public void GenerarPagoEmpleadoMensual(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal)
        {
            try
            {
                string tipoContrato = ObtenerTipoContratoEmpleado(idEmpleado);
                Console.WriteLine(tipoContrato);
                int salarioBruto = ObtenerSalarioBruto(idEmpleado, fechaInicio, fechaFinal);

                if (salarioBruto == -1)
                {
                    throw new Exception("Contrato o Salario Invalidos");
                }
                else if (salarioBruto == -2)
                {

                    throw new Exception("El empleado no tiene horas aceptadas en el periodo seleccionado.");
                }

                if (tipoContrato == "Medio Tiempo" || tipoContrato == "Tiempo Completo" || tipoContrato == "Servicios Profesionales")
                {
                    if (salarioBruto > 0)
                    {
                        salarioBruto = ObtenerSalarioEmpleado(idEmpleado);
                    }
                }
                else if (tipoContrato == "Por Horas")
                {
                    Console.WriteLine(tipoContrato);
                }

                int totalDeducciones = 0;
                int impuestoRentaMensual = 0;
                int SEM = 0;
                int IVM = 0;
                int BancoPopular = 0;

                if (tipoContrato == "Medio Tiempo" || tipoContrato == "Tiempo Completo" || tipoContrato == "Por Horas")
                {
                    impuestoRentaMensual = CalcularImpuestoRenta(salarioBruto);
                    SEM = (int)(salarioBruto * 0.0550); // 5.50%
                    IVM = (int)(salarioBruto * 0.0417); // 4.17%
                    BancoPopular = (int)(salarioBruto * 0.01); // 1%
                    totalDeducciones = impuestoRentaMensual + SEM + IVM + BancoPopular;
                }

                List<(Guid idBeneficio, int monto)> deduccionesVoluntarias = new();

                string queryBeneficios = @"
                    SELECT b.Id, b.Costo,b.NombreDeAPI,b.EsAPI
                    FROM BeneficiosEmpleado be
                    JOIN Beneficio b ON be.IdBeneficio = b.Id
                    WHERE be.IdEmpleado = @IdEmpleado";
                using (SqlCommand cmd = new SqlCommand(queryBeneficios, _conexion))
                {
                    cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                    DataTable dt = CrearTablaConsulta(cmd);

                    foreach (DataRow row in dt.Rows)
                    {

                        if (row["Id"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["Id"].ToString()))
                        {
                            Guid idBeneficio = Guid.Parse(row["Id"].ToString());
                            int monto = Convert.ToInt32(row["Costo"]);
                            bool esAPI = Convert.ToBoolean(row["EsAPI"]);
                            string nombreDeAPI = row["NombreDeAPI"]?.ToString() ?? string.Empty;
                            if (esAPI && !string.IsNullOrWhiteSpace(nombreDeAPI))
                            {


                                monto = ObtenerMontoAPI(idEmpleado, nombreDeAPI, salarioBruto);

                                deduccionesVoluntarias.Add((idBeneficio, monto));
                                totalDeducciones += monto;
                            }
                            else
                            {
                                deduccionesVoluntarias.Add((idBeneficio, monto));
                                totalDeducciones += monto;
                            }


                            if (totalDeducciones > salarioBruto)
                            {
                                throw new Exception("El total de deducciones no puede ser mayor al salario bruto.");
                            }
                        }
                    }
                }

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
                    cmd.Parameters.AddWithValue("@MontoPago", salarioBruto - totalDeducciones);
                    cmd.Parameters.AddWithValue("@FechaRealizada", DateTime.Now);

                    _conexion.Open();
                    cmd.ExecuteNonQuery();
                    _conexion.Close();
                }

                InsertDeduccion(idPago, "Obligatoria", null, impuestoRentaMensual);
                InsertDeduccion(idPago, "Obligatoria", null, SEM);
                InsertDeduccion(idPago, "Obligatoria", null, IVM);
                InsertDeduccion(idPago, "Obligatoria", null, BancoPopular);

                foreach (var (idBeneficio, monto) in deduccionesVoluntarias)
                {
                    InsertDeduccion(idPago, "Voluntaria", idBeneficio, monto);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            finally
            {
                if (_conexion.State == ConnectionState.Open)
                {
                    _conexion.Close();
                }
            }
        }
        public void GenerarPagoEmpleadoQuincenal(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal)
        {
            try
            {
                Console.WriteLine(idEmpleado);
                string tipoContrato = ObtenerTipoContratoEmpleado(idEmpleado);
                Console.WriteLine(tipoContrato);
                int salarioBruto = ObtenerSalarioBruto(idEmpleado, fechaInicio, fechaFinal);
                int salarioBrutoMensual = 0;
                int salarioBrutoQuincenal = 0;
                Console.WriteLine(salarioBrutoQuincenal);

                if (salarioBruto == -1)
                {
                    throw new Exception("Contrato o Salario Invalidos");
                }
                else if (salarioBruto == -2)
                {

                    throw new Exception("El empleado no tiene horas aceptadas en el periodo seleccionado.");
                }

                if (tipoContrato == "Medio Tiempo" || tipoContrato == "Tiempo Completo" || tipoContrato == "Servicios Profesionales")
                {
                    if (salarioBruto > 0)
                    {
                        salarioBrutoMensual = ObtenerSalarioEmpleado(idEmpleado);
                        salarioBrutoQuincenal = salarioBrutoMensual / 2;
                        Console.WriteLine("Salario mensual " + salarioBrutoMensual);
                        Console.WriteLine("Salario quincenal " + salarioBrutoQuincenal);
                    }
                }


                else if (tipoContrato == "Por Horas")
                {
                    salarioBrutoQuincenal = salarioBruto;
                }
                Console.WriteLine("Salario mensual " + salarioBrutoMensual);
                Console.WriteLine("Salario quincenal " + salarioBrutoQuincenal);

                bool esSegundaQuincena = fechaFinal.Day > 15;

                int totalDeducciones = 0;
                int impuestoRentaMensual = 0;
                int impuestoRentaQuincenal = 0;
                int SEM = 0;
                int IVM = 0;
                int BancoPopular = 0;


                if (tipoContrato == "Medio Tiempo" || tipoContrato == "Tiempo Completo" || tipoContrato == "Por Horas")
                {

                    impuestoRentaMensual = CalcularImpuestoRenta(salarioBrutoMensual);
                    impuestoRentaQuincenal = impuestoRentaMensual / 2;


                    SEM = (int)(salarioBrutoMensual * 0.0550); // 5.50%
                    IVM = (int)(salarioBrutoMensual * 0.0417); // 4.17%
                    BancoPopular = (int)(salarioBrutoMensual * 0.01); // 1%
                    totalDeducciones = impuestoRentaQuincenal + SEM / 2 + IVM / 2 + BancoPopular / 2;
                }

                List<(Guid idBeneficio, int monto)> deduccionesVoluntarias = new();




                string queryBeneficios = @"
                    SELECT b.Id, b.Costo,b.NombreDeAPI,b.EsAPI
                    FROM BeneficiosEmpleado be
                    JOIN Beneficio b ON be.IdBeneficio = b.Id
                    WHERE be.IdEmpleado = @IdEmpleado";
                using (SqlCommand cmd = new SqlCommand(queryBeneficios, _conexion))
                {
                    cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                    DataTable dt = CrearTablaConsulta(cmd);

                    foreach (DataRow row in dt.Rows)
                    {

                        if (row["Id"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["Id"].ToString()))
                        {
                            Guid idBeneficio = Guid.Parse(row["Id"].ToString());
                            int monto = Convert.ToInt32(row["Costo"]);

                            bool esAPI = Convert.ToBoolean(row["EsAPI"]);
                            string nombreDeAPI = row["NombreDeAPI"]?.ToString() ?? string.Empty;
                            if (esAPI && !string.IsNullOrWhiteSpace(nombreDeAPI))
                            {


                                monto = ObtenerMontoAPI(idEmpleado, nombreDeAPI, salarioBrutoMensual);
                                monto = monto / 2;
                                Console.WriteLine("Monto Mensual: " + monto);
                                Console.WriteLine("Monto Quincenal: " + monto);
                                deduccionesVoluntarias.Add((idBeneficio, monto));
                                totalDeducciones += monto;
                            }
                            else
                            {
                                monto = monto / 2;
                                deduccionesVoluntarias.Add((idBeneficio, monto));
                                totalDeducciones += monto;
                            }


                            if (totalDeducciones > salarioBrutoQuincenal)
                            {

                                throw new Exception("El total de deducciones no puede ser mayor al salario bruto.");
                            }
                        }
                    }
                }


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
                    cmd.Parameters.AddWithValue("@MontoBruto", salarioBrutoQuincenal);
                    cmd.Parameters.AddWithValue("@MontoPago", salarioBrutoQuincenal - totalDeducciones);
                    cmd.Parameters.AddWithValue("@FechaRealizada", DateTime.Now);

                    _conexion.Open();
                    cmd.ExecuteNonQuery();
                    _conexion.Close();
                }


                InsertDeduccion(idPago, "Obligatoria", null, impuestoRentaQuincenal);
                InsertDeduccion(idPago, "Obligatoria", null, SEM);
                InsertDeduccion(idPago, "Obligatoria", null, IVM);
                InsertDeduccion(idPago, "Obligatoria", null, BancoPopular);


                if (esSegundaQuincena)
                {
                    foreach (var (idBeneficio, monto) in deduccionesVoluntarias)
                    {
                        InsertDeduccion(idPago, "Voluntaria", idBeneficio, monto);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error En pago Quincenal: " + ex.Message);
                throw new Exception(ex.Message);

            }
            finally
            {
                if (_conexion.State == ConnectionState.Open)
                {
                    _conexion.Close();
                }
            }
        }
        public void GenerarPagoEmpleadoSemanal(Guid idEmpleado, Guid idPlanilla, DateTime fechaInicio, DateTime fechaFinal)
        {
            try
            {
                string tipoContrato = ObtenerTipoContratoEmpleado(idEmpleado);
                Console.WriteLine(tipoContrato);
                int salarioBruto = ObtenerSalarioBruto(idEmpleado, fechaInicio, fechaFinal);
                int salarioBrutoMensual = 0;
                int salarioBrutoSemanal = 0;

                if (salarioBruto == -1)
                {
                    throw new Exception("Contrato o Salario Invalidos");
                }
                else if (salarioBruto == -2)
                {
                    throw new Exception("El empleado no tiene horas aceptadas en el periodo seleccionado.");
                }

                if (tipoContrato == "Medio Tiempo" || tipoContrato == "Tiempo Completo" || tipoContrato == "Servicios Profesionales")
                {
                    if (salarioBruto > 0)
                    {
                        salarioBrutoMensual = ObtenerSalarioEmpleado(idEmpleado);

                        salarioBrutoSemanal = salarioBrutoMensual / 4;
                    }
                }
                else if (tipoContrato == "Por Horas")
                {
                    salarioBrutoSemanal = salarioBruto;
                }

                int totalDeducciones = 0;
                int impuestoRentaMensual = 0;
                int impuestoRentaSemanal = 0;
                int SEM = 0;
                int IVM = 0;
                int BancoPopular = 0;


                if (tipoContrato == "Medio Tiempo" || tipoContrato == "Tiempo Completo" || tipoContrato == "Por Horas")
                {

                    impuestoRentaMensual = CalcularImpuestoRenta(salarioBrutoMensual);
                    impuestoRentaSemanal = impuestoRentaMensual / 4;

                    SEM = (int)(salarioBrutoSemanal * 0.0550); // 5.50%
                    IVM = (int)(salarioBrutoSemanal * 0.0417); // 4.17%
                    BancoPopular = (int)(salarioBrutoSemanal * 0.01); // 1%
                    totalDeducciones = impuestoRentaSemanal + SEM + IVM + BancoPopular;
                }

                List<(Guid idBeneficio, int monto)> deduccionesVoluntarias = new();

                bool esUltimaSemanaDelMes = fechaFinal.AddDays(7).Month != fechaFinal.Month;
                if (esUltimaSemanaDelMes)
                {
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

                            if (row["Id"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["Id"].ToString()))
                            {
                                Guid idBeneficio = Guid.Parse(row["Id"].ToString());
                                int monto = Convert.ToInt32(row["Costo"]);
                                bool esAPI = Convert.ToBoolean(row["EsAPI"]);
                                string nombreDeAPI = row["NombreDeAPI"]?.ToString() ?? string.Empty;
                                if (esAPI && !string.IsNullOrWhiteSpace(nombreDeAPI))
                                {


                                    monto = ObtenerMontoAPI(idEmpleado, nombreDeAPI, salarioBruto);

                                    deduccionesVoluntarias.Add((idBeneficio, monto));
                                    totalDeducciones += monto;
                                }
                                else
                                {
                                    deduccionesVoluntarias.Add((idBeneficio, monto));
                                    totalDeducciones += monto;
                                }


                                if (totalDeducciones > salarioBruto)
                                {
                                    throw new Exception("El total de deducciones no puede ser mayor al salario bruto.");
                                }
                            }
                        }
                    }
                }

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
                    cmd.Parameters.AddWithValue("@MontoBruto", salarioBrutoSemanal);
                    cmd.Parameters.AddWithValue("@MontoPago", salarioBrutoSemanal - totalDeducciones);
                    cmd.Parameters.AddWithValue("@FechaRealizada", DateTime.Now);

                    _conexion.Open();
                    cmd.ExecuteNonQuery();
                    _conexion.Close();
                }


                InsertDeduccion(idPago, "Obligatoria", null, impuestoRentaSemanal);
                InsertDeduccion(idPago, "Obligatoria", null, SEM);
                InsertDeduccion(idPago, "Obligatoria", null, IVM);
                InsertDeduccion(idPago, "Obligatoria", null, BancoPopular);


                if (esUltimaSemanaDelMes)
                {
                    foreach (var (idBeneficio, monto) in deduccionesVoluntarias)
                    {
                        InsertDeduccion(idPago, "Voluntaria", idBeneficio, monto);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (_conexion.State == ConnectionState.Open)
                {
                    _conexion.Close();
                }
            }
        }

        public void InsertDeduccion(Guid idPago, string tipo, Guid? idBeneficio, int monto)
        {
            try
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
        public int ObtenerMontoAPI(Guid idEmpleado, string nombreAPI, int salarioBruto)
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
                        int amount = Convert.ToInt32(rawAmount);


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
                        int cost = json.RootElement.GetProperty("monthlyCost").GetInt32();

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



                        if (int.TryParse(content, out int amount))
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
    }
}