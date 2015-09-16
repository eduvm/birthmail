#region Usings

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

using Cliente.Helpers;

#endregion

namespace Cliente {

    /// <summary>
    ///     Interaction logic for AltUsuario.xaml
    /// </summary>
    public partial class AltUsuario : Window {
        #region Declaração de variáveis

        private string codOpe;

        private string idUsuario;

        #endregion

        #region Métodos

        private void carregaDados() {
            // Gera novo objeto de Conexao ao banco
            var dataBase = new DatabaseHelper("aniversariantes");

            // Gera Sql para pegar dados do usuario a ser alterado
            var query = string.Format("SELECT id, c_usuario, c_senha, b_ativo, b_admin, c_nome, c_email FROM dados.usuarios WHERE id = {0}", idUsuario);

            // Executa a query
            var result = dataBase.GetDataTable(query);

            // Se não existe o usuario no banco
            if (result.Rows.Count == 0) {
                // Informa que não encontrou o usuário
                MessageBox.Show("Erro ao encontrar usuario");

                Close();
            }

            // Cria novo objeto de Encryptação
            var objEncrypt = new EncryptHelper();

            // Desencrypta a senha
            tbAltSenha.Text = objEncrypt.Decrypt((string) result.Rows[0][2]);

            tbCodigo.Text = Convert.ToString(result.Rows[0][0]);
            tbAltUsuario.Text = (string) result.Rows[0][1];
            cbAtivo.IsChecked = (bool) result.Rows[0][3];
            cbAdmin.IsChecked = (bool) result.Rows[0][4];
            tbAltNome.Text = (string) result.Rows[0][5];
            tbAltEmail.Text = (string) result.Rows[0][6];
        }

        private void IncluiUsuario() {
            // CHama validação dos dados
            validaDados();

            // Gera objeto para encritar a senha
            var objEncryptar = new EncryptHelper();

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
            var dataBase = new DatabaseHelper("aniversariantes");

            // Gero nova lista com dados de campos e valores
            var lista = new Dictionary<string, string>();

            // Adiciona campos na lista com os respectivos valores
            lista.Add("c_usuario", usuario);
            lista.Add("c_senha", senha);
            lista.Add("b_ativo", ativo.ToString());
            lista.Add("b_admin", admin.ToString());
            lista.Add("c_nome", nome);
            lista.Add("c_email", email);

            // Chama método que insere os usuarios na tabela passando os parametros
            if (dataBase.Insert("dados.usuarios", lista)) {
                MessageBox.Show("Usuario adicionado com sucesso");
            }
            else {
                MessageBox.Show("Ocorreu um erro ao tentar cadastrar o usuário.");
            }

            Close();
        }

        private void AlteraUsuario(string id) {
            // CHama validação dos dados
            validaDados();

            // Gera objeto para encritar a senha
            var objEncryptar = new EncryptHelper();

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
            var dataBase = new DatabaseHelper("aniversariantes");

            // Gero nova lista com dados de campos e valores
            var lista = new Dictionary<string, string>();

            // Adiciona campos na lista com os respectivos valores
            lista.Add("c_usuario", usuario);
            lista.Add("c_senha", senha);
            lista.Add("b_ativo", ativo.ToString());
            lista.Add("b_admin", admin.ToString());
            lista.Add("c_nome", nome);
            lista.Add("c_email", email);

            // Verifica se consegue dar update no usuario
            if (dataBase.Update("dados.usuarios", lista, "id = " + id)) {
                MessageBox.Show("Usuario alterado com sucesso");
            }
            else {
                MessageBox.Show("Ocorreu um erro ao tentar atualizar o usuário");
            }

            Close();
        }

        private void validaDados() {
            // Verifica se campo nome foi prenchido
            if (string.IsNullOrEmpty(tbAltNome.Text)) {
                MessageBox.Show("Nome não preenchido");
                return;
            }

            // Verifica email
            if (string.IsNullOrEmpty(tbAltEmail.Text)) {
                MessageBox.Show("Email nao especificado");
                return;
            }

            // Verifica usuario
            if (string.IsNullOrEmpty(tbAltUsuario.Text)) {
                MessageBox.Show("Usuário não preenchido");
                return;
            }

            // Verifica senha
            if (string.IsNullOrEmpty(tbAltSenha.Text)) {
                MessageBox.Show("Senha não preenchida");
            }
        }

        #endregion

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
            DragMove();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void btnMax_Click(object sender, RoutedEventArgs e) {
            if (WindowState == WindowState.Normal) {
                WindowState = WindowState.Maximized;
            }

            else {
                WindowState = WindowState.Normal;
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
            Close();
        }

        #endregion Botões
    }

}