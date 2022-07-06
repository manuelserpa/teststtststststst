using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.ExceptionManagement;
using Cmf.Navigo.BusinessOrchestration.ExceptionManagement.InputObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

                if (dataCollectionInstance != null)
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

            // Load DataCollectionInstance data from Input
            DataCollectionInstance dataCollectionInstance = AMSOsramUtilities.GetInputItem<DataCollectionInstance>(Input, Navigo.Common.Constants.DataCollectionInstance);
            dataCollectionInstance.Load();

            if (dataCollectionInstance is null)
            {
                throw new CmfBaseException("Cannot be possible to get DataCollectionInstance data from DEE Action Input.");
            }

            // Load DataCollection Parameter Limit
            DataCollectionLimitSet dataCollectionLimitSet = dataCollectionInstance.DataCollectionLimitSet;
            dataCollectionLimitSet.LoadRelations(Navigo.Common.Constants.DataCollectionParameterLimit);
            dataCollectionLimitSet.DataCollectionParameterLimits.Load();

            // Load DataCollection Points
            dataCollectionInstance.LoadRelations(Navigo.Common.Constants.DataCollectionPoint);

            bool isOutOfSpec = false;

            foreach (DataCollectionPoint dataCollectionPoint in dataCollectionInstance.DataCollectionPoints)
            {
                DataCollectionParameterLimit parameterLimit = dataCollectionLimitSet.DataCollectionParameterLimits.FirstOrDefault(limit => limit.TargetEntity.GetNativeValue<long>("TargetEntity").Equals(dataCollectionPoint.TargetEntity.GetNativeValue<long>("TargetEntity")));

                decimal dcPointValue = AMSOsramUtilities.GetValueAsDecimal(dataCollectionPoint.Value.ToString());

                if (parameterLimit.LowerErrorLimit != null &&
                    parameterLimit.UpperErrorLimit != null &&
                    (dcPointValue < parameterLimit.LowerErrorLimit ||
                    dcPointValue > parameterLimit.UpperErrorLimit))
                {
                    isOutOfSpec = true;

                    break;
                }

                if (parameterLimit.LowerWarningLimit != null &&
                    parameterLimit.UpperWarningLimit != null &&
                    (dcPointValue < parameterLimit.LowerWarningLimit ||
                    dcPointValue > parameterLimit.UpperWarningLimit))
                {
                    isOutOfSpec = true;

                    break;
                }
            }

            // Load SubMaterial
            Material subMaterial = new Material()
            {
                Name = dataCollectionInstance.DataCollectionPoints.FirstOrDefault().SampleId
            };
            subMaterial.Load();

            Material material = subMaterial.ParentMaterial as Material;
            material.Load();

            ReasonCollection reasons = new ReasonCollection();

            // Put Material on hold when a Parameter Limit
            if (isOutOfSpec)
            {
                Reason reason = new Reason()
                {
                    Name = AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.DefaultLotIncomingHoldReasonConfig)
                };

                //reason.Load();
                reasons.Add(reason);
            }

            if (reasons.IsNullOrEmpty())
            {
                reasons.Load();

                // Put lot on hold 
                AMSOsramUtilities.PutLotOnHold(material, reasons);
            }
            else
            {
                // Open protocol
                Protocol protocol = new Protocol()
                {
                    Name = AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.DefaultSpaceProtocol)
                };

                if (protocol.ObjectExists())
                {
                    protocol.Load();

                    OpenProtocolInstanceInput openProtocolInstanceInput = new OpenProtocolInstanceInput()
                    {
                        Protocol = protocol,
                        MaterialsToAssociate = new MaterialCollection()
                        {
                            material
                        }
                    };

                    ExceptionManagementOrchestration.OpenProtocolInstance(openProtocolInstanceInput);
                }
            }

            Site site = material.Facility?.Site;
            site.Load();

            string siteCode = site.GetAttributeValue(AMSOsramConstants.CustomSiteCodeAttribute, true).ToString();

            // Create Lot Values Message
            CustomReportEDCToSpace customSendLotDCInformation = AMSOsramUtilities.CreateSpaceInfoWaferValues(material, dataCollectionInstance, dataCollectionLimitSet, siteCode);

            Utilities.PublishTransactionalMessage("CustomReportEDCToSpace", customSendLotDCInformation.SerializeToXML());

            //---End DEE Code---

            return Input;
        }
    }
}
