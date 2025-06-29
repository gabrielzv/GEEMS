using BackendGeems.Application;


using Microsoft.Data.SqlClient;
using System.Data;

namespace BackendGeems.Infraestructure

{
    public class EmpleadoRepo : IEmpleadoRepo
    {

        private SqlConnection _conexion;
        private string _cadenaConexion;

        public string CadenaConexion => _cadenaConexion;

        public EmpleadoRepo()
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
        public bool VerificarRelacionEmpleadoPlanilla(string cedula)
        {

            Guid empleadoId = Guid.Empty;
            string queryEmpleado = "SELECT Id FROM Empleado WHERE CedulaPersona = @Cedula";
            using (SqlCommand cmdEmpleado = new SqlCommand(queryEmpleado, _conexion))
            {
                cmdEmpleado.Parameters.AddWithValue("@Cedula", cedula);
                _conexion.Open();
                var result = cmdEmpleado.ExecuteScalar();
                _conexion.Close();
                if (result == null || result == DBNull.Value)
                    return false;
                empleadoId = Guid.Parse(result.ToString());
            }

            string queryPago = "SELECT COUNT(*) FROM Pago WHERE IdEmpleado = @IdEmpleado";
            using (SqlCommand cmdPago = new SqlCommand(queryPago, _conexion))
            {
                cmdPago.Parameters.AddWithValue("@IdEmpleado", empleadoId);
                _conexion.Open();
                int countPago = (int)cmdPago.ExecuteScalar();
                _conexion.Close();
                if (countPago > 0)
                    return true;
            }
            string queryPlanilla = "SELECT COUNT(*) FROM Planilla WHERE IdPayroll = @IdEmpleado";
            using (SqlCommand cmdPlanilla = new SqlCommand(queryPlanilla, _conexion))
            {
                cmdPlanilla.Parameters.AddWithValue("@IdEmpleado", empleadoId);
                _conexion.Open();
                int countPlanilla = (int)cmdPlanilla.ExecuteScalar();
                _conexion.Close();
                if (countPlanilla > 0)
                    return true;
            }

            return false;
        }

        public void BorrarLogicoEmpleado(string cedula)
        {
            try
            {
                string queryEmpleado = "UPDATE Empleado SET EstaBorrado = 1 WHERE CedulaPersona = @Cedula";
                using (SqlCommand cmdEmpleado = new SqlCommand(queryEmpleado, _conexion))
                {
                    cmdEmpleado.Parameters.AddWithValue("@Cedula", cedula);
                    _conexion.Open();
                    cmdEmpleado.ExecuteNonQuery();
                    _conexion.Close();
                }


                string queryPersona = "UPDATE Persona SET EstaBorrado = 1 WHERE Cedula = @Cedula";
                using (SqlCommand cmdPersona = new SqlCommand(queryPersona, _conexion))
                {
                    cmdPersona.Parameters.AddWithValue("@Cedula", cedula);
                    _conexion.Open();
                    cmdPersona.ExecuteNonQuery();
                    _conexion.Close();
                }


                string queryUsuario = "UPDATE Usuario SET EstaBorrado = 1 WHERE CedulaPersona = @Cedula";
                using (SqlCommand cmdUsuario = new SqlCommand(queryUsuario, _conexion))
                {
                    cmdUsuario.Parameters.AddWithValue("@Cedula", cedula);
                    _conexion.Open();
                    cmdUsuario.ExecuteNonQuery();
                    _conexion.Close();
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public void BorrarPermanenteEmpleado(string cedula)
        {
            try
            {
                string queryUsuario = "DELETE Usuario  WHERE CedulaPersona = @Cedula";
                using (SqlCommand cmdUsuario = new SqlCommand(queryUsuario, _conexion))
                {
                    cmdUsuario.Parameters.AddWithValue("@Cedula", cedula);
                    _conexion.Open();
                    cmdUsuario.ExecuteNonQuery();
                    _conexion.Close();
                }

                string queryEmpleado = "DELETE Empleado  WHERE CedulaPersona = @Cedula";
                using (SqlCommand cmdEmpleado = new SqlCommand(queryEmpleado, _conexion))
                {
                    cmdEmpleado.Parameters.AddWithValue("@Cedula", cedula);
                    _conexion.Open();
                    cmdEmpleado.ExecuteNonQuery();
                    _conexion.Close();
                }


                string queryPersona = "DELETE Persona  WHERE Cedula = @Cedula";
                using (SqlCommand cmdPersona = new SqlCommand(queryPersona, _conexion))
                {
                    cmdPersona.Parameters.AddWithValue("@Cedula", cedula);
                    _conexion.Open();
                    cmdPersona.ExecuteNonQuery();
                    _conexion.Close();
                }



            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        public bool UsuarioEstaBorrado(string cedula)
        {
            string query = "SELECT EstaBorrado FROM Usuario WHERE CedulaPersona = @Cedula";
            using (SqlCommand cmd = new SqlCommand(query, _conexion))
            {
                cmd.Parameters.AddWithValue("@Cedula", cedula);
                _conexion.Open();
                var result = cmd.ExecuteScalar();
                _conexion.Close();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result) == 1;
                }
                return false;
            }
        }

        public void BorrarTimesheetEmpleado(string cedula)
        {
            try
            {

                Guid empleadoId = Guid.Empty;
                string queryEmpleado = "SELECT Id FROM Empleado WHERE CedulaPersona = @Cedula";
                using (SqlCommand cmdEmpleado = new SqlCommand(queryEmpleado, _conexion))
                {
                    cmdEmpleado.Parameters.AddWithValue("@Cedula", cedula);
                    _conexion.Open();
                    var result = cmdEmpleado.ExecuteScalar();
                    _conexion.Close();
                    if (result == null || result == DBNull.Value)
                        return;
                    empleadoId = Guid.Parse(result.ToString());
                }


                string queryBorrarRegistro = "DELETE FROM Registro WHERE IdEmpleado = @IdEmpleado AND Estado = @Estado";
                using (SqlCommand cmdBorrar = new SqlCommand(queryBorrarRegistro, _conexion))
                {
                    cmdBorrar.Parameters.AddWithValue("@IdEmpleado", empleadoId);
                    cmdBorrar.Parameters.AddWithValue("@Estado", "NoRevisado");
                    _conexion.Open();
                    cmdBorrar.ExecuteNonQuery();
                    _conexion.Close();
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public string ObtenerNombreEmpleadoPorCedula(string cedula)
        {
            try
            {
                string NombreEmpleado = string.Empty;
                string apellido1 = string.Empty;
                string apellido2 = string.Empty;
                string queryNombreEmpleado = "SELECT NombrePila, Apellido1, Apellido2 FROM Persona WHERE Cedula = @Cedula";
                using (SqlCommand cmdEmpleado = new SqlCommand(queryNombreEmpleado, _conexion))
                {
                    cmdEmpleado.Parameters.AddWithValue("@Cedula", cedula);
                    _conexion.Open();
                    var result = cmdEmpleado.ExecuteScalar();
                    

                    if (result != null && result != DBNull.Value)
                    {
                        
                        using (SqlDataReader reader = cmdEmpleado.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                NombreEmpleado = reader["NombrePila"].ToString();
                                apellido1 = reader["Apellido1"].ToString();
                                apellido2 = reader["Apellido2"].ToString();
                                NombreEmpleado = $"{NombreEmpleado} {apellido1} {apellido2}";
                            }
                        }
                        _conexion.Close();
                    }
                    else
                    {
                        throw new ArgumentException("Empleado no encontrado con la cédula proporcionada.");
                    }
                }
                return NombreEmpleado;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

        }
        public string ObtenerCorreoEmpleadoPorCedula(string cedula)
        {
            try
            {
                string correoEmpleado = string.Empty;
                string queryCorreoEmpleado = "SELECT Correo FROM Persona WHERE Cedula = @Cedula";
                using (SqlCommand cmdEmpleado = new SqlCommand(queryCorreoEmpleado, _conexion))
                {
                    cmdEmpleado.Parameters.AddWithValue("@Cedula", cedula);
                    _conexion.Open();
                    var result = cmdEmpleado.ExecuteScalar();
                    _conexion.Close();
                    if (result != null && result != DBNull.Value)
                    {
                        correoEmpleado = result.ToString();
                    }
                    else
                    {
                        throw new ArgumentException("Empleado no encontrado con la cédula proporcionada.");
                    }
                }
                return correoEmpleado;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        public string ObtenerNombreEmpresaPorCedula(string cedula)
        {
            try
            {
                string nombreEmpresa = string.Empty;
                string queryNombreEmpresa = "SELECT NombreEmpresa FROM Empleado WHERE CedulaPersona = @Cedula";
                using (SqlCommand cmdEmpresa = new SqlCommand(queryNombreEmpresa, _conexion))
                {
                    cmdEmpresa.Parameters.AddWithValue("@Cedula", cedula);
                    _conexion.Open();
                    var result = cmdEmpresa.ExecuteScalar();
                    _conexion.Close();
                    if (result != null && result != DBNull.Value)
                    {
                        nombreEmpresa = result.ToString();
                    }
                    else
                    {
                        throw new ArgumentException("Empleado no encontrada con la cédula proporcionada.");
                    }
                }
                return nombreEmpresa;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
    }
}
