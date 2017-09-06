using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32;
using Application = System.Windows.Forms.Application;

namespace VodAdsBlocker
{
    public partial class MainWindow : IDisposable
    {
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private AdBlocker adBlocker;

        private SettingsWindow settingsWindow;

        public MainWindow()
        {
            InitializeComponent();

            settingsWindow = new SettingsWindow();

            adBlocker = new AdBlocker();
            adBlocker.OnStarted += OnStarted;
            adBlocker.OnStopped += OnStopped;

            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Włącz blokowanie", (o, args) => adBlocker?.Start());
            trayMenu.MenuItems.Add("Wyłącz blokowanie", (o, args) => adBlocker?.Stop());
            trayMenu.MenuItems.Add("Ustawienia", (o, args) => settingsWindow.ShowDialog());
            trayMenu.MenuItems.Add("-");
            trayMenu.MenuItems.Add("Zamknij", OnExit);

            trayMenu.MenuItems[1].Visible = false;

            trayIcon = new NotifyIcon();
            trayIcon.Text = "VOD AdBlocker";
            trayIcon.Icon = new Icon(Properties.Resources.off, 40, 40);

            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }

        private void OnStarted(object sender, EventArgs eventArgs)
        {
            trayIcon.Icon = new Icon(Properties.Resources.on, 40, 40);
            trayMenu.MenuItems[0].Visible = false;
            trayMenu.MenuItems[1].Visible = true;
        }

        private void OnStopped(object sender, EventArgs eventArgs)
        {
            trayIcon.Icon = new Icon(Properties.Resources.off, 40, 40);
            trayMenu.MenuItems[0].Visible = true;
            trayMenu.MenuItems[1].Visible = false;
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

            adBlocker?.Dispose();
            trayIcon?.Dispose();
        }

        private void OnExit(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private bool disposed;

        ~MainWindow()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    adBlocker?.Dispose();
                    trayIcon?.Dispose();
                }

                disposed = true;
            }
        }
    }
}
