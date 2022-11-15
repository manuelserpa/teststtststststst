using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cmf.Custom.amsOSRAM.Orchestration.DataStructures
{
    public class CustomGetMaterialAttributesInputDS
    {
        public MaterialCollection MaterialName { get; set; }
        public AttributeCollection AttributeName { get; set; }
        public bool IncludeSubMaterial { get; set; }
    }
}
