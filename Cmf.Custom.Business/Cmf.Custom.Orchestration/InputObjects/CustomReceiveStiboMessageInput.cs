using Cmf.Foundation.BusinessOrchestration;
using System.Runtime.Serialization;

namespace Cmf.Custom.amsOSRAM.Orchestration.InputObjects
{
    /// <summary>
    /// Input Data Contract for the CustomReceiveStiboMessage service
    /// </summary>
    [DataContract(Name = "CustomReceiveStiboMessageInput")]
    public class CustomReceiveStiboMessageInput : BaseInput
    {
        #region Private Variables
        #endregion

        #region Public Variables
        #endregion

        #region Properties

        /// <summary>
        /// Message
        /// </summary>
        [DataMember(Name = "Message", Order = 100)]
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// MessageType
        /// </summary>
        [DataMember(Name = "MessageType", Order = 100)]
        public string MessageType
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
