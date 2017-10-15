using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using VodAdsBlocker.Modules;

namespace VodAdsBlocker
{
    public partial class MainWindow : IDisposable
    {
        private readonly NotifyIcon _trayIcon;
        private readonly ContextMenu _trayMenu;
        private readonly AdBlocker _adBlocker;

        public MainWindow()
        {
            InitializeComponent();

            var settingsWindow = new SettingsWindow();

            _adBlocker = new AdBlocker();
            _adBlocker.OnStarted += OnStarted;
            _adBlocker.OnStopped += OnStopped;

            _trayMenu = new ContextMenu();
            _trayMenu.MenuItems.Add("Włącz blokowanie", (o, args) => _adBlocker?.Start());
            _trayMenu.MenuItems.Add("Wyłącz blokowanie", (o, args) => _adBlocker?.Stop());
            _trayMenu.MenuItems.Add("Ustawienia", (o, args) => settingsWindow.ShowDialog());
            _trayMenu.MenuItems.Add("-");
            _trayMenu.MenuItems.Add("Zamknij", OnExit);

            _trayMenu.MenuItems[1].Visible = false;

            _trayIcon = new NotifyIcon();
            _trayIcon.Text = "VOD AdBlocker";
            _trayIcon.Icon = new Icon(Properties.Resources.off, 40, 40);

            _trayIcon.ContextMenu = _trayMenu;
            _trayIcon.Visible = true;
        }

        private void OnStarted(object sender, EventArgs eventArgs)
        {
            _trayIcon.Icon = new Icon(Properties.Resources.on, 40, 40);
            _trayMenu.MenuItems[0].Visible = false;
            _trayMenu.MenuItems[1].Visible = true;
        }

        private void OnStopped(object sender, EventArgs eventArgs)
        {
            _trayIcon.Icon = new Icon(Properties.Resources.off, 40, 40);
            _trayMenu.MenuItems[0].Visible = true;
            _trayMenu.MenuItems[1].Visible = false;
        }


        protected override void OnInitialized(EventArgs e)
        {
            Visibility = Visibility.Hidden;
            ShowInTaskbar = false;

            base.OnInitialized(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            _adBlocker?.Dispose();
            _trayIcon?.Dispose();
        }

        private void OnExit(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _adBlocker?.Dispose();
                    _trayIcon?.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
