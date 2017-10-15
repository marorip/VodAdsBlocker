using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.Win32;

namespace VodAdsBlocker
{
    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();

            Loaded += Window_Loaded;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;   
            this.Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (registryKey == null)
                return;

            cbAutostart.IsChecked = registryKey.GetValue("VodAdsBlocker") != null;
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            RegisterInStartup(true);
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            RegisterInStartup(false);
        }

        private void RegisterInStartup(bool isChecked)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (registryKey == null)
                return;

            if (isChecked)
            {
                registryKey.SetValue("VodAdsBlocker", System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                registryKey.DeleteValue("VodAdsBlocker");
            }
        }
    }
}
