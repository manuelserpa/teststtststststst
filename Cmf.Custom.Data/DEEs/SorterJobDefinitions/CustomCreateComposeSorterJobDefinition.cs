using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cmf.Custom.amsOSRAM.Actions.SorterJobDefinitions
{
    public class CustomCreateComposeSorterJobDefinition : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *     DEE Action responsible for the creation of compose custom sorter job definitions.
            */

            #endregion Info

            return true;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.BusinessObjects.CustomSorterJobDefinition.dll", " Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.Extensions");
            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");

            // System
            UseReference("", "System.Data");
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");

            IServiceProvider serviceProvider = DeeActionHelper.GetInputItem<IServiceProvider>(Input, "ServiceProvider");
            IBOM bom = DeeActionHelper.GetInputItem<IBOM>(Input, "BOM");

            if (bom == null)
            {
                throw new ArgumentNullCmfException("BOM");
            }

            // BomName.Revision.Version
            string BOMNameFormat = "{0}.{1}.{2}";
            string customSorterJobDefinitionName = String.Format(BOMNameFormat, bom.Name, bom.Revision, bom.Version);

            ICustomSorterJobDefinition customSorterJobDefinition = serviceProvider.GetService<ICustomSorterJobDefinition>();
            customSorterJobDefinition.Name = customSorterJobDefinitionName;

            bool foundCustomSorterJobDefinition = customSorterJobDefinition.ObjectExists();

            if (!foundCustomSorterJobDefinition)
            {
                bom.LoadAttributes(new Collection<string> { amsOSRAMConstants.BOMAttributeIsForLotCompose, amsOSRAMConstants.BOMAttributeIsStartingCarrierType });
                bom.LoadRelations("BOMProduct");

                if (bom.RelatedAttributes.TryGetValueAs("StartingCarrierType", out string startingCarrierType))
                {
                    JArray jArray = new JArray();

                    Dictionary<long, List<long>> subs = new Dictionary<long, List<long>>();
                    Dictionary<long, int> parentBomProd = new Dictionary<long, int>();
                    HashSet<long> productIds = new HashSet<long>();

                    bom.BomProducts.Load();

                    foreach (IBOMProduct bomProduct in bom.BomProducts)
                    {
                        long bomId = bomProduct.GetNativeValue<long>("TargetEntity");
                        productIds.Add(bomId);

                        if (bomProduct.Parent == null)
                        {
                            parentBomProd.Add(bomId, Convert.ToInt32(bomProduct.Quantity ?? 0));
                        }
                        else
                        {
                            long parentBOMId = bomProduct.Parent.GetNativeValue<long>("TargetEntity");
                            productIds.Add(parentBOMId);

                            if (subs.ContainsKey(parentBOMId))
                            {
                                subs[parentBOMId].Add(bomId);
                            }
                            else
                            {
                                subs[parentBOMId] = new List<long>
                                {
                                    bomId
                                };
                            }
                        }
                    }

                    IProductCollection products = serviceProvider.GetService<IProductCollection>();
                    products.LoadByIDs(productIds.ToList());

                    int containerPosition = 0;
                    foreach (KeyValuePair<long, int> parentBomProductNeeded in parentBomProd)
                    {
                        JArray jArraySub = new JArray();
                        int subPriority = 1;

                        if (subs.ContainsKey(parentBomProductNeeded.Key))
                        {
                            foreach (long subId in subs[parentBomProductNeeded.Key])
                            {
                                JObject jObject = new JObject
                                {
                                    ["Substitute"] = products.FirstOrDefault(f => f.Id == subId).Name,
                                    ["Priority"] = subPriority
                                };
                                jArraySub.Add(jObject);
                                subPriority++;
                            }
                        }

                        for (int i = 1; i <= parentBomProductNeeded.Value; i++)
                        {
                            containerPosition++;

                            JObject jObject = new JObject
                            {
                                ["Product"] = products.FirstOrDefault(f => f.Id == parentBomProductNeeded.Key).Name,
                                ["Substitutes"] = jArraySub,
                                ["SourceContainer"] = "",
                                ["MaterialName"] = "",
                                ["DestinationContainer"] = "",
                                ["SourcePosition"] = 0,
                                ["DestinationPosition"] = containerPosition
                            };

                            jArray.Add(jObject);
                        }
                    }

                    if (jArray.Count > 0)
                    {
                        customSorterJobDefinition.SourceCarrierType = startingCarrierType;
                        customSorterJobDefinition.TargetCarrierType = startingCarrierType;
                        customSorterJobDefinition.LogisticalProcess = amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessCompose;
                        customSorterJobDefinition.FlipWafer = false;
                        customSorterJobDefinition.ReadWaferId = true;
                        customSorterJobDefinition.WaferIdOnBottom = true;
                        customSorterJobDefinition.MovementList = new JObject
                        {
                            [amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertyFutureActionType] = "Compose",
                            [amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertyMoves] = jArray,
                            [amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertyDeleteOnCompletion] = false
                        }.ToString();
                        customSorterJobDefinition.Create();
                    }
                }
            }

            if (foundCustomSorterJobDefinition || customSorterJobDefinition.Id > 0)
            {
                if (customSorterJobDefinition.Id <= 0)
                {
                    customSorterJobDefinition.Load();
                }

                Input.Add("CustomSorterJobDefinition", customSorterJobDefinition);
            }

            //---End DEE Code---

            return Input;
        }
    }
}