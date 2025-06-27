using BackendGeems.Application;
using BackendGeems.Domain;
using Microsoft.Data.SqlClient;
using System.Data;


namespace BackendGeems.Infraestructure
{
    public class GeneralRepo : IGeneralRepo
    {
        private SqlConnection _conexion;
        private string _cadenaConexion;

        public string CadenaConexion => _cadenaConexion;

        public GeneralRepo()
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
        public List<Empleado> ObtenerEmpleadosPorEmpresa(string nombreEmpresa)
        {
            List<Empleado> empleados = new List<Empleado>();
            string query = @"SELECT * FROM Empleado WHERE NombreEmpresa = @NombreEmpresa";
            using (SqlCommand comando = new SqlCommand(query, _conexion))
            {
                comando.Parameters.AddWithValue("@NombreEmpresa", nombreEmpresa);
                DataTable tabla = CrearTablaConsulta(comando);
                foreach (DataRow fila in tabla.Rows)
                {
                    empleados.Add(new Empleado
                    {
                        Id = fila["Id"] == DBNull.Value ? Guid.Empty : Guid.Parse(fila["Id"].ToString()),
                        CedulaPersona = fila["CedulaPersona"] == DBNull.Value ? 0 : Convert.ToInt32(fila["CedulaPersona"]),
                        Contrato = fila["Contrato"] == DBNull.Value ? "" : fila["Contrato"].ToString(),
                        NumHorasTrabajadas = fila["NumHorasTrabajadas"] == DBNull.Value ? 0 : Convert.ToInt32(fila["NumHorasTrabajadas"]),
                        Genero = fila["Genero"] == DBNull.Value ? "" : fila["Genero"].ToString(),
                        EstadoLaboral = fila["EstadoLaboral"] == DBNull.Value ? "" : fila["EstadoLaboral"].ToString(),
                        SalarioBruto = fila["SalarioBruto"] == DBNull.Value ? 0 : Convert.ToInt32(fila["SalarioBruto"]),
                        Tipo = fila["Tipo"] == DBNull.Value ? "" : fila["Tipo"].ToString(),
                        FechaIngreso = fila["FechaIngreso"] == DBNull.Value ? "" : fila["FechaIngreso"].ToString(),
                        NombreEmpresa = fila["NombreEmpresa"] == DBNull.Value ? "" : fila["NombreEmpresa"].ToString()
                    });
                }
            }
            return empleados;
        }

        public List<Planilla> ObtenerPlanillasPorEmpresa(string nombreEmpresa)
        {
            List<Planilla> planillas = new List<Planilla>();
            string query = @"
                SELECT p.Id, p.FechaInicio, p.FechaFinal
                FROM Planilla p
                INNER JOIN Empleado e ON p.IdPayroll = e.Id
                WHERE e.NombreEmpresa = @NombreEmpresa
                ORDER BY p.FechaInicio DESC";

            using (SqlCommand comando = new SqlCommand(query, _conexion))
            {
                comando.Parameters.AddWithValue("@NombreEmpresa", nombreEmpresa);
                DataTable tabla = CrearTablaConsulta(comando);
                foreach (DataRow fila in tabla.Rows)
                {
                    planillas.Add(new Planilla
                    {
                        Id = Guid.Parse(fila["Id"].ToString()),
                        FechaInicio = Convert.ToDateTime(fila["FechaInicio"]),
                        FechaFinal = Convert.ToDateTime(fila["FechaFinal"])
                    });
                }
            }
            return planillas;
        }

        public void CrearPlanilla(Planilla planilla)
        {
            using (var connection = new SqlConnection(CadenaConexion))
            {
                var query = @"INSERT INTO Planilla (Id, FechaInicio, FechaFinal, IdPayroll)
                            VALUES (@Id, @FechaInicio, @FechaFinal, @IdPayroll)";
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", planilla.Id);
                    cmd.Parameters.AddWithValue("@FechaInicio", planilla.FechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFinal", planilla.FechaFinal);
                    cmd.Parameters.AddWithValue("@IdPayroll", planilla.IdPayroll);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
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
            using (SqlCommand cmdPlanilla = new SqlCommand (queryPlanilla,_conexion))
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




    }
}
