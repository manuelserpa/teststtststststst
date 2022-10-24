using Cmf.Custom.TibcoEMS.ServiceManager.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Specialized;

namespace Cmf.Custom.TibcoEMS.Service
{
    /// <summary>
    /// Main Program class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger Logger;

        /// <summary>
        /// Tibco Configs
        /// </summary>
        private static NameValueCollection TibcoConfigs;

        public static void Main(string[] args)
        {
            Log4NetProvider log4NetProvider = new Log4NetProvider();
            Logger = log4NetProvider.CreateLogger();

            try
            {
                // Configure LBOs
                TibcoEMSUtilities.InitialConfigurations();

                Logger.LogInformation("Getting Tibco configurations...");
                TibcoConfigs = TibcoEMSUtilities.GetTibcoConfigs();

                if (TibcoConfigs != null && TibcoConfigs.Keys.Count > 0)
                {
                    bool tibcoIsEnabled = bool.TryParse(TibcoConfigs["IsEnabled"], out tibcoIsEnabled) ? tibcoIsEnabled : false;

                    if (tibcoIsEnabled)
                    {
                        Logger.LogInformation("Starting Service...");

                        CreateHostBuilder(args).Build().Run();
                    }
                    else
                    {
                        Logger.LogWarning("Unable to run the service because Tibco configuration is disabled.");
                    }
                }
                else
                {
                    Logger.LogWarning("Unable to run the service because it was not possible to load the Tibco settings.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, null);

                Logger.LogWarning("It was not possible to start the Service.");
            }
        }
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                   .CreateDefaultBuilder(args)
                   .UseWindowsService()
                   .ConfigureServices((hostContext, services) =>
                   {
                       services.AddSingleton(TibcoConfigs);
                       services.AddSingleton(Logger);
                       services.AddHostedService<Worker>();
                   });
        }
    }
}
