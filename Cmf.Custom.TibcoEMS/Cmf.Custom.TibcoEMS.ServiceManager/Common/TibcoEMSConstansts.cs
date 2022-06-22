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
        /// LogErrorOnMessageReceived
        /// </summary>
        public static string LogErrorOnMessageReceived = "[SUBJECT]: {0} [TOPIC]: {1} [ACTION]: {2}";

        /// <summary>
        /// LogErrorOnMessageReceived
        /// </summary>
        public static string LogErrorOnInvalidateCache = "[SUBJECT]: {0}";

        #endregion
    }
}
