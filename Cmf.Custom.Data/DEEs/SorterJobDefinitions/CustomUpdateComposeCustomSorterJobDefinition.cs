using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Foundation.Common.LocalizationService;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;

namespace Cmf.Custom.amsOSRAM.Actions.SorterJobDefinitions
{
    public class CustomUpdateComposeCustomSorterJobDefinition : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *     DEE responsible for updating the Compose CustomSorterJobDefinition based on BOMs.
             * Action Groups:
             *      BusinessObjects.BOM.MakeEffective.Pre
             *      BusinessObjects.BOM.MakeEffective.Post
            */

            #endregion Info

            bool canExecute = false;
            string actionGroup = DeeActionHelper.GetInputItem<string>(Input, "ActionGroupName");
            IServiceProvider serviceProvider = DeeActionHelper.GetInputItem<IServiceProvider>(Input, "ServiceProvider");
            IBOMCollection bomCollectionInput = DeeActionHelper.GetInputItem<IBOMCollection>(Input, "BOMCollection");

            if (bomCollectionInput != null)
            {
                if (actionGroup.EndsWith("BOM.MakeEffective.Pre"))
                {
                    IBOMCollection bomCollection = serviceProvider.GetService<IBOMCollection>();
                    bomCollection.AddRange(bomCollectionInput.Select(s =>
                    {
                        IBOM bom = serviceProvider.GetService<IBOM>();
                        bom.Name = s.Name;
                        return bom;
                    }));

                    bomCollection.Load();

                    ApplicationContext.CallContext.SetInformationContext("CustomBOMCollectionPre", bomCollection);
                }
                else if (actionGroup.EndsWith("BOM.MakeEffective.Post"))
                {
                    bomCollectionInput.LoadAttributes(new Collection<string> { amsOSRAMConstants.BOMAttributeIsForLotCompose, amsOSRAMConstants.BOMAttributeIsStartingCarrierType });

                    foreach (IBOM bom in bomCollectionInput)
                    {
                        bom.RelatedAttributes.TryGetValueAs(amsOSRAMConstants.BOMAttributeIsForLotCompose, out bool bomIsForLotCompose);
                        bom.RelatedAttributes.TryGetValueAs(amsOSRAMConstants.BOMAttributeIsStartingCarrierType, out string bomStartingCarrierType);

                        if (bomIsForLotCompose)
                        {
                            if (string.IsNullOrWhiteSpace(bomStartingCarrierType))
                            {
                                ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();

                                throw new CmfBaseException(
                                    string.Format(
                                        localizationService.Localize(
                                            Thread.CurrentThread.CurrentCulture.Name,
                                            amsOSRAMConstants.LocalizedMessageBomForLotComposeRequiresStartingCarrierType)
                                        , bom.Name));
                            }

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

            // Foundation
            UseReference("Cmf.Foundation.Common.dll", "Cmf.Foundation.Common.LocalizationService");

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.BusinessObjects.CustomSorterJobDefinition.dll", " Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.Extensions");
            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");

            // System
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");
            UseReference("", "System.Data");
            UseReference("", "System.Threading");

            IServiceProvider serviceProvider = DeeActionHelper.GetInputItem<IServiceProvider>(Input, "ServiceProvider");
            IBOMCollection boms = DeeActionHelper.GetInputItem<IBOMCollection>(Input, "BOMCollection");
            string newBOMName = "{0}.{1}.{2}";

            IBOMCollection bomCollectionPre = ApplicationContext.CallContext.GetInformationContext("CustomBOMCollectionPre") as IBOMCollection;
            List<string> customSorterJobDefinitionNames = bomCollectionPre.Select(b => String.Format(newBOMName, b.Name, b.Revision, b.Version)).ToList();

            #region Handle CustomSorterJobDefinitionContext SmartTable data

            ISmartTable customSorterJobDefinitionContextST = serviceProvider.GetService<ISmartTable>();
            customSorterJobDefinitionContextST.Name = amsOSRAMConstants.CustomSorterJobDefinitionContextName;
            customSorterJobDefinitionContextST.Load();

            IFilterCollection filters = serviceProvider.GetService<IFilterCollection>();
            IFilter filter = serviceProvider.GetService<IFilter>();
            filter.Name = "CustomSorterJobDefinition";
            filter.Operator = FieldOperator.In;
            filter.Value = customSorterJobDefinitionNames;
            filter.LogicalOperator = LogicalOperator.Nothing;
            filters.Add(filter);

            // Load SmartTable data with filters
            customSorterJobDefinitionContextST.LoadData(filters);

            DataSet customSorterJobDefinitionContextDataSet = customSorterJobDefinitionContextST.HasData
                ? NgpDataSet.ToDataSet(customSorterJobDefinitionContextST.Data)
                : null;

            bool customSorterJobDefinitionDataSetHasData =
                customSorterJobDefinitionContextDataSet != null && customSorterJobDefinitionContextDataSet.HasData();

            #endregion Handle CustomSorterJobDefinitionContext SmartTable data

            // Load BOMs current effective version (at this point it is outdated!)
            ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();

            foreach (IBOM bom in boms)
            {
                string newCustomSorterJobDefinitionName = String.Format(newBOMName, bom.Name, bom.Revision, bom.Version);

                // Validate if the CustomSorterJobDefinition already exists
                ICustomSorterJobDefinition customSorterJobDefinition = serviceProvider.GetService<ICustomSorterJobDefinition>();
                customSorterJobDefinition.Name = newCustomSorterJobDefinitionName;

                if (customSorterJobDefinition.ObjectExists())
                {
                    // Load CustomSorterJobDefinition
                    customSorterJobDefinition.Load();

                    if (customSorterJobDefinition.UniversalState == Foundation.Common.Base.UniversalState.Terminated)
                    {
                        ApplicationContext.CallContext.GetHistoryCollection().AddFeedbackMessage(new TransactionalFeedbackMessage()
                        {
                            Message = string.Format(
                                localizationService.Localize(
                                    Thread.CurrentThread.CurrentCulture.Name,
                                    amsOSRAMConstants.LocalizedMessageCustomSorterJobDefinitionTerminated),
                                newCustomSorterJobDefinitionName,
                                amsOSRAMConstants.CustomSorterJobDefinitionContextName),
                            MessageType = TransactionalFeedbackMessageType.Warning
                        });

                        continue;
                    }
                }
                else
                {
                    #region CustomSorterJobDefinition creation

                    // Execute DEE action CustomCreateComposeSorterJobDefinition to create a new CustomSorterJobDefinition
                    IAction actionCustomCreateComposeSorterJobDefinition = serviceProvider.GetService<IAction>();
                    actionCustomCreateComposeSorterJobDefinition.Load(amsOSRAMConstants.DEECustomCreateComposeSorterJobDefinition);

                    Dictionary<string, object> inputCreateComposeSorterJobDefinition = actionCustomCreateComposeSorterJobDefinition.ExecuteAction(new Dictionary<string, object>()
                    {
                        { "BOM", bom }
                    });

                    if (!inputCreateComposeSorterJobDefinition.ContainsKey("CustomSorterJobDefinition")
                        || inputCreateComposeSorterJobDefinition.GetValueOrDefault("CustomSorterJobDefinition") == null
                        || !(inputCreateComposeSorterJobDefinition.GetValueOrDefault("CustomSorterJobDefinition") is ICustomSorterJobDefinition newCustomSorterJobDefinition))
                    {
                        ApplicationContext.CallContext.GetHistoryCollection().AddFeedbackMessage(new TransactionalFeedbackMessage()
                        {
                            Message = string.Format(
                                localizationService.Localize(
                                    Thread.CurrentThread.CurrentCulture.Name,
                                    amsOSRAMConstants.LocalizedMessageCustomSorterJobDefinitionWasNotCreated),
                                newCustomSorterJobDefinitionName,
                                bom.Name),
                            MessageType = TransactionalFeedbackMessageType.Warning
                        });

                        continue;
                    }

                    customSorterJobDefinition = serviceProvider.GetService<ICustomSorterJobDefinition>(); ;
                    customSorterJobDefinition = newCustomSorterJobDefinition;

                    #endregion CustomSorterJobDefinition creation
                }

                #region Update CustomSorterJobDefinitionContext

                // If a CustomSorterJobDefinitionContext was found for the previous BOM version, we have to update it with the new version
                if (customSorterJobDefinitionDataSetHasData)
                {
                    foreach (DataRow dataRow in customSorterJobDefinitionContextDataSet.Tables[0].Rows)
                    {
                        dataRow[amsOSRAMConstants.CustomSorterJobDefinitionContextColumnCustomSorterJobDefinition] = customSorterJobDefinition.Name;
                    }

                    // Update CustomSorterJobDefinitionContextw with new CustomSorterJobDefinition
                    customSorterJobDefinitionContextST.InsertOrUpdateRows(NgpDataSet.FromDataSet(customSorterJobDefinitionContextDataSet));

                    TransactionalFeedbackMessage feedbackMessage = new TransactionalFeedbackMessage()
                    {
                        Message = string.Format(
                                localizationService.Localize(
                                    Thread.CurrentThread.CurrentCulture.Name,
                                    amsOSRAMConstants.LocalizedMessageCustomSorterJobDefinitionContextUpdated),
                                amsOSRAMConstants.CustomSorterJobDefinitionContextName,
                                customSorterJobDefinition.Name),
                        MessageType = TransactionalFeedbackMessageType.Warning
                    };

                    ApplicationContext.CallContext.GetHistoryCollection().AddFeedbackMessage(feedbackMessage);
                }
                else
                {
                    ApplicationContext.CallContext.GetHistoryCollection().AddFeedbackMessage(new TransactionalFeedbackMessage()
                    {
                        Message = string.Format(
                                localizationService.Localize(
                                    Thread.CurrentThread.CurrentCulture.Name,
                                    amsOSRAMConstants.LocalizedMessageCustomSorterJobDefinitionContextConfigurationNeeded),
                                customSorterJobDefinition.Name,
                                amsOSRAMConstants.CustomSorterJobDefinitionContextName),
                        MessageType = TransactionalFeedbackMessageType.Warning
                    });
                }

                #endregion Update CustomSorterJobDefinitionContext
            }

            //---End DEE Code---

            return Input;
        }
    }
}