using System;
using System.Linq;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BuzzOn.UI.Resources;
using BuzzOn.UI.Model;
using BuzzOn.UI.ViewModels;
using System.Device.Location;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using System.Windows.Media;
using BuzzOn.UI.Facade;
using BuzzOn.UI.Data;
using Windows.Storage;
using System.IO;
using System.Threading.Tasks;
using SQLite;

namespace BuzzOn.UI
{
    public partial class App : Application
    {
        public static string DB_PATH { get { return Path.Combine(ApplicationData.Current.LocalFolder.Path, "buzzon.sqlite"); } }

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        private static LinhaOnibusViewModel _linhaOnibusViewModel;
        public static LinhaOnibusViewModel LinhaOnibusViewModel { get { return _linhaOnibusViewModel; } }

        private static ItinerarioViewModel _itinerarioViewModel;
        public static ItinerarioViewModel ItinerarioViewModel { get { return _itinerarioViewModel; } }

        private static LinhaModel _linhaSelecionada;
        public static LinhaModel LinhaSelecionada
        {
            get { return _linhaSelecionada; }
            set { _linhaSelecionada = value; }
        }

        private static PerfilSaldoViewModel _perfilSaldoViewModel;
        public static PerfilSaldoViewModel PerfisSaldoViewModel { get { return _perfilSaldoViewModel; } }

        public static String UserId
        {
            get
            {
                var userId = ParametroFacade.Instance.BuscarPorNome("UserId");
                return userId != null ? userId.Valor : null;
            }
        }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Language display initialization
            InitializeLanguage();

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            try
            {
                var connection = new BuzzOnConnection();
                connection.OnCreate<Linha>();
                connection.OnCreate<Horario>();
                connection.OnCreate<Ponto>();
                connection.OnCreate<Rota>();
                connection.OnCreate<Versao>();
                connection.OnCreate<Parametro>();
                connection.OnCreate<PerfilSaldo>();
                connection.OnCreate<HistoricoSaldo>();
            }
            catch { }

            _itinerarioViewModel = new ItinerarioViewModel();
            _linhaOnibusViewModel = new LinhaOnibusViewModel();
            _perfilSaldoViewModel = new PerfilSaldoViewModel();

            LinhaOnibusViewModel.CarregarColecaoDeDadosDaBase();
            PerfisSaldoViewModel.CarregarColecaoDeDadosDaBase();

            RootFrame.Navigating += RootFrame_Navigating;
        }

        private void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri.ToString().Contains("/MainPage.xaml") != true)
                return;

            if (BuzzOnConnection.Instance.Any<Linha>() == false)
            {
                e.Cancel = true;
                RootFrame.Dispatcher.BeginInvoke(delegate
                {
                    RootFrame.Navigate(new Uri("/Welcome.xaml", UriKind.Relative));
                });
            }
        }

        // Code to execute when a contract activation such as a file open or save picker returns 
        // with the picked file or other return values
        private void Application_ContractActivated(object sender, Windows.ApplicationModel.Activation.IActivatedEventArgs e) { }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e) { }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e) { }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e) { }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e) { }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            //RootFrame = new PhoneApplicationFrame();
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Handle contract activation such as a file open or save picker
            PhoneApplicationService.Current.ContractActivated += Application_ContractActivated;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}