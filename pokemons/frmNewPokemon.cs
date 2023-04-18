using dominio1;
using negocio1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace pokemons
{
    public partial class frmNewPokemon : Form
    {
        private Pokemon pokemon = null;
        //Declaramos un atributo o un pokemon, para poder guardar aquí el Pokemon
        //que recibimos por parametros del constructor y acceder a su información (estado).
        private OpenFileDialog archivo= null;
        public frmNewPokemon()
        {
            InitializeComponent();
        }
        public frmNewPokemon(Pokemon pokemon)
        {
            InitializeComponent();
            this.pokemon = pokemon;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            //Dos formas de poner el close()
            Close();
            //this.Close(); 
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            //Pokemon newPokemon = new Pokemon();
            PokemonNegocio negocio = new PokemonNegocio();

            try
            {
                if (pokemon == null)
                {
                    pokemon = new Pokemon();
                }
                pokemon.Nombre = txtNombre.Text;
                pokemon.Descripcion = txtDescripcion.Text;
                pokemon.UrlImagen = txtUrl.Text;
                pokemon.Tipo = (Elemento)cBxTipo.SelectedItem;
                pokemon.Debilidad = (Elemento)cBxDebilidad.SelectedItem;
                pokemon.Numero = int.Parse(txtNumero.Text);

                if (pokemon.Id == 0)
                {
                    negocio.Agregar(pokemon);
                    MessageBox.Show("Agregado.");
                }
                else
                {
                    negocio.Modificar(pokemon);
                    MessageBox.Show("Modificado.");
                }
                if(archivo != null && !(txtUrl.Text.Contains("http"))) //Cambia el valor de null a una instancia cuando hacemos clic en el botón agregarImagen.
                                                                       //Y con la segunda condición nos aseguramos que el usuario realmente quiere guardar una imagen local.
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-file"] + archivo.SafeFileName);

                Close();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void frmNewPokemon_Load(object sender, EventArgs e)
        {
            ElementoNegocio elementoNegocio = new ElementoNegocio();
            try
            {
                //Aquí les pasamos a los combo box las listas que deben mostrar
                cBxTipo.DataSource= elementoNegocio.listar();
                //Para acceder a un valor preseleccionado debemos definir una clave y su valor.
                cBxTipo.ValueMember = "Id";
                cBxTipo.DisplayMember = "Descripcion";
                cBxDebilidad.DataSource = elementoNegocio.listar();
                cBxDebilidad.ValueMember = "Id";
                cBxDebilidad.DisplayMember = "Descripcion";

                //Validar si el pokemon sigue o no en Null.
                if (pokemon != null)
                { //Es distinto de null, por ende el usuario hizo clic en Modificar
                  //y debe aparecer la información del pokémon.

                    txtNumero.Text = pokemon.Numero.ToString();
                    txtNombre.Text = pokemon.Nombre;
                    txtDescripcion.Text = pokemon.Descripcion;
                    txtUrl.Text = pokemon.UrlImagen;
                    cargarImagen(pokemon.UrlImagen);
                    cBxTipo.SelectedValue = pokemon.Tipo.Id;
                    cBxDebilidad.SelectedValue = pokemon.Debilidad.Id;

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void txtUrl_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtUrl.Text);
        }
        private void cargarImagen(string Url)
        {
            try
            {
                pBxPokemon.Load(Url);
            }
            catch (Exception ex)
            {
                pBxPokemon.Load("https://tse2.mm.bing.net/th?id=OIP.2KdMLsskO-By1dqK2epgegHaHa&pid=Api&P=0");
            }
        }

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg |png|*.png";
            if (archivo.ShowDialog() == DialogResult.OK) //Abre la ventana, y verifica si se hizo clic en el OK.
            {
                txtUrl.Text = archivo.FileName;
                cargarImagen(archivo.FileName);

                //Cómo guardar la imagen
                //File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-file"] + archivo.SafeFileName);
            }
        }
    }
}
