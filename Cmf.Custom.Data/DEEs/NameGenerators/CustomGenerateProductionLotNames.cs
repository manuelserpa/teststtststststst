using System.Collections.Generic;
using System;
using System.Linq;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.Configuration;
using Cmf.Foundation.Configuration.Abstractions;
using Cmf.Common.CustomActionUtilities;
using Cmf.Foundation.Common;
using Cmf.Foundation.BusinessObjects.GenericTables;
using System.Data;
using System.Text;
using Cmf.Foundation.BusinessObjects;

namespace Cmf.Custom.amsOSRAM.Actions.NameGenerators
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
                IMaterial material = Input["EntitySource"] as IMaterial;

                if (material != null && material.Product != null && material.Form.Equals(amsOSRAMConstants.MaterialLotForm, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Throw an exception case Configuration has no associated value
                    if (!Config.TryGetConfig(amsOSRAMConstants.DefaultLotNameAllowedCharacters, out IConfig lotNameAllowedCharactersConfig) ||
                        string.IsNullOrWhiteSpace(lotNameAllowedCharactersConfig.GetConfigValue<string>()))
                    {
                        throw new Exception(amsOSRAMUtilities.GetLocalizedMessage(amsOSRAMConstants.LocalizedMessageConfigMissingValue,
                                                                                  amsOSRAMConstants.DefaultLotNameAllowedCharacters));
                    }

                    // Throw an exception case ProductionLine attribute has no associated value
                    if (!material.Product.HasRelatedAttribute(amsOSRAMConstants.ProductAttributeProductionLine, true))
                    {
                        throw new Exception(amsOSRAMUtilities.GetLocalizedMessage(amsOSRAMConstants.LocalizedMessageProductionLineAttributeWithoutValue,
                                                                                  material.Product.Name));
                    }

                    // Set alphanumeric allowed characters Config value to Context
                    ApplicationContext.CallContext.SetInformationContext("LotNameAllowedCharacters", lotNameAllowedCharactersConfig.GetConfigValue<string>());

                    // Set ProductLine Product attribute value
                    string productionLine = material.Product.GetRelatedAttributeValue(amsOSRAMConstants.ProductAttributeProductionLine) as string;

                    // Load Generic Table CustomProductionLineConversion
                    GenericTable customProdLineConversionGT = new GenericTable() { Name = amsOSRAMConstants.GenericTableCustomProductionLineConversion };
                    customProdLineConversionGT.Load();

                    // Based on ProductLine Product attribute get Site and Facility name from Generic Table
                    customProdLineConversionGT.LoadData(new Foundation.BusinessObjects.QueryObject.FilterCollection()
                    {
                        new Foundation.BusinessObjects.QueryObject.Filter()
                        {
                            Name = amsOSRAMConstants.GenericTableCustomProductionLineConversionProductionLineProperty,
                            Operator = FieldOperator.IsEqualTo,
                            LogicalOperator = LogicalOperator.Nothing,
                            Value = productionLine
                        }
                    });

                    // Throw an exception case GT has no data for specific ProductionLine value
                    if (!customProdLineConversionGT.HasData)
                    {
                        throw new Exception(amsOSRAMUtilities.GetLocalizedMessage(amsOSRAMConstants.LocalizedMessageGTWihtoutDataForSpecificProductionLine,
                                                                                  amsOSRAMConstants.GenericTableCustomProductionLineConversion,
                                                                                  productionLine));
                    }

                    // Convert Generic Table data to DataSet
                    DataSet prodLineConversionDataSet = NgpDataSet.ToDataSet(customProdLineConversionGT.Data);

                    // Set Site associated to ProductionLine attribute to Context 
                    ApplicationContext.CallContext.SetInformationContext("SiteName", prodLineConversionDataSet.Tables[0].Rows[0][amsOSRAMConstants.GenericTableCustomProductionLineConversionSiteProperty]);

                    // Set Facility associated to ProductionLine attribute to Context 
                    ApplicationContext.CallContext.SetInformationContext("FacilityName", prodLineConversionDataSet.Tables[0].Rows[0][amsOSRAMConstants.GenericTableCustomProductionLineConversionFacilityProperty]);

                    canExecute = true;
                }
            }

            return canExecute;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            // System
            UseReference("", "System.Data");
            UseReference("", "System.Text");

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");

            // Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.GenericTables");

            // Common
            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            // Load Name Generator
            NameGenerator customGenProdLotNamesNG = new NameGenerator() { Name = amsOSRAMConstants.CustomGenerateProductionLotNames };
            customGenProdLotNamesNG.Load();

            // Get Site name from Context
            string siteName = ApplicationContext.CallContext.GetInformationContext("SiteName") as string;

            // Get Facility name from Context
            string facilityName = ApplicationContext.CallContext.GetInformationContext("FacilityName") as string;

            // Get alphanumeric allowed characters from Context
            string lotNameAllowedCharacters = ApplicationContext.CallContext.GetInformationContext("LotNameAllowedCharacters") as string;

            // Get alphanumeric allowed characters size
            int allowedCharactersSize = lotNameAllowedCharacters.Length;

            // Set number of characters that will be generated
            int numberOfCharacters = 6;

            // Lot Name Generator builder
            StringBuilder generatedLotName = new StringBuilder();

            // Site and Facility name identifier
            generatedLotName.AppendFormat("{0}{1}", siteName.Substring(0, 1), facilityName.Substring(0, 1));

            // Context key
            string contextKey = generatedLotName.ToString();

            // Load Current Context associated to Name Generator
            customGenProdLotNamesNG.LoadGeneratorContexts(out int totalRows);
            IGeneratorContext contextNG = customGenProdLotNamesNG.Contexts.FirstOrDefault(ng => ng.Context == contextKey);

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
                int currLetterInt = lastCounterValue % allowedCharactersSize;
                alphanumericCounter += (char)lotNameAllowedCharacters[0 + currLetterInt];
                lastCounterValue /= allowedCharactersSize;
            }

            // Throw an exception case NG counter has insufficient number of digits
            if (lastCounterValue > 0)
            {
                throw new Exception(amsOSRAMUtilities.GetLocalizedMessage(amsOSRAMConstants.LocalizedMessageInsufficientDigitsForNameGenerator,
                                                                          amsOSRAMConstants.CustomGenerateProductionLotNames,
                                                                          nextCounterValue.ToString()));
            }

            // Revert the counter Chars order
            char[] counterChars = alphanumericCounter.ToCharArray();
            Array.Reverse(counterChars);
            alphanumericCounter = new string(counterChars);

            // 6-digits and Lot Counter Value identifier
            generatedLotName.AppendFormat("{0}00", alphanumericCounter);

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
            Input.Add("Result", generatedLotName.ToString().ToUpper());

            //---End DEE Code---

            return Input;
        }
    }
}
