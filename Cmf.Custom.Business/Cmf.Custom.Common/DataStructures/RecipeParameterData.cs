using System.Runtime.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures
{
    /// <summary>
    /// Support class that represents a recipe parameter
    /// </summary>
    [DataContract]
    public class RecipeParameterData
    {
        /// <summary>
        /// The name of the given parameter
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// The value of the given parameter
        /// </summary>
        [DataMember]
        public string Value { get; set; }
    }
}
