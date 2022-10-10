using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures
{
    /// <summary>
    /// Defines the Custom Import PRoduction Order Collection.
    /// </summary>
    [XmlRoot("CustomImportProductionOrders")]
    public class CustomImportProductionOrderCollection : List<CustomImportProductionOrder>
    {

    }
}
