using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.ExceptionManagement;
using Cmf.Navigo.BusinessOrchestration.ExceptionManagement.InputObjects;
using System.Collections.Generic;
using System.Linq;

namespace Cmf.Custom.AMSOsram.Actions.Space
{
    public class CustomReportEDCToSpaceHandler : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---   

            #region Info
            /// <summary>
            /// Summary text
            ///     DEE Action to     
            /// 
            /// Action Groups:
            ///     - DataCollectionManagement.DataCollectionInstanceManagementOrchestration.PostDataCollectionPoints.Post
            ///     - DataCollectionManagement.DataCollectionInstanceManagementOrchestration.CloseDataCollectionInstance.Post
            ///     - EdcManagement.DataCollectionManagement.ComplexPerformDataCollection.Post
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion

            if (Input != null)
            {
                DataCollectionInstance dataCollectionInstance = AMSOsramUtilities.GetInputItem<DataCollectionInstance>(Input, Navigo.Common.Constants.DataCollectionInstance);

                if (dataCollectionInstance != null && dataCollectionInstance.DataCollectionLimitSet != null)
                {
                    dataCollectionInstance.Step.Load();

                    if (dataCollectionInstance.Step.HasAttribute(AMSOsramConstants.StepAttributeRequiresSpaceConfirmation, true))
                    {
                        // The Action only executed if Step needs Space confirmation
                        return (bool)dataCollectionInstance.Step.GetAttributeValue(AMSOsramConstants.StepAttributeRequiresSpaceConfirmation);
                    }
                }
            }

            return false;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            //Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("", "Cmf.Foundation.Common.Exceptions");
            UseReference("", "Cmf.Foundation.Common");

            //Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.OutputObjects");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.ExceptionManagement");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.ExceptionManagement.InputObjects");

            //Common
            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");

            //Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.DataStructures");

            // Get DataCollectionInstance from Input
            DataCollectionInstance dataCollectionInstance = AMSOsramUtilities.GetInputItem<DataCollectionInstance>(Input, Navigo.Common.Constants.DataCollectionInstance);

            if (dataCollectionInstance is null)
            {
                throw new CmfBaseException("Cannot be possible to get DataCollectionInstance data from DEE Action Input.");
            }

            // Load DataCollection Instance
            dataCollectionInstance.Load();

            // DataCollection Limit Set
            DataCollectionLimitSet dataCollectionLimitSet = dataCollectionInstance.DataCollectionLimitSet;

            // Load DataCollection Parameter Limit
            dataCollectionLimitSet.LoadRelations(Navigo.Common.Constants.DataCollectionParameterLimit);

            // Load DataCollection Points
            dataCollectionInstance.LoadRelations(Navigo.Common.Constants.DataCollectionPoint);

            // Material associated to DataCollection
            Material material = dataCollectionInstance.Material;

            // Check if Material associated to DataCollection have a Parent Material
            if (dataCollectionInstance.Material.ParentMaterial != null)
            {
                // Set Parent Material to Material instance
                material = dataCollectionInstance.Material.ParentMaterial;
            }

            // Load Material
            material.Load();

            // Check limits of Data Collection Points
            foreach (DataCollectionPoint dataCollectionPoint in dataCollectionInstance.DataCollectionPoints)
            {
                DataCollectionParameterLimit parameterLimit = dataCollectionLimitSet.DataCollectionParameterLimits.FirstOrDefault(limit => limit.TargetEntity.GetNativeValue<long>("TargetEntity").Equals(dataCollectionPoint.TargetEntity.GetNativeValue<long>("TargetEntity")));

                // Parse DataCollection Point Value to Decimal
                decimal dcPointValue = AMSOsramUtilities.GetValueAsDecimal(dataCollectionPoint.Value.ToString());

                // Check Parameter Warning Limits
                if (parameterLimit.LowerErrorLimit != null && parameterLimit.UpperErrorLimit != null &&
                   (dcPointValue < parameterLimit.LowerErrorLimit || dcPointValue > parameterLimit.UpperErrorLimit))
                {
                    // Hold Material
                    material.HoldMaterial(AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.DefaultLotIncomingHoldReasonConfig);

                    break;
                }

                // Check Parameter Warning Limits
                if (parameterLimit.LowerWarningLimit != null && parameterLimit.UpperWarningLimit != null &&
                   (dcPointValue < parameterLimit.LowerWarningLimit || dcPointValue > parameterLimit.UpperWarningLimit))
                {
                    // Hold Material
                    material.HoldMaterial(AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.DefaultLotIncomingHoldReasonConfig);

                    break;
                }
            }

            if (material.HoldCount == 0)
            {
                // Open protocol
                Protocol protocol = new Protocol()
                {
                    Name = AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.DefaultSpaceProtocol)
                };

                if (protocol.ObjectExists())
                {
                    protocol.Load();

                    OpenProtocolInstanceInput openProtocol = new OpenProtocolInstanceInput()
                    {
                        Protocol = protocol,
                        MaterialsToAssociate = new MaterialCollection()
                        {
                            material
                        }
                    };

                    ExceptionManagementOrchestration.OpenProtocolInstance(openProtocol);
                }
            }

            // Load Site
            Site site = material.Facility?.Site;
            site.Load();

            // Get SiteCode attribute value
            string siteCode = site.GetAttributeValue(AMSOsramConstants.CustomSiteCodeAttribute, true).ToString();

            // Create Message to send for Space
            CustomReportEDCToSpace dataCollectionInfoMessage = AMSOsramUtilities.CreateSpaceInfoWaferValues(material, dataCollectionInstance, dataCollectionLimitSet, siteCode);

            // Publish message on Message Bus
            Utilities.PublishTransactionalMessage("CustomReportEDCToSpace", dataCollectionInfoMessage.SerializeToXML());

            //---End DEE Code---

            return Input;
        }
    }
}
