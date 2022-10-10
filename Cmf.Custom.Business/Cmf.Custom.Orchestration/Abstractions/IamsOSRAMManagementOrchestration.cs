using Cmf.Custom.amsOSRAM.Orchestration.InputObjects;
using Cmf.Custom.amsOSRAM.Orchestration.OutputObjects;

namespace Cmf.Custom.amsOSRAM.Orchestration.Abstractions
{

    public interface IamsOSRAMManagementOrchestration
    {
        /// <summary>
        /// Performs the Material Track In in the Resource
        /// </summary>
        /// <param name="input"></param>
        /// <returns>MaterialIn Output object</returns>
        MaterialInOutput MaterialIn(MaterialInInput input);

        /// <summary>
        /// Performs the Material Track out from the Resource
        /// </summary>
        /// <param name="input"></param>
        /// <returns>MaterialOut Output object</returns>
        MaterialOutOutput MaterialOut(MaterialOutInput input);

        /// <summary>
        /// Service to generate an Integration Entry based on the received Stibo message
        /// </summary>
        /// <param name="input">Input Object</param>
        /// <returns>Output Object</returns>
        /// <exception cref="Cmf.Foundation.Common.CmfBaseException">If any unexpected error occurs.</exception>
        CustomReceiveStiboMessageOutput CustomReceiveStiboMessage(CustomReceiveStiboMessageInput input);

        /// <summary>
        /// Service to generate an Integration Entry based on the received message
        /// </summary>
        /// <param name="input">CustomReceiveERPMessage Input</param>
        /// <returns>CustomReceiveERPMessage Output</returns>
        /// <exception cref="Cmf.Foundation.Common.CmfBaseException">If any unexpected error occurs.</exception>
        CustomReceiveERPMessageOutput CustomReceiveERPMessage(CustomReceiveERPMessageInput input);

        /// <summary>
        /// Service to provide flow information to ERP
        /// </summary>
        /// <param name="input">GetFlowInformationForERP Input</param>
        /// <returns>GetFlowInformationForERP Output</returns>
        /// <exception cref="Cmf.Foundation.Common.CmfBaseException">If any unexpected error occurs.</exception>
        CustomGetFlowInformationForERPOutput GetFlowInformationForERP(CustomGetFlowInformationForERPInput input);
    }
}