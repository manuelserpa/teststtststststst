using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;

namespace Cmf.Custom.amsOSRAM.Actions.FutureActions
{
    class CustomGenerateSorterJobDefinitionFromFutureAction : DeeDevBase
	{
		/// <summary>
		/// DEE Test Condition.
		/// </summary>
		/// <param name="Input">The Input.</param>
		/// <returns></returns>
		public override bool DeeTestCondition(Dictionary<string, object> Input)
		{
			//---Start DEE Condition Code---

			#region Info

			/* Description:
             *		Dee action to Generate a Custom Sorter Job Definition if exists a Required Future Action
             *			for a given material.
             *		The Required Future Action must be of type Split or Merge.
             *		If it was possible to create a movement list the rule must terminate the future action
             *			and insert into the Custom Sorter Job Definition Context the generated Sorter Job.
             *  
             * Action Groups:
             *      MaterialManagement.MaterialManagementOrchestration.MoveMaterialsToNextStep.Post
             *      MaterialManagement.MaterialManagementOrchestration.ReleaseMaterial.Post
             *      MaterialManagement.MaterialManagementOrchestration.SpecialReleaseMaterial.Post
             *      MaterialManagement.MaterialManagementOrchestration.ChangeMaterialFlowAndStep.Post
             *     
            */

			#endregion

			Dictionary<IMaterial, IFutureAction> materialsWithFutureActions = new Dictionary<IMaterial, IFutureAction>();

            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

			IMaterialCollection materials = entityFactory.CreateCollection<IMaterialCollection>();

            if (Input.ContainsKey("MoveMaterialsToNextStepOutput") && Input["MoveMaterialsToNextStepOutput"] is MoveMaterialsToNextStepOutput moveMaterialsToNextStepOutput)
			{
				materials = moveMaterialsToNextStepOutput.Materials;
			}
			else if (Input.ContainsKey("ChangeMaterialFlowAndStepOutput") && Input["ChangeMaterialFlowAndStepOutput"] is ChangeMaterialFlowAndStepOutput changeMaterialFlowAndStepOutput)
			{
                materials.Add(changeMaterialFlowAndStepOutput.Material);
			}
			else if (Input.ContainsKey("ReleaseMaterialOutput") && Input["ReleaseMaterialOutput"] is ReleaseMaterialOutput releaseMaterialOutput)
			{
                materials.Add(releaseMaterialOutput.Material);
			}
			else if (Input.ContainsKey("SpecialReleaseMaterialOutput") && Input["SpecialReleaseMaterialOutput"] is SpecialReleaseMaterialOutput specialReleaseMaterialOutput)
			{
				materials.Add(specialReleaseMaterialOutput.Material);
			}

			if (materials != null && materials.Count > 0)
			{
				foreach (IMaterial material in materials)
				{
					if (material.RequiredFutureAction == null)
					{
						material.Load();
					}

					IFutureAction futureAction = material.RequiredFutureAction;

					if (futureAction != null && futureAction.Material != null && futureAction.Material.Name == material.Name && futureAction.ExecutionMode == FutureActionExecutionMode.Manual)
					{
						materialsWithFutureActions.Add(material, futureAction);
					}
				}
			}

			if (materialsWithFutureActions != null && materialsWithFutureActions.Count() > 0)
			{
				ApplicationContext.CallContext.SetInformationContext("MaterialsWithFutureActions", materialsWithFutureActions);
			}

			return materialsWithFutureActions.Count() > 0;

			//---End DEE Condition Code---
		}

		/// <summary>
		/// DEE Action Code.
		/// </summary>
		/// <param name="Input">The Input.</param>
		/// <returns></returns>
		public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
		{
            //---Start DEE Code---

			// Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.SmartTables");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.QueryObject");
            
			// Navigo
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects");
            
			// Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.BusinessObjects.CustomSorterJobDefinition.dll", "Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.Extensions");
            
			// System
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json");
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");
            UseReference("%MicrosoftNetPath%System.Data.Common.dll", "System.Data");

            Dictionary<IMaterial, IFutureAction> materialsWithFutureActions = ApplicationContext.CallContext.GetInformationContext("MaterialsWithFutureActions") as Dictionary<IMaterial, IFutureAction>;

            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            foreach (KeyValuePair<IMaterial, IFutureAction> materialWithFutureAction in materialsWithFutureActions)
			{
				IMaterial material = materialWithFutureAction.Key;
				IContainerCollection containersAssociatedWithLot = material.GetContainersForMaterialFamily();

				/* TO-DO: This exception needs to be handled.
				if (containersAssociatedWithLot == null || containersAssociatedWithLot.Count == 0)
				{
					throw new CmfBaseException($"The Material ({material.Name}) doesn't have a container associated!");
				}
				*/

				IFutureAction futureAction = materialWithFutureAction.Value;

				if (futureAction != null && futureAction.Material != null && futureAction.Material.Name == material.Name && futureAction.ExecutionMode == FutureActionExecutionMode.Manual)
				{
					// Check if there's already a sorter job definition defined for this material
					ICustomSorterJobDefinition existingSorterJob = amsOSRAMUtilities.GetSorterJobDefinition(material);

					/* TO-DO: This exception needs to be handled.
					if (existingSorterJob != null)
					{
						throw new CmfBaseException($"For the Material ({material.Name}) there's already a pending custom sorter job definition ({existingSorterJob.Name}).{Environment.NewLine}");
					}
					*/

					string smartTableName = amsOSRAMConstants.CustomSorterJobDefinitionContextName;
					JArray movementList = new JArray();
					string sourceCarrierType = string.Empty;
					string targetCarrierType = string.Empty;

					if (futureAction.Action == FutureActionType.Split)
					{
						IFutureActionSplitMaterialCollection splitMaterialsToGenerate = entityFactory.CreateCollection<IFutureActionSplitMaterialCollection>();

						// handle split materials
						futureAction.LoadSplitMaterials();
						splitMaterialsToGenerate.AddRange(futureAction.SplitMaterials);
						splitMaterialsToGenerate.LoadRelations(Navigo.Common.Constants.FutureActionSplitMaterialId);

						int containerIndex = 1;

						foreach (IFutureActionSplitMaterial fasm in splitMaterialsToGenerate)
						{
							if (fasm.RelationCollection != null &&
								fasm.RelationCollection.ContainsKey(Navigo.Common.Constants.FutureActionSplitMaterialId))
							{
								foreach (IFutureActionSplitMaterialId fasmid in fasm.RelationCollection[Navigo.Common.Constants.FutureActionSplitMaterialId].Cast<IFutureActionSplitMaterialId>())
								{
									IMaterial materialToSplit = fasmid.TargetEntity;
									materialToSplit.Load();
									materialToSplit.LoadRelations(Navigo.Common.Constants.MaterialContainer);

									// Can only create a sorter job movement if wafer is in a container
									if (materialToSplit.MaterialContainer != null && materialToSplit.MaterialContainer.Count() > 0)
									{
										IContainer container = materialToSplit.MaterialContainer.First().TargetEntity;
										container.Load();

										sourceCarrierType = container.Type;
										targetCarrierType = container.Type;

										JObject jObject = new JObject
										{
											[amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName] = materialToSplit.Name,
											[amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer] = container.Name,
											[amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition] = materialToSplit.MaterialContainer.First().Position,
											[amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer] = "#" + containerIndex,
											[amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition] = materialToSplit.MaterialContainer.First().Position
										};

										movementList.Add(jObject);
									}
								}
							}

							containerIndex++;
						}
					}
					else if (futureAction.Action == FutureActionType.Merge)
					{
						IContainer destinationContainer = containersAssociatedWithLot.FirstOrDefault();
						destinationContainer.Load();
						destinationContainer.LoadRelations(Navigo.Common.Constants.MaterialContainer);

						if (destinationContainer != null)
						{
							targetCarrierType = destinationContainer.Type;

							// fill queue free positions on the container
							Queue<int> freePositions = new Queue<int>();

							for (int i = 1; i <= (destinationContainer.TotalPositions ?? 25); i++)
							{
								if (destinationContainer.ContainerMaterials == null || !destinationContainer.ContainerMaterials.Any(m => m.Position == i))
								{
									freePositions.Enqueue(i);
								}
							}

							IFutureActionMergeMaterialCollection mergeMaterialsToGenerate = entityFactory.CreateCollection<IFutureActionMergeMaterialCollection>();

							// handle merge materials
							futureAction.LoadMergeMaterials();
							mergeMaterialsToGenerate.AddRange(futureAction.MergeMaterials);

							foreach (IFutureActionMergeMaterial famm in mergeMaterialsToGenerate)
							{
								IMaterial lotToMerge = famm.Material;
								lotToMerge.Load();
								DataSet dataSet = lotToMerge.GetChildrenWithContainersAsDataSet();

								if (dataSet.Tables != null && dataSet.Tables.Count > 0 &&
									dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
								{
									foreach (DataRow row in dataSet.Tables[0].Rows)
									{
										string containerName = string.Empty;
										string materialName = string.Empty;
										string containerType = string.Empty;
										int positionOnContainer = 0;

										if (row["Name"] != null && row["Name"] is string strMaterialName)
										{
											materialName = strMaterialName;
										}

										if (row["ContainerName"] != null && row["ContainerName"] is string strContainerName)
										{
											containerName = strContainerName;
										}

										if (row["ContainerType"] != null && row["ContainerType"] is string strContainerType)
										{
											containerType = strContainerType;
											sourceCarrierType = strContainerType;
										}

										if (row["Position"] != null && row["Position"] is int position)
										{
											positionOnContainer = position;
										}

										JObject jObject = new JObject
										{
											[amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName] = materialName,
											[amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer] = containerName,
											[amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition] = positionOnContainer,
											[amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer] = destinationContainer.Name,
											[amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition] = freePositions.Dequeue()
										};

										movementList.Add(jObject);
									}
								}
							}
						}
					}

					if (movementList.Count > 0)
					{
						// Create custom sorter job definition
						ICustomSorterJobDefinition customSorterJobDefinition = entityFactory.Create<ICustomSorterJobDefinition>();
						customSorterJobDefinition.Name = Guid.NewGuid().ToString();
						customSorterJobDefinition.SourceCarrierType = sourceCarrierType;
						customSorterJobDefinition.TargetCarrierType = targetCarrierType;
						customSorterJobDefinition.LogisticalProcess = amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessTransferWafers;
						customSorterJobDefinition.FlipWafer = false;
						customSorterJobDefinition.ReadWaferId = true;
						customSorterJobDefinition.WaferIdOnBottom = true;

						JObject mainObj = new JObject
						{
							[amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertyFutureActionType] = futureAction.Action.ToString(),
							[amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertyMoves] = movementList,
							[amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertyDeleteOnCompletion] = true
						};

						customSorterJobDefinition.MovementList = mainObj.ToString();

						customSorterJobDefinition.Create();
						customSorterJobDefinition.Load();

						// Add custom sorter job definitin to context table
						ISmartTable smartTable = new SmartTable() { Name = smartTableName };
						smartTable.Load();

						DataSet dataSet = smartTable.GetEmptyTableDataSet();

						DataRow customSorterJobDefinitionContextRow = dataSet.Tables[0].NewRow();
						customSorterJobDefinitionContextRow[amsOSRAMConstants.CustomSorterJobDefinitionContextColumnStep] = material.Step.Name;
						customSorterJobDefinitionContextRow[amsOSRAMConstants.CustomSorterJobDefinitionContextColumnProduct] = null;
						customSorterJobDefinitionContextRow[amsOSRAMConstants.CustomSorterJobDefinitionContextColumnProductGroup] = null;
						customSorterJobDefinitionContextRow[amsOSRAMConstants.CustomSorterJobDefinitionContextColumnFlow] = null;
						customSorterJobDefinitionContextRow[amsOSRAMConstants.CustomSorterJobDefinitionContextColumnMaterial] = material.Name;
						customSorterJobDefinitionContextRow[amsOSRAMConstants.CustomSorterJobDefinitionContextColumnMaterialType] = null;
						customSorterJobDefinitionContextRow[amsOSRAMConstants.CustomSorterJobDefinitionContextColumnCustomSorterJobDefinition] = customSorterJobDefinition.Name;

						dataSet.Tables[0].Rows.Add(customSorterJobDefinitionContextRow);
						dataSet.AcceptChanges();

						smartTable.InsertOrUpdateRows(NgpDataSet.FromDataSet(dataSet));

						futureAction.Terminate();
					}
				}
			}

			//---End DEE Code---

			return Input;
		}
	}
}
