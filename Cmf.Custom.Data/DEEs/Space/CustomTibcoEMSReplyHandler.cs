using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessOrchestration.Abstractions;
using Cmf.Navigo.BusinessOrchestration.ExceptionManagement.InputObjects;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            string replyMessageMockUp = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
 <response uploadSuccess=""true"" validationSuccess=""true"">
     <samples accepted=""true"" excluded=""0"" parameterName=""NZ-A8-AM-Flattening-MS-Test"" parameterUnit=""mm"" sampleId=""215916825"">
         <keys>
             <key id=""1"" name=""Materialsystem"" type=""ExKey"">InGaN</key>
             <key id=""2"" name=""Equipment"" type=""ExKey"">EQ1</key>
         </keys>
         <channelCkdIds>
             <channelCkdlds channelId=""119178"" ckcId=""1""/>
         </channelCkdIds>
         <violations>
             <violation id=""1"" name=""Raw above specification""/>
         </violations>
     </samples>
     <samples accepted=""true"" excluded=""0"" parameterName=""NZ-A8-AM-Flattening-MS-Test"" parameterUnit=""mm"" sampleId=""215916826"">
         <keys>
             <key id=""1"" name=""Materialsystem"" type=""ExKey"">InGaN</key>
             <key id=""2"" name=""Equipment"" type=""ExKey"">EQ1</key>
         </keys>
         <channelCkdIds>
             <channelCkdlds channelId=""119178"" ckcId=""1""/>
         </channelCkdIds>
         <violations>
             <violation id=""1"" name=""Raw above specification""/>
         </violations>
     </samples>
 </response>";

            XDocument xml = XDocument.Parse(replyMessageMockUp);
            

            IDictionary<string, object> contextMockUp = new Dictionary<string, object>(){
                {"Subject", "CustomReportEDCToSpace"},
                {"Lot", "LotProposal_001"},
                {"ProtocolInstance", "8D-000000003"},
                {"ActionGroupName", "MaterialManagement.MaterialManagementOrchestration.ComplexTrackOutMaterials.Post"}
            };


            string replyMessage = xml.ToString();//amsOSRAMUtilities.GetInputItem<string>(Input, "ReplyMessage");
            IDictionary<string, object> context = contextMockUp;//amsOSRAMUtilities.GetInputItem<IDictionary<string, object>>(Input, "Context");

            if (context != null)
            {
                string contextSubject = context["Subject"].ToString();

                switch (contextSubject)
                {
                    case amsOSRAMConstants.CustomReportEDCToSpace:

                        string lotName = context["Lot"].ToString();
                        string protocolInstanceName = context["ProtocolInstance"].ToString();
                        string actionGroupName = context["ActionGroupName"].ToString();

                        IProtocolInstance protocolInstance = serviceProvider.GetService<IProtocolInstance>();
                        protocolInstance.Load(protocolInstanceName);
                        if(protocolInstance.UniversalState != Foundation.Common.Base.UniversalState.Active)
                        {
                            break;
                        }

                        bool allowMoveNext = false;
                        bool uploadSuccess = false;
                        bool validationSuccess = false;
                        bool accepted = false;
                        
                        if (!string.IsNullOrWhiteSpace(replyMessage))
                        {
                            int outsideLimitCount = 0;
                            try
                            {
                                CustomReportEDCToSpaceResponse customReportEDCToSpaceResponse = amsOSRAMUtilities.DeserializeXmlToObject<CustomReportEDCToSpaceResponse>(replyMessage);
                                uploadSuccess = customReportEDCToSpaceResponse.UploadSuccess;
                                validationSuccess = customReportEDCToSpaceResponse.ValidationSuccess;

                                foreach (var sample in customReportEDCToSpaceResponse.Samples)
                                {
                                    if (!sample.Accepted)
                                    {
                                        outsideLimitCount++;
                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                throw new Exception("Xml validation failed.");
                            }
                            
                            
                            if (outsideLimitCount == 0)
                            {
                                accepted = true;
                            }
                        }

                        // If all conditions are true, close the protocol
                        if (uploadSuccess && validationSuccess && accepted)
                        {
                            

                            IExceptionOrchestration exceptionOrchestration = serviceProvider.GetService<IExceptionOrchestration>();

                            CloseProtocolInstanceInput closeProtocol = new CloseProtocolInstanceInput()
                            {
                                ProtocolInstance = protocolInstance
                            };
                            exceptionOrchestration.CloseProtocolInstance(closeProtocol);

                            allowMoveNext = true;

                        }

                        if (allowMoveNext && actionGroupName == "MaterialManagement.MaterialManagementOrchestration.ComplexTrackOutAndMoveMaterialsToNextStep.Post")
                        {
                            IMaterial material = entityFactory.Create<IMaterial>();
                            material.Name = lotName;
                            material.Load();

                            material.MoveToNextStep(material.FlowPath);
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
