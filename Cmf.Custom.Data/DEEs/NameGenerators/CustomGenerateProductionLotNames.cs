using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Cmf.Custom.AMSOsram.Actions.NameGenerators
{
    public class CustomGenerateProductionLotNames : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *      DEE action used to generate new Lot Names.
             *
             * Action Groups:
             *      N/A.
            */

            #endregion

            bool canExecute = false;

            if (Input != null && Input.ContainsKey("EntitySource"))
            {
                Material material = Input["EntitySource"] as Material;

                string productionLine = string.Empty;

                if (material != null && material.Product != null)
                {
                    if (material.Product.HasRelatedAttribute(AMSOsramConstants.ProductAttributeProductionLine, true))
                    {
                        productionLine = material.Product.GetRelatedAttributeValue(AMSOsramConstants.ProductAttributeProductionLine) as string;
                    }
                    else
                    {
                        return canExecute;
                    }
                }

                if (!string.IsNullOrWhiteSpace(productionLine))
                {
                    GenericTable customProdLineConversionGT = new GenericTable() { Name = AMSOsramConstants.GenericTableCustomProductionLineConversion };
                    customProdLineConversionGT.Load();

                    customProdLineConversionGT.LoadData(new FilterCollection()
                    {
                        new Filter()
                        {
                            Name = AMSOsramConstants.GenericTableCustomProductionLineConversionProductionLineProperty,
                            Operator = FieldOperator.IsEqualTo,
                            LogicalOperator = LogicalOperator.Nothing,
                            Value = productionLine
                        }
                    });

                    if (customProdLineConversionGT.HasData)
                    {
                        DataSet prodLineConversionDataSet = NgpDataSet.ToDataSet(customProdLineConversionGT.Data);

                        ApplicationContext.CallContext.SetInformationContext("SiteName", prodLineConversionDataSet.Tables[0].Rows[0][AMSOsramConstants.GenericTableCustomProductionLineConversionSiteProperty]);
                        ApplicationContext.CallContext.SetInformationContext("FacilityName", prodLineConversionDataSet.Tables[0].Rows[0][AMSOsramConstants.GenericTableCustomProductionLineConversionFacilityProperty]);

                        if (Config.TryGetConfig(AMSOsramConstants.DefaultProductionLotNameAllowedDigits, out Config prodLotNameAllowedDigitsConfig) &&
                            !string.IsNullOrWhiteSpace(prodLotNameAllowedDigitsConfig.GetConfigValue<string>()))
                        {
                            ApplicationContext.CallContext.SetInformationContext("AlphanumericAllowedDigits", prodLotNameAllowedDigitsConfig.GetConfigValue<string>());

                            canExecute = true;
                        }
                    }
                }
            }

            return canExecute;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("", "Cmf.Foundation.Common.Exceptions");
            UseReference("", "Cmf.Foundation.Common");
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");

            // Load Name Generator
            NameGenerator customGenProdLotNamesNG = new NameGenerator() { Name = AMSOsramConstants.CustomGenerateProductionLotNames };
            customGenProdLotNamesNG.Load();

            // Get Site Name from Context
            string siteName = ApplicationContext.CallContext.GetInformationContext("SiteName") as string;

            // Get Facility Name from Context
            string facilityName = ApplicationContext.CallContext.GetInformationContext("FacilityName") as string;

            // Get Allowed Digits from Context
            string allowedDigits = ApplicationContext.CallContext.GetInformationContext("AlphanumericAllowedDigits") as string;

            // Lot Name Generator builder
            StringBuilder nameLotGenerated = new StringBuilder();

            // Site name identifier
            nameLotGenerated.Append(siteName.Substring(0, 1));

            // Facility name identifier
            nameLotGenerated.Append(facilityName.Substring(0, 1));

            // Context key
            string contextKey = nameLotGenerated.ToString();

            // Load Current Context associated to Name Generator
            GeneratorContext contextNG = null;
            customGenProdLotNamesNG.LoadGeneratorContexts(out int totalRows);
            contextNG = customGenProdLotNamesNG.Contexts.FirstOrDefault(ng => ng.Context == contextKey);

            string counterValue = string.Empty;

            if (contextNG != null)
            {
                // Counter based on ASCII Table
                counterValue = string.Format("{0:000000000000}", contextNG.LastCounterValue);
            }
            else
            {
                /* Based on ASCII Table:
                 *  - Dec: 48 is equals to Char: 0
                 *  - Default: "48 48 48 48 48 48" => "0 0 0 0 0 0"
                 */
                counterValue = "484848484848";
            }

            // Get Char value from the Substring Decimal value
            char firstDigit = Convert.ToChar(Convert.ToInt32(counterValue.Substring(0, 2)));
            char secondDigit = Convert.ToChar(Convert.ToInt32(counterValue.Substring(2, 2)));
            char thirdDigit = Convert.ToChar(Convert.ToInt32(counterValue.Substring(4, 2)));
            char fourthDigit = Convert.ToChar(Convert.ToInt32(counterValue.Substring(6, 2)));
            char fifthDigit = Convert.ToChar(Convert.ToInt32(counterValue.Substring(8, 2)));
            char sixthDigit = Convert.ToChar(Convert.ToInt32(counterValue.Substring(10, 2)));

            string newCounterValue = $"{firstDigit}{secondDigit}{thirdDigit}{fourthDigit}{fifthDigit}{sixthDigit}";

            bool addValue = true;

            // Calculate new counter Value
            for (int i = newCounterValue.Length - 1; i >= 0 && addValue; i--)
            {
                int position = allowedDigits.IndexOf(newCounterValue[i]);

                if (position != (allowedDigits.Length - 1))
                {
                    newCounterValue = newCounterValue.Remove(i, 1).Insert(i, allowedDigits[position + 1].ToString());
                    addValue = false;
                }
                else
                {
                    newCounterValue = newCounterValue.Remove(i, 1).Insert(i, allowedDigits[0].ToString());
                }
            }

            // 6-digits Counter Value identifier
            nameLotGenerated.Append(newCounterValue);

            // Split Lot counter identifier
            nameLotGenerated.Append("00");

            // Convert the counter value to Integer
            string lastCounterValue = string.Empty;

            foreach (char value in newCounterValue)
            {
                lastCounterValue += string.Format("{0:00}", Convert.ToInt32(value));
            }

            // Save Name Generator context
            if (contextNG != null)
            {
                contextNG.LastCounterValue = Convert.ToInt32(lastCounterValue);
                contextNG.Save();
            }
            else
            {
                customGenProdLotNamesNG.AddGeneratorContexts(new GeneratorContextCollection()
                {
                    new GeneratorContext
                    {
                        Context = contextKey,
                        LastCounterValue = Convert.ToInt32(lastCounterValue)
                    }
                });
            }

            Input.Add("Result", nameLotGenerated.ToString());

            //---End DEE Code---

            return Input;

        }

    }
}