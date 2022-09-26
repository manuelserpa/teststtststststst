using System;
using System.Collections.Generic;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Cmf.Custom.amsOSRAM.Actions.ProcessRules._2._0._0.Before
{
    public class CustomEnableIoTFlagResource : DeeDevBase
    {
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            const string ResourceEntityTypeName = "Resource";

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            IEntityType resourceEntityType = entityFactory.Create<IEntityType>();
            resourceEntityType.Name = ResourceEntityTypeName;

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
