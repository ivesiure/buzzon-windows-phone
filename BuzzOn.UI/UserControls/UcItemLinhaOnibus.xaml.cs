using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BuzzOn.UI.Business;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BuzzOn.UI.Model.TableEntities;

namespace BuzzOn.UI.UserControls
{
    public partial class UcItemLinhaOnibus : UserControl
    {
        #region Delegate
        public delegate void ValueChangedEventHandler(object sender, String e);
        #endregion

        #region Events
        public event ValueChangedEventHandler FavoritarClicado;
        public event ValueChangedEventHandler DesfavoritarClicado;
        public event ValueChangedEventHandler MapaClicado;
        public event ValueChangedEventHandler ProximosHorariosClicado;
        public event ValueChangedEventHandler TabelaHorariosClicado;
        #endregion

        #region Dependency Properties

        #region LinhaOnibus
        public static readonly DependencyProperty LinhaProperty = DependencyProperty.Register("Linha",
                                                                                               typeof(object),
                                                                                               typeof(UcItemLinhaOnibus),
                                                                                               new PropertyMetadata(default(object),
                                                                                               LinhaPropertyChanged));

        private static void LinhaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ucLinhaOnibus = d as UcItemLinhaOnibus;
            var linha = e.NewValue as Linha;

            if (linha.Favorita)
            {
                var imgSource = "/Assets/Buttons/broken.heart.png";
                ucLinhaOnibus.imgFavoritos.Source = new BitmapImage(new Uri(imgSource, UriKind.RelativeOrAbsolute));
            }
            else
            {
                var imgSource = "/Assets/Buttons/like.png";
                ucLinhaOnibus.imgFavoritos.Source = new BitmapImage(new Uri(imgSource, UriKind.RelativeOrAbsolute));
            }

            ucLinhaOnibus.btnFavoritos.CommandParameter =
                ucLinhaOnibus.btnMapa.CommandParameter =
                    ucLinhaOnibus.btnProximosHorarios.CommandParameter =
                        ucLinhaOnibus.btnTabelaHorarios.CommandParameter = linha.Codigo;

            ucLinhaOnibus.txtCategoria.Text = linha.Categoria;

            ucLinhaOnibus.txtNome.Text = linha.NomeCompleto;

            ucLinhaOnibus.rtgCorpo.Fill =
                ucLinhaOnibus.rtgEsquerdo.Fill =
                    ucLinhaOnibus.cvnFavoritos.Background =
                        ucLinhaOnibus.cvnMapa.Background =
                            ucLinhaOnibus.cvnProximosHorarios.Background =
                                ucLinhaOnibus.cvnTabHorarios.Background = new SolidColorBrush(linha.ObjetoCor);
        }

        public object Linha
        {
            get { return (object)GetValue(LinhaProperty); }
            set { SetValue(LinhaProperty, value); }
        }
        #endregion

        #endregion

        public UcItemLinhaOnibus()
        {
            InitializeComponent();
        }

        #region Botão de Favoritar Linha
        private void btnFavoritos_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (FavoritarClicado != null)
            {
                var codigo = ((Button)sender).CommandParameter as String;
                FavoritarClicado(this, codigo);
            }
        }
        #endregion

        #region Botão de Tabela de Horários
        private void btnTabelaHorarios_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (TabelaHorariosClicado != null)
            {
                var codigo = ((Button)sender).CommandParameter as String;
                TabelaHorariosClicado(this, codigo);
            }
        }
        #endregion

        #region Botão de Próximos Horários
        private void btnProximosHorarios_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (ProximosHorariosClicado != null)
            {
                var codigo = ((Button)sender).CommandParameter as String;
                ProximosHorariosClicado(this, codigo);
            }
        }
        #endregion

        #region Botão de Mapa
        private void btnMapa_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (MapaClicado != null)
            {
                var codigo = ((Button)sender).CommandParameter as String;
                MapaClicado(this, codigo);
            }
        }

        #endregion

        #region Mudar botão de Favorito
        public void MudarAparenciaBotao()
        {
            var linha = this.Linha as Linha;
            if (linha.Favorita)
            {
                var imgSource = "/Assets/Buttons/broken.heart.png";
                this.imgFavoritos.Source = new BitmapImage(new Uri(imgSource, UriKind.RelativeOrAbsolute));
            }
            else
            {
                var imgSource = "/Assets/Buttons/like.png";
                this.imgFavoritos.Source = new BitmapImage(new Uri(imgSource, UriKind.RelativeOrAbsolute));
            }

        }
        #endregion
    }
}
