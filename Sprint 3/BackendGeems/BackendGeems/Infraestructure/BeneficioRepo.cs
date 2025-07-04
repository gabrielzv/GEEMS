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
        private readonly CorreoSender _correoSender;

        public BeneficioRepo()
        {
            var builder = WebApplication.CreateBuilder();
            _cadenaConexion = builder.Configuration.GetConnectionString("DefaultConnection");
            _conexion = new SqlConnection(_cadenaConexion);
            _correoSender = new CorreoSender(builder.Configuration);
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
            using (SqlConnection conn = new SqlConnection(_cadenaConexion))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Se verifica si el nombre del beneficio ya existe para esa empresa
                        string checkQuery = "SELECT Id FROM Beneficio WHERE Nombre = @Nombre AND CedulaJuridica = @CedulaJuridica";
                        using (SqlCommand checkCommand = new SqlCommand(checkQuery, conn, transaction))
                        {
                            checkCommand.Parameters.AddWithValue("@Nombre", beneficio.Nombre);
                            checkCommand.Parameters.AddWithValue("@CedulaJuridica", beneficio.CedulaJuridica);
                            var result = checkCommand.ExecuteScalar();
                            if (result != null)
                                throw new Exception("Ya existe un beneficio con el mismo nombre para esta empresa.");
                        }

                        // Se hace la inserción del nuevo beneficio
                        Guid beneficioId = Guid.NewGuid();
                        string insertQuery = @"INSERT INTO Beneficio 
                            (Id, Costo, TiempoMinimoEnEmpresa, Frecuencia, Descripcion, Nombre, CedulaJuridica, NombreDeAPI, EsAPI, EsPorcentual, Estado, EstaBorrado) 
                            VALUES 
                            (@Id, @Costo, @TiempoMinimo, @Frecuencia, @Descripcion, @Nombre, @CedulaJuridica, @NombreDeAPI, @EsApi, @EsPorcentual, @Estado, @EstaBorrado)";
                        using (SqlCommand insertCommand = new SqlCommand(insertQuery, conn, transaction))
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
                            insertCommand.Parameters.AddWithValue("@Estado", beneficio.Estado);
                            insertCommand.Parameters.AddWithValue("@EstaBorrado", beneficio.EstaBorrado);
                            insertCommand.ExecuteNonQuery();
                        }

                        // Se insertan los contratos elegibles seleccionados
                        if (beneficio.ContratosElegibles != null && beneficio.ContratosElegibles.Count > 0)
                        {
                            string contratoQuery = "INSERT INTO BeneficioContratoElegible (IdBeneficio, ContratoEmpleado) VALUES (@IdBeneficio, @ContratoEmpleado)";
                            foreach (var contrato in beneficio.ContratosElegibles)
                            {
                                using (SqlCommand contratoCommand = new SqlCommand(contratoQuery, conn, transaction))
                                {
                                    contratoCommand.Parameters.AddWithValue("@IdBeneficio", beneficioId);
                                    contratoCommand.Parameters.AddWithValue("@ContratoEmpleado", contrato);
                                    contratoCommand.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void EditarBeneficio(Beneficio beneficio)
        {
            using (SqlConnection conn = new SqlConnection(_cadenaConexion))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Se verifica si el nombre del beneficio ya existe para esa empresa, excluyendo el propio beneficio
                        string checkQuery = @"SELECT COUNT(*) FROM Beneficio WHERE Nombre = @Nombre AND CedulaJuridica = @CedulaJuridica AND Id <> @Id";
                        using (SqlCommand checkCommand = new SqlCommand(checkQuery, conn, transaction))
                        {
                            checkCommand.Parameters.AddWithValue("@Nombre", beneficio.Nombre);
                            checkCommand.Parameters.AddWithValue("@CedulaJuridica", beneficio.CedulaJuridica);
                            checkCommand.Parameters.AddWithValue("@Id", beneficio.Id);

                            int count = (int)checkCommand.ExecuteScalar();
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
                                EsPorcentual = @EsPorcentual,
                                Estado = @Estado,
                                EstaBorrado = @EstaBorrado
                            WHERE Id = @Id";
                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
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
                            cmd.Parameters.AddWithValue("@Estado", beneficio.Estado);
                            cmd.Parameters.AddWithValue("@EstaBorrado", beneficio.EstaBorrado);

                            cmd.ExecuteNonQuery();
                        }

                        // Se eliminan los contratos elegibles antiguos
                        string deleteQuery = "DELETE FROM BeneficioContratoElegible WHERE IdBeneficio = @IdBeneficio";
                        using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, conn, transaction))
                        {
                            deleteCommand.Parameters.AddWithValue("@IdBeneficio", beneficio.Id);
                            deleteCommand.ExecuteNonQuery();
                        }

                        // Se insertan contratos elegibles
                        if (beneficio.ContratosElegibles != null && beneficio.ContratosElegibles.Count > 0)
                        {
                            string contratoQuery = "INSERT INTO BeneficioContratoElegible (IdBeneficio, ContratoEmpleado) VALUES (@IdBeneficio, @ContratoEmpleado)";
                            foreach (var contrato in beneficio.ContratosElegibles)
                            {
                                using (SqlCommand contratoCommand = new SqlCommand(contratoQuery, conn, transaction))
                                {
                                    contratoCommand.Parameters.AddWithValue("@IdBeneficio", beneficio.Id);
                                    contratoCommand.Parameters.AddWithValue("@ContratoEmpleado", contrato);
                                    contratoCommand.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public Beneficio GetBeneficio(Guid id)
        {
            Beneficio? beneficio = null;

            string query = @"SELECT Id, Costo, TiempoMinimoEnEmpresa, Frecuencia, Descripcion, Nombre, CedulaJuridica, NombreDeAPI, EsAPI, EsPorcentual, Estado, EstaBorrado
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
                        EsPorcentual = fila["EsPorcentual"] == DBNull.Value ? false : Convert.ToBoolean(fila["EsPorcentual"]),
                        Estado = fila["Estado"]?.ToString(),
                        EstaBorrado = fila["EstaBorrado"] == DBNull.Value ? false : Convert.ToBoolean(fila["EstaBorrado"])
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
                WHERE de.CedulaEmpresa = @CedulaJuridica
                AND b.Estado = 'Activo';";

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

        public void MatricularBeneficio(BeneficiosEmpleado beneficioEmpleado)
        {
            if (beneficioEmpleado == null ||
            string.IsNullOrWhiteSpace(beneficioEmpleado.IdEmpleado) ||
            string.IsNullOrWhiteSpace(beneficioEmpleado.IdBeneficio))
            {
                throw new Exception("Los datos del beneficio y el empleado son obligatorios.");
            }

            string query = @"
                    INSERT INTO BeneficiosEmpleado (IdEmpleado, IdBeneficio)
                    VALUES (@IdEmpleado, @IdBeneficio);";
            using (var command = new SqlCommand(query, _conexion))
            {
                command.Parameters.AddWithValue("@IdEmpleado", beneficioEmpleado.IdEmpleado);
                command.Parameters.AddWithValue("@IdBeneficio", beneficioEmpleado.IdBeneficio);

                _conexion.Open();
                command.ExecuteNonQuery();
                _conexion.Close();
            }
        }

        public void EliminarBeneficio(string IdBeneficio)
        {
            using (SqlConnection conn = new SqlConnection(_cadenaConexion))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        bool tieneEmpleados;
                        string existeMatriculaQuery = @"SELECT COUNT(*) FROM BeneficiosEmpleado WHERE IdBeneficio = @id";
                        using (SqlCommand command = new SqlCommand(existeMatriculaQuery, conn, transaction))
                        {
                            command.Parameters.AddWithValue("@id", IdBeneficio);
                            tieneEmpleados = (int)command.ExecuteScalar() > 0;
                        }

                        if (!tieneEmpleados)
                        {
                            // Se hace el caso 1, se elimina el beneficio
                            RealizarEliminacion(IdBeneficio, conn, transaction);
                            transaction.Commit();
                            return;
                        }

                        bool planillaPagada;
                        string pagosRelacionadosQuery = @"
                            SELECT COUNT(*)
                            FROM Pago p
                            JOIN BeneficiosEmpleado be ON p.IdEmpleado = be.IdEmpleado
                            WHERE be.IdBeneficio = @idBeneficio";
                        using (SqlCommand command = new SqlCommand(pagosRelacionadosQuery, conn, transaction))
                        {
                            command.Parameters.AddWithValue("@idBeneficio", IdBeneficio);
                            planillaPagada = (int)command.ExecuteScalar() > 0;
                        }

                        if (!planillaPagada)
                        {
                            // Se hace el caso 2, se elimina el beneficio, las asociaciones y se notifica al empleado
                            NotificarEmpleados(IdBeneficio);
                            RealizarEliminacion(IdBeneficio, conn, transaction);
                        }
                        else
                        {
                            // Se hace el caso 3, se hace el borrado lógico del beneficio y se notifica al empleado
                            NotificarEmpleados(IdBeneficio);
                            BorradoLogico(IdBeneficio, conn, transaction);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Funciones privadas para realizar validaciones de las restricciones de los requerimientos del borrado de beneficios, se hacen dentro de la transacción
        private void RealizarEliminacion(string id, SqlConnection conn, SqlTransaction transaction)
        {
            string query = @"
                DELETE
                FROM BeneficiosEmpleado
                WHERE IdBeneficio = @id;

                DELETE
                FROM BeneficioContratoElegible
                WHERE IdBeneficio = @id;

                DELETE
                FROM Beneficio
                WHERE Id = @id;";
            using (SqlCommand command = new SqlCommand(query, conn, transaction))
            {
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }

        private void BorradoLogico(string id, SqlConnection conn, SqlTransaction transaction)
        {
            string update = @"
                UPDATE Beneficio
                SET Estado = 'PendienteBorrado',
                    EstaBorrado = 1
                WHERE Id = @id;

                DELETE
                FROM BeneficiosEmpleado
                WHERE IdBeneficio = @id;";
            using (SqlCommand command = new SqlCommand(update, conn, transaction))
            {
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }
        
        private void NotificarEmpleados(string IdBeneficio)
        {
            // Se obtiene primero el nombre del beneficio.
            string nombreBeneficio = "";
            string queryNombre = "SELECT Nombre FROM Beneficio WHERE Id = @IdBeneficio";
            using (SqlCommand cmdNombre = new SqlCommand(queryNombre, _conexion))
            {
                cmdNombre.Parameters.AddWithValue("@IdBeneficio", IdBeneficio);
                _conexion.Open();
                var result = cmdNombre.ExecuteScalar();
                if (result != null)
                    nombreBeneficio = result.ToString();
                _conexion.Close();
            }

            string asunto = "Notificación de beneficio eliminado";
            string mensaje = $"El beneficio \"{nombreBeneficio}\" ha sido eliminado.";

            // Se obtienen los correos de los empleados que matricularon ese beneficio.
            string query = @"
                SELECT u.CorreoPersona
                FROM BeneficiosEmpleado be
                JOIN Empleado e ON be.IdEmpleado = e.Id
                JOIN Usuario u ON e.CedulaPersona = u.CedulaPersona
                WHERE be.IdBeneficio = @IdBeneficio;";

            using (SqlCommand command = new SqlCommand(query, _conexion))
            {
                command.Parameters.AddWithValue("@IdBeneficio", IdBeneficio);
                _conexion.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string email = reader["CorreoPersona"].ToString();
                    if (!string.IsNullOrEmpty(email))
                    {
                        _correoSender.EnviarCorreoAsync(email, asunto, mensaje);
                    }
                }
                _conexion.Close();
            }
        }
    }
}