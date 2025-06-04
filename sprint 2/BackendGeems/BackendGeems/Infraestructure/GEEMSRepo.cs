using BackendGeems.Application;
using BackendGeems.Domain;
using Microsoft.Data.SqlClient;
using System.Data;


namespace BackendGeems.Infraestructure
{
    public class GEEMSRepo : IGEEMSRepo
    {
        private SqlConnection _conexion;
        private string _cadenaConexion;

        public string CadenaConexion => _cadenaConexion;

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

        public List<Registro> ObtenerRegistros(Guid idEmpleado)
        {
            List<Registro> registros = new List<Registro>();

            string query = @"SELECT Id, NumHoras, Fecha, Estado, IdEmpleado 
                     FROM Registro 
                     WHERE IdEmpleado = @IdEmpleado";

            using (SqlCommand comando = new SqlCommand(query, _conexion))
            {
                comando.Parameters.AddWithValue("@IdEmpleado", idEmpleado);

                DataTable tablaConsulta = CrearTablaConsulta(comando);

                foreach (DataRow fila in tablaConsulta.Rows)
                {
                    Registro registro = new Registro
                    {
                        Id = fila["Id"] == DBNull.Value ? Guid.Empty : Guid.Parse(fila["Id"].ToString()),
                        NumHoras = fila["NumHoras"] == DBNull.Value ? 0 : Convert.ToInt32(fila["NumHoras"]),
                        Fecha = fila["Fecha"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(fila["Fecha"]),
                        Estado = fila["Estado"]?.ToString(),
                        IdEmpleado = fila["IdEmpleado"] == DBNull.Value ? Guid.Empty : Guid.Parse(fila["IdEmpleado"].ToString())
                    };

                    registros.Add(registro);
                }
            }
            return registros;
        }
        public void InsertRegister(Registro inserting)
        {
            string query = @"INSERT INTO Registro (Id, NumHoras, Fecha, Estado, IdEmpleado)
                     VALUES (@Id, @NumHoras, @Fecha, @Estado, @IdEmpleado)";

            using (SqlCommand comando = new SqlCommand(query, _conexion))
            {
                comando.Parameters.AddWithValue("@Id", inserting.Id);
                comando.Parameters.AddWithValue("@NumHoras", inserting.NumHoras);
                comando.Parameters.AddWithValue("@Fecha", inserting.Fecha);
                comando.Parameters.AddWithValue("@Estado", inserting.Estado ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@IdEmpleado", inserting.IdEmpleado);

                try
                {
                    _conexion.Open();
                    comando.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al insertar el registro: " + ex.Message);
                }
                finally
                {
                    _conexion.Close();
                }
            }
        }
        public Registro GetRegister(Guid id)
        {
            Registro registro = null;

            Console.WriteLine("Se recibe en GetRegister el Guid " + id);

            string query = @"SELECT Id, NumHoras, Fecha, Estado, IdEmpleado 
                     FROM Registro 
                     WHERE Id = @Id";

            using (SqlCommand comando = new SqlCommand(query, _conexion))
            {
                comando.Parameters.AddWithValue("@Id", id);

                DataTable tablaConsulta = CrearTablaConsulta(comando);

                if (tablaConsulta.Rows.Count > 0)
                {
                    DataRow fila = tablaConsulta.Rows[0];

                    registro = new Registro
                    {
                        Id = fila["Id"] == DBNull.Value ? Guid.Empty : Guid.Parse(fila["Id"].ToString()),
                        NumHoras = fila["NumHoras"] == DBNull.Value ? 0 : Convert.ToInt32(fila["NumHoras"]),
                        Fecha = fila["Fecha"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(fila["Fecha"]),
                        Estado = fila["Estado"]?.ToString(),
                        IdEmpleado = fila["IdEmpleado"] == DBNull.Value ? Guid.Empty : Guid.Parse(fila["IdEmpleado"].ToString())
                    };
                }
            }

            return registro;
        }
        public void EditRegister(Registro editing, Guid oldId)
        {

            string query = @"UPDATE Registro
                     SET NumHoras = @NumHoras,
                         Fecha = @Fecha,
                         Estado = @Estado,
                         IdEmpleado = @IdEmpleado
                     WHERE Id = @OldId";

            using (SqlCommand comando = new SqlCommand(query, _conexion))
            {
                comando.Parameters.AddWithValue("@NumHoras", editing.NumHoras);
                comando.Parameters.AddWithValue("@Fecha", editing.Fecha);
                comando.Parameters.AddWithValue("@Estado", editing.Estado ?? (object)DBNull.Value);
                comando.Parameters.AddWithValue("@IdEmpleado", editing.IdEmpleado);
                comando.Parameters.AddWithValue("@OldId", oldId);

                try
                {
                    _conexion.Open();
                    comando.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al editar el registro: " + ex.Message);
                }
                finally
                {
                    _conexion.Close();
                }
            }
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
      

        public int GetMonthHours(Guid idEmpleado, DateTime fecha)
        {
            int horas = 0;
            string query = "SELECT dbo.fnHorasTrabajadasPorMes(@IdEmpleado, @Fecha)";

            using (SqlCommand comando = new SqlCommand(query, _conexion))
            {
                comando.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                comando.Parameters.AddWithValue("@Fecha", fecha);

                try
                {
                    _conexion.Open();
                    var result = comando.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        horas = Convert.ToInt32(result);
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener horas trabajadas: " + ex.Message);
                }
                finally
                {
                    _conexion.Close();
                }
            }
            Console.WriteLine("Se encuentra que la persona tiene horas trabajadas ese mes en: " + horas);
            return horas;
        }
    }
}
