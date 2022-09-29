using Cmf.Custom.amsOSRAM.Common;
using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.Configuration.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Foundation.Common.LocalizationService;
using Cmf.Navigo.BusinessObjects;

namespace Cmf.Custom.amsOSRAM.Actions.Automation
{
	public class CustomSetHoldEntity : DeeDevBase
	{
		public override bool DeeTestCondition(Dictionary<string, object> Input)
		{
			//---Start DEE Condition Code---

			#region Info

			/* Description:
             *     DEE Action is responsible to set an entity on hold.
             *     That entity can be either a Container or a Material.
             *
             * Action Groups:
             *      None
             *
            */

			#endregion Info

			return true;

			//---End DEE Condition Code---
		}

		public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
		{
            //---Start DEE Code---

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");

            // System
            UseReference("%MicrosoftNetPath%System.Private.CoreLib.dll", "System.Threading");

            // Foundation
            UseReference("", "Cmf.Foundation.Common.LocalizationService");

            IContainer container = null;
            IMaterial material = null;
            bool setMaterialOnHold = false;

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            #region Input Validation

            if (Input.ContainsKey("MaterialName") &&
                Input["MaterialName"] != null &&
                Input["MaterialName"] is string strMaterialName)
            {
                material = entityFactory.Create<IMaterial>();
                material.Name = strMaterialName;

                material.Load();
            }

            if (Input.ContainsKey("ContainerName") &&
                Input["ContainerName"] != null &&
                Input["ContainerName"] is string strContainerName)
            {
                container = entityFactory.Create<IContainer>();
                container.Name = strContainerName;

                container.Load();
            }

            if (Input.ContainsKey("SetMaterialOnContainerOnHold") &&
                Input["SetMaterialOnContainerOnHold"] != null &&
                Input["SetMaterialOnContainerOnHold"] is string strMaterialOnHold)
            {
                setMaterialOnHold = bool.Parse(strMaterialOnHold);
            }

            #endregion Input Validation

            if (container != null && setMaterialOnHold)
            {
                container.LoadRelations("MaterialContainer");
                IMaterialContainer firstMaterialContainer = container.ContainerMaterials.FirstOrDefault();

                if (firstMaterialContainer != null)
                {
                    material = firstMaterialContainer.SourceEntity.GetTopMostMaterial();
                }
            }

            if (material != null)
            {
                // We only want to set Material Lots on hold
                if (material.Form != "Lot")
                {
                    throw new InvalidPropertyValueCmfException(material.Form, "Form", "Lot");
                }

                // Hold Step Reason config
                if (!Config.TryGetConfig(amsOSRAMConstants.DefaultAbortProcessHoldReasonConfig, out IConfig abortProcessHoldReason)
                    || string.IsNullOrWhiteSpace(abortProcessHoldReason.GetConfigValue<string>()))
                {
                    ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();
                    throw new CmfBaseException(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageNoHoldReasonAvailableErrorMessage));
                }


                IReason reason = entityFactory.Create<IReason>();
                reason.Name = abortProcessHoldReason.Value.ToString();

                // Validate if hold reason exists
                if (!reason.ObjectExists())
                {
                    throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Reason, abortProcessHoldReason.Value.ToString());
                }

                // Load hold reason
                reason.Load();

                // Check if the reason is already set
                if (material.HoldCount > 0)
                {
                    material.LoadRelations(Navigo.Common.Constants.MaterialHoldReason);

                    if (material.MaterialHoldReasons != null)
                    {
                        IMaterialHoldReason mhReason = material.MaterialHoldReasons.FirstOrDefault(mh => mh.TargetEntity.Id == reason.Id);

                        if (mhReason != null && !mhReason.TargetEntity.EnableConcurrentInstances)
                        {
                            throw new CmfBaseException("Material already has the hold reason set.");
                        }
                    }
                }

                // Reason must be a hold reason
                if (reason.ReasonType != ReasonType.Hold)
                {
                    ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();

                    throw new CmfBaseException(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageNoHoldReasonAvailableErrorMessage));
                }

                // Load Step Reasons
                material.Step.LoadStepReasons();

                if (material.Step.HoldReasons == null ||
                        (material.Step.HoldReasons != null &&
                            material.Step.HoldReasons.Where(hr => hr.TargetEntity.Name.Equals(abortProcessHoldReason.Value.ToString(), StringComparison.InvariantCultureIgnoreCase)).Count() == 0))
                {
                    IStepReason newStepReason = entityFactory.Create<IStepReason>();
                    newStepReason.SourceEntity = material.Step;
                    newStepReason.TargetEntity = reason;

                    // Add the hold reason configured to the step
                    IStepReasonCollection stepReasons = entityFactory.CreateCollection<IStepReasonCollection>();
                    stepReasons.Add(newStepReason);

                    material.Step.AddStepReasons(stepReasons);
                }

                IMaterialHoldReasonCollection materialHoldReasons = entityFactory.CreateCollection<IMaterialHoldReasonCollection>();
                IMaterialHoldReason materialHoldReason = entityFactory.Create<IMaterialHoldReason>();
                materialHoldReason.SourceEntity = material;
                materialHoldReason.TargetEntity = reason;
                materialHoldReason.Comment = string.Format($"CustomSetHoldEntity set on hold for container or material!");

                materialHoldReasons.Add(materialHoldReason);

                //Put Lot on Hold
                material.Hold(materialHoldReasons);
            }
            else if (container != null)
            {
                if (container.SystemState != ContainerSystemState.Unavailable)
                {
                    container.HoldForMaintenance(1);
                }
            }
            else
            {
                throw new CmfBaseException("It was not possible to retrive a container or a material.");
            }

            //---End DEE Code---

            return Input;
		}
	}
}
