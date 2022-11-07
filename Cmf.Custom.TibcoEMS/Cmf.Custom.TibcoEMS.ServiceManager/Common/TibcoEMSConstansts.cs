namespace Cmf.Custom.TibcoEMS.ServiceManager.Common
{
    public static class TibcoEMSConstants
    {
        #region Generic Table

        /// <summary>
        /// Custom TibcoEMS Gateway Resolver table name
        /// </summary>
        public const string GenericTableCustomTibcoEMSGatewayResolver = "CustomTibcoEMSGatewayResolver";

        /// <summary>
        /// Custom TibcoEMS Gateway Resolver table Subject Property
        /// </summary>
        public const string GenericTableCustomTibcoEMSGatewayResolverSubjectProperty = "Subject";

        /// <summary>
        /// Custom TibcoEMS Gateway Resolver table Topic Property
        /// </summary>
        public const string GenericTableCustomTibcoEMSGatewayResolverTopicProperty = "Topic";

        /// <summary>
        /// Custom TibcoEMS Gateway Resolver table ReplyTo Property
        /// </summary>
        public const string GenericTableCustomTibcoEMSGatewayResolverReplyToProperty = "ReplyTo";

        /// <summary>
        /// Custom TibcoEMS Gateway Resolver table Rule Property
        /// </summary>
        public const string GenericTableCustomTibcoEMSGatewayResolverRuleProperty = "Rule";

        /// <summary>
        /// Custom TibcoEMS Gateway Resolver table IsEnabled Property
        /// </summary>
        public const string GenericTableCustomTibcoEMSGatewayResolverIsEnabledProperty = "IsEnabled";

        /// <summary>
        /// Custom TibcoEMS Gateway Resolver table QueueMessage Property
        /// </summary>
        public const string GenericTableCustomTibcoEMSGatewayResolverQueueMessageProperty = "QueueMessage";

        /// <summary>
        /// Custom TibcoEMS Gateway Resolver table TextMessage Property
        /// </summary>
        public const string GenericTableCustomTibcoEMSGatewayResolverTextMessageProperty = "TextMessage";

        /// <summary>
        /// Custom TibcoEMS Gateway Resolver table CompressMessage Property
        /// </summary>
        public const string GenericTableCustomTibcoEMSGatewayResolverCompressMessageProperty = "CompressMessage";

        #endregion Generic Table

        #region Message Bus Subjects

        /// <summary>
        /// CustomTibcoEMSGatewayInvalidateCache
        /// </summary>
        public const string SubjectCustomTibcoEMSGatewayInvalidateCache = "CustomTibcoEMSGatewayInvalidateCache";

        #endregion Message Bus Subjects

        #region Log Messages

        /// <summary>
        /// DefaultLogDataFormat: Message Received >> SUBJECT: {0}; TOPIC: {1}; ACTION: {2}
        /// </summary>
        public static string DefaultLogDataFormat = "Message Received >> SUBJECT: {0}; TOPIC: {1}; ACTION: {2}";

        #endregion Log Messages

        #region DEE Actions

        /// <summary>
        /// DEE Action: CustomGetTibcoConfigurations
        /// </summary>
        public const string CustomGetTibcoConfigurations = "CustomGetTibcoConfigurations";

        /// <summary>
        /// DEE Action: CustomTibcoEMSReplyHandler
        /// </summary>
        public const string CustomTibcoEMSReplyHandler = "CustomTibcoEMSReplyHandler";

        #endregion DEE Actions

        #region TibcoEMS Properties

        /// <summary>
        /// Property: TibcoEMSPropertyCompressTextMessage
        /// </summary>
        public const string TibcoEMSPropertyCompressTextMessage = "JMS_TIBCO_COMPRESS";

        /// <summary>
        /// Property: TibcoEMSPropertyMapMessageField
        /// </summary>
        public const string TibcoEMSPropertyMapMessageField = "field";

        #endregion TibcoEMS Properties
    }
}