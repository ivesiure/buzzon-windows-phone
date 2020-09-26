using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Controls;
using Windows.Devices.Geolocation;
using Newtonsoft.Json;
using BuzzOn.UI.ViewModels;
using System.Device.Location;
using System.ComponentModel;
using BuzzOn.UI.Utils;
using Microsoft.Phone.Net.NetworkInformation;
using BuzzOn.UI.Resources;
using Newtonsoft.Json.Linq;
using BuzzOn.UI.Facade;
using BuzzOn.UI.Model;

namespace BuzzOn.UI
{
    public partial class Mapa : PhoneApplicationPage
    {
        #region Propriedades da página
        private MapLayer LayerPosicaoOnibus { get; set; }
        private MapLayer LayerMinhaLocalizacao { get; set; }
        private MapOverlay MinhaPosicao { get; set; }
        private WebBrowser webBrowser;
        private bool PrimeiraEntrada = true;
        protected string NomeLinha { get; set; }
        private ApplicationBarIconButton btnFavorito;

        #endregion

        #region Métodos da página

        public Mapa()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            this.txtNomeLinha.Text = string.Format("Linha {0} - {1}", App.LinhaSelecionada.Codigo, App.LinhaSelecionada.Nome);

            this.LayerPosicaoOnibus = new MapLayer();
            this.LayerMinhaLocalizacao = new MapLayer();

            mapOnibus.MapElements.Clear();
            mapOnibus.MapElements.Add(App.ItinerarioViewModel.ShapeRota);

            mapOnibus.Layers.Clear();
            mapOnibus.Layers.Add(LayerPosicaoOnibus);

            MinhaPosicao = new MapOverlay();
            MinhaPosicao.Content = PinMaker.Usuario();
            MinhaPosicao.PositionOrigin = new Point(0.5, 0.5);

            webBrowser = new WebBrowser();
            webBrowser.IsScriptEnabled = true;
            webBrowser.ScriptNotify += webBrowser_ScriptNotify;
            webBrowser.Navigate(new Uri("/App_Files/HtmlMapa.html#" + App.LinhaSelecionada.Codigo, UriKind.Relative));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            var url = e.Uri.ToString();
            if (url.ToUpper() == "/MAINPAGE.XAML")
            {
                mapOnibus.MapElements.Clear();
                mapOnibus.Layers.Clear();
                App.LinhaSelecionada = null;
                var obj = webBrowser.InvokeScript("disconnect");
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            MarcarMinhaLocalizacao();

            var horarios = NavigationService.BackStack.FirstOrDefault(i => i.Source.OriginalString == "/Horarios.xaml");
            if (horarios != null)
            {
                NavigationService.RemoveBackEntry();
            }
        }
        #endregion

        #region ApplicationBar

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBarIconButton appBarButtonPontos = new ApplicationBarIconButton(new Uri("/Assets/icons/bus.stop.png", UriKind.Relative));
            appBarButtonPontos.Text = "Exibir paradas";
            appBarButtonPontos.Click += AppBarButtonPontos_Click;

            ApplicationBarIconButton appBarButtonUpdate = new ApplicationBarIconButton(new Uri("/Assets/icons/sync.png", UriKind.Relative));
            appBarButtonUpdate.Text = "Buscar atualização";
            appBarButtonUpdate.Click += appBarButtonUpdate_Click;

            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = true;
            ApplicationBar.Buttons.Add(appBarButtonPontos);
            ApplicationBar.Buttons.Add(appBarButtonUpdate);

            this.btnFavorito = new ApplicationBarIconButton(new Uri(App.LinhaSelecionada.ImagemFavorito, UriKind.Relative));
            this.btnFavorito.Text = App.LinhaSelecionada.Favorita ? "Desfavoritar" : "Favoritar";
            this.btnFavorito.Click += btnFavorito_Click;
            ApplicationBar.Buttons.Add(btnFavorito);

            if (HorarioFacade.Instance.PossuiHorarios(App.LinhaSelecionada.Codigo))
            {
                var btnHorarios = new ApplicationBarIconButton(new Uri("/Assets/buttons/schedule.png", UriKind.Relative));
                btnHorarios.Text = "Horários";
                btnHorarios.Click += btnHorarios_Click;
                ApplicationBar.Buttons.Add(btnHorarios);
            }
        }

        void appBarButtonUpdate_Click(object sender, EventArgs e)
        {
            bool estaConectado = NetworkInterface.GetIsNetworkAvailable();
            if (estaConectado)
            {
                String Url = WebApiResources.VersaoItinerario.Replace("{codigo}", App.LinhaSelecionada.Codigo).Replace("{token}", App.UserId);

                WebClient clientAtualizacaoItinerario = new WebClient();
                clientAtualizacaoItinerario.DownloadStringCompleted += (snd, evt) =>
                {
                    if (String.IsNullOrEmpty(evt.Result) == false)
                    {
                        try
                        {
                            var retorno = JObject.Parse(evt.Result);
                            var versaoRotaAtual = VersaoFacade.Instance.ObterVersaoRota(App.LinhaSelecionada.Codigo);
                            var versaoPontoAtual = VersaoFacade.Instance.ObterVersaoPonto(App.LinhaSelecionada.Codigo);
                            var versaoRotaServidor = retorno["vRotas"].ToString();
                            var versaoPontoServidor = retorno["vPontos"].ToString();

                            string vRota = versaoRotaAtual == null ? "" : versaoRotaAtual.Valor;
                            string vPonto = versaoPontoAtual == null ? "" : versaoPontoAtual.Valor;

                            if (vRota.Equals(versaoRotaServidor) == false || vPonto.Equals(versaoPontoServidor) == false)
                            {
                                var decisao = MessageBox.Show("Existe uma atualização de deste itinerário disponível.\nDeseja baixá-la agora?", "Atualização disponível", MessageBoxButton.OKCancel);
                                if (decisao == MessageBoxResult.OK)
                                {
                                    estaConectado = NetworkInterface.GetIsNetworkAvailable();
                                    if (estaConectado)
                                    {
                                        string url = WebApiResources.Itinerario.Replace("{codigo}", App.LinhaSelecionada.Codigo).Replace("{token}", App.UserId);
                                        WebClient clientAtualizacao = new WebClient();
                                        clientAtualizacao.DownloadStringCompleted += clientAtualizacao_DownloadStringCompleted;

                                        ucLoading.Show("Carregando atualização. Por favor, aguarde alguns instantes...");

                                        clientAtualizacao.DownloadStringAsync(new Uri(url));
                                    }
                                }
                            }
                            else
                            {
                                ucToastMessage.ShowToast("Não existem atualizações disponíveis para este itinerário no momento.");
                            }
                        }
                        catch (Exception)
                        {
                            ucToastMessage.ShowToast("Desculpe, não foi possível completar esta solicitação.");
                        }
                    }
                };
                clientAtualizacaoItinerario.DownloadStringAsync(new Uri(Url));
            }
            else
            {
                ucToastMessage.ShowToast("Você precisa estar conectado à internet para executar esta operação...", 4);
            }
        }

        private void clientAtualizacao_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                bool atualizou = false;

                var itinerario = JObject.Parse(e.Result);

                var rotas = itinerario["Rotas"].ToString();
                if (String.IsNullOrEmpty(rotas) == false)
                {
                    var jsonRotas = JArray.Parse(rotas);
                    var listaRotas = jsonRotas.Select(i => new RotaModel
                    {
                        Latitude = i["lt"].Value<String>(),
                        Longitude = i["lg"].Value<String>(),
                        Codigo = App.LinhaSelecionada.Codigo
                    }).ToList();

                    RotaFacade.Instance.RemoverRotaDaLinha(App.LinhaSelecionada.Codigo);
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
                        Codigo = App.LinhaSelecionada.Codigo
                    }).ToList();

                    PontoFacade.Instance.RemoverPontosDaLinha(App.LinhaSelecionada.Codigo);
                    PontoFacade.Instance.AdicionarLista(listaPontos);
                    atualizou = true;
                }

                var vPontos = itinerario["vPontos"].ToString();
                if (String.IsNullOrEmpty(pontos) == false)
                {
                    VersaoFacade.Instance.AtualizarVersaoPonto(App.LinhaSelecionada.Codigo, vPontos);
                    atualizou = true;
                }

                var vRotas = itinerario["vRotas"].ToString();
                if (String.IsNullOrEmpty(vRotas) == false)
                {
                    VersaoFacade.Instance.AtualizarVersaoRotas(App.LinhaSelecionada.Codigo, vRotas);
                    atualizou = true;
                }

                if (atualizou)
                {
                    App.ItinerarioViewModel.CarregarDados(App.LinhaSelecionada.Id);

                    mapOnibus.MapElements.Clear();
                    mapOnibus.MapElements.Add(App.ItinerarioViewModel.ShapeRota);

                    ucToastMessage.ShowToast("Itinerário atualizado com sucesso.");

                    ucLoading.Hide();
                }
            }
            catch (Exception)
            {
                ucLoading.Hide();
                ucToastMessage.ShowToast("Não foi possível prosseguir com a sua solicitação.\nPor favor, tente novamente mais tarde.");
            }
        }

        private void AppBarButtonPontos_Click(object sender, EventArgs e)
        {
            var button = ApplicationBar.Buttons[0] as ApplicationBarIconButton;

            if (mapOnibus.Layers.Any(i => i == App.ItinerarioViewModel.LayerPontosOnibus))
            {
                mapOnibus.Layers.Remove(App.ItinerarioViewModel.LayerPontosOnibus);
                button.Text = "Exibir Paradas";
            }
            else
            {
                mapOnibus.Layers.Add(App.ItinerarioViewModel.LayerPontosOnibus);
                button.Text = "Esconder Paradas";
            }
        }

        protected void btnFavorito_Click(object sender, EventArgs e)
        {
            if (App.LinhaSelecionada.Favorita)
            {
                App.LinhaOnibusViewModel.DesfavoritarLinha(App.LinhaSelecionada.Id);
                App.LinhaSelecionada.Favorita = false;
                btnFavorito.Text = "Favoritar";
                ucToastMessage.ShowToast("Linha " + App.LinhaSelecionada.Nome + " removida dos favoritos");
            }
            else
            {
                App.LinhaOnibusViewModel.FavoritarLinha(App.LinhaSelecionada.Id);
                App.LinhaSelecionada.Favorita = true;
                btnFavorito.Text = "Desfavoritar";
                ucToastMessage.ShowToast("Linha " + App.LinhaSelecionada.Nome + " adicionada aos favoritos");
            }
            this.btnFavorito.IconUri = new Uri(App.LinhaSelecionada.ImagemFavorito, UriKind.Relative);
        }

        protected void btnHorarios_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Horarios.xaml", UriKind.Relative));
        }
        #endregion

        #region Eventos do Controle de Mapa

        private void mapOnibus_ResolveCompleted(object sender, MapResolveCompletedEventArgs e)
        {
            if (PrimeiraEntrada)
            {
                mapOnibus.SetView(App.ItinerarioViewModel.LayoutRota);
                PrimeiraEntrada = false;
            }
        }

        private void mapOnibus_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "4e16ec90-3a16-415e-aef1-7ab45b523543";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "_FIHedlH_VJ26GtD9dG9hQ";
        }

        #endregion

        #region Marcar Minha Posicao
        private async void MarcarMinhaLocalizacao()
        {
            try
            {
                var locationFinder = new Geolocator
                {
                    DesiredAccuracyInMeters = 50,
                    DesiredAccuracy = PositionAccuracy.Default,
                    MovementThreshold = 50
                };

                locationFinder.PositionChanged += LocationFinder_PositionChanged;
                locationFinder.GetGeopositionAsync(maximumAge: TimeSpan.FromSeconds(120), timeout: TimeSpan.FromSeconds(10));
            }
            catch { }
        }

        private void LocationFinder_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            try
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MinhaPosicao.GeoCoordinate = new GeoCoordinate
                    {
                        Latitude = args.Position.Coordinate.Point.Position.Latitude,
                        Longitude = args.Position.Coordinate.Point.Position.Longitude,
                    };

                    if (mapOnibus.Layers.Any(i => i == LayerMinhaLocalizacao) == false)
                    {
                        LayerMinhaLocalizacao.Add(MinhaPosicao);
                        mapOnibus.Layers.Add(LayerMinhaLocalizacao);
                    }
                });
            }
            catch (Exception ex) { }
        }
        #endregion

        #region Eventos do Controle WebBrowser

        private void webBrowser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            try
            {
                var json = e.Value;
                if (String.IsNullOrEmpty(json) || json.ToUpper().Equals("NULL"))
                {
                    LayerPosicaoOnibus.Clear();
                    ucToastMessage.ShowToast("Não existem veículos desta linha em funcionamento no momento.");
                }
                else
                {
                    var lista = JsonConvert.DeserializeObject<List<Coordenada>>(json);

                    LayerPosicaoOnibus.Clear();
                    var corLinha = App.LinhaSelecionada.ObjetoCor;

                    foreach (var item in lista)
                    {
                        Double latitude = Convert.ToDouble(item.latitude);
                        Double longitude = Convert.ToDouble(item.longitude);

                        var coordenada = new GeoCoordinate(latitude, longitude);

                        var mapOver = new MapOverlay();
                        mapOver.GeoCoordinate = coordenada;
                        mapOver.Content = PinMaker.Onibus(corLinha, item.hora);
                        mapOver.PositionOrigin = new Point(0.5, 0.5);

                        LayerPosicaoOnibus.Add(mapOver);
                    }
                }
            }
            catch (Exception ex0)
            {
            }
        }

        #endregion

        private class Coordenada
        {
            public String latitude { get; set; }
            public String longitude { get; set; }
            public String hora { get; set; }
        }
    }
}