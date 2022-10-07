using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cmf.Navigo.BusinessObjects.Abstractions;
using System;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Foundation.Common.LocalizationService;
using Cmf.Navigo.BusinessOrchestration.Abstractions;
using Cmf.Navigo.BusinessObjects;

namespace Cmf.Custom.amsOSRAM.Actions.Automation
{
    class CustomSendMaterialToFutureHold : DeeDevBase
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
             *     Dee action is triggered by IoT Automation to put a wafer based on:
             *     - MapCarrier (wafer transfer in the same container)
             *     - TransferWafers (wafer transfer beetween different containers)            
             *  
             * Action Groups:
             *      None
             *     
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

            // Foundation
            UseReference("", "Cmf.Foundation.Common.LocalizationService");

            // Navigo
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects");
            UseReference("", "Cmf.Navigo.BusinessOrchestration.Abstractions");

            // System
            UseReference("", "System.Threading");
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");
            UseReference("%MicrosoftNetPath%System.ObjectModel.dll", "");

            #region Validate Input Parameters

            if (!Input.ContainsKey("MaterialName"))
            {
                throw new ArgumentNullCmfException("MaterialName");
            }
            string materialName = Input["MaterialName"].ToString();

            if (!Input.ContainsKey("ResourceName"))
            {
                throw new ArgumentNullCmfException("ResourceName");
            }
            string resourceName = Input["ResourceName"].ToString();

            if (!Input.ContainsKey("ReasonName"))
            {
                throw new ArgumentNullCmfException("ReasonName");
            }

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            string reasonName = Input["ReasonName"].ToString();
			
            string composedComment = "No Comment";

            if (!Input.ContainsKey("LocalizedMessageParameters"))
            {
                Input.Add("LocalizedMessageParameters", new object[] { resourceName, "No Wafer Information" });
            }

            if (Input.ContainsKey("LocalizedMessageName"))
            {
                IEnumerable<string> localizedMessageParameters = ((JArray)Input["LocalizedMessageParameters"]).Select(x => x.ToString());
                ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();

                composedComment = string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name,
                    Input["LocalizedMessageName"].ToString()), localizedMessageParameters.ToArray());
            }

            #endregion

            bool hasHoldForInvalidWaferForSlot = false;
            IReason holdForInvalidWaferForSlot = entityFactory.Create<IReason>();

            IMaterial material = entityFactory.Create<IMaterial>();
            material.Load(materialName);
            material.Step.LoadStepReasons();

            if (material.Step.HoldReasons != null && material.Step.HoldReasons.Count > 0)
            {
                hasHoldForInvalidWaferForSlot = material.Step.HoldReasons.Any(hr => hr.TargetEntity.Name == reasonName);
            }

            if (!hasHoldForInvalidWaferForSlot)
            {
                holdForInvalidWaferForSlot = entityFactory.Create<IReason>();
                holdForInvalidWaferForSlot.Name = reasonName;

                //Validate if HoldIvalidWaferForSlot exists
                if (!holdForInvalidWaferForSlot.ObjectExists())
                {
                    throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Reason, reasonName);
                }

                // Load HoldIvalidWaferForSlot reason
                holdForInvalidWaferForSlot.Load();

                IStepReason holdAfterScrapStepReason = entityFactory.Create<IStepReason>();
                holdAfterScrapStepReason.SourceEntity = material.Step;
                holdAfterScrapStepReason.TargetEntity = holdForInvalidWaferForSlot;

                // Add Reason to the Step
                IStepReasonCollection stepReasonCollection = entityFactory.CreateCollection<IStepReasonCollection>();
                stepReasonCollection.Add(holdAfterScrapStepReason);

                material.Step.AddStepReasons(stepReasonCollection);
            }
            else
            {
                // Select HoldIvalidWaferForSlot reason available
                holdForInvalidWaferForSlot = serviceProvider.GetService<IReason>();
                holdForInvalidWaferForSlot.Name = reasonName;

                // Load HoldIvalidWaferForSlot reason
                holdForInvalidWaferForSlot.Load();
            }

            IFutureActionCollection materialFutureActions = material.GetMaterialFutureActionsByType(FutureActionType.Hold);
            bool hasReasonFutureAction = false;
            IFutureAction materialFutureAction = null;

            if (materialFutureActions != null && materialFutureActions.Count > 0)
            {
                materialFutureAction = materialFutureActions.FirstOrDefault(fa => fa.Reason.Name.Equals(holdForInvalidWaferForSlot.Name) && fa.Step.Name.Equals(material.Step.Name)
                && fa.State == FutureActionState.Processed);

                hasReasonFutureAction = materialFutureAction != null;
            }

            ManageFutureActionsInput manageFutureActionsInput = new ManageFutureActionsInput();
            IFutureActionCollection futureActionCollection = entityFactory.CreateCollection<IFutureActionCollection>();
            IMaterialOrchestration materialOrchestration = serviceProvider.GetService<IMaterialOrchestration>();

            if (!hasReasonFutureAction)
            {
                IFutureAction futureAction = entityFactory.Create<IFutureAction>();
                futureAction.Action = FutureActionType.Hold;
                futureAction.State = FutureActionState.Processed;
                futureAction.Comment = composedComment;
                futureAction.Reason = holdForInvalidWaferForSlot;
                futureAction.Step = material.Step;
                futureAction.Material = material;
                futureAction.Source = FutureActionSource.User;

                futureActionCollection.Add(futureAction);

                if (futureActionCollection.Count > 0)
                {
                    manageFutureActionsInput.AddedUpdatedFutureActions = futureActionCollection;

                    materialOrchestration.ManageFutureActions(manageFutureActionsInput);
                }
            }
            else
            {
                materialFutureAction.Comment += "; " + composedComment;

                futureActionCollection.Add(materialFutureAction);

                if (futureActionCollection.Count > 0)
                {
                    manageFutureActionsInput.AddedUpdatedFutureActions = futureActionCollection;
                    materialOrchestration.ManageFutureActions(manageFutureActionsInput);
                }
            }

            //---End DEE Code---

            return Input;
        }
	}
}
