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

        public Planilla ObtenerPlanillaPorId(Guid id)
        {
            Planilla planilla = null;
            string query = @"SELECT * FROM Planilla WHERE Id = @Id";
            using (SqlCommand comando = new SqlCommand(query, _conexion))
            {
                comando.Parameters.AddWithValue("@Id", id);
                DataTable tabla = CrearTablaConsulta(comando);
                if (tabla.Rows.Count > 0)
                {
                    DataRow fila = tabla.Rows[0];
                    planilla = new Planilla
                    {
                        Id = Guid.Parse(fila["Id"].ToString()),
                        FechaInicio = Convert.ToDateTime(fila["FechaInicio"]),
                        FechaFinal = Convert.ToDateTime(fila["FechaFinal"]),
                        IdPayroll = Guid.Parse(fila["IdPayroll"].ToString())
                    };
                }
            }
            return planilla;
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
    }
}
