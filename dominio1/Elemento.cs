using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dominio1
{
    public class Elemento
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }

        public override string ToString() //Para solucionar el problema que devuelva o escriba 
                                          //la definicion namespace.NombreClase en el "dgvPokemon"
                                          //debemos sobrecargar este método que se auto aplica
                                          //para devolver el objeto transformado en string.
        {
            return Descripcion;//De esta manera, devuelve el valor guardado en la Propiedad Descripcion.
        }
    }
}
