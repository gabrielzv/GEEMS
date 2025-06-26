using BackendGeems.Application;
using BackendGeems.Domain;
using Microsoft.Data.SqlClient;
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

    }
}
