using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.GenericTables;
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
             *    DEE Action used to generate new Lot names.
             *      - The lot names will be generated based on ProductionLine Product attribute.
             *      - The 6-digits counter will only use the alphanumeric digits defined in the Config.
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
                    // Set ProductLine Product attribute
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
                    // Load Generic Table CustomProductionLineConversion
                    GenericTable customProdLineConversionGT = new GenericTable() { Name = AMSOsramConstants.GenericTableCustomProductionLineConversion };
                    customProdLineConversionGT.Load();

                    // Based on ProductLine Product attribute get Site and Facility name from Generic Table
                    customProdLineConversionGT.LoadData(new Foundation.BusinessObjects.QueryObject.FilterCollection()
                    {
                        new Foundation.BusinessObjects.QueryObject.Filter()
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

                        // Set Site associated to ProductionLine attribute to Context 
                        ApplicationContext.CallContext.SetInformationContext("SiteName", prodLineConversionDataSet.Tables[0].Rows[0][AMSOsramConstants.GenericTableCustomProductionLineConversionSiteProperty]);

                        // Set Facility associated to ProductionLine attribute to Context 
                        ApplicationContext.CallContext.SetInformationContext("FacilityName", prodLineConversionDataSet.Tables[0].Rows[0][AMSOsramConstants.GenericTableCustomProductionLineConversionFacilityProperty]);

                        // Get alphanumeric allowed digits from Config
                        if (Config.TryGetConfig(AMSOsramConstants.DefaultProductionLotNameAlphanumericAllowedDigits, out Config prodLotNameAplhanumericAllowedDigitsConfig) &&
                            !string.IsNullOrWhiteSpace(prodLotNameAplhanumericAllowedDigitsConfig.GetConfigValue<string>()))
                        {
                            // Set alphanumeric allowed digits Config value to Context 
                            ApplicationContext.CallContext.SetInformationContext("AlphanumericAllowedDigits", prodLotNameAplhanumericAllowedDigitsConfig.GetConfigValue<string>());

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

            UseReference("", "System.Data");
            UseReference("", "System.Text");

            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration");
            UseReference("", "Cmf.Foundation.BusinessObjects.GenericTables");
            UseReference("", "Cmf.Foundation.Common");
            UseReference("", "Cmf.Foundation.Common.Exceptions");

            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");

            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");

            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");

            // Load Name Generator
            NameGenerator customGenProdLotNamesNG = new NameGenerator() { Name = AMSOsramConstants.CustomGenerateProductionLotNames };
            customGenProdLotNamesNG.Load();

            // Get Site name from Context
            string siteName = ApplicationContext.CallContext.GetInformationContext("SiteName") as string;

            // Get Facility name from Context
            string facilityName = ApplicationContext.CallContext.GetInformationContext("FacilityName") as string;

            // Get alphanumeric allowed digits from Context
            string alphanumericAllowedDigits = ApplicationContext.CallContext.GetInformationContext("AlphanumericAllowedDigits") as string;

            // Get alphanumeric allowed digits size
            int alphaNumericDigitsSize = alphanumericAllowedDigits.Length;

            // Set number of characters that will be generated
            int numberOfCharacters = 6;

            // Lot Name Generator builder
            StringBuilder generatedLotName = new StringBuilder();

            // Site name identifier
            generatedLotName.Append(siteName.Substring(0, 1));

            // Facility name identifier
            generatedLotName.Append(facilityName.Substring(0, 1));

            // Context key
            string contextKey = generatedLotName.ToString();

            // Load Current Context associated to Name Generator
            GeneratorContext contextNG = null;
            customGenProdLotNamesNG.LoadGeneratorContexts(out int totalRows);
            contextNG = customGenProdLotNamesNG.Contexts.FirstOrDefault(ng => ng.Context == contextKey);

            // Create counter
            int lastCounterValue = 0;

            if (contextNG != null)
            {
                // Get last counter value from NG Context
                lastCounterValue = contextNG.LastCounterValue;
            }

            // Increment last counter value
            lastCounterValue++;

            // Set next counter value
            int nextCounterValue = lastCounterValue;

            string alphanumericCounter = string.Empty;

            for (int i = 0; i < numberOfCharacters; i++)
            {
                int currLetterInt = lastCounterValue % alphaNumericDigitsSize;
                alphanumericCounter += (char)alphanumericAllowedDigits[0 + currLetterInt];
                lastCounterValue /= alphaNumericDigitsSize;
            }

            // Revert the counter Chars order
            char[] counterChars = alphanumericCounter.ToCharArray();
            Array.Reverse(counterChars);
            alphanumericCounter = new string(counterChars);

            // 6-digits Counter Value identifier
            generatedLotName.Append(alphanumericCounter);

            // Split Lot counter identifier
            generatedLotName.Append(AMSOsramConstants.CustomNameGeneratorSplitLotCounter);

            // Save Name Generator context
            if (contextNG != null)
            {
                contextNG.LastCounterValue = nextCounterValue;
                contextNG.Save();
            }
            else
            {
                customGenProdLotNamesNG.AddGeneratorContexts(new GeneratorContextCollection()
                {
                    new GeneratorContext
                    {
                        Context = contextKey,
                        LastCounterValue = nextCounterValue
                    }
                });
            }

            // Set generated Lot name to returned Collection
            Input.Add("Result", generatedLotName.ToString());

            //---End DEE Code---

            return Input;
        }
    }
}