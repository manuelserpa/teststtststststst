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


            return true;

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
            string alphanumericAllowedDigits = "0123456789";
            if (Config.TryGetConfig(AMSOsramConstants.DefaultProductionLotNameAlphanumericAllowedDigits, out Config prodLotNameAplhanumericAllowedDigitsConfig) &&
                        !string.IsNullOrWhiteSpace(prodLotNameAplhanumericAllowedDigitsConfig.GetConfigValue<string>()))
            {
                alphanumericAllowedDigits = prodLotNameAplhanumericAllowedDigitsConfig.GetConfigValue<string>();
            }
            int charListSize = alphanumericAllowedDigits.Length;

            // Load Current Context associated to Name Generator
            GeneratorContext contextNG = null;
            customGenSplitLotNamesNG.LoadGeneratorContexts(out int totalRows);
            contextNG = customGenSplitLotNamesNG.Contexts.FirstOrDefault(ng => ng.Context == parentMaterialNameSubstring);

            // Number of characters that will be generated
            int numberOfCharacters = 2;

            // Counter based on ASCII Table
            int counter = 0;

            if (contextNG != null)
            {
                // Get last counter value from NG Context
                counter = int.Parse(contextNG.LastCounterValue.ToString());
            }

            counter++;

            // Build list from start to end
            int nextCounter = counter;
            string result = string.Empty;
            for(int i = 0; i < numberOfCharacters; i++)
            {
                int currLetterInt = counter % charListSize;
                result += (char)alphanumericAllowedDigits[0 + currLetterInt];
                counter /= charListSize;
            }

            if (counter > 0)
            {
                // insufficient number of digits to represent the counter
                throw new ArgumentOutOfRangeCmfException("Name Generator Token Counter", nextCounter.ToString(), Math.Pow(charListSize, numberOfCharacters).ToString(), false);
            }

            // Revert the characters
            char[] charArray = result.ToCharArray();
            Array.Reverse(charArray);
            result = new string(charArray);

            // Save Generator context
            if (contextNG != null)
            {
                contextNG.LastCounterValue = Convert.ToInt32(nextCounter);
                contextNG.Save();
            }
            else
            {
                customGenSplitLotNamesNG.AddGeneratorContexts(new GeneratorContextCollection()
                {
                    new GeneratorContext()
                    {
                        Context = parentMaterialNameSubstring,
                        LastCounterValue = Convert.ToInt32(nextCounter)
                    }
                });
            }

            Input.Add("Result", parentMaterialNameSubstring + result);

            //---End DEE Code---

            return Input;
        }
    }
}
