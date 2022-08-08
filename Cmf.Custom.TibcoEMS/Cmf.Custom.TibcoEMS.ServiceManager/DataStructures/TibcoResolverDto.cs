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
        /// QueueFlag
        /// </summary>
        public bool QueueFlag { get; set; }

        /// <summary>
        /// CompressFlag
        /// </summary>
        public bool CompressFlag { get; set; }

        /// <summary>
        /// MapTextFlag
        /// </summary>
        public bool MapTextFlag { get; set; }
    }
}
