namespace Cmf.Custom.TibcoEMS.ServiceManager.DataStructures
{
    /// <summary>
    /// Data structure to support resolve Tibco topics
    /// </summary>
    public class TibcoResolverDto
    {
        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Topic
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Rule
        /// </summary>
        public string Rule { get; set; }

        /// <summary>
        /// IsToQueueMessage
        /// </summary>
        public bool IsToQueueMessage { get; set; }

        /// <summary>
        /// IsToCompressMessage
        /// </summary>
        public bool IsToCompressMessage { get; set; }

        /// <summary>
        /// IsTextMessage
        /// </summary>
        public bool IsTextMessage { get; set; }
    }
}
