using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using BuzzOn.UI.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BuzzOn.UI.Model;
using BuzzOn.UI.Facade;

namespace BuzzOn.UI
{
    public partial class Welcome : PhoneApplicationPage
    {
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        public Welcome()
        {
            InitializeComponent();
            this.Loaded += Welcome_Loaded;
        }

        private void Welcome_Loaded(object sender, RoutedEventArgs e)
        {
            BuscarDadosAPI();
        }

        private void BuscarDadosAPI()
        {
            if (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                ExibirPainelLoading();
                WebClient wcLinhas = new WebClient();
                wcLinhas.Headers["Accept"] = "application/json";
                wcLinhas.DownloadStringCompleted += wcLinhas_DownloadCompleted;
                wcLinhas.DownloadProgressChanged += WcLinhas_DownloadProgressChanged;

                wcLinhas.DownloadStringAsync(new Uri(WebApiResources.DadosIniciais));
            }
            else
            {
                MessageBox.Show("Não foi possível continuar. Verifique se seu dispositivo está conectado à internet e tente novamente.", "Atenção", MessageBoxButton.OK);
                ExibirBotaoRetry();
            }
        }

        private void WcLinhas_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            txtStatus.Text = "Fazendo download dos dados... " + e.ProgressPercentage.ToString() + "%";
        }

        private void wcLinhas_DownloadCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            txtStatus.Text = "Salvando dados...";
            System.Threading.Tasks.Task.Run(() => SalvarDados(e.Result));
        }

        private void SalvarDados(String jsonResult)
        {
            try
            {
                var retornoDados = JsonConvert.DeserializeObject<ViewModels.ConjuntoDadosViewModel>(jsonResult);

                ParametroFacade.Instance.SalvarTokenUsuario(retornoDados.Token);
                LinhaFacade.Instance.InserirTodos(retornoDados.Linhas);
                HorarioFacade.Instance.InserirTodos(retornoDados.Horarios);
                VersaoFacade.Instance.InserirLista(retornoDados.Versoes);

                //App.BuzzOnContext.SubmitChanges();

                WebClient wcLinhas = new WebClient();
                wcLinhas.OpenReadAsync(new Uri(WebApiResources.ConcluirCadastro.Replace("{token}", retornoDados.Token)));

                Dispatcher.BeginInvoke(() => ExibirBotaoInicio());
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Não foi possível continuar. Verifique se seu dispositivo está conectado à internet e tente novamente.", "Atenção", MessageBoxButton.OK);
                    ExibirBotaoRetry();
                });
            }
        }

        private void btnComecar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void btnRetry_Click(object sender, RoutedEventArgs e)
        {
            BuscarDadosAPI();
        }

        private void ExibirPainelLoading()
        {
            pnlLoading.Visibility = Visibility.Visible;
            btnRetry.Visibility = Visibility.Collapsed;
            btnComecar.Visibility = Visibility.Collapsed;
        }

        private void ExibirBotaoInicio()
        {
            pnlLoading.Visibility = Visibility.Collapsed;
            btnRetry.Visibility = Visibility.Collapsed;
            btnComecar.Visibility = Visibility.Visible;
        }

        private void ExibirBotaoRetry()
        {
            pnlLoading.Visibility = Visibility.Collapsed;
            btnRetry.Visibility = Visibility.Visible;
            btnComecar.Visibility = Visibility.Collapsed;
        }
    }
}