using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cmf.Custom.AMSOsram.BusinessObjects;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Custom.AMSOsram.Orchestration.InputObjects;
using Cmf.Custom.AMSOsram.Orchestration.OutputObjects;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;

namespace Cmf.Custom.AMSOsram.Orchestration
{
    public static class AMSOsramOrchestration
    {
        private const string OBJECT_TYPE_NAME = "Cmf.Custom.AMSOsram.Orchestration.AMSOsramManagementOrchestration";
        
        private const string MATERIAL_IN = "MaterialIn";
        private const string MATERIAL_IN_INPUT = "MaterialInInput";
        private const string MATERIAL_IN_OUTPUT = "MaterialInOutput";
        private const string MATERIAL_OUT = "MaterialOut";
        private const string MATERIAL_OUT_INPUT = "MaterialOutInput";
        private const string MATERIAL_OUT_OUTPUT = "MaterialOutOutput";
        private const string DOCK_CONTAINER = "DockContainer";
        private const string DOCK_CONTAINER_INPUT = "DockContainerInput";
        private const string DOCK_CONTAINER_OUTPUT = "DockContainerOutput";
        private const string UNDOCK_CONTAINER = "UndockContainer";
        private const string UNDOCK_CONTAINER_INPUT = "UndockContainerInput";
        private const string UNDOCK_CONTAINER_OUTPUT = "UndockContainerOutput";
        private const string REQUEST_CONTAINER_MOVEMENT = "RequestContainerMovement";
        private const string REQUEST_CONTAINER_MOVEMENT_INPUT = "RequestContainerMovementInput";
        private const string REQUEST_CONTAINER_MOVEMENT_OUTPUT = "RequestContainerMovementOutput";
        private const string AUTOMATION_MANAGER_DOWNLOADER = "AutomationManagerDownloadArtifacts";
        private const string AUTOMATION_MANAGER_DOWNLOADER_INPUT = "CustomAutomationManagerDownloaderInput";
        private const string AUTOMATION_MANAGER_DOWNLOADER_OUTPUT = "CustomAutomationManagerDownloaderOutput";
        private const string GET_ALL_RECIPES_VERSION = "GetAllRecipeVersions";
        private const string GET_ALL_RECIPES_VERSION_INPUT = "GetAllRecipeVersionsInput";
        private const string GET_ALL_RECIPES_VERSION_OUTPUT = "GetAllRecipeVersionsOutput";
        private const string IMPORT_ERP_MESSAGE = "ImportERPMessage";
        private const string IMPORT_ERP_MESSAGE_INPUT = "ImportERPMessageInput";
        private const string IMPORT_ERP_MESSAGE_OUTPUT = "ImportERPMessageOutput";
        private const string SET_HOLD_FOR_PRODUCT_LINE = "SetHoldForProductLine";
        private const string SET_HOLD_FOR_PRODUCT_LINE_INPUT = "SetHoldForProductLineInput";
        private const string SET_HOLD_FOR_PRODUCT_LINE_OUTPUT = "SetHoldForProductLineOutput";
        private const string RELEASE_HOLD_FOR_PRODUCT_LINE = "ReleaseHoldForProductLine";
        private const string RELEASE_HOLD_FOR_PRODUCT_LINE_INPUT = "ReleaseHoldForProductLineInput";
        private const string RELEASE_HOLD_FOR_PRODUCT_LINE_OUTPUT = "ReleaseHoldForProductLineOutput";

        #region Material
        /// <summary>
        /// Performs the Material Track In in the Resource
        /// </summary>
        /// <param name="input"></param>
        /// <returns>MaterialIn Output object</returns>
        public static MaterialInOutput MaterialIn(MaterialInInput input)
        {
            MaterialInOutput output = null;
            Utilities.StartMethod(OBJECT_TYPE_NAME, MATERIAL_IN,
                new KeyValuePair<string, object>(MATERIAL_IN_INPUT, input));
            try
            {
                output = new MaterialInOutput();

                #region validation input
                Cmf.Foundation.Common.DynamicExecutionEngine.Action actionCustomMaterialInValidation = new Foundation.Common.DynamicExecutionEngine.Action();
                actionCustomMaterialInValidation.Load("CustomMaterialInValidation");

                Dictionary<string, object> customMaterialInValidationInput = new Dictionary<string, object>()
                {
                    { "MaterialInInput", input }
                };

                actionCustomMaterialInValidation.ExecuteAction(customMaterialInValidationInput);

                Resource resource = customMaterialInValidationInput["Resource"] as Resource;
                Material waferToTrackIn = customMaterialInValidationInput["Material"] as Material;
                bool.TryParse(customMaterialInValidationInput["IsSorter"] as string, out bool isSorter);
                #endregion

                // Tuple structure
                //	- Item 1: Material
                //	- Item 2: Load Port Name
                //	- Item 3: Container Name
                //	- Item 4: Map Container Needed
                List<(Material material, string loadPort, string container, bool mapContainerNeeded)> lotsAssociatedToContainersDockedOnResource = new List<(Material material, string loadPort, string container, bool mapContainerNeeded)>();

                if (waferToTrackIn != null)
                {
                    lotsAssociatedToContainersDockedOnResource.Add((waferToTrackIn, "", string.IsNullOrWhiteSpace(input.CarrierId) ? "" : input.CarrierId, false));
                }
                else
                {
                    #region Trying to fetch materials docked on load ports
                    Cmf.Foundation.Common.DynamicExecutionEngine.Action actionCustomMaterialInGetLotsFromDockedContainers = new Foundation.Common.DynamicExecutionEngine.Action();
                    actionCustomMaterialInGetLotsFromDockedContainers.Load("CustomMaterialInGetLotsFromDockedContainers");

                    Dictionary<string, object> actionInputCustomMaterialInGetLotsFromDockedContainers = new Dictionary<string, object>()
                    {
                        { "Resource", resource }
                    };

                    actionCustomMaterialInGetLotsFromDockedContainers.ExecuteAction(actionInputCustomMaterialInGetLotsFromDockedContainers);

                    lotsAssociatedToContainersDockedOnResource = actionInputCustomMaterialInGetLotsFromDockedContainers["LotsDockedOnSorters"] as List<(Material material, string loadPort, string container, bool mapContainerNeeded)>;

                    if (lotsAssociatedToContainersDockedOnResource.Count == 0)
                    {
                        throw new Exception($"No materials found to Track-In in the resource ({resource.Name}).");
                    }
                    #endregion
                }

                bool isToThrowException = false;
                string exceptionMessage = string.Empty;
                #region Execution
                foreach ((Material material, string loadPort, string container, bool mapContainerNeeded) lotDockedOnSorter in lotsAssociatedToContainersDockedOnResource)
                {
                    // lotDockedOnSorter.Item4 = FORCED MAP CONTAINER
                    // Because Forced Map Carrier can be executed at container level, without a lot associated
                    // This operation takes precedence over anything else
                    // Also, when we don't have a lot associated with a container but the forced map container is set to true
                    //		we are using a dummy lot to be able to handle this scenario and we cannot do a Load on a dummy material.
                    // Also, if it is a Forced Map Carrier we cannot perform a track-in we just want to read the wafers on the container.
                    if (isSorter && lotDockedOnSorter.mapContainerNeeded)
                    {
                        Cmf.Foundation.Common.DynamicExecutionEngine.Action actionCustomSendStartMapProccessToIoT = new Foundation.Common.DynamicExecutionEngine.Action();
                        actionCustomSendStartMapProccessToIoT.Load("CustomSendStartMapProccessToIoT");

                        Dictionary<string, object> actionInputCustomSendStartMapProccessToIoT = new Dictionary<string, object>()
                        {
                            { "ContainerName", lotDockedOnSorter.container },
                            { "ResourceName",  input.ResourceName}
                        };

                        actionCustomSendStartMapProccessToIoT.ExecuteAction(actionInputCustomSendStartMapProccessToIoT);

                        break;
                    }

                    Material materialToTrackIn = null;
                    if (lotDockedOnSorter.material != null)
                    {
                        materialToTrackIn = lotDockedOnSorter.material;
                        materialToTrackIn.Load();
                    }

                    if (materialToTrackIn != null && materialToTrackIn.SystemState != MaterialSystemState.InProcess)
                    {
                        // IS TOP MOST MATERIAL
                        if (materialToTrackIn.ParentMaterial == null && materialToTrackIn.Form == "Lot")
                        {
                            // Validate if a lot can start the process
                            bool canStartProcess = true;
                            CustomSorterJobDefinition customSorterJobDefinition = null;
                            string futureActionType = string.Empty;

                            // Containers docked will be updated by DEE Action 'CustomProcessSorterJobDefinition'
                            // Because it will be that information used to check if we can start a process or not
                            List<ResourceLoadPortData> containersDocked = null;

                            if (isSorter)
                            {
                                Resource currentLoadPort;
                                Container currentContainer;

                                // Validate current load port
                                currentLoadPort = new Resource { Name = lotDockedOnSorter.loadPort };
                                if (string.IsNullOrWhiteSpace(lotDockedOnSorter.loadPort) || !currentLoadPort.ObjectExists())
                                {
                                    throw new Exception($"There isn't a valid load port associated with the material ({materialToTrackIn.Name}).");
                                }
                                else
                                {
                                    currentLoadPort.Load();
                                }

                                // Validate current container
                                currentContainer = new Container { Name = lotDockedOnSorter.container };
                                if (string.IsNullOrWhiteSpace(lotDockedOnSorter.container) || !currentContainer.ObjectExists())
                                {
                                    throw new Exception($"There isn't a valid container associated with the material ({materialToTrackIn.Name}).");
                                }
                                else
                                {
                                    currentContainer.Load();
                                }

                                BOM bom = null;

                                #region SmartTable resolution for a valid custom sorter job
                                customSorterJobDefinition = AMSOsramUtilities.GetSorterJobDefinition(materialToTrackIn);
                                #endregion

                                if (customSorterJobDefinition != null)
                                {
                                    if (customSorterJobDefinition.LogisticalProcess == AMSOsramConstants.LookupTableCustomSorterLogisticalProcessCompose)
                                    {
                                        bom = AMSOsramUtilities.ResolveBOMContext(materialToTrackIn);

                                        if (bom is null)
                                        {
                                            isToThrowException = true;
                                            exceptionMessage = $"Cannot resolve a BOM context for the material: ({materialToTrackIn.Name}).{Environment.NewLine}";
                                            // Jump to the next foreach element
                                            continue;
                                        }
                                    }

                                    Cmf.Foundation.Common.DynamicExecutionEngine.Action actionCustomMaterialInProcessSorterJobDefinition = new Foundation.Common.DynamicExecutionEngine.Action();
                                    actionCustomMaterialInProcessSorterJobDefinition.Load("CustomMaterialInProcessSorterJobDefinition");

                                    Dictionary<string, object> actionInputCustomMaterialInProcessSorterJobDefinition = new Dictionary<string, object>()
                                    {
                                        { "Resource", resource },
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

                                    if (canStartProcess)
                                    {
                                        // Set the CustomSorterJobDefinition in the context for future usage
                                        ApplicationContext.CallContext.SetInformationContext("CustomSorterJobDefinitionName", customSorterJobDefinition.Name);
                                        ApplicationContext.CallContext.SetInformationContext("CustomSorterJobDefinitionMovementList", customSorterJobDefinition.MovementList);
                                    }
                                    else
                                    {
                                        isToThrowException = true;
                                        exceptionMessage = $"Cannot TrackIn material({materialToTrackIn.Name}) because conditions where not met for Custom Sorter Job Definition ({customSorterJobDefinition.Name}).{Environment.NewLine}";
                                    }
                                }
                                else
                                {
                                    canStartProcess = false;
                                    isToThrowException = true;
                                    exceptionMessage = $"Cannot TrackIn material({materialToTrackIn.Name}) because no Custom Sorter Job Definition context was found.{Environment.NewLine}";
                                }
                            }

                            if (canStartProcess)
                            {
                                #region Check Material dispatch status

                                if (materialToTrackIn.SystemState == MaterialSystemState.Queued)
                                {
                                    Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects.DispatchMaterialOutput dispatchOutput = MaterialManagementOrchestration.DispatchMaterial(new DispatchMaterialInput()
                                    {
                                        Material = materialToTrackIn,
                                        Resource = resource,
                                        MaterialResource = new MaterialResource()
                                        {
                                            SourceEntity = materialToTrackIn,
                                            TargetEntity = resource
                                        }
                                    });

                                    materialToTrackIn = dispatchOutput.Material;
                                    resource = dispatchOutput.Resource;
                                }

                                #endregion

                                #region TrackIn current Material

                                if (materialToTrackIn.SystemState == MaterialSystemState.Dispatched)
                                {
                                    Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects.GetDataForTrackInWizardOutput dataForTrackIn = MaterialManagementOrchestration.GetDataForTrackInWizard(new GetDataForTrackInWizardInput()
                                    {
                                        Material = materialToTrackIn,
                                        Resource = resource
                                    });

                                    Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects.ComplexTrackInMaterialInput complexTrackIn = new Navigo.BusinessOrchestration.MaterialManagement.InputObjects.ComplexTrackInMaterialInput()
                                    {
                                        Material = materialToTrackIn,
                                        Resource = resource,
                                        StateModel = resource?.CurrentMainState?.StateModel ?? null,
                                        StateModelTransition = dataForTrackIn.ResourcePossibleTransitions?.DefaultTransition ?? null,
                                        StateModelTransitionReason = dataForTrackIn.ResourcePossibleTransitions?.DefaultTransition?.ReasonDefaultValue ?? null
                                    };

                                    if (!string.IsNullOrWhiteSpace(lotDockedOnSorter.loadPort))
                                    {
                                        ApplicationContext.CallContext.SetInformationContext("CurrentLoadPort", lotDockedOnSorter.loadPort);
                                    }

                                    if (!string.IsNullOrWhiteSpace(lotDockedOnSorter.container))
                                    {
                                        ApplicationContext.CallContext.SetInformationContext("CurrentContainer", lotDockedOnSorter.container);
                                    }

                                    waferToTrackIn = Cmf.Navigo.BusinessOrchestration.MaterialManagement.MaterialManagementOrchestration.ComplexTrackInMaterial(complexTrackIn).Material;
                                    isToThrowException = false;
                                }

                                #endregion

                                // If it was possible to process a custom sorter job definition we will need to check
                                //		what are the load ports to update its state to inUse.
                                // Also, if it's a merge we also need to Track-In lots associated with the containers defined as source carriers
                                //		on the movement list.
                                if (waferToTrackIn.SystemState == MaterialSystemState.InProcess &&
                                    isSorter &&
                                    customSorterJobDefinition != null)
                                {
                                    Cmf.Foundation.Common.DynamicExecutionEngine.Action actionCustomMaterialInPostProcessSorterJobDefinition = new Foundation.Common.DynamicExecutionEngine.Action();
                                    actionCustomMaterialInPostProcessSorterJobDefinition.Load("CustomMaterialInPostProcessSorterJobDefinition");

                                    Dictionary<string, object> actionInputCustomMaterialInPostProcessSorterJobDefinition = new Dictionary<string, object>()
                                    {
                                        { "Resource", resource },
                                        { "CurrentContainerName",  lotDockedOnSorter.container},
                                        { "CurrentMaterialName",  waferToTrackIn.Name},
                                        { "CustomSorterJobDefinition",  customSorterJobDefinition},
                                        { "ContainersDocked", containersDocked},
                                        { "CurrentLoadPort", lotDockedOnSorter.loadPort},
                                        { "FutureActionType", futureActionType}
                                    };

                                    actionCustomMaterialInPostProcessSorterJobDefinition.ExecuteAction(actionInputCustomMaterialInPostProcessSorterJobDefinition);
                                }

                                break;
                            }
                        }
                        else if (materialToTrackIn.ParentMaterial != null)
                        {
                            List<EntityRelation> subResourceToTrackin;

                            resource.LoadRelations("SubResource");

                            Resource resourceToTrackIn = null;
                            int subResourceOrder = 0;

                            if (resource.RelationCollection.ContainsKey("SubResource") && resource.RelationCollection["SubResource"] != null)
                            {
                                //SubResourceOrder Starts in 1
                                if (input.SubResourceOrder != null)
                                {
                                    //SubResourceOrder Starts in 1
                                    subResourceOrder = input.SubResourceOrder.Value - 1;
                                }

                                subResourceToTrackin = resource.RelationCollection["SubResource"].Where(sbr => (sbr as SubResource).TargetEntity.ProcessingType == ProcessingType.Process).OrderBy(sbr => (sbr as SubResource).Order).ToList();

                                if (subResourceToTrackin.Count > 0)
                                {
                                    if (subResourceToTrackin.Count < subResourceOrder || subResourceOrder < 0)
                                    {
                                        throw new IndexOutOfRangeException("Sub Resource Order parameter is Out of Range");
                                    }

                                    resourceToTrackIn = (Resource)subResourceToTrackin[subResourceOrder].TargetEntity;
                                }
                            }

                            materialToTrackIn.TrackIn(resourceToTrackIn);
                        }

                        //materialToTrackIn.Load();
                    }
                }

                if (isToThrowException)
                {
                    throw new Exception(exceptionMessage);
                }


                output.Material = waferToTrackIn;

                #endregion

                Utilities.EndMethod(-1, -1,
                        new KeyValuePair<string, object>(MATERIAL_IN_INPUT, input),
                        new KeyValuePair<string, object>(MATERIAL_IN_OUTPUT, output));
            }
            catch (CmfBaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CmfBaseException(ex.Message, ex);
            }

            return output;
        }

        /// <summary>
        /// Performs the Material Track out from the Resource
        /// </summary>
        /// <param name="input"></param>
        /// <returns>MaterialOut Output object</returns>
        public static MaterialOutOutput MaterialOut(MaterialOutInput input)
        {
            MaterialOutOutput output = null;
            Utilities.StartMethod(OBJECT_TYPE_NAME, MATERIAL_OUT,
                new KeyValuePair<string, object>(MATERIAL_OUT_INPUT, input));
            try
            {
                bool canTrackOut = false;
                output = new MaterialOutOutput();
                #region validation input

                Cmf.Foundation.Common.Utilities.ValidateNullInput(input, new List<string> { "CustomSorterJobDefinition" });

                bool containerOnlyProcess = input.ContainerOnlyProcess;
                canTrackOut = !containerOnlyProcess;

                if (string.IsNullOrWhiteSpace(input.MaterialName) && string.IsNullOrEmpty(input.CarrierId))
                {
                    throw new MissingMandatoryFieldCmfException("MaterialName or CarrierId");
                }

                if (string.IsNullOrWhiteSpace(input.ResourceName))
                {
                    throw new MissingMandatoryFieldCmfException("ResourceName");
                }

                Resource resource = new Resource() { Name = input.ResourceName };

                if (!resource.ObjectExists())
                {
                    throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Resource, input.ResourceName);
                }

                #endregion

                #region Execution
                if (canTrackOut)
                {
                    Material lot = new Material();
                    if (!string.IsNullOrWhiteSpace(input.MaterialName))
                    {
                        lot.Name = input.MaterialName;

                        if (!lot.ObjectExists())
                        {
                            throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Material, input.MaterialName);
                        }

                        lot.Load();
                    }

                    #region Handle Sorter Job Definition for different scenarios (Split, Merge, Compose)
                    if (input.CustomSorterJobDefinition != null &&
                        input.CustomSorterJobDefinition.LogisticalProcess != AMSOsramConstants.LookupTableCustomSorterLogisticalProcessMapCarrier)
                    {
                        Cmf.Foundation.Common.DynamicExecutionEngine.Action actionCustomMaterialOutProcessSorterJobDefinition = new Foundation.Common.DynamicExecutionEngine.Action();
                        actionCustomMaterialOutProcessSorterJobDefinition.Load("CustomMaterialOutProcessSorterJobDefinition");

                        Dictionary<string, object> actionInputCustomMaterialOutProcessSorterJobDefinition = new Dictionary<string, object>()
                        {
                            { "Resource", resource },
                            { "Lot",  lot },
                            { "CustomSorterJobDefinition",  input.CustomSorterJobDefinition },
                        };

                        actionCustomMaterialOutProcessSorterJobDefinition.ExecuteAction(actionInputCustomMaterialOutProcessSorterJobDefinition);
                    }
                    #endregion

                    if (lot.UniversalState != Foundation.Common.Base.UniversalState.Terminated)
                    {
                        MaterialInInput materialInInput = new MaterialInInput()
                        {
                            MaterialName = input.MaterialName,
                            ResourceName = input.ResourceName,
                            CarrierId = input.CarrierId
                        };

                        Material material;

                        if (lot.SystemState == MaterialSystemState.InProcess)
                        {
                            material = lot;
                        }
                        else
                        {
                            material = MaterialIn(materialInInput).Material;
                        }

                        if (material.SystemState == MaterialSystemState.InProcess)
                        {
                            if (material.ParentMaterial == null)
                            {
                                // Load resource to get the Main State Model.
                                Resource currentTrackoutResource = new Resource() { Name = resource.Name };
                                currentTrackoutResource.Load();

                                Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects.GetDataForMultipleTrackOutAndMoveNextWizardOutput getDataForMultipleTrackOutAndMoveNextWizardOutput = MaterialManagementOrchestration.GetDataForMultipleTrackOutAndMoveNextWizard(new Navigo.BusinessOrchestration.MaterialManagement.InputObjects.GetDataForMultipleTrackOutAndMoveNextWizardInput()
                                {
                                    Materials = new MaterialCollection() { material }
                                });

                                Cmf.Navigo.BusinessOrchestration.MaterialManagement.MaterialManagementOrchestration.ComplexTrackOutAndMoveMaterialToNextStep(new Navigo.BusinessOrchestration.MaterialManagement.InputObjects.ComplexTrackOutAndMoveMaterialToNextStepInput()
                                {
                                    Material = material,
                                    StateModel = currentTrackoutResource?.CurrentMainState?.StateModel ?? null,
                                    StateModelTransition = getDataForMultipleTrackOutAndMoveNextWizardOutput.ResourcePossibleTransitions?.DefaultTransition ?? null,
                                    StateModelTransitionReason = getDataForMultipleTrackOutAndMoveNextWizardOutput.ResourcePossibleTransitions?.DefaultTransition?.ReasonDefaultValue ?? null
                                });
                            }
                            else
                            {
                                material.TrackOut();
                            }
                        }
                        else
                        {
                            throw new InvalidObjectStateCmfException(string.Format("Material {0}", material.Name), MaterialSystemState.InProcess.ToString());
                        }
                    }
                    else if (lot.UniversalState == Foundation.Common.Base.UniversalState.Terminated)
                    {
                        LocalSendTrackOutToIoT(input, resource, lot.Id.ToString());
                    }

                    // update output object
                    output.MaterialName = lot.Name;
                    output.ResourceName = resource.Name;
                }
                else
                {
                    if (containerOnlyProcess)
                    {
                        // Have to send message to trackout in order for the materialdata on the IoT persistency to be deleted
                        LocalSendTrackOutToIoT(input, resource);
                    }
                }

                #endregion

                Utilities.EndMethod(-1, -1,
                    new KeyValuePair<string, object>(MATERIAL_OUT_INPUT, input),
                    new KeyValuePair<string, object>(MATERIAL_OUT_OUTPUT, output));
            }
            catch (CmfBaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CmfBaseException(ex.Message, ex);
            }

            return output;
        }

        private static void LocalSendTrackOutToIoT(MaterialOutInput input, Resource resource, string materialId = null)
        {
            resource.Load();
            AutomationControllerInstance controllerInstance = resource.GetAutomationControllerInstance();
            List<MaterialData> materialDataToIot = new List<MaterialData>();
            MaterialData materialData = new MaterialData
            {
                ContainerName = input.CarrierId,
                MaterialId = string.IsNullOrWhiteSpace(materialId) ? input.MaterialName : materialId,
                MaterialName = input.MaterialName
            };
            materialDataToIot.Add(materialData);

            if (controllerInstance != null)
            {
                // Get EI default timeout
                //  --> /Cmf/Custom/Automation/GenericRequestTimeout
                int requestTimeout = AMSOsramUtilities.GetConfig<int>(AMSOsramConstants.AutomationGenericRequestTimeoutConfigurationPath);

                // Send Synchronous request to automation TrackOut the Material in the Equipment
                string requestType = AMSOsramConstants.AutomationRequestTypeTrackOut;
                var obj = controllerInstance.SendRequest(requestType, materialDataToIot.ToJsonString(), requestTimeout);

                if (obj == null)
                {
                    throw new CmfBaseException(string.Format(LocalizedMessage.GetLocalizedMessage(Thread.CurrentThread.CurrentCulture.Name, AMSOsramConstants.LocalizedMessageIoTConnectionTimeout).MessageText, requestType));
                }
            }
        }

        #endregion Material
    }
}
