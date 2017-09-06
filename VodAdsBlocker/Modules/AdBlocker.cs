using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Fiddler;

namespace VodAdsBlocker
{
    public class AdBlocker : IDisposable
    {
        private bool disposed;
        private VodFilters filters;

        public EventHandler OnStarted;
        public EventHandler OnStopped;
        
        public AdBlocker()
        {
            UpdateFilters();

            FiddlerApplication.BeforeRequest += delegate (Session oSession)
            {
                if (filters?.Filters?.Any(x => oSession.fullUrl.Contains(x.Query)) == true)
                {
                    Debug.WriteLine(string.Format("Req: {0}", oSession.fullUrl));
                    oSession.bBufferResponse = true;
                }

                if (oSession.fullUrl.Contains("//s.tvp.pl") && oSession.fullUrl.Contains(".css"))
                {
                    Debug.WriteLine(string.Format("TVP Req: {0}", oSession.fullUrl));
                    oSession.bBufferResponse = true;
                }
            };

            FiddlerApplication.BeforeResponse += delegate (Session oSession)
            {
                Filter filter = filters?.Filters?.FirstOrDefault(x => oSession.fullUrl.Contains(x.Query));
                if (filter != null)
                {
                    Debug.WriteLine(string.Format("Resp: {0}", oSession.fullUrl));
                    oSession.utilSetResponseBody(filter.Response);
                }

                if (oSession.fullUrl.Contains("//s.tvp.pl") && oSession.fullUrl.Contains(".css"))
                {
                    Debug.WriteLine(string.Format("TVP Resp: {0}", oSession.fullUrl));
                    string response = oSession.GetResponseBodyAsString().Replace("#tvpoverlay_abdinfo{", "#tvpoverlay_abdinfo{display:none;");
                    oSession.utilSetResponseBody(response);
                }
            };
        }

        public void UpdateFilters()
        {
            string filterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VODFilter.xml");
            if (File.Exists(filterPath))
            {
                filters = Utils.DeserializeFromFile<VodFilters>(filterPath);
            }
        }

        public void Start()
        {
            InstallCertificate();

            FiddlerApplication.Startup(8877, true, true);

            OnStarted?.Invoke(this, EventArgs.Empty);
        }

        public static bool InstallCertificate()
        {
            if (!CertMaker.rootCertExists())
            {
                if (!CertMaker.createRootCert())
                    return false;
            }

            if (!CertMaker.rootCertIsTrusted())
            {
                if (!CertMaker.trustRootCert())
                    return false;
            }

            return true;
        }

        public static bool UninstallCertificate()
        {
            if (CertMaker.rootCertExists())
            {
                if (!CertMaker.removeFiddlerGeneratedCerts(true))
                    return false;
            }

            return true;
        }

        public void Stop()
        {
            UninstallCertificate();

            FiddlerApplication.Shutdown();
            System.Threading.Thread.Sleep(750);

            OnStopped?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Stop();
            }
            
            disposed = true;
        }
    }
}
