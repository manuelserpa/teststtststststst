using Cmf.Custom.TibcoEMS.ServiceManager;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace Cmf.Custom.TibcoEMS.Service
{
    /// <summary>
    /// Tibco Windows Service worker
    /// </summary>
    public class Worker : IHostedService
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger Logger;

        /// <summary>
        /// TibcoServiceManager
        /// </summary>
        private TibcoEMSServiceManager TibcoServiceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="Worker"/> class.
        /// </summary>
        public Worker(ILogger logger, NameValueCollection tibcoConfigs)
        {
            this.TibcoServiceManager = new TibcoEMSServiceManager(logger, tibcoConfigs);

            this.Logger = logger;
        }

        /// <summary>
        /// Triggered when the Windows Service is ready to start the service.
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                this.TibcoServiceManager.OnStart();

                this.Logger.LogInformation("Service Started.");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, ex.Message, null);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Triggered when the Windows Service is performing a graceful shutdown.
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                this.Logger.LogWarning("Stoping Service...");

                this.TibcoServiceManager.OnStop();

                this.Logger.LogInformation("Service Stopped.");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, ex.Message, null);
            }

            return Task.CompletedTask;
        }
    }
}
