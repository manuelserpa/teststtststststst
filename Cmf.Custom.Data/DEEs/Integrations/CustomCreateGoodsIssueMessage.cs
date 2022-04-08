using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Navigo.BusinessObjects;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cmf.Custom.AMSOsram.Actions.Integrations
{
    public class CustomCreateGoodsIssueMessage : DeeDevBase
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

            DeeContext deeContext = DeeContextHelper.SetCurrentServiceContext("CustomCreateGoodsIssueMessage");

            bool isPreAction = deeContext.TriggerPoint == DeeTriggerPoint.Pre;

            MaterialCollection materials = Input["MaterialCollection"] as MaterialCollection;

            if (materials.IsNullOrEmpty())
            {
                return false;
            }

            // Get all the Material with the respective Steps
            Dictionary<long, long> materialsPreActionSteps = new Dictionary<long, long>();

            MaterialCollection availableMaterials = new MaterialCollection();

            foreach (Material material in materials)
            {
                // Check if Material has a ProductionOrder
                if (material.GetNativeValue<long>("ProductionOrder") > 0)
                {
                    if (isPreAction)
                    {
                        materialsPreActionSteps[material.Id] = material.GetNativeValue<long>("Step");
                    }

                    availableMaterials.Add(material);
                }
            }

            if (isPreAction)
            {
                // Save Material(s) as valid(s) with the Step(s) on Pre operation
                DeeContextHelper.SetContextParameter("MaterialsPreActionSteps", materialsPreActionSteps);
            }

            if (!availableMaterials.IsNullOrEmpty())
            {
                // Save the Material(s) as valid(s)
                DeeContextHelper.SetContextParameter("AvailableMaterials", availableMaterials);

                if (!isPreAction)
                {
                    return true;
                }
            }

            return false;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            //System
            UseReference("", "System.Linq");
            UseReference("", "System.Collections.Generic");
            UseReference("", "System.IO");
            UseReference("", "System.Threading");
            UseReference("", "System");

            //Common
            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");

            //Foundation
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");

            //Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");

            //Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");

            Dictionary<long, long> materialsPreActionSteps = new Dictionary<long, long>();

            MaterialCollection availableMaterials = new MaterialCollection();

            Collection<object> integrationEntries = new Collection<object>();

            // Get the Material(s) with the Step(s) of Pre Action
            materialsPreActionSteps = DeeContextHelper.GetContextParameter("MaterialsPreActionSteps") as Dictionary<long, long>;

            // Get the Material(s)
            availableMaterials = DeeContextHelper.GetContextParameter("AvailableMaterials") as MaterialCollection;

            // Clean the Context Parameter
            DeeContextHelper.SetContextParameter("MaterialsPreActionSteps", null);

            // Clean the Context Parameter
            DeeContextHelper.SetContextParameter("AvailableMaterials", null);

            // Get all ProductionOrder Id(s)
            HashSet<long> productionOrderIds = new HashSet<long>();

            // Get all Pre Action Step Id(s)
            HashSet<long> stepPreIds = new HashSet<long>();

            // Get all step Id(s)
            HashSet<long> allStepIds = new HashSet<long>();

            foreach (Material material in availableMaterials)
            {
                long productionOrderId = material.GetNativeValue<long>("ProductionOrder");
                productionOrderIds.Add(productionOrderId);

                long materialCurrentStepId = material.GetNativeValue<long>("Step");
                allStepIds.Add(materialCurrentStepId);

                // add pre action step id on both hashsets
                long materialPreActionStepId = materialsPreActionSteps[material.Id];
                stepPreIds.Add(materialPreActionStepId);
                allStepIds.Add(materialPreActionStepId);
            }

            //---End DEE Code---

            return Input;
        }
    }
}
