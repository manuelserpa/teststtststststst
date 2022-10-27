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
using Cmf.Navigo.BusinessObjects;

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

                IDataCollectionInstance dataCollectionInstance = amsOSRAMUtilities.GetInputItem<IDataCollectionInstance>(Input, Navigo.Common.Constants.DataCollectionInstance);

                // Check if Input data returns DataCollection and associated DataCollection Limit Sets
                if (reportEDCToSpace && dataCollectionInstance != null && dataCollectionInstance.DataCollectionLimitSet != null)
                {
                    dataCollectionInstance.Step.Load();

                    // The Action only executed if Step needs Space confirmation
                    if (dataCollectionInstance.Step.HasAttribute(amsOSRAMConstants.StepAttributeRequiresSpaceConfirmation, true) &&
                        (bool)dataCollectionInstance.Step.GetAttributeValue(amsOSRAMConstants.StepAttributeRequiresSpaceConfirmation))
                    {
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

            // Get DataCollectionInstance from Input
            IDataCollectionInstance dataCollectionInstance = amsOSRAMUtilities.GetInputItem<IDataCollectionInstance>(Input, Navigo.Common.Constants.DataCollectionInstance);

            if (dataCollectionInstance is null)
            {
                throw new CmfBaseException("Cannot be possible to get DataCollectionInstance data from Input.");
            }

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            dataCollectionInstance.Load();
            dataCollectionInstance.LoadRelations(Navigo.Common.Constants.DataCollectionPoint);

            IDataCollectionLimitSet dataCollectionLimitSet = dataCollectionInstance.DataCollectionLimitSet;
            dataCollectionLimitSet.LoadRelations(Navigo.Common.Constants.DataCollectionParameterLimit);

            // Check if Material associated to DataCollection have a Parent Material
            IMaterial material = dataCollectionInstance.Material.ParentMaterial != null 
                ? dataCollectionInstance.Material.ParentMaterial 
                : dataCollectionInstance.Material;


            material.Load();

            // Check limits of Data Collection Points
            foreach (IDataCollectionPoint dataCollectionPoint in dataCollectionInstance.DataCollectionPoints)
            {
                IDataCollectionParameterLimit parameterLimit = dataCollectionLimitSet.DataCollectionParameterLimits?.FirstOrDefault(limit => limit.TargetEntity.Name == dataCollectionPoint.TargetEntity.Name);

                if (parameterLimit == null)
                {
                    continue;
                }

                decimal dcPointValue = amsOSRAMUtilities.GetValueAsDecimal(dataCollectionPoint.Value.ToString());

                if (parameterLimit.LowerErrorLimit != null && parameterLimit.UpperErrorLimit != null &&
                    (dcPointValue < parameterLimit.LowerErrorLimit || dcPointValue > parameterLimit.UpperErrorLimit))
                {
                    material.HoldMaterial(amsOSRAMUtilities.GetConfig<string>(amsOSRAMConstants.DefaultLotIncomingHoldReasonConfig));

                    break;
                }

                if (parameterLimit.LowerWarningLimit != null && parameterLimit.UpperWarningLimit != null &&
                    (dcPointValue < parameterLimit.LowerWarningLimit || dcPointValue > parameterLimit.UpperWarningLimit))
                {
                    material.HoldMaterial(amsOSRAMUtilities.GetConfig<string>(amsOSRAMConstants.DefaultLotIncomingHoldReasonConfig));

                    break;
                }
            }

            IProtocolInstance protocolInstance = null;

            if (material.HoldCount == 0)
            {
                IProtocol protocol = entityFactory.Create<IProtocol>();
                protocol.Name = amsOSRAMUtilities.GetConfig<string>(amsOSRAMConstants.DefaultSpaceProtocol);

                if (protocol.ObjectExists())
                {
                    protocol.Load();

                    IMaterialCollection materials = entityFactory.CreateCollection<IMaterialCollection>();
                    materials.Add(material);

                    OpenProtocolInstanceInput openProtocol = new OpenProtocolInstanceInput()
                    {
                        Protocol = protocol,
                        MaterialsToAssociate = materials
                    };

                    IExceptionOrchestration exceptionOrchestration = serviceProvider.GetService<IExceptionOrchestration>();

                    protocolInstance = exceptionOrchestration.OpenProtocolInstance(openProtocol).ProtocolInstance;
                }
            }

            // Create Message to send for Space
            CustomReportEDCToSpace dataCollectionInfoMessage = amsOSRAMSpaceUtilities.CreateSpaceInfoWaferValues(material, dataCollectionInstance, dataCollectionLimitSet);

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(dataCollectionInfoMessage.SerializeToXML());

            Utilities.PublishTransactionalMessage(amsOSRAMConstants.CustomReportEDCToSpace,
                                                  JsonConvert.SerializeObject(new
                                                  {
                                                      Message = xmlDocument.InnerXml,
                                                      Context = new { 
                                                          Subject = amsOSRAMConstants.CustomReportEDCToSpace,
                                                          Lot = material.Name,
                                                          ProtocolInstance = protocolInstance?.Name,
                                                          ActionGroupName = amsOSRAMUtilities.GetInputItem<string>(Input, "ActionGroupName"),
                                                      }
                                                  }));

            //---End DEE Code---

            return Input;
        }
    }
}
