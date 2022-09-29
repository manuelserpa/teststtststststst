using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Foundation.Common;
using System;
using System.Collections.Generic;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Cmf.Custom.amsOSRAM.Actions.Automation
{
    class CustomAutomationAbortMaterial : DeeDevBase
    {
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.DataStructures");

            if (!Input.ContainsKey("MaterialName"))
            {
                throw new ArgumentNullCmfException("MaterialName");
            }

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            IMaterial material = entityFactory.Create<IMaterial>();
            material.Name = Input["MaterialName"] as string;

            material.Load();

            material.AbortProcess();

            IResource resource = material.LastProcessedResource;
            resource.Load();

            List<MaterialData> materialDataToIot = new List<MaterialData>();

            MaterialData materialData = new MaterialData();
            materialData.MaterialId = material.Id.ToString();
            materialData.MaterialName = material.Name;
            materialDataToIot.Add(materialData);

            IAutomationControllerInstance controllerInstance = resource.GetAutomationControllerInstance();

            if (controllerInstance != null)
            {
                // Get EI default timeout
                //  --> /Cmf/Custom/Automation/GenericRequestTimeout
                int requestTimeout = amsOSRAMUtilities.GetConfig<int>(amsOSRAMConstants.AutomationGenericRequestTimeoutConfigurationPath);

                // Send Synchronous request to automation Abort the Material in the Equipment
                string requestType = amsOSRAMConstants.AutomationRequestTypeAbort;
                object obj = controllerInstance.SendRequest(requestType, materialDataToIot.ToJsonString(), requestTimeout);

                if (obj == null)
                {
                    throw new CmfBaseException(string.Format("Failed to Abort Material {0}", material.Name));
                }
            }
            //---End DEE Code---

            return Input;
        }

        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *     Dee action is triggered by IoT Automation to adjust the state of a Material 
             *  
             * Action Groups:
             *      None
             *     
            */

            #endregion

            return true;

            //---End DEE Condition Code---
        }
    }
}
