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
    }
}
