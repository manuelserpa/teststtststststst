using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Custom.amsOSRAM.Orchestration.InputObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cmf.Custom.amsOSRAM.Actions.Materials
{
    public class CustomMaterialInValidation : DeeDevBase
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

            #endregion Info

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

            // Custom
            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.Extensions");
            UseReference("Cmf.Custom.amsOSRAM.Orchestration.dll", "Cmf.Custom.amsOSRAM.Orchestration.InputObjects");

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            bool isSorter = false;

            if (!Input.ContainsKey("MaterialInInput"))
            {
                throw new CmfBaseException("Not a valid MaterialInInput Input!");
            }

            MaterialInInput input = Input["MaterialInInput"] as MaterialInInput;

            Utilities.ValidateNullInput(input);

            if (string.IsNullOrWhiteSpace(input.ResourceName))
            {
                throw new MissingMandatoryFieldCmfException("ResourceName");
            }

            IResource resource = entityFactory.Create<IResource>();
            resource.Name = input.ResourceName;

            if (!resource.ObjectExists())
            {
                throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Resource, input.ResourceName);
            }

            resource.Load();
            resource.LoadAttributes(new Collection<string> { amsOSRAMConstants.ResourceAttributeIsSorter });

            if (resource.Attributes != null &&
                resource.Attributes.ContainsKey(amsOSRAMConstants.ResourceAttributeIsSorter) &&
                resource.Attributes[amsOSRAMConstants.ResourceAttributeIsSorter] != null)
            {
                resource.Attributes.TryGetValueAs(amsOSRAMConstants.ResourceAttributeIsSorter, out isSorter);
            }

            IMaterial waferToTrackIn = null;

            // Material Name takes precedence over ContainerId
            if (!string.IsNullOrWhiteSpace(input.MaterialName))
            {
                waferToTrackIn = entityFactory.Create<IMaterial>();
                waferToTrackIn.Name = input.MaterialName;

                if (!waferToTrackIn.ObjectExists())
                {
                    throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Material, input.MaterialName);
                }
            }
            else if (!string.IsNullOrWhiteSpace(input.CarrierId))
            {
                IContainer container = entityFactory.Create<IContainer>();
                container.Name = input.CarrierId;

                if (!container.ObjectExists())
                {
                    throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Container, input.CarrierId);
                }

                container.Load();
                container.LoadRelations("MaterialContainer");

                // Assuming one container only has one Parent Material
                if (container.ContainerMaterials != null && container.ContainerMaterials.Count > 0)
                {
                    IMaterial possibleLotToTrackIn = container.ContainerMaterials.First().SourceEntity.TopMostMaterial ?? container.ContainerMaterials.First().SourceEntity;

                    if (possibleLotToTrackIn.Step.HasAttribute(amsOSRAMConstants.StepAttributeIsWaferReception, true) &&
                        !possibleLotToTrackIn.Step.GetAttributeValueOrDefault<bool>(amsOSRAMConstants.StepAttributeIsWaferReception, false, false))
                    {
                        waferToTrackIn = possibleLotToTrackIn;
                    }
                }
            }

            Input.Add("Resource", resource);
            Input.Add("Material", waferToTrackIn);
            Input.Add("IsSorter", isSorter);

            //---End DEE Code---

            return Input;
        }
    }
}
