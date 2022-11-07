using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Foundation.BusinessObjects.LocalizationService;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Foundation.Common.LocalizationService;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Navigo.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Cmf.Custom.amsOSRAM.Actions.Materials
{
    public class CustomValidateMaterialReceptionSubstrate : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *     Dee action responsible for validating that the wafer can proceed with the transfer. It is only possible if below conditions are met:
             *      - Wafer ID is available and registered in MES
             *      - Wafer is at wafer reception step and proceed with substrate
             *      - wafer is active and not on hold
             *      - Wafer's product must match the product of all wafers in the destination container.
             *
             * Action Groups:
             *
            */

            #endregion Info

            return true;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            // Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.SmartTables");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration.Abstractions");
            UseReference("Cmf.Foundation.Common.dll", "Cmf.Foundation.Common.LocalizationService");

            // Navigo
            UseReference("Cmf.Navigo.Common.dll", "Cmf.Navigo.Common");

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.Extensions");

            // System
            UseReference("", "System.Threading");

            #region Check if all arguments exist

            if (!Input.TryGetValueAs("WaferID", out string waferID))
            {
                throw new ArgumentNullCmfException("WaferID");
            }

            if (!Input.TryGetValueAs("TargetContainer", out string targetContainerName))
            {
                throw new ArgumentNullCmfException("TargetContainer");
            }

            if (!Input.TryGetValueAs("SorterProcess", out string sorterProcess))
            {
                sorterProcess = "WaferReception";
            }

            #endregion Check if all arguments exist

            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();
            ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();

            #region Validate arguments

            IMaterial wafer = entityFactory.Create<IMaterial>();
            wafer.Name = waferID;

            if (!wafer.ObjectExists())
            {
                throw new ObjectNotFoundCmfException("Material", wafer.Name);
            }

            IContainer targetContainer = entityFactory.Create<IContainer>();
            targetContainer.Name = targetContainerName;

            if (!targetContainer.ObjectExists())
            {
                throw new ObjectNotFoundCmfException("Container", targetContainer.Name);
            }

            #endregion Validate arguments

            #region Validate the Wafer

            wafer.Load();

            if (wafer.UniversalState != Cmf.Foundation.Common.Base.UniversalState.Active)
            {
                throw new ObjectNotActiveCmfException(wafer.Name);
            }

            if (wafer.HoldCount > 0)
            {
                throw new ObjectIsOnHoldCmfException("Material", wafer.Name);
            }

            #endregion Validate the Wafer

            #region Validate the Wafer Step

            IStep materialStep = wafer.Step;
            materialStep.LoadAttributes(new Collection<string>() { amsOSRAMConstants.StepAttributeIsWaferReception });

            if (!materialStep.Attributes.TryGetValueAs(amsOSRAMConstants.StepAttributeIsWaferReception, out bool isWaferReception) || isWaferReception == false)
            {
                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageCustomStepNoWaferReception), materialStep.Name, amsOSRAMConstants.StepAttributeIsWaferReception));
            }

            #endregion Validate the Wafer Step

            #region Validate the Container

            targetContainer.Load();
            targetContainer.LoadRelations(Navigo.Common.Constants.MaterialContainer);

            bool isTargetContainerLegible = targetContainer.ContainerMaterials == null ||
                            targetContainer.ContainerMaterials.Count == 0 ||
                            !targetContainer.ContainerMaterials.Any(s => s.SourceEntity.Product.Name != wafer.Product.Name);

            if (!isTargetContainerLegible)
            {
                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageCustomContainerDifferentProducts), targetContainer.Name, wafer.Product.Name));
            }

            #endregion Validate the Container

            //---End DEE Code---

            return Input;
        }
    }
}
