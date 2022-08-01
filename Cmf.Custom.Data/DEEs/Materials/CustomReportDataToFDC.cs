using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.Extensions;
using Cmf.Custom.OntoFDC;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;

namespace Cmf.Custom.AMSOsram.Actions.Materials
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
             *      BusinessObjects.MaterialCollection.TrackIn.Post
             *      BusinessObjects.MaterialCollection.TrackOut.Post
            */

            bool canExecute = false;


            #endregion

            string actionGroup = Input["ActionGroupName"].ToString();

            // Validate if FDC is active
            if (Config.TryGetConfig(AMSOsramConstants.FDCConfigActivePath, out Config isActive) &&
                    isActive.GetConfigValue<bool>())
            {
                // Abort operation
                if (actionGroup.Contains("AbortMaterialsProcess"))
                {
                    if (Input["AbortMaterialsProcessOutput"] is AbortMaterialsProcessOutput abortMaterials &&
                        abortMaterials.Materials != null && abortMaterials.Materials.Count > 0)
                    {
                        ApplicationContext.CallContext.SetInformationContext("Materials", abortMaterials.Materials);
                        canExecute = true;
                    }
                    else
                    {
                        throw new ArgumentNullCmfException("AbortMaterialsProcessOutput");
                    }
                }

                // TrackIn / TrackOut operations
                if (actionGroup.Contains("TrackIn") || actionGroup.Contains("TrackOut"))
                {
                    if (Input.ContainsKey(Navigo.Common.Constants.MaterialCollection))
                    {
                        ApplicationContext.CallContext.SetInformationContext("Materials", Input[Navigo.Common.Constants.MaterialCollection]);
                        canExecute = true;
                    }
                    else
                    {
                        throw new ArgumentNullCmfException(Navigo.Common.Constants.MaterialCollection);
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
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("", "Cmf.Foundation.Common.Exceptions");
            UseReference("", "Cmf.Foundation.Common");
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            UseReference("", "System.Text");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.Extensions");
            UseReference("Cmf.Custom.OntoFDC.dll", "Cmf.Custom.OntoFDC");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects");

            MaterialCollection materials = ApplicationContext.CallContext.GetInformationContext("Materials") as MaterialCollection;
            materials.Load();

            foreach (Material material in materials)
            {
                if (material.LastProcessedResource != null)
                {
                    // Validate FDCCommunication attribute from Resource - what about the wafers??
                    material.LastProcessedResource.LoadAttributes(new Collection<string> { AMSOsramConstants.ResourceAttributeFDCCommunication });

                    if (material.LastProcessedResource.Attributes.ContainsKey(AMSOsramConstants.ResourceAttributeFDCCommunication) &&
                        (bool)material.LastProcessedResource.Attributes[AMSOsramConstants.ResourceAttributeFDCCommunication])
                    {
                        string messageType = string.Empty;
                        string integrationEntryMessageXml = string.Empty;
                        FdcWaferInfo fdcWaferInfo = new FdcWaferInfo();
                        FdcLotInfo fdcLotInfo = new FdcLotInfo();

                        switch (material.SystemState)
                        {
                            case MaterialSystemState.Queued:
                                if (material.ParentMaterial == null && material.Form.Equals("Lot"))
                                {
                                    messageType = AMSOsramConstants.MessageType_LOTOUT;

                                    // SendFDCLotEnd
                                    material.Step.Load();

                                    fdcLotInfo.LotName = material.Name;
                                    fdcLotInfo.BatchName = "";
                                    fdcLotInfo.Operation = material.Step.Name;
                                    //fdcLotInfo.Chamber = material.LastProcessedResource.Name; // ???

                                    //Serialize Integration Entry Message into xml
                                    integrationEntryMessageXml = fdcLotInfo.ToXml();
                                }
                                break;
                            case MaterialSystemState.InProcess:
                                if (material.ParentMaterial == null && material.Form.Equals("Lot"))
                                {
                                    messageType = AMSOsramConstants.MessageType_LOTIN;

                                    // SendFDCLotStart
                                    material.Step.Load();
                                    material.LastProcessedResource.LastService.Load();
                                    material.Product.Load();
                                    material.Flow.Load();
                                    material.Facility.Load();
                                    material.LoadChildren();

                                    fdcLotInfo.LotName = material.Name;
                                    fdcLotInfo.BatchName = "";
                                    fdcLotInfo.Operation = material.Step.Name;
                                    fdcLotInfo.SPS = material.LastProcessedResource.LastService.Name; //???
                                    fdcLotInfo.RecipeName = material.CurrentRecipeInstance != null ? material.CurrentRecipeInstance.Name : string.Empty;
                                    fdcLotInfo.ProductName = material.Product.Name;
                                    fdcLotInfo.ProductRoute = material.Flow.Name;
                                    fdcLotInfo.NumberOfWafersInBatch = (int)material.PrimaryQuantity; //???
                                    fdcLotInfo.Chamber = material.LastProcessedResource.Name; //?? sub-resource??
                                    fdcLotInfo.FacilityName = material.Facility.Name;
                                    //fdcWaferInfo.Owner = ""; //??
                                    //fdcWaferInfo.ProductionLevel = ""; // ???

                                    //Serialize Integration Entry Message into xml
                                    integrationEntryMessageXml = fdcLotInfo.ToXml();
                                }
                                else if (material.ParentMaterial != null) // Validate if Logical wafer?
                                {
                                    // SendFDCWaferIn
                                    messageType = AMSOsramConstants.MessageType_WAFERIN;
                                    int? containerPosition = null;
                                    // Load materialContainer
                                    material.LoadRelations("MaterialContainer");

                                    if (material.RelationCollection != null && material.RelationCollection.ContainsKey("MaterialContainer")
                                        && material.RelationCollection["MaterialContainer"].Count > 0)
                                    {
                                        containerPosition = material.MaterialContainer.First().Position;
                                    }

                                    fdcWaferInfo.BatchName = "";
                                    fdcWaferInfo.LotName = material.ParentMaterial.Name;
                                    fdcWaferInfo.WaferName = material.Name;
                                    fdcWaferInfo.SlotPos = containerPosition;
                                    fdcWaferInfo.Chamber = material.LastProcessedResource.Name; //??
                                    fdcWaferInfo.LotPos = containerPosition; //??
                                    fdcWaferInfo.QtyIn = (int)material.PrimaryQuantity; //??
                                    //fdcWaferInfo.IsDummy = true; //?? ProductionType?
                                    //fdcWaferInfo.CarrierGravure = ""; //???
                                    //fdcWaferInfo.Gravure = ""; // ???
                                    //fdcWaferInfo.VendorName = ""; //??
                                    //fdcWaferInfo.Frame = ""; //???
                                    //fdcWaferInfo.WaferSize = ""; //??
                                    //fdcWaferInfo.BondingBoat = ""; //??
                                    // MF ID ???
                                    // MASKID ???
                                    // EPI_POSITION ??? 

                                    //Serialize Integration Entry Message into xml
                                    integrationEntryMessageXml = fdcWaferInfo.ToXml();
                                }
                                break;
                            case MaterialSystemState.Processed:
                                if (material.ParentMaterial == null && material.Form.Equals("Lot"))
                                {
                                    // SendFDCLotEnd
                                    messageType = AMSOsramConstants.MessageType_LOTOUT;

                                    material.Step.Load();

                                    fdcLotInfo.LotName = material.Name;
                                    fdcLotInfo.BatchName = "";
                                    fdcLotInfo.Operation = material.Step.Name;
                                    fdcLotInfo.Chamber = material.LastProcessedResource.Name; // ???  sub-resource??

                                    //Serialize Integration Entry Message into xml
                                    integrationEntryMessageXml = fdcLotInfo.ToXml();
                                }
                                else if (material.ParentMaterial != null) // Validate if Logical wafer?
                                {
                                    // SendFDCWaferOut
                                    messageType = AMSOsramConstants.MessageType_WAFEROUT;

                                    fdcWaferInfo.BatchName = "";
                                    fdcWaferInfo.LotName = material.ParentMaterial.Name;
                                    fdcWaferInfo.WaferName = material.Name;
                                    fdcWaferInfo.Chamber = material.LastProcessedResource.Name; //??
                                    fdcWaferInfo.Processed = true; //???
                                    fdcWaferInfo.WaferState = "Processed"; //??
                                    //fdcWaferInfo.ReadQuality = ""; //??
                                    // MASKID ???

                                    //Serialize Integration Entry Message into xml
                                    integrationEntryMessageXml = fdcWaferInfo.ToXml();
                                }
                                break;
                        }

                        #region Create Integration Entry

                        if (!string.IsNullOrEmpty(integrationEntryMessageXml))
                        {
                            //Build Integration Entry
                            IntegrationEntry integrationEntry = new IntegrationEntry();
                            integrationEntry.Name = $"{ material.Name.Replace(" ", "_") }-{ Guid.NewGuid()}";
                            integrationEntry.MessageType = messageType;
                            integrationEntry.MessageDate = DateTime.Now;
                            integrationEntry.IntegrationMessage.Message = Encoding.UTF8.GetBytes(integrationEntryMessageXml);
                            integrationEntry.EventName = AMSOsramConstants.OsramEventName;
                            integrationEntry.SourceSystem = Constants.MesSystemDesignation;
                            integrationEntry.TargetSystem = AMSOsramConstants.TargetSystem_OntoFDC;
                            integrationEntry.NumberOfRetries = 1;
                            integrationEntry.IsRetriable = true;
                            integrationEntry.SystemState = Foundation.Common.Integration.IntegrationEntrySystemState.Received;

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
