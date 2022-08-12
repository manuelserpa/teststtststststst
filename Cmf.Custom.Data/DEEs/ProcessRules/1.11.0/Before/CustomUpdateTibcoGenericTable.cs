using System.Collections.Generic;
using System.Linq;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.GenericTables;

namespace Cmf.Custom.AMSOsram.Actions.ProcessRules._1._11._0.Before
{
    public class CustomUpdateTibcoGenericTable : DeeDevBase
    {
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---
            //System
            UseReference("", "System.Linq");
            UseReference("", "System.Collections.Generic");

            //Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("", "Cmf.Foundation.BusinessObjects.GenericTables");

            //Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");

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
                    Description = "If true, then send the message to a queue. Otherwise, send the message to Tibco topics.",
                    ScalarType = booleanScalarType,
                    ReferenceType = Foundation.Common.ReferenceType.None,
                    Position = lastPropertyPosition,
                    IsMandatory = false
                });
            }

            // Check maptext flag
            if (customTibcoEMSGatewayResolver.GenericTableProperties.FirstOrDefault(e => e.Name == AMSOsramConstants.GenericTableCustomTibcoEMSGatewayResolverTextMessageProperty) == null)
            {
                lastPropertyPosition++;
                propsToAddOrUpdate.Add(new GenericTableProperty
                {
                    Name = AMSOsramConstants.GenericTableCustomTibcoEMSGatewayResolverTextMessageProperty,
                    Description = "If true, then send the message as a Text message. Otherwise, send the message as MapMessage.",
                    ScalarType = booleanScalarType,
                    ReferenceType = Foundation.Common.ReferenceType.None,
                    Position = lastPropertyPosition,
                    IsMandatory = false

                });
            }

            // Check compress flag
            if (customTibcoEMSGatewayResolver.GenericTableProperties.FirstOrDefault(e => e.Name == AMSOsramConstants.GenericTableCustomTibcoEMSGatewayResolverCompressMessageProperty) == null)
            {
                lastPropertyPosition++;
                propsToAddOrUpdate.Add(new GenericTableProperty
                {
                    Name = AMSOsramConstants.GenericTableCustomTibcoEMSGatewayResolverCompressMessageProperty,
                    Description = "If true, then compresses the content of the message.",
                    ScalarType = booleanScalarType,
                    ReferenceType = Foundation.Common.ReferenceType.None,
                    Position = lastPropertyPosition,
                    IsMandatory = false
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
