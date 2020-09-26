using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Threading;

namespace BuzzOn.UI.UserControls
{
    public partial class UcToastMessage : UserControl
    {
        private DispatcherTimer timer;

        public UcToastMessage()
        {
            InitializeComponent();
            this.timer = new DispatcherTimer();
        }

        #region ToastMessage
        public void ShowToast(String pMensagem, Int32 pSegundos = 0)
        {
            lblToast.Text = pMensagem;
            panelLayoutRoot.Visibility = Visibility.Visible;

            timer.Interval = pSegundos == 0 ? new TimeSpan(0, 0, 5) : new TimeSpan(0, 0, pSegundos);
            timer.Start();
            timer.Tick += timer_Tick;
        }

        void timer_Tick(object sender, object e)
        {
            lblToast.Text = "";
            panelLayoutRoot.Visibility = Visibility.Collapsed;
            timer.Stop();
        }
        #endregion

    }
}
