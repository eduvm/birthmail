#region Usings

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

using Cliente.Helpers;

#endregion

namespace Cliente {

    /// <summary>
    ///     Interaction logic for AltAniversariantes.xaml
    /// </summary>
    public partial class AltAniversariantes : Window {

        private readonly string _codOpe;

        private readonly string _idAniversariante;

        #region Construtores

        public AltAniversariantes(string operacao) {
            InitializeComponent();

            // Define usuario da classe como o usuario recebido
            _codOpe = operacao;
        }

        public AltAniversariantes(string id, string operacao) {
            InitializeComponent();

            // Define usuario da classe como o usuario recebido
            _idAniversariante = id;

            // Define operação
            _codOpe = operacao;

            // Carrega dados
            CarregaDados();
        }

        #endregion Construtores

        #region Métodos

        private void CarregaDados() {
            // Gera novo objeto de Conexao ao banco
            var dataBase = new DatabaseHelper("aniversariantes");

            // Gera Sql para pegar dados do usuario a ser alterado
            var query = string.Format("SELECT id, c_nome, c_email, d_data_completa, n_mes, n_dia, b_Ativo, n_mensagem_id FROM dados.aniversariantes WHERE b_deletado <> true AND id = {0}", _idAniversariante);

            // Executa a query
            var result = dataBase.GetDataTable(query);

            // Se não existe o usuario no banco
            if (result.Rows.Count == 0) {
                // Informa que não encontrou o usuário
                MessageBox.Show("Erro ao encontrar o aniversariante");

                Close();
            }

            // Popula campos com reslultado do Query ao Banco
            TbCod.Text = Convert.ToString(result.Rows[0][0]);
            TbNome.Text = Convert.ToString(result.Rows[0][1]);
            TbEmail.Text = Convert.ToString(result.Rows[0][2]);
            DtDateNas.Text = Convert.ToString(result.Rows[0][3]);
            TbMes.Text = Convert.ToString(result.Rows[0][4]);
            TbDia.Text = Convert.ToString(result.Rows[0][5]);
            CbAtivo.IsChecked = (bool) result.Rows[0][6];
            TbNMensagem.Text = Convert.ToString(result.Rows[0][7]);
        }

        private void AlteraAniversariante(string id) {
            // Se passar no teste de validação
            if (ValidaDados()) {
                var objDB2 = new DatabaseHelper("aniversariantes");

                // Gero nova lista com dados de campos e valores
                var lista = new Dictionary<string, string>();

                // Defino ativo
                var ativo = CbAtivo.IsChecked.Value;

                // Adiciona campos na lista com os respectivos valores
                lista.Add("c_nome", TbNome.Text);
                lista.Add("c_email", TbEmail.Text);
                lista.Add("d_data_completa", DtDateNas.Text);
                lista.Add("n_mes", TbMes.Text);
                lista.Add("n_dia", TbDia.Text);
                lista.Add("b_ativo", ativo.ToString());
                lista.Add("n_mensagem_id", TbNMensagem.Text);

                // Verifica se consegue dar update no usuario
                if (objDB2.Update("dados.aniversariantes", lista, "id = " + id)) {
                    MessageBox.Show("Aniversariante alterado com sucesso");
                }

                else {
                    MessageBox.Show("Ocorreu um erro ao tentar atualizar o aniversariante");
                }

                Close();
            }

            else {
                MessageBox.Show("Ocoreu erro ao tentar salvar o aniversariante");
            }
        }

        private bool ValidaDados() {
            if (string.IsNullOrEmpty(TbNome.Text)) {
                MessageBox.Show("O nome não pode ser vazio");
                return false;
            }

            if (string.IsNullOrEmpty(TbEmail.Text)) {
                MessageBox.Show("O email não pode ser vazio");
                return false;
            }

            if (string.IsNullOrEmpty(DtDateNas.Text)) {
                MessageBox.Show("A data de nascimento não pode ser vazio");
                return false;
            }

            if (string.IsNullOrEmpty(TbMes.Text)) {
                MessageBox.Show("O mes nao pode ser vazio");
                return false;
            }

            if (string.IsNullOrEmpty(TbDia.Text)) {
                MessageBox.Show("O dia não pode ser vazio");
                return false;
            }

            if (string.IsNullOrEmpty(TbNMensagem.Text)) {
                MessageBox.Show("O código da mensagem não pode ser vazio");
                return false;
            }

            return true;
        }

        private void IncluirAniversariante() {
            // CHama validação dos dados
            if (ValidaDados()) {
                // Gera novo objeto de conexao ao banco de dados
                var dataBase = new DatabaseHelper("aniversariantes");

                // Gero nova lista com dados de campos e valores
                var lista = new Dictionary<string, string>();

                // Defino ativo
                var ativo = CbAtivo.IsChecked.Value;

                // Adiciona campos na lista com os respectivos valores
                lista.Add("c_nome", TbNome.Text);
                lista.Add("c_email", TbEmail.Text);
                lista.Add("d_data_completa", DtDateNas.Text);
                lista.Add("n_mes", TbMes.Text);
                lista.Add("n_dia", TbDia.Text);
                lista.Add("b_ativo", ativo.ToString());
                lista.Add("n_mensagem_id", TbNMensagem.Text);

                // Chama método que insere os usuarios na tabela passando os parametros
                if (dataBase.Insert("dados.aniversariantes", lista)) {
                    MessageBox.Show("Aniversariante adicionado com sucesso");
                }
                else {
                    MessageBox.Show("Ocorreu um erro ao tentar adicionar um aniversariante");
                }

                Close();
            }

            else {
                MessageBox.Show("Ocoreu erro ao tentar salvar o aniversariante");
            }
        }

        #endregion

        #region Botões

        private void titleBar_MouseDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e) {
            // Fecha a janela atual
            Close();
        }

        private void btnFechar1_Click(object sender, RoutedEventArgs e) {
            // Fecha a janela atual
            Close();
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e) {
            switch (_codOpe) {
                case "incluir":
                    IncluirAniversariante();
                    break;

                case "alterar":
                    AlteraAniversariante(_idAniversariante);
                    break;
            }
        }

        #endregion Botões

        private void btnMin_Click(object sender, RoutedEventArgs e) {
            if (WindowState == WindowState.Normal) {
                WindowState = WindowState.Minimized;
            }

            else {
                WindowState = WindowState.Normal;
            }
        }
    }

}