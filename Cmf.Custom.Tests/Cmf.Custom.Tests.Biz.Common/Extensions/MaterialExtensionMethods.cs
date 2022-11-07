using Cmf.Custom.TestUtilities;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cmf.Custom.Tests.Biz.Common.Extensions
{
    public static class MaterialExtensionMethods
    {
        #region Material methods
        /// <summary>
        /// Changes the quantity of the material without record actual loss/bonus
        /// </summary>
        /// <param name="material"> The material whose quantity will be changed</param>
        /// <param name="primQtyOffset">Primary quantity offset (add or remove quantity)</param>
        /// <param name="secQtyOffset">Secondary quantity offset (add or remove quantity)</param>
        public static void ChangeQuantity(this Material material, decimal primQtyOffset = 0, decimal secQtyOffset = 0)
        {
            var materialQtyChangeObj = new MaterialQuantityChange()
            {
                NewPrimaryQuantity = material.PrimaryQuantity + primQtyOffset,
                Material = material
            };
            if (!String.IsNullOrEmpty(material.SecondaryUnits))
                materialQtyChangeObj.NewSecondaryQuantity = material.SecondaryQuantity + secQtyOffset;

            material.CopyFrom(new ChangeMaterialQuantityInput()
            {
                MaterialQuantityChange = materialQtyChangeObj,
                IgnoreLastServiceId = true
            }.ChangeMaterialQuantitySync().Material);
        }
        /// <summary>
        /// Loads the children for the input parent material
        /// </summary>
        /// <param name="parent"></param>
        public static void LoadChildren(this Material parent)
        {
            parent.CopyFrom(Cmf.TestScenarios.MaterialManagement.MaterialScenarios.MaterialScenario.LoadChildren(parent));
        }

        /// <summary>
        /// Expands a material into a set of submaterials based on primary quantity
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="expandForm"></param>
        public static void Expand(this Material instance, string expandForm)
        {
            if (instance != null && instance.ObjectExists() && instance.PrimaryQuantity > 0)
            {
                int numberOfSubmaterials = (int)Math.Ceiling(instance.PrimaryQuantity.GetValueOrDefault());
                decimal? secondaryQuantityPerChild = instance.SecondaryQuantity != null ? instance.SecondaryQuantity / numberOfSubmaterials : null;

                // create n sub materials
                MaterialCollection subMaterials = new MaterialCollection();
                for (int iPos = 1; iPos <= numberOfSubmaterials; iPos++)
                {
                    subMaterials.Add(new Material()
                    {
                        Name = String.Format("{0}-{1}", instance.Name, iPos.ToString().PadLeft(2, '0')),
                        PrimaryQuantity = 1,
                        SecondaryQuantity = secondaryQuantityPerChild
                    });
                }

                ExpandMaterialOutput expandMaterialOutput = new ExpandMaterialInput()
                {
                    Material = instance,
                    SubMaterials = subMaterials,
                    Form = expandForm
                }.ExpandMaterialSync();

                // perform expand
                instance.CopyFrom(expandMaterialOutput.Material);

                instance.SubMaterials = expandMaterialOutput.ExpandedSubMaterials;
            }
        }

        /// <summary>
        /// Extension to Hold a Material with Hold Reason Comment
        /// </summary>
        /// <param name="instance">The material to be out on Hold</param>
        /// <param name="holdReasons">All the reasons and corresponding hold comments</param>
        public static void HoldMaterial(this Material instance, Dictionary<Reason, string> holdReasons, string comment = "ReleaseComment")
        {
            MaterialHoldReasonCollection holdReasonsCollection = new MaterialHoldReasonCollection();

            foreach (KeyValuePair<Reason, string> holdReason in holdReasons)
            {
                holdReasonsCollection.Add(new MaterialHoldReason()
                {
                    SourceEntity = instance,
                    TargetEntity = holdReason.Key,
                    Comment = holdReason.Value
                });
            }

            new HoldMaterialInput()
            {
                ServiceComments = comment,
                Material = instance,
                IgnoreLastServiceId = true,
                MaterialHoldReasonCollection = holdReasonsCollection
            }.HoldMaterialSync();

        }

        /// <summary>
        /// Extension method to change material flow and step
        /// </summary>
        /// <param name="instance">The material instance</param>
        /// <param name="flowName">The target flow name</param>
        /// <param name="stepName">The target step name</param>
        public static void ChangeFlowAndStep(this Material instance, string flowName, string stepName)
        {
            if (instance == null || string.IsNullOrWhiteSpace(flowName) || string.IsNullOrWhiteSpace(stepName))
            {
                throw new ArgumentNullException("Invalid arguments");
            }

            Flow flow = new Flow() { Name = flowName };
            Step step = new Step() { Name = stepName };

            ChangeMaterialFlowAndStepInput changeMaterialFlowAndStepInput = new ChangeMaterialFlowAndStepInput();
            changeMaterialFlowAndStepInput.Material = instance;
            changeMaterialFlowAndStepInput.Flow = flow;
            changeMaterialFlowAndStepInput.Step = step;
            changeMaterialFlowAndStepInput.FlowPath = FlowExtensionMethods.CustomGetFlowPath(flow, stepName);
            changeMaterialFlowAndStepInput.IgnoreLastServiceId = true;
            var output = changeMaterialFlowAndStepInput.ChangeMaterialFlowAndStepSync();

            instance.CopyFrom(output.Material);
        }

        #endregion
    }
}
