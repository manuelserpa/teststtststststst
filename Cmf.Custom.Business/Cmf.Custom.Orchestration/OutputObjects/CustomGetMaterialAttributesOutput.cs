using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cmf.Custom.amsOSRAM.Orchestration.OutputObjects
{
    /// <summary>
    /// Output Object for MaterialAttribute Service
    /// </summary>
    [DataContract(Name = "CustomGetMaterialAttributesOutput")]
    public class CustomGetMaterialAttributesOutput : BaseOutput
    {
        #region Properties
        #region Private Variables
        #endregion

        #region Public Variables
        #endregion

        #region Properties

        /// <summary>
		/// Result
		/// </summary>
		[DataMember(Name = "Result", Order = 100)]
        public string Result
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
        #endregion

    }
}
