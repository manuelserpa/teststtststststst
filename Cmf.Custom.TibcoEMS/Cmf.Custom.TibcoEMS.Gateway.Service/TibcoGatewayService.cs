using Cmf.Custom.TibcoEMS.Gateway.Logic;
using System.Diagnostics;
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
            Debugger.Launch();
            this.TibcoGateway.OnStart();
        }

        protected override void OnStop()
        {
            this.TibcoGateway.OnStop();
        }
    }
}
