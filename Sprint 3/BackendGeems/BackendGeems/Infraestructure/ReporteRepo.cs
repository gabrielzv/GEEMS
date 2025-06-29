using BackendGeems.Application;
using BackendGeems.Controllers;
using BackendGeems.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace BackendGeems.Infraestructure
{
    public class ReporteRepo : IReporteRepo
    {
        private readonly string _cadenaConexion;
        public ReporteRepo()
        {
            var builder = WebApplication.CreateBuilder();
            _cadenaConexion = builder.Configuration.GetConnectionString("DefaultConnection");
        }

        public List<SalarioPorContratoDto> ObtenerSalariosPorContrato(Guid idPlanilla)
        {
            var lista = new List<SalarioPorContratoDto>();
            string query = @"
                SELECT 
                    e.Contrato AS Contrato, 
                    SUM(p.MontoBruto) AS TotalSalario
                FROM Pago p
                INNER JOIN Empleado e ON p.IdEmpleado = e.Id
                WHERE p.IdPlanilla = @IdPlanilla
                GROUP BY e.Contrato";

            using (var conn = new SqlConnection(_cadenaConexion))
            {
                var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdPlanilla", idPlanilla);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new SalarioPorContratoDto
                        {
                            TipoContrato = reader["Contrato"].ToString(),
                            TotalSalario = reader["TotalSalario"] != DBNull.Value ? Convert.ToDouble(reader["TotalSalario"]) : 0
                        });
                    }
                }
            }
            return lista;
        }

        public List<DeduccionResumenDto> ObtenerDeduccionesPorPlanilla(Guid idPlanilla)
        {
            var lista = new List<DeduccionResumenDto>();

            // Obtener los pagos de la planilla
            var pagos = new List<Guid>();
            string pagosQuery = "SELECT Id FROM Pago WHERE IdPlanilla = @IdPlanilla";
            using (var conn = new SqlConnection(_cadenaConexion))
            {
                var cmd = new SqlCommand(pagosQuery, conn);
                cmd.Parameters.AddWithValue("@IdPlanilla", idPlanilla);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pagos.Add((Guid)reader["Id"]);
                    }
                }
            }

            if (pagos.Count == 0)
                return lista;

            // Obtener deducciones de esos pagos
            string deduccionesQuery = $@"
                SELECT 
                    d.Nombre,
                    d.IdBeneficio,
                    SUM(d.Monto) AS Total
                FROM Deducciones d
                WHERE d.IdPago IN ({string.Join(",", pagos.Select((_, i) => $"@p{i}"))})
                GROUP BY d.Nombre, d.IdBeneficio";

            using (var conn = new SqlConnection(_cadenaConexion))
            {
                var cmd = new SqlCommand(deduccionesQuery, conn);
                for (int i = 0; i < pagos.Count; i++)
                    cmd.Parameters.AddWithValue($"@p{i}", pagos[i]);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new DeduccionResumenDto
                        {
                            Nombre = reader["Nombre"].ToString(),
                            Total = reader["Total"] != DBNull.Value ? Convert.ToDouble(reader["Total"]) : 0,
                            EsBeneficio = reader["IdBeneficio"] != DBNull.Value && reader["IdBeneficio"] != null
                        });
                    }
                }
            }
            return lista;
        }
    }
}