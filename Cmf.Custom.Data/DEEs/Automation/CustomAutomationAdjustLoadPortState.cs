using Cmf.Custom.AMSOsram.Actions;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.Extensions;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Cmf.Custom.AMSOsram.Actions.Automation
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

			UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
			UseReference("", "Cmf.Foundation.BusinessObjects.SmartTables");
			UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
			UseReference("", "Cmf.Foundation.Common.Exceptions");
			UseReference("", "Cmf.Foundation.Common");
			UseReference("Cmf.Foundation.Configuration.dll", "Cmf.Foundation.Configuration");
			UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.Cultures");
			// Navigo
			UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
			// Custom
			UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
			UseReference("Cmf.Custom.AMSOsram.Common.dll", " Cmf.Custom.AMSOsram.Common.Extensions");
			// System
			UseReference("", "System.Data");
			UseReference("%MicrosoftNetPath%System.Private.CoreLib.dll", "System.Threading");

			#region Input validation

			string inputKey = "ResourceName";
			Resource resource = null;
			string resourceName = AMSOsramUtilities.GetInputItem<string>(Input, inputKey);

			if (!string.IsNullOrWhiteSpace(resourceName))
			{
				resource = new Resource() { Name = resourceName };
			}
			else
			{
				throw new ArgumentNullCmfException("ResourceName");
			}

			inputKey = "StateName";
			string state = AMSOsramUtilities.GetInputItem<string>(Input, inputKey);

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
			string loadPortName = AMSOsramUtilities.GetInputItem<string>(Input, inputKey);

			if (!loadPortNumber.HasValue &&
				string.IsNullOrWhiteSpace(loadPortName))
			{
				throw new ArgumentNullCmfException("LoadPortNumber or LoadPortName");
			}

			inputKey = "CarrierID";
			string containerName = AMSOsramUtilities.GetInputItem<string>(Input, inputKey, string.Empty);
			Container container = null;

			if (!string.IsNullOrWhiteSpace(containerName))
			{
				container = new Container
				{
					Name = containerName
				};

				if (container.ObjectExists())
				{
					container.Load(containerName);
				}
			}

			#endregion Input validation

			Resource loadPort = null;

			if (loadPortNumber != null)
			{
				resource.Load();
				ResourceHierarchy resourceHierarchy = resource.GetDescendentResources(1);

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
				loadPort = new Resource()
				{
					Name = loadPortName
				};
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
				!loadPort.CurrentMainState.StateModel.Name.Equals(AMSOsramConstants.CustomLoadPortStateModel))
			{
				StateModel loadPortStateModel = new StateModel() { Name = AMSOsramConstants.CustomLoadPortStateModel };

				if (loadPortStateModel.ObjectExists())
				{
					loadPortStateModel.Load();
					loadPort.SetMainStateModel(loadPortStateModel);
					loadPort.Load();
				}
				else
				{
					throw new CmfBaseException(string.Format(LocalizedMessage.GetLocalizedMessage(Thread.CurrentThread.CurrentCulture.Name, AMSOsramConstants.LocalizedMessageStateModelDoesNotExistException).MessageText, AMSOsramConstants.CustomLoadPortStateModel));
				}
			}

			StateModelState toStateModelState = loadPort.CurrentMainState.StateModel.States.FirstOrDefault(s => s.Name.Equals(state, StringComparison.InvariantCultureIgnoreCase));

			if (toStateModelState != null)
			{
				StateModelTransition stateModelTransition = loadPort.GetStateModelTransitionForState(toStateModelState.Name);

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
				throw new CmfBaseException(string.Format(LocalizedMessage.GetLocalizedMessage(Thread.CurrentThread.CurrentCulture.Name, AMSOsramConstants.LocalizedMessageStateModelStateDoesNotExistException).MessageText, AMSOsramConstants.CustomLoadPortStateModelStateReservedStateModelState));
			}

			#endregion Load Port State Model and State Model State Validations and Events

			if (state.Equals(AMSOsramConstants.CustomLoadPortStateModelStateReadyToLoadStateModelState, StringComparison.InvariantCultureIgnoreCase) &&
				loadPort.AutomationMode == ResourceAutomationMode.Online)
			{
				loadPort.SaveAttributes(new AttributeCollection() { { AMSOsramConstants.ResourceAttributeIsLoadPortInUse, false } });

				if (!string.IsNullOrEmpty(containerName))
				{
					container.Undock();
				}

				//TODO: Call DEE ACtion to get Transport Request when Transport System is available
			}
			else if (state.Equals(AMSOsramConstants.CustomLoadPortStateModelStateReadyToUnloadStateModelState, StringComparison.InvariantCultureIgnoreCase) &&
				!string.IsNullOrEmpty(containerName) &&
				loadPort.AutomationMode == ResourceAutomationMode.Online &&
				loadPort.UsedPositions > 0)
			{
				container.SaveAttributes(new AttributeCollection()
					{
						{ AMSOsramConstants.ContainerAttributeMapContainerNeeded, false },
						{ AMSOsramConstants.ContainerAttributeProduct, string.Empty }
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