#region Usings

using System;
using System.Windows;
using System.Windows.Input;

using Cliente.Helpers;

#endregion

namespace Cliente {

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            // Inicializa componentes do formulário/window
            InitializeComponent();

            // Seta foco no campo de usuário
            tbUser.Focus();
        }

        /// <summary>
        ///     Evento ao clicar no botão fechar
        /// </summary>
        /// <param name="sender">Objeto de origem do click</param>
        /// <param name="e">Parametros passados pelo objeto</param>
        private void btnFechar_Click(object sender, RoutedEventArgs e) {
            // Fecha o formulário/window
            Close();
        }

        /// <summary>
        ///     Verifica e executa o login do usuário
        /// </summary>
        private void Logar() {
            // Tenta
            try {
                // Gera objeto de encriptação
                var teste = new EncryptHelper();

                // Encrypta senha
                var senha = teste.Encrypt(tbPass.Password);

                // PEga informacao de usuario
                var user = tbUser.Text;

                // Gera novo objeto de conexao ao banco de dados
                var usuario = new DatabaseHelper("aniversariantes");

                // Define SQL Query
                var query = string.Format("SELECT c_usuario FROM dados.usuarios WHERE c_usuario = '{0}' AND c_senha = '{1}' AND b_ativo = true AND b_deletado = false;", user, senha);

                // Executa a query
                var result = usuario.ExecuteScalar(query);

                // Se não existe o usuario no banco
                if (string.IsNullOrEmpty(result)) {
                    // Informa que não encontrou o usuário
                    MessageBox.Show("Usuário ou senha incoretos");
                }

                // Se existir
                else {
                    // Carrega o formulário principal
                    var winPrincipal = new WinPrincipal();

                    // Mostra o formulário principal
                    winPrincipal.Show();

                    // Fecha o formulário de login
                    Close();
                }
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

        /// <summary>
        ///     Método ao pressionar tecla no campo de usuário
        /// </summary>
        /// <param name="sender">Objeto de origem</param>
        /// <param name="e">Parametros passados pelo objeto</param>
        private void tbUser_KeyDown(object sender, KeyEventArgs e) {
            // Verifica se a tecla for "Enter"
            if (e.Key == Key.Enter) {
                // Seta foco no campo de senha
                tbPass.Focus();
            }
        }

        /// <summary>
        ///     Método ao pressionar tecla no campo de senha
        /// </summary>
        /// <param name="sender">Objeto de origem</param>
        /// <param name="e">Parametros passados pelo objeto</param>
        private void tbPass_KeyDown(object sender, KeyEventArgs e) {
            // Verifica se a tecla é o Enter
            if (e.Key == Key.Enter) {
                // Chama rotina que trata do login
                Logar();
            }
        }

        /// <summary>
        ///     Método ao clicar no botão Logar
        /// </summary>
        /// <param name="sender">Objeto de origem</param>
        /// <param name="e">Parametros passados pelo objeto</param>
        private void btnLogar_Click(object sender, RoutedEventArgs e) {
            // Chama rotina que trata do login
            Logar();
        }

    }

}