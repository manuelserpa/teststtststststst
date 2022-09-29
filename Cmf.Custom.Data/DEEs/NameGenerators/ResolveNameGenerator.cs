using Cmf.Custom.amsOSRAM.Common;
using Cmf.Foundation.BusinessOrchestration.ConnectIoTManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.ContainerManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MappingManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using System.Collections.Generic;
using System.Linq;
using Cmf.Navigo.BusinessObjects.Abstractions;
using System;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.BusinessObjects;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.Common;
using System.Data;
using Cmf.Navigo.BusinessOrchestration.MaterialLogisticsManagement.InputObjects;

namespace Cmf.Custom.amsOSRAM.Actions.NameGenerators
{
    public class ResolveNameGenerator : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---   

            #region Info
            /// <summary>
            /// Summary text
            ///     
            /// Action Groups:
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion

            return true;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            // System
            UseReference("%MicrosoftNetPath%Microsoft.CSharp.dll", "");
            UseReference("", "System.Data");

            // Foundation
            UseReference("", "Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects");
            UseReference("", "Cmf.Foundation.BusinessOrchestration.ConnectIoTManagement.InputObjects");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.SmartTables");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects");
            UseReference("", "Cmf.Navigo.BusinessOrchestration.ContainerManagement.InputObjects");
            UseReference("", "Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.InputObjects");
            UseReference("", "Cmf.Navigo.BusinessOrchestration.MappingManagement.InputObjects");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialLogisticsManagement.InputObjects");

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");

            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];

            // If it is to use the Name Generator Context, it resolves the Name and Revision generator 
            if (Input.ContainsKey("Operation") && Input.ContainsKey("Instance") &&
                Input["Instance"] != null && Input["Instance"] is IEntity)
            {
                Dictionary<string, object> Output = new Dictionary<string, object>();
                IEntity instance = Input["Instance"] as IEntity;
                string entityType = instance.EntityType.Name;
                bool isTypeAvailableOnEntityType = instance.EntityType.Properties.Any(t => t.Name.Equals(Constants.Type));
                string entityTypeType = isTypeAvailableOnEntityType ? instance.GetNativeValue<string>(Constants.Type) : string.Empty;
                string operation = Input["Operation"] as string;

                ISmartTable smartTable = serviceProvider.GetService<ISmartTable>();
                smartTable.Load("NameGeneratorContext");
                INgpDataRow row = new NgpDataRow();
                row.Add("EntityType", entityType);
                row.Add("EntityTypeType", entityTypeType);
                row.Add("Operation", operation);
                INgpDataSet resolveReturnData = smartTable.Resolve(row, true);

                DataSet ds = NgpDataSet.ToDataSet(resolveReturnData);
                if (ds?.Tables?.Count > 0 && ds.Tables[0].Rows?.Count > 0)
                {
                    string nameGenerator = ds.Tables[0].Rows[0]["NameGeneratorName"] as string ?? string.Empty;
                    string revisionGenerator = ds.Tables[0].Rows[0]["RevisionGeneratorName"] as string ?? string.Empty;
                    Output.Add("NameGeneratorName", nameGenerator);
                    Output.Add("RevisionGeneratorName", revisionGenerator);
                }

                return Output;
            }

            bool isFound = false;
            foreach (KeyValuePair<string, object> myKey in Input)
            {
                if (myKey.Value != null)
                {
                    switch (myKey.Key)
                    {
                        case "CreateMaterialInput":
                            if (myKey.Value != null && ((CreateMaterialInput)myKey.Value).Material != null)
                            {
                                if (string.IsNullOrWhiteSpace(((CreateMaterialInput)myKey.Value).Material.Name))
                                {
                                    ((CreateMaterialInput)myKey.Value).NameGeneratorName = amsOSRAMConstants.CustomGenerateProductionLotNames;
                                }
                                isFound = true;
                            }
                            break;
                        case "CreateMaterialsInput":
                            if (myKey.Value != null)
                            {
                                if (((CreateMaterialsInput)myKey.Value).Materials != null && ((CreateMaterialsInput)myKey.Value).Materials.Count > 0)
                                {
                                    foreach (IMaterial myMat in ((CreateMaterialsInput)myKey.Value).Materials)
                                    {
                                        if (myMat != null && string.IsNullOrWhiteSpace(myMat.Name))
                                        {
                                            ((CreateMaterialsInput)myKey.Value).NameGeneratorName = amsOSRAMConstants.CustomGenerateProductionLotNames;
                                            break;
                                        }
                                    }
                                    isFound = true;

                                }
                            }
                            break;

                        case "SplitMaterialInput":
                            if (myKey.Value != null)
                            {
                                if (((SplitMaterialInput)myKey.Value).ChildMaterials != null && ((SplitMaterialInput)myKey.Value).ChildMaterials.Count > 0)
                                {
                                    foreach (ISplitInputParameters myMat in ((SplitMaterialInput)myKey.Value).ChildMaterials)
                                    {
                                        if (myMat != null && string.IsNullOrWhiteSpace(myMat.Name))
                                        {
                                            ((SplitMaterialInput)myKey.Value).NameGeneratorName = amsOSRAMConstants.CustomGenerateSplitLotNames;
                                            break;
                                        }
                                    }
                                }
                                isFound = true;
                            }
                            break;
                        case "ExpandMaterialInput":
                            if (myKey.Value != null)
                            {
                                if (((ExpandMaterialInput)myKey.Value).SubMaterials != null && ((ExpandMaterialInput)myKey.Value).SubMaterials.Count > 0)
                                {
                                    foreach (IMaterial myMat in ((ExpandMaterialInput)myKey.Value).SubMaterials)
                                    {
                                        if (myMat != null && string.IsNullOrWhiteSpace(myMat.Name))
                                        {
                                            ((ExpandMaterialInput)myKey.Value).NameGeneratorName = "MaterialNameGenerator";
                                            break;
                                        }
                                    }
                                }
                                isFound = true;
                            }
                            break;
                        case "CreateMaterialSubProductsInput":
                            if (myKey.Value != null)
                            {
                                if (((CreateMaterialSubProductsInput)myKey.Value).MaterialSubProducts != null && ((CreateMaterialSubProductsInput)myKey.Value).MaterialSubProducts.Count > 0)
                                {
                                    foreach (IMaterial myMat in ((CreateMaterialSubProductsInput)myKey.Value).MaterialSubProducts)
                                    {
                                        if (myMat != null && string.IsNullOrWhiteSpace(myMat.Name))
                                        {
                                            ((CreateMaterialSubProductsInput)myKey.Value).NameGeneratorName = "MaterialNameGenerator";
                                            break;
                                        }
                                    }
                                }
                                isFound = true;
                            }
                            break;
                        case "GradeMaterialInput":
                            if (myKey.Value != null)
                            {
                                if (((GradeMaterialInput)myKey.Value).MaterialSubProducts != null && ((GradeMaterialInput)myKey.Value).MaterialSubProducts.Count > 0)
                                {
                                    foreach (IMaterial myMat in ((GradeMaterialInput)myKey.Value).MaterialSubProducts)
                                    {
                                        if (myMat != null && string.IsNullOrWhiteSpace(myMat.Name))
                                        {
                                            ((GradeMaterialInput)myKey.Value).NameGeneratorName = "MaterialNameGenerator";
                                            break;
                                        }
                                    }
                                }
                                isFound = true;
                            }
                            break;
                        case "SplitMaterialByProductInput":
                            if (myKey.Value != null)
                            {
                                if (((SplitMaterialByProductInput)myKey.Value).SplitByProductMaterials != null && ((SplitMaterialByProductInput)myKey.Value).SplitByProductMaterials.Count > 0)
                                {
                                    foreach (ISplitByProductMaterial myMat in ((SplitMaterialByProductInput)myKey.Value).SplitByProductMaterials)
                                    {
                                        if (myMat != null && string.IsNullOrWhiteSpace(myMat.Material.Name))
                                        {
                                            ((SplitMaterialByProductInput)myKey.Value).NameGeneratorName = "MaterialNameGenerator";
                                            break;
                                        }
                                    }
                                }
                                isFound = true;
                            }
                            break;
                        case "CreateContainerInput":
                            if (myKey.Value != null && ((CreateContainerInput)myKey.Value).Container != null)
                            {
                                if (string.IsNullOrWhiteSpace(((CreateContainerInput)myKey.Value).Container.Name))
                                {
                                    ((CreateContainerInput)myKey.Value).NameGeneratorName = "ContainerNameGenerator";
                                }
                                isFound = true;
                            }
                            break;

                        case "CreateContainersInput":
                            if (myKey.Value != null)
                            {
                                if (((CreateContainersInput)myKey.Value).Containers != null && ((CreateContainersInput)myKey.Value).Containers.Count > 0)
                                {
                                    foreach (IContainer myContainer in ((CreateContainersInput)myKey.Value).Containers)
                                    {
                                        if (myContainer != null && string.IsNullOrWhiteSpace(myContainer.Name))
                                        {
                                            ((CreateContainersInput)myKey.Value).NameGeneratorName = "ContainerNameGenerator";
                                            break;
                                        }
                                    }
                                    isFound = true;

                                }
                            }
                            break;
                        case "PerformImmediateDataCollectionInput":
                            if (myKey.Value != null && ((PerformImmediateDataCollectionInput)myKey.Value).DataCollectionInstance != null)
                            {
                                if (string.IsNullOrWhiteSpace(((PerformImmediateDataCollectionInput)myKey.Value).DataCollectionInstance.Name))
                                {
                                    if (((PerformImmediateDataCollectionInput)myKey.Value).DataCollectionInstance.DataCollection == null || ((PerformImmediateDataCollectionInput)myKey.Value).DataCollectionInstance.DataCollection.Id <= 0)
                                    {
                                        ((PerformImmediateDataCollectionInput)myKey.Value).NameGeneratorName = "AdhocDataCollectionInstanceNameGenerator";
                                    }
                                    else
                                    {
                                        ((PerformImmediateDataCollectionInput)myKey.Value).NameGeneratorName = "DataCollectionInstanceNameGenerator";
                                    }
                                }
                                isFound = true;
                            }
                            break;
                        case "OpenDataCollectionInstanceInput":
                            if (myKey.Value != null && ((OpenDataCollectionInstanceInput)myKey.Value).DataCollectionInstance != null)
                            {
                                if (string.IsNullOrWhiteSpace(((OpenDataCollectionInstanceInput)myKey.Value).DataCollectionInstance.Name))
                                {
                                    ((OpenDataCollectionInstanceInput)myKey.Value).NameGeneratorName = "DataCollectionInstanceNameGenerator";
                                }
                                isFound = true;
                            }
                            break;
                        case "CreateObjectInput":
                            if (myKey.Value != null && ((CreateObjectInput)myKey.Value).Object != null)
                            {
                                dynamic obj = ((CreateObjectInput)myKey.Value).Object;

                                if (string.IsNullOrWhiteSpace(obj.Name))
                                {
                                    string typeName = obj.ToString();
                                    if (typeName.LastIndexOf(".") > 0)
                                    {
                                        typeName = typeName.Substring(typeName.LastIndexOf(".") + 1);
                                    }
                                    ((CreateObjectInput)myKey.Value).NameGeneratorName = typeName + "NameGenerator";
                                }
                                isFound = true;
                            }
                            break;
                        case "CloneObjectInput":
                            if (myKey.Value != null && ((CloneObjectInput)myKey.Value).Targets != null)
                            {
                                foreach (dynamic obj in ((CloneObjectInput)myKey.Value).Targets)
                                {
                                    if (string.IsNullOrWhiteSpace(obj.Name))
                                    {
                                        string typeName = obj.ToString();

                                        if (typeName.LastIndexOf(".") > 0)
                                        {
                                            typeName = typeName.Substring(typeName.LastIndexOf(".") + 1);
                                        }
                                        ((CloneObjectInput)myKey.Value).NameGeneratorName = typeName + "NameGenerator";
                                    }
                                    isFound = true;
                                }
                            }
                            break;
                        case "CreateMapInput":
                            if (myKey.Value != null && ((CreateMapInput)myKey.Value).Map != null)
                            {
                                if (string.IsNullOrWhiteSpace(((CreateMapInput)myKey.Value).Map.Name))
                                {
                                    ((CreateMapInput)myKey.Value).NameGeneratorName = "MapNameGenerator";
                                }
                                isFound = true;
                            }
                            break;
                        case "FullUpdateAutomationDriverDefinitionInput":
                            if (myKey.Value != null && ((FullUpdateAutomationDriverDefinitionInput)myKey.Value).AutomationDriverDefinition != null)
                            {
                                if (((FullUpdateAutomationDriverDefinitionInput)myKey.Value).FullUpdateParameters != null &&
                                    ((FullUpdateAutomationDriverDefinitionInput)myKey.Value).FullUpdateParameters.EventPropertiesToAdd != null &&
                                    ((FullUpdateAutomationDriverDefinitionInput)myKey.Value).FullUpdateParameters.EventPropertiesToAdd.Any() &&
                                    string.IsNullOrWhiteSpace(((FullUpdateAutomationDriverDefinitionInput)myKey.Value).FullUpdateParameters.EventPropertiesToAdd[0].Name))
                                {
                                    ((FullUpdateAutomationDriverDefinitionInput)myKey.Value).NameGeneratorNameEventProperty = "AutomationEventPropertyNameGenerator";
                                }

                                isFound = true;
                            }
                            break;
                        case "FullUpdateAutomationControllerInput":
                            if (myKey.Value != null && ((FullUpdateAutomationControllerInput)myKey.Value).AutomationController != null)
                            {
                                if (((FullUpdateAutomationControllerInput)myKey.Value).FullUpdateParameters != null &&
                                    ((FullUpdateAutomationControllerInput)myKey.Value).FullUpdateParameters.ControllerDriverDefinitionsToAdd != null &&
                                    ((FullUpdateAutomationControllerInput)myKey.Value).FullUpdateParameters.ControllerDriverDefinitionsToAdd.Any() &&
                                    string.IsNullOrWhiteSpace(((FullUpdateAutomationControllerInput)myKey.Value).FullUpdateParameters.ControllerDriverDefinitionsToAdd[0].Name))
                                {
                                    ((FullUpdateAutomationControllerInput)myKey.Value).NameGeneratorNameAutomationControllerDriverDefinition = "AutomationControllerDriverDefinitionNameGenerator";
                                }

                                isFound = true;
                            }
                            break;
                        case "RegisterMaterialsInput":
                            if (myKey.Value != null)
                            {
                                if (((RegisterMaterialsInput)myKey.Value).Materials?.Count > 0)
                                {
                                    foreach (RegisterMaterialInput matInput in ((RegisterMaterialsInput)myKey.Value).Materials)
                                    {
                                        if (string.IsNullOrWhiteSpace(matInput?.Material?.Name))
                                        {
                                            ((RegisterMaterialsInput)myKey.Value).NameGeneratorName = "MaterialNameGenerator";
                                            break;
                                        }
                                    }
                                }
                                isFound = true;
                            }
                            break;
                        default:
                            break;
                    }
                }

                if (isFound)
                {
                    break;
                }
            }

            //---End DEE Code---

            return Input;
        }
    }
}
