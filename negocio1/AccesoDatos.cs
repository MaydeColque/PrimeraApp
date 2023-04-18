using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace negocio1
{
    public class AccesoDatos
    {
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;
        public SqlDataReader Lector
        {
            get{ return lector; }
        }
        public AccesoDatos()
        {
            conexion = new SqlConnection("server=.\\SQLEXPRESS; database=POKEMON_DB; integrated security=true");
            comando = new SqlCommand();
        }
        public void setQuery(string consulta)
        {
            comando.CommandType= System.Data.CommandType.Text;
            comando.CommandText= consulta;
        }
        public void setParametro(string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor);
        }
        public void ejecutarLector()
        {

            comando.Connection= conexion;
            try
            {
                conexion.Open();
                lector = comando.ExecuteReader();//Con esto obtenemos el lector?
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void ejecutarInstruccion()
        {
            comando.Connection= conexion;
            try
            {
                conexion.Open();
                comando.ExecuteNonQuery();//Que hace esto?   
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void cerrarConexion()
        {
            //Si hay un lector abrierto también lo cierra
            if (lector != null)
            {
                lector.Close();
            }
            conexion.Close();
        }

        
    }
}
