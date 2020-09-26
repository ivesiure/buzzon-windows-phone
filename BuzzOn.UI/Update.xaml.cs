using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BuzzOn.UI.ViewModels;
using BuzzOn.UI.Resources;
using Newtonsoft.Json;
using System.Threading.Tasks;
using BuzzOn.UI.Model;
using BuzzOn.UI.Facade;

namespace BuzzOn.UI
{
    public partial class Update : PhoneApplicationPage
    {
        private ObservableCollection<LogAtualizacaoViewModel> ListaLogAtualizacao;

        public Update()
        {
            InitializeComponent();

            ListaLogAtualizacao = new ObservableCollection<LogAtualizacaoViewModel>() 
            {  
                new LogAtualizacaoViewModel{ Titulo = "Download dos dados", SubTitulo = "Em andamento...", Visibilidade = System.Windows.Visibility.Visible }
            };
            llsLog.ItemsSource = ListaLogAtualizacao;

            IniciarAtualizacao();
        }

        private void IniciarAtualizacao()
        {
            String Url = String.IsNullOrEmpty(App.UserId) ? WebApiResources.LinhasHorariosVersoes : WebApiResources.LinhasHorariosVersoes.Replace("{token}", App.UserId);
            WebClient clientAtualizacao = new WebClient();
            clientAtualizacao.DownloadStringCompleted += ClientAtualizacao_DownloadStringCompleted;
            clientAtualizacao.DownloadStringAsync(new Uri(Url));
        }

        private void ClientAtualizacao_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    AdicionarItemLista("Atualização das linhas");

                    var retornoDados = JsonConvert.DeserializeObject<ViewModels.ConjuntoDadosViewModel>(e.Result);
                    
                    LinhaFacade.Instance.AtualizarListaDeLinhas(retornoDados.Linhas);

                    AdicionarItemLista("Atualização dos horários");

                    HorarioFacade.Instance.AtualizarListaDeHorarios(retornoDados.Horarios);

                    VersaoFacade.Instance.AtualizarVersoesLinhasHorarios(retornoDados.Versoes);
                    
                    AdicionarItemLista(null);
                }
                catch
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show("Desculpe, não foi possível concluir a atualização.", "Atenção", MessageBoxButton.OK));
                }
            });
        }

        private void AdicionarItemLista(String Titulo)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var ultimoRegistroLog = ListaLogAtualizacao.Last();
                ultimoRegistroLog.SubTitulo = "Concluído";
                ultimoRegistroLog.Visibilidade = Visibility.Collapsed;
                if (Titulo != null)
                { 
                    ListaLogAtualizacao.Add(new LogAtualizacaoViewModel { Titulo = Titulo, SubTitulo = "Em andamento...", Visibilidade = Visibility.Visible });
                }
                else
                {
                    btnContinuar.Visibility = System.Windows.Visibility.Visible;
                }
            });
        }

        private void btnContinuar_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}