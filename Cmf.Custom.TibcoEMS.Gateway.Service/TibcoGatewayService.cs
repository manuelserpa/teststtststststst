using Cmf.Custom.TibcoEMS.Gateway.Logic;
using System.ServiceProcess;

namespace Cmf.Custom.TibcoEMS.Gateway.Service
{
    public partial class TibcoGatewayService : ServiceBase
    {
        private TibcoGateway TibcoGateway;

        public TibcoGatewayService()
        {
            InitializeComponent();

            this.TibcoGateway = new TibcoGateway();
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();
            this.TibcoGateway.Start();
        }

        protected override void OnStop()
        {
            this.TibcoGateway.Stop();
        }
    }
}
