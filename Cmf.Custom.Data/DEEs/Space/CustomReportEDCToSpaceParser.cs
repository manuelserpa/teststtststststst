using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using System;
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
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.DataStructures");

            if (Input != null)
            {
                Material material = new Material() { Name = Input["Material"].ToString() };
                material.Load();

                string message = string.Empty;

                Facility facility = material.Facility;
                facility.Load();
                Site site = facility.Site;
                site.Load();
                site.LoadAttributes(new Collection<string>() { AMSOsramConstants.CustomSiteCodeAttribute });
                string siteCode = site.Attributes[AMSOsramConstants.CustomSiteCodeAttribute].ToString();

                string recipeName = material.CurrentRecipeInstance != null ? material.CurrentRecipeInstance.ParentEntity.Name : string.Empty;

                DataCollectionInstance dataCollectionInstance = new DataCollectionInstance() { Name = Input["DataCollectionInstance"].ToString() };
                dataCollectionInstance.Load();

                DataCollectionLimitSet limitSet = new DataCollectionLimitSet() { Name = Input["LimitSet"].ToString() };
                limitSet.Load();
                limitSet.LoadRelations(Cmf.Navigo.Common.Constants.DataCollectionParameterLimit);

                string host = System.Configuration.ConfigurationManager.AppSettings["ServerName"];

                // Create Lot Values Message
                CustomReportEDCToSpace customSendLotDCInformation = AMSOsramUtilities.CreateSpaceInfoWaferValues(material, dataCollectionInstance, limitSet, host, new List<string>() { siteCode }, recipeName);

                // Serialize object to XML 
                message = customSendLotDCInformation.SerializeToXML();

                // create integration entry for demo 
                string name = $"SpaceEDC_{material.Name}_{Guid.NewGuid().ToString("N").Substring(0, 5)}";
                AMSOsramUtilities.CreateOutboundIntegrationEntry(message, string.Empty, name);

                Input.Add("Message", message);
            }
            //---End DEE Code---

            return Input;
        }
    }
}
