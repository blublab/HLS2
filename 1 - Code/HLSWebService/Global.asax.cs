using System;
using System.Web.Routing;

namespace HLSWebService
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // HLS initialisieren.
            Application["HLS"] = new HLS();

            // Routen für RESTful Webservice konfigurieren.
            RouteTable.Routes.Add(
                null,
                new Route(
                    "Sendungsanfragen/{saNr}",
                    new RouteValueDictionary { { "saNr", "-1" } },
                    new PageRouteHandler("~/Sendungsanfrage.aspx")));
            RouteTable.Routes.Add(
                null,
                new Route(
                    "Kundenrechnungen/{krNr}",
                    new RouteValueDictionary { { "krNr", "-1" } },
                    new PageRouteHandler("~/Kundenrechnung.aspx")));
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}