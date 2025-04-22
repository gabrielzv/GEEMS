using backend_lab.Models;
using System.Data;
using Microsoft.Data.SqlClient;
namespace backend_lab.Handlers
{
    public class PaisesHandler
    {
        private SqlConnection _conexion;
        private string _rutaConexion;
        public PaisesHandler()
        {
            var builder = WebApplication.CreateBuilder();
            _rutaConexion =
            builder.Configuration.GetConnectionString("PaisesContext");
            _conexion = new SqlConnection(_rutaConexion);
        }
        private DataTable CrearTablaConsulta(string consulta)
        {
            SqlCommand comandoParaConsulta = new
            SqlCommand(consulta, _conexion);
            SqlDataAdapter adaptadorParaTabla = new
            SqlDataAdapter(comandoParaConsulta);
            DataTable consultaFormatoTabla = new DataTable();
            _conexion.Open();
            adaptadorParaTabla.Fill(consultaFormatoTabla);
            _conexion.Close();
            return consultaFormatoTabla;
        }
        public List<PaisModel> ObtenerPaises()
        {
            List<PaisModel> paises = new List<PaisModel>();
            string consulta = "SELECT * FROM dbo.Pais ";
            DataTable tablaResultado =
            CrearTablaConsulta(consulta);
            foreach (DataRow columna in tablaResultado.Rows)
            {
                paises.Add(
                new PaisModel
                {
                    Id = Convert.ToInt32(columna["Id"]),
                    Nombre = Convert.ToString(columna["Nombre"]),
                    Idioma = Convert.ToString(columna["Idioma"]),
                    Continente =
                Convert.ToString(columna["Continente"]),
                });
            }
            return paises;
        }
    }
}