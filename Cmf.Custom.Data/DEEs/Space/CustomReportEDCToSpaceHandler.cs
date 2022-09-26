using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessOrchestration.ExceptionManagement.InputObjects;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Cmf.Navigo.BusinessObjects.Abstractions;
using System;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Navigo.BusinessOrchestration.Abstractions;

namespace Cmf.Custom.amsOSRAM.Actions.Space
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
                IDataCollectionInstance dataCollectionInstance = amsOSRAMUtilities.GetInputItem<IDataCollectionInstance>(Input, Navigo.Common.Constants.DataCollectionInstance);

                // Check if Input data returns DataCollection and associated DataCollection Limit Sets
                if (reportEDCToSpace && dataCollectionInstance != null && dataCollectionInstance.DataCollectionLimitSet != null)
                {
                    // Load DataCollection Step
                    dataCollectionInstance.Step.Load();

                    // Check if step needs Space Confirmation
                    if (dataCollectionInstance.Step.HasAttribute(amsOSRAMConstants.StepAttributeRequiresSpaceConfirmation, true) &&
                        (bool)dataCollectionInstance.Step.GetAttributeValue(amsOSRAMConstants.StepAttributeRequiresSpaceConfirmation))
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

            //Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects.Abstractions");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.Abstractions");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.ExceptionManagement.InputObjects");

            //Common
            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");

            //Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.DataStructures");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.DataStructures");

            // Get DataCollectionInstance from Input
            IDataCollectionInstance dataCollectionInstance = amsOSRAMUtilities.GetInputItem<IDataCollectionInstance>(Input, Navigo.Common.Constants.DataCollectionInstance);

            if (dataCollectionInstance is null)
            {
                throw new CmfBaseException("Cannot be possible to get DataCollectionInstance data from Input.");
            }

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            // Load DataCollection Instance
            dataCollectionInstance.Load();

            // DataCollection Limit Set
            IDataCollectionLimitSet dataCollectionLimitSet = dataCollectionInstance.DataCollectionLimitSet;

            // Load DataCollection Parameter Limit
            dataCollectionLimitSet.LoadRelations(Navigo.Common.Constants.DataCollectionParameterLimit);

            // Load DataCollection Points
            dataCollectionInstance.LoadRelations(Navigo.Common.Constants.DataCollectionPoint);

            // Material associated to DataCollection
            IMaterial material = dataCollectionInstance.Material;

            // Check if Material associated to DataCollection have a Parent Material
            if (dataCollectionInstance.Material.ParentMaterial != null)
            {
                // Set Parent Material to Material instance
                material = dataCollectionInstance.Material.ParentMaterial;
            }

            // Load Material
            material.Load();

            // Check limits of Data Collection Points
            foreach (IDataCollectionPoint dataCollectionPoint in dataCollectionInstance.DataCollectionPoints)
            {
                IDataCollectionParameterLimit parameterLimit = dataCollectionLimitSet.DataCollectionParameterLimits?.FirstOrDefault(limit => limit.TargetEntity.GetNativeValue<long>("TargetEntity").Equals(dataCollectionPoint.TargetEntity.GetNativeValue<long>("TargetEntity")));

                // Parse DataCollection Point Value to Decimal
                decimal dcPointValue = amsOSRAMUtilities.GetValueAsDecimal(dataCollectionPoint.Value.ToString());

                // Check Parameter Error Limits
                if (parameterLimit.LowerErrorLimit != null && parameterLimit.UpperErrorLimit != null &&
                    (dcPointValue < parameterLimit.LowerErrorLimit || dcPointValue > parameterLimit.UpperErrorLimit))
                {
                    // Hold Material
                    material.HoldMaterial(amsOSRAMUtilities.GetConfig<string>(amsOSRAMConstants.DefaultLotIncomingHoldReasonConfig));

                    break;
                }

                // Check Parameter Warning Limits
                if (parameterLimit.LowerWarningLimit != null && parameterLimit.UpperWarningLimit != null &&
                    (dcPointValue < parameterLimit.LowerWarningLimit || dcPointValue > parameterLimit.UpperWarningLimit))
                {
                    // Hold Material
                    material.HoldMaterial(amsOSRAMUtilities.GetConfig<string>(amsOSRAMConstants.DefaultLotIncomingHoldReasonConfig));

                    break;
                }
            }

            // Check if Material doens't have any Hold Reasons associated
            if (material.HoldCount == 0)
            {
                // Protocol
                IProtocol protocol = entityFactory.Create<IProtocol>();
                protocol.Name = amsOSRAMUtilities.GetConfig<string>(amsOSRAMConstants.DefaultSpaceProtocol);

                // Check if protocol exists
                if (protocol.ObjectExists())
                {
                    // Load Protocol
                    protocol.Load();

                    IMaterialCollection materials = entityFactory.CreateCollection<IMaterialCollection>();
                    materials.Add(material);

                    // Create relation between Protocol and Material
                    OpenProtocolInstanceInput openProtocol = new OpenProtocolInstanceInput()
                    {
                        Protocol = protocol,
                        MaterialsToAssociate = materials
                    };

                    IExceptionOrchestration exceptionOrchestration = serviceProvider.GetService<IExceptionOrchestration>();

                    // Open Protocol associated to a Material
                    exceptionOrchestration.OpenProtocolInstance(openProtocol);
                }
            }

            // Create Message to send for Space
            CustomReportEDCToSpace dataCollectionInfoMessage = amsOSRAMUtilities.CreateSpaceInfoWaferValues(material, dataCollectionInstance, dataCollectionLimitSet);

            // Load Xml into XmlDocument to get InnerXml without formatting
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(dataCollectionInfoMessage.SerializeToXML());

            // Publish message on Message Bus
            Utilities.PublishTransactionalMessage(amsOSRAMConstants.CustomReportEDCToSpace,
                                                  JsonConvert.SerializeObject(new
                                                  {
                                                      Message = xmlDocument.InnerXml
                                                  }));

            //---End DEE Code---

            return Input;
        }
    }
}
