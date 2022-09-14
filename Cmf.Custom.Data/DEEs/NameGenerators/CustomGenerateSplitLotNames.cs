using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmf.Custom.AMSOsram.Actions.NameGenerators
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
                Material material = Input["EntitySource"] as Material;
                if(material != null && material.Product != null && material.Form.Equals(AMSOsramConstants.MaterialLotForm, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Throw an exception case Configuration has no associated value
                    if (!Config.TryGetConfig(AMSOsramConstants.DefaultLotNameAllowedCharacters, out Config lotNameAllowedCharactersConfig) ||
                        string.IsNullOrWhiteSpace(lotNameAllowedCharactersConfig.GetConfigValue<string>()))
                    {
                        throw new Exception(AMSOsramUtilities.GetLocalizedMessage(AMSOsramConstants.LocalizedMessageConfigMissingValue,
                                                                                  AMSOsramConstants.DefaultLotNameAllowedCharacters));
                    }
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
            UseReference("", "System.Linq");
            UseReference("", "System.Collections.Generic");
            UseReference("", "System.Text");

            // Foundation
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("", "Cmf.Foundation.Common");

            // Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");

            // Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");

            // Load Name Generator
            NameGenerator customGenSplitLotNamesNG = new NameGenerator() { Name = AMSOsramConstants.CustomGenerateSplitLotNames };
            customGenSplitLotNamesNG.Load();

            Material materialLot = Input["EntitySource"] as Material;

            // Get the first 8 digits of the Parent Material
            string parentMaterialNameSubstring = materialLot.Name.Substring(0, 8);

            // Get alphanumeric allowed digits from Config
            string lotNameAllowedCharacters = null;
            if (Config.TryGetConfig(AMSOsramConstants.DefaultLotNameAllowedCharacters, out Config prodLotNameAplhanumericAllowedDigitsConfig) &&
                        !string.IsNullOrWhiteSpace(prodLotNameAplhanumericAllowedDigitsConfig.GetConfigValue<string>()))
            {
                lotNameAllowedCharacters = prodLotNameAplhanumericAllowedDigitsConfig.GetConfigValue<string>();
            }
            int allowedCharactersSize = lotNameAllowedCharacters.Length;

            // Load Current Context associated to Name Generator
            GeneratorContext contextNG = null;
            customGenSplitLotNamesNG.LoadGeneratorContexts(out int totalRows);
            contextNG = customGenSplitLotNamesNG.Contexts.FirstOrDefault(ng => ng.Context == parentMaterialNameSubstring);

            // Number of characters that will be generated
            int numberOfCharacters = 2;

            // Counter based on ASCII Table
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
                throw new Exception(AMSOsramUtilities.GetLocalizedMessage(AMSOsramConstants.LocalizedMessageInsufficientDigitsForNameGenerator,
                                                                          AMSOsramConstants.CustomGenerateProductionLotNames,
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
                customGenSplitLotNamesNG.AddGeneratorContexts(new GeneratorContextCollection()
                {
                    new GeneratorContext()
                    {
                        Context = parentMaterialNameSubstring,
                        LastCounterValue = nextCounterValue
                    }
                });
            }

            Input.Add("Result", parentMaterialNameSubstring + alphanumericCounter);

            //---End DEE Code---

            return Input;
        }
    }
}
