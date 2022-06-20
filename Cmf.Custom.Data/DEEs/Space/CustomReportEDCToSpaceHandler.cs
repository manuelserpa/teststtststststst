using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.OutputObjects;
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

            // Validate that the step has the attribute "Needs Space confirmation" other wise do not execute this DEE Action
            if (Input != null)
            {
                ComplexPerformDataCollectionOutput performImmediateDataCollectionInput = AMSOsramUtilities.GetInputItem<ComplexPerformDataCollectionOutput>(Input, "ComplexPerformDataCollectionOutput");

                DataCollectionInstance dataCollectionInstance = performImmediateDataCollectionInput.DataCollectionInstances.FirstOrDefault();

                Material material = dataCollectionInstance.Material;
                material.Load();

                Step step = material.Step;
                step.Load();
                step.LoadAttributes(new Collection<string> { AMSOsramConstants.StepAttributeRequiresSpaceConfirmation });
                bool requiresSpaceConfirmation = (bool)step.GetAttributeValue(AMSOsramConstants.StepAttributeRequiresSpaceConfirmation);

                if (requiresSpaceConfirmation)
                {
                    //DeeContextHelper.SetContextParameter("CustomReportEDCToSpaceHandler_Material", material);
                    DeeContextHelper.SetContextParameter("CustomReportEDCToSpaceHandler_DataCollectionInstance", dataCollectionInstance);

                    return true;
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

            bool isOutOfSpec = false;
            ReasonCollection reasons = new ReasonCollection();

            DataCollectionInstance dataCollectionInstance = DeeContextHelper.GetContextParameter("CustomReportEDCToSpaceHandler_DataCollectionInstance") as DataCollectionInstance;
            dataCollectionInstance.Load();

            DataCollectionLimitSet limitSet = dataCollectionInstance.DataCollectionLimitSet;
            limitSet.LoadRelations(Navigo.Common.Constants.DataCollectionParameterLimit);
            limitSet.DataCollectionParameterLimits.Load();

            // Get Data collection
            dataCollectionInstance.LoadRelations("DataCollectionPoint");
            foreach (DataCollectionPoint dcPoint in dataCollectionInstance.DataCollectionPoints)
            {
                DataCollectionParameterLimit parameterLimit = limitSet.DataCollectionParameterLimits.FirstOrDefault(limit => limit.TargetEntity.GetNativeValue<long>("TargetEntity").Equals(dcPoint.TargetEntity.GetNativeValue<long>("TargetEntity")));

                if (parameterLimit.LowerErrorLimit != null &&
                    parameterLimit.UpperErrorLimit != null &&
                    (Convert.ToDecimal(dcPoint.Value) < parameterLimit.LowerErrorLimit ||
                    Convert.ToDecimal(dcPoint.Value) > parameterLimit.UpperErrorLimit))
                {
                    isOutOfSpec = true;
                    break;
                }

                if (parameterLimit.LowerWarningLimit != null &&
                    parameterLimit.UpperWarningLimit != null &&
                    (Convert.ToDecimal(dcPoint.Value) < parameterLimit.LowerWarningLimit ||
                    Convert.ToDecimal(dcPoint.Value) > parameterLimit.UpperWarningLimit))
                {
                    isOutOfSpec = true;
                    break;
                }
            }

            Material submaterial = new Material() { Name = dataCollectionInstance.DataCollectionPoints.FirstOrDefault().SampleId };
            submaterial.Load();

            Material material = submaterial.ParentMaterial as Material;
            material.Load();

            if (isOutOfSpec)
            {
                // hold material with out of spec reason
                string outOfSpecName = AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.DefaultLotIncomingHoldReasonConfig);
                Reason reason = new Reason() { Name = outOfSpecName };
                reason.Load();
                reasons.Add(reason);
            }

            if (reasons.Count > 0)
            {
                // Put lot on hold 
                AMSOsramUtilities.PutLotOnHold(material, reasons);
            }
            else
            {
                // open protocol
                Protocol protocol = new Protocol() { Name = AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.DefaultSpaceProtocol) };
                if (protocol.ObjectExists())
                {
                    protocol.Load();

                    OpenProtocolInstanceInput openProtocolInstanceInput = new OpenProtocolInstanceInput()
                    {
                        Protocol = protocol,
                        MaterialsToAssociate = new MaterialCollection() { material }
                    };

                    ExceptionManagementOrchestration.OpenProtocolInstance(openProtocolInstanceInput);

                }
            }

            //Material material = new Material() { Name = Input["Material"].ToString() };
            //material.Load();

            Facility facility = material.Facility;
            facility.Load();
            Site site = facility.Site;
            site.Load();
            site.LoadAttributes(new Collection<string>() { AMSOsramConstants.CustomSiteCodeAttribute });
            string siteCode = site.Attributes[AMSOsramConstants.CustomSiteCodeAttribute].ToString();

            string recipeName = material.CurrentRecipeInstance != null ? material.CurrentRecipeInstance.ParentEntity.Name : string.Empty;

            //DataCollectionInstance dataCollectionInstance = new DataCollectionInstance() { Name = Input["DataCollectionInstance"].ToString() };
            //dataCollectionInstance.Load();

            //DataCollectionLimitSet limitSet = new DataCollectionLimitSet() { Name = Input["LimitSet"].ToString() };
            //limitSet.Load();
            //limitSet.LoadRelations(Cmf.Navigo.Common.Constants.DataCollectionParameterLimit);

            string host = Cmf.Foundation.Common.Configuration.AppSettings.Current["ServerName"];

            // Create Lot Values Message
            CustomReportEDCToSpace customSendLotDCInformation = AMSOsramUtilities.CreateSpaceInfoWaferValues(material, dataCollectionInstance, limitSet, host, new List<string>() { siteCode }, recipeName);

            Utilities.PublishTransactionalMessage("CustomReportEDCToSpace", customSendLotDCInformation.SerializeToXML());

            //---End DEE Code---

            return Input;
        }
    }
}
