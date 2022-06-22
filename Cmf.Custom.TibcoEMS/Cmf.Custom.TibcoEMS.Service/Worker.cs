using Cmf.Custom.TibcoEMS.ServiceManager;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cmf.Custom.TibcoEMS.Service
{
    /// <summary>
    /// Tibco Windows Service worker
    /// </summary>
    public class Worker : IHostedService
    {
        private readonly ILogger Logger;

        private TibcoEMSServiceManager TibcoServiceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="Worker"/> class.
        /// </summary>
        public Worker(ILogger logger)
        {
            this.TibcoServiceManager = new TibcoEMSServiceManager(logger);

            this.Logger = logger;
        }

        /// <summary>
        /// Triggered when the Windows Service is ready to start the service.
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                this.Logger.LogWarning("Starting...");

                this.TibcoServiceManager.OnStart();
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
                this.Logger.LogWarning("Stoping...");

                this.TibcoServiceManager.OnStop();
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, ex.Message, null);
            }

            return Task.CompletedTask;
        }
    }
}
