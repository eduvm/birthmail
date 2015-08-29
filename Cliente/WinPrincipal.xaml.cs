using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Cliente {

    /// <summary>
    /// Interaction logic for WinPrincipal.xaml
    /// </summary>
    public partial class WinPrincipal : Window {

        public WinPrincipal() {
            InitializeComponent();
        }

        private void titleBar_MouseDown(object sender, MouseButtonEventArgs e) {
            this.DragMove();

        }

        //private DataTable CarregaRecentes() {

        //    // Gera novo objeto de conexao ao banco de dados
        //    DatabaseHelper recentes = new DatabaseHelper("aniversariantes");

        //    // Define SQL Query
        //    String query = String.Format("SELECT * FROM");

        //    // Executa a query
        //    //var result = usuario.ExecuteScalar(query);

        //}

    }
}