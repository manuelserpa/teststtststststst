using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Foundation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.Common.LocalizationService;
using Cmf.Navigo.BusinessObjects;
using Cmf.Foundation.BusinessObjects;

namespace Cmf.Custom.amsOSRAM.Actions.Automation
{
	internal class CustomAutomationAdjustLoadPortState : DeeDevBase
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
             *     Dee action is triggered by IoT Automation to adjust the state of a Load Port based on:
             *     - Load Port Order (Display Order of the SubResource) and Parent Resource
             *     - Or Load Port Resource Name
             *
             * Action Groups:
             *      None
             *
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

            // Foundation
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects");
            UseReference("Cmf.Foundation.Common.dll", "Cmf.Foundation.Common.LocalizationService");

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.BusinessObjects.CustomSorterJobDefinition.dll", "Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.Extensions");

            // System
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json");
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Schema");
            UseReference("%MicrosoftNetPath%System.Private.CoreLib.dll", "System.Threading");

            #region Input validation

            string inputKey = "ResourceName";

			string resourceName = amsOSRAMUtilities.GetInputItem<string>(Input, inputKey);

			if (string.IsNullOrWhiteSpace(resourceName))
			{
                throw new ArgumentNullCmfException(inputKey);
            }

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            IResource resource = entityFactory.Create<IResource>();
            resource.Name = resourceName;

            inputKey = "StateName";
			string state = amsOSRAMUtilities.GetInputItem<string>(Input, inputKey);

			if (string.IsNullOrWhiteSpace(state))
			{
				throw new ArgumentNullCmfException("StateName");
			}

			inputKey = "LoadPortNumber";
			int? loadPortNumber = null;

			if (Input.ContainsKey(inputKey) &&
				Input[inputKey] != null)
			{
				loadPortNumber = int.Parse(Input["LoadPortNumber"].ToString());
			}

			inputKey = "LoadPortName";
			string loadPortName = amsOSRAMUtilities.GetInputItem<string>(Input, inputKey);

			if (!loadPortNumber.HasValue &&
				string.IsNullOrWhiteSpace(loadPortName))
			{
				throw new ArgumentNullCmfException("LoadPortNumber or LoadPortName");
			}

			inputKey = "CarrierID";
			string containerName = amsOSRAMUtilities.GetInputItem(Input, inputKey, string.Empty);
			IContainer container = entityFactory.Create<IContainer>();

			if (!string.IsNullOrWhiteSpace(containerName))
			{
				container.Name = containerName;

				if (container.ObjectExists())
				{
					container.Load(containerName);
				}
			}

			#endregion Input validation

			IResource loadPort = entityFactory.Create<IResource>(); ;

			if (loadPortNumber != null)
			{
				resource.Load();
				IResourceHierarchy resourceHierarchy = resource.GetDescendentResources(1);

				if (resourceHierarchy == null ||
					resourceHierarchy.Count <= 0)
				{
					throw new MissingMandatoryPropertyCmfException("SubResource", resource.Name); ;
				}

				loadPort = resourceHierarchy.FirstOrDefault(s => s.ChildResource.DisplayOrder != null &&
								s.ChildResource.DisplayOrder.Value == loadPortNumber &&
								s.ChildResource.ProcessingType == ProcessingType.LoadPort).ChildResource;
			}
			else if (!String.IsNullOrEmpty(loadPortName))
			{
				loadPort.Name = loadPortName;
			}
			else
			{
				throw new InvalidNullInstanceValuesForMandatoryPropertyException("LoadPortName or LoadPortNumber");
			}

			if (loadPort == null)
			{
				throw new ObjectNotFoundCmfException("LoadPort Resource",
					(loadPortNumber != null ? loadPortNumber.Value.ToString() : loadPortName));
			}

			loadPort.Load();

			#region Load Port State Model and State Model State Validations and Events

			// Enforce 'CustomLoadPortStateModelState' State Model if it is not the correct one
			if (loadPort.CurrentMainState != null &&
				loadPort.CurrentMainState.StateModel != null &&
				!loadPort.CurrentMainState.StateModel.Name.Equals(amsOSRAMConstants.CustomLoadPortStateModel))
			{

				IStateModel loadPortStateModel = entityFactory.Create<IStateModel>();
                loadPortStateModel.Name = amsOSRAMConstants.CustomLoadPortStateModel;
				
				if (loadPortStateModel.ObjectExists())
				{
					loadPortStateModel.Load();
					loadPort.SetMainStateModel(loadPortStateModel);
					loadPort.Load();
				}
				else
				{
					ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();
                    throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageStateModelDoesNotExistException), amsOSRAMConstants.CustomLoadPortStateModel));
				}
			}

			IStateModelState toStateModelState = loadPort.CurrentMainState.StateModel.States.FirstOrDefault(s => s.Name.Equals(state, StringComparison.InvariantCultureIgnoreCase));

			if (toStateModelState != null)
			{
				IStateModelTransition stateModelTransition = loadPort.GetStateModelTransitionForState(toStateModelState.Name);

				if (stateModelTransition != null)
				{
					loadPort.LogEvent(stateModelTransition.Name, loadPort.CurrentMainState.StateModel);
				}
				else
				{
					// stateModelTransition does not exist we need to adjust this manually
					loadPort.AdjustState(state, loadPort.CurrentMainState.StateModel);
				}
			}
			else
			{
                ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();
                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageStateModelStateDoesNotExistException), amsOSRAMConstants.CustomLoadPortStateModelStateReservedStateModelState));
			}

			#endregion Load Port State Model and State Model State Validations and Events

			if (state.Equals(amsOSRAMConstants.CustomLoadPortStateModelStateReadyToLoadStateModelState, StringComparison.InvariantCultureIgnoreCase) &&
				loadPort.AutomationMode == ResourceAutomationMode.Online)
			{
				loadPort.SaveAttributes(new AttributeCollection() { { amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, false } });

				if (!string.IsNullOrEmpty(containerName))
				{
					container.Undock();
				}

				//TODO: Call DEE ACtion to get Transport Request when Transport System is available
			}
			else if (state.Equals(amsOSRAMConstants.CustomLoadPortStateModelStateReadyToUnloadStateModelState, StringComparison.InvariantCultureIgnoreCase) &&
				!string.IsNullOrEmpty(containerName) &&
				loadPort.AutomationMode == ResourceAutomationMode.Online &&
				loadPort.UsedPositions > 0)
			{
				container.SaveAttributes(new AttributeCollection()
					{
						{ amsOSRAMConstants.ContainerAttributeMapContainerNeeded, false },
						{ amsOSRAMConstants.ContainerAttributeProduct, string.Empty }
					}
				);

				//TODO: Call DEE ACtion to get Transport Request when Transport System is available
			}

			Input.Add("LoadPort", loadPort);
			Input.Add("LoadPortType", loadPort.LoadPortType != null ? loadPort.LoadPortType.ToString() : "none");

			//---End DEE Code---

			return Input;
		}
	}
}
