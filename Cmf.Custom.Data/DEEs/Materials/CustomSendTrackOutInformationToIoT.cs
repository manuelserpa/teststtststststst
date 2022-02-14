using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Cmf.Custom.AMSOsram.Actions.Materials
{
    class CustomSendTrackOutInformationToIoT : DeeDevBase
    {
        /// <summary>
        /// Dee test condition.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <returns></returns>
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *     Dee action to Trigger IoT call to TrackOut the Materials
             *     Rule must be the last to run on the [Orchestration] Complex Track Out Materials Post Action Group.
             *     Rule must only execute if:
             *       The Resource Automation Mode is set to Online;
             *       The Material being Track Out is the Top-Most (this rule must not execute/send request on sub-material);
             *  
             * Action Groups:
             *      MaterialManagement.MaterialManagementOrchestration.ComplexTrackOutMaterials.Post
             *     
            */

            #endregion
            bool canExecute = false;

            if (Input.ContainsKey("ComplexTrackOutMaterialsOutput"))
            {
                ComplexTrackOutMaterialsOutput inputObject = Input["ComplexTrackOutMaterialsOutput"] as ComplexTrackOutMaterialsOutput;
                if (inputObject != null && inputObject.Materials != null && inputObject.Materials.Count > 0)
                {
                    // The Material being Track In is the Top - Most(this rule must not execute / send request on sub - material tracking);
                    List<Material> materialsToTrackOut = inputObject.Materials.Keys.Where(m => m.ParentMaterial == null).ToList();
                    if (materialsToTrackOut.Count > 0)
                    {
                        Resource resource = new Resource() { Name = materialsToTrackOut.First().LastProcessedResource.Name };
                        resource.Load();

                        // The Resource Automation Mode is set to Online;
                        canExecute = (resource.AutomationMode == ResourceAutomationMode.Online);
                        if (canExecute)
                        {
                            ApplicationContext.CallContext.SetInformationContext("MaterialsToTrackOut", materialsToTrackOut);
                            ApplicationContext.CallContext.SetInformationContext("ResourceToTrackOut", resource);
                        }
                    }
                }
            }

            return canExecute;

            //---End DEE Condition Code---
        }

        /// <summary>
        /// Dee action code.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <returns></returns>
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            // Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("", "Cmf.Foundation.Common");
            UseReference("", "Cmf.Foundation.BusinessObjects.Cultures");
            // Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects");
            // Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.DataStructures");
            // System
            UseReference("", "System.Threading");

            List<MaterialData> materialDataToIot = new List<MaterialData>();
            List<Material> materialsTotrackOut = ApplicationContext.CallContext.GetInformationContext("MaterialsToTrackOut") as List<Material>;
            Resource resourceToTrackOut = ApplicationContext.CallContext.GetInformationContext("ResourceToTrackOut") as Resource;

            foreach (Material materialIn in materialsTotrackOut)
            {
                MaterialData materialData = new MaterialData();
                materialData.MaterialId = materialIn.Id.ToString();
                materialData.MaterialName = materialIn.Name;
                materialDataToIot.Add(materialData);
            }

            #region IoT call

            if (materialDataToIot.Count > 0)
            {
                // TODO: Later confirm that message was sent to IoT correctly (find a way to test it automatically)
                // Utilities.PublishMessage("AMSOsram.Test.SendTrackOutInformationToIoT", new Dictionary<string, object>() { { "Data", materialDataToIot.ToJsonString() } });

                AutomationControllerInstance controllerInstance = resourceToTrackOut.GetAutomationControllerInstance();
                if (controllerInstance != null)
                {
                    // Get EI default timeout
                    //  --> /Cmf/Custom/Automation/GenericRequestTimeout
                    int requestTimeout = AMSOsramUtilities.GetConfig<int>(AMSOsramConstants.AutomationGenericRequestTimeoutConfigurationPath);

                    // Send Synchronous request to automation TrackOut the Material in the Equipment
                    string requestType = AMSOsramConstants.AutomationRequestTypeTrackOut;
                    var obj = controllerInstance.SendRequest(requestType, materialDataToIot.ToJsonString(), requestTimeout);

                    if (obj == null)
                    {
                        throw new CmfBaseException(string.Format(LocalizedMessage.GetLocalizedMessage(Thread.CurrentThread.CurrentCulture.Name, AMSOsramConstants.LocalizedMessageIoTConnectionTimeout).MessageText, requestType));
                    }
                }
            }

            #endregion

            //---End DEE Code---

            return Input;
        }
    }
}
