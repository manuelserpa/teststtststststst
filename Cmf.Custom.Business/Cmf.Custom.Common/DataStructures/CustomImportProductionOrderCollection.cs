using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Cmf.Custom.AMSOsram.Common.DataStructures
{
    /// <summary>
    /// Defines the Custom Import PRoduction Order Collection.
    /// </summary>
    [XmlRoot]
    public class CustomImportProductionOrderCollection : List<CustomImportProductionOrder>
    {

    }
}
