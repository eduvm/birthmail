#region Usings

using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using Cliente;

#endregion

namespace Servidor {

    /// <summary>
    ///     Classe do arquivo MainWindow
    /// </summary>
    public partial class MainWindow : Window {

        private string cDia, cMes, cAno;

        private List<EnviosRecentes> Envios = new List<EnviosRecentes>();

        public MainWindow() {
            // Inicializa componentes
            InitializeComponent();

            //  Cria novo dispatcher timer
            var dispatcherTimer = new DispatcherTimer();

            // Define evento para cada tick
            dispatcherTimer.Tick += dispatcherTimer_Tick;

            // Define intervalo dos ticks (10 minutos)
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);

            // Inicia timer
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e) {
            // Chama método que verifica se deve enviar email
            VerificaEmail();

            // Updating the Label which displays the current second
            lvRecentes.ItemsSource = null;

            // Adiciona novamente
            lvRecentes.ItemsSource = Envios;

            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e) {
            // Define mensagem
            const string message = "Tem certeza que deseja fechar o serviço?\nO envio de emails para os aniversariantes será paralizado";

            // Define título
            const string caption = "Atenção";

            // Salva resultado da dialog na variavel result
            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

            // Verifica se o resultado foi yes, ou seja, o usuário clicou em sim
            if (result == MessageBoxResult.Yes) {
                Close();
            }
        }

        public void VerificaEmail() {
            // Pega data atual
            cDia = DateTime.Now.ToString("dd");
            cMes = DateTime.Now.ToString("MM");
            cAno = DateTime.Now.ToString("yyyy");

            // Seleciona todos os aniversariantes do dia
            var objDB = new DatabaseHelper("aniversariantes");

            // Gero query que vai retornar os aniversariantes do dia
            var query = $"SELECT id, c_nome, c_email, n_mensagem_id, n_dia, n_mes FROM dados.aniversariantes WHERE n_dia = '{cDia}' AND n_mes = '{cMes}' AND b_ativo = true AND b_deletado = false ORDER BY id";

            // Salvo os aniversariantes em um DataTable
            var AniversariantesDeHoje = objDB.GetDataTable(query);

            // Para cada linha do DataTable
            foreach (DataRow row in AniversariantesDeHoje.Rows) {
                // Defino id do usuario
                var idUsuario = row["id"].ToString();

                // Gero novo objeto de acesso ao banco de dados
                var objAniversariante = new DatabaseHelper("aniversariantes");

                // Gero Sql para verificar se já foi enviado um email para este usuário nesta data
                var query2 = $"SELECT * FROM dados.historico WHERE n_aniversarianteid = '{idUsuario}' AND  c_dia = '{cDia}' AND c_mes = '{cMes}' AND c_ano = '{cAno}' AND b_deletado = false";

                // Executo o query e salvo na variavel result
                var result = objAniversariante.ExecuteScalar(query2);

                // Se não existir a mensagem ainda,  ou seja, não foi enviado email para este usuário
                if (string.IsNullOrEmpty(result)) {
                    EnviaEmail(idUsuario);
                }
            }
        }

        /// <summary>
        ///     Envia email par ao aniversariante
        /// </summary>
        /// <param name="id"></param>
        public void EnviaEmail(string id) {
            // Defino novo objeto de banco de dados
            var objDb = new DatabaseHelper("aniversariantes");

            // Defino novo sql
            var query = $"SELECT A.id, A.c_nome, A.c_email, B.t_mensagem, B.c_titulo FROM dados.aniversariantes A, dados.mensagem B WHERE A.id = '{id}' AND A.n_mensagem_id = B.id AND A.b_ativo = true AND A.b_deletado = false AND B.b_deletado = false";

            // Executo o SQL
            var result = objDb.GetDataTable(query);

            // Executo forEach para pegar os campos
            foreach (DataRow row in result.Rows) {
                // Defino id do usuario
                var idUsuario = row["id"].ToString();

                // Defino o nome do aniversariante
                var nome = row["c_nome"].ToString();

                // Defino email do Aniversariante
                var email = row["c_email"].ToString();

                // Defino titulo da mensagem
                var tituloMensagem = row["c_titulo"].ToString();

                // Defino codigo da mensagem
                var mensagem = row["t_mensagem"].ToString();

                // Troca palavras chave dentro da mensagem
                //TODO - Cadastro de palavras chaves e um loop para substitiuir todas
                mensagem = mensagem.Replace("[NOMEANIVERSARIANTE]", nome);

                // Pega data atual
                var cDia = DateTime.Now.ToString("dd");
                var cMes = DateTime.Now.ToString("MM");
                var cAno = DateTime.Now.ToString("yyyy");

                // Defino data de envio
                var dDataEnvio = DateTime.Now.ToString("dd/MM/yy");

                // Define novo objeto de Smtp
                var client = new SmtpClient();

                // Define host
                client.Host = "smtp.gmail.com";

                // Define porta
                client.Port = 587;

                // Define se usa ssl
                client.EnableSsl = true;

                // Define método de envio
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                // Define para não usar credenciais
                client.UseDefaultCredentials = false;

                // Define endereço de origem
                var fromAddress = new MailAddress("masterautoparts.informatica@gmail.com", "Master Auto Parts");

                // Define senha
                var fromPassword = "master102030";

                // Define credencial
                var credencial = new NetworkCredential(fromAddress.Address, fromPassword);

                // Define endereço de destino
                var toAddress = new MailAddress(email, nome);

                // Define assunto
                const string subject = "Feliz aniversário";

                // Defome credenciais
                client.Credentials = credencial;

                // Define novo objeto de email
                var mail = new MailMessage(fromAddress, toAddress);

                mail.Subject = subject;
                mail.IsBodyHtml = true;
                mail.Body = mensagem;

                try {
                    // Tenta enviar o email
                    client.Send(mail);

                    // Cria novo objeto de Database
                    var objInserir = new DatabaseHelper("aniversariantes");

                    // Gera lista com dados a serem inseridos
                    var lista = new Dictionary<string, string>();

                    // Adiciona campos na lista com os respectivos valores
                    lista.Add("c_nome", nome);
                    lista.Add("c_email", email);
                    lista.Add("t_mensagem", mensagem);
                    lista.Add("c_titulo_mensagem",tituloMensagem);
                    lista.Add("d_data_envio", dDataEnvio);
                    lista.Add("c_dia", cDia);
                    lista.Add("c_mes", cMes);
                    lista.Add("c_ano", cAno);
                    lista.Add("n_aniversarianteid", id);

                    // Chama método que insere os usuarios na tabela passando os parametros
                    if (objInserir.Insert("dados.historico", lista)) {
                        // Adiciona informação na listview
                        Envios.Add(new EnviosRecentes {
                            Nome = nome, Email = email, DataEnvio = dDataEnvio
                        });
                    }

                    else {
                        MessageBox.Show("Ocorreu um erro ao tentar cadastrar o envio de email");
                        Close();
                    }
                }

                catch (Exception erro) {
                    // Trata mensagem de erro
                    MessageBox.Show("Ocorreu o seguinte erro ao tentar enviar o email:\n" + erro);
                    Close();
                }
            }
        }

    }

    public class EnviosRecentes {

        public string Nome {
            get;
            set;
        }

        public string Email {
            get;
            set;
        }

        public string DataEnvio {
            get;
            set;
        }

    }

}