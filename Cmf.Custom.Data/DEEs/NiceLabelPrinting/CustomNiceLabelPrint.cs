using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;
using System.Collections.Generic;
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
            
            
            return true;

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

            if (Input.ContainsKey("ComplexTrackOutMaterialsInput"))
            {
                ComplexTrackOutMaterialsInput complexTrackOutInput = Input["ComplexTrackOutMaterialsInput"] as ComplexTrackOutMaterialsInput;
                operation = GetDataForTrackOutAndMoveNextOperation.TrackOut.ToString();
                materialCollection.AddRange(complexTrackOutInput.Materials.Keys);
            }
            else if (Input.ContainsKey("ComplexTrackInMaterialsOutput"))
            {
                materialCollection = (Input["ComplexTrackInMaterialsOutput"] as ComplexTrackInMaterialsOutput).Materials;
                operation = GetDataForTrackInOperation.TrackIn.ToString();
            }
            else if (Input.ContainsKey("MoveMaterialsToNextStepOutput"))
            {
                materialCollection = (Input["MoveMaterialsToNextStepOutput"] as MoveMaterialsToNextStepOutput).Materials;
                operation = "Move Next";
            }
            materialCollection.Load();

            Dictionary<string, Dictionary<string, string>> materials = new Dictionary<string, Dictionary<string, string>>();

            // resolve custom ST
            foreach (Material material in materialCollection)
            {
                // Get Material information
                string stepName = material.Step.Name;
                string materialName = material.Name;
                string productName = material.Product.Name;
                string logicalFlowPath = material.LogicalFlowPath;
                // Product group isn't mandatory, we have to null check it
                string productGroupName = material.Product.ProductGroup?.Name;
                string flowName = material.Flow.Name;
                Resource resource = material.LastProcessedResource;


                Dictionary<string, string> materialNiceLabelPrintContext = AMSOsramUtilities.CustomResolveSTCustomMaterialNiceLabelPrintContext(stepName,
                                                                                                                                                logicalFlowPath,
                                                                                                                                                productName,
                                                                                                                                                productGroupName,
                                                                                                                                                flowName,
                                                                                                                                                materialName,
                                                                                                                                                material.Type,
                                                                                                                                                resource.Name,
                                                                                                                                                resource.Type,
                                                                                                                                                resource.Model,
                                                                                                                                                operation);
                // missing information for tags


                if (materialNiceLabelPrintContext != null)
                {
                    materials.Add(material.Name, materialNiceLabelPrintContext);
                }


            }

            #region IoT call

            if (materials.Count > 0)
            {
                string resourceName = AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.AutomationGenericNiceLabelPrintResourcePath);
                Resource resourceToPrint = new Resource();
                resourceToPrint.Load(resourceName);

                AutomationControllerInstance controllerInstance = resourceToPrint.GetAutomationControllerInstance();
                if (controllerInstance != null)
                {
                    // Get EI default timeout
                    //  --> /Cmf/Custom/Automation/TrackInTimeout
                    int requestTimeout = AMSOsramUtilities.GetConfig<int>(AMSOsramConstants.AutomationTrackInTimeoutConfigurationPath);

                    // Send Synchronous request to automation TrackIn the Material in the Equipment
                    string requestType = AMSOsramConstants.AutomationRequestSendNiceLabelPrintInformation;
                    var obj = controllerInstance.SendRequest(requestType, materials.ToJsonString(), requestTimeout);

                    if (obj == null)
                    {
                        throw new CmfBaseException(string.Format(LocalizedMessage.GetLocalizedMessage(Thread.CurrentThread.CurrentCulture.Name, AMSOsramConstants.LocalizedMessageIoTConnectionTimeout).MessageText, requestType));
                    }
                    else if (obj.ToString().Contains("Error"))
                    {
                        throw new CmfBaseException(obj.ToString());
                    }
                }
            }

            #endregion IoT call


            Input.Add("NiceLabelInformation", materials);

            //---End DEE Code---


            return Input;
        }
    }
}
