using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessOrchestration.Abstractions;
using Cmf.Navigo.BusinessOrchestration.ExceptionManagement.InputObjects;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Cmf.Custom.amsOSRAM.Actions.Space
{
    public class CustomTibcoEMSReplyHandler : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---   

            #region Info
            /// <summary>
            /// Summary text
            ///     DEE Action to handle the reply send from Tibco EMS.
            ///
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion

            bool canExecute = true;

            return canExecute;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            //System
            UseReference("%MicrosoftNetPath%System.Private.Xml.dll", "System.Xml");
            UseReference("%MicrosoftNetPath%System.Private.Xml.Linq.dll", "System.Xml.Linq");
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

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();;

            string replyMessage = amsOSRAMUtilities.GetInputItem<string>(Input, "ReplyMessage");
            Dictionary<string, object> context = amsOSRAMUtilities.GetInputItem<Dictionary<string, object>>(Input, "Context");

            if (context != null && context.TryGetValueAs("Subject", out string contextSubject))
            {
                switch (contextSubject)
                {
                    case amsOSRAMConstants.CustomReportEDCToSpace:
                        if (String.IsNullOrWhiteSpace(replyMessage))
                        {
                            break;
                        }

                        context.TryGetValueAs("ProtocolInstance", out string protocolInstanceName);

                        IProtocolInstance protocolInstance = serviceProvider.GetService<IProtocolInstance>();
                        protocolInstance.Load(protocolInstanceName);

                        if (protocolInstance.UniversalState != Foundation.Common.Base.UniversalState.Active)
                        {
                            break;
                        }

                        CustomReportEDCToSpaceResponse customReportEDCToSpaceResponse = amsOSRAMUtilities.DeserializeXmlToObject<CustomReportEDCToSpaceResponse>(replyMessage);

                        bool uploadSuccess = customReportEDCToSpaceResponse.UploadSuccess;
                        bool validationSuccess = customReportEDCToSpaceResponse.ValidationSuccess;
                        bool anyFailed = customReportEDCToSpaceResponse.Samples.Any(sample => sample.Accepted == false);

                        // If all conditions are true, close the protocol
                        if (uploadSuccess && validationSuccess && !anyFailed)
                        {
                            IExceptionOrchestration exceptionOrchestration = serviceProvider.GetService<IExceptionOrchestration>();

                            CloseProtocolInstanceInput closeProtocol = new CloseProtocolInstanceInput()
                            {
                                ProtocolInstance = protocolInstance
                            };
                            exceptionOrchestration.CloseProtocolInstance(closeProtocol);

                            context.TryGetValueAs("Lot", out string lotName);
                            context.TryGetValueAs("ActionGroupName", out string actionGroupName);
                            
                            if (actionGroupName == "MaterialManagement.MaterialManagementOrchestration.ComplexTrackOutAndMoveMaterialsToNextStep.Post")
                            {
                                IMaterial material = entityFactory.Create<IMaterial>();
                                material.Name = lotName;
                                material.Load();

                                material.MoveToNextStep(material.FlowPath);
                            }

                        }

                        break;

                    default:
                        break;
                }
            }

            //---End DEE Code---

            return Input;
        }
    }
}
