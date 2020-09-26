using BuzzOn.UI.Facade;
using BuzzOn.UI.Utils;
using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BuzzOn.UI.ViewModels
{
    public delegate void CarregamentoCompletoEventHandler(object sender, EventArgs e);

    public class ItinerarioViewModel
    {
        public MapLayer LayerPontosOnibus { get; set; }
        public MapPolyline ShapeRota { get; set; }
        public LocationRectangle LayoutRota { get; set; }
        
        public event CarregamentoCompletoEventHandler CarregamentoCompleto;

        public void CarregarDados(Int32 pLinhaId)
        {
            this.LayerPontosOnibus = new MapLayer();
            this.ShapeRota = new MapPolyline();

            App.LinhaSelecionada = LinhaFacade.Instance.ObterPorId(pLinhaId);

            #region Shape da linha

            var listaRotas = RotaFacade.Instance.ListarRotaPorLinha(App.LinhaSelecionada.Codigo);
            
            var CoordenadasRota = new GeoCoordinateCollection();
            var listaCoordenadas = listaRotas.Select(rota => new GeoCoordinate(Convert.ToDouble(rota.Latitude), Convert.ToDouble(rota.Longitude))).ToList();
            foreach (var item in listaCoordenadas)
            {
                CoordenadasRota.Add(item);
            }
            ShapeRota.StrokeColor = App.LinhaSelecionada.ObjetoCor;
            ShapeRota.StrokeThickness = 4;
            ShapeRota.Path = CoordenadasRota;

            LayoutRota = LocationRectangle.CreateBoundingRectangle(CoordenadasRota);
            #endregion

            #region Pontos da linha
            //Desenha os pontos da linha no layer 

            var listaPontos = PontoFacade.Instance.ListarPontosPorLinha(App.LinhaSelecionada.Codigo);
            foreach (var ponto in listaPontos)
            {
                double latitude = 0, longitude = 0;

                Double.TryParse(ponto.Latitude, out latitude);
                Double.TryParse(ponto.Longitude, out longitude);

                var coordenada = new GeoCoordinate(latitude, longitude);

                var imgPontoOnibus = PinMaker.PontoDeOnibus(ponto) as Image;
                imgPontoOnibus.Tap += (sender, e) =>
                {
                    MessageBox.Show(ponto.Nome);
                };

                var mapOver = new MapOverlay();
                mapOver.GeoCoordinate = coordenada;
                mapOver.Content = imgPontoOnibus;
                mapOver.PositionOrigin = new Point(0.5, 1);

                LayerPontosOnibus.Add(mapOver);
            }
            #endregion

            if (CarregamentoCompleto != null)
            {
                CarregamentoCompleto(this, null);
            }
        }
    }
}
