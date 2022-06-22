namespace Cmf.Custom.TibcoEMS.ServiceManager.Common
{
    public static class TibcoEMSConstants
    {
        #region Generic Table

        /// <summary>
        /// CustomTibcoEMSGatewayResolver
        /// </summary>
        public const string GTCustomTibcoEMSGatewayResolver = "CustomTibcoEMSGatewayResolver";

        #endregion

        #region Message Bus Subjects

        /// <summary>
        /// CustomTibcoEMSGatewayInvalidateCache
        /// </summary>
        public const string SubjectCustomTibcoEMSGatewayInvalidateCache = "CustomTibcoEMSGatewayInvalidateCache";

        #endregion

        #region Log Messages

        /// <summary>
        /// DefaultLogDataFormat: Message Received >> SUBJECT: {0}; TOPIC: {1}; ACTION: {2}
        /// </summary>
        public static string DefaultLogDataFormat = "Message Received >> SUBJECT: {0}; TOPIC: {1}; ACTION: {2}";

        #endregion

        #region Configs

        /// <summary>
        /// Tibco EMS Configurations Path
        /// </summary>
        public const string TibcoEMSPathConfigurations = "/AMSOsram/TibcoEMS/";

        #endregion
    }
}
