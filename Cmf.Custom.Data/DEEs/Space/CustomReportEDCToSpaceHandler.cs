using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.ExceptionManagement;
using Cmf.Navigo.BusinessOrchestration.ExceptionManagement.InputObjects;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

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
            ///     DEE Action to validate DataCollection and create a XML message to be sent to Space system.
            ///
            /// Action Groups:
            ///     - BusinessObjects.DataCollectionInstance.PerformImmediate.Post
            ///     - BusinessObjects.DataCollectionInstance.Close.Post
            ///
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion
            bool canExecute = false;
            bool reportEDCToSpace = true;

            if (Input != null)
            {
                // If this DEE Action was called from CustomIncomingMaterialLotCreation, we don't want to report the EDC data to Space
                if ((bool?)ApplicationContext.CallContext.GetInformationContext("ReportEDCToSpace") == false)
                {
                    reportEDCToSpace = false;
                }

                // Get DataCollectionInstance from Input
                DataCollectionInstance dataCollectionInstance = AMSOsramUtilities.GetInputItem<DataCollectionInstance>(Input, Navigo.Common.Constants.DataCollectionInstance);

                // Check if Input data returns DataCollection and associated DataCollection Limit Sets
                if (reportEDCToSpace && dataCollectionInstance != null && dataCollectionInstance.DataCollectionLimitSet != null)
                {
                    // Load DataCollection Step
                    dataCollectionInstance.Step.Load();

                    // Check if step needs Space Confirmation
                    if (dataCollectionInstance.Step.HasAttribute(AMSOsramConstants.StepAttributeRequiresSpaceConfirmation, true) &&
                        (bool)dataCollectionInstance.Step.GetAttributeValue(AMSOsramConstants.StepAttributeRequiresSpaceConfirmation))
                    {
                        // The Action only executed if Step needs Space confirmation
                        canExecute = true;
                    }
                }
            }

            return canExecute;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            //System
            UseReference("%MicrosoftNetPath%System.Private.Xml.dll", "System.Xml");
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json");

            //Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("", "Cmf.Foundation.Common.Exceptions");
            UseReference("", "Cmf.Foundation.Common");

            //Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
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
                throw new CmfBaseException("Cannot be possible to get DataCollectionInstance data from Input.");
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
                DataCollectionParameterLimit parameterLimit = dataCollectionLimitSet.DataCollectionParameterLimits?.FirstOrDefault(limit => limit.TargetEntity.GetNativeValue<long>("TargetEntity").Equals(dataCollectionPoint.TargetEntity.GetNativeValue<long>("TargetEntity")));

                // Parse DataCollection Point Value to Decimal
                decimal dcPointValue = AMSOsramUtilities.GetValueAsDecimal(dataCollectionPoint.Value.ToString());

                // Check Parameter Error Limits
                if (parameterLimit.LowerErrorLimit != null && parameterLimit.UpperErrorLimit != null &&
                    (dcPointValue < parameterLimit.LowerErrorLimit || dcPointValue > parameterLimit.UpperErrorLimit))
                {
                    // Hold Material
                    material.HoldMaterial(AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.DefaultLotIncomingHoldReasonConfig));

                    break;
                }

                // Check Parameter Warning Limits
                if (parameterLimit.LowerWarningLimit != null && parameterLimit.UpperWarningLimit != null &&
                    (dcPointValue < parameterLimit.LowerWarningLimit || dcPointValue > parameterLimit.UpperWarningLimit))
                {
                    // Hold Material
                    material.HoldMaterial(AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.DefaultLotIncomingHoldReasonConfig));

                    break;
                }
            }

            // Check if Material doens't have any Hold Reasons associated
            if (material.HoldCount == 0)
            {
                // Protocol
                Protocol protocol = new Protocol()
                {
                    Name = AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.DefaultSpaceProtocol)
                };

                // Check if protocol exists
                if (protocol.ObjectExists())
                {
                    // Load Protocol
                    protocol.Load();

                    // Create relation between Protocol and Material
                    OpenProtocolInstanceInput openProtocol = new OpenProtocolInstanceInput()
                    {
                        Protocol = protocol,
                        MaterialsToAssociate = new MaterialCollection()
                        {
                            material
                        }
                    };

                    // Open Protocol associated to a Material
                    ExceptionManagementOrchestration.OpenProtocolInstance(openProtocol);
                }
            }

            // Create Message to send for Space
            CustomReportEDCToSpace dataCollectionInfoMessage = AMSOsramUtilities.CreateSpaceInfoWaferValues(material, dataCollectionInstance, dataCollectionLimitSet);

            // Load Xml into XmlDocument to get InnerXml without formatting
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(dataCollectionInfoMessage.SerializeToXML());

            // Publish message on Message Bus
            Utilities.PublishTransactionalMessage(AMSOsramConstants.CustomReportEDCToSpace,
                                                  JsonConvert.SerializeObject(new
                                                  {
                                                      Message = xmlDocument.InnerXml
                                                  }));

            //---End DEE Code---

            return Input;
        }
    }
}
