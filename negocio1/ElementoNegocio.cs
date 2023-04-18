using dominio1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace negocio1
{
    public class ElementoNegocio
    {
        public List<Elemento> listar()
        {
            List<Elemento> lista = new List<Elemento>();
            AccesoDatos datos = new AccesoDatos();
            
            try
			{
                datos.setQuery("select id, Descripcion from ELEMENTOS");
                datos.ejecutarLector();

                while (datos.Lector.Read())
                {
                    Elemento elemento = new Elemento();
                    elemento.Id = (int)datos.Lector["id"];
                    elemento.Descripcion = (string)datos.Lector["Descripcion"];

                    lista.Add(elemento);
                }

                return lista;
            }
			catch (Exception ex)
			{

				throw ex;
			}
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
