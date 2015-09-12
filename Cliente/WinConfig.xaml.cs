#region Usings

using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

using Cliente.Helpers;

#endregion

namespace Cliente {

    /// <summary>
    ///     Interaction logic for WinConfig.xaml
    /// </summary>
    public partial class WinConfig : Window {
        #region Construtores

        public WinConfig() {
            InitializeComponent();

            CarregaDados();
        }

        #endregion Construtores

        private void CarregaDados() {
            // Limpa dataGrid
            dgridTipos.ItemsSource = null;

            // Tenta
            try {
                // Gera novo objeto de conexao ao banco de dados
                var objDb = new DatabaseHelper("aniversariantes");

                // Define SQL Query
                var query = "SELECT id , c_tipo FROM dados.tipo_aniversariantes WHERE b_deletado = false";

                // Executa a query
                var dt = objDb.GetDataTable(query);

                // Seta itens do datagrid com o retorno da query
                dgridTipos.ItemsSource = dt.DefaultView;
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

        private void btnAlterar_Click(object sender, RoutedEventArgs e) {
            // Verifica se existe usuario selecionado
            if (dgridTipos.SelectedItem == null) {
                MessageBox.Show("Você deve selecionar um usuário para alterar");
            }

            // Defino DataRow para poder pegar item selecionado
            var rowview = dgridTipos.SelectedItem as DataRowView;

            // Defino valor da coluna id
            var strId = rowview.Row["id"].ToString();

            // Defino nova janela passando id e operação
            var AlteraTipo = new AltTipo(strId);

            AlteraTipo.ShowDialog();

            // Update DataGrid
            CarregaDados();
        }

        #region Botões

        private void titleBar_MouseDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        #endregion Botões
    }

}