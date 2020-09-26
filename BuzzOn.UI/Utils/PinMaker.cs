using BuzzOn.UI.Model;
using Microsoft.Phone.Maps.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BuzzOn.UI.Utils
{
    public class PinMaker
    {
        public static object PontoDeOnibus(PontoModel ponto)
        {
            var myImage1 = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/Buzzon/buzzon_stop.png", UriKind.RelativeOrAbsolute));
            var image = new Image() { Width = 50, Height = 50, Opacity = 100, Source = myImage1 };
            return image;
        }

        public static object Onibus(Color color, String hora)
        {
            var myImage1 = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/Buzzon/buzzon_bus.png", UriKind.RelativeOrAbsolute));
            var image = new Image()
            {
                Width = 26,
                Height = 26,
                Opacity = 100,
                Source = myImage1
            };
            image.Tap += (sender, e) => { MessageBox.Show("Última atualização\n\n" + hora); };

            Rectangle ret = new Rectangle()
            {
                Width = 26,
                Height = 26,
                RadiusX = 5,
                RadiusY = 5,
                Stroke = new SolidColorBrush(Colors.White),
                Fill = new SolidColorBrush(color)
            };

            Canvas canvas = new Canvas();
            canvas.Children.Add(ret);
            canvas.Children.Add(image);
            canvas.Width = 30;
            canvas.Height = 30;

            return canvas;
        }

        public static object Usuario()
        {
            var myImage1 = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/Buzzon/buzzon_my_location.png", UriKind.RelativeOrAbsolute));
            var image = new Image() { Width = 30, Height = 30, Opacity = 100, Source = myImage1 };
            image.Tap += (sender, e) => { MessageBox.Show("Esse é você! :D"); };
            return image;
        }
    }
}
