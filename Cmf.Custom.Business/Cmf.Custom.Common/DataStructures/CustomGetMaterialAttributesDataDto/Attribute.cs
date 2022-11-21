using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures.CustomGetMaterialAttributesDataDto
{
    /// <summary>
    /// Attribute information class
    /// </summary>
    public class Attribute
    {
        [XmlAttribute()]
        public string Name { get; set; }

        [XmlText()]
        public string Value { get; set; }
    }
}
