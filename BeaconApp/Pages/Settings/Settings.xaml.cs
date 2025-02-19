using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;
using System.Globalization;

namespace beacon.BeaconApp.Pages.Settings
{
    public sealed partial class Settings : Page
    {
        private ComboBoxItem _previousSelectedItem;

        public Settings()
        {
            Windows.ApplicationModel.Resources.Core.ResourceManager.Current.DefaultContext.Reset();
            this.InitializeComponent();

            string currentLanguage = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride;
            if (string.IsNullOrEmpty(currentLanguage))
            {
                currentLanguage = CultureInfo.CurrentUICulture.Name;
            }

            string langText;
            switch (currentLanguage)
            {
                case "en-US":
                case "en":
                    langText = "English";
                    break;
                case "es-ES":
                case "es":
                    langText = "Español";
                    break;
                case "fr-FR":
                case "fr":
                    langText = "Français";
                    break;
                default:
                    langText = CultureInfo.CurrentUICulture.DisplayName;
                    break;
            }
            languageComboBox.PlaceholderText = langText;

            languageComboBox.SelectionChanged -= LanguageComboBox_SelectionChanged;

            foreach (ComboBoxItem item in languageComboBox.Items)
            {
                if (item.Tag?.ToString() == currentLanguage ||
                    (currentLanguage.StartsWith("en") && item.Tag?.ToString() == "en-US") ||
                    (currentLanguage.StartsWith("es") && item.Tag?.ToString() == "es-ES") ||
                    (currentLanguage.StartsWith("fr") && item.Tag?.ToString() == "fr-FR"))
                {
                    languageComboBox.SelectedItem = item;
                    _previousSelectedItem = item;
                    break;
                }
            }

            languageComboBox.SelectionChanged += LanguageComboBox_SelectionChanged;

            if (ApplicationData.Current.LocalSettings.Values["LaunchMinimized"] is bool launchMinimized)
            {
                toggleLaunchMinimized.IsOn = launchMinimized;
            }
        }

        private async void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (selectedItem == _previousSelectedItem)
                    return;

                string langTag = selectedItem.Tag as string;

                var loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();
                string dialogTitle = loader.GetString("ChangeLangTitle");
                string dialogContent = loader.GetString("ChangeLangContent");
                string dialogYes = loader.GetString("ChangeLangYes");
                string dialogCancel = loader.GetString("ChangeLangCancel");

                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = dialogTitle,
                    Content = dialogContent,
                    PrimaryButtonText = dialogYes,
                    CloseButtonText = dialogCancel,
                    DefaultButton = ContentDialogButton.Primary
                };

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    _previousSelectedItem = selectedItem;
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = langTag;
                    Windows.ApplicationModel.Resources.Core.ResourceManager.Current.DefaultContext.Reset();
                    if (App.CurrentMainWindow is not null)
                    {
                        Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
                    }
                }
                else
                {
                    comboBox.SelectionChanged -= LanguageComboBox_SelectionChanged;
                    comboBox.SelectedItem = _previousSelectedItem;
                    comboBox.SelectionChanged += LanguageComboBox_SelectionChanged;
                }
            }
        }

        private void ToggleLaunchMinimized_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggle)
            {
                bool launchMinimized = toggle.IsOn;
                ApplicationData.Current.LocalSettings.Values["LaunchMinimized"] = launchMinimized;
            }
        }

        private void GoToMain_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(beacon.BeaconApp.Pages.Home.Home));
        }
    }
}
