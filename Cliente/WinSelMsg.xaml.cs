#region Usings

using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

using Cliente.Helpers;

#endregion

namespace Cliente {

    /// <summary>
    ///     Interaction logic for WinSelMsg.xaml
    /// </summary>
    public partial class WinSelMsg : Window {
        #region Botões

        private void btnSalvar_Click(object sender, RoutedEventArgs e) {


            // Verifica se tem algum registro selecionado
            if (dgridSelMsg.SelectedItem == null) {

                MessageBox.Show("Você deve selecionar uma mensagem para alterar");

            }
            else {

                // Defino DataRow para poder pegar item selecionado
                var rowview = dgridSelMsg.SelectedItem as DataRowView;

                // Seta id com o objeto selecionado
                id = rowview.Row["id"].ToString();

                Close();

            }


        }

        private void titleBar_MouseDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        #endregion

        #region Propriedades

        public WinSelMsg() {

            // Inicializa os componentes
            InitializeComponent();

            // Inicializa o carregamento de dados
            CarregaDados();
        }

        public string id {
            get;
            set;
        }

        #endregion

        #region Métodos

        private void CarregaDados() {

            // Limpa dataGrid
            dgridSelMsg.ItemsSource = null;

            // Tenta
            try {
                // Gera novo objeto de conexao ao banco de dados
                var objDb = new DatabaseHelper("aniversariantes");

                // Define SQL Query
                var query = "SELECT id, c_titulo FROM dados.mensagem WHERE b_deletado <> true";

                // Executa a query
                var dt = objDb.GetDataTable(query);

                // Seta itens do datagrid com o retorno da query
                dgridSelMsg.ItemsSource = dt.DefaultView;
            }

            // Trata excessão
            catch (Exception fail) {
                // Seta mensagem de erro
                var error = "O seguinte erro ocorreu:\n\n";

                // Anexa mensagem de erro na mensagem
                error += fail.Message + "\n\n";

                // Apresenta mensagem na tela
                MessageBox.Show(error);

                // Fecha o formulário
                Close();
            }
        }

        #endregion

        private void dgridSelMsg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (dgridSelMsg.SelectedItem == null) {

                return;

            }

            else {

                // Defino DataRow para poder pegar item selecionado
                var rowview = dgridSelMsg.SelectedItem as DataRowView;

                // Seta id com o objeto selecionado
                id = rowview.Row["id"].ToString();

                Close();
            }

        }
    }

}