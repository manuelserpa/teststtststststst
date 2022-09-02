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

                if(material != null && material.ParentMaterial != null)
                {
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
            NameGenerator customGenSplitLotNamesNG = new NameGenerator() { Name = AMSOsramConstants.CustomGenerateSplitLotNames};
            customGenSplitLotNamesNG.Load();

            Material materialLot = Input["EntitySource"] as Material;

            // Get the first 8 digits of the Parent Material
            string parentMaterialNameSubstring = materialLot.ParentMaterial.Name.Substring(0, 8);

            // Get alphanumeric allowed digits from Context
            string alphanumericAllowedDigits = ApplicationContext.CallContext.GetInformationContext("AlphanumericAllowedDigits") as string;

            // Lot Name Generator builder
            StringBuilder splitMaterialGeneratedName = new StringBuilder();
            splitMaterialGeneratedName.Append(parentMaterialNameSubstring);

            // Context Key
            string contextKey = parentMaterialNameSubstring;

            // Load Current Context associated to Name Generator
            GeneratorContext contextNG = null;
            customGenSplitLotNamesNG.LoadGeneratorContexts(out int totalRows);
            contextNG = customGenSplitLotNamesNG.Contexts.FirstOrDefault(ng => ng.Context == contextKey);

            // Counter based on ASCII Table
            string currentCounterValue = string.Empty;

            if (contextNG != null)
            {
                // Get last counter value from NG Context
                currentCounterValue = string.Format("{0:0000}", contextNG.LastCounterValue);
            }
            else
            {
                // - Int: 48 is equals to Char: 0
                // - Default: "48 48" => "0 0"
                currentCounterValue = "4848";
            }

            // Get Char value from the Substring Decimal value
            char firstDigit = Convert.ToChar(Convert.ToInt32(currentCounterValue.Substring(0, 2)));
            char secondDigit = Convert.ToChar(Convert.ToInt32(currentCounterValue.Substring(2, 2)));

            string newCounterValue = $"{firstDigit}{secondDigit}";

            bool addValue = true;

            for(int i = newCounterValue.Length - 1; i >= 0 && addValue; i--)
            {
                int position = alphanumericAllowedDigits.IndexOf(newCounterValue[i]);

                if(position != (alphanumericAllowedDigits.Length - 1))
                {
                    newCounterValue = newCounterValue.Remove(i, 1).Insert(i, alphanumericAllowedDigits[position + 1].ToString());
                    addValue = false;
                } else
                {
                    newCounterValue = newCounterValue.Remove(i, 1).Insert(i, newCounterValue[0].ToString());
                }
            }

            splitMaterialGeneratedName.Append(newCounterValue);

            // Convert the current number to Integer
            string lastCounterValue = null;

            foreach(char value in newCounterValue)
            {
                lastCounterValue += string.Format("{0:00", Convert.ToInt32(value));
            }

            // Save Generator context
            if(contextNG != null)
            {
                contextNG.LastCounterValue = Convert.ToInt32(lastCounterValue);
                contextNG.Save();
            } 
            else
            {
                customGenSplitLotNamesNG.AddGeneratorContexts(new GeneratorContextCollection()
                {
                    new GeneratorContext()
                    {
                        Context = parentMaterialNameSubstring,
                        LastCounterValue = Convert.ToInt32(lastCounterValue)
                    }
                });
            }

            Input.Add("Result", splitMaterialGeneratedName.ToString());

            //---End DEE Code---

            return Input;
        }
    }
}
