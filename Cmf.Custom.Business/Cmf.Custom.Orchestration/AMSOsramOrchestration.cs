using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.BusinessObjects;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Custom.AMSOsram.Orchestration.InputObjects;
using Cmf.Custom.AMSOsram.Orchestration.OutputObjects;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Integration;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using static Cmf.Custom.AMSOsram.Common.DataStructures.CustomFlowInformationToERPData;

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
        private const string GET_FLOW_INFORMATION_FOR_ERP = "GetFlowInformationForERP";
        private const string GET_FLOW_INFORMATION_FOR_ERP_INPUT = "CustomGetFlowInformationForERPInput";
        private const string GET_FLOW_INFORMATION_FOR_ERP_OUTPUT = "CustomGetFlowInformationForERPOutput";

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

        /// <summary>
        /// Sends the TrackOut message to IoT
        /// </summary>
        /// <param name="input"></param>
        /// <param name="resource"></param>
        /// <param name="materialId"></param>
        private static void LocalSendTrackOutToIoT(MaterialOutInput input, Resource resource, string materialId = null)
        {
            resource.Load();
            AutomationControllerInstance controllerInstance = resource.GetAutomationControllerInstance();
            List<Common.DataStructures.MaterialData> materialDataToIot = new List<Common.DataStructures.MaterialData>();
            Common.DataStructures.MaterialData materialData = new Common.DataStructures.MaterialData
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

        /// <summary>
        /// Service to generate an Integration Entry based on the received Stibo message
        /// </summary>
        /// <param name="input">Input Object</param>
        /// <returns>Output Object</returns>
        /// <exception cref="Cmf.Foundation.Common.CmfBaseException">If any unexpected error occurs.</exception>
        public static CustomReceiveStiboMessageOutput CustomReceiveStiboMessage(CustomReceiveStiboMessageInput input)
        {
            Utilities.StartMethod(
                OBJECT_TYPE_NAME,
                "CustomReceiveStiboMessage",
                new KeyValuePair<string, object>("CustomReceiveStiboMessageInput", input));

            CustomReceiveStiboMessageOutput output = new CustomReceiveStiboMessageOutput();

            try
            {
                Utilities.ValidateNullInput(input);

                if (!string.IsNullOrEmpty(input.Message))
                {
                    // Set Integration Entry properties
                    IntegrationEntry integrationEntry = new IntegrationEntry
                    {
                        Name = Guid.NewGuid().ToString("N"),
                        EventName = AMSOsramConstants.CustomIntegrationInboundEventName,
                        SourceSystem = AMSOsramConstants.CustomStiboSystem,
                        TargetSystem = Constants.MesSystemDesignation,
                        MessageType = input.MessageType,
                        IntegrationMessage = new IntegrationMessage
                        {
                            Message = Encoding.Default.GetBytes(input.Message)
                        },
                        SystemState = IntegrationEntrySystemState.Received,
                        MessageDate = DateTime.Now
                    };

                    // Create Integration Entry and Set to Result
                    output.Result = GenericServiceManagementOrchestration.CreateObject(
                        createObjectInput: new CreateObjectInput
                        {
                            Object = integrationEntry
                        }).Object as IntegrationEntry;
                }
                else
                {
                    throw new Exception(LocalizedMessage.GetLocalizedMessage(messageName: AMSOsramConstants.CustomReceiveEmptyMessage).MessageText);
                }

                Utilities.EndMethod(
                    -1,
                    -1,
                    new KeyValuePair<string, object>("CustomReceiveStiboMessageInput", input),
                    new KeyValuePair<string, object>("CustomReceiveStiboMessageOutput", output));
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
        /// Service to generate an Integration Entry based on the received message
        /// </summary>
        /// <param name="input">CustomReceiveERPMessage Input</param>
        /// <returns>CustomReceiveERPMessage Output</returns>
        /// <exception cref="Cmf.Foundation.Common.CmfBaseException">If any unexpected error occurs.</exception>
        public static CustomReceiveERPMessageOutput CustomReceiveERPMessage(CustomReceiveERPMessageInput input)
        {

            Utilities.StartMethod(
                OBJECT_TYPE_NAME,
                "CustomReceiveERPMessage",
                new KeyValuePair<string, object>("CustomReceiveERPMessageInput", input));

            CustomReceiveERPMessageOutput output = new CustomReceiveERPMessageOutput();

            try
            {
                Utilities.ValidateNullInput(input);

                if (!string.IsNullOrEmpty(input.Message))
                {
                    // create Integration Entry
                    IntegrationEntry integrationEntry = new IntegrationEntry
                    {
                        Name = Guid.NewGuid().ToString("N"),
                        EventName = AMSOsramConstants.CustomIntegrationInboundEventName,
                        SourceSystem = AMSOsramConstants.CustomERPSystem,
                        TargetSystem = Constants.MesSystemDesignation,
                        MessageType = input.MessageType,
                        IntegrationMessage = new IntegrationMessage
                        {
                            Message = Encoding.Default.GetBytes(input.Message)
                        },
                        SystemState = IntegrationEntrySystemState.Received,
                        MessageDate = DateTime.Now
                    };

                    // update output
                    output.Result = GenericServiceManagementOrchestration.CreateObject(new CreateObjectInput
                    {
                        Object = integrationEntry
                    }).Object as IntegrationEntry;

                }
                else
                {
                    throw new Exception(LocalizedMessage.GetLocalizedMessage(AMSOsramConstants.CustomReceiveEmptyMessage).MessageText);
                }

                Utilities.EndMethod(
                    -1,
                    -1,
                    new KeyValuePair<string, object>("CustomReceiveERPMessageInput", input),
                    new KeyValuePair<string, object>("CustomReceiveERPMessageOutput", output));
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
        /// Service to provide flow information to ERP
        /// </summary>
        /// <param name="CustomGetFlowInformationForERPInput">Input Object</param>
        /// <returns></returns>
        public static CustomGetFlowInformationForERPOutput GetFlowInformationForERP(CustomGetFlowInformationForERPInput input)
        {
            Utilities.StartMethod(OBJECT_TYPE_NAME, GET_FLOW_INFORMATION_FOR_ERP,
                                  new KeyValuePair<string, object>(GET_FLOW_INFORMATION_FOR_ERP_INPUT, input));

            CustomGetFlowInformationForERPOutput output = new CustomGetFlowInformationForERPOutput();

            try
            {
                Utilities.ValidateNullInput(input);

                if (!string.IsNullOrWhiteSpace(input.ProductName) && !string.IsNullOrWhiteSpace(input.FlowName))
                {
                    throw new Exception(LocalizedMessage.GetLocalizedMessage(AMSOsramConstants.LocalizedMessageProductNameAndFlowNameAtSameTime).MessageText);
                }

                if (string.IsNullOrWhiteSpace(input.ProductName) && string.IsNullOrWhiteSpace(input.FlowName))
                {
                    throw new Exception(LocalizedMessage.GetLocalizedMessage(AMSOsramConstants.LocalizedMessageProductNameOrFlowNameNotDefined).MessageText);
                }

                if (string.IsNullOrWhiteSpace(input.FlowName) && !string.IsNullOrWhiteSpace(input.FlowVersion))
                {
                    throw new Exception(LocalizedMessage.GetLocalizedMessage(AMSOsramConstants.LocalizedMessageFlowVersionWithoutFlowName).MessageText);
                }

                // Use input FlowName by default
                string flowName = input.FlowName;

                CustomFlowInformationToERPData flowInfoData = new CustomFlowInformationToERPData();

                #region Product Info

                if (!string.IsNullOrWhiteSpace(input.ProductName))
                {
                    Product product = new Product() { Name = input.ProductName };
                    product.Load();

                    if (string.IsNullOrWhiteSpace(product.FlowPath))
                    {
                        throw new Exception(LocalizedMessage.GetLocalizedMessage(AMSOsramConstants.LocalizedMessageProductHasNoFlowPath).MessageText);
                    }

                    // Set Flow associated to the product
                    flowName = product.Flow.Name;

                    product.LoadAttributes();

                    #region Site Mapping

                    // Set Site associated to ProductLine product attribute
                    if (product.HasRelatedAttribute(AMSOsramConstants.ProductAttributeProductionLine))
                    {
                        string productionLine = product.GetRelatedAttributeValue(AMSOsramConstants.ProductAttributeProductionLine, true) as string;

                        if (!string.IsNullOrWhiteSpace(productionLine))
                        {
                            // Load Generic Table CustomProductionLineConversion
                            GenericTable customProdLineConversionGT = new GenericTable() { Name = AMSOsramConstants.GenericTableCustomProductionLineConversion };
                            customProdLineConversionGT.Load();

                            // Based on ProductLine Product attribute get Site and Facility name from Generic Table
                            customProdLineConversionGT.LoadData(new Foundation.BusinessObjects.QueryObject.FilterCollection()
                                {
                                    new Foundation.BusinessObjects.QueryObject.Filter()
                                    {
                                        Name = AMSOsramConstants.GenericTableCustomProductionLineConversionProductionLineProperty,
                                        Operator = FieldOperator.IsEqualTo,
                                        LogicalOperator = LogicalOperator.Nothing,
                                        Value = productionLine
                                    }
                                });

                            if (customProdLineConversionGT.HasData)
                            {
                                DataSet prodLineConversionDataSet = NgpDataSet.ToDataSet(customProdLineConversionGT.Data);

                                flowInfoData.Site = prodLineConversionDataSet.Tables[0].Rows[0][AMSOsramConstants.GenericTableCustomProductionLineConversionSiteProperty].ToString();
                            }
                        }
                    }

                    #endregion Site Mapping

                    #region Product Mapping

                    flowInfoData.ProductInformationData = new ProductInformation()
                    {
                        Name = product.Name,
                        Description = product.Description,
                        Timestamp = product.CreatedOn.ToString(),
                        Type = product.Type,
                        State = product.UniversalState.ToString(),
                        Maturity = product.Maturity,
                        Yield = product.Yield.ToString(),
                        CycleTime = product.CycleTime.ToString(),
                        MaximumMaterialSize = product.MaximumMaterialSize.ToString()
                    };

                    #region Attributes Mapping

                    flowInfoData.ProductInformationData.ProductAttributes = new List<AttributeInformation>();

                    if (product.RelatedAttributes.Any())
                    {
                        foreach (KeyValuePair<string, object> relatedAttribute in product.RelatedAttributes)
                        {
                            AttributeInformation productRelatedAttribute = new AttributeInformation()
                            {
                                Name = relatedAttribute.Key,
                                Value = relatedAttribute.Value.ToString()
                            };

                            flowInfoData.ProductInformationData.ProductAttributes.Add(productRelatedAttribute);
                        }
                    }

                    if (product.Attributes.Any())
                    {
                        foreach (KeyValuePair<string, object> relatedAttribute in product.Attributes)
                        {
                            AttributeInformation productAttribute = new AttributeInformation()
                            {
                                Name = relatedAttribute.Key,
                                Value = relatedAttribute.Value.ToString()
                            };

                            flowInfoData.ProductInformationData.ProductAttributes.Add(productAttribute);
                        }
                    }

                    #endregion Attributes Mapping

                    #region Parameters Mapping

                    ParameterSourceCollection productParameters = product.GetProductParameters();

                    if (productParameters != null && productParameters.Any())
                    {
                        flowInfoData.ProductInformationData.ProductParameters = new List<ParameterInformation>();

                        foreach (ParameterSource parameter in productParameters)
                        {
                            ParameterInformation productParameter = new ParameterInformation()
                            {
                                Name = parameter.Parameter.Name,
                                Type = parameter.Type.ToString(),
                                Value = parameter.Value
                            };

                            flowInfoData.ProductInformationData.ProductParameters.Add(productParameter);
                        }
                    }

                    #endregion Parameters Mapping

                    #endregion Product Mapping

                }

                #endregion Product Info

                #region Flow Info

                Flow flow = new Flow()
                {
                    Name = flowName,
                    Version = !string.IsNullOrWhiteSpace(input.FlowVersion) ? Convert.ToInt32(input.FlowVersion) : 0
                };
                flow.Load();

                #region Flow Mapping

                flowInfoData.FlowInformationData = new FlowInformation()
                {
                    Name = flow.Name,
                    Description = flow.Description,
                    Timestamp = flow.CreatedOn.ToString(),
                    Type = flow.Type.ToString(),
                    State = flow.UniversalState.ToString(),
                    Version = flow.Version.ToString(),
                    LogicalName = flow.LogicalNames?.FirstOrDefault()?.LogicalName
                };

                #endregion Flow Mapping

                #region Steps Mapping

                flow.LoadChilds();

                FlowStepCollection flowSteps = flow.FlowSteps;

                if (flowSteps != null && flowSteps.Any())
                {
                    #region Area Mapping

                    flowSteps.FirstOrDefault().TargetEntity.LoadRelations(Cmf.Navigo.Common.Constants.StepArea, 1);

                    // Set Cost Center associated to the first Area of the first FlowStep
                    flowInfoData.CostCenter = flowSteps.FirstOrDefault().TargetEntity?.StepAreas?.FirstOrDefault()?.TargetEntity?.CostCenter;

                    #endregion Area Mapping

                    flowInfoData.FlowInformationData.Steps = new List<StepInformation>();

                    foreach (FlowStep flowStep in flowSteps)
                    {
                        StepInformation stepInformation = new StepInformation()
                        {
                            Name = flowStep.TargetEntity?.Name,
                            Description = flowStep.TargetEntity?.Description,
                            Timestamp = flowStep.TargetEntity?.CreatedOn.ToString(),
                            Type = flowStep.TargetEntity?.Type,
                            State = flowStep.TargetEntity?.UniversalState.ToString(),
                            LogicalName = flowStep.LogicalName,
                            Maturity = flowStep.TargetEntity?.Maturity
                        };

                        flowInfoData.FlowInformationData.Steps.Add(stepInformation);

                        #region Attributes Mapping

                        flowStep.TargetEntity?.LoadAttributes();

                        if (flowStep.TargetEntity?.Attributes != null && flowStep.TargetEntity.Attributes.Any())
                        {
                            stepInformation.StepAttributes = new List<AttributeInformation>();

                            foreach (KeyValuePair<string, object> attribute in flowStep.TargetEntity?.Attributes)
                            {
                                AttributeInformation stepAttribute = new AttributeInformation()
                                {
                                    Name = attribute.Key,
                                    Value = attribute.Value.ToString()
                                };

                                stepInformation.StepAttributes.Add(stepAttribute);
                            }
                        }

                        #endregion Attributes Mapping
                    }
                }

                #endregion FlowSteps Mapping

                #endregion Flow Info

                #region Returned Message

                if (flowInfoData is null && flowInfoData.ProductInformationData is null && flowInfoData.FlowInformationData is null)
                {
                    throw new Exception(LocalizedMessage.GetLocalizedMessage(AMSOsramConstants.LocalizedMessageCustomFlowInformationToERPDataObjectNull).MessageText);
                }

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(flowInfoData.SerializeToXML());

                output.ResultXml = xmlDocument.InnerXml;

                #endregion Returned Message

                Utilities.EndMethod(-1, -1,
                                    new KeyValuePair<string, object>(GET_FLOW_INFORMATION_FOR_ERP_INPUT, input),
                                    new KeyValuePair<string, object>(GET_FLOW_INFORMATION_FOR_ERP_OUTPUT, output));
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
    }
}
