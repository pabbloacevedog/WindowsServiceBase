using System.Data.SqlClient;
using WindowsServiceBase.Sistema;

namespace WindowsServiceBase.Modelo
{
    public class ConexionBD
    {
        public static SqlConnection ObtenerConexionBD()
        {
            string Cadena = "DATA source=" + CONFIG.DB_SERVER + ";Initial Catalog=" + CONFIG.DB_NAME + ";User Id=" + CONFIG.DB_USER + ";Password=" + CONFIG.DB_PASS;
            SqlConnection Conn = null;
            try
            {
                Conn = new SqlConnection(Cadena);
                Conn.Open();
            }
            catch (SqlException error)
            {
                ControlExcepciones.SQLException("ObtenerConexionBD - No se ha podido establecer conexión con el servidor de Base de Datos", error);
            }
            return Conn;
        }
    }
}
