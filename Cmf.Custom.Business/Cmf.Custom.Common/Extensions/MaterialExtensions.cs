using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cmf.Custom.AMSOsram.Common.Extensions
{
	public static class MaterialExtensions
	{
        /// <summary>
        /// Get Step Parent Flow
        /// 
        /// </summary>
        /// <param name="material"></param>
        /// <returns>The immediate step parent flow</returns>
		public static Flow GetStepParentFlow(this Material material) 
		{
            string[] flowParts = material.FlowPath.Split('/');
            if (flowParts.Length == 2)
                return material.Flow;

            #region Get FlowName
            string flowName = material.FlowPath.Substring(0, material.FlowPath.LastIndexOf("/"));
            flowName = flowName.Substring(0, flowName.LastIndexOf(":"));
            flowName = flowName.Substring(flowName.LastIndexOf("/") + 1);
            #endregion

            Flow flowToReturn = new Flow();
            flowToReturn.Load(flowName);
            return flowToReturn;
        }
	}
}
