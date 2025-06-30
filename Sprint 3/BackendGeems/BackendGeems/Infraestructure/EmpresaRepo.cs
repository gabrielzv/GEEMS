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

            using SqlTransaction transaction = conn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
            try
            {
                string query = "SELECT * FROM Empresa WHERE CedulaJuridica = @CedulaJuridica";

                using SqlCommand cmd = new SqlCommand(query, conn, transaction);
                cmd.Parameters.AddWithValue("@CedulaJuridica", cedula);

                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Empresa empresa = new Empresa
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

                    transaction.Commit();
                    return empresa;
                }

                transaction.Commit();
                return null;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error al obtener la empresa: " + ex.Message);
            }
        }

        public List<Empleado> GetEmpleados(string cedula)
        {
            List<Empleado> empleados = new List<Empleado>();

            using SqlConnection conn = new SqlConnection(_cadenaConexion);
            conn.Open();

            using SqlTransaction transaction = conn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
            try
            {
                string query = @"
                    SELECT em.* 
                    FROM Empleado em
                    JOIN Empresa e ON em.NombreEmpresa = e.Nombre
                    WHERE e.CedulaJuridica = @CedulaJuridica";

                using SqlCommand cmd = new SqlCommand(query, conn, transaction);
                cmd.Parameters.AddWithValue("@CedulaJuridica", cedula);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
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
                }

                transaction.Commit();
                return empleados;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error al obtener los empleados: " + ex.Message);
            }
        }

        public void BorradoLogico(string cedula)
        {
            using SqlConnection conn = new SqlConnection(_cadenaConexion);
            conn.Open();

            using SqlTransaction transaction = conn.BeginTransaction();
    {
                try
                {
                    string query = @"UPDATE Empresa 
                             SET EstaBorrado = 1 
                             WHERE CedulaJuridica = @CedulaJuridica";

                    using SqlCommand cmd = new SqlCommand(query, conn, transaction);
                    cmd.Parameters.AddWithValue("@CedulaJuridica", cedula);
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error al realizar el borrado lógico: " + ex.Message);
                }
            }
        }

        public void BorradoFisico(string cedula)
        {
            using SqlConnection conn = new SqlConnection(_cadenaConexion);
            conn.Open();

            using SqlTransaction transaction = conn.BeginTransaction();
    {
                try
                {
                    EliminarDatosPrivadosEmpresa(conn, transaction, cedula);
                    EliminarSuperAdminEmpresa(conn, transaction, cedula);
                    EliminarEmpresa(conn, transaction, cedula);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error al borrar la empresa y sus referencias: " + ex.Message);
                }
            }
        }
        private void EliminarDatosPrivadosEmpresa(SqlConnection conn, SqlTransaction transaction, string cedula)
        {
            string query = @"DELETE FROM DatosPrivadosEmpresa WHERE CedulaJuridica = @CedulaJuridica";
            using SqlCommand cmd = new SqlCommand(query, conn, transaction);
            cmd.Parameters.AddWithValue("@CedulaJuridica", cedula);
            cmd.ExecuteNonQuery();
        }
        private void EliminarSuperAdminEmpresa(SqlConnection conn, SqlTransaction transaction, string cedula)
        {
            string query = @"DELETE FROM SuperAdminAdministraEmpresa WHERE CedulaJuridicaEmpresa = @CedulaJuridica";
            using SqlCommand cmd = new SqlCommand(query, conn, transaction);
            cmd.Parameters.AddWithValue("@CedulaJuridica", cedula);
            cmd.ExecuteNonQuery();
        }
        private void EliminarEmpresa(SqlConnection conn, SqlTransaction transaction, string cedula)
        {
            string query = @"DELETE FROM Empresa WHERE CedulaJuridica = @CedulaJuridica";
            using SqlCommand cmd = new SqlCommand(query, conn, transaction);
            cmd.Parameters.AddWithValue("@CedulaJuridica", cedula);
            cmd.ExecuteNonQuery();
        }
        public bool GetEstadoEliminadoEmpresaEmpleado(int cedulaPersona)
        {
            using SqlConnection conn = new SqlConnection(_cadenaConexion);
            conn.Open();

            using SqlTransaction transaction = conn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
            try
            {
                string query = @"
                    SELECT e.EstaBorrado
                    FROM Empleado em
                    JOIN Empresa e ON em.NombreEmpresa = e.Nombre
                    WHERE em.CedulaPersona = @CedulaPersona";

                using SqlCommand cmd = new SqlCommand(query, conn, transaction);
                cmd.Parameters.AddWithValue("@CedulaPersona", cedulaPersona);

                object result = cmd.ExecuteScalar();

                transaction.Commit();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToBoolean(result);
                }

                throw new Exception($"No se encontró empresa para la persona con cédula {cedulaPersona}");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error al obtener el estado de eliminación de la empresa del empleado: " + ex.Message);
            }
        }
        public bool GetEstadoEliminadoEmpresaDueno(int cedulaPersona)
        {
            using SqlConnection conn = new SqlConnection(_cadenaConexion);
            conn.Open();

            using SqlTransaction transaction = conn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
            try
            {
                string query = @"
                    SELECT e.EstaBorrado
                    FROM DuenoEmpresa d
                    JOIN Empresa e ON d.CedulaEmpresa = e.CedulaJuridica
                    WHERE d.CedulaPersona = @CedulaPersona";

                using SqlCommand cmd = new SqlCommand(query, conn, transaction);
                cmd.Parameters.AddWithValue("@CedulaPersona", cedulaPersona);

                object result = cmd.ExecuteScalar();

                transaction.Commit();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToBoolean(result);
                }

                throw new Exception($"No se encontró empresa asociada al dueño con cédula {cedulaPersona}");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error al obtener el estado de eliminación de la empresa del dueño: " + ex.Message);
            }
        }
        public string GetTipo(int cedulaPersona)
        {
            using SqlConnection conn = new SqlConnection(_cadenaConexion);
            conn.Open();

            using SqlTransaction transaction = conn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
            try
            {
                string query = @"
                    SELECT Tipo
                    FROM Usuario
                    WHERE CedulaPersona = @CedulaPersona";

                using SqlCommand cmd = new SqlCommand(query, conn, transaction);
                cmd.Parameters.AddWithValue("@CedulaPersona", cedulaPersona);

                object result = cmd.ExecuteScalar();

                transaction.Commit();

                if (result != null && result != DBNull.Value)
                {
                    return result.ToString();
                }

                throw new Exception($"No se encontró el tipo para la persona con cédula {cedulaPersona}");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error al obtener el tipo de usuario: " + ex.Message);
            }
        }
        public bool GetEstadoEliminadoEmpresa(string nombreEmpresa)
        {
            using SqlConnection conn = new SqlConnection(_cadenaConexion);
            conn.Open();

            using SqlTransaction transaction = conn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
            try
            {
                string query = @"
                    SELECT e.EstaBorrado
                    FROM Empresa e
                    WHERE e.Nombre = @Nombre";

                using SqlCommand cmd = new SqlCommand(query, conn, transaction);
                cmd.Parameters.AddWithValue("@Nombre", nombreEmpresa);

                object result = cmd.ExecuteScalar();

                transaction.Commit();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToBoolean(result);
                }

                throw new Exception($"No se encontró empresa asociada al dueño con cédula {nombreEmpresa}");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error al obtener el estado de eliminación de la empresa: " + ex.Message);
            }
        }


    }
}
