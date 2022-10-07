using Cmf.Custom.amsOSRAM.Common;
using System.Collections.Generic;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using System;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.Configuration.Abstractions;
using Cmf.Foundation.Configuration;
using Cmf.Foundation.Common;
using System.Linq;
using Cmf.Foundation.BusinessObjects;

namespace Cmf.Custom.amsOSRAM.Actions.NameGenerators
{
    public class CustomGenerateSplitLotNames : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---   

            #region Info
            /// <summary>
            /// Summary text
            ///     - DEE used by Name Generator CustomGenerateSplitLotNames to generate names for splited Materials
            /// Depends On:
            ///     - ResolveNameGenerator
            /// </summary>
            #endregion
            
            bool canExecute = false;

            if(Input != null && Input.ContainsKey("EntitySource"))
            {
                IMaterial material = Input["EntitySource"] as IMaterial;

                if(material != null && material.Product != null && material.Form.Equals(amsOSRAMConstants.MaterialLotForm, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Throw an exception case Configuration has no associated value
                    if (!Config.TryGetConfig(amsOSRAMConstants.DefaultLotNameAllowedCharacters, out IConfig lotNameAllowedCharactersConfig) ||
                        string.IsNullOrWhiteSpace(lotNameAllowedCharactersConfig.GetConfigValue<string>()))
                    {
                        throw new Exception(amsOSRAMUtilities.GetLocalizedMessage(amsOSRAMConstants.LocalizedMessageConfigMissingValue,
                                                                                  amsOSRAMConstants.DefaultLotNameAllowedCharacters));
                    }

                    ApplicationContext.CallContext.SetInformationContext("LotNameAllowedCharacters", lotNameAllowedCharactersConfig.GetConfigValue<string>());
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
            INameGenerator nameGenerator = entityFactory.Create<INameGenerator>();
            nameGenerator.Load(amsOSRAMConstants.CustomGenerateSplitLotNames);

            IMaterial materialLot = Input["EntitySource"] as IMaterial;

            // Get the first 8 digits of the Parent Material
            string parentMaterialNameSubstring = materialLot.Name.Substring(0, 8);

            // Get alphanumeric allowed digits from Config
            string lotNameAllowedCharacters = ApplicationContext.CallContext.GetInformationContext("LotNameAllowedCharacters") as string;
            int allowedCharactersSize = lotNameAllowedCharacters.Length;

            // Load Current Context associated to Name Generator
            nameGenerator.LoadGeneratorContexts(out int totalRows);
            IGeneratorContext contextNG = nameGenerator.Contexts.FirstOrDefault(ng => ng.Context == parentMaterialNameSubstring);

            // Number of characters that will be generated
            int numberOfCharacters = 2;

            int lastCounterValue = 0;

            if (contextNG != null)
            {
                // Get last counter value from NG Context
                lastCounterValue = contextNG.LastCounterValue;
            }

            lastCounterValue++;

            // Build list from start to end
            int nextCounterValue = lastCounterValue;
            string alphanumericCounter = string.Empty;
            for(int i = 0; i < numberOfCharacters; i++)
            {
                int currLetterInt = lastCounterValue % allowedCharactersSize;
                alphanumericCounter += (char)lotNameAllowedCharacters[0 + currLetterInt];
                lastCounterValue /= allowedCharactersSize;
            }

            if (lastCounterValue > 0)
            {
                // insufficient number of digits to represent the counter
                throw new Exception(amsOSRAMUtilities.GetLocalizedMessage(amsOSRAMConstants.LocalizedMessageInsufficientDigitsForNameGenerator,
                                                                          amsOSRAMConstants.CustomGenerateProductionLotNames,
                                                                          nextCounterValue.ToString()));
            }

            // Revert the characters
            char[] charArray = alphanumericCounter.ToCharArray();
            Array.Reverse(charArray);
            alphanumericCounter = new string(charArray);

            // Save Generator context
            if (contextNG != null)
            {
                contextNG.LastCounterValue = nextCounterValue;
                contextNG.Save();
            }
            else
            {
                nameGenerator.AddGeneratorContexts(new GeneratorContextCollection()
                {
                    new GeneratorContext()
                    {
                        Context = parentMaterialNameSubstring,
                        LastCounterValue = nextCounterValue
                    }
                });
            }

            Input.Add("Result", String.Format("{0}{1}", parentMaterialNameSubstring, alphanumericCounter));

            //---End DEE Code---

            return Input;
        }
    }
}
