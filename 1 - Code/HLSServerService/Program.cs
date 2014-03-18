using log4net;
using System;
using System.ServiceProcess;

namespace HLSServerService
{
    public static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        public static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
                        
            try
            {
                Log.Debug("Building HLS application.");
                ApplicationBuilder.BuildApplication();
                Log.Debug("HLS application was built.");

                Log.Debug("Creating testdata.");
                ApplicationBuilder.CreateTestdata();
                Log.Debug("Testdata ware created.");
            }
            catch (Exception ex)
            {
                Log.Fatal("Initialization of application has failed. Exception was: " + ex.ToString());
                Log.Fatal("Shutting down.");
                System.Environment.Exit(-1);
            }
            
            Log.Debug("Creating HLS Service.");
            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[] 
            { 
                new HLSServerService() 
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
