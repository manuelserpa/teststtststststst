using Cmf.Custom.AMSOsram.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMSOsramEIAutomaticTests.Objects.Persistence
{
    public class MaterialData
    {
        /// <summary>
        /// MO material ID
        /// </summary>
        public string MaterialId { get; set; }

        /// <summary>
        /// MO material Name
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// Material Quantity
        /// </summary>
        public MaterialStateEnum MaterialState { get; set; }

        /// <summary>
        /// Quantity Produced 
        /// </summary>
        public string ContainerName { get; set; }

        /// <summary>
        /// Quantity Outsorted
        /// </summary>
        public SubMaterialStructure[] SubMaterials { get; set; }

        /// <summary>
        /// Quantity Infeeder
        /// </summary>
        public RecipeStructure Recipe { get; set; }

        /// <summary>
        /// The sorter job information
        /// </summary>
        public CustomSorterJobDefinition SorterJobInformation { get; set; }

        /// <summary>
        /// The sorter job information
        /// </summary>
        public string ProcessJobId { get; set; }

        /// <summary>
        /// The sorter job information
        /// </summary>
        public string ControlJobId { get; set; }
    }

    public enum MaterialStateEnum
    {
        Setup,
        InProcess,
        Complete,
        Aborted
    }
}
