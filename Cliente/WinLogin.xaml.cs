using System;
using System.Data;
using System.Windows;

namespace Cliente
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {

            this.Close();

        }

        private void btnLogar_Click(object sender, RoutedEventArgs e)
        {


            try
            {

                // Gera objeto de encriptação
                EncryptHelper teste = new EncryptHelper();

                // Encrypta senha
                var senha = teste.Encrypt(tbPass.Password);

                // PEga informacao de usuario
                var user = tbUser.Text;

                // Gera novo objeto de conexao ao banco de dados
                DatabaseHelper usuario = new DatabaseHelper("aniversariantes");

                // Define SQL Query
                String query = String.Format("SELECT usuario FROM dados.usuarios WHERE usuario = '{0}' and senha = '{1}' and ativo = true;", user, senha);

                // Executa a query
                var result = usuario.ExecuteScalar(query);

                // Verifica se existe o usuario no banco
                if (String.IsNullOrEmpty(result))
                {

                    // Informa que não encontrou o usuário
                    MessageBox.Show("Usuário ou senha incoretos");

                }

                else
                {

                    // Carrega o formulário principal
                    WinPrincipal winPrincipal = new WinPrincipal();

                    // Mostra o formulário principal
                    winPrincipal.Show();

                    // Fecha o formulário de login
                    this.Close();

                }

            }

            catch (Exception fail)
            {

                String error = "The following error has occurred:\n\n";

                error += fail.Message.ToString() + "\n\n";

                MessageBox.Show(error);

                this.Close();

            }

        }

    }
}