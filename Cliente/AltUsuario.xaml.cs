using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Input;

using Cliente.Helpers;

namespace Cliente {

    /// <summary>
    /// Interaction logic for AltUsuario.xaml
    /// </summary>
    public partial class AltUsuario : Window {

        private string idUsuario;
        private string codOpe;

        #region Construtores

        public AltUsuario(string id, string operacao) {

            // Inicializa componentes
            InitializeComponent();

            // Define usuario da classe como o usuario recebido
            idUsuario = id;

            // Defino operação da classe com a operaão repassada
            codOpe = operacao;

            // Carrega dados
            carregaDados();

            // Seta foco no campo nome
            tbAltNome.Focus();

        }

        public AltUsuario(string operacao) {

            InitializeComponent();

            // Defino operação da classe com a operaão repassada
            codOpe = operacao;

            // Seta foco no campo nome
            tbAltNome.Focus();

        }

        #endregion Construtores

        #region Botões

        private void titleBar_MouseDown(object sender, MouseButtonEventArgs e) {

            this.DragMove();

        }

        private void btnFechar_Click(object sender, RoutedEventArgs e) {

            this.Close();

        }

        private void btnMax_Click(object sender, RoutedEventArgs e) {

            if (this.WindowState == System.Windows.WindowState.Normal) {

                this.WindowState = System.Windows.WindowState.Maximized;

            }

            else {

                this.WindowState = System.Windows.WindowState.Normal;

            }

        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e) {

            switch (codOpe) {

                case "incluir":
                    IncluiUsuario();
                    break;

                case "alterar":
                    AlteraUsuario(idUsuario);
                    break;
            }
        }

        private void btnFechar1_Click(object sender, RoutedEventArgs e) {

            // Fecha a janela atual
            this.Close();

        }

        #endregion Botões

        private void carregaDados() {

            // Gera novo objeto de Conexao ao banco
            DatabaseHelper dataBase = new DatabaseHelper("aniversariantes");

            // Gera Sql para pegar dados do usuario a ser alterado
            String query = String.Format("SELECT id, c_usuario, c_senha, b_ativo, b_admin, c_nome, c_email FROM dados.usuarios WHERE id = {0}", idUsuario);

            // Executa a query
            DataTable result = dataBase.GetDataTable(query);

            // Se não existe o usuario no banco
            if (result.Rows.Count == 0) {

                // Informa que não encontrou o usuário
                MessageBox.Show("Erro ao encontrar usuario");

                this.Close();

                return;

            }

            else {

                // Cria novo objeto de Encryptação
                EncryptHelper objEncrypt = new EncryptHelper();

                // Desencrypta a senha
                tbAltSenha.Text = objEncrypt.Decrypt((string)result.Rows[0][2]);

                tbCodigo.Text = Convert.ToString(result.Rows[0][0]);
                tbAltUsuario.Text = (string)result.Rows[0][1];
                cbAtivo.IsChecked = (bool)result.Rows[0][3];
                cbAdmin.IsChecked = (bool)result.Rows[0][4];
                tbAltNome.Text = (string)result.Rows[0][5];
                tbAltEmail.Text = (string)result.Rows[0][6];

            }

        }

        private void IncluiUsuario() {

            // CHama validação dos dados
            validaDados();

            // Gera objeto para encritar a senha
            EncryptHelper objEncryptar = new EncryptHelper();

            // Encrypto a senha
            var senha = objEncryptar.Encrypt(tbAltSenha.Text);

            // Defino usuario
            var usuario = tbAltUsuario.Text;

            // Defino email
            var email = tbAltEmail.Text;

            // Defino nome
            var nome = tbAltNome.Text;

            // Defino admin
            var admin = cbAdmin.IsChecked.Value;

            // Defino ativo
            var ativo = cbAtivo.IsChecked.Value;

            // Gera novo objeto de conexao ao banco de dados
            DatabaseHelper dataBase = new DatabaseHelper("aniversariantes");

            // Gero nova lista com dados de campos e valores
            Dictionary<string, string> lista = new Dictionary<string, string>();

            // Adiciona campos na lista com os respectivos valores
            lista.Add("c_usuario", usuario);
            lista.Add("c_senha", senha);
            lista.Add("b_ativo", ativo.ToString());
            lista.Add("b_admin", admin.ToString());
            lista.Add("c_nome", nome);
            lista.Add("c_email", email);

            // Chama método que insere os usuarios na tabela passando os parametros
            if (dataBase.Insert("dados.usuarios", lista) != false) {

                MessageBox.Show("Usuario adicionado com sucesso");

            }
            else {

                MessageBox.Show("Ocorreu um erro ao tentar cadastrar o usuário.");

            }

            this.Close();

        }

        private void AlteraUsuario(string id) {

            // CHama validação dos dados
            validaDados();

            // Gera objeto para encritar a senha
            EncryptHelper objEncryptar = new EncryptHelper();

            // Encrypto a senha
            var senha = objEncryptar.Encrypt(tbAltSenha.Text);

            // Defino usuario
            var usuario = tbAltUsuario.Text;

            // Defino email
            var email = tbAltEmail.Text;

            // Defino nome
            var nome = tbAltNome.Text;

            // Defino admin
            var admin = cbAdmin.IsChecked.Value;

            // Defino ativo
            var ativo = cbAtivo.IsChecked.Value;

            // Gera novo objeto de conexao ao banco de dados
            DatabaseHelper dataBase = new DatabaseHelper("aniversariantes");

            // Gero nova lista com dados de campos e valores
            Dictionary<string, string> lista = new Dictionary<string, string>();

            // Adiciona campos na lista com os respectivos valores
            lista.Add("c_usuario", usuario);
            lista.Add("c_senha", senha);
            lista.Add("b_ativo", ativo.ToString());
            lista.Add("b_admin", admin.ToString());
            lista.Add("c_nome", nome);
            lista.Add("c_email", email);

            // Verifica se consegue dar update no usuario
            if (dataBase.Update("dados.usuarios", lista, "id = " + id) != false) {

                MessageBox.Show("Usuario alterado com sucesso");

            }
            else {

                MessageBox.Show("Ocorreu um erro ao tentar atualizar o usuário");

            }

            this.Close();

        }

        private void validaDados() {
            // Verifica se campo nome foi prenchido
            if (String.IsNullOrEmpty(tbAltNome.Text)) {
                MessageBox.Show("Nome não preenchido");
                return;
            }

            // Verifica email
            if (String.IsNullOrEmpty(tbAltEmail.Text)) {
                MessageBox.Show("Email nao especificado");
                return;
            }

            // Verifica usuario
            if (String.IsNullOrEmpty(tbAltUsuario.Text)) {
                MessageBox.Show("Usuário não preenchido");
                return;
            }

            // Verifica senha
            if (String.IsNullOrEmpty(tbAltSenha.Text)) {
                MessageBox.Show("Senha não preenchida");
                return;
            }
        }

    }
}