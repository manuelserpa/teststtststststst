using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Cmf.Custom.AMSOsram.Actions.NiceLabelPrinting
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

            if (!string.IsNullOrEmpty(AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.AutomationGenericNiceLabelPrintResourcePath)))
            {
                isToExecute = true;
            }
            
            return isToExecute;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            //System
            UseReference("", "System.Threading");
            UseReference("", "System");

            //Foundation
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");

            //Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects");

            //Common
            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");

            //Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");

            MaterialCollection materialCollection = new MaterialCollection();
            string operation = null;
            Resource resource = null;

            if (Input.ContainsKey("ComplexTrackOutMaterialsInput"))
            {
                ComplexTrackOutMaterialsInput complexTrackOutInput = Input["ComplexTrackOutMaterialsInput"] as ComplexTrackOutMaterialsInput;
                operation = GetDataForTrackOutAndMoveNextOperation.TrackOut.ToString();
                materialCollection.AddRange(complexTrackOutInput.Materials.Keys);
                resource = materialCollection.First().LastProcessedResource;
            }
            else if (Input.ContainsKey("ComplexTrackInMaterialsOutput"))
            {
                materialCollection = (Input["ComplexTrackInMaterialsOutput"] as ComplexTrackInMaterialsOutput).Materials;
                operation = GetDataForTrackInOperation.TrackIn.ToString();
                resource = (Input["ComplexTrackInMaterialsOutput"] as ComplexTrackInMaterialsOutput).Resource;
            }
            else if (Input.ContainsKey("MoveMaterialsToNextStepOutput"))
            {
                materialCollection = (Input["MoveMaterialsToNextStepOutput"] as MoveMaterialsToNextStepOutput).Materials;
                resource = materialCollection.First().LastProcessedResource;
                operation = "Move Next";
            }

            materialCollection.Load(1);
            
            Dictionary<string, Dictionary<string, string>> materials = new Dictionary<string, Dictionary<string, string>>();

            // resolve custom ST
            foreach (Material material in materialCollection)
            {
                Dictionary<string, string> materialNiceLabelPrintInformation = AMSOsramUtilities.GetDataForNiceLabelPrinting(material, resource, operation);

                if (materialNiceLabelPrintInformation != null && materialNiceLabelPrintInformation.Count > 0)
                {
                    // associate the material with the printing information
                    materials.Add(material.Name, materialNiceLabelPrintInformation);
                }

            }

            #region IoT call

            if (materials != null && materials.Count > 0)
            {
                string resourceName = AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.AutomationGenericNiceLabelPrintResourcePath);
                Resource resourceToPrint = new Resource();
                resourceToPrint.Load(resourceName);

                AutomationControllerInstance controllerInstance = resourceToPrint.GetAutomationControllerInstance();
                if (controllerInstance != null)
                {
                    // Send an Assynchronous message to automation controller in the Equipment
                    string requestType = AMSOsramConstants.AutomationRequestSendNiceLabelPrintInformation;

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
