using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using BuzzOn.UI.Facade;
using BuzzOn.UI.ViewModels;
using System.Collections.ObjectModel;

namespace BuzzOn.UI
{
    public partial class Horarios : PhoneApplicationPage
    {
        private ObservableCollection<DiaSemanaViewModel> TabelaHorarios;

        private ApplicationBarIconButton btnFavorito;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var mapa = NavigationService.BackStack.FirstOrDefault(i => i.Source.OriginalString == "/Mapa.xaml");
            if (mapa != null)
            {
                NavigationService.RemoveBackEntry();
            }
        }

        public Horarios()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                var tabelaHorarios = HorarioFacade.Instance.MontarTabelaHorarios(App.LinhaSelecionada.Codigo);
                TabelaHorarios = new ObservableCollection<DiaSemanaViewModel>(tabelaHorarios);

                Dispatcher.BeginInvoke(() =>
                {
                    pvtHorarios.ItemsSource = TabelaHorarios;
                    BuildLocalizedApplicationBar();
                    ucLoading.Hide();
                });
            });
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            this.btnFavorito = new ApplicationBarIconButton(new Uri(App.LinhaSelecionada.ImagemFavorito, UriKind.Relative));
            this.btnFavorito.Text = App.LinhaSelecionada.Favorita ? "Desfavoritar" : "Favoritar";
            this.btnFavorito.Click += btnFavorito_Click;
            ApplicationBar.Buttons.Add(btnFavorito);

            if (RotaFacade.Instance.PossuiRotas(App.LinhaSelecionada.Codigo))
            {
                var btnMapa = new ApplicationBarIconButton(new Uri("/Assets/icons/map.png", UriKind.Relative));
                btnMapa.Text = "Itinerário";
                btnMapa.Click += btnMapa_Click;
                ApplicationBar.Buttons.Add(btnMapa);
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

        protected void btnMapa_Click(object sender, EventArgs e)
        {
            App.ItinerarioViewModel.CarregarDados(App.LinhaSelecionada.Id);
            NavigationService.Navigate(new Uri("/Mapa.xaml", UriKind.Relative));
        }
    }
}