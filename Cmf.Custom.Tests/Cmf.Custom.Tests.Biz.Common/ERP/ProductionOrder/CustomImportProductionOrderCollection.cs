using Cmf.Custom.Tests.Biz.Common.ERP.ProductionOrder;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Cmf.Custom.Tests.Biz.Common.ERP.ProductionOrder
{
    /// <summary>
    /// Defines the Custom Import PRoduction Order Collection.
    /// </summary>
    [XmlRoot("CustomImportProductionOrders")]
    public class CustomImportProductionOrderCollection : List<CustomImportProductionOrder>
    {

    }
}
