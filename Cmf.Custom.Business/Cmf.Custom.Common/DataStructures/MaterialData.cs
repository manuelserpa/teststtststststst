using System.Collections.Generic;

namespace Cmf.Custom.AMSOsram.Common.DataStructures
{
	/// <summary>
	/// Support class to send Material data to IoT
	/// </summary>
	public class MaterialData
    {
        /// <summary>
        /// Material ID
        /// </summary>
        public string MaterialId { get; set; }

        /// <summary>
        /// Material Name
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// Top-Most material (lot) -> CurrentMainState.StateModel.Name
        /// Sub-Material system state -> SystemState (MaterialStateEnum)
        /// </summary>
        public string MaterialState { get; set; }

        /// <summary>
        /// Sub Material List
        /// </summary>
        public List<MaterialData> SubMaterials { get; set; }

        /// <summary>
        /// Recipe Information
        /// </summary>
        public RecipeData Recipe { get; set; }

        /// <summary>
        /// Container Name (Carrier Name)
        /// </summary>
        public string ContainerName { get; set; }

        /// <summary>
        /// Container Id (Carrier Id)
        /// </summary>
        public string ContainerId { get; set; }

        /// <summary>
        /// Carrier position (Wafer position in the Container)
        /// </summary>
        /// <remarks>Intended to be used for sub-materials</remarks>
        public string Slot { get; set; }

        /// <summary>
        /// Last Time the Material was updated  (For Iot only)
        /// </summary>
        public string LastUpdate { get; set; }

        /// <summary>
        /// Load Port Name
        /// </summary>
        public string LoadPortName { get; set; }

        /// <summary>
        /// Load Port Position
        /// </summary>
        public string LoadPortPosition { get; set; }

        /// <summary>
        /// Check if the tool is able to download recipes at TrackIn
        /// </summary>
        public bool AllowDownloadRecipeAtTrackIn { get; set; }

        /// <summary>
        /// Check to see if we process only the container and not the material
        /// </summary>
        public bool ContainerOnlyProcess { get; set; }

        /// <summary>
        /// The sorter job information
        /// </summary>
        //public CustomSorterJobDefinition SorterJobInformation { get; set; }
    }
}