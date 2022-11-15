using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;

namespace Cmf.Custom.amsOSRAM.Orchestration.DataStructures
{
    public class CustomGetMaterialAttributesInputDS
    {
        public MaterialCollection MaterialName { get; set; }
        public AttributeCollection AttributeName { get; set; }
        public bool IncludeSubMaterial { get; set; }
    }
}
