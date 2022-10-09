using Newtonsoft.Json;

namespace Cmf.Custom.Tests.Biz.Common.Tibco
{
    public class TibcoCustomSendEventMessage
    {
        /// <summary>
        /// A User's username. eg: "sergiotapia, mrkibbles, matumbo"
        /// </summary>
        [JsonProperty("Header")]
        public CustomSendEventMessageHeader Header { get; set; }

        /// <summary>
        /// A User's name. eg: "Sergio Tapia, John Cosack, Lucy McMillan"
        /// </summary>
        [JsonProperty("Message")]
        public string Message { get; set; }
    }

    public class CustomSendEventMessageHeader
    {
        /// <summary>
        /// A User's username. eg: "sergiotapia, mrkibbles, matumbo"
        /// </summary>
        [JsonProperty("stdObjectName")]
        public string stdObjectName { get; set; }

        /// <summary>
        /// A User's name. eg: "Sergio Tapia, John Cosack, Lucy McMillan"
        /// </summary>
        [JsonProperty("stdFrom")]
        public string stdFrom { get; set; }

        /// <summary>
        /// A User's name. eg: "Sergio Tapia, John Cosack, Lucy McMillan"
        /// </summary>
        [JsonProperty("stdTo")]
        public string stdTo { get; set; }

        /// <summary>
        /// A User's name. eg: "Sergio Tapia, John Cosack, Lucy McMillan"
        /// </summary>
        [JsonProperty("stdProductType")]
        public string stdProductType { get; set; }

        /// <summary>
        /// A User's name. eg: "Sergio Tapia, John Cosack, Lucy McMillan"
        /// </summary>
        [JsonProperty("stdDataOrigin")]
        public string stdDataOrigin { get; set; }

        /// <summary>
        /// A User's name. eg: "Sergio Tapia, John Cosack, Lucy McMillan"
        /// </summary>
        [JsonProperty("stdTransaction")]
        public string stdTransaction { get; set; }
    }
}