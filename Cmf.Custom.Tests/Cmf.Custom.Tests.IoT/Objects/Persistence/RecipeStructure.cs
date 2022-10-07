using Newtonsoft.Json;

namespace amsOSRAMEIAutomaticTests.Objects.Persistence
{
    public class RecipeStructure
    {
        /// <summary>
        /// Recipe Resource Name
        /// </summary>
        public string RecipeName { get; set; }

        /// <summary>
        /// Parameter Name
        /// </summary>
        public string RecipeId { get; set; }

        /// <summary>
        /// Parameter Value
        /// </summary>
        public string NameOnEquipement { get; set; }

        /// <summary>
        /// Parameter Value Type
        /// </summary>
        public string Checksum { get; set; }

        /// <summary>
        /// Parameter ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long Order { get; set; }

        /// <summary>
        /// Parameter Units
        /// </summary>
        public RecipeStructure[] SubRecipes { get; set; }
    }
}
