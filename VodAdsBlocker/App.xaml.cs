using System.Threading;
using System.Windows;

namespace VodAdsBlocker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string MutexName = "##||VodAdsBlocker||##";
        private readonly Mutex _mutex;
        bool createdNew;

        public App()
        {
            _mutex = new Mutex(true, MutexName, out createdNew);
            if (!createdNew)
            {
                MessageBox.Show("This program is already running");
                Application.Current.Shutdown(0);
            }
        }
    }
}
