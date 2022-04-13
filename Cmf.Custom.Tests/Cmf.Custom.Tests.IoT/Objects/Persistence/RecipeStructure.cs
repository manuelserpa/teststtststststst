using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMSOsramEIAutomaticTests.Objects.Persistence
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
