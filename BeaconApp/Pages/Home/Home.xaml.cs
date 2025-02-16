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
        private DispatcherTimer timerDrinkWater;
        private DispatcherTimer timerStretchHands;
        private DispatcherTimer timerStretchLegs;
        private DispatcherTimer timerRelaxEyes;
        private DispatcherTimer timerSitProperly;

        public Home()
        {
            this.InitializeComponent();
            this.Loaded += Home_Loaded;
        }

        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
            // Load settings after controls are created.
            LoadSettings();

            // Update slider labels based on loaded values.
            UpdateTimerInterval(sliderDrinkWater, textDrinkWater, null);
            UpdateTimerInterval(sliderStretchHands, textStretchHands, null);
            UpdateTimerInterval(sliderStretchLegs, textStretchLegs, null);
            UpdateTimerInterval(sliderRelaxEyes, textRelaxEyes, null);
            UpdateTimerInterval(sliderSitProperly, textSitProperly, null);

            // Wire up slider value changed events.
            sliderDrinkWater.ValueChanged += (s, ev) =>
            {
                UpdateTimerInterval(sliderDrinkWater, textDrinkWater, timerDrinkWater);
                ApplicationData.Current.LocalSettings.Values["sliderDrinkWater"] = (int)sliderDrinkWater.Value;
            };

            sliderStretchHands.ValueChanged += (s, ev) =>
            {
                UpdateTimerInterval(sliderStretchHands, textStretchHands, timerStretchHands);
                ApplicationData.Current.LocalSettings.Values["sliderStretchHands"] = (int)sliderStretchHands.Value;
            };

            sliderStretchLegs.ValueChanged += (s, ev) =>
            {
                UpdateTimerInterval(sliderStretchLegs, textStretchLegs, timerStretchLegs);
                ApplicationData.Current.LocalSettings.Values["sliderStretchLegs"] = (int)sliderStretchLegs.Value;
            };

            sliderRelaxEyes.ValueChanged += (s, ev) =>
            {
                UpdateTimerInterval(sliderRelaxEyes, textRelaxEyes, timerRelaxEyes);
                ApplicationData.Current.LocalSettings.Values["sliderRelaxEyes"] = (int)sliderRelaxEyes.Value;
            };

            sliderSitProperly.ValueChanged += (s, ev) =>
            {
                UpdateTimerInterval(sliderSitProperly, textSitProperly, timerSitProperly);
                ApplicationData.Current.LocalSettings.Values["sliderSitProperly"] = (int)sliderSitProperly.Value;
            };

            // Wire up checkbox events and persist state.
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

            // Set initial enabled state based on the toggle switch.
            sliderDrinkWater.IsEnabled = toggleReminders.IsOn && (chkDrinkWater.IsChecked == true);
            sliderStretchHands.IsEnabled = toggleReminders.IsOn && (chkStretchHands.IsChecked == true);
            sliderStretchLegs.IsEnabled = toggleReminders.IsOn && (chkStretchLegs.IsChecked == true);
            sliderRelaxEyes.IsEnabled = toggleReminders.IsOn && (chkRelaxEyes.IsChecked == true);
            sliderSitProperly.IsEnabled = toggleReminders.IsOn && (chkSitProperly.IsChecked == true);

            // Ensure checkboxes are enabled based on the overall toggle.
            chkDrinkWater.IsEnabled = toggleReminders.IsOn;
            chkStretchHands.IsEnabled = toggleReminders.IsOn;
            chkStretchLegs.IsEnabled = toggleReminders.IsOn;
            chkRelaxEyes.IsEnabled = toggleReminders.IsOn;
            chkSitProperly.IsEnabled = toggleReminders.IsOn;

            // Apply current overall toggle state.
            ToggleReminders_Toggled(toggleReminders, null);

            // Start timers.
            StartTimers();
        }

        private void LoadSettings()
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            if (localSettings.Values["sliderDrinkWater"] != null)
            {
                sliderDrinkWater.Value = Convert.ToDouble(localSettings.Values["sliderDrinkWater"]);
            }
            if (localSettings.Values["sliderStretchHands"] != null)
            {
                sliderStretchHands.Value = Convert.ToDouble(localSettings.Values["sliderStretchHands"]);
            }
            if (localSettings.Values["sliderStretchLegs"] != null)
            {
                sliderStretchLegs.Value = Convert.ToDouble(localSettings.Values["sliderStretchLegs"]);
            }
            if (localSettings.Values["sliderRelaxEyes"] != null)
            {
                sliderRelaxEyes.Value = Convert.ToDouble(localSettings.Values["sliderRelaxEyes"]);
            }
            if (localSettings.Values["sliderSitProperly"] != null)
            {
                sliderSitProperly.Value = Convert.ToDouble(localSettings.Values["sliderSitProperly"]);
            }

            // Load checkbox states.
            if (localSettings.Values["chkDrinkWater"] != null)
            {
                chkDrinkWater.IsChecked = Convert.ToBoolean(localSettings.Values["chkDrinkWater"]);
            }
            if (localSettings.Values["chkStretchHands"] != null)
            {
                chkStretchHands.IsChecked = Convert.ToBoolean(localSettings.Values["chkStretchHands"]);
            }
            if (localSettings.Values["chkStretchLegs"] != null)
            {
                chkStretchLegs.IsChecked = Convert.ToBoolean(localSettings.Values["chkStretchLegs"]);
            }
            if (localSettings.Values["chkRelaxEyes"] != null)
            {
                chkRelaxEyes.IsChecked = Convert.ToBoolean(localSettings.Values["chkRelaxEyes"]);
            }
            if (localSettings.Values["chkSitProperly"] != null)
            {
                chkSitProperly.IsChecked = Convert.ToBoolean(localSettings.Values["chkSitProperly"]);
            }

            // Load overall toggle state, defaulting to true.
            if (localSettings.Values["toggleReminders"] != null)
            {
                toggleReminders.IsOn = Convert.ToBoolean(localSettings.Values["toggleReminders"]);
            }
            else
            {
                toggleReminders.IsOn = true;
            }
        }

        private void GoToSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(beacon.BeaconApp.Pages.Settings.Settings));
        }

        private void UpdateTimerInterval(Slider slider, TextBlock label, DispatcherTimer timer)
        {
            int minutes = (int)slider.Value;
            label.Text = $"{minutes} min";
            if (timer != null)
            {
                timer.Interval = TimeSpan.FromMinutes(minutes);
            }
        }

        private void StartTimers()
        {
            // Timer for Drink water.
            timerDrinkWater = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes((int)sliderDrinkWater.Value)
            };
            timerDrinkWater.Tick += (s, e) =>
            {
                if (chkDrinkWater.IsChecked == true)
                    ShowNotification("Drink water");
            };
            timerDrinkWater.Start();

            // Timer for Stretch your hands.
            timerStretchHands = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes((int)sliderStretchHands.Value)
            };
            timerStretchHands.Tick += (s, e) =>
            {
                if (chkStretchHands.IsChecked == true)
                    ShowNotification("Stretch your hands");
            };
            timerStretchHands.Start();

            // Timer for Stretch your legs.
            timerStretchLegs = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes((int)sliderStretchLegs.Value)
            };
            timerStretchLegs.Tick += (s, e) =>
            {
                if (chkStretchLegs.IsChecked == true)
                    ShowNotification("Stretch your legs");
            };
            timerStretchLegs.Start();

            // Timer for Relax your eyes.
            timerRelaxEyes = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes((int)sliderRelaxEyes.Value)
            };
            timerRelaxEyes.Tick += (s, e) =>
            {
                if (chkRelaxEyes.IsChecked == true)
                    ShowNotification("Relax your eyes");
            };
            timerRelaxEyes.Start();

            // Timer for Sit properly.
            timerSitProperly = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes((int)sliderSitProperly.Value)
            };
            timerSitProperly.Tick += (s, e) =>
            {
                if (chkSitProperly.IsChecked == true)
                    ShowNotification("Sit properly");
            };
            timerSitProperly.Start();
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