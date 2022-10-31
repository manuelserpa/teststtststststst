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
        /// ReplyTo
        /// </summary>
        public string ReplyTo { get; set; }

        /// <summary>
        /// Rule
        /// </summary>
        public string Rule { get; set; }

        /// <summary>
        /// IsToQueueMessage
        /// </summary>
        public bool IsQueue { get; set; }

        /// <summary>
        /// IsToCompressMessage
        /// </summary>
        public bool IsToCompress { get; set; }

        /// <summary>
        /// IsTextMessage
        /// </summary>
        public bool IsTextMessage { get; set; }
    }
}