using System.Collections.Generic;
using System.Linq;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.BusinessObjects;

namespace Cmf.Custom.amsOSRAM.Actions.ProcessRules._1._11._0.Before
{
    public class CustomUpdateTibcoGenericTable : DeeDevBase
    {
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            //Foundation
            UseReference("", "Cmf.Foundation.BusinessObjects.GenericTables");

            //Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");

            IGenericTable customTibcoEMSGatewayResolver = new GenericTable()
            {
                Name = amsOSRAMConstants.GenericTableCustomTibcoEMSGatewayResolver
            };

            customTibcoEMSGatewayResolver.Load();
            customTibcoEMSGatewayResolver.LoadProperties();

            IGenericTablePropertiesCollection propsToAddOrUpdate = new GenericTablePropertiesCollection();

            IScalarType booleanScalarType = new ScalarType();
            booleanScalarType.Load("Bit");
            int lastPropertyPosition = customTibcoEMSGatewayResolver.GenericTableProperties.Count;

            // Check queue flag
            if(customTibcoEMSGatewayResolver.GenericTableProperties.FirstOrDefault(e => e.Name == amsOSRAMConstants.GenericTableCustomTibcoEMSGatewayResolverQueueMessageProperty) == null)
            {
                lastPropertyPosition++;
                propsToAddOrUpdate.Add(new GenericTableProperty
                {
                    Name = amsOSRAMConstants.GenericTableCustomTibcoEMSGatewayResolverQueueMessageProperty,
                    Description = "If true, then send the message to a queue. Otherwise, send the message to Tibco topics.",
                    ScalarType = booleanScalarType,
                    ReferenceType = Foundation.Common.ReferenceType.None,
                    Position = lastPropertyPosition,
                    IsMandatory = true,
                    DefaultValue = false
                });
            }

            // Check maptext flag
            if (customTibcoEMSGatewayResolver.GenericTableProperties.FirstOrDefault(e => e.Name == amsOSRAMConstants.GenericTableCustomTibcoEMSGatewayResolverTextMessageProperty) == null)
            {
                lastPropertyPosition++;
                propsToAddOrUpdate.Add(new GenericTableProperty
                {
                    Name = amsOSRAMConstants.GenericTableCustomTibcoEMSGatewayResolverTextMessageProperty,
                    Description = "If true, then send the message as a Text message. Otherwise, send the message as MapMessage.",
                    ScalarType = booleanScalarType,
                    ReferenceType = Foundation.Common.ReferenceType.None,
                    Position = lastPropertyPosition,
                    IsMandatory = true,
                    DefaultValue = false

                });
            }

            // Check compress flag
            if (customTibcoEMSGatewayResolver.GenericTableProperties.FirstOrDefault(e => e.Name == amsOSRAMConstants.GenericTableCustomTibcoEMSGatewayResolverCompressMessageProperty) == null)
            {
                lastPropertyPosition++;
                propsToAddOrUpdate.Add(new GenericTableProperty
                {
                    Name = amsOSRAMConstants.GenericTableCustomTibcoEMSGatewayResolverCompressMessageProperty,
                    Description = "If true, then compresses the content of the message.",
                    ScalarType = booleanScalarType,
                    ReferenceType = Foundation.Common.ReferenceType.None,
                    Position = lastPropertyPosition,
                    IsMandatory = true,
                    DefaultValue = false
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
