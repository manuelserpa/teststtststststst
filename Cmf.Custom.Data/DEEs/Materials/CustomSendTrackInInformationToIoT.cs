using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions;
using Cmf.Foundation.Common.LocalizationService;

namespace Cmf.Custom.amsOSRAM.Actions.Materials
{
	public class CustomSendTrackInInformationToIoT : DeeDevBase
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
						List<IMaterial> materialsTotrackIn = inputObject.Materials.Where(m => m.ParentMaterial == null).ToList();
						if (materialsTotrackIn.Count > 0)
						{
                            // Get services provider information
                            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
                            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

							IResource resource = entityFactory.Create<IResource>();
							resource.Name = inputObject.Resource.Name;
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

			// Foundation
            UseReference("Cmf.Foundation.Common.dll", "Cmf.Foundation.Common.LocalizationService");
            
			// Navigo
			UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects");
            
			// Custom
            UseReference("Cmf.Custom.amsOSRAM.BusinessObjects.CustomSorterJobDefinition.dll", "Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.DataStructures");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.Extensions");
            
			// Other
            UseReference("", "System.Threading");

            List<MaterialData> materialDataToIot = new List<MaterialData>();
			List<IMaterial> materialsTotrackIn = ApplicationContext.CallContext.GetInformationContext("MaterialsToTrackIn") as List<IMaterial>;
			IResource resourceToTrackIn = ApplicationContext.CallContext.GetInformationContext("ResourceToTrackIn") as IResource;

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            IMaterialCollection materialsToSave = entityFactory.CreateCollection<IMaterialCollection>();

			#region Retrieve Resource Attributes (AllowDownloadRecipeAtTrackIn, IsSorter)

			resourceToTrackIn.LoadAttributes(new Collection<string> { amsOSRAMConstants.ResourceAttributeAllowDownloadRecipeAtTrackIn, amsOSRAMConstants.ResourceAttributeIsSorter });
			bool allowDownloadRecipeAtTrackIn = false;
			bool isSorter = false;

			if (resourceToTrackIn.Attributes != null)
			{
				if (resourceToTrackIn.Attributes.ContainsKey(amsOSRAMConstants.ResourceAttributeAllowDownloadRecipeAtTrackIn) &&
			resourceToTrackIn.Attributes[amsOSRAMConstants.ResourceAttributeAllowDownloadRecipeAtTrackIn] != null)
				{
					resourceToTrackIn.Attributes.TryGetValueAs(amsOSRAMConstants.ResourceAttributeAllowDownloadRecipeAtTrackIn, out allowDownloadRecipeAtTrackIn);
				}
				if (resourceToTrackIn.Attributes.ContainsKey(amsOSRAMConstants.ResourceAttributeIsSorter) &&
				resourceToTrackIn.Attributes[amsOSRAMConstants.ResourceAttributeIsSorter] != null)
				{
					resourceToTrackIn.Attributes.TryGetValueAs(amsOSRAMConstants.ResourceAttributeIsSorter, out isSorter);
				}
			}

			#endregion Retrieve Resource Attributes (AllowDownloadRecipeAtTrackIn, IsSorter)

			foreach (IMaterial materialIn in materialsTotrackIn)
			{
				#region Materials

				MaterialData materialData = new MaterialData();
				IMaterial material = entityFactory.Create<IMaterial>();
				material.Name = materialIn.Name;

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

                    ICustomSorterJobDefinition customSorterJobDefinition = entityFactory.Create<ICustomSorterJobDefinition>();

					// It wasn't possible to resolve the Sorter Job Definition Context
					if (string.IsNullOrWhiteSpace(customSorterJobDefinitionName))
					{
						if (material != null && material.ParentMaterial == null)
						{
							bool canStartProcess = false;

							customSorterJobDefinition = amsOSRAMUtilities.GetSorterJobDefinition(material);

							if (customSorterJobDefinition != null)
							{
								IContainer currentContainer = null;
								IResource currentLoadPort = null;
								IBOM bom = null;
								string futureActionType = string.Empty;
								List<ResourceLoadPortData> containersDocked = null;

								// Flag to check if container and load port can be retrieved for the main lot
								bool containerAndLoadPortFound = amsOSRAMUtilities.RetrieveContainerAndLoadPortFromMaterial(material, ref currentContainer, ref currentLoadPort);

                                if (customSorterJobDefinition.LogisticalProcess == amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessCompose)
								{
									bom = amsOSRAMUtilities.ResolveBOMContext(material);

									if (bom is null)
									{
										throw new CmfBaseException($"No BOM was found for material ({material.Name}).");
									}

									if (!containerAndLoadPortFound)
									{
                                        List<ResourceLoadPortData> dockedContainersOnResourceLoadPorts = amsOSRAMUtilities.DockedContainersOnLoadPortsByParentResource(resourceToTrackIn);
                                        ResourceLoadPortData containerAtLoadPort = dockedContainersOnResourceLoadPorts
                                            .Where(w =>
												w.LoadPortInUse == false &&
												string.IsNullOrWhiteSpace(w.ContainerLotAttribute) &&
												string.IsNullOrWhiteSpace(w.ParentMaterialName) &&
												w.ContainerMapContainerNeededAttribute == false)
											.OrderBy(d => d.LoadPortModifiedOn).ThenBy(d => d.ContainerLotAttribute).FirstOrDefault();

										if (containerAtLoadPort != null)
										{
											currentContainer = entityFactory.Create<IContainer>();
											currentContainer.Name = containerAtLoadPort.ContainerName;
											currentContainer.Load();

                                            currentLoadPort = entityFactory.Create<IResource>();
                                            currentLoadPort.Name = containerAtLoadPort.LoadPortName;
                                            currentLoadPort.Load();

                                            containerAndLoadPortFound = true;
                                        }
                                    }
								}

								// Check if it is possible to retrieve the container and the load port from a sub material of the current lot
								if (!containerAndLoadPortFound)
								{
									IMaterial subMaterial = material.SubMaterials.FirstOrDefault();

									if (subMaterial != null)
									{
										subMaterial.Load();
										subMaterial.LoadRelations(Navigo.Common.Constants.MaterialContainer);
										containerAndLoadPortFound = amsOSRAMUtilities.RetrieveContainerAndLoadPortFromMaterial(subMaterial, ref currentContainer, ref currentLoadPort);
									}
								}

								if (containerAndLoadPortFound)
								{
									IAction actionCustomMaterialInProcessSorterJobDefinition = new Foundation.Common.DynamicExecutionEngine.Action();
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
										IAction actionCustomMaterialInPostProcessSorterJobDefinition = new Foundation.Common.DynamicExecutionEngine.Action();
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
					IStateModelState state = material.CurrentMainState.StateModel.States.First(s => s.Name == amsOSRAMConstants.MaterialStateModelStateSetup);

                    ICurrentEntityState currentEntityState = entityFactory.Create<ICurrentEntityState>();
                    currentEntityState.Entity = material;
                    currentEntityState.StateModel = material.CurrentMainState.StateModel;
                    currentEntityState.CurrentState = state;

                    material.CurrentMainState = currentEntityState;
					materialsToSave.Add(material);

					materialData.MaterialState = material.CurrentMainState.CurrentState.Name;
				}
				else
				{
					IStateModel customMaterialStateModel = new StateModel() { Name = amsOSRAMConstants.MaterialStateModel };
					if (customMaterialStateModel.ObjectExists())
					{
						customMaterialStateModel.Load();
						IStateModelState state = customMaterialStateModel.States.First(s => s.Name == amsOSRAMConstants.MaterialStateModelStateSetup);

                        ICurrentEntityState currentEntityState = entityFactory.Create<ICurrentEntityState>();
                        currentEntityState.Entity = material;
                        currentEntityState.StateModel = customMaterialStateModel;
                        currentEntityState.CurrentState = state;

                        material.CurrentMainState = currentEntityState;
						materialsToSave.Add(material);
					}

					materialData.MaterialState = amsOSRAMConstants.MaterialStateModelStateSetup;
				}

				if (material.SubMaterialCount > 0)
				{
					materialData.SubMaterials = new List<MaterialData>();
					foreach (IMaterial subMaterial in material.SubMaterials)
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

					IResource loadPort = entityFactory.Create<IResource>();

					if (string.IsNullOrWhiteSpace(currentLoadPort))
					{
						IContainer container = entityFactory.Create<IContainer>();
						container.Name = materialData.ContainerName;
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
					IRecipe recipe = entityFactory.Create<IRecipe>();
					recipe.Name = material.CurrentRecipeInstance.ParentEntity.Name;
					recipe.Load();

					Func<IRecipe, IRecipeInstance, RecipeData> RecursiveSubRecipeCollection = null;

					RecursiveSubRecipeCollection = new Func<IRecipe, IRecipeInstance, RecipeData>((recipeToSet, recipeInstance) =>
					{
						RecipeData recipeDataToReturn = new RecipeData()
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
							foreach (ISubRecipe subRecipe in recipeToSet.SubRecipes)
							{
								IRecipeInstance subRecipeInstance = null;
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

							List<KeyValuePair<string, IEntityRelationCollection<IEntityRelation>>> recipeInstanceParameters = recipeInstance.RelationCollection.Where(r => r.Key == Navigo.Common.Constants.RecipeInstanceParameter).ToList();
							
							if (recipeInstanceParameters != null && recipeInstanceParameters.Count > 0)
							{
								recipeDataToReturn.RecipeParameters = new List<RecipeParameterData>();
								
								foreach (KeyValuePair<string, IEntityRelationCollection<IEntityRelation>> riP in recipeInstanceParameters)
								{
									IEntityRelationCollection<IEntityRelation> recipeInstanceValueCollection = riP.Value;

									foreach (IEntityRelation recipeInstanceValueRelation in recipeInstanceValueCollection)
									{
										IRecipeInstanceParameter recipeInstanceValue = recipeInstanceValueRelation as IRecipeInstanceParameter;

										RecipeParameterData recipeInstanceParameterToSet = new RecipeParameterData
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

            // DO NOT DELETE: This is a hook for test purposes

            #region IoT call

            if (materialDataToIot.Count > 0)
			{
				// TODO: Later confirm that message was sent to IoT correctly (find a way to test it automatically)
				// Utilities.PublishMessage("amsOSRAM.Test.SendTrackInInformationToIoT", new Dictionary<string, object>() { { "Data", materialDataToIot.ToJsonString() } });

				IAutomationControllerInstance controllerInstance = resourceToTrackIn.GetAutomationControllerInstance();
				if (controllerInstance != null)
				{
					// Get EI default timeout
					//  --> /Cmf/Custom/Automation/TrackInTimeout
					int requestTimeout = amsOSRAMUtilities.GetConfig<int>(amsOSRAMConstants.AutomationTrackInTimeoutConfigurationPath);

					// Send Synchronous request to automation TrackIn the Material in the Equipment
					string requestType = amsOSRAMConstants.AutomationRequestTypeTrackIn;
					object obj = controllerInstance.SendRequest(requestType, materialDataToIot.ToJsonString(), requestTimeout);

					if (obj == null)
					{
                        ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();
                        throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageIoTConnectionTimeout), requestType));
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
