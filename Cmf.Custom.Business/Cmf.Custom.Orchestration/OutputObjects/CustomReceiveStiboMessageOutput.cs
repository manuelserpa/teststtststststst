using Cmf.Foundation.BusinessOrchestration;
using System.Runtime.Serialization;
using Cmf.Foundation.BusinessObjects.Abstractions;

namespace Cmf.Custom.amsOSRAM.Orchestration.OutputObjects
{
    /// <summary>
    /// Output Data Contract for the CustomReceiveStiboMessage service
    /// </summary>
    [DataContract(Name = "CustomReceiveStiboMessageOutput")]
    public class CustomReceiveStiboMessageOutput : BaseOutput
    {
        #region Private Variables
        #endregion

        #region Public Variables
        #endregion

        #region Properties

        /// <summary>
        /// Result
        /// </summary>
        [DataMember(Name = "Result", Order = 100)]
        public IIntegrationEntry Result
        {
            get;
            set;
        }

        #endregion

        #region Constructors
        #endregion

        #region Private & Internal Methods
        #endregion

        #region Public Methods
        #endregion

        #region Event handling Methods
        #endregion
    }
}
