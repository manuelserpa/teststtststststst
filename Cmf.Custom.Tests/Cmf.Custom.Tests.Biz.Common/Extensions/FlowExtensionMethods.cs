using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cmf.Custom.TestUtilities;
using Cmf.Navigo.BusinessObjects;

namespace Cmf.Custom.Tests.Biz.Common.Extensions
{
    /// <summary>
    /// Extension class that extends the Flow features
    /// </summary>
    public static class FlowExtensionMethods
    {
        /// <summary>
        /// Method that returns a list of the flow paths
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="flowPath"></param>
        /// <returns></returns>
        public static List<string> CustomGetFlowPaths(Flow instance, string flowPath = "")
        {
            string instacePath = "";
            List<string> strs = new List<string>();
            if (string.IsNullOrWhiteSpace(flowPath))
            {
                instacePath = string.Concat(instance.Name, ":", 1);
            }
            if (instance != null)
            {
                instance.SpecialLoad<Flow>(1);
                FlowChildType? childType = instance.ChildType;
                if (!(childType.GetValueOrDefault() == 0 & childType.HasValue))
                {
                    instance.LoadRelation<Flow>("SubFlow", 1);
                    SubFlowCollection subFlows = instance.SubFlows;
                    if (subFlows != null && subFlows.Count > 0)
                    {
                        foreach (SubFlow subFlow in subFlows)
                        {
                            flowPath = string.Concat(new object[] { instacePath, "/", subFlow.TargetEntity.Name, ":", subFlow.CorrelationID });
                            strs.AddRange(subFlow.TargetEntity.GetFlowPaths(flowPath));
                        }
                    }
                }
                else
                {
                    instance.LoadRelation<Flow>("FlowStep", 1);
                    FlowStepCollection flowSteps = instance.FlowSteps;
                    if (flowSteps != null && flowSteps.Count > 0)
                    {
                        foreach (FlowStep flowStep in flowSteps)
                        {
                            strs.Add(string.Concat(new object[] { instacePath, "/", flowStep.TargetEntity.Name, ":", flowStep.CorrelationID }));
                        }
                    }
                }
            }
            return strs;
        }

        /// <summary>
        /// Method that returns the flow path
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="stepName"></param>
        /// <param name="stepOccurrenceNumber"></param>
        /// <returns></returns>
        public static string CustomGetFlowPath(Flow instance, string stepName, int stepOccurrenceNumber = 1)
        {
            if (instance == null || string.IsNullOrEmpty(stepName))
            {
                return null;
            }
            if (string.IsNullOrEmpty(instance.Name))
            {
                instance.SpecialLoad<Flow>(1);
            }
            string str = string.Format("{0}|||{1}|||{2}", instance.Name, stepName, stepOccurrenceNumber);
            List<string> flowPaths = instance.GetFlowPaths("");
            flowPaths = CustomGetFlowPaths(instance, "");
            Step step = new Step() { Name = stepName };
            step.Load<Step>();
            List<string> list = (
                from fPath in flowPaths
                where fPath.Contains(string.Format("{0}:", stepName))
                select fPath).ToList<string>();
            if (list.Count == 0)
            {
                throw new Exception(string.Format("Flow \"{0}\" (Version: {1}) does not contain step \"{2}\"", instance.Name, instance.Version, stepName));
            }
            string str1 = null;
            str1 = (stepOccurrenceNumber <= list.Count ? list[stepOccurrenceNumber - 1] : list.Last<string>());
            return str1;
        }
    }
}
