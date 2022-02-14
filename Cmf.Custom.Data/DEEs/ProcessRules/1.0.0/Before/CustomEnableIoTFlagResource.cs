using System.Collections.Generic;
using Cmf.Foundation.BusinessObjects;

namespace Cmf.Custom.AMSOsram.Actions.ProcessRules._1._0._0.Before
{
    public class CustomEnableIoTFlagResource : DeeDevBase
    {
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---


            // Foundation
            UseReference("Cmf.Foundation.Common.dll", "Cmf.Foundation.Common.Base");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.CommunicationLayer.Sap.dll", "Cmf.Foundation.CommunicationLayer.Converters");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration");


            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration.EntityTypeManagement");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration.EntityTypeManagement.InputObjects");

            const string ResourceEntityTypeName = "Resource";

            EntityType resourceEntityType = new EntityType() { Name = ResourceEntityTypeName };

            if (resourceEntityType.ObjectExists())
            {
                resourceEntityType.Load();

                if (!resourceEntityType.ConnectIoTEnabled)
                {
                    resourceEntityType.ConnectIoTEnabled = true;

                    resourceEntityType.Save();

                }

                
            }

            //---End DEE Code---
            return Input;
        }

        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            return true;

            //---End DEE Condition Code---
        }
    }
}
