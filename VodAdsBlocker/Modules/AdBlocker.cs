using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace VodAdsBlocker.Modules
{
    public class AdBlocker : IDisposable
    {
        private bool _disposed;
        private VodFilters _filters;

        public EventHandler OnStarted;
        public EventHandler OnStopped;

        private readonly ProxyServer _proxyServer;

        public AdBlocker()
        {
            UpdateFilters();

            _proxyServer = new ProxyServer
            {
                TrustRootCertificate = true
            };

            //proxyServer.BeforeRequest += OnRequest;
            _proxyServer.BeforeResponse += OnResponse;
            _proxyServer.ServerCertificateValidationCallback += OnCertificateValidation;
            _proxyServer.ClientCertificateSelectionCallback += OnCertificateSelection;
        }

        public Task OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            //set IsValid to true/false based on Certificate Errors
            if (e.SslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                e.IsValid = true;

            return Task.FromResult(0);
        }

        /// Allows overriding default client certificate selection logic during mutual authentication
        public Task OnCertificateSelection(object sender, CertificateSelectionEventArgs e)
        {
            //set e.clientCertificate to override
            return Task.FromResult(0);
        }

        private async Task OnResponse(object arg1, SessionEventArgs arg2)
        {
            var url = arg2.WebSession.Request.Url;

            Filter filter = _filters?.Filters?.FirstOrDefault(x => url.Contains(x.Query));
            if (filter != null)
            {
                Debug.WriteLine($"Resp: {url}");
                await arg2.SetResponseBodyString(filter.Response);
            }

            if (url.Contains("//s.tvp.pl") && url.Contains(".css"))
            {
                Debug.WriteLine($"TVP Resp: {url}");

                string response = await arg2.GetResponseBodyAsString();
                response = response.Replace("#tvpoverlay_abdinfo{", "#tvpoverlay_abdinfo{display:none;");
                await arg2.SetResponseBodyString(response);
            }
        }

        //private Task OnRequest(object arg1, SessionEventArgs arg2)
        //{
        //    var url = arg2.WebSession.Request.Url;

        //    if (filters?.Filters?.Any(x => url.Contains(x.Query)) == true)
        //    {
        //        Debug.WriteLine($"Req: {url}");
        //    }

        //    if (url.Contains("//s.tvp.pl") && url.Contains(".css"))
        //    {
        //        Debug.WriteLine($"TVP Req: {url}");
        //    }

        //    return Task.FromResult(0);
        //}

        public void UpdateFilters()
        {
            string filterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VODFilter.xml");
            if (File.Exists(filterPath))
            {
                _filters = Utils.DeserializeFromFile<VodFilters>(filterPath);
            }
        }

        public void Start()
        {
            var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8000, true)
            {
                ExcludedHttpsHostNameRegex = new List<string>
                {
                    "player.com",
                    "dropbox.com"
                }
            };
            
            _proxyServer.AddEndPoint(explicitEndPoint);
            _proxyServer.Start();

            _proxyServer.SetAsSystemHttpProxy(explicitEndPoint);
            _proxyServer.SetAsSystemHttpsProxy(explicitEndPoint);

            OnStarted?.Invoke(this, EventArgs.Empty);
        }

        public static bool InstallCertificate()
        {
            return true;
        }

        public static bool UninstallCertificate()
        {
            return true;
        }

        public void Stop()
        {
            _proxyServer?.Stop();

            OnStopped?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Stop();

                _proxyServer?.Dispose();
            }
            
            _disposed = true;
        }
    }
}
