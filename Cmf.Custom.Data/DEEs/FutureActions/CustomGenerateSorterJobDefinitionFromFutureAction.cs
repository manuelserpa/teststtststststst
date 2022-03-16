using Cmf.Custom.AMSOsram.BusinessObjects;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.Extensions;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Cmf.Custom.AMSOsram.Actions.FutureActions
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
			Dictionary<Material, FutureAction> materialsWithFutureActions = new Dictionary<Material, FutureAction>();
			MaterialCollection materials = null;

			if (Input.ContainsKey("MoveMaterialsToNextStepOutput") && Input["MoveMaterialsToNextStepOutput"] is MoveMaterialsToNextStepOutput moveMaterialsToNextStepOutput)
			{
				materials = moveMaterialsToNextStepOutput.Materials;
			}
			else if (Input.ContainsKey("ChangeMaterialFlowAndStepOutput") && Input["ChangeMaterialFlowAndStepOutput"] is ChangeMaterialFlowAndStepOutput changeMaterialFlowAndStepOutput)
			{
				materials = new MaterialCollection { changeMaterialFlowAndStepOutput.Material };
			}
			else if (Input.ContainsKey("ReleaseMaterialOutput") && Input["ReleaseMaterialOutput"] is ReleaseMaterialOutput releaseMaterialOutput)
			{
				materials = new MaterialCollection { releaseMaterialOutput.Material };
			}
			else if (Input.ContainsKey("SpecialReleaseMaterialOutput") && Input["SpecialReleaseMaterialOutput"] is SpecialReleaseMaterialOutput specialReleaseMaterialOutput)
			{
				materials = new MaterialCollection { specialReleaseMaterialOutput.Material };
			}

			if (materials != null && materials.Count > 0)
			{
				foreach (Material material in materials)
				{
					if (material.RequiredFutureAction == null)
					{
						material.Load();
					}

					FutureAction futureAction = material.RequiredFutureAction;

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
			UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
			UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
			UseReference("", "Cmf.Foundation.Common.Exceptions");
			UseReference("", "Cmf.Foundation.Common");
			UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.SmartTables");
			UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.QueryObject");
			// Navigo
			UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
			UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects");
			// Custom
			UseReference("Cmf.Custom.AMSOsram.BusinessObjects.CustomSorterJobDefinition.dll", "Cmf.Custom.AMSOsram.BusinessObjects");
			UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
			UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.Extensions");
			// System
			UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json");
			UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");
			UseReference("%MicrosoftNetPath%System.Data.Common.dll", "System.Data");


			Dictionary<Material, FutureAction> materialsWithFutureActions = ApplicationContext.CallContext.GetInformationContext("MaterialsWithFutureActions") as Dictionary<Material, FutureAction>;

			foreach (var materialWithFutureAction in materialsWithFutureActions)
			{
				Material material = materialWithFutureAction.Key;
				ContainerCollection containersAssociatedWithLot = material.GetContainersForMaterialFamily();

				/* TO-DO: This exception needs to be handled.
				if (containersAssociatedWithLot == null || containersAssociatedWithLot.Count == 0)
				{
					throw new CmfBaseException($"The Material ({material.Name}) doesn't have a container associated!");
				}
				*/

				FutureAction futureAction = materialWithFutureAction.Value;

				if (futureAction != null && futureAction.Material != null && futureAction.Material.Name == material.Name && futureAction.ExecutionMode == FutureActionExecutionMode.Manual)
				{
					// Check if there's already a sorter job definition defined for this material
					CustomSorterJobDefinition existingSorterJob = AMSOsramUtilities.GetSorterJobDefinition(material);

					/* TO-DO: This exception needs to be handled.
					if (existingSorterJob != null)
					{
						throw new CmfBaseException($"For the Material ({material.Name}) there's already a pending custom sorter job definition ({existingSorterJob.Name}).{Environment.NewLine}");
					}
					*/

					string smartTableName = AMSOsramConstants.CustomSorterJobDefinitionContextName;
					JArray movementList = new JArray();
					string sourceCarrierType = string.Empty;
					string targetCarrierType = string.Empty;

					if (futureAction.Action == FutureActionType.Split)
					{
						FutureActionSplitMaterialCollection splitMaterialsToGenerate = new FutureActionSplitMaterialCollection();

						// handle split materials
						futureAction.LoadSplitMaterials();
						splitMaterialsToGenerate.AddRange(futureAction.SplitMaterials);
						splitMaterialsToGenerate.LoadRelations(Cmf.Navigo.Common.Constants.FutureActionSplitMaterialId);

						int containerIndex = 1;

						foreach (FutureActionSplitMaterial fasm in splitMaterialsToGenerate)
						{
							if (fasm.RelationCollection != null &&
								fasm.RelationCollection.ContainsKey(Cmf.Navigo.Common.Constants.FutureActionSplitMaterialId))
							{
								foreach (FutureActionSplitMaterialId fasmid in fasm.RelationCollection[Cmf.Navigo.Common.Constants.FutureActionSplitMaterialId].Cast<FutureActionSplitMaterialId>())
								{
									Material materialToSplit = fasmid.TargetEntity;
									materialToSplit.Load();
									materialToSplit.LoadRelations(Navigo.Common.Constants.MaterialContainer);

									// Can only create a sorter job movement if wafer is in a container
									if (materialToSplit.MaterialContainer != null && materialToSplit.MaterialContainer.Count() > 0)
									{
										Container container = materialToSplit.MaterialContainer.First().TargetEntity;
										container.Load();

										sourceCarrierType = container.Type;
										targetCarrierType = container.Type;

										JObject jObject = new JObject
										{
											[AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName] = materialToSplit.Name,
											[AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer] = container.Name,
											[AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition] = materialToSplit.MaterialContainer.First().Position,
											[AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer] = "#" + containerIndex,
											[AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition] = materialToSplit.MaterialContainer.First().Position
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
						Container destinationContainer = containersAssociatedWithLot.FirstOrDefault();
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

							FutureActionMergeMaterialCollection mergeMaterialsToGenerate = new FutureActionMergeMaterialCollection();

							// handle merge materials
							futureAction.LoadMergeMaterials();
							mergeMaterialsToGenerate.AddRange(futureAction.MergeMaterials);

							foreach (FutureActionMergeMaterial famm in mergeMaterialsToGenerate)
							{
								Material lotToMerge = famm.Material;
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
											[AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName] = materialName,
											[AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer] = containerName,
											[AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition] = positionOnContainer,
											[AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer] = destinationContainer.Name,
											[AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition] = freePositions.Dequeue()
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
						CustomSorterJobDefinition customSorterJobDefinition = new CustomSorterJobDefinition
						{
							Name = Guid.NewGuid().ToString(),
							SourceCarrierType = sourceCarrierType,
							TargetCarrierType = targetCarrierType,
							LogisticalProcess = AMSOsramConstants.LookupTableCustomSorterLogisticalProcessTransferWafers,
							FlipWafer = false,
							ReadWaferId = true,
							WaferIdOnBottom = true
						};

						JObject mainObj = new JObject
						{
							[AMSOsramConstants.CustomSorterJobDefinitionJsonPropertyFutureActionType] = futureAction.Action.ToString(),
							[AMSOsramConstants.CustomSorterJobDefinitionJsonPropertyMoves] = movementList,
							[AMSOsramConstants.CustomSorterJobDefinitionJsonPropertyDeleteOnCompletion] = true
						};

						customSorterJobDefinition.MovementList = mainObj.ToString();

						customSorterJobDefinition.Create();
						customSorterJobDefinition.Load();

						// Add custom sorter job definitin to context table
						SmartTable smartTable = new SmartTable() { Name = smartTableName };
						smartTable.Load();

						DataSet dataSet = smartTable.GetEmptyTableDataSet();

						DataRow customSorterJobDefinitionContextRow = dataSet.Tables[0].NewRow();
						customSorterJobDefinitionContextRow[AMSOsramConstants.CustomSorterJobDefinitionContextColumnStep] = material.Step.Name;
						customSorterJobDefinitionContextRow[AMSOsramConstants.CustomSorterJobDefinitionContextColumnProduct] = null;
						customSorterJobDefinitionContextRow[AMSOsramConstants.CustomSorterJobDefinitionContextColumnProductGroup] = null;
						customSorterJobDefinitionContextRow[AMSOsramConstants.CustomSorterJobDefinitionContextColumnFlow] = null;
						customSorterJobDefinitionContextRow[AMSOsramConstants.CustomSorterJobDefinitionContextColumnMaterial] = material.Name;
						customSorterJobDefinitionContextRow[AMSOsramConstants.CustomSorterJobDefinitionContextColumnMaterialType] = null;
						customSorterJobDefinitionContextRow[AMSOsramConstants.CustomSorterJobDefinitionContextColumnCustomSorterJobDefinition] = customSorterJobDefinition.Name;

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
