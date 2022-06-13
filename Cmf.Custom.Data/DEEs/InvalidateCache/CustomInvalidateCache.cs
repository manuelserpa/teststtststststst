using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.Common;
using System;
using System.Collections.Generic;

namespace Cmf.Custom.AMSOsram.Actions.InvalidateCache
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

            string genericTableName = AMSOsramUtilities.GetInputItem<GenericTable>(Input, "GenericTable").Name;

            if (!string.Equals(genericTableName, AMSOsramConstants.GenericTableCustomTibcoEMSGatewayResolver))
            {
                return false;
            }

            return true;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            //System
            UseReference("", "System");
            UseReference("", "System.Collections.Generic");

            //Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.GenericTables");
            UseReference("Cmf.Foundation.Common.dll", "Cmf.Foundation.Common");

            //Common
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");

            string actionGroupName = AMSOsramUtilities.GetInputItem<string>(Input, "ActionGroupName");

            if (!string.IsNullOrWhiteSpace(actionGroupName))
            {
                if (string.Equals(actionGroupName, "GenericTables.GenericTable.InsertOrUpdateRows.Post", StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(actionGroupName, "GenericTables.GenericTable.RemoveRows.Post", StringComparison.InvariantCultureIgnoreCase) &&
                    Input.ContainsKey("GenericTable"))
                {
                    GenericTable genericTable = AMSOsramUtilities.GetInputItem<GenericTable>(Input, "GenericTable");

                    Utilities.PublishMessage($"Custom.UpdateGenericTable.{genericTable.Name}");
                }
            }

            //---End DEE Code---

            return Input;
        }
    }
}
