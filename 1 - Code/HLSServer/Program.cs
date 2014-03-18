using Owin;
using System.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using System.Net.Http;

namespace HLSServer
{
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
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        }
    } 

    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:5555/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                //HttpClient client = new HttpClient();
                //var response = client.GetAsync(baseAddress + "api/values").Result;
                //Console.WriteLine(response);
                //Console.WriteLine(response.Content.ReadAsStringAsync().Result);

                Console.WriteLine("HLS Server has started. Press any key to shutdown server.");
                Console.ReadLine(); 
            }
        }
    }
}
