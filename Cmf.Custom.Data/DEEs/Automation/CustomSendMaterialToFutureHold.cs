using Cmf.Custom.AMSOsram.Actions;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Cmf.Custom.AMSOsram.Actions.Automation
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
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("", "Cmf.Foundation.Common.Exceptions");
            UseReference("", "Cmf.Foundation.Common");
            UseReference("", "Cmf.Foundation.BusinessObjects.Cultures");
            //Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement");
            //Other
            UseReference("", "System.Threading");
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");
			UseReference("%MicrosoftNetPath%System.ObjectModel.dll", "");
            //Please start code here

            #region Validate Input Parameters

            if (!Input.ContainsKey("MaterialName"))
            {
                throw new ArgumentNullCmfException("MaterialName");
            }
            var materialName = Input["MaterialName"].ToString();

            if (!Input.ContainsKey("ResourceName"))
            {
                throw new ArgumentNullCmfException("ResourceName");
            }
            var resourceName = Input["ResourceName"].ToString();

            if (!Input.ContainsKey("ReasonName"))
            {
                throw new ArgumentNullCmfException("ReasonName");
            }
            var reasonName = Input["ReasonName"].ToString();
			
            string composedComment = "No Comment";

            if (!Input.ContainsKey("LocalizedMessageParameters"))
            {
                Input.Add("LocalizedMessageParameters", new object[] { resourceName, "No Wafer Information" });
            }

            if (Input.ContainsKey("LocalizedMessageName"))
            {
                var localizedMessageParameters = ((JArray)Input["LocalizedMessageParameters"]).Select(x => x.ToString());
                composedComment = string.Format(LocalizedMessage.GetLocalizedMessage(Thread.CurrentThread.CurrentCulture.Name,
                    Input["LocalizedMessageName"].ToString()).MessageText, localizedMessageParameters.ToArray());
            }


            #endregion


            bool hasHoldForInvalidWaferForSlot = false;
            Reason holdForInvalidWaferForSlot = new Reason();

            Material material = new Material();
            material.Load(materialName);
            material.Step.LoadStepReasons();

            if (material.Step.HoldReasons != null && material.Step.HoldReasons.Count > 0)
            {
                hasHoldForInvalidWaferForSlot = material.Step.HoldReasons.Any(hr => hr.TargetEntity.Name == reasonName);
            }


            if (!hasHoldForInvalidWaferForSlot)
            {
                holdForInvalidWaferForSlot = new Reason()
                {
                    Name = reasonName
                };


                //Validate if HoldIvalidWaferForSlot exists
                if (!holdForInvalidWaferForSlot.ObjectExists())
                {
                    throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Reason, reasonName);
                }

                // Load HoldIvalidWaferForSlot reason
                holdForInvalidWaferForSlot.Load();

                StepReason holdAfterScrapStepReason = new StepReason()
                {
                    SourceEntity = material.Step,
                    TargetEntity = holdForInvalidWaferForSlot
                };

                // Add Reason to the Step
                material.Step.AddStepReasons(new StepReasonCollection() { holdAfterScrapStepReason });
            }
            else
            {
                // Select HoldIvalidWaferForSlot reason available
                holdForInvalidWaferForSlot = new Reason()
                {
                    Name = reasonName
                };

                // Load HoldIvalidWaferForSlot reason
                holdForInvalidWaferForSlot.Load();
            }


            FutureActionCollection materialFutureActions = material.GetMaterialFutureActionsByType(FutureActionType.Hold);
            bool hasReasonFutureAction = false;
            FutureAction materialFutureAction = null;



            if (materialFutureActions != null && materialFutureActions.Count > 0)
            {
                materialFutureAction = materialFutureActions.FirstOrDefault(fa => fa.Reason.Name.Equals(holdForInvalidWaferForSlot.Name) && fa.Step.Name.Equals(material.Step.Name)
                && fa.State == FutureActionState.Processed);


                hasReasonFutureAction = materialFutureAction != null;
            }

            ManageFutureActionsInput manageFutureActionsInput = new ManageFutureActionsInput();
            FutureActionCollection futureActionCollection = new FutureActionCollection();


            if (!hasReasonFutureAction)
            {

                FutureAction futureAction = new FutureAction()
                {
                    Action = FutureActionType.Hold,
                    State = FutureActionState.Processed,
                    Comment = composedComment,
                    Reason = holdForInvalidWaferForSlot,
                    Step = material.Step,
                    Material = material,
                    Source = FutureActionSource.User
                };

                futureActionCollection.Add(futureAction);

                if (futureActionCollection.Count > 0)
                {
                    manageFutureActionsInput.AddedUpdatedFutureActions = futureActionCollection;
                    MaterialManagementOrchestration.ManageFutureActions(manageFutureActionsInput);
                }
            }
            else
            {
                materialFutureAction.Comment += "; " + composedComment;

                futureActionCollection.Add(materialFutureAction);

                if (futureActionCollection.Count > 0)
                {
                    manageFutureActionsInput.AddedUpdatedFutureActions = futureActionCollection;
                    MaterialManagementOrchestration.ManageFutureActions(manageFutureActionsInput);
                }

            }


            //---End DEE Code---

            return Input;
        }
	}
}
