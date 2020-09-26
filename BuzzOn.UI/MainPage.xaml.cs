using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BuzzOn.UI.Resources;
using Microsoft.Phone.Net.NetworkInformation;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Windows.Media;
using System.Collections.Generic;
using Newtonsoft.Json;
using Windows.ApplicationModel;
using BuzzOn.UI.Facade;
using BuzzOn.UI.Model;

namespace BuzzOn.UI
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Métodos da Página
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ucLoading.Hide();

            var welcome = NavigationService.BackStack.FirstOrDefault(i => i.Source.OriginalString == "/Welcome.xaml");
            if (welcome != null)
            {
                NavigationService.RemoveBackEntry();
            }

            base.OnNavigatedTo(e);
            if (App.LinhaOnibusViewModel.ColecoesForamCarregadas() == false)
                App.LinhaOnibusViewModel.CarregarColecaoDeDadosDaBase();

            if (App.LinhaOnibusViewModel.LinhasFavoritas.Any() == false)
            {
                pivotLinhas.SelectedIndex = 1;
                txtListaVazia.Visibility = Visibility.Visible;
            }
            else
            {
                txtListaVazia.Visibility = Visibility.Collapsed;
            }

            System.Threading.Tasks.Task.Run(() => BuscarPorAtualizacoes(false));
        }
        #endregion

        #region Construtor
        public MainPage()
        {
            InitializeComponent();

            this.DataContext = App.LinhaOnibusViewModel;
            App.ItinerarioViewModel.CarregamentoCompleto += ItinerarioViewModel_CarregamentoCompleto;

            BuildLocalizedApplicationBar();
        }
        #endregion

        #region Busca por atualizações
        private void BuscarPorAtualizacoes(bool solicitadoPeloUsuario)
        {
            bool estaConectado = NetworkInterface.GetIsNetworkAvailable();
            if (estaConectado)
            {
                String Url = WebApiResources.TabelaVersoesLinhasHorarios.Replace("{token}", App.UserId);

                WebClient clientAtualizacaoLinhas = new WebClient();
                clientAtualizacaoLinhas.DownloadStringCompleted += ClientAtualizacaoLinhas_DownloadStringCompleted; ;
                clientAtualizacaoLinhas.DownloadStringAsync(new Uri(Url), solicitadoPeloUsuario);
            }
        }

        private void ClientAtualizacaoLinhas_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    var versoesServidor = JObject.Parse(e.Result);
                    var listaVersoes = versoesServidor["Horarios"].Select(i => new VersaoModel { Tabela = "Horario", Codigo = i["Codigo"].Value<String>(), Valor = i["Valor"].Value<String>() }).ToList();
                    listaVersoes.Insert(0, new VersaoModel { Tabela = "Linha", Valor = versoesServidor["Linhas"].Value<String>() });

                    var existemAtualizacoes = VersaoFacade.Instance.VerificaExistemAtualizacoes(listaVersoes);
                    if (existemAtualizacoes)
                    {
                        MessageBoxResult responseMessage;
                        Dispatcher.BeginInvoke(() =>
                        {
                            responseMessage = MessageBox.Show("Existe uma atualização de dados disponível.\nDeseja baixá-la agora?", "Atenção", MessageBoxButton.OKCancel);
                            if (responseMessage == MessageBoxResult.OK)
                            {
                                NavigationService.Navigate(new Uri("/Update.xaml", UriKind.Relative));
                            }
                            ApplicationBar.IsVisible = true;
                        });
                    }
                    else
                    {
                        var solicitadoPeloUsuario = Convert.ToBoolean(e.UserState);
                        if (solicitadoPeloUsuario)
                        {
                            Dispatcher.BeginInvoke(() =>
                            {
                                ucLoading.Hide();
                                MessageBox.Show("Não existem novas atualizações disponíveis no momento.");
                                ApplicationBar.IsVisible = true;
                            });
                        }
                    }
                }
                catch { }
            });
        }
        #endregion

        #region ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var mItemAtualizacao = new ApplicationBarMenuItem("Buscar atualização");
            var mItemSaldo = new ApplicationBarMenuItem("Saldo do cartão transporte");
            var mItemFaleConosco = new ApplicationBarMenuItem("Fale conosco");
            var mItemSobre = new ApplicationBarMenuItem("Sobre o BuzzOn");
            var mItemPolitica = new ApplicationBarMenuItem("Política de segurança");

            var aibItemSaldo = new ApplicationBarIconButton(new Uri("/Assets/Icons/money.png", UriKind.Relative));
            aibItemSaldo.Text = "Saldo do cartão";

            var aibItemAtualizacao = new ApplicationBarIconButton(new Uri("/Assets/Icons/sync.png", UriKind.Relative));
            aibItemAtualizacao.Text = "Buscar atualização";

            ApplicationBar.MenuItems.Add(mItemAtualizacao);
            ApplicationBar.MenuItems.Add(mItemFaleConosco);
            ApplicationBar.MenuItems.Add(mItemSaldo);
            ApplicationBar.MenuItems.Add(mItemSobre);
            ApplicationBar.MenuItems.Add(mItemPolitica);

            ApplicationBar.Buttons.Add(aibItemSaldo);
            ApplicationBar.Buttons.Add(aibItemAtualizacao);

            aibItemAtualizacao.Click += Atualizacao_Click;

            mItemAtualizacao.Click += Atualizacao_Click;

            mItemFaleConosco.Click += (sender, evt) =>
            {
                NavigationService.Navigate(new Uri("/FaleConosco.xaml", UriKind.Relative));
            };

            mItemSobre.Click += (sender, evt) =>
            {
                PackageVersion pv = Package.Current.Id.Version;
                var versaoAssembly = String.Format("Versão {0}.{1}.{2}.{3}", Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build, Package.Current.Id.Version.Revision);

                var sbSobre = new StringBuilder();
                sbSobre.AppendLine("\nBuzzOn");
                sbSobre.AppendLine(versaoAssembly);
                sbSobre.AppendLine("Copyright 2017 - Ives Iure M. Ancelmo");
                sbSobre.AppendLine("Todos os direitos reservados");
                sbSobre.AppendLine("\nContato: contato@buzzon.com.br");
                sbSobre.AppendLine("\nFacebook: fb.com/buzzoncwb");
                MessageBox.Show(sbSobre.ToString(), "Sobre o aplicativo", MessageBoxButton.OK);
            };

            aibItemSaldo.Click += Saldo_Click;
            mItemSaldo.Click += Saldo_Click;

            mItemPolitica.Click += (sender, evt) =>
            {
                Windows.System.Launcher.LaunchUriAsync(new Uri("https://buzzon.com.br/politica-de-seguranca.html"));
            };
        }

        protected void Atualizacao_Click(object sender, EventArgs e)
        {
            ApplicationBar.IsVisible = false;
            ucLoading.Show("Por favor, aguarde...");
            BuscarPorAtualizacoes(true);
        }

        protected void Saldo_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SaldoURBS.xaml", UriKind.Relative));
        }
        #endregion

        #region Eventos dos Controles de Pesquisa
        private void txtPesquisa_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (String.IsNullOrEmpty(txtPesquisa.Text) == false)
            {
                if (txtPesquisa.Text.Length >= 3)
                {
                    var pesquisa = Utils.Encoding.RemoveAccents(txtPesquisa.Text.ToUpper());
                    App.LinhaOnibusViewModel.FiltrarLinhas(i => i.NomeCompletoFiltro.Contains(pesquisa));
                }
            }
            else
            {
                App.LinhaOnibusViewModel.RemoverFiltro();
            }
        }
        #endregion

        #region Métodos de Controles da página

        #region Eventos dos Controles LongListSelector

        private void LongListSelector_ItemUnrealized(object sender, ItemRealizationEventArgs e)
        {
            txtListaVazia.Visibility = App.LinhaOnibusViewModel.LinhasFavoritas.Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        private void LongListSelector_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            txtListaVazia.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Eventos dos Controles Button

        private void btnTabelaHorarios_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var linhaSelecionada = (sender as Button).DataContext as LinhaModel;
            if (HorarioFacade.Instance.PossuiHorarios(linhaSelecionada.Codigo))
            {
                App.LinhaSelecionada = App.LinhaOnibusViewModel.LinhasOnibus.FirstOrDefault(i => i.Id == linhaSelecionada.Id);
                NavigationService.Navigate(new Uri("/Horarios.xaml", UriKind.Relative));
            }
            else
            {
                ucToastMessage.ShowToast("Desculpe.\nOs horários desta linha não estão disponíveis.");
            }
        }

        private void btnFavoritos_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                var btnFavorito = sender as Button;
                var linhaSelecionada = btnFavorito.DataContext as LinhaModel;
                var linha = App.LinhaOnibusViewModel.LinhasFavoritas.FirstOrDefault(i => i.Id == linhaSelecionada.Id);

                if (linha != null)
                {
                    var msgResult = MessageBox.Show(("Deseja remover a linha " + linha.Nome + " dos favoritos?"), "Atenção", MessageBoxButton.OKCancel);
                    if (msgResult == MessageBoxResult.OK)
                    {
                        App.LinhaOnibusViewModel.DesfavoritarLinha(linhaSelecionada.Id);
                        ucToastMessage.ShowToast("Linha " + linhaSelecionada.Nome + " removida dos favoritos");
                    }
                }
                else
                {
                    App.LinhaOnibusViewModel.FavoritarLinha(linhaSelecionada.Id);
                    ucToastMessage.ShowToast("Linha " + linhaSelecionada.Nome + " adicionada aos favoritos");
                }
            }
            catch (Exception)
            {
                ucToastMessage.ShowToast("Desculpe. Não foi possível concluir a operação.");
            }
        }

        private void btnMapa_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                var linhaSelecionada = (sender as Button).DataContext as LinhaModel;
                if (RotaFacade.Instance.PossuiRotas(linhaSelecionada.Codigo))
                {
                    ucLoading.Show("Carregando...");
                    App.LinhaSelecionada = App.LinhaOnibusViewModel.LinhasOnibus.FirstOrDefault(i => i.Id == linhaSelecionada.Id);
                    App.ItinerarioViewModel.CarregarDados(App.LinhaSelecionada.Id);
                }
                else
                {
                    bool estaConectado = NetworkInterface.GetIsNetworkAvailable();
                    if (estaConectado)
                    {
                        ucLoading.Show("Buscando as rotas desta linha...");
                        App.LinhaSelecionada = App.LinhaOnibusViewModel.LinhasOnibus.FirstOrDefault(i => i.Id == linhaSelecionada.Id);
                        var url = WebApiResources.Itinerario.Replace("{codigo}", linhaSelecionada.Codigo).Replace("{token}", App.UserId);

                        WebClient client = new WebClient();
                        client.DownloadStringCompleted += Client_DownloadStringCompleted;
                        client.DownloadStringAsync(new Uri(url), linhaSelecionada.Codigo);
                    }
                    else
                    {
                        ucLoading.Hide();
                        ucToastMessage.ShowToast("Desculpe.\nA rota desta linha não está disponível.");
                    }
                }
            }
            catch (Exception ex)
            {
                ucToastMessage.ShowToast("Desculpe. Não foi possível concluir a operação.");
            }

        }

        private void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                bool atualizou = false;

                var codigo = Convert.ToString(e.UserState);
                var itinerario = JObject.Parse(e.Result);

                var rotas = itinerario["Rotas"].ToString();
                if (String.IsNullOrEmpty(rotas) == false)
                {
                    var jsonRotas = JArray.Parse(rotas);
                    var listaRotas = jsonRotas.Select(i => new RotaModel
                    {
                        Latitude = i["lt"].Value<String>(),
                        Longitude = i["lg"].Value<String>(),
                        Codigo = codigo
                    }).ToList();
                    RotaFacade.Instance.AdicionarLista(listaRotas);
                    atualizou = true;
                }

                var pontos = itinerario["Pontos"].ToString();
                if (String.IsNullOrEmpty(pontos) == false)
                {
                    var jsonPontos = JArray.Parse(pontos);
                    var listaPontos = jsonPontos.Select(i => new PontoModel
                    {
                        Latitude = i["lt"].Value<String>(),
                        Longitude = i["lg"].Value<String>(),
                        Nome = i["nm"].Value<String>(),
                        Sentido = i["st"].Value<String>(),
                        Tipo = i["tp"].Value<String>(),
                        Codigo = codigo
                    }).ToList();
                    PontoFacade.Instance.AdicionarLista(listaPontos);
                    atualizou = true;
                }

                var vPontos = itinerario["vPontos"].ToString();
                if (String.IsNullOrEmpty(pontos) == false)
                {
                    VersaoFacade.Instance.Inserir(new BuzzOn.UI.Data.Versao { Codigo = codigo, Tabela = "Ponto", Valor = vPontos }); ;
                    atualizou = true;
                }

                var vRotas = itinerario["vRotas"].ToString();
                if (String.IsNullOrEmpty(vRotas) == false)
                {
                    VersaoFacade.Instance.Inserir(new BuzzOn.UI.Data.Versao { Codigo = codigo, Tabela = "Rota", Valor = vRotas }); ;
                    atualizou = true;
                }

                if (atualizou)
                {
                    App.ItinerarioViewModel.CarregarDados(App.LinhaSelecionada.Id);
                }
                else
                {
                    ucLoading.Hide();
                    ucToastMessage.ShowToast("Desculpe. O itinerário desta linha não está disponível no momento.");
                }
            }
            catch (Exception)
            {
                ucLoading.Hide();
                ucToastMessage.ShowToast("Não foi possível prosseguir com a sua solicitação.\nPor favor, tente novamente mais tarde.");
            }
        }

        private void ItinerarioViewModel_CarregamentoCompleto(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Mapa.xaml", UriKind.Relative));
        }

        #endregion

        #region Eventos de Grid
        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var foiDisparadoPorBotao = e.OriginalSource is Image || e.OriginalSource is Button;
            if (foiDisparadoPorBotao == false)
            {
                var grid = sender as Grid;
                App.LinhaSelecionada = grid.DataContext as LinhaModel;

                try
                {
                    if (HorarioFacade.Instance.PossuiHorarios(App.LinhaSelecionada.Codigo))
                    {
                        NavigationService.Navigate(new Uri("/Horarios.xaml", UriKind.Relative));
                    }
                    else
                    {
                        ucToastMessage.ShowToast("Desculpe.\nOs horários desta linha não estão disponíveis.");
                    }
                }
                catch (Exception)
                {
                    ucToastMessage.ShowToast("Desculpe. Não foi possível concluir a operação.");
                }
            }
        }
        #endregion

        #endregion
    }
}