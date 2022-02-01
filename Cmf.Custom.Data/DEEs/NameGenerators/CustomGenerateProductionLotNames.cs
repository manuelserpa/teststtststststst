using System.Collections.Generic;
using System;
using System.Linq;
using Cmf.Foundation.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;

namespace Cmf.Custom.AMSOsram.Actions.NameGenerators
{
    public class CustomGenerateProductionLotNames : DeeDevBase
    {
        
        /// <summary>
        /// Dees the test condition.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <returns></returns>
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

            return true;

            //---End DEE Condition Code---
        }

        /// <summary>
        /// Dees the action code.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <returns></returns>
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("", "Cmf.Foundation.Common.Exceptions");
            UseReference("", "Cmf.Foundation.Common");
            //Please start code here
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");

            System.Text.StringBuilder newName = new System.Text.StringBuilder();

            NameGenerator ng = new NameGenerator();
            ng.Load("CustomProductionLotNameGenerator"); // Need to match the Name Generator name
            GeneratorContext existingContext = null;

            // Site
            newName.Append("T");

            // Fiscal Information
            Calendar c = new Calendar();
            c.Load("General");                          // Need to match the Calendar name
            var y = c.GetFiscalInformation(DateTime.Now);
            newName.Append(y.FiscalYear.ToString().Substring(2));
            newName.AppendFormat("{0:00}", y.FiscalWeek);

            // Running Number
            string contextToSearch = newName.ToString();

            string oldRunningNumberInt = "484848";  // Counter Value for '000'
            // Get Previous Counter Value
            {
                ng.LoadGeneratorContexts(out int totalRows);
                existingContext = ng.Contexts.FirstOrDefault(E => E.Context == contextToSearch);

            
                if (existingContext != null)
                {
                    oldRunningNumberInt = string.Format("{0:000000}", existingContext.LastCounterValue);
                }
            }

            // Convert Previous Counter Value to digits
            char firstDigit = (char)int.Parse(oldRunningNumberInt.Substring(0, oldRunningNumberInt.Length-4));
            char secondDigit = (char)int.Parse(oldRunningNumberInt.Substring(oldRunningNumberInt.Length-4, 2));
            char thirdDigit = (char)int.Parse(oldRunningNumberInt.Substring(oldRunningNumberInt.Length-2));
            string oldRunningNumber = string.Format("{0}{1}{2}",firstDigit , secondDigit , thirdDigit);

            // Alphanumeric need to exclude the following letters B,D,E,G,I,J,K,O,P,Q,S,V,W,Y,Z
            string charList = "0123456789ACFHLMNRTUX";

            // Calculate new counter Value
            bool addValue = true;

            for (int i = oldRunningNumber.Length - 1; i >= 0 && addValue; i--)
            {
                int position = charList.IndexOf(oldRunningNumber[i]);

                if (position != (charList.Length - 1))
                {
                    oldRunningNumber = oldRunningNumber.Remove(i, 1).Insert(i, charList[position + 1].ToString());
                    addValue = false;
                }
                else
                {
                    oldRunningNumber = oldRunningNumber.Remove(i, 1).Insert(i, charList[0].ToString());
                }
            }

            // Add new running number to the final name
            newName.Append(oldRunningNumber);

            // Convert the counter value to Integer
            string runningNumberInt = "";
            foreach (char cr in oldRunningNumber)
            {
                runningNumberInt += string.Format("{0:00}", (int)cr);
            }

            // Save the Context counter value
            if (existingContext != null)
            {
                existingContext.LastCounterValue = int.Parse(runningNumberInt);
                existingContext.Save();
            }
            else
            {
                ng.AddGeneratorContexts(new GeneratorContextCollection()
                {
                    new GeneratorContext
                    {
                        Context = contextToSearch,
                        LastCounterValue = int.Parse(runningNumberInt)
                    }
                });
            }

            Input.Add("Result", newName.ToString());
            //---End DEE Code---

            return Input;
          
        }

    }
}