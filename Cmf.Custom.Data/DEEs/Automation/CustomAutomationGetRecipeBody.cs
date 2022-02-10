using Cmf.Custom.AMSOsram.Actions;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cmf.Custom.AMSOsram.Actions.Automation
{
    class CustomAutomationGetRecipeBody : DeeDevBase
    {
        /// <summary>
        /// Dee test condition.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <returns></returns>
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *     Dee action is triggered by IoT Automation to adjust the state of a Load Port based on:
             *     - Load Port Order (Display Order of the SubResource) and Parent Resource
             *     - Or Load Port Resource Name            
             *  
             * Action Groups:
             *      None
             *     
            */

            #endregion

            return true;

            //---End DEE Condition Code---
        }

        /// <summary>
        /// Dee action code.
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
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");

            UseReference("", "Cmf.Foundation.BusinessObjects.Cultures");
            UseReference("", "System.Threading");


            if (!Input.ContainsKey("RecipeName"))
            {
                throw new ArgumentNullCmfException("RecipeName");
            }

            string recipeName = Input["RecipeName"].ToString();

            Recipe recipe = new Recipe()
            {
                Name = recipeName
            };

            if (!recipe.ObjectExists())
            {
                throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Recipe, recipe.Name);
            }

            recipe.Load();

            if (recipe.Body == null)
            {
                throw new CmfBaseException(string.Format(LocalizedMessage.GetLocalizedMessage(Thread.CurrentThread.CurrentCulture.Name, AMSOsramConstants.LocalizedMessageRecipeWithoutBody).MessageText, recipe.Name));
            }

            recipe.Body.Load();

            if (recipe.Body.Body == null || !recipe.Body.Body.Any())
            {
                throw new CmfBaseException(string.Format(LocalizedMessage.GetLocalizedMessage(Thread.CurrentThread.CurrentCulture.Name, AMSOsramConstants.LocalizedMessageRecipeBodyEmpty).MessageText, recipe.Name));
            }

            var base64String = Convert.ToBase64String(recipe.Body.Body);

            Input.Add("RecipeBody", base64String);

            Input.Add("RecipeNameOnEquipment", recipe.ResourceRecipeName);
            return Input;

            //---End DEE Code---

        }
    }
}
