using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.ExceptionManagement;
using Cmf.Navigo.BusinessOrchestration.ExceptionManagement.InputObjects;
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
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion

            // Validate that the step has the attribute "Needs Space confirmation" other wise do not execute this DEE Action
            // Input != null && ((Input.ContainsKey("FinalDataCollectionPoints") && (Input["FinalDataCollectionPoints"] as DataCollectionPointCollection).Any())) && Input.ContainsKey("Material")
            if (Input != null)
            {
                PerformImmediateDataCollectionInput performImmediateDataCollectionInput = Input["PerformImmediateDataCollectionInput"] as PerformImmediateDataCollectionInput;

                DataCollectionInstance dataCollectionInstance = performImmediateDataCollectionInput.DataCollectionInstance;
                

                Material material = dataCollectionInstance.Material;
                material.Load();

                Step step = material.Step;
                step.Load();
                step.LoadAttributes(new Collection<string> { AMSOsramConstants.StepAttributeRequiresSpaceConfirmation });
                bool requiresSpaceConfirmation = (bool)step.GetAttributeValue(AMSOsramConstants.StepAttributeRequiresSpaceConfirmation);

                if (requiresSpaceConfirmation)
                {
                    DeeContextHelper.SetContextParameter("CustomReportEDCToSpaceHandler_Material", material);
                    DeeContextHelper.SetContextParameter("CustomReportEDCToSpaceHandler_DataCollectionInstance", dataCollectionInstance);
                    return true;
                    
                    //DeeContextHelper.SetContextParameter("SiteCode", siteCode);
                }
            }

            return false;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            //System
            UseReference("", "System.Linq");
            UseReference("", "System");

            //Foundation
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");

            //Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");

            //Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");

            // Sync method 
            // Utilities.PublishTransactionalMessage("CustomReportEDCToSpace", new Dictionary<string, object> () { {"EDC", dci }});

            //Validate response 

            bool isOutOfSpec = false;
            ReasonCollection reasons = new ReasonCollection();

            Material material = DeeContextHelper.GetContextParameter("CustomReportEDCToSpaceHandler_Material") as Material;

            DataCollectionInstance dataCollectionInstance = DeeContextHelper.GetContextParameter("CustomReportEDCToSpaceHandler_DataCollectionInstance") as DataCollectionInstance;
            dataCollectionInstance.Load();

            DataCollectionLimitSet limitSet = dataCollectionInstance.DataCollectionLimitSet;
            limitSet.LoadRelations("DataCollectionParameterLimit");
            limitSet.DataCollectionParameterLimits.Load();

            // Get Data collection
            dataCollectionInstance.LoadRelations("DataCollectionPoint");
            foreach (DataCollectionPoint dcPoint in dataCollectionInstance.DataCollectionPoints)
            {
                DataCollectionParameterLimit parameterLimit = limitSet.DataCollectionParameterLimits.FirstOrDefault(ls => ls.TargetEntity.GetNativeValue<long>("TargetEntity").Equals(dcPoint.TargetEntity.GetNativeValue<long>("TargetEntity")));

                if (parameterLimit.LowerErrorLimit != null && parameterLimit.UpperErrorLimit != null && (System.Convert.ToDecimal(dcPoint.Value) < parameterLimit.LowerErrorLimit || System.Convert.ToDecimal(dcPoint.Value) > parameterLimit.UpperErrorLimit))
                {
                    isOutOfSpec = true;
                    break;
                }
                else if (parameterLimit.LowerWarningLimit != null && parameterLimit.UpperWarningLimit != null && ((decimal)dcPoint.Value < parameterLimit.LowerWarningLimit || (decimal)dcPoint.Value > parameterLimit.UpperWarningLimit))
                {
                    isOutOfSpec = true;
                    break ;
                }
            }

            if (isOutOfSpec)
            {
                // hold material with out of spec reason
                string outOfSpecName = AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.DefaultLotIncomingHoldReasonConfig);
                Reason reason = new Reason() { Name = outOfSpecName};
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
                        MaterialsToAssociate = new MaterialCollection(){material}
                    };

                    ExceptionManagementOrchestration.OpenProtocolInstance(openProtocolInstanceInput);

                }
            }

            AMSOsramUtilities.CreateSpaceInfoLotValues(material, dataCollectionInstance, limitSet, "hostServerName", new List<string>() { "SiteCodeItem"});

            //---End DEE Code---

            return Input;
        }
    }
}
