using System.ServiceProcess;

namespace Cmf.Custom.TibcoEMS.Gateway.Service
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new TibcoGatewayService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
