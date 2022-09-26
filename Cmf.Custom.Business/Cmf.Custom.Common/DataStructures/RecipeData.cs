using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures
{
    /// <summary>
    /// Support class to send Recipe data to IoT
    /// </summary>
    [DataContract]
    public class RecipeData
    {
        /// <summary>
        /// Recipe Name
        /// </summary>
        [DataMember]
        public string RecipeName { get; set; }

        /// <summary>
        /// Recipe Id
        /// </summary>
        [DataMember]
        public string RecipeId { get; set; }

        /// <summary>
        /// Recipe name on equipment 
        /// </summary>
        [DataMember]
        public string NameOnEquipment { get; set; }

        /// <summary>
        /// Recipe Checksum (recipe.BodyChecksum)
        /// </summary>
        [DataMember]
        public string Checksum { get; set; }

        /// <summary>
        /// Recipe order
        /// </summary>
        [DataMember]
        public string Order { get; set; }

        /// <summary>
        /// Sub Recipes
        /// </summary>
        [DataMember]
        public List<RecipeData> SubRecipes { get; set; }

        /// <summary>
        /// Recipe Parameters
        /// </summary>
        [DataMember]
        public List<RecipeParameterData> RecipeParameters { get; set; }
    }
}
