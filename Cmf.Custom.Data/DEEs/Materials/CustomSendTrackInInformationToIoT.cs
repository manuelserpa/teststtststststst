using Cmf.Custom.AMSOsram.BusinessObjects;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Custom.AMSOsram.Common.Extensions;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Cmf.Custom.AMSOsram.Actions.Materials
{
	class CustomSendTrackInInformationToIoT : DeeDevBase
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
             *     Dee action to Trigger IoT call to TrackIn the Materials
             *     Rule must be the last to run on the [Orchestration] Complex Track In Materials Post Action Group.
             *     Rule must only execute if:
             *       The Resource Automation Mode is set to Online;
             *       The Material being Track In is the Top-Most (this rule must not execute/send request on sub-material tracking);
             *  
             * Action Groups:
             *      MaterialManagement.MaterialManagementOrchestration.ComplexTrackInMaterials.Post
             *     
            */

			#endregion
			bool canExecute = false;

			bool trackInForChildLot = (bool)(ApplicationContext.CallContext.GetInformationContext("TrackInForChildLot") ?? false);

			if (!trackInForChildLot && Input.ContainsKey("ComplexTrackInMaterialsOutput"))
			{
				ComplexTrackInMaterialsOutput inputObject = Input["ComplexTrackInMaterialsOutput"] as ComplexTrackInMaterialsOutput;

				if (inputObject != null)
				{
					if (inputObject.Resource != null && inputObject.Materials != null && inputObject.Materials.Count > 0)
					{
						// The Material being Track In is the Top - Most(this rule must not execute / send request on sub - material tracking);
						List<Material> materialsTotrackIn = inputObject.Materials.Where(m => m.ParentMaterial == null).ToList();
						if (materialsTotrackIn.Count > 0)
						{
							Resource resource = new Resource() { Name = inputObject.Resource.Name };
							resource.Load();

							// The Resource Automation Mode is set to Online;
							canExecute = (resource.AutomationMode == ResourceAutomationMode.Online);
							if (canExecute)
							{
								ApplicationContext.CallContext.SetInformationContext("MaterialsToTrackIn", materialsTotrackIn);
								ApplicationContext.CallContext.SetInformationContext("ResourceToTrackIn", resource);
							}
						}
					}
				}
			}

			return canExecute;

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
			UseReference("", "Cmf.Foundation.Common");
			UseReference("", "Cmf.Foundation.BusinessObjects.Cultures");
			// Navigo
			UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
			UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects");
			// Custom
			UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
			UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.DataStructures");
			UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.Extensions");
			UseReference("Cmf.Custom.AMSOsram.BusinessObjects.CustomSorterJobDefinition.dll", "Cmf.Custom.AMSOsram.BusinessObjects");
			// Other
			UseReference("", "System.Threading");

			List<MaterialData> materialDataToIot = new List<MaterialData>();
			List<Material> materialsTotrackIn = ApplicationContext.CallContext.GetInformationContext("MaterialsToTrackIn") as List<Material>;
			Resource resourceToTrackIn = ApplicationContext.CallContext.GetInformationContext("ResourceToTrackIn") as Resource;
			MaterialCollection materialsToSave = new MaterialCollection();

			#region Retrieve Resource Attributes (AllowDownloadRecipeAtTrackIn, IsSorter)

			resourceToTrackIn.LoadAttributes(new Collection<string> { AMSOsramConstants.ResourceAttributeAllowDownloadRecipeAtTrackIn, AMSOsramConstants.ResourceAttributeIsSorter });
			bool allowDownloadRecipeAtTrackIn = false;
			bool isSorter = false;

			if (resourceToTrackIn.Attributes != null)
			{
				if (resourceToTrackIn.Attributes.ContainsKey(AMSOsramConstants.ResourceAttributeAllowDownloadRecipeAtTrackIn) &&
			resourceToTrackIn.Attributes[AMSOsramConstants.ResourceAttributeAllowDownloadRecipeAtTrackIn] != null)
				{
					resourceToTrackIn.Attributes.TryGetValueAs(AMSOsramConstants.ResourceAttributeAllowDownloadRecipeAtTrackIn, out allowDownloadRecipeAtTrackIn);
				}
				if (resourceToTrackIn.Attributes.ContainsKey(AMSOsramConstants.ResourceAttributeIsSorter) &&
				resourceToTrackIn.Attributes[AMSOsramConstants.ResourceAttributeIsSorter] != null)
				{
					resourceToTrackIn.Attributes.TryGetValueAs(AMSOsramConstants.ResourceAttributeIsSorter, out isSorter);
				}
			}

			#endregion Retrieve Resource Attributes (AllowDownloadRecipeAtTrackIn, IsSorter)

			foreach (Material materialIn in materialsTotrackIn)
			{
				#region Materials

				MaterialData materialData = new MaterialData();
				Material material = new Material() { Name = materialIn.Name };
				material.Load();
				material.LoadChildren(1);
				material.LoadRelations(Navigo.Common.Constants.MaterialContainer);
				material.SubMaterials.LoadRelations(Navigo.Common.Constants.MaterialContainer);
				materialData.MaterialId = material.Id.ToString();
				materialData.MaterialName = material.Name;
				materialData.AllowDownloadRecipeAtTrackIn = allowDownloadRecipeAtTrackIn;

				if (isSorter)
				{
					string customSorterJobDefinitionName = ApplicationContext.CallContext.GetInformationContext("CustomSorterJobDefinitionName") as string;
					string customSorterJobDefinitionmovementList = ApplicationContext.CallContext.GetInformationContext("CustomSorterJobDefinitionMovementList") as string;

					CustomSorterJobDefinition customSorterJobDefinition = new CustomSorterJobDefinition();

					// It wasn't possible to resolve the Sorter Job Definition Context
					if (string.IsNullOrWhiteSpace(customSorterJobDefinitionName))
					{
						if (material != null && material.ParentMaterial == null)
						{
							bool canStartProcess = false;

							customSorterJobDefinition = AMSOsramUtilities.GetSorterJobDefinition(material);

							if (customSorterJobDefinition != null)
							{
								Container currentContainer = null;
								Resource currentLoadPort = null;
								BOM bom = null;
								string futureActionType = string.Empty;
								List<ResourceLoadPortData> containersDocked = null;

								if (customSorterJobDefinition.LogisticalProcess == AMSOsramConstants.LookupTableCustomSorterLogisticalProcessCompose)
								{
									bom = AMSOsramUtilities.ResolveBOMContext(material);

									if (bom is null)
									{
										throw new CmfBaseException($"No BOM was found for material ({material.Name}).");
									}
								}

								// Flag to check if container and load port can be retrieved for the main lot
								bool containerAndLoadPortFound = AMSOsramUtilities.RetrieveContainerAndLoadPortFromMaterial(material, ref currentContainer, ref currentLoadPort);

								// Check if it is possible to retrieve the container and the load port from a sub material of the current lot
								if (!containerAndLoadPortFound)
								{
									Material subMaterial = material.SubMaterials.FirstOrDefault();

									if (subMaterial != null)
									{
										subMaterial.Load();
										subMaterial.LoadRelations(Navigo.Common.Constants.MaterialContainer);
										containerAndLoadPortFound = AMSOsramUtilities.RetrieveContainerAndLoadPortFromMaterial(subMaterial, ref currentContainer, ref currentLoadPort);
									}
								}

								if (containerAndLoadPortFound)
								{
									Cmf.Foundation.Common.DynamicExecutionEngine.Action actionCustomMaterialInProcessSorterJobDefinition = new Foundation.Common.DynamicExecutionEngine.Action();
									actionCustomMaterialInProcessSorterJobDefinition.Load("CustomMaterialInProcessSorterJobDefinition");

									Dictionary<string, object> actionInputCustomMaterialInProcessSorterJobDefinition = new Dictionary<string, object>()
									{
										{ "Resource", resourceToTrackIn },
										{ "Container",  currentContainer },
										{ "LoadPort",  currentLoadPort },
										{ "CustomSorterJobDefinition",  customSorterJobDefinition },
										{ "BOM",  bom }
									};

									actionCustomMaterialInProcessSorterJobDefinition.ExecuteAction(actionInputCustomMaterialInProcessSorterJobDefinition);

									if (actionInputCustomMaterialInProcessSorterJobDefinition.ContainsKey("CanStartProcess") &&
										actionInputCustomMaterialInProcessSorterJobDefinition["CanStartProcess"] != null &&
										actionInputCustomMaterialInProcessSorterJobDefinition["CanStartProcess"] is string strCanStartProcess)
									{
										canStartProcess = bool.Parse(strCanStartProcess);
									}

									if (actionInputCustomMaterialInProcessSorterJobDefinition.ContainsKey("FutureActionType") &&
										actionInputCustomMaterialInProcessSorterJobDefinition["FutureActionType"] != null &&
										actionInputCustomMaterialInProcessSorterJobDefinition["FutureActionType"] is string strFutureActionType)
									{
										futureActionType = strFutureActionType;
									}

									if (actionInputCustomMaterialInProcessSorterJobDefinition.ContainsKey("ContainersDocked") &&
										actionInputCustomMaterialInProcessSorterJobDefinition["ContainersDocked"] != null &&
										actionInputCustomMaterialInProcessSorterJobDefinition["ContainersDocked"] is List<ResourceLoadPortData> tempContainersDocked)
									{
										containersDocked = tempContainersDocked;
									}

									if (material.SystemState == MaterialSystemState.InProcess &&
										isSorter &&
										customSorterJobDefinition != null && canStartProcess)
									{
										Cmf.Foundation.Common.DynamicExecutionEngine.Action actionCustomMaterialInPostProcessSorterJobDefinition = new Foundation.Common.DynamicExecutionEngine.Action();
										actionCustomMaterialInPostProcessSorterJobDefinition.Load("CustomMaterialInPostProcessSorterJobDefinition");

										Dictionary<string, object> actionInputCustomMaterialInPostProcessSorterJobDefinition = new Dictionary<string, object>()
										{
											{ "Resource", resourceToTrackIn },
											{ "CurrentContainerName",  currentContainer.Name },
											{ "CurrentMaterialName",  material.Name },
											{ "CustomSorterJobDefinition",  customSorterJobDefinition },
											{ "ContainersDocked", containersDocked },
											{ "CurrentLoadPort", currentLoadPort.Name },
											{ "FutureActionType", futureActionType }
										};

										actionCustomMaterialInPostProcessSorterJobDefinition.ExecuteAction(actionInputCustomMaterialInPostProcessSorterJobDefinition);
									}
								}

								if (!canStartProcess)
								{
									throw new CmfBaseException("Custom Sorter Job Definition conditions weren't met. The material cannot TrackIn.");
								}
							}
							else
							{
								throw new CmfBaseException("Cannot TrackIn because no Custom Sorter Job Definition context was found.");
							}
						}
					}
					else
					{
						customSorterJobDefinition.Load(customSorterJobDefinitionName);

						// Check if a sorter job definition was parsed into actual real MES data
						if (!string.IsNullOrWhiteSpace(customSorterJobDefinitionmovementList))
						{
							// Update the Sorter Job Definition movement list with the actual parsed data from MES
							customSorterJobDefinition.MovementList = customSorterJobDefinitionmovementList;
						}
					}

					materialData.SorterJobInformation = customSorterJobDefinition;
				}

				if (material.CurrentMainState != null && material.CurrentMainState.CurrentState != null)
				{
					material.CurrentMainState.StateModel.Load();
					var state = material.CurrentMainState.StateModel.States.First(s => s.Name == AMSOsramConstants.MaterialStateModelStateSetup);
					material.CurrentMainState = new CurrentEntityState(material, material.CurrentMainState.StateModel, state);
					materialsToSave.Add(material);

					materialData.MaterialState = material.CurrentMainState.CurrentState.Name;
				}
				else
				{
					StateModel customMaterialStateModel = new StateModel() { Name = AMSOsramConstants.MaterialStateModel };
					if (customMaterialStateModel.ObjectExists())
					{
						customMaterialStateModel.Load();
						var state = customMaterialStateModel.States.First(s => s.Name == AMSOsramConstants.MaterialStateModelStateSetup);
						material.CurrentMainState = new CurrentEntityState(material, customMaterialStateModel, state);
						materialsToSave.Add(material);
					}

					materialData.MaterialState = AMSOsramConstants.MaterialStateModelStateSetup;
				}

				if (material.SubMaterialCount > 0)
				{
					materialData.SubMaterials = new List<MaterialData>();
					foreach (var subMaterial in material.SubMaterials)
					{
						MaterialData subMaterialData = new MaterialData()
						{
							MaterialId = subMaterial.Id.ToString(),
							MaterialName = subMaterial.Name,
							MaterialState = subMaterial.SystemState.ToString()
						};

						if (subMaterial.MaterialContainer != null && subMaterial.MaterialContainer.Count > 0)
						{
							if (string.IsNullOrEmpty(materialData.ContainerName))
							{
								materialData.ContainerId = subMaterial.MaterialContainer.First().TargetEntity.Id.ToString();
								materialData.ContainerName = subMaterial.MaterialContainer.First().TargetEntity.Name;
							}
							subMaterialData.Slot = subMaterial.MaterialContainer.First().Position.ToString();
						}

						materialData.SubMaterials.Add(subMaterialData);
					}
				}

				if (string.IsNullOrEmpty(materialData.ContainerName))
				{
					string currentContainerName = ApplicationContext.CallContext.GetInformationContext("CurrentContainer") as string;

					if (!string.IsNullOrWhiteSpace(currentContainerName))
					{
						materialData.ContainerName = currentContainerName;
					}
				}

				if (!string.IsNullOrEmpty(materialData.ContainerName))
				{
					string currentLoadPort = ApplicationContext.CallContext.GetInformationContext("CurrentLoadPort") as string;

					Resource loadPort = new Resource();

					if (string.IsNullOrWhiteSpace(currentLoadPort))
					{
						Container container = new Container() { Name = materialData.ContainerName };
						container.Load();
						container.LoadRelations(Navigo.Common.Constants.ContainerResource);

						if (container.ContainerResourceRelations != null && container.ContainerResourceRelations.Count > 0)
						{
							loadPort = container.ContainerResourceRelations.First().TargetEntity;
						}
					}
					else
					{
						loadPort.Name = currentLoadPort;
						loadPort.Load();
					}

					materialData.LoadPortName = loadPort.Name;
					int loadPortPosition = loadPort.DisplayOrder ?? 0;
					materialData.LoadPortName = loadPort.Name;
					materialData.LoadPortPosition = loadPortPosition.ToString();
				}

				#endregion Materials

				#region Recipe

				if (material.CurrentRecipeInstance != null)
				{
					Recipe recipe = new Recipe() { Name = material.CurrentRecipeInstance.ParentEntity.Name };
					recipe.Load();

					Func<Recipe, RecipeInstance, RecipeData> RecursiveSubRecipeCollection = null;

					RecursiveSubRecipeCollection = new Func<Recipe, RecipeInstance, RecipeData>((recipeToSet, recipeInstance) =>
					{
						var recipeDataToReturn = new RecipeData()
						{
							RecipeId = recipeToSet.Id.ToString(),
							RecipeName = recipeToSet.Name,
							NameOnEquipment = recipeToSet.ResourceRecipeName,
							Checksum = recipeToSet.BodyChecksum,
						};

						recipeToSet.LoadSubRecipes();

						if (recipeToSet.SubRecipes != null && recipeToSet.SubRecipes.Count > 0)
						{
							recipeDataToReturn.SubRecipes = new List<RecipeData>();
							recipeInstance.LoadSubRecipeInstances();
							foreach (SubRecipe subRecipe in recipeToSet.SubRecipes)
							{
								RecipeInstance subRecipeInstance = null;
								if (recipeInstance.SubRecipeInstances != null)
								{
									subRecipeInstance = recipeInstance.SubRecipeInstances.FirstOrDefault(sr => sr.ParentEntity.Name == subRecipe.ChildRecipe.Name);
								}

								//subRecipe.ChildRecipe.Load();
								recipeDataToReturn.SubRecipes.Add(RecursiveSubRecipeCollection(subRecipe.ChildRecipe, subRecipeInstance));
							}
						}

						if (recipeInstance != null)
						{
							recipeInstance.LoadRelations(Navigo.Common.Constants.RecipeInstanceParameter);

							var recipeInstanceParameters = recipeInstance.RelationCollection.Where(r => r.Key == Cmf.Navigo.Common.Constants.RecipeInstanceParameter).ToList();
							if (recipeInstanceParameters != null && recipeInstanceParameters.Count > 0)
							{
								recipeDataToReturn.RecipeParameters = new List<RecipeParameterData>();
								foreach (var riP in recipeInstanceParameters)
								{
									var recipeInstanceValueCollection = riP.Value;

									foreach (var recipeInstanceValueRelation in recipeInstanceValueCollection)
									{
										var recipeInstanceValue = recipeInstanceValueRelation as RecipeInstanceParameter;

										var recipeInstanceParameterToSet = new RecipeParameterData
										{
											Name = recipeInstanceValue.TargetEntity.Name.ToString(),
											Value = recipeInstanceValue.Value.ToString()
										};

										recipeDataToReturn.RecipeParameters.Add(recipeInstanceParameterToSet);
									}
								}
							}
						}

						return recipeDataToReturn;
					});

					materialData.Recipe = RecursiveSubRecipeCollection(recipe, material.CurrentRecipeInstance);
				}

				#endregion Recipe

				materialDataToIot.Add(materialData);
			}

			// Save collection if CustomeMaterialStateModel will be set.
			if (materialsToSave.Count > 0)
			{
				materialsToSave.Save();
			}

			#region IoT call

			if (materialDataToIot.Count > 0)
			{
				// TODO: Later confirm that message was sent to IoT correctly (find a way to test it automatically)
				// Utilities.PublishMessage("AMSOsram.Test.SendTrackInInformationToIoT", new Dictionary<string, object>() { { "Data", materialDataToIot.ToJsonString() } });

				AutomationControllerInstance controllerInstance = resourceToTrackIn.GetAutomationControllerInstance();
				if (controllerInstance != null)
				{
					// Get EI default timeout
					//  --> /Cmf/Custom/Automation/TrackInTimeout
					int requestTimeout = AMSOsramUtilities.GetConfig<int>(AMSOsramConstants.AutomationTrackInTimeoutConfigurationPath);

					// Send Synchronous request to automation TrackIn the Material in the Equipment
					string requestType = AMSOsramConstants.AutomationRequestTypeTrackIn;
					var obj = controllerInstance.SendRequest(requestType, materialDataToIot.ToJsonString(), requestTimeout);

					if (obj == null)
					{
						throw new CmfBaseException(string.Format(LocalizedMessage.GetLocalizedMessage(Thread.CurrentThread.CurrentCulture.Name, AMSOsramConstants.LocalizedMessageIoTConnectionTimeout).MessageText, requestType));
					}
					else if (obj.ToString().Contains("Error"))
					{
						throw new CmfBaseException(obj.ToString());
					}
				}
			}

			#endregion IoT call


			//---End DEE Code---

			return Input;
		}
	}
}
