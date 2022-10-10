using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using System;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.Common.LocalizationService;

namespace Cmf.Custom.amsOSRAM.Actions.Materials
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
                    List<IMaterial> materialsToTrackOut = inputObject.Materials.Keys.Where(m => m.ParentMaterial == null).ToList();
                    if (materialsToTrackOut.Count > 0)
                    {
                        // Get services provider information
                        IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
                        IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

                        IResource resource = entityFactory.Create<IResource>();
                        resource.Name = materialsToTrackOut.First().LastProcessedResource.Name;
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
            UseReference("Cmf.Foundation.Common.dll", " Cmf.Foundation.Common.LocalizationService");
            
            // Navigo
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects");
            
            // Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.DataStructures");
            
            // System
            UseReference("", "System.Threading");

            List<MaterialData> materialDataToIot = new List<MaterialData>();
            List<IMaterial> materialsTotrackOut = ApplicationContext.CallContext.GetInformationContext("MaterialsToTrackOut") as List<IMaterial>;
            IResource resourceToTrackOut = ApplicationContext.CallContext.GetInformationContext("ResourceToTrackOut") as IResource;

            foreach (IMaterial materialIn in materialsTotrackOut)
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
                // Utilities.PublishMessage("amsOSRAM.Test.SendTrackOutInformationToIoT", new Dictionary<string, object>() { { "Data", materialDataToIot.ToJsonString() } });

                IAutomationControllerInstance controllerInstance = resourceToTrackOut.GetAutomationControllerInstance();
                if (controllerInstance != null)
                {
                    // Get EI default timeout
                    //  --> /Cmf/Custom/Automation/GenericRequestTimeout
                    int requestTimeout = amsOSRAMUtilities.GetConfig<int>(amsOSRAMConstants.AutomationGenericRequestTimeoutConfigurationPath);

                    // Send Synchronous request to automation TrackOut the Material in the Equipment
                    string requestType = amsOSRAMConstants.AutomationRequestTypeTrackOut;
                    object obj = controllerInstance.SendRequest(requestType, materialDataToIot.ToJsonString(), requestTimeout);

                    if (obj == null)
                    {
                        IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
                        ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();

                        throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageIoTConnectionTimeout), requestType));
                    }
                }
            }

            #endregion

            //---End DEE Code---

            return Input;
        }
    }
}
