using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BuzzOn.UI.Data;
using System.Collections.ObjectModel;
using BuzzOn.UI.App_Classes;
using BuzzOn.UI.Model;

namespace BuzzOn.UI
{
    public partial class SaldoURBS : PhoneApplicationPage
    {
        public SaldoURBS()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
            Transitions.UseSlideTransition(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.PerfisSaldoViewModel.CarregarColecaoDeDadosDaBase();
            llsPerfisSaldo.ItemsSource = App.PerfisSaldoViewModel.ListaPerfisSaldo;
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var btnAdd = new ApplicationBarIconButton(new Uri("/Assets/icons/add.png", UriKind.Relative));
            btnAdd.Text = "Adicionar perfil";
            ApplicationBar.Buttons.Add(btnAdd);
            btnAdd.Click += (sender, evt) =>
            {
                NavigationService.Navigate(new Uri("/FormPerfilSaldo.xaml", UriKind.Relative));
            };
        }

        private void stkPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var perfil = ((StackPanel)sender).DataContext as PerfilSaldoModel;
            NavigationService.Navigate(new Uri("/HistoricoSaldoURBS.xaml?idPerfil=" + perfil.Id.ToString(), UriKind.Relative));
        }

        private void ctxMenuEditar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var perfil = ((MenuItem)sender).DataContext as PerfilSaldoModel;
                NavigationService.Navigate(new Uri("/FormPerfilSaldo.xaml?idPerfil=" + perfil.Id.ToString(), UriKind.Relative));
            }
            catch { }
        }

        private void ctxMenuRemover_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var perfil = ((MenuItem)sender).DataContext as PerfilSaldoModel;
                var result = MessageBox.Show("Deseja mesmo remover o perfil " + perfil.Nome + "?", "Atenção", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    var conn = BuzzOnConnection.Instance;
                    conn.DeleteAll<HistoricoSaldo>(i => i.IdPerfilSaldo == perfil.Id);
                    conn.Delete<PerfilSaldo>(i => i.Id == perfil.Id);

                    App.PerfisSaldoViewModel.CarregarColecaoDeDadosDaBase();
                    llsPerfisSaldo.ItemsSource = App.PerfisSaldoViewModel.ListaPerfisSaldo;

                    ucToastMessage.ShowToast("Perfil removido com sucesso", 4);
                }
            }
            catch { }
        }

        private void llsPerfisSaldo_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (App.PerfisSaldoViewModel.ListaPerfisSaldo.Any())
            {
                txtVazio.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}