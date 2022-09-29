using Cmf.Custom.amsOSRAM.Common;
using Cmf.Foundation.Common;
using System;
using System.Collections.Generic;
using Cmf.Foundation.BusinessObjects.Abstractions;

namespace Cmf.Custom.amsOSRAM.Actions.InvalidateCache
{
    public class CustomInvalidateCache : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---   

            #region Info
            /// <summary>
            /// Summary text
            ///     
            /// Action Groups:
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion

            string actionGroupName = amsOSRAMUtilities.GetInputItem<string>(Input, "ActionGroupName");

            if (!string.IsNullOrWhiteSpace(actionGroupName))
            {
                if (string.Equals(actionGroupName, "GenericTables.GenericTable.InsertOrUpdateRows.Post", StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(actionGroupName, "GenericTables.GenericTable.RemoveRows.Post", StringComparison.InvariantCultureIgnoreCase) &&
                    Input.ContainsKey(Constants.GenericTable))
                {
                    IGenericTable genericTable = amsOSRAMUtilities.GetInputItem<IGenericTable>(Input, Constants.GenericTable);

                    if (genericTable != null)
                    {
                        return string.Equals(genericTable.Name, amsOSRAMConstants.GenericTableCustomTibcoEMSGatewayResolver);
                    }
                }
            }

            return false;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            //Common
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");

            Utilities.PublishMessage(amsOSRAMConstants.CustomTibcoEMSGatewayInvalidateCache);

            //---End DEE Code---

            return Input;
        }
    }
}
