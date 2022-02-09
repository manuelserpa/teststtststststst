using Cmf.Custom.AMSOsram.Actions;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.Extensions;
using Cmf.Custom.AMSOsram.Orchestration.InputObjects;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cmf.Custom.AMSOsram.Actions.Materials
{
	class CustomMaterialInValidation : DeeDevBase
	{
        /// <summary>
        /// Dee test condition.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <returns></returns>
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *     DEE Action to validate input for the material in service.
             *     Also, tries to retrieve if the current resource is a Sorter and 
             *      if there is a Material to TrackIn.
            */

            /* Action Groups:
             *     N/A
            */

            #endregion

            return true;

            //---End DEE Condition Code---
        }
        /// <summary>
        /// Dee action code.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <returns></returns>
		public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
		{
            //---Start DEE Code---
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("", "Cmf.Foundation.Common.Exceptions");
            UseReference("", "Cmf.Foundation.Common");
            // Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            // Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("", "Cmf.Custom.AMSOsram.Common.Extensions");
            UseReference("Cmf.Custom.AMSOsram.Orchestration.dll", "Cmf.Custom.AMSOsram.Orchestration.InputObjects");


            bool isSorter = false;

            if (!Input.ContainsKey("MaterialInInput"))
            {
                throw new CmfBaseException("Not a valid MaterialInInput Input!");
            }

            MaterialInInput input = Input["MaterialInInput"] as MaterialInInput;

            Cmf.Foundation.Common.Utilities.ValidateNullInput(input);

            if (string.IsNullOrWhiteSpace(input.ResourceName))
            {
                throw new MissingMandatoryFieldCmfException("ResourceName");
            }

            Resource resource = new Resource() { Name = input.ResourceName };

            if (!resource.ObjectExists())
            {
                throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Resource, input.ResourceName);
            }

            resource.Load();
            resource.LoadAttributes(new Collection<string> { AMSOsramConstants.ResourceAttributeIsSorter });

            if (resource.Attributes != null &&
                resource.Attributes.ContainsKey(AMSOsramConstants.ResourceAttributeIsSorter) &&
                resource.Attributes[AMSOsramConstants.ResourceAttributeIsSorter] != null)
            {
                resource.Attributes.TryGetValueAs(AMSOsramConstants.ResourceAttributeIsSorter, out isSorter);
            }

            Material waferToTrackIn = null;

            // Material Name takes precedence over ContainerId
            if (!string.IsNullOrWhiteSpace(input.MaterialName))
            {
                waferToTrackIn = new Material() { Name = input.MaterialName };

                if (!waferToTrackIn.ObjectExists())
                {
                    throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Material, input.MaterialName);
                }
            }
            else if (!string.IsNullOrWhiteSpace(input.CarrierId))
            {
                Container container = new Container() { Name = input.CarrierId };

                if (!container.ObjectExists())
                {
                    throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Container, input.CarrierId);
                }

                container.Load();
                container.LoadRelations("MaterialContainer");

                // Assuming one container only has one Parent Material
                if (container.ContainerMaterials != null && container.ContainerMaterials.Count > 0)
                {
                    waferToTrackIn = container.ContainerMaterials.First().SourceEntity.TopMostMaterial ?? container.ContainerMaterials.First().SourceEntity;
                }
            }

            Input.Add("Resource", resource);
            Input.Add("Material", waferToTrackIn);
            Input.Add("IsSorter", isSorter.ToString());

            //---End DEE Code---

            return Input;
        }
    }
}
