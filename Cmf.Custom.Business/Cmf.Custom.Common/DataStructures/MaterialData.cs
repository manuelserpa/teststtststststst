using System.Collections.Generic;
using System.Runtime.Serialization;
using Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures
{
    /// <summary>
    /// Support class to send Material data to IoT
    /// </summary>
    [DataContract]
    public class MaterialData
    {
        /// <summary>
        /// Material ID
        /// </summary>
        [DataMember]
        public string MaterialId { get; set; }

        /// <summary>
        /// Material Name
        /// </summary>
        [DataMember]
        public string MaterialName { get; set; }

        /// <summary>
        /// Top-Most material (lot) -> CurrentMainState.StateModel.Name
        /// Sub-Material system state -> SystemState (MaterialStateEnum)
        /// </summary>
        [DataMember]
        public string MaterialState { get; set; }

        /// <summary>
        /// Sub Material List
        /// </summary>
        [DataMember]
        public List<MaterialData> SubMaterials { get; set; }

        /// <summary>
        /// Recipe Information
        /// </summary>
        [DataMember]
        public RecipeData Recipe { get; set; }

        /// <summary>
        /// Container Name (Carrier Name)
        /// </summary>
        [DataMember]
        public string ContainerName { get; set; }

        /// <summary>
        /// Container Id (Carrier Id)
        /// </summary>
        [DataMember]
        public string ContainerId { get; set; }

        /// <summary>
        /// Carrier position (Wafer position in the Container)
        /// </summary>
        /// <remarks>Intended to be used for sub-materials</remarks>
        [DataMember]
        public string Slot { get; set; }

        /// <summary>
        /// Last Time the Material was updated  (For Iot only)
        /// </summary>
        [DataMember]
        public string LastUpdate { get; set; }

        /// <summary>
        /// Load Port Name
        /// </summary>
        [DataMember]
        public string LoadPortName { get; set; }

        /// <summary>
        /// Load Port Position
        /// </summary>
        [DataMember]
        public string LoadPortPosition { get; set; }

        /// <summary>
        /// Check if the tool is able to download recipes at TrackIn
        /// </summary>
        [DataMember]
        public bool AllowDownloadRecipeAtTrackIn { get; set; }

        /// <summary>
        /// Check to see if we process only the container and not the material
        /// </summary>
        [DataMember]
        public bool ContainerOnlyProcess { get; set; }

        /// <summary>
        /// The sorter job information
        /// </summary>
        [DataMember]
        public ICustomSorterJobDefinition SorterJobInformation { get; set; }
    }
}
