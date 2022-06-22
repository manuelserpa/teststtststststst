using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cmf.Custom.TibcoEMS.Service
{
    /// <summary>
    /// Main Program class.
    /// </summary>
    public class Program
    {
        private static ILogger Log4NetAdapter;

        public static void Main(string[] args)
        {
            Log4NetProvider log4NetProvider = new Log4NetProvider();
            Log4NetAdapter = log4NetProvider.CreateLogger();

            CreateHostBuilder(args, log4NetProvider, Log4NetAdapter).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, ILoggerProvider log4NetProvider, ILogger log4NetAdapter)
        {
            return Host
                   .CreateDefaultBuilder(args)
                   .UseWindowsService()
                   .ConfigureServices((hostContext, services) =>
                   {
                       services.AddSingleton(log4NetAdapter);
                       services.AddSingleton(log4NetProvider);
                       services.AddHostedService<Worker>();
                   });
        }
    }
}
