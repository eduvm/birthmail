#region Usings

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;

using Cliente.Helpers;

using WinInterop = System.Windows.Interop;

#endregion

namespace Cliente {

    /// <summary>
    ///     Interaction logic for WinCadUsuarios.xaml
    /// </summary>
    public partial class WinCadUsuarios : Window {
        #region Construtores

        public WinCadUsuarios() {
            InitializeComponent();

            stItem3.Content = "v" + Assembly.GetEntryAssembly().GetName().Version;

            // Define novo evento para tratar da maximizaçao da janela
            SourceInitialized += win_SourceInitialized;

            // Chama função que carrega os dados do banco e popula o DataGrid
            CarregaDados();
        }

        #endregion

        #region Métodos

        private void CarregaDados() {
            // Limpa dataGrid
            dgridUsuarios.ItemsSource = null;

            // Tenta
            try {
                // Gera novo objeto de conexao ao banco de dados
                var usuario = new DatabaseHelper("aniversariantes");

                // Define SQL Query
                var query = "SELECT id , c_nome, c_usuario, c_email, b_admin FROM dados.usuarios WHERE b_ativo = true AND b_deletado = false";

                // Executa a query
                var dt = usuario.GetDataTable(query);

                // Gera nova lista de pessoas
                var pList = new Pessoas();

                // Faz for para preencher a lista de pessoas
                foreach (DataRow row in dt.Rows) {
                    pList.Add(new Pessoa {
                        Id = row["id"].ToString(),
                        Nome = row["c_nome"].ToString(),
                        Usuario = row["c_usuario"].ToString(),
                        Email = row["c_email"].ToString(),
                        Admin = (bool) row["b_admin"]
                    });
                }

                // Faz bind da lista de pessoas no Grid
                dgridUsuarios.ItemsSource = pList;
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

        #endregion

        #region Pesquisa

        private void tbPesquisar_TextChanged(object sender, TextChangedEventArgs e) {
            var t = (TextBox) sender;
            var filter = t.Text;
            var cv = CollectionViewSource.GetDefaultView(dgridUsuarios.ItemsSource);

            if (filter == "") {
                cv.Filter = null;
            }
            else {
                cv.Filter = o => {
                    var p = o as Pessoa;
                    return (p.Nome.ToUpper().StartsWith(filter.ToUpper()));
                };
            }
        }

        #endregion Pesquisa

        #region Classes Internas

        public class Pessoa {

            public string Id {
                get;
                set;
            }

            public string Nome {
                get;
                set;
            }

            public string Usuario {
                get;
                set;
            }

            public string Email {
                get;
                set;
            }

            public bool Admin {
                get;
                set;
            }

        }

        public class Pessoas : ObservableCollection<Pessoa> {

        }

        #endregion Classes Internas

        #region Carrega WinAPI

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        /// <summary>
        /// </summary>
        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        #endregion Carrega WinAPI

        #region Maximização

        private void win_SourceInitialized(object sender, EventArgs e) {
            var handle = (new WinInterop.WindowInteropHelper(this)).Handle;
            WinInterop.HwndSource.FromHwnd(handle).AddHook(WindowProc);
        }

        private static IntPtr WindowProc(
            IntPtr hwnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled) {
            switch (msg) {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return (IntPtr) 0;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam) {
            var mmi = (MINMAXINFO) Marshal.PtrToStructure(lParam, typeof (MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            var MONITOR_DEFAULTTONEAREST = 0x00000002;
            var monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero) {
                var monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                var rcWorkArea = monitorInfo.rcWork;
                var rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        /// <summary>
        ///     POINT aka POINTAPI
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT {

            /// <summary>
            ///     x coordinate of point.
            /// </summary>
            public int x;

            /// <summary>
            ///     y coordinate of point.
            /// </summary>
            public int y;

            /// <summary>
            ///     Construct a point of coordinates (x,y).
            /// </summary>
            public POINT(int x, int y) {
                this.x = x;
                this.y = y;
            }

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO {

            public POINT ptReserved;

            public POINT ptMaxSize;

            public POINT ptMaxPosition;

            public POINT ptMinTrackSize;

            public POINT ptMaxTrackSize;

        };

        private void win_Loaded(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO {

            /// <summary>
            /// </summary>
            public int cbSize = Marshal.SizeOf(typeof (MONITORINFO));

            /// <summary>
            /// </summary>
            public RECT rcMonitor = new RECT();

            /// <summary>
            /// </summary>
            public RECT rcWork = new RECT();

            /// <summary>
            /// </summary>
            public int dwFlags = 0;

        }

        /// <summary> Win32 </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT {

            /// <summary> Win32 </summary>
            public int left;

            /// <summary> Win32 </summary>
            public int top;

            /// <summary> Win32 </summary>
            public int right;

            /// <summary> Win32 </summary>
            public int bottom;

            /// <summary> Win32 </summary>
            public static readonly RECT Empty;

            /// <summary> Win32 </summary>
            public int Width {
                get {
                    return Math.Abs(right - left);
                } // Abs needed for BIDI OS
            }

            /// <summary> Win32 </summary>
            public int Height {
                get {
                    return bottom - top;
                }
            }

            /// <summary> Win32 </summary>
            public RECT(int left, int top, int right, int bottom) {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            /// <summary> Win32 </summary>
            public RECT(RECT rcSrc) {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }

            /// <summary> Win32 </summary>
            public bool IsEmpty {
                get {
                    // BUGBUG : On Bidi OS (hebrew arabic) left > right
                    return left >= right || top >= bottom;
                }
            }

            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString() {
                if (this == Empty) {
                    return "RECT {Empty}";
                }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
            public override bool Equals(object obj) {
                if (!(obj is Rect)) {
                    return false;
                }
                return (this == (RECT) obj);
            }

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode() {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }

            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2) {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
            }

            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2) {
                return !(rect1 == rect2);
            }

        }

        #endregion Maximização

        #region ResizeWindows

        private bool ResizeInProcess;

        private void Resize_Init(object sender, MouseButtonEventArgs e) {
            var senderRect = sender as Rectangle;
            if (senderRect != null) {
                ResizeInProcess = true;
                senderRect.CaptureMouse();
            }
        }

        private void Resize_End(object sender, MouseButtonEventArgs e) {
            var senderRect = sender as Rectangle;
            if (senderRect != null) {
                ResizeInProcess = false;
                ;
                senderRect.ReleaseMouseCapture();
            }
        }

        private void Resizeing_Form(object sender, MouseEventArgs e) {
            if (ResizeInProcess) {
                var senderRect = sender as Rectangle;
                var mainWindow = senderRect.Tag as Window;

                if (senderRect != null) {
                    var width = e.GetPosition(mainWindow).X;
                    var height = e.GetPosition(mainWindow).Y;
                    senderRect.CaptureMouse();

                    if (senderRect.Name.ToLower().Contains("right")) {
                        width += 5;

                        // Verifico o tamanho minimo da janela

                        if (width > 0) {
                            if (width > mainWindow.MinWidth) {
                                mainWindow.Width = width;
                            }
                        }
                    }

                    if (senderRect.Name.ToLower().Contains("left")) {
                        width -= 5;
                        mainWindow.Left += width;
                        width = mainWindow.Width - width;
                        if (width > 0) {
                            if (width > mainWindow.MinWidth) {
                                mainWindow.Width = width;
                            }
                        }
                    }

                    if (senderRect.Name.ToLower().Contains("bottom")) {
                        height += 5;
                        if (height > 0) {
                            if (height > mainWindow.MinHeight) {
                                mainWindow.Height = height;
                            }
                        }
                    }

                    if (senderRect.Name.ToLower().Contains("top")) {
                        height -= 5;
                        mainWindow.Top += height;
                        height = mainWindow.Height - height;

                        if (height > 0) {
                            if (height > mainWindow.MinHeight) {
                                mainWindow.Height = height;
                            }
                        }
                    }
                }
            }
        }

        #endregion ResizeWindows

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

        private void btnIncluir_Click(object sender, RoutedEventArgs e) {
            // Defino nova janela passando id e operação
            var alterarUsuario = new AltUsuario("incluir");

            // Mostro a janela criada
            alterarUsuario.ShowDialog();

            // Update DataGrid
            CarregaDados();
        }

        private void btnAlterar_Click(object sender, RoutedEventArgs e) {
            // Verifica se existe usuario selecionado
            if (dgridUsuarios.SelectedItem == null) {
                MessageBox.Show("Você deve selecionar um usuário para alterar");
            }


            // Pega objeto selecionado
            Pessoa selecionado = (Pessoa)dgridUsuarios.SelectedItem;

            // Defino valor da coluna id
            var strId = selecionado.Id;

            // Defino nova janela passando id e operação
            var alterarUsuario = new AltUsuario(strId, "alterar");

            alterarUsuario.ShowDialog();

            // Update DataGrid
            CarregaDados();
        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e) {
            // Verifica se existe usuario selecionado
            if (dgridUsuarios.SelectedItem == null) {
                MessageBox.Show("Você deve selecionar um usuário para deletar");
            }
            else {
                // Defino DataRow para poder pegar item selecionado
                var rowview = dgridUsuarios.SelectedItem as DataRowView;

                // Defino valor da coluna id
                var strId = rowview.Row["id"].ToString();

                // Cria novo objeto de database
                var deletar = new DatabaseHelper();

                if (deletar.Delete("dados.usuarios", "id = " + strId)) {
                    MessageBox.Show("Usuario deletado");

                    CarregaDados();
                }

                else {
                    MessageBox.Show("Ocorreu erro ao tentar deletar o usuário");
                }
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