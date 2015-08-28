using System;
using System.Data;
using System.Windows;

namespace Cliente {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e) {

            this.Close();

        }

        private void btnLogar_Click(object sender, RoutedEventArgs e) {


            try {

                DatabaseHelper usuario = new DatabaseHelper("aniversariantes");
                
                DataTable recipe;

                String query = String.Format("SELECT usuario FROM dados.usuarios WHERE usuario = '{0}' and senha = '{1}';",tbUser.Text,tbPass.Password);


                var result = usuario.ExecuteScalar(query);


                if (String.IsNullOrEmpty(result)) {

                    MessageBox.Show("Usuário ou senha não encontrados");
                
                }
                else {

                    // TODO - Rotina de acessar formulário principal
                    WinPrincipal winPrincipal = new WinPrincipal();
                    winPrincipal.Show();

                    this.Close();    
                
                }


            }
            catch (Exception fail) {

                String error = "The following error has occurred:\n\n";

                error += fail.Message.ToString() + "\n\n";

                MessageBox.Show(error);

                this.Close();

            }

        }

    }
}