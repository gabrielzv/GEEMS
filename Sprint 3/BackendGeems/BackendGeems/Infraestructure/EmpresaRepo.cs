using BackendGeems.Application;
using BackendGeems.Domain;
using Microsoft.Data.SqlClient;
using System.Transactions;
namespace BackendGeems.Infraestructure
{
    public class EmpresaRepo : IEmpresaRepo
    {
        private SqlConnection _conexion;
        private string _cadenaConexion;
        public EmpresaRepo()
        {
            var builder = WebApplication.CreateBuilder();
            _cadenaConexion = builder.Configuration.GetConnectionString("DefaultConnection");
            _conexion = new SqlConnection(_cadenaConexion);
        }
        public Empresa GetEmpresa(string cedula)
        {
            using SqlConnection conn = new SqlConnection(_cadenaConexion);
            conn.Open();

            string query = "SELECT * FROM Empresa WHERE CedulaJuridica = @CedulaJuridica";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CedulaJuridica", cedula);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Empresa
                {
                    CedulaJuridica = reader["CedulaJuridica"].ToString(),
                    Nombre = reader["Nombre"].ToString(),
                    Descripcion = reader["Descripcion"].ToString(),
                    Telefono = reader["Telefono"].ToString(),
                    Correo = reader["Correo"].ToString(),
                    Provincia = reader["Provincia"].ToString(),
                    Canton = reader["Canton"].ToString(),
                    Distrito = reader["Distrito"].ToString(),
                    Senas = reader["Senas"].ToString(),
                    ModalidadPago = reader["ModalidadPago"] == DBNull.Value ? null : reader["ModalidadPago"].ToString(),
                    MaxBeneficiosXEmpleado = Convert.ToInt32(reader["MaxBeneficiosXEmpleado"]),
                    EstaBorrado = Convert.ToBoolean(reader["EstaBorrado"])
                };
            }
            return null;
        }
        public List<Empleado> GetEmpleados(string cedula)
        {
            List<Empleado> empleados = new List<Empleado>();

            using SqlConnection conn = new SqlConnection(_cadenaConexion);
            conn.Open();

            string query = @"SELECT * FROM Empleado WHERE NombreEmpresa = (
                        SELECT Nombre FROM Empresa WHERE CedulaJuridica = @CedulaJuridica
                     )";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CedulaJuridica", cedula);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                empleados.Add(new Empleado
                {
                    Id = Guid.Parse(reader["Id"].ToString()),
                    CedulaPersona = Convert.ToInt32(reader["CedulaPersona"]),
                    Contrato = reader["Contrato"].ToString(),
                    NumHorasTrabajadas = Convert.ToInt32(reader["NumHorasTrabajadas"]),
                    Genero = reader["Genero"].ToString(),
                    EstadoLaboral = reader["EstadoLaboral"].ToString(),
                    SalarioBruto = Convert.ToInt32(reader["SalarioBruto"]),
                    Tipo = reader["Tipo"].ToString(),
                    FechaIngreso = reader["FechaIngreso"].ToString(),
                    NombreEmpresa = reader["NombreEmpresa"].ToString(),
                    CantidadDependientes = Convert.ToInt32(reader["NumDependientes"]),
                    fechaNacimiento = Convert.ToDateTime(reader["FechaNacimiento"])
                });
            }

            return empleados;
        }
        public void BorradoLogico(string cedula)
        {
            using SqlConnection conn = new SqlConnection(_cadenaConexion);
            conn.Open();

            string query = @"UPDATE Empresa 
                     SET EstaBorrado = 1 
                     WHERE CedulaJuridica = @CedulaJuridica";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CedulaJuridica", cedula);

            cmd.ExecuteNonQuery();
        }
        public void BorradoFisico(string cedula)
        {
            using SqlConnection conn = new SqlConnection(_cadenaConexion);
            conn.Open();

            using SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                string queryDatosPrivados = @"DELETE FROM DatosPrivadosEmpresa 
                                      WHERE CedulaJuridica = @CedulaJuridica";
                using SqlCommand cmdDatosPrivados = new SqlCommand(queryDatosPrivados, conn, transaction);
                cmdDatosPrivados.Parameters.AddWithValue("@CedulaJuridica", cedula);
                cmdDatosPrivados.ExecuteNonQuery();

                string queryEmpresa = @"DELETE FROM Empresa 
                                WHERE CedulaJuridica = @CedulaJuridica";
                using SqlCommand cmdEmpresa = new SqlCommand(queryEmpresa, conn, transaction);
                cmdEmpresa.Parameters.AddWithValue("@CedulaJuridica", cedula);
                cmdEmpresa.ExecuteNonQuery();

                string queryUpdateDueno = @"UPDATE DuenoEmpresa
                                    SET CedulaEmpresa = 'ELIMINADO'
                                    WHERE CedulaEmpresa = @CedulaJuridica";
                using SqlCommand cmdUpdateDueno = new SqlCommand(queryUpdateDueno, conn, transaction);
                cmdUpdateDueno.Parameters.AddWithValue("@CedulaJuridica", cedula);
                cmdUpdateDueno.ExecuteNonQuery();

                string queryDeleteSuperAdmin = @"DELETE FROM SuperAdminAdministraEmpresa
                                         WHERE CedulaJuridicaEmpresa = @CedulaJuridica";
                using SqlCommand cmdDeleteSuperAdmin = new SqlCommand(queryDeleteSuperAdmin, conn, transaction);
                cmdDeleteSuperAdmin.Parameters.AddWithValue("@CedulaJuridica", cedula);
                cmdDeleteSuperAdmin.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error al borrar la empresa y sus referencias: " + ex.Message);
            }
        }


        public bool GetEstadoEliminadoEmpresaEmpleado(int cedulaPersona)
        {
            using SqlConnection conn = new SqlConnection(_cadenaConexion);
            conn.Open();

            string query = @"
                SELECT e.EstaBorrado
                FROM Empleado em
                JOIN Empresa e ON em.NombreEmpresa = e.Nombre
                WHERE em.CedulaPersona = @CedulaPersona";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CedulaPersona", cedulaPersona);

            object result = cmd.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                return Convert.ToBoolean(result);
            }

            throw new Exception($"No se encontró empresa para la persona con cédula {cedulaPersona}");
        }
        public bool GetEstadoEliminadoEmpresaDueno(int cedulaPersona)
        {
            using SqlConnection conn = new SqlConnection(_cadenaConexion);
            conn.Open();

            string query = @"
                SELECT e.EstaBorrado
                FROM DuenoEmpresa d
                JOIN Empresa e ON d.CedulaEmpresa = e.CedulaJuridica
                WHERE d.CedulaPersona = @CedulaPersona";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CedulaPersona", cedulaPersona);

            object result = cmd.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                return Convert.ToBoolean(result);
            }

            throw new Exception($"No se encontró empresa asociada al dueño con cédula {cedulaPersona}");
        }

        public string GetTipo(int cedulaPersona)
        {
            using SqlConnection conn = new SqlConnection(_cadenaConexion);
            conn.Open();

            string query = @"
                SELECT Tipo
                FROM Usuario
                WHERE CedulaPersona = @CedulaPersona";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CedulaPersona", cedulaPersona);

            object result = cmd.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                return result.ToString();
            }

            throw new Exception($"No se encontró el tipo para la persona con cédula {cedulaPersona}");
        }
        public bool GetEstadoEliminadoEmpresa(string nombreEmpresa)
        {
            using SqlConnection conn = new SqlConnection(_cadenaConexion);
            conn.Open();

            string query = @"
                SELECT e.EstaBorrado
                FROM Empresa e
                WHERE e.Nombre = @Nombre";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Nombre", nombreEmpresa);

            object result = cmd.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                return Convert.ToBoolean(result);
            }

            throw new Exception($"No se encontró empresa asociada al dueño con cédula {nombreEmpresa}");
        }

    }
}
