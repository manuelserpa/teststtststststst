using Cmf.Custom.amsOSRAM.Common;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;
using System.Collections.Generic;
using System.Linq;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using System;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Navigo.BusinessObjects;

namespace Cmf.Custom.amsOSRAM.Actions.NiceLabelPrinting
{
    public class CustomNiceLabelPrint : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---   

            #region Info
            /// <summary>
            /// Summary text
            ///     - DEE Action to be triggered on material track out to send retrive and send information for the nice label printing.
            ///     
            /// Action Groups:
            ///     - MaterialManagement.MaterialManagementOrchestration.ComplexTrackInMaterials.Post
            ///     - MaterialManagement.MaterialManagementOrchestration.ComplexTrackOutMaterials.Post
            ///     - MaterialManagement.MaterialManagementOrchestration.MoveMaterialsToNextStep.Post
            ///     
            /// </summary>
            #endregion

            bool isToExecute = false;

            if (!string.IsNullOrEmpty(amsOSRAMUtilities.GetConfig<string>(amsOSRAMConstants.AutomationGenericNiceLabelPrintResourcePath)))
            {
                isToExecute = true;
            }
            
            return isToExecute;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            //Navigo
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects");

            //Common
            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");

            //Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            IMaterialCollection materialCollection = entityFactory.CreateCollection<IMaterialCollection>();
            string operation = null;
            IResource resource = null;

            if (Input.ContainsKey("ComplexTrackOutMaterialsInput"))
            {
                ComplexTrackOutMaterialsInput complexTrackOutInput = Input["ComplexTrackOutMaterialsInput"] as ComplexTrackOutMaterialsInput;
                materialCollection.AddRange(complexTrackOutInput.Materials.Keys);
                materialCollection.Load();
                resource = materialCollection.First().LastProcessedResource;
                operation = GetDataForTrackOutAndMoveNextOperation.TrackOut.ToString();
            }
            else if (Input.ContainsKey("ComplexTrackInMaterialsOutput"))
            {
                materialCollection = (Input["ComplexTrackInMaterialsOutput"] as ComplexTrackInMaterialsOutput).Materials;
                materialCollection.Load();
                resource = (Input["ComplexTrackInMaterialsOutput"] as ComplexTrackInMaterialsOutput).Resource;
                operation = GetDataForTrackInOperation.TrackIn.ToString();
            }
            else if (Input.ContainsKey("MoveMaterialsToNextStepOutput"))
            {
                materialCollection = (Input["MoveMaterialsToNextStepOutput"] as MoveMaterialsToNextStepOutput).Materials;
                materialCollection.Load();
                resource = materialCollection.First().LastProcessedResource;
                operation = "MoveNext";
            }
            
            Dictionary<string, Dictionary<string, string>> materials = new Dictionary<string, Dictionary<string, string>>();

            foreach (IMaterial material in materialCollection)
            {
                Dictionary<string, string> materialNiceLabelPrintInformation = amsOSRAMUtilities.GetDataForNiceLabelPrinting(material, resource, operation);

                if (materialNiceLabelPrintInformation != null && materialNiceLabelPrintInformation.Count > 0)
                {
                    // associate the material with the printing information
                    materials.Add(material.Name, materialNiceLabelPrintInformation);
                }

            }

            #region IoT call

            if (materials != null && materials.Count > 0)
            {
                string resourceName = amsOSRAMUtilities.GetConfig<string>(amsOSRAMConstants.AutomationGenericNiceLabelPrintResourcePath);
                IResource resourceToPrint = entityFactory.Create<IResource>();
                resourceToPrint.Load(resourceName);

                IAutomationControllerInstance controllerInstance = resourceToPrint.GetAutomationControllerInstance();
                if (controllerInstance != null)
                {
                    // Send an Assynchronous message to automation controller in the Equipment
                    string requestType = amsOSRAMConstants.AutomationRequestSendNiceLabelPrintInformation;

                    controllerInstance.Publish(requestType, materials.ToJsonString());
                }
            }

            #endregion IoT call


            Input.Add("NiceLabelInformation", materials);

            //---End DEE Code---


            return Input;
        }
    }
}
