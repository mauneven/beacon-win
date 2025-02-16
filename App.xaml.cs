using Microsoft.UI.Xaml;
using System;

namespace beacon
{
    public partial class App : Application
    {
        private Window? m_window;
        public static MainWindow? CurrentMainWindow { get; private set; }

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            CurrentMainWindow = m_window as MainWindow;
            m_window.Activate();
        }
    }
}