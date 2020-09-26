using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BuzzOn.UI.Utils
{
    public class ColorUtil
    {
        private static Regex _hexColorMatchRegex = new Regex("^#?(?<a>[a-z0-9][a-z0-9])?(?<r>[a-z0-9][a-z0-9])(?<g>[a-z0-9][a-z0-9])(?<b>[a-z0-9][a-z0-9])$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static Color GetColorFromHex(String hexColorString)
        {
            if (String.IsNullOrEmpty(hexColorString) == false)
            {
                if (hexColorString.Contains("#") == false)
                    hexColorString = "#" + hexColorString;

                if (hexColorString == null)
                    throw new NullReferenceException("Hex string can't be null.");

                // Regex match the string
                var match = _hexColorMatchRegex.Match(hexColorString);

                if (!match.Success)
                    throw new InvalidCastException("Fail converting the color " + hexColorString);

                // a value is optional
                byte a = 255, r = 0, b = 0, g = 0;
                if (match.Groups["a"].Success)
                    a = Convert.ToByte(match.Groups["a"].Value, 16);
                // r,b,g values are not optional
                r = Convert.ToByte(match.Groups["r"].Value, 16);
                b = Convert.ToByte(match.Groups["b"].Value, 16);
                g = Convert.ToByte(match.Groups["g"].Value, 16);

                return Color.FromArgb(a, r, g, b);
            }

            var corTemaCelular = (Color)Application.Current.Resources["PhoneAccentColor"];
            if (corTemaCelular != null)
            {
                return corTemaCelular;
            }

            return Colors.Cyan;
        }

        public static String GetHexFromColor(Color c)
        {
            if (c != null)
                return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");

            return null;
        }
    }
}
