using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration;
using Cmf.Foundation.Security;
using Cmf.Navigo.BusinessObjects;
using System.Runtime.Serialization;

namespace Cmf.Custom.AMSOsram.Orchestration.OutputObjects
{
    /// <summary>
    /// Output Data Contract for the CustomReceiveERPMessage service
    /// </summary>
    [DataContract(Name = "CustomReceiveERPMessageOutput")]
    public class CustomReceiveERPMessageOutput : BaseOutput
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
        public IntegrationEntry Result
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
