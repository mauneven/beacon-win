using Microsoft.UI.Xaml;
using Windows.Storage;

namespace beacon
{
    public partial class App : Application
    {
        private Window? m_window;
        public static MainWindow? CurrentMainWindow { get; private set; }
        public static TrayIconManager? TrayIconManagerInstance { get; private set; }

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.ExtendsContentIntoTitleBar = true;
            CurrentMainWindow = m_window as MainWindow;

            TrayIconManagerInstance = new TrayIconManager(m_window);

            bool launchMinimized = false;
            if (ApplicationData.Current.LocalSettings.Values["LaunchMinimized"] is bool setting)
            {
                launchMinimized = setting;
            }

            // Si no se recibió ningún argumento, se asume un lanzamiento manual
            bool isManualLaunch = string.IsNullOrEmpty(args.Arguments);
            if (!launchMinimized || isManualLaunch)
            {
                m_window.Activate();
            }
        }
    }
}
