using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio1;
using negocio1;

namespace pokemons
{
    public partial class Form1 : Form
    {
        //Atributo Privado creado al principio de la construción de la app
        private List<Pokemon> listaPokemon;
        public Form1()
        {
            InitializeComponent();
        }
    
        private void Form1_Load(object sender, EventArgs e)
        {
            Cargar();
            cBxCampo.Items.Add("Número");
            cBxCampo.Items.Add("Nombre");
            cBxCampo.Items.Add("Tipo");
        }

        private void Cargar()
        {
            PokemonNegocio negocio = new PokemonNegocio();
            listaPokemon = negocio.lista();
            dgvPokemons.DataSource = listaPokemon;
            dgvPokemons.Columns["UrlImagen"].Visible = false; //Para hacer que una columna no aparezca al ejecutar la app
            dgvPokemons.Columns["Id"].Visible = false;
            //cargarImagen(listaPokemon[0].UrlImagen);
            //dgvPokemons.DataGridViewColum.Name;
        }

        private void dgvPokemons_SelectionChanged(object sender, EventArgs e)
        {
            Pokemon seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;
            cargarImagen(seleccionado.UrlImagen);
        }
        
        private void cargarImagen(string Url)
        {
            try
            {
                pbxPokemon.Load(Url);
            }
            catch (Exception ex)
            {
                pbxPokemon.Load("https://tse2.mm.bing.net/th?id=OIP.2KdMLsskO-By1dqK2epgegHaHa&pid=Api&P=0");
            }
        }

        private void btnNewPokemon_Click(object sender, EventArgs e)
        {
            frmNewPokemon newPokemon = new frmNewPokemon();
            newPokemon.ShowDialog();
            Cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            //Aca le pasamos por parametro el objeto que queremos modificar.
            if (dgvPokemons.CurrentRow != null)
            {
                Pokemon seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;
                frmNewPokemon modificar = new frmNewPokemon(seleccionado);
                modificar.ShowDialog();
                Cargar();
            }
            else
            {
                MessageBox.Show("Ups selecciona un Pokémon antes de modificarlo.");
            }
            
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Eliminar();//La bandera "logico" no cambia y se ejecuta el else. 
        }

        private void btnEliminarLogico_Click(object sender, EventArgs e)
        {
            Eliminar(true); //La bandera "logico" cambia y valida que se ha hecho clic en el botón eliminar logico. 
        }

        private void Eliminar(bool logico = false)
        {
            PokemonNegocio negocio = new PokemonNegocio();

            try
            {
                Pokemon seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;
                DialogResult resultado = MessageBox.Show($"¿Deseas eliminar a {seleccionado.Nombre}?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (resultado == DialogResult.Yes)
                {
                    if (logico)

                        negocio.EliminarLogico(seleccionado.Id);

                    else

                        negocio.Eliminar(seleccionado.Id);

                    Cargar();
                    MessageBox.Show($"Se ha eliminado a {seleccionado.Nombre}"); //Puede que de un error(xq no encuentra el pokemon) o que no se muestre el nombre.
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (ValidarFiltro())
                return; //No retorna nada, se pone así por si da true a qué no hay ningún item seleccionado.

            PokemonNegocio filtroAvanzado = new PokemonNegocio();
            string campo = cBxCampo.SelectedItem.ToString();
            string criterio = cBxCriterio.SelectedItem.ToString();
            string txtFiltro = txtFiltroAvanzado.Text;
            List<Pokemon> listafiltradoAvanzado = filtroAvanzado.filtrar(campo, criterio, txtFiltro);
            dgvPokemons.DataSource = null;
            dgvPokemons.DataSource = listafiltradoAvanzado;
        }
        private bool ValidarFiltro()
        {
            if (cBxCampo.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un Campo para filtrar.");
                return true;
            }
            if (cBxCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione un Criterio para filtrar.");
                return true;
            }
            if (cBxCampo.SelectedItem.ToString() == "Número")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Por favor ingrese un número.");
                    return true;
                }
                if (!(soloNumeros(txtFiltroAvanzado.Text)))
                {
                    MessageBox.Show("Por favor, ingrese solo números");
                    return true;
                }
                
            }

            return false;

        }
        private bool soloNumeros(string cadena)
        {
            foreach (char item in cadena)
            {
                if (!(char.IsNumber(item)))
                {
                    return false;
                }

            }
            return true;
        }
        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            List<Pokemon> listaFiltrada = new List<Pokemon>();


            string filtro = txtFiltro.Text;
            if (filtro != "")
            {
                //listaFiltrada = listaPokemon.FindAll(x => x.Nombre == txtFiltro.Text);
                listaFiltrada = listaPokemon.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.Tipo.Descripcion.ToUpper().Contains(filtro.ToUpper()) || x.Id.ToString()== filtro);
            }
            else
            {
                listaFiltrada = listaPokemon;
            }

            dgvPokemons.DataSource = null;
            dgvPokemons.DataSource = listaFiltrada;
            dgvPokemons.Columns["UrlImagen"].Visible = false;
        }

        private void cBxCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string campoSeleccionado = cBxCampo.SelectedItem.ToString();
            cBxCriterio.Items.Clear();
            if (campoSeleccionado == "Número")
            {
                cBxCriterio.Items.Add("Mayor a");
                cBxCriterio.Items.Add("Menor a");
                cBxCriterio.Items.Add("Igual a");
            }
            else
            {
                cBxCriterio.Items.Add("Comienza con");
                cBxCriterio.Items.Add("Termina con");
                cBxCriterio.Items.Add("Contiene");
            }
        }
    }
}
