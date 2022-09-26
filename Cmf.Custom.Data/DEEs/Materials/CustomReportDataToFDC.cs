using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.FDC;
using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.Configuration.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Cmf.Custom.amsOSRAM.Actions.Materials
{
    public class CustomReportDataToFDC : DeeDevBase
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
             *     Dee action is triggered to create an integration entry with the material data to send to FDC.
             *  
             * Action Groups:
             *      MaterialManagement.MaterialManagementOrchestration.AbortMaterialsProcess.Post
             *      MaterialManagement.MaterialManagementOrchestration.ComplexTrackInMaterials.Post
             *      MaterialManagement.MaterialManagementOrchestration.ComplexTrackOutMaterials.Post
            */

            #endregion

            bool canExecute = false;
            string actionGroup = Input["ActionGroupName"].ToString();
            List<IMaterial> materials = new List<IMaterial>();

            // Validate if FDC is active
            if (Config.TryGetConfig(amsOSRAMConstants.FDCConfigActivePath, out IConfig isActive) &&
                    isActive.GetConfigValue<bool>())
            {
                // Abort operation
                if (actionGroup.Contains("AbortMaterialsProcess"))
                {
                    if (Input["AbortMaterialsProcessOutput"] is AbortMaterialsProcessOutput abortMaterials &&
                        abortMaterials.Materials != null && abortMaterials.Materials.Count > 0)
                    {
                        materials = abortMaterials.Materials.ToList();
                    }
                    else
                    {
                        throw new ArgumentNullCmfException("AbortMaterialsProcessOutput");
                    }
                }

                // TrackIn operation
                if (actionGroup.Contains("ComplexTrackInMaterials"))
                {
                    ComplexTrackInMaterialsOutput trackInMaterialsInput = Input["ComplexTrackInMaterialsOutput"] as ComplexTrackInMaterialsOutput;

                    if (trackInMaterialsInput != null && trackInMaterialsInput.Materials != null)
                    {
                        materials = trackInMaterialsInput.Materials.ToList();
                    }
                    else
                    {
                        throw new ArgumentNullCmfException(Navigo.Common.Constants.MaterialCollection);
                    }
                }

                // TrackOut operation
                if (actionGroup.Contains("ComplexTrackOutMaterials"))
                {
                    ComplexTrackOutMaterialsOutput trackOutMaterialsInput = Input["ComplexTrackOutMaterialsOutput"] as ComplexTrackOutMaterialsOutput;

                    if (trackOutMaterialsInput != null && trackOutMaterialsInput.Materials != null)
                    {
                        materials = trackOutMaterialsInput.Materials.Keys.ToList();
                    }
                    else
                    {
                        throw new ArgumentNullCmfException(Navigo.Common.Constants.MaterialCollection);
                    }
                }

                if (materials != null && materials.Count > 0)
                {
                    ApplicationContext.CallContext.SetInformationContext("Materials", materials);
                    canExecute = true;
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

            // System
            UseReference("", "System.Text");

            // Navigo
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects");
            
            // Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.FDC");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.Extensions");

            List<IMaterial> materials = ApplicationContext.CallContext.GetInformationContext("Materials") as List<IMaterial>;

            foreach (IMaterial material in materials)
            {
                if (material.LastProcessedResource != null)
                {
                    // Validate FDCCommunication attribute from Resource
                    material.LastProcessedResource.LoadAttributes(new Collection<string> { amsOSRAMConstants.ResourceAttributeFDCCommunication });

                    if (material.LastProcessedResource.Attributes.ContainsKey(amsOSRAMConstants.ResourceAttributeFDCCommunication) &&
                        (bool)material.LastProcessedResource.Attributes[amsOSRAMConstants.ResourceAttributeFDCCommunication])
                    {
                        string messageType = string.Empty;
                        string integrationEntryMessageXml = string.Empty;
                        FDCWaferInfo fdcWaferInfo = new FDCWaferInfo();
                        FDCLotInfo fdcLotInfo = new FDCLotInfo();

                        switch (material.SystemState)
                        {
                            case MaterialSystemState.Queued:
                                if (material.ParentMaterial == null && material.Form.Equals("Lot"))
                                {
                                    messageType = amsOSRAMConstants.MessageType_LOTOUT;

                                    // SendFDCLotEnd
                                    fdcLotInfo.LotName = material.Name;
                                    fdcLotInfo.BatchName = material.Name;
                                    fdcLotInfo.Operation = material.Step != null ? material.Step.Name : string.Empty;
                                    
                                    //Serialize Integration Entry Message into xml
                                    integrationEntryMessageXml = fdcLotInfo.SerializeToXML();
                                }
                                break;
                            case MaterialSystemState.InProcess:
                                if (material.ParentMaterial == null && material.Form.Equals("Lot"))
                                {
                                    messageType = amsOSRAMConstants.MessageType_LOTIN;

                                    // SendFDCLotStart
                                    fdcLotInfo.LotName = material.Name;
                                    fdcLotInfo.BatchName = material.Name;
                                    fdcLotInfo.Operation = !string.IsNullOrEmpty(material.Step?.Name) ? material.Step.Name : string.Empty;
                                    fdcLotInfo.SPS = !string.IsNullOrEmpty(material.LastProcessedResource?.LastService?.Name) ? material.LastProcessedResource.LastService.Name : string.Empty;
                                    fdcLotInfo.RecipeName = !string.IsNullOrEmpty(material.CurrentRecipeInstance?.ParentEntity?.Name) ? 
                                        material.CurrentRecipeInstance.ParentEntity.Name : string.Empty;
                                    fdcLotInfo.ProductName = !string.IsNullOrEmpty(material.Product?.Name)? material.Product.Name : string.Empty;
                                    fdcLotInfo.ProductRoute = !string.IsNullOrEmpty(material.Flow?.Name)? material.Flow.Name : string.Empty;
                                    fdcLotInfo.NumberOfWafersInBatch = Convert.ToInt16(material.PrimaryQuantity);
                                    fdcLotInfo.FacilityName = !string.IsNullOrEmpty(material.Facility?.Name) ? material.Facility.Name : string.Empty;

                                    //Serialize Integration Entry Message into xml
                                    integrationEntryMessageXml = fdcLotInfo.SerializeToXML();
                                }
                                else if (material.ParentMaterial != null)
                                {
                                    // SendFDCWaferIn
                                    messageType = amsOSRAMConstants.MessageType_WAFERIN;
                                    int? containerPosition = null;

                                    // Load materialContainer
                                    material.LoadRelations("MaterialContainer");

                                    if (material.RelationCollection != null && material.RelationCollection.ContainsKey("MaterialContainer")
                                        && material.RelationCollection["MaterialContainer"].Count > 0)
                                    {
                                        containerPosition = material.MaterialContainer.First().Position;
                                    }

                                    fdcWaferInfo.BatchName = !string.IsNullOrEmpty(material.ParentMaterial?.Name) ? material.ParentMaterial.Name : string.Empty;
                                    fdcWaferInfo.LotName = !string.IsNullOrEmpty(material.ParentMaterial?.Name) ? material.ParentMaterial.Name : string.Empty;
                                    fdcWaferInfo.WaferName = material.Name;
                                    fdcWaferInfo.SlotPos = containerPosition;
                                    fdcWaferInfo.LotPos = containerPosition;
                                    fdcWaferInfo.QtyIn = Convert.ToInt16(material.PrimaryQuantity);
                                    fdcWaferInfo.Chamber = !string.IsNullOrEmpty(material.LastProcessedResource?.Name) ?
                                        material.LastProcessedResource.Name : string.Empty;

                                    //Serialize Integration Entry Message into xml
                                    integrationEntryMessageXml = fdcWaferInfo.SerializeToXML();
                                }
                                break;
                            case MaterialSystemState.Processed:
                                if (material.ParentMaterial == null && material.Form.Equals("Lot"))
                                {
                                    // SendFDCLotEnd
                                    messageType = amsOSRAMConstants.MessageType_LOTOUT;

                                    fdcLotInfo.LotName = material.Name;
                                    fdcLotInfo.BatchName = material.Name;
                                    fdcLotInfo.Operation = !string.IsNullOrEmpty(material.Step?.Name) ? material.Step.Name : string.Empty;
                                    
                                    //Serialize Integration Entry Message into xml
                                    integrationEntryMessageXml = fdcLotInfo.SerializeToXML();
                                }
                                else if (material.ParentMaterial != null)
                                {
                                    // SendFDCWaferOut
                                    messageType = amsOSRAMConstants.MessageType_WAFEROUT;

                                    fdcWaferInfo.BatchName = !string.IsNullOrEmpty(material.ParentMaterial?.Name) ? material.ParentMaterial.Name : string.Empty;
                                    fdcWaferInfo.LotName = !string.IsNullOrEmpty(material.ParentMaterial?.Name) ? material.ParentMaterial.Name : string.Empty;
                                    fdcWaferInfo.WaferName = material.Name;
                                    fdcWaferInfo.Processed = true;
                                    fdcWaferInfo.WaferState = "Processed";
                                    fdcWaferInfo.Chamber = !string.IsNullOrEmpty(material.LastProcessedResource?.Name) ? 
                                        material.LastProcessedResource.Name : string.Empty;

                                    //Serialize Integration Entry Message into xml
                                    integrationEntryMessageXml = fdcWaferInfo.SerializeToXML();
                                }
                                break;
                        }

                        #region Create Integration Entry

                        IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
                        IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

                        if (!string.IsNullOrEmpty(integrationEntryMessageXml))
                        {
                            //Build Integration Entry
                            IIntegrationEntry integrationEntry = entityFactory.Create<IIntegrationEntry>();
                            integrationEntry.Name = $"{ material.Name.Replace(" ", "_") }-{ Guid.NewGuid()}";
                            integrationEntry.MessageType = messageType;
                            integrationEntry.MessageDate = DateTime.Now;
                            integrationEntry.IntegrationMessage.Message = Encoding.UTF8.GetBytes(integrationEntryMessageXml);
                            integrationEntry.EventName = amsOSRAMConstants.OsramEventName;
                            integrationEntry.SourceSystem = Constants.MesSystemDesignation;
                            integrationEntry.TargetSystem = amsOSRAMConstants.TargetSystem_OntoFDC;
                            integrationEntry.NumberOfRetries = 1;
                            integrationEntry.IsRetriable = true;
                            integrationEntry.SystemState = IntegrationEntrySystemState.Received;

                            integrationEntry.Create();
                        }

                        #endregion Create Integration Entry
                    }
                }
            }

            //---End DEE Code---

            return Input;
        }
    }
}
