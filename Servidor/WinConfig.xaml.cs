using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Servidor.Properties;

namespace Servidor {
    /// <summary>
    /// Interaction logic for WinConfig.xaml
    /// </summary>
    public partial class WinConfig : Window {
        public WinConfig() {

            InitializeComponent();

            CarregaDados();

        }

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

        private void btnFechar1_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
