using Cmf.Custom.amsOSRAM.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cmf.Custom.amsOSRAM.Actions.Materials
{
    public class CustomMaterialAttributesOnTrackOut : DeeDevBase
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
             *     DEE action responsible to set attributes on Material on TrackOut
             *
             * Action Groups:
             *      BusinessObjects.MaterialCollection.TrackOut.Post
             *
            */

            #endregion Info

            IMaterialCollection materialCollection = amsOSRAMUtilities.GetInputItem<IMaterialCollection>(Input, Navigo.Common.Constants.MaterialCollection);

            return materialCollection.Any(s => s.LastRecipe != null);

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
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];

            IMaterialCollection materialCollection = amsOSRAMUtilities.GetInputItem<IMaterialCollection>(Input, Navigo.Common.Constants.MaterialCollection);
            IMaterialCollection materialCollectionToUpdate = serviceProvider.GetService<IMaterialCollection>();

            materialCollectionToUpdate.AddRange(materialCollection.Where(s => s.LastRecipe != null && s.LastProcessStepResource.ProcessingType == Navigo.BusinessObjects.ProcessingType.Process));

            foreach (IMaterial material in materialCollectionToUpdate)
            {
                material.SaveAttributes(new AttributeCollection
                {
                    { amsOSRAMConstants.MaterialLastProcessRecipeAttribute, material.LastRecipe.Name }
                });
            }

            //---End DEE Code---

            return Input;
        }
    }
}
