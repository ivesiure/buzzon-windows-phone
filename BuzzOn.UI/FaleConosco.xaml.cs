using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BuzzOn.UI.Utils;
using BuzzOn.UI.Resources;
using System.Text.RegularExpressions;

namespace BuzzOn.UI
{
    public partial class FaleConosco : PhoneApplicationPage
    {
        private bool InformouCamposObrigatorios
        {
            get
            {
                return String.IsNullOrEmpty(txtNome.Text) == false && String.IsNullOrEmpty(txtEmail.Text) == false && String.IsNullOrEmpty(txtMensagem.Text) == false;
            }
        }

        private bool EmailValido
        {
            get
            {
                return Regex.IsMatch(txtEmail.Text,
                  @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                  @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
            }
        }

        public FaleConosco()
        {
            InitializeComponent();
        }

        private void btnEnviar_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (InformouCamposObrigatorios)
            {
                if (EmailValido)
                {
                    ExibirLoading(true);

                    var parametro = Encoding.Base64Encode("{" + String.Format("\"nome\": \"{0}\", \"email\": \"{1}\", \"telefone\": \"{2}\", \"mensagem\": \"{3}\", \"data\": \"{4}\", \"lida\": \"N\"", txtNome.Text, txtEmail.Text, txtTelefone.Text, txtMensagem.Text, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")) + "}");

                    String Url = String.IsNullOrEmpty(App.UserId) ? WebApiResources.FaleConosco.Replace("{mensagemUsuario}", parametro) : WebApiResources.FaleConosco.Replace("{token}", App.UserId).Replace("{mensagemUsuario}", parametro);
                    
                    WebClient client = new WebClient();
                    client.DownloadStringCompleted += Client_DownloadStringCompleted;
                    client.DownloadStringAsync(new Uri(Url, UriKind.Absolute));
                }
                else
                {
                    MessageBox.Show("Email inválido");
                }
            }
            else
            {
                MessageBox.Show("Os campos marcados com * são obrigatórios");
            }
        }

        private void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            ExibirLoading(false);
            MessageBox.Show("Mensagem enviada com sucesso");
            NavigationService.GoBack();
        }

        private void ExibirLoading(Boolean exibir)
        {
            retLoading.Visibility = txtCarregando.Visibility = barraProgresso.Visibility = exibir ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}