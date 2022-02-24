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

            materialCollection.Load(1);
            
            Dictionary<string, Dictionary<string, string>> materials = new Dictionary<string, Dictionary<string, string>>();

            materialCollection.LoadRelations("MaterialContainer", 1);

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

                Dictionary<string, string> materialNiceLabelPrintInformation = AMSOsramUtilities.CustomResolveSTCustomMaterialNiceLabelPrintContext(stepName,
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

                if (materialNiceLabelPrintInformation != null)
                {
                    // add addictional information about the material
                    materialNiceLabelPrintInformation.Add("LotName", material.Name);
                    materialNiceLabelPrintInformation.Add("LotAlias","");                                                   // TODO: Missing information to map
                    materialNiceLabelPrintInformation.Add("ProductName", productName);
                    materialNiceLabelPrintInformation.Add("ProductDesc", material.Product?.Description);
                    materialNiceLabelPrintInformation.Add("ProductType",material.Product?.ProductType.ToString());
                    materialNiceLabelPrintInformation.Add("Product_Type", material.Product?.Type);
                    materialNiceLabelPrintInformation.Add("ProductGroupName", productGroupName);
                    materialNiceLabelPrintInformation.Add("ProductGroup_Type", material.Product?.ProductGroup?.Type);
                    materialNiceLabelPrintInformation.Add("FlowName", flowName);
                    materialNiceLabelPrintInformation.Add("BatchName", "");                                                 // TODO: Missing information to map

                    Container container = new Container();
                    if (material.RelationCollection.ContainsKey("MaterialContainer"))
                    {
                        container = material.RelationCollection["MaterialContainer"][0].TargetEntity as Container;
                        container.Load();
                    }
                    materialNiceLabelPrintInformation.Add("ContainerName", container.Name);
                    materialNiceLabelPrintInformation.Add("ExperimentName", material.Experiment?.Name);
                    materialNiceLabelPrintInformation.Add("ProductionOrder", material.ProductionOrder?.Name);
                    materialNiceLabelPrintInformation.Add("LotOwner", "");                                                  // TODO: Missing information to map                                        
                    materialNiceLabelPrintInformation.Add("ResourceName", resource.Name);
                    materialNiceLabelPrintInformation.Add("WaferLogicalName", "");                                          // TODO: Missing information to map
                    materialNiceLabelPrintInformation.Add("WaferSlotPosition", "");                                         // TODO: Missing information to map
                    materialNiceLabelPrintInformation.Add("WaferCrystalName", "");                                          // TODO: Missing information to map
                    materialNiceLabelPrintInformation.Add("LotWaferCount", "");                                             // TODO: Missing information to map
                    materialNiceLabelPrintInformation.Add("LotPrimaryQty", material.PrimaryQuantity.HasValue?material.PrimaryQuantity.ToString():string.Empty);
                    materialNiceLabelPrintInformation.Add("LotSecundaryQty", material.SecondaryQuantity.HasValue ? material.SecondaryQuantity.ToString() : string.Empty);
                    materialNiceLabelPrintInformation.Add("WaferPrimaryQty", "");                                           // TODO: Missing information to map
                    materialNiceLabelPrintInformation.Add("WaferSecundaryQty", "");                                         // TODO: Missing information to map
                    materialNiceLabelPrintInformation.Add("Lot_Type", material.Type);

                    Step step = material.Step;
                    // Get Area from Step
                    Area area = null;
                    step.LoadStepAreas();
                    if (step.StepAreas.Count > 0)
                    {
                        area = step.StepAreas.First().TargetEntity;
                        area.Load();
                    }
                    materialNiceLabelPrintInformation.Add("Area", area?.Name);
                    materialNiceLabelPrintInformation.Add("Facility", material.Facility.Name);

                    // associate the material with the printing information
                    materials.Add(material.Name, materialNiceLabelPrintInformation);
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
