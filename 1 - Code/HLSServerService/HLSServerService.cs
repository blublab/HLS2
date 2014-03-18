using log4net;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.ServiceProcess;
using System.Web.Http;

namespace HLSServerService
{
    /// <summary>
    /// todo: http://msdn.microsoft.com/en-us/library/zt39148a.aspx
    /// </summary>
    public partial class HLSServerService : ServiceBase
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IDisposable webApp;

        public HLSServerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // set baseAddress to Win-Devel host
            string baseAddress = "http://win-devel.informatik.haw-hamburg.de:5555/";

            // Start OWIN host
            Log.Debug("Starting HLS Service.");
            webApp = WebApp.Start<Startup>(url: baseAddress);
            Log.Debug("HLS Service has started.");
        }

        protected override void OnStop()
        {
            Log.Debug("Stopping HLS Service.");
            webApp.Dispose();
            Log.Debug("HLS Service has stopped.");
        }
    }

    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            appBuilder.UseWebApi(config);
        }
    } 
}
