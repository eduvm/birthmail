#region Usings

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

using Cliente.Helpers;

using WinInterop = System.Windows.Interop;

#endregion

namespace Cliente {

    /// <summary>
    ///     Classe que trata da alteração das mensagens
    /// </summary>
    public partial class AltMensagem : Window {

        private readonly string _codOpe;

        private readonly string _idMensagem;

        #region Construtores

        // Construtor
        public AltMensagem(string operacao) {
            // Inicializa componentes
            InitializeComponent();

            // Define usuario da classe como o usuario recebido
            _codOpe = operacao;

            // Define novo evento para tratar da maximizaçao da janela
            SourceInitialized += win_SourceInitialized;
        }

        // Construtor
        public AltMensagem(string id, string operacao) {
            // Inicializa componentes
            InitializeComponent();

            // Define usuario da classe como o usuario recebido
            _idMensagem = id;

            // Define operação
            _codOpe = operacao;

            // Carrega dados
            CarregaDados();

            // Define novo evento para tratar da maximizaçao da janela
            SourceInitialized += win_SourceInitialized;
        }

        #endregion Construtores

        #region Métodos

        private void CarregaDados() {
            // Gera novo objeto de Conexao ao banco
            var dataBase = new DatabaseHelper("aniversariantes");

            // Gera Sql para pegar dados do usuario a ser alterado
            var query = string.Format("SELECT id, t_mensagem, c_titulo FROM dados.mensagem WHERE b_deletado <> true AND id = {0}", _idMensagem);

            // Executa a query
            var result = dataBase.GetDataTable(query);

            // Se não existe o usuario no banco
            if (result.Rows.Count == 0) {
                // Informa que não encontrou o usuário
                MessageBox.Show("Erro ao encontrar mensagem");

                Close();
            }

            // Popula campos com reslultado do Query ao Banco
            TbCod.Text = Convert.ToString(result.Rows[0][0]);
            BrowserMsg.ContentHtml = Convert.ToString(result.Rows[0][1]);
            TbTitulo.Text = Convert.ToString(result.Rows[0][2]);
        }

        private void AlteraMensagem(string id) {
            // Se passar no teste de validação
            if (ValidaDados()) {
                var objDB2 = new DatabaseHelper("aniversariantes");

                // Gero nova lista com dados de campos e valores
                var lista = new Dictionary<string, string>();

                // Adiciona campos na lista com os respectivos valores
                lista.Add("c_titulo", TbTitulo.Text);
                lista.Add("t_mensagem", BrowserMsg.ContentHtml);

                // Verifica se consegue dar update no usuario
                if (objDB2.Update("dados.mensagem", lista, "id = " + id)) {
                    MessageBox.Show("Mensagem alterada com sucesso");
                }

                else {
                    MessageBox.Show("Ocorreu um erro ao tentar atualizar o tipo");
                }

                Close();
            }

            else {
                MessageBox.Show("Ocoreu erro ao tentar salvar a mensagem");
            }
        }

        private bool ValidaDados() {
            if (string.IsNullOrEmpty(BrowserMsg.ContentHtml)) {
                MessageBox.Show("A mensangem não pode ser vazia");

                return false;
            }

            return true;
        }

        private void IncluirMensagem() {
            // CHama validação dos dados
            ValidaDados();

            // Defino usuario
            var titulo = TbTitulo.Text;

            // Defino email
            var mensagem = BrowserMsg.ContentHtml;

            // Gera novo objeto de conexao ao banco de dados
            var dataBase = new DatabaseHelper("aniversariantes");

            // Gero nova lista com dados de campos e valores
            var lista = new Dictionary<string, string>();

            // Adiciona campos na lista com os respectivos valores
            lista.Add("c_titulo", titulo);
            lista.Add("t_mensagem", mensagem);

            // Chama método que insere os usuarios na tabela passando os parametros
            if (dataBase.Insert("dados.mensagem", lista)) {
                MessageBox.Show("Mensagem adicionada com sucesso");
            }
            else {
                MessageBox.Show("Ocorreu um erro ao tentar cadastrar a mensagem.");
            }

            Close();
        }

        #endregion

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

        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
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

                        if (width > 0) {
                            mainWindow.Width = width;
                        }
                    }

                    if (senderRect.Name.ToLower().Contains("left")) {
                        width -= 5;
                        mainWindow.Left += width;
                        width = mainWindow.Width - width;

                        if (width > 0) {
                            mainWindow.Width = width;
                        }
                    }

                    if (senderRect.Name.ToLower().Contains("bottom")) {
                        height += 5;
                        if (height > 0) {
                            mainWindow.Height = height;
                        }
                    }

                    if (senderRect.Name.ToLower().Contains("top")) {
                        height -= 5;
                        mainWindow.Top += height;
                        height = mainWindow.Height - height;

                        if (height > 0) {
                            mainWindow.Height = height;
                        }
                    }
                }
            }
        }

        #endregion ResizeWindows

        #region Botoões

        private void btnFechar1_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e) {
            switch (_codOpe) {
                case "incluir":
                    IncluirMensagem();
                    break;

                case "alterar":
                    AlteraMensagem(_idMensagem);
                    break;
            }
        }

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

        #endregion Botoões
    }

}