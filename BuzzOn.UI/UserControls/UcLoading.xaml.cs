using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BuzzOn.UI.UserControls
{
    public partial class UcLoading : UserControl
    {
        public UcLoading()
        {
            InitializeComponent();
        }

        public void Hide()
        {
            this.Visibility = LayoutRoot.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void Show(string message = null)
        {
            this.Visibility = LayoutRoot.Visibility = System.Windows.Visibility.Visible;
            if (string.IsNullOrEmpty(message) == false)
            {
                txtMensagem.Text = message;
            }
        }
    }
}
