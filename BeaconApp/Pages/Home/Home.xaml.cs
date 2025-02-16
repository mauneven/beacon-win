using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.Storage;

namespace beacon.BeaconApp.Pages.Home
{
    public sealed partial class Home : Page
    {
        private DispatcherTimer timerCheckReminders;

        private DateTime nextDrinkWaterTime;
        DateTime nextStretchHandsTime;
        DateTime nextStretchLegsTime;
        DateTime nextRelaxEyesTime;
        DateTime nextSitProperlyTime;

        public Home()
        {
            this.InitializeComponent();
            this.Loaded += Home_Loaded;
        }

        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();

            UpdateTimerInterval(sliderDrinkWater, textDrinkWater, null);
            UpdateTimerInterval(sliderStretchHands, textStretchHands, null);
            UpdateTimerInterval(sliderStretchLegs, textStretchLegs, null);
            UpdateTimerInterval(sliderRelaxEyes, textRelaxEyes, null);
            UpdateTimerInterval(sliderSitProperly, textSitProperly, null);

            sliderDrinkWater.ValueChanged += (s, ev) =>
            {
                UpdateTimerInterval(sliderDrinkWater, textDrinkWater, null);
                ApplicationData.Current.LocalSettings.Values["sliderDrinkWater"] = (int)sliderDrinkWater.Value;
            };

            sliderStretchHands.ValueChanged += (s, ev) =>
            {
                UpdateTimerInterval(sliderStretchHands, textStretchHands, null);
                ApplicationData.Current.LocalSettings.Values["sliderStretchHands"] = (int)sliderStretchHands.Value;
            };

            sliderStretchLegs.ValueChanged += (s, ev) =>
            {
                UpdateTimerInterval(sliderStretchLegs, textStretchLegs, null);
                ApplicationData.Current.LocalSettings.Values["sliderStretchLegs"] = (int)sliderStretchLegs.Value;
            };

            sliderRelaxEyes.ValueChanged += (s, ev) =>
            {
                UpdateTimerInterval(sliderRelaxEyes, textRelaxEyes, null);
                ApplicationData.Current.LocalSettings.Values["sliderRelaxEyes"] = (int)sliderRelaxEyes.Value;
            };

            sliderSitProperly.ValueChanged += (s, ev) =>
            {
                UpdateTimerInterval(sliderSitProperly, textSitProperly, null);
                ApplicationData.Current.LocalSettings.Values["sliderSitProperly"] = (int)sliderSitProperly.Value;
            };

            chkDrinkWater.Checked += (s, ev) =>
            {
                if (toggleReminders.IsOn)
                    sliderDrinkWater.IsEnabled = true;
                ApplicationData.Current.LocalSettings.Values["chkDrinkWater"] = true;
            };
            chkDrinkWater.Unchecked += (s, ev) =>
            {
                sliderDrinkWater.IsEnabled = false;
                ApplicationData.Current.LocalSettings.Values["chkDrinkWater"] = false;
            };

            chkStretchHands.Checked += (s, ev) =>
            {
                if (toggleReminders.IsOn)
                    sliderStretchHands.IsEnabled = true;
                ApplicationData.Current.LocalSettings.Values["chkStretchHands"] = true;
            };
            chkStretchHands.Unchecked += (s, ev) =>
            {
                sliderStretchHands.IsEnabled = false;
                ApplicationData.Current.LocalSettings.Values["chkStretchHands"] = false;
            };

            chkStretchLegs.Checked += (s, ev) =>
            {
                if (toggleReminders.IsOn)
                    sliderStretchLegs.IsEnabled = true;
                ApplicationData.Current.LocalSettings.Values["chkStretchLegs"] = true;
            };
            chkStretchLegs.Unchecked += (s, ev) =>
            {
                sliderStretchLegs.IsEnabled = false;
                ApplicationData.Current.LocalSettings.Values["chkStretchLegs"] = false;
            };

            chkRelaxEyes.Checked += (s, ev) =>
            {
                if (toggleReminders.IsOn)
                    sliderRelaxEyes.IsEnabled = true;
                ApplicationData.Current.LocalSettings.Values["chkRelaxEyes"] = true;
            };
            chkRelaxEyes.Unchecked += (s, ev) =>
            {
                sliderRelaxEyes.IsEnabled = false;
                ApplicationData.Current.LocalSettings.Values["chkRelaxEyes"] = false;
            };

            chkSitProperly.Checked += (s, ev) =>
            {
                if (toggleReminders.IsOn)
                    sliderSitProperly.IsEnabled = true;
                ApplicationData.Current.LocalSettings.Values["chkSitProperly"] = true;
            };
            chkSitProperly.Unchecked += (s, ev) =>
            {
                sliderSitProperly.IsEnabled = false;
                ApplicationData.Current.LocalSettings.Values["chkSitProperly"] = false;
            };

            // Configurar el estado inicial de habilitación de los controles basado en el toggle general.
            sliderDrinkWater.IsEnabled = toggleReminders.IsOn && (chkDrinkWater.IsChecked == true);
            sliderStretchHands.IsEnabled = toggleReminders.IsOn && (chkStretchHands.IsChecked == true);
            sliderStretchLegs.IsEnabled = toggleReminders.IsOn && (chkStretchLegs.IsChecked == true);
            sliderRelaxEyes.IsEnabled = toggleReminders.IsOn && (chkRelaxEyes.IsChecked == true);
            sliderSitProperly.IsEnabled = toggleReminders.IsOn && (chkSitProperly.IsChecked == true);

            chkDrinkWater.IsEnabled = toggleReminders.IsOn;
            chkStretchHands.IsEnabled = toggleReminders.IsOn;
            chkStretchLegs.IsEnabled = toggleReminders.IsOn;
            chkRelaxEyes.IsEnabled = toggleReminders.IsOn;
            chkSitProperly.IsEnabled = toggleReminders.IsOn;

            ToggleReminders_Toggled(toggleReminders, null);

            StartTimers();
        }

        private void LoadSettings()
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            if (localSettings.Values["sliderDrinkWater"] != null)
                sliderDrinkWater.Value = Convert.ToDouble(localSettings.Values["sliderDrinkWater"]);
            if (localSettings.Values["sliderStretchHands"] != null)
                sliderStretchHands.Value = Convert.ToDouble(localSettings.Values["sliderStretchHands"]);
            if (localSettings.Values["sliderStretchLegs"] != null)
                sliderStretchLegs.Value = Convert.ToDouble(localSettings.Values["sliderStretchLegs"]);
            if (localSettings.Values["sliderRelaxEyes"] != null)
                sliderRelaxEyes.Value = Convert.ToDouble(localSettings.Values["sliderRelaxEyes"]);
            if (localSettings.Values["sliderSitProperly"] != null)
                sliderSitProperly.Value = Convert.ToDouble(localSettings.Values["sliderSitProperly"]);

            if (localSettings.Values["chkDrinkWater"] != null)
                chkDrinkWater.IsChecked = Convert.ToBoolean(localSettings.Values["chkDrinkWater"]);
            if (localSettings.Values["chkStretchHands"] != null)
                chkStretchHands.IsChecked = Convert.ToBoolean(localSettings.Values["chkStretchHands"]);
            if (localSettings.Values["chkStretchLegs"] != null)
                chkStretchLegs.IsChecked = Convert.ToBoolean(localSettings.Values["chkStretchLegs"]);
            if (localSettings.Values["chkRelaxEyes"] != null)
                chkRelaxEyes.IsChecked = Convert.ToBoolean(localSettings.Values["chkRelaxEyes"]);
            if (localSettings.Values["chkSitProperly"] != null)
                chkSitProperly.IsChecked = Convert.ToBoolean(localSettings.Values["chkSitProperly"]);

            if (localSettings.Values["toggleReminders"] != null)
                toggleReminders.IsOn = Convert.ToBoolean(localSettings.Values["toggleReminders"]);
            else
                toggleReminders.IsOn = true;
        }

        private void GoToSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(beacon.BeaconApp.Pages.Settings.Settings));
        }

        private void UpdateTimerInterval(Slider slider, TextBlock label, DispatcherTimer timer)
        {
            int minutes = (int)slider.Value;
            label.Text = $"{minutes} min";

            if (slider == sliderDrinkWater)
                nextDrinkWaterTime = DateTime.Now.AddMinutes(minutes);
            else if (slider == sliderStretchHands)
                nextStretchHandsTime = DateTime.Now.AddMinutes(minutes);
            else if (slider == sliderStretchLegs)
                nextStretchLegsTime = DateTime.Now.AddMinutes(minutes);
            else if (slider == sliderRelaxEyes)
                nextRelaxEyesTime = DateTime.Now.AddMinutes(minutes);
            else if (slider == sliderSitProperly)
                nextSitProperlyTime = DateTime.Now.AddMinutes(minutes);
        }

        private void StartTimers()
        {
            nextDrinkWaterTime = DateTime.Now.AddMinutes(sliderDrinkWater.Value);
            nextStretchHandsTime = DateTime.Now.AddMinutes(sliderStretchHands.Value);
            nextStretchLegsTime = DateTime.Now.AddMinutes(sliderStretchLegs.Value);
            nextRelaxEyesTime = DateTime.Now.AddMinutes(sliderRelaxEyes.Value);
            nextSitProperlyTime = DateTime.Now.AddMinutes(sliderSitProperly.Value);

            timerCheckReminders = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timerCheckReminders.Tick += TimerCheckReminders_Tick;
            timerCheckReminders.Start();
        }

        private void TimerCheckReminders_Tick(object sender, object e)
        {
            var now = DateTime.Now;

            if (toggleReminders.IsOn)
            {
                if (chkDrinkWater.IsChecked == true && now >= nextDrinkWaterTime)
                {
                    ShowNotification("Drink water");
                    nextDrinkWaterTime = now.AddMinutes(sliderDrinkWater.Value);
                }
                if (chkStretchHands.IsChecked == true && now >= nextStretchHandsTime)
                {
                    ShowNotification("Stretch your hands");
                    nextStretchHandsTime = now.AddMinutes(sliderStretchHands.Value);
                }
                if (chkStretchLegs.IsChecked == true && now >= nextStretchLegsTime)
                {
                    ShowNotification("Stretch your legs");
                    nextStretchLegsTime = now.AddMinutes(sliderStretchLegs.Value);
                }
                if (chkRelaxEyes.IsChecked == true && now >= nextRelaxEyesTime)
                {
                    ShowNotification("Relax your eyes");
                    nextRelaxEyesTime = now.AddMinutes(sliderRelaxEyes.Value);
                }
                if (chkSitProperly.IsChecked == true && now >= nextSitProperlyTime)
                {
                    ShowNotification("Sit properly");
                    nextSitProperlyTime = now.AddMinutes(sliderSitProperly.Value);
                }
            }
        }

        private void ToggleReminders_Toggled(object sender, RoutedEventArgs e)
        {
            bool overallEnabled = toggleReminders.IsOn;
            ApplicationData.Current.LocalSettings.Values["toggleReminders"] = overallEnabled;
            chkDrinkWater.IsEnabled = overallEnabled;
            chkStretchHands.IsEnabled = overallEnabled;
            chkStretchLegs.IsEnabled = overallEnabled;
            chkRelaxEyes.IsEnabled = overallEnabled;
            chkSitProperly.IsEnabled = overallEnabled;
            sliderDrinkWater.IsEnabled = overallEnabled && (chkDrinkWater.IsChecked == true);
            sliderStretchHands.IsEnabled = overallEnabled && (chkStretchHands.IsChecked == true);
            sliderStretchLegs.IsEnabled = overallEnabled && (chkStretchLegs.IsChecked == true);
            sliderRelaxEyes.IsEnabled = overallEnabled && (chkRelaxEyes.IsChecked == true);
            sliderSitProperly.IsEnabled = overallEnabled && (chkSitProperly.IsChecked == true);
        }

        private void ShowNotification(string message)
        {
            ToastTemplateType templateType = ToastTemplateType.ToastText01;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(templateType);
            var textNodes = toastXml.GetElementsByTagName("text");
            textNodes[0].AppendChild(toastXml.CreateTextNode(message));
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
