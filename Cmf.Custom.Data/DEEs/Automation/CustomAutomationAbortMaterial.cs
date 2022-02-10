using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Cmf.Custom.AMSOsram.Actions.Automation
{
    class CustomAutomationAbortMaterial : DeeDevBase
    {
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("", "Cmf.Foundation.Common.Exceptions");
            UseReference("", "Cmf.Foundation.Common");
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.DataStructures");

            if (!Input.ContainsKey("MaterialName"))
            {
                throw new ArgumentNullCmfException("MaterialName");
            }
            Material material = new Material() { Name = Input["MaterialName"] as String };

            material.Load();

            material.AbortProcess();

            Resource resource = material.LastProcessedResource;
            resource.Load();

            List<MaterialData> materialDataToIot = new List<MaterialData>();

            MaterialData materialData = new MaterialData();
            materialData.MaterialId = material.Id.ToString();
            materialData.MaterialName = material.Name;
            materialDataToIot.Add(materialData);


            AutomationControllerInstance controllerInstance = resource.GetAutomationControllerInstance();
            if (controllerInstance != null)
            {
                // Get EI default timeout
                //  --> /Cmf/Custom/Automation/GenericRequestTimeout
                int requestTimeout = AMSOsramUtilities.GetConfig<int>(AMSOsramConstants.AutomationGenericRequestTimeoutConfigurationPath);

                // Send Synchronous request to automation Abort the Material in the Equipment
                string requestType = AMSOsramConstants.AutomationRequestTypeAbort;
                var obj = controllerInstance.SendRequest(requestType, materialDataToIot.ToJsonString(), requestTimeout);

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
