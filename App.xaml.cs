using Microsoft.UI.Xaml;
using Windows.Storage;
using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace beacon
{
    public partial class App : Application
    {
        private Window? m_window;
        private static Mutex? _mutex;
        public static MainWindow? CurrentMainWindow { get; private set; }
        public static TrayIconManager? TrayIconManagerInstance { get; private set; }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            bool createdNew;
            _mutex = new Mutex(true, "BeaconSingleInstanceMutex", out createdNew);
            if (!createdNew)
            {
                IntPtr hWnd = FindWindow(null, "Beacon App");
                if (hWnd != IntPtr.Zero)
                {
                    SetForegroundWindow(hWnd);
                }
                Application.Current.Exit();
                return;
            }

            m_window = new MainWindow();
            m_window.ExtendsContentIntoTitleBar = true;
            CurrentMainWindow = m_window as MainWindow;

            TrayIconManagerInstance = new TrayIconManager(m_window);

            bool launchMinimized = false;
            if (ApplicationData.Current.LocalSettings.Values["LaunchMinimized"] is bool setting)
            {
                launchMinimized = setting;
            }

            bool isManualLaunch = string.IsNullOrEmpty(args.Arguments);
            if (!launchMinimized || isManualLaunch)
            {
                m_window.Activate();
            }
        }
    }
}
