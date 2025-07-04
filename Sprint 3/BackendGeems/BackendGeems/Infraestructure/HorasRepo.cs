using BackendGeems.Application;
using BackendGeems.Domain;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BackendGeems.Infraestructure
{
    public class HorasRepo : IHorasRepo
    {
        private SqlConnection _conexion;
        private string _cadenaConexion;
        public string CadenaConexion => _cadenaConexion;

        public HorasRepo()
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
            using (SqlConnection conn = new SqlConnection(_cadenaConexion))
            {
                conn.Open();
                using (SqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        string query = @"INSERT INTO Registro (Id, NumHoras, Fecha, Estado, IdEmpleado)
                                        VALUES (@Id, @NumHoras, @Fecha, @Estado, @IdEmpleado)";
                        using (SqlCommand comando = new SqlCommand(query, conn, tx))
                        {
                            comando.Parameters.AddWithValue("@Id", inserting.Id);
                            comando.Parameters.AddWithValue("@NumHoras", inserting.NumHoras);
                            comando.Parameters.AddWithValue("@Fecha", inserting.Fecha);
                            comando.Parameters.AddWithValue("@Estado", inserting.Estado ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@IdEmpleado", inserting.IdEmpleado);

                            comando.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                    catch (SqlException ex)
                    {
                        tx.Rollback();
                        throw new Exception("Error al insertar el registro: " + ex.Message);
                    }
                }
            }
        }

        public int GetMonthHours(Guid idEmpleado, DateTime fecha)
        {
            Console.WriteLine("Se entra al GetMonthHours");
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
