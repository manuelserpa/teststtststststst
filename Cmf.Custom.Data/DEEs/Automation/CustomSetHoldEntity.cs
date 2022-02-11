using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Cmf.Custom.AMSOsram.Actions.Automation
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
			UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.Cultures");
			UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
			UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
			UseReference("", "Cmf.Foundation.Common.Exceptions");
			UseReference("", "Cmf.Foundation.Common");
			// Navigo
			UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
			// Custom
			UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
			// System
			UseReference("%MicrosoftNetPath%System.Private.CoreLib.dll", "System.Threading");
			//Please start code here

			Container container = null;
			Material material = null;
			bool setMaterialOnHold = false;

			#region Input Validation

			if (Input.ContainsKey("MaterialName") &&
				Input["MaterialName"] != null &&
				Input["MaterialName"] is string strMaterialName)
			{
				material = new Material()
				{
					Name = strMaterialName
				};

				material.Load();
			}

			if (Input.ContainsKey("ContainerName") &&
				Input["ContainerName"] != null &&
				Input["ContainerName"] is string strContainerName)
			{
				container = new Container()
				{
					Name = strContainerName
				};

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
				MaterialContainer firstMaterialContainer = container.ContainerMaterials.FirstOrDefault();

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
				if (!Config.TryGetConfig(AMSOsramConstants.DefaultAbortProcessHoldReasonConfig, out Config abortProcessHoldReason)
					|| string.IsNullOrWhiteSpace(abortProcessHoldReason.GetConfigValue<string>()))
				{
					throw new CmfBaseException(LocalizedMessage.GetLocalizedMessage(Thread.CurrentThread.CurrentCulture.Name, AMSOsramConstants.LocalizedMessageNoHoldReasonAvailableErrorMessage).MessageText);
				}

				Reason reason = new Reason()
				{
					Name = abortProcessHoldReason.Value.ToString()
				};

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
						MaterialHoldReason mhReason = material.MaterialHoldReasons.FirstOrDefault(mh => mh.TargetEntity.Id == reason.Id);

						if (mhReason != null && !mhReason.TargetEntity.EnableConcurrentInstances)
						{
							throw new CmfBaseException("Material already has the hold reason set.");
						}
					}
				}

				// Reason must be a hold reason
				if (reason.ReasonType != ReasonType.Hold)
				{
					throw new CmfBaseException(LocalizedMessage.GetLocalizedMessage(Thread.CurrentThread.CurrentCulture.Name, AMSOsramConstants.LocalizedMessageNoHoldReasonAvailableErrorMessage).MessageText);
				}

				// Load Step Reasons
				material.Step.LoadStepReasons();

				if (material.Step.HoldReasons == null ||
						(material.Step.HoldReasons != null &&
							material.Step.HoldReasons.Where(hr => hr.TargetEntity.Name.Equals(abortProcessHoldReason.Value.ToString(), StringComparison.InvariantCultureIgnoreCase)).Count() == 0))
				{
					StepReason newStepReason = new StepReason()
					{
						SourceEntity = material.Step,
						TargetEntity = reason
					};

					// Add the hold reason configured to the step
					material.Step.AddStepReasons(new StepReasonCollection() { newStepReason });
				}

				MaterialHoldReasonCollection materialHoldReason = new MaterialHoldReasonCollection()
				{
					new MaterialHoldReason()
					{
						SourceEntity = material,
						TargetEntity = reason,
						Comment = string.Format($"CustomSetHoldEntity set on hold for container or material!")
					}
				};

				//Put Lot on Hold
				material.Hold(materialHoldReason);
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