using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using dominio1;

namespace negocio1
{
    public class PokemonNegocio
    {
        public List<Pokemon> lista()
        {

            //Instanciamos los objetos y declaramos la variable que necesitamos
            List<Pokemon> lista = new List<Pokemon>();  //Instanciamos la Lista de Pokemons
            SqlConnection conexion = new SqlConnection(); //Instanciamos la conexion
            SqlCommand comando = new SqlCommand(); //Instanciamos los comandos
            SqlDataReader lector; //Declaramos la variable lector, en este momento no sería posible instanciarlo.

            try
            {
                //le pasamos la conexión a la BD.
                conexion.ConnectionString = "server=DESKTOP-1RDJ28S\\SQLEXPRESS; database=POKEMON_DB; integrated security=true";
                //Definimos el tipo de comando
                comando.CommandType = System.Data.CommandType.Text;
                //Le inyectamos la instruccion SQL al comando
                comando.CommandText = "select Numero, Nombre, P.Descripcion, UrlImagen, E.Descripcion Tipo, D.Descripcion Debilidad, P.IdTipo, P.IdDebilidad, P.Id from POKEMONS P, ELEMENTOS E, ELEMENTOS D where E.Id = P.IdTipo AND D.Id = P.IdDebilidad AND P.Activo = 1"; //es recomendable, copiar y pegar la instrucción desde SQL. Ya que es común los errores en principiantes.
                                                                                          //Le pasamos la direccion de en que DB conectarse para realizar la acción-consulta
                comando.Connection = conexion;

                conexion.Open();
                lector = comando.ExecuteReader(); // Aquí se realiza la lectura de la consulta-instruccion
                                                  //En la linea 32, se crea una especie de tabla con los datos que le consultamos a la DB. Y tiene un puntero, pero no apunta a ningun lado.

                while (lector.Read()) // Esta condición hace 2 cosas, primero devuelve un bool (t o f). Y luego, situa el puntero en toda la primera linea de nuestra tabla generada arriba. 
                {
                    Pokemon aux = new Pokemon();//En cada vuelta va a crear una nueva instancia de un Pokemon.
                    aux.Id = (int)lector["Id"];
                    aux.Numero = lector.GetInt32(0); //Dos formas. Esta primera, teniendo en cuenta la posicion de la columna
                    aux.Nombre = (string)lector["Nombre"]; //2da forma. Le pasamos la referencia por nombre de columna.
                    aux.Descripcion = (string)lector["Descripcion"];

                    if (!(lector["UrlImagen"] is DBNull))
                    {
                        aux.UrlImagen = (string)lector["UrlImagen"];
                    }
                    
                    aux.Tipo = new Elemento(); //Esta propiedad guarda un objeto, por eso debemos instanciarlo primero.
                    aux.Tipo.Descripcion = (string)lector["Tipo"];//Una vez instanciado el Objeto, procedemos a guardar el valor de su propiedad descripción.
                    aux.Tipo.Id = (int)lector["IdTipo"];
                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Descripcion = (string)lector["Debilidad"];
                    aux.Debilidad.Id = (int)lector["IdDebilidad"];


                    lista.Add(aux);
                }
                conexion.Close(); //Cerramos la conexion una vez recorrido todas las filas. Aunque hay otra manera de hacero. Nos enseñara más adelante.

                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Agregar(Pokemon newP)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setQuery($"insert into POKEMONS(Numero, Nombre, Descripcion, Activo, IdTipo, IdDebilidad, UrlImagen) values ({newP.Numero},'{newP.Nombre}','{newP.Descripcion}', 1, @idTipo, {newP.Debilidad.Id}, @UrlImagen)");
                datos.setParametro("@idTipo", newP.Tipo.Id);
                datos.setParametro("@UrlImagen", newP.UrlImagen);
                //datos.setParametro("@idDebilidad", newP.Debilidad.Id);

                datos.ejecutarInstruccion();
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
        public void Modificar(Pokemon pokemon) 
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setQuery("update POKEMONS set Numero = @numero, Nombre = @nombre, Descripcion = @descripcion, UrlImagen = @url,IdTipo = @idTipo, IdDebilidad =@idDebilidad where Id = @idPokemon");
                datos.setParametro("@numero", pokemon.Numero);
                datos.setParametro("@nombre",pokemon.Nombre);
                datos.setParametro("@descripcion",pokemon.Descripcion);
                datos.setParametro("@url",pokemon.UrlImagen);
                datos.setParametro("@idTipo", pokemon.Tipo.Id);
                datos.setParametro("@idDebilidad", pokemon.Debilidad.Id);
                datos.setParametro("@idPokemon", pokemon.Id);

                datos.ejecutarInstruccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally { datos.cerrarConexion();}
        }
        public void Eliminar(int Id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setQuery("delete from POKEMONS where Id = @Id");
                datos.setParametro("@Id", Id);
                datos.ejecutarInstruccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally { datos.cerrarConexion();}
        }
        public void EliminarLogico(int Id)
        {
            AccesoDatos datos=new AccesoDatos();
            try
            {
                datos.setQuery("update POKEMONS set Activo = 0 where Id = @id");
                datos.setParametro("@id", Id);
                datos.ejecutarInstruccion();
            
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally { datos.cerrarConexion();}
        }

        public List<Pokemon> filtrar(string campo, string criterio, string texto)
        {
            List<Pokemon> lista = new List<Pokemon>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                string consulta = "select Numero, Nombre, P.Descripcion, UrlImagen, E.Descripcion Tipo, D.Descripcion Debilidad, P.IdTipo, P.IdDebilidad, P.Id from POKEMONS P, ELEMENTOS E, ELEMENTOS D where E.Id = P.IdTipo AND D.Id = P.IdDebilidad AND P.Activo = 1 AND ";

                if (campo == "Número")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += $"Numero > {texto}";
                            break;
                        case "Menor a":
                            consulta += $"Numero < {texto}";
                            break;
                        default:
                            consulta += $"Numero = {texto}";
                            break;
                    }
                }
                else if (campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += $"Nombre LIKE '{texto}%'";
                            break;
                        case "Termina con":
                            consulta += $"Nombre LIKE '%{texto}'";
                            break;
                        default:
                            consulta += $"Nombre LIKE '%{texto}%'";
                            break;
                    }
                }
                else
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += $"E.Descripcion LIKE '{texto}%'";
                            break;
                        case "Termina con":
                            consulta += $"E.Descripcion LIKE '%{texto}'";
                            break;
                        default:
                            consulta += $"E.Descripcion LIKE '%{texto}%'";
                            break;
                    }
                }
                datos.setQuery(consulta);
                datos.ejecutarLector();

                while (datos.Lector.Read()) 
                {
                    Pokemon aux = new Pokemon();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Numero = datos.Lector.GetInt32(0); 
                    aux.Nombre = (string)datos.Lector["Nombre"]; 
                    aux.Descripcion = (string)datos.Lector["Descripcion"];

                    if (!(datos.Lector["UrlImagen"] is DBNull))
                    {
                        aux.UrlImagen = (string)datos.Lector["UrlImagen"];
                    }

                    aux.Tipo = new Elemento(); 
                    aux.Tipo.Descripcion = (string)datos.Lector["Tipo"];
                    aux.Tipo.Id = (int)datos.Lector["IdTipo"];
                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Descripcion = (string)datos.Lector["Debilidad"];
                    aux.Debilidad.Id = (int)datos.Lector["IdDebilidad"];


                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
