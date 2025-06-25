using BackendGeems.Application;
using BackendGeems.Domain;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BackendGeems.Infraestructure
{
    public class BeneficioRepo : IBeneficioRepo
    {
        private SqlConnection _conexion;
        private string _cadenaConexion;
        public string CadenaConexion => _cadenaConexion;

        public BeneficioRepo()
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

        public void CrearBeneficio(Beneficio beneficio)
        {
            // Se verifica si el nombre del beneficio ya existe para esa empresa
            string checkQuery = "SELECT Id FROM Beneficio WHERE Nombre = @Nombre AND CedulaJuridica = @CedulaJuridica";
            using (SqlCommand checkCommand = new SqlCommand(checkQuery, _conexion))
            {
                checkCommand.Parameters.AddWithValue("@Nombre", beneficio.Nombre);
                checkCommand.Parameters.AddWithValue("@CedulaJuridica", beneficio.CedulaJuridica);

                _conexion.Open();
                var result = checkCommand.ExecuteScalar();
                _conexion.Close();

                if (result != null)
                    throw new Exception("Ya existe un beneficio con el mismo nombre para esta empresa.");
            }

            // Se hace la inserción del nuevo beneficio
            Guid beneficioId = Guid.NewGuid();
            string insertQuery = @"INSERT INTO Beneficio 
                (Id, Costo, TiempoMinimoEnEmpresa, Frecuencia, Descripcion, Nombre, CedulaJuridica, NombreDeAPI, EsAPI, EsPorcentual) 
                VALUES 
                (@Id, @Costo, @TiempoMinimo, @Frecuencia, @Descripcion, @Nombre, @CedulaJuridica, @NombreDeAPI, @EsApi, @EsPorcentual)";
            using (SqlCommand insertCommand = new SqlCommand(insertQuery, _conexion))
            {
                insertCommand.Parameters.AddWithValue("@Id", beneficioId);
                insertCommand.Parameters.AddWithValue("@Costo", beneficio.Costo);
                insertCommand.Parameters.AddWithValue("@TiempoMinimo", beneficio.TiempoMinimo);
                insertCommand.Parameters.AddWithValue("@Frecuencia", beneficio.Frecuencia);
                insertCommand.Parameters.AddWithValue("@Descripcion", beneficio.Descripcion);
                insertCommand.Parameters.AddWithValue("@Nombre", beneficio.Nombre);
                insertCommand.Parameters.AddWithValue("@CedulaJuridica", beneficio.CedulaJuridica);
                insertCommand.Parameters.AddWithValue("@NombreDeAPI", beneficio.NombreDeAPI);
                insertCommand.Parameters.AddWithValue("@EsApi", beneficio.EsApi);
                insertCommand.Parameters.AddWithValue("@EsPorcentual", beneficio.EsPorcentual);

                _conexion.Open();
                insertCommand.ExecuteNonQuery();
                _conexion.Close();
            }

            // Se insertan los contratos elegibles seleccionados
            if (beneficio.ContratosElegibles != null && beneficio.ContratosElegibles.Count > 0)
            {
                string contratoQuery = "INSERT INTO BeneficioContratoElegible (IdBeneficio, ContratoEmpleado) VALUES (@IdBeneficio, @ContratoEmpleado)";
                foreach (var contrato in beneficio.ContratosElegibles)
                {
                    using (SqlCommand contratoCommand = new SqlCommand(contratoQuery, _conexion))
                    {
                        contratoCommand.Parameters.AddWithValue("@IdBeneficio", beneficioId);
                        contratoCommand.Parameters.AddWithValue("@ContratoEmpleado", contrato);

                        _conexion.Open();
                        contratoCommand.ExecuteNonQuery();
                        _conexion.Close();
                    }
                }
            }
        }

        public void EditarBeneficio(Beneficio beneficio)
        {
            // Se verifica si el nombre del beneficio ya existe para esa empresa, excluyendo el propio beneficio
            string checkQuery = @"SELECT COUNT(*) FROM Beneficio WHERE Nombre = @Nombre AND CedulaJuridica = @CedulaJuridica AND Id <> @Id";
            using (SqlCommand checkCommand = new SqlCommand(checkQuery, _conexion))
            {
                checkCommand.Parameters.AddWithValue("@Nombre", beneficio.Nombre);
                checkCommand.Parameters.AddWithValue("@CedulaJuridica", beneficio.CedulaJuridica);
                checkCommand.Parameters.AddWithValue("@Id", beneficio.Id);

                _conexion.Open();
                int count = (int)checkCommand.ExecuteScalar();
                _conexion.Close();

                if (count > 0)
                    throw new Exception("Ya existe un beneficio con el mismo nombre para esta empresa.");
            }

            // Se actualiza el beneficio
            string updateQuery = @"UPDATE Beneficio
                SET Nombre = @Nombre,
                    Descripcion = @Descripcion,
                    Costo = @Costo,
                    TiempoMinimoEnEmpresa = @TiempoMinimo,
                    Frecuencia = @Frecuencia,
                    NombreDeAPI = @NombreDeAPI,
                    EsAPI = @EsApi,
                    EsPorcentual = @EsPorcentual
                WHERE Id = @Id";
            using (SqlCommand cmd = new SqlCommand(updateQuery, _conexion))
            {
                cmd.Parameters.AddWithValue("@Id", beneficio.Id);
                cmd.Parameters.AddWithValue("@Nombre", beneficio.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", beneficio.Descripcion);
                cmd.Parameters.AddWithValue("@Costo", beneficio.Costo);
                cmd.Parameters.AddWithValue("@TiempoMinimo", beneficio.TiempoMinimo);
                cmd.Parameters.AddWithValue("@Frecuencia", beneficio.Frecuencia);
                cmd.Parameters.AddWithValue("@NombreDeAPI", beneficio.NombreDeAPI);
                cmd.Parameters.AddWithValue("@EsApi", beneficio.EsApi);
                cmd.Parameters.AddWithValue("@EsPorcentual", beneficio.EsPorcentual);

                _conexion.Open();
                cmd.ExecuteNonQuery();
                _conexion.Close();
            }

            // Se eliminan los contratos elegibles antiguos
            string deleteQuery = "DELETE FROM BeneficioContratoElegible WHERE IdBeneficio = @IdBeneficio";
            using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, _conexion))
            {
                deleteCommand.Parameters.AddWithValue("@IdBeneficio", beneficio.Id);

                _conexion.Open();
                deleteCommand.ExecuteNonQuery();
                _conexion.Close();
            }

            // Se insertan contratos elegibles
            if (beneficio.ContratosElegibles != null && beneficio.ContratosElegibles.Count > 0)
            {
                string contratoQuery = "INSERT INTO BeneficioContratoElegible (IdBeneficio, ContratoEmpleado) VALUES (@IdBeneficio, @ContratoEmpleado)";
                foreach (var contrato in beneficio.ContratosElegibles)
                {
                    using SqlCommand contratoCommand = new SqlCommand(contratoQuery, _conexion);
                    contratoCommand.Parameters.AddWithValue("@IdBeneficio", beneficio.Id);
                    contratoCommand.Parameters.AddWithValue("@ContratoEmpleado", contrato);

                    _conexion.Open();
                    contratoCommand.ExecuteNonQuery();
                    _conexion.Close();
                }
            }
        }

        public Beneficio GetBeneficio(Guid id)
        {
            Beneficio? beneficio = null;

            string query = @"SELECT Id, Costo, TiempoMinimoEnEmpresa, Frecuencia, Descripcion, Nombre, CedulaJuridica, NombreDeAPI, EsAPI, EsPorcentual
                             FROM Beneficio WHERE Id = @Id";
            using (SqlCommand cmd = new SqlCommand(query, _conexion))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                DataTable tablaConsulta = CrearTablaConsulta(cmd);

                if (tablaConsulta.Rows.Count > 0)
                {
                    DataRow fila = tablaConsulta.Rows[0];
                    beneficio = new Beneficio
                    {
                        Id = fila["Id"].ToString(),
                        Costo = fila["Costo"] == DBNull.Value ? 0 : Convert.ToDecimal(fila["Costo"]),
                        TiempoMinimo = fila["TiempoMinimoEnEmpresa"] == DBNull.Value ? 0 : Convert.ToInt32(fila["TiempoMinimoEnEmpresa"]),
                        Frecuencia = fila["Frecuencia"]?.ToString(),
                        Descripcion = fila["Descripcion"]?.ToString(),
                        Nombre = fila["Nombre"]?.ToString(),
                        CedulaJuridica = fila["CedulaJuridica"]?.ToString(),
                        NombreDeAPI = fila["NombreDeAPI"]?.ToString(),
                        EsApi = fila["EsAPI"] == DBNull.Value ? false : Convert.ToBoolean(fila["EsAPI"]),
                        EsPorcentual = fila["EsPorcentual"] == DBNull.Value ? false : Convert.ToBoolean(fila["EsPorcentual"])
                    };
                }
            }
            return beneficio;
        }

        public List<object> GetCompanyBenefits(string CedulaJuridica)
        {
            List<object> beneficios = new List<object>();

            string query = @"
                SELECT b.Id, b.Nombre, b.Descripcion, b.Costo, b.TiempoMinimoEnEmpresa
                FROM Beneficio b
                INNER JOIN DuenoEmpresa de ON b.CedulaJuridica = de.CedulaEmpresa
                WHERE de.CedulaEmpresa = @CedulaJuridica;";

            using (var command = new SqlCommand(query, _conexion))
            {
                command.Parameters.AddWithValue("@CedulaJuridica", CedulaJuridica);
                DataTable tablaConsulta = CrearTablaConsulta(command);

                foreach (DataRow fila in tablaConsulta.Rows)
                {
                    beneficios.Add(new 
                    {
                        Id = fila["Id"].ToString(),
                        Nombre = fila["Nombre"]?.ToString(),
                        Descripcion = fila["Descripcion"]?.ToString(),
                        Costo = fila["Costo"] == DBNull.Value ? 0 : Convert.ToDecimal(fila["Costo"]),
                        TiempoMinimo = fila["TiempoMinimoEnEmpresa"] == DBNull.Value ? 0 : Convert.ToInt32(fila["TiempoMinimoEnEmpresa"])
                    });
                }
            }
            return beneficios;
        }

        public List<object> GetBenefitsEmployeeContract(string CedulaJuridica, string IdEmpleado)
        {
            List<object> beneficios = new List<object>();

            // Obtener el contrato del empleado
            string employeeQuery = "SELECT Contrato FROM Empleado WHERE Id = @IdEmpleado;";
            string? employeeContract = null;
            using (SqlCommand employeeCommand = new SqlCommand(employeeQuery, _conexion))
            {
                employeeCommand.Parameters.AddWithValue("@IdEmpleado", IdEmpleado);
                _conexion.Open();
                employeeContract = employeeCommand.ExecuteScalar()?.ToString();
                _conexion.Close();
            }

            if (string.IsNullOrWhiteSpace(employeeContract))
            {
                throw new Exception("Empleado no encontrado o sin contrato.");
            }

            // Obtener los beneficios asociados al contrato del empleado
            string query = @"
                SELECT b.Id, b.Nombre, b.Descripcion, b.Costo, b.TiempoMinimoEnEmpresa
                FROM Beneficio b
                INNER JOIN DuenoEmpresa de ON b.CedulaJuridica = de.CedulaEmpresa
                LEFT JOIN BeneficioContratoElegible bce ON b.Id = bce.IdBeneficio
                WHERE de.CedulaEmpresa = @CedulaJuridica
                AND (bce.IdBeneficio IS NULL OR bce.ContratoEmpleado = @EmployeeContract);";

            using (SqlCommand command = new SqlCommand(query, _conexion))
            {
                command.Parameters.AddWithValue("@CedulaJuridica", CedulaJuridica);
                command.Parameters.AddWithValue("@EmployeeContract", employeeContract);
                DataTable tablaConsulta = CrearTablaConsulta(command);

                foreach (DataRow fila in tablaConsulta.Rows)
                {
                    beneficios.Add(new 
                    {
                        Id = fila["Id"].ToString(),
                        Nombre = fila["Nombre"].ToString(),
                        Descripcion = fila["Descripcion"].ToString(),
                        Costo = Convert.ToDecimal(fila["Costo"]),
                        TiempoMinimo = Convert.ToInt32(fila["TiempoMinimoEnEmpresa"]),
                    });
                }
            }
            return beneficios;
        }

        public List<object> GetEmployeeBenefits(string IdEmpleado)
        {
            List<object> beneficios = new List<object>();

            string query = @"
                SELECT b.Nombre AS NombreBeneficio, b.Descripcion, b.Costo, b.Frecuencia
                FROM BeneficiosEmpleado be
                JOIN Beneficio b ON be.IdBeneficio = b.Id
                WHERE be.IdEmpleado = @IdEmpleado;";

            using (SqlCommand command = new SqlCommand(query, _conexion))
            {
                command.Parameters.AddWithValue("@IdEmpleado", IdEmpleado);
                DataTable tablaConsulta = CrearTablaConsulta(command);

                foreach (DataRow fila in tablaConsulta.Rows)
                {
                    beneficios.Add(new
                    {
                        Nombre = fila["NombreBeneficio"].ToString(),
                        Descripcion = fila["Descripcion"].ToString(),
                        Costo = Convert.ToDecimal(fila["Costo"]),
                        Frecuencia = fila["Frecuencia"].ToString(),
                    });
                }
            }
            return beneficios;
        }
    }
}