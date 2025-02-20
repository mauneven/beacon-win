using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.Storage;

namespace beacon.BeaconApp.Pages.Home
{
    public sealed partial class Home : Page
    {
        private DispatcherTimer timerCheckReminders;
        private List<Reminder> reminders;
        private int _counter;

        private class Reminder
        {
            public Slider Slider { get; set; }
            public TextBlock Label { get; set; }
            public CheckBox CheckBox { get; set; }
            public int NextTimeSeconds { get; set; }
            public string SliderSettingKey { get; set; }
            public string CheckSettingKey { get; set; }
            public string ResourceKey { get; set; }
        }

        public Home()
        {
            this.InitializeComponent();
            this.Loaded += Home_Loaded;
        }

        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();

            reminders = new List<Reminder>
            {
                new Reminder{ Slider = sliderDrinkWater, Label = textDrinkWater, CheckBox = chkDrinkWater, SliderSettingKey="sliderDrinkWater", CheckSettingKey="chkDrinkWater", ResourceKey="DrinkWater" },
                new Reminder{ Slider = sliderStretchHands, Label = textStretchHands, CheckBox = chkStretchHands, SliderSettingKey="sliderStretchHands", CheckSettingKey="chkStretchHands", ResourceKey="StretchHands" },
                new Reminder{ Slider = sliderStretchLegs, Label = textStretchLegs, CheckBox = chkStretchLegs, SliderSettingKey="sliderStretchLegs", CheckSettingKey="chkStretchLegs", ResourceKey="StretchLegs" },
                new Reminder{ Slider = sliderRelaxEyes, Label = textRelaxEyes, CheckBox = chkRelaxEyes, SliderSettingKey="sliderRelaxEyes", CheckSettingKey="chkRelaxEyes", ResourceKey="RelaxEyes" },
                new Reminder{ Slider = sliderSitProperly, Label = textSitProperly, CheckBox = chkSitProperly, SliderSettingKey="sliderSitProperly", CheckSettingKey="chkSitProperly", ResourceKey="SitProperly" }
            };

            foreach (var reminder in reminders)
            {
                UpdateReminder(reminder);

                reminder.Slider.ValueChanged += (s, ev) =>
                {
                    UpdateReminder(reminder);
                    ApplicationData.Current.LocalSettings.Values[reminder.SliderSettingKey] = (int)reminder.Slider.Value;
                };

                reminder.CheckBox.Checked += (s, ev) =>
                {
                    reminder.Slider.IsEnabled = toggleReminders.IsOn;
                    UpdateReminder(reminder);
                    ApplicationData.Current.LocalSettings.Values[reminder.CheckSettingKey] = true;
                };

                reminder.CheckBox.Unchecked += (s, ev) =>
                {
                    reminder.Slider.IsEnabled = false;
                    ApplicationData.Current.LocalSettings.Values[reminder.CheckSettingKey] = false;
                };
            }

            toggleReminders.Toggled += ToggleReminders_Toggled;
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

        private void UpdateReminder(Reminder reminder)
        {
            int minutes = (int)reminder.Slider.Value;
            reminder.Label.Text = $"{minutes} min";
            reminder.NextTimeSeconds = _counter + (minutes >= 1 ? minutes * 60 : int.MaxValue);
        }

        private void StartTimers()
        {
            _counter = 0;
            foreach (var reminder in reminders)
            {
                int minutes = (int)reminder.Slider.Value;
                reminder.NextTimeSeconds = minutes >= 1 ? _counter + minutes * 60 : int.MaxValue;
            }
            timerCheckReminders = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timerCheckReminders.Tick += TimerCheckReminders_Tick;
            timerCheckReminders.Start();
        }

        private void TimerCheckReminders_Tick(object sender, object e)
        {
            _counter++;
            if (toggleReminders.IsOn)
            {
                foreach (var reminder in reminders)
                {
                    if (reminder.CheckBox.IsChecked == true && _counter >= reminder.NextTimeSeconds)
                    {
                        ShowNotification(reminder.ResourceKey);
                        int minutes = (int)reminder.Slider.Value;
                        reminder.NextTimeSeconds = minutes >= 1 ? _counter + minutes * 60 : int.MaxValue;
                    }
                }
            }
        }

        private void ShowNotification(string resourceKey)
        {
            var loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();
            string message = loader.GetString($"{resourceKey}/Text");
            if (string.IsNullOrEmpty(message))
                message = resourceKey;

            ToastTemplateType templateType = ToastTemplateType.ToastText01;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(templateType);
            var textNodes = toastXml.GetElementsByTagName("text");
            textNodes[0].AppendChild(toastXml.CreateTextNode(message));
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void ToggleReminders_Toggled(object sender, RoutedEventArgs e)
        {
            if (toggleReminders == null)
                return;

            bool overallEnabled = toggleReminders.IsOn;
            ApplicationData.Current.LocalSettings.Values["toggleReminders"] = overallEnabled;

            if (reminders == null)
                return;

            foreach (var reminder in reminders)
            {
                if (reminder?.CheckBox != null)
                {
                    reminder.CheckBox.IsEnabled = overallEnabled;
                    if (reminder.Slider != null)
                    {
                        reminder.Slider.IsEnabled = overallEnabled && (reminder.CheckBox.IsChecked == true);
                        if (overallEnabled && reminder.CheckBox.IsChecked == true)
                        {
                            int minutes = (int)reminder.Slider.Value;
                            reminder.NextTimeSeconds = _counter + minutes * 60;
                        }
                    }
                }
            }
        }
    }
}
