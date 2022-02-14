using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.FacilityManagement.FlowManagement.InputObjects;
using AMSOsramEIAutomaticTests.Objects.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMSOsramEIAutomaticTests.Objects.Extensions
{
    /// <summary>
    /// Extension Methods Class that adapts step services as Business Objects extension methods or adds functionality need for testing
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Adds a given service to the step
        /// </summary>
        /// <param name="instance">step instance to which service will be added</param>
        /// <param name="serviceToAdd"></param>
        public static void AddService(this Step instance, Service serviceToAdd)
        {
            if (instance != null)
            {
                // get contexts smart table structure
                instance.CopyFrom(new LoadStepServiceContextsInput()
                {
                    Step = instance
                    ,
                    Filters = new FilterCollection()
                    {
                        new Filter()
                        {
                            Name = "Step"
                            , Operator = FieldOperator.IsEqualTo
                            , Value = String.Empty
                            , LogicalOperator = LogicalOperator.Nothing
                        }
                    }
                }.LoadStepServiceContextsSync().Step);

                // transform it to dataset
                DataSet baseSet = Cmf.TestScenarios.Others.Utilities.ToDataSet(instance.StepServiceContexts);

                // clone the data set
                DataSet baseClone = baseSet.Clone();

                // get new row
                DataRow dr = baseClone.Tables[0].NewRow();
                dr["Step"] = instance.Name;
                dr["Service"] = serviceToAdd.Name;
                dr["LastServiceHistoryId"] = -1;
                dr["LastOperationHistorySeq"] = -1;
                dr["ServiceContextId"] = -1;
                baseClone.Tables[0].Rows.Add(dr);

                // add new service
                instance.CopyFrom(new AddStepServiceContextsInput()
                {
                    Step = instance
                    ,
                    ServiceContextsToAdd = Cmf.TestScenarios.Others.Utilities.FromDataSet(baseClone)
                }.AddStepServiceContextsSync().Step);

            }
        }

    

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="reasonType"></param>
        /// <returns></returns>
        public static Reason GetRandomReason(this Step instance, ReasonType reasonType = ReasonType.Loss)
        {
            Reason reasonToReturn = null;

            ReasonCollection reasons = instance.GetRandomReasons(1, reasonType);
            if (reasons != null && reasons.Count > 0)
            {
                reasonToReturn = reasons[0];
            }

            return reasonToReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="numberOfReasons"></param>
        /// <param name="reasonType"></param>
        /// <param name="isEligibleForTerminate"></param>
        /// <returns></returns>
        public static ReasonCollection GetRandomReasons(this Step instance, Int32 numberOfReasons, ReasonType reasonType = ReasonType.Loss, Boolean? isEligibleForTerminate = null)
        {
            if (instance == null || numberOfReasons <= 0)
                return null;

            ReasonCollection reasonToReturn = null;

            // ensure object is correctly loaded...
            instance.SpecialLoad();

            // check if reasons are set. if not, or if no reason of the required type exists, perform a sanity check load
            if (instance.StepReasons == null || !instance.StepReasons.Any(E => E.TargetEntity.ReasonType == reasonType))
                instance.LoadReasons(reasonType);

            // check if we have enough reasons to return
            if (instance.StepReasons != null)
            {
                IEnumerable<StepReason> eligibleReasons = instance.StepReasons.Where(E => E.TargetEntity.ReasonType == reasonType && (isEligibleForTerminate == null || E.ApplicableToTerminate == isEligibleForTerminate.GetValueOrDefault()));
                if (eligibleReasons.Count() >= numberOfReasons)
                {
                    // select random elements from the list of available units
                    List<Int32> selectedPositions = new List<Int32>();
                    List<Int32> availablePositions = new List<Int32>();
                    availablePositions.AddRange(eligibleReasons.Select((E, Index) => Index));

                    Random random = new Random();
                    if (availablePositions.Count == numberOfReasons)
                    {
                        selectedPositions = availablePositions;
                    }
                    else
                    {
                        Int32 numberOfRemainingItems = numberOfReasons;
                        while (numberOfRemainingItems > 0)
                        {
                            Int32 nextPosition = random.Next(0, availablePositions.Count - 1);
                            selectedPositions.Add(availablePositions[nextPosition]);
                            availablePositions.RemoveAt(nextPosition);

                            numberOfRemainingItems--;
                        }
                    }

                    reasonToReturn = new ReasonCollection();
                    foreach (Int32 iPosition in selectedPositions)
                    {
                        Reason reasonSelected = eligibleReasons.ElementAt(iPosition).TargetEntity.SpecialLoad();
                        reasonToReturn.Add(reasonSelected);
                    }
                }
            }

            return reasonToReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="reasonTypes"></param>
        public static void LoadReasons(this Step instance, ReasonType? reasonTypes = null)
        {
            instance.CopyFrom(new LoadStepReasonsInput() { Step = instance, ReasonType = reasonTypes, LevelsToLoad = 1 }.LoadStepReasonsSync().Step);
        }

    }
}
