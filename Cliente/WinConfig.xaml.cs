#region Usings

using System.Windows;
using System.Windows.Input;

using Cliente.Properties;

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

        private void btnSalvar_Click(object sender, RoutedEventArgs e) {

            // Verifica se todos os dados estão ok
            if (ValidaDados()) {

                // Se estiverem, salva as alterações nas preferencias
                Settings.Default.Host = tbHost.Text;
                Settings.Default.Port = tbPort.Text;
                Settings.Default.User = tbUser.Text;
                Settings.Default.Password = tbPassword.Text;
                Settings.Default.Database = tbDatabase.Text;
                Settings.Default.Save();

                // Fecha janela
                Close();

            }

        }

        #region Methods

        private void CarregaDados() {
            // Whe the window open, i will take the settings from the preferences
            tbHost.Text = Settings.Default.Host;
            tbPort.Text = Settings.Default.Port;
            tbUser.Text = Settings.Default.User;
            tbPassword.Text = Settings.Default.Password;
            tbDatabase.Text = Settings.Default.Database;
        }

        /// <summary>
        ///     Método que vai verificar se os campos estão em branco
        /// </summary>
        /// <returns>Retorna true se não estivere, e false se estivere</returns>
        private bool ValidaDados() {
            if (string.IsNullOrEmpty(tbHost.Text)) {
                return false;
            }

            if (string.IsNullOrEmpty(tbPort.Text)) {
                return false;
            }

            if (string.IsNullOrEmpty(tbUser.Text)) {
                return false;
            }

            if (string.IsNullOrEmpty(tbPassword.Text)) {
                return false;
            }

            if (string.IsNullOrEmpty(tbPassword.Text)) {
                return false;
            }

            if (string.IsNullOrEmpty(tbDatabase.Text)) {
                return false;
            }

            return true;
        }

        #endregion

        #region Botões

        private void titleBar_MouseDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }
        

        private void btnFechar1_Click(object sender, RoutedEventArgs e) {
            // Fecha a janela atual
            Close();
        }

        #endregion Botões

        private void btnFechar_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void btnMin_Click(object sender, RoutedEventArgs e) {

        }
    }

}