using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cmf.Custom.AMSOsram.Actions.Space
{
    public class CustomReportEDCToSpaceParser : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---   

            #region Info
            /// <summary>
            /// Summary text
            ///     
            /// Action Groups:
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion

            return true;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            //System
            UseReference("", "System.Linq");
            UseReference("", "System.Collections.Generic");
            UseReference("", "System.IO");
            UseReference("", "System.Threading");
            UseReference("", "System");

            //Foundation
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");

            //Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");

            //Custom

            if (Input != null)
            {
                Material material = Input["Material"] as Material;
                material.Load();

                string message = string.Empty;

                Facility facility = material.Facility;
                facility.Load();
                Site site = facility.Site;
                site.Load();
                site.LoadAttributes(new Collection<string>() { AMSOsramConstants.CustomSiteCodeAttribute });
                string siteCode = site.Attributes[AMSOsramConstants.CustomSiteCodeAttribute].ToString();

                if (material.ParentMaterial == null)
                {
                    // Create Lot Values Message
                    //CustomReportEDCToSpace customSendLotDCInformation = AMSOsramUtilities.CreateSpaceInfoLotValues(material, dataCollectionInstance, "", new List<string>() { siteCode });

                    // Serialize object to XML 
                    //message = customSendLotDCInformation.SerializeToXML();
                }
                else
                {
                    // Create Wafer Values Message
                    //CustomReportEDCToSpace customSendWaferDCInformation = AMSOsramUtilities.CreateSpaceInfoWaferValues(material, dataCollectionInstance, "", new List<string>() { siteCode });

                    // Serialize object to XML 
                    //message = customSendWaferDCInformation.SerializeToXML();
                }

                // create integration entry for demo 

                Input.Add("Message", message);
            }
            //---End DEE Code---

            return Input;
        }
    }
}
