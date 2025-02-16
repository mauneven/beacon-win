using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.Core;

namespace beacon.BeaconApp.Pages.Settings
{
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            Windows.ApplicationModel.Resources.Core.ResourceManager.Current.DefaultContext.Reset();
            this.InitializeComponent();
        }

        private void ChangeLanguage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = button.Tag as string;
            Windows.ApplicationModel.Resources.Core.ResourceManager.Current.DefaultContext.Reset();

            if (App.CurrentMainWindow is not null)
            {
                Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
            }
        }

        private void GoToMain_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the home page using its fully-qualified type.
            this.Frame.Navigate(typeof(beacon.BeaconApp.Pages.Home.Home));
        }
    }
}