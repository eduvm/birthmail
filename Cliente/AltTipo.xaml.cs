#region Usings

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

#endregion

namespace Cliente {

    /// <summary>
    ///     Interaction logic for AltTipo.xaml
    /// </summary>
    public partial class AltTipo : Window {

        private string idTipo;

        #region Construtores

        public AltTipo(string id) {
            // Inicializa componentes
            InitializeComponent();

            // Define usuario da classe como o usuario recebido
            idTipo = id;

            // Carrega dados
            carregaDados();

            // Seta foco no campo nome
            tbAltDesc.Focus();
        }

        #endregion Construtores

        #region Botões

        private void titleBar_MouseDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e) {
            AlterarTipo(idTipo);
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e) {
            // Fecha a janela atual
            Close();
        }

        private void btnFechar1_Click(object sender, RoutedEventArgs e) {
            // Fecha a janela atual
            Close();
        }

        #endregion Botões

        #region Métodos

        private void carregaDados() {
            // Gera novo objeto de Conexao ao banco
            var dataBase = new DatabaseHelper("aniversariantes");

            // Gera Sql para pegar dados do usuario a ser alterado
            var query = string.Format("SELECT id, c_tipo FROM dados.tipo_aniversariantes WHERE id = {0}", idTipo);

            // Executa a query
            var result = dataBase.GetDataTable(query);

            // Se não existe o usuario no banco
            if (result.Rows.Count == 0) {
                // Informa que não encontrou o usuário
                MessageBox.Show("Erro ao encontrar tipo");

                Close();
            }

            tbCodigo.Text = Convert.ToString(result.Rows[0][0]);
            tbAltDesc.Text = (string) result.Rows[0][1];
        }

        private void AlterarTipo(string id) {
            // CHama validação dos dados
            validaDados();

            // Defino tipo
            var tipo = tbAltDesc.Text;

            // Gera novo objeto de conexao ao banco de dados
            var dataBase = new DatabaseHelper("aniversariantes");

            // Gero nova lista com dados de campos e valores
            var lista = new Dictionary<string, string>();

            // Adiciona campos na lista com os respectivos valores
            lista.Add("c_tipo", tipo);

            // Verifica se consegue dar update no usuario
            if (dataBase.Update("dados.tipo_aniversariantes", lista, "id = " + id)) {
                MessageBox.Show("Tipo alterado com sucesso");
            }

            else {
                MessageBox.Show("Ocorreu um erro ao tentar atualizar o tipo");
            }

            Close();
        }

        private void validaDados() {
            // Verifica se campo nome foi prenchido
            if (string.IsNullOrEmpty(tbAltDesc.Text)) {
                MessageBox.Show("Descrição não preenchida");
            }
        }

        #endregion Métodos
    }

}