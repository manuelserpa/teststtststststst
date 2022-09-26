using Cmf.Custom.amsOSRAM.Common;
using Cmf.Foundation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Foundation.Common.LocalizationService;

namespace Cmf.Custom.amsOSRAM.Actions.Automation
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

            // System
            UseReference("", "System.Threading");

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");

            // Foundation
            UseReference("", "Cmf.Foundation.Common.LocalizationService");

            if (!Input.ContainsKey("RecipeName"))
            {
                throw new ArgumentNullCmfException("RecipeName");
            }

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            string recipeName = Input["RecipeName"].ToString();

            IRecipe recipe = entityFactory.Create<IRecipe>();
            recipe.Name = recipeName;

            if (!recipe.ObjectExists())
            {
                throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Recipe, recipe.Name);
            }

            recipe.Load();

            ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();

            if (recipe.Body == null)
            {
                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageRecipeWithoutBody), recipe.Name));
            }

            recipe.Body.Load();

            if (recipe.Body.Body == null || !recipe.Body.Body.Any())
            {
                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageRecipeBodyEmpty), recipe.Name));
            }

            Input.Add("RecipeBody", Convert.ToBase64String(recipe.Body.Body));
            Input.Add("RecipeNameOnEquipment", recipe.ResourceRecipeName);

            //---End DEE Code---

            return Input;
        }
    }
}
