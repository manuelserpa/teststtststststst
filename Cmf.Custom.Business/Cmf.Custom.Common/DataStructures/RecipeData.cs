using System.Collections.Generic;

namespace Cmf.Custom.AMSOsram.Common.DataStructures
{
    /// <summary>
    /// Support class to send Recipe data to IoT
    /// </summary>
    public class RecipeData
    {
        /// <summary>
        /// Recipe Name
        /// </summary>
        public string RecipeName { get; set; }

        /// <summary>
        /// Recipe Id
        /// </summary>
        public string RecipeId { get; set; }

        /// <summary>
        /// Recipe name on equipment 
        /// </summary>
        public string NameOnEquipment { get; set; }

        /// <summary>
        /// Recipe Checksum (recipe.BodyChecksum)
        /// </summary>
        public string Checksum { get; set; }

        /// <summary>
        /// Recipe order
        /// </summary>
        public string Order { get; set; }

        /// <summary>
        /// Sub Recipes
        /// </summary>
        public List<RecipeData> SubRecipes { get; set; }

        /// <summary>
        /// Recipe Parameters
        /// </summary>
        public List<RecipeParameterData> RecipeParameters { get; set; }
    }
}
