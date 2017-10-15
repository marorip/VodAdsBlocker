using System.Threading;
using System.Windows;

namespace VodAdsBlocker
{
    public partial class App : Application
    {
        private const string MutexName = "##||VodAdsBlocker||##";
        private readonly Mutex _mutex;

        public App()
        {
            _mutex = new Mutex(true, MutexName, out var createdNew);
            if (!createdNew)
            {
                MessageBox.Show("This program is already running");
                Current.Shutdown(0);
            }
        }
    }
}
