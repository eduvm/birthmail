using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

using Cliente.Helpers;

namespace Cliente {

    /// <summary>
    /// Interaction logic for WinConfig.xaml
    /// </summary>
    public partial class WinConfig : Window {

        #region Construtores

        public WinConfig() {

            InitializeComponent();

            CarregaDados();

        }

        #endregion Construtores

        #region Botões

        private void titleBar_MouseDown(object sender, MouseButtonEventArgs e) {

            this.DragMove();

        }

        private void btnFechar_Click(object sender, RoutedEventArgs e) {

            this.Close();

        }

        #endregion Botões

        private void CarregaDados() {

            // Limpa dataGrid
            dgridTipos.ItemsSource = null;

            // Tenta
            try {
                // Gera novo objeto de conexao ao banco de dados
                DatabaseHelper objDb = new DatabaseHelper("aniversariantes");

                // Define SQL Query
                String query = "SELECT id , c_tipo FROM dados.tipo_aniversariantes WHERE b_deletado = false";

                // Executa a query
                DataTable dt = objDb.GetDataTable(query);

                // Seta itens do datagrid com o retorno da query
                dgridTipos.ItemsSource = dt.DefaultView;
            }

            // Trata excessão
            catch (Exception fail) {

                // Seta mensagem de erro
                String error = "O seguinte erro ocorreu:\n\n";

                // Anexa mensagem de erro na mensagem
                error += fail.Message.ToString() + "\n\n";

                // Apresenta mensagem na tela
                MessageBox.Show(error);

                // Fecha o formulário
                this.Close();

            }

        }

        private void btnAlterar_Click(object sender, RoutedEventArgs e) {

            // Verifica se existe usuario selecionado
            if (dgridTipos.SelectedItem == null) {

                MessageBox.Show("Você deve selecionar um usuário para alterar");

                return;

            }

            else {

                // Defino DataRow para poder pegar item selecionado
                DataRowView rowview = dgridTipos.SelectedItem as DataRowView;

                // Defino valor da coluna id
                string strId = rowview.Row["id"].ToString();

                // Defino nova janela passando id e operação
                AltTipo AlteraTipo = new AltTipo(strId);

                AlteraTipo.ShowDialog();

                // Update DataGrid
                CarregaDados();

            }
        }

    }

}