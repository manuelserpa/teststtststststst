using System.Collections.Generic;
using System.Linq;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.GenericTables;

namespace Cmf.Custom.AMSOsram.Actions.ProcessRules._1._11._0.After
{
    public class CustomUpdateTibcoGenericTable : DeeDevBase
    {
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---


            // Foundation
            UseReference("Cmf.Foundation.Common.dll", "Cmf.Foundation.Common.Base");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.CommunicationLayer.Sap.dll", "Cmf.Foundation.CommunicationLayer.Converters");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration.EntityTypeManagement");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration.EntityTypeManagement.InputObjects");

            GenericTable customTibcoEMSGatewayResolver = new GenericTable()
            {
                Name = AMSOsramConstants.GenericTableCustomTibcoEMSGatewayResolver
            };

            customTibcoEMSGatewayResolver.Load();
            customTibcoEMSGatewayResolver.LoadProperties();

            GenericTablePropertiesCollection propsToAddOrUpdate = new GenericTablePropertiesCollection();

            ScalarType booleanScalarType = new ScalarType();
            booleanScalarType.Load("Bit");
            int lastPropertyPosition = customTibcoEMSGatewayResolver.GenericTableProperties.Count;

            // Check queue flag
            if(customTibcoEMSGatewayResolver.GenericTableProperties.FirstOrDefault(e => e.Name == AMSOsramConstants.GenericTableCustomTibcoEMSGatewayResolverQueueMessageProperty) == null)
            {
                lastPropertyPosition++;
                propsToAddOrUpdate.Add(new GenericTableProperty
                {
                    Name = AMSOsramConstants.GenericTableCustomTibcoEMSGatewayResolverQueueMessageProperty,
                    Description = "Put the message to be sent to Tibco in a Queue.",
                    ScalarType = booleanScalarType,
                    ReferenceType = Foundation.Common.ReferenceType.None,
                    Position = lastPropertyPosition,
                    IsMandatory = true
                });
            }

            // Check maptext flag
            if (customTibcoEMSGatewayResolver.GenericTableProperties.FirstOrDefault(e => e.Name == AMSOsramConstants.GenericTableCustomTibcoEMSGatewayResolverTextMessageProperty) == null)
            {
                lastPropertyPosition++;
                propsToAddOrUpdate.Add(new GenericTableProperty
                {
                    Name = AMSOsramConstants.GenericTableCustomTibcoEMSGatewayResolverTextMessageProperty,
                    Description = "Send message as TextMessage instead of using MapMessage format.",
                    ScalarType = booleanScalarType,
                    ReferenceType = Foundation.Common.ReferenceType.None,
                    Position = lastPropertyPosition,
                    IsMandatory = true

                });
            }

            // Check compress flag
            if (customTibcoEMSGatewayResolver.GenericTableProperties.FirstOrDefault(e => e.Name == AMSOsramConstants.GenericTableCustomTibcoEMSGatewayResolverCompressMessageProperty) == null)
            {
                lastPropertyPosition++;
                propsToAddOrUpdate.Add(new GenericTableProperty
                {
                    Name = AMSOsramConstants.GenericTableCustomTibcoEMSGatewayResolverCompressMessageProperty,
                    Description = "Compress TextMessage.",
                    ScalarType = booleanScalarType,
                    ReferenceType = Foundation.Common.ReferenceType.None,
                    Position = lastPropertyPosition,
                    IsMandatory = true
                });
            }

            if(propsToAddOrUpdate.Count > 0)
            {
                customTibcoEMSGatewayResolver.ManageProperties(propsToAddOrUpdate, new GenericTablePropertiesCollection());
                customTibcoEMSGatewayResolver.GenerateSchema();
            }
            //---End DEE Code---
            return Input;
        }

        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            return true;

            //---End DEE Condition Code---
        }
    }
}
