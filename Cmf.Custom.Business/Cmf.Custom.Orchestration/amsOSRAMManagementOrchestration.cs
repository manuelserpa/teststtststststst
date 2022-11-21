using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Custom.amsOSRAM.Orchestration.Abstractions;
using Cmf.Custom.amsOSRAM.Orchestration.InputObjects;
using Cmf.Custom.amsOSRAM.Orchestration.OutputObjects;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.BusinessOrchestration.Abstractions;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Foundation.Common.LocalizationService;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessOrchestration.Abstractions;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using static Cmf.Custom.amsOSRAM.Common.DataStructures.CustomFlowInformationToERPData;

namespace Cmf.Custom.amsOSRAM.Orchestration
{
    public class amsOSRAMManagementOrchestration : IamsOSRAMManagementOrchestration
    {
        // Entity Factory
        private readonly IEntityFactory _entityFactory;

        private readonly ILocalizationService _localizationService;

        // Orchestrations
        private readonly IGenericServiceOrchestration _genericServiceOrchestration;

        private readonly IMaterialOrchestration _materialOrchestration;

        // Constants
        private const string OBJECT_TYPE_NAME = "Cmf.Custom.amsOSRAM.Orchestration.amsOSRAMManagementOrchestration";

        private const string MATERIAL_IN = "MaterialIn";
        private const string MATERIAL_IN_INPUT = "MaterialInInput";
        private const string MATERIAL_IN_OUTPUT = "MaterialInOutput";

        private const string GET_FLOW_INFORMATION_FOR_ERP = "GetFlowInformationForERP";
        private const string GET_FLOW_INFORMATION_FOR_ERP_INPUT = "GetFlowInformationForERPInput";
        private const string GET_FLOW_INFORMATION_FOR_ERP_OUTPUT = "GetFlowInformationForERPOutput";

        private const string CUSTOM_RECEIVE_STIBO_MESSAGE = "CustomReceiveStiboMessage";
        private const string CUSTOM_RECEIVE_STIBO_MESSAGE_INPUT = "CustomReceiveStiboMessageInput";
        private const string CUSTOM_RECEIVE_STIBO_MESSAGE_OUTPUT = "CustomReceiveStiboMessageOutput";

        private const string CUSTOM_RECEIVE_ERP_MESSAGE = "CustomReceiveERPMessage";
        private const string CUSTOM_RECEIVE_ERP_MESSAGE_INPUT = "CustomReceiveERPMessageInput";
        private const string CUSTOM_RECEIVE_ERP_MESSAGE_OUTPUT = "CustomReceiveERPMessageOutput";

        private const string MATERIAL_OUT = "MaterialOut";
        private const string MATERIAL_OUT_INPUT = "MaterialOutInput";
        private const string MATERIAL_OUT_OUTPUT = "MaterialOutOutput";

        private const string CUSTOM_GET_MATERIAL_ATTRIBUTES = "CustomGetMaterialAttributes";
        private const string CUSTOM_GET_MATERIAL_ATTRIBUTES_OUTPUT = "CustomGetMaterialAttributesOutput";
        private const string CUSTOM_GET_MATERIAL_ATTRIBUTES_INPUT = "CustomGetMaterialAttributesInput";

        /// <summary>
        /// Initializes a new instance of the <see cref="amsOSRAMManagementOrchestration"/> class.
        /// </summary>
        [ActivatorUtilitiesConstructor]
        public amsOSRAMManagementOrchestration(IGenericServiceOrchestration genericServiceOrchestration, IMaterialOrchestration materialOrchestration) : base()
        {
            this._genericServiceOrchestration = genericServiceOrchestration;
            this._materialOrchestration = materialOrchestration;
            IServiceProvider services = ApplicationContext.CurrentServiceProvider;

            // Get Entity Factory
            _entityFactory = services.GetService<IEntityFactory>();
            _localizationService = services.GetService<ILocalizationService>();
        }

        #region Material

        /// <summary>
        /// Performs the Material Track In in the Resource
        /// </summary>
        /// <param name="input"></param>
        /// <returns>MaterialIn Output object</returns>
        public MaterialInOutput MaterialIn(MaterialInInput input)
        {
            MaterialInOutput output = null;
            Utilities.StartMethod(OBJECT_TYPE_NAME, MATERIAL_IN,
                new KeyValuePair<string, object>(MATERIAL_IN_INPUT, input));
            try
            {
                output = new MaterialInOutput();

                #region Input Validation

                Dictionary<string, object> customMaterialInValidationInput = new Dictionary<string, object>()
                {
                    { "MaterialInInput", input }
                };

                IAction actionCustomMaterialInValidation = new Foundation.Common.DynamicExecutionEngine.Action();
                actionCustomMaterialInValidation.Load("CustomMaterialInValidation");
                Dictionary<string, object> customMaterialInValidationOutput = actionCustomMaterialInValidation.ExecuteAction(customMaterialInValidationInput);

                IResource resource = DeeActionHelper.GetInputItem<IResource>(customMaterialInValidationOutput, "Resource");
                IMaterial waferToTrackIn = DeeActionHelper.GetInputItem<IMaterial>(customMaterialInValidationOutput, "Material");
                bool isSorter = DeeActionHelper.GetInputItem<bool>(customMaterialInValidationOutput, "IsSorter");

                #endregion Input Validation

                // Tuple structure
                //	- Item 1: Material
                //	- Item 2: Load Port Name
                //	- Item 3: Container Name
                //	- Item 4: Map Container Needed
                List<(IMaterial material, string loadPort, string container, bool mapContainerNeeded)> lotsAssociatedToContainersDockedOnResource = new List<(IMaterial material, string loadPort, string container, bool mapContainerNeeded)>();

                if (waferToTrackIn != null)
                {
                    lotsAssociatedToContainersDockedOnResource.Add((waferToTrackIn, "", string.IsNullOrWhiteSpace(input.CarrierId) ? "" : input.CarrierId, false));
                }
                else
                {
                    #region Trying to fetch materials docked on load ports

                    Dictionary<string, object> actionInputCustomMaterialInGetLotsFromDockedContainers = new Dictionary<string, object>()
                    {
                        { "Resource", resource }
                    };

                    IAction actionCustomMaterialInGetLotsFromDockedContainers = new Foundation.Common.DynamicExecutionEngine.Action();
                    actionCustomMaterialInGetLotsFromDockedContainers.Load("CustomMaterialInGetLotsFromDockedContainers");
                    actionCustomMaterialInGetLotsFromDockedContainers.ExecuteAction(actionInputCustomMaterialInGetLotsFromDockedContainers);

                    lotsAssociatedToContainersDockedOnResource = actionInputCustomMaterialInGetLotsFromDockedContainers["LotsDockedOnSorters"] as List<(IMaterial material, string loadPort, string container, bool mapContainerNeeded)>;

                    #endregion Trying to fetch materials docked on load ports
                }

                if (lotsAssociatedToContainersDockedOnResource.Count == 0)
                {
                    throw new Exception($"No materials found to Track-In in the resource ({resource.Name}).");
                }

                bool isToThrowException = false;
                string exceptionMessage = string.Empty;

                #region Execution

                foreach ((IMaterial material, string loadPort, string container, bool mapContainerNeeded) lotDockedOnSorter in lotsAssociatedToContainersDockedOnResource)
                {
                    // lotDockedOnSorter.Item4 = FORCED MAP CONTAINER
                    // Because Forced Map Carrier can be executed at container level, without a lot associated
                    // This operation takes precedence over anything else
                    // Also, when we don't have a lot associated with a container but the forced map container is set to true
                    //		we are using a dummy lot to be able to handle this scenario and we cannot do a Load on a dummy material.
                    // Also, if it is a Forced Map Carrier we cannot perform a track-in we just want to read the wafers on the container.
                    if (isSorter && lotDockedOnSorter.mapContainerNeeded)
                    {
                        Dictionary<string, object> actionInputCustomSendStartMapProccessToIoT = new Dictionary<string, object>()
                        {
                            { "ContainerName", lotDockedOnSorter.container },
                            { "ResourceName",  input.ResourceName}
                        };

                        IAction actionCustomSendStartMapProcessToIoT = new Foundation.Common.DynamicExecutionEngine.Action();
                        actionCustomSendStartMapProcessToIoT.Load("CustomSendStartMapProccessToIoT");
                        actionCustomSendStartMapProcessToIoT.ExecuteAction(actionInputCustomSendStartMapProccessToIoT);

                        break;
                    }

                    IMaterial materialToTrackIn = lotDockedOnSorter.material;

                    if (materialToTrackIn != null)
                    {
                        materialToTrackIn.Load();

                        if (materialToTrackIn.SystemState != MaterialSystemState.InProcess)
                        {
                            // IS TOP MOST MATERIAL
                            if (materialToTrackIn.ParentMaterial == null && materialToTrackIn.Form == "Lot")
                            {
                                // Validate if a lot can start the process
                                bool canStartProcess = true;
                                ICustomSorterJobDefinition customSorterJobDefinition = null;
                                string futureActionType = string.Empty;

                                // Containers docked will be updated by DEE Action 'CustomProcessSorterJobDefinition'
                                // Because it will be that information used to check if we can start a process or not
                                List<ResourceLoadPortData> containersDocked = null;

                                if (isSorter)
                                {
                                    // Validate current load port
                                    IResource currentLoadPort = _entityFactory.Create<IResource>();
                                    currentLoadPort.Name = lotDockedOnSorter.loadPort;

                                    if (string.IsNullOrWhiteSpace(lotDockedOnSorter.loadPort) || !currentLoadPort.ObjectExists())
                                    {
                                        throw new Exception($"There isn't a valid load port associated with the material ({materialToTrackIn.Name}).");
                                    }
                                    else
                                    {
                                        currentLoadPort.Load();
                                    }

                                    // Validate current container
                                    IContainer currentContainer = _entityFactory.Create<IContainer>();
                                    currentContainer.Name = lotDockedOnSorter.container;

                                    if (string.IsNullOrWhiteSpace(lotDockedOnSorter.container) || !currentContainer.ObjectExists())
                                    {
                                        throw new Exception($"There isn't a valid container associated with the material ({materialToTrackIn.Name}).");
                                    }
                                    else
                                    {
                                        currentContainer.Load();
                                    }

                                    IBOM bom = null;

                                    // SmartTable resolution for a valid custom sorter job

                                    customSorterJobDefinition = amsOSRAMUtilities.GetSorterJobDefinition(materialToTrackIn);

                                    if (customSorterJobDefinition != null)
                                    {
                                        if (customSorterJobDefinition.LogisticalProcess == amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessCompose)
                                        {
                                            bom = amsOSRAMUtilities.ResolveBOMContext(materialToTrackIn);

                                            if (bom is null)
                                            {
                                                isToThrowException = true;
                                                exceptionMessage = $"Cannot resolve a BOM context for the material: ({materialToTrackIn.Name}).{Environment.NewLine}";
                                                // Jump to the next foreach element
                                                continue;
                                            }
                                        }

                                        IAction actionCustomMaterialInProcessSorterJobDefinition = new Foundation.Common.DynamicExecutionEngine.Action();
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
                                        IMaterialResource materialResource = _entityFactory.Create<IMaterialResource>();
                                        materialResource.SourceEntity = materialToTrackIn;
                                        materialResource.TargetEntity = resource;

                                        DispatchMaterialOutput dispatchOutput = _materialOrchestration.DispatchMaterial(new DispatchMaterialInput()
                                        {
                                            Material = materialToTrackIn,
                                            Resource = resource,
                                            MaterialResource = materialResource
                                        });

                                        materialToTrackIn = dispatchOutput.Material;
                                        resource = dispatchOutput.Resource;
                                    }

                                    #endregion Check Material dispatch status

                                    #region TrackIn current Material

                                    if (materialToTrackIn.SystemState == MaterialSystemState.Dispatched)
                                    {
                                        GetDataForTrackInWizardOutput dataForTrackIn = _materialOrchestration.GetDataForTrackInWizard(new GetDataForTrackInWizardInput()
                                        {
                                            Material = materialToTrackIn,
                                            Resource = resource
                                        });

                                        ComplexTrackInMaterialInput complexTrackIn = new ComplexTrackInMaterialInput()
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

                                        waferToTrackIn = _materialOrchestration.ComplexTrackInMaterial(complexTrackIn).Material;
                                        isToThrowException = false;
                                    }

                                    #endregion TrackIn current Material

                                    // If it was possible to process a custom sorter job definition we will need to check
                                    //		what are the load ports to update its state to inUse.
                                    // Also, if it's a merge we also need to Track-In lots associated with the containers defined as source carriers
                                    //		on the movement list.
                                    if (waferToTrackIn.SystemState == MaterialSystemState.InProcess &&
                                        isSorter &&
                                        customSorterJobDefinition != null)
                                    {
                                        IAction actionCustomMaterialInPostProcessSorterJobDefinition = new Foundation.Common.DynamicExecutionEngine.Action();
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
                                List<IEntityRelation> subResourceToTrackIn;

                                resource.LoadRelations("SubResource");

                                IResource resourceToTrackIn = null;
                                int subResourceOrder = 0;

                                if (resource.RelationCollection.ContainsKey("SubResource") && resource.RelationCollection["SubResource"] != null)
                                {
                                    //SubResourceOrder Starts in 1
                                    if (input.SubResourceOrder != null)
                                    {
                                        //SubResourceOrder Starts in 1
                                        subResourceOrder = input.SubResourceOrder.Value - 1;
                                    }

                                    subResourceToTrackIn = resource.RelationCollection["SubResource"].Where(sbr => (sbr as ISubResource).TargetEntity.ProcessingType == ProcessingType.Process).OrderBy(sbr => (sbr as ISubResource).Order).ToList();

                                    if (subResourceToTrackIn.Count > 0)
                                    {
                                        if (subResourceToTrackIn.Count < subResourceOrder || subResourceOrder < 0)
                                        {
                                            throw new IndexOutOfRangeException("Sub Resource Order parameter is Out of Range");
                                        }

                                        resourceToTrackIn = (IResource)subResourceToTrackIn[subResourceOrder].TargetEntity;
                                    }
                                }

                                materialToTrackIn.TrackIn(resourceToTrackIn);
                            }
                        }
                    }
                }

                if (isToThrowException)
                {
                    throw new CmfBaseException(exceptionMessage);
                }

                output.Material = waferToTrackIn;

                #endregion Execution

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
        public MaterialOutOutput MaterialOut(MaterialOutInput input)
        {
            MaterialOutOutput output = null;
            Utilities.StartMethod(OBJECT_TYPE_NAME, MATERIAL_OUT,
                new KeyValuePair<string, object>(MATERIAL_OUT_INPUT, input));
            try
            {
                output = new MaterialOutOutput();

                #region Input Validation

                Utilities.ValidateNullInput(input, new List<string> { "CustomSorterJobDefinition" });

                bool containerOnlyProcess = input.ContainerOnlyProcess;
                bool canTrackOut = !containerOnlyProcess;

                if (string.IsNullOrWhiteSpace(input.MaterialName) && string.IsNullOrEmpty(input.CarrierId))
                {
                    throw new MissingMandatoryFieldCmfException("MaterialName or CarrierId");
                }

                if (string.IsNullOrWhiteSpace(input.ResourceName))
                {
                    throw new MissingMandatoryFieldCmfException("ResourceName");
                }

                IResource resource = _entityFactory.Create<IResource>();
                resource.Name = input.ResourceName;

                if (!resource.ObjectExists())
                {
                    throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Resource, input.ResourceName);
                }

                #endregion Input Validation

                #region Execution

                if (canTrackOut)
                {
                    IMaterial lot = _entityFactory.Create<IMaterial>();

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
                        input.CustomSorterJobDefinition.LogisticalProcess != amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessMapCarrier)
                    {
                        Dictionary<string, object> actionInputCustomMaterialOutProcessSorterJobDefinition = new Dictionary<string, object>()
                        {
                            { "Resource", resource },
                            { "Lot",  lot },
                            { "CustomSorterJobDefinition",  input.CustomSorterJobDefinition },
                        };

                        IAction actionCustomMaterialOutProcessSorterJobDefinition = new Foundation.Common.DynamicExecutionEngine.Action();
                        actionCustomMaterialOutProcessSorterJobDefinition.Load("CustomMaterialOutProcessSorterJobDefinition");
                        actionCustomMaterialOutProcessSorterJobDefinition.ExecuteAction(actionInputCustomMaterialOutProcessSorterJobDefinition);
                    }

                    #endregion Handle Sorter Job Definition for different scenarios (Split, Merge, Compose)

                    if (lot.UniversalState != Foundation.Common.Base.UniversalState.Terminated)
                    {
                        IMaterial material;

                        if (lot.SystemState == MaterialSystemState.InProcess)
                        {
                            material = lot;
                        }
                        else
                        {
                            MaterialInInput materialInInput = new MaterialInInput()
                            {
                                MaterialName = input.MaterialName,
                                ResourceName = input.ResourceName,
                                CarrierId = input.CarrierId
                            };

                            material = MaterialIn(materialInInput).Material;
                        }

                        if (material.SystemState == MaterialSystemState.InProcess)
                        {
                            if (material.ParentMaterial == null)
                            {
                                // Load resource to get the Main State Model.
                                IResource currentTrackOutResource = _entityFactory.Create<IResource>();
                                currentTrackOutResource.Name = resource.Name;
                                currentTrackOutResource.Load();

                                IMaterialCollection materials = _entityFactory.CreateCollection<IMaterialCollection>();
                                materials.Add(material);

                                GetDataForMultipleTrackOutAndMoveNextWizardOutput getDataForMultipleTrackOutAndMoveNextWizardOutput = _materialOrchestration.GetDataForMultipleTrackOutAndMoveNextWizard(new GetDataForMultipleTrackOutAndMoveNextWizardInput()
                                {
                                    Materials = materials
                                });

                                _materialOrchestration.ComplexTrackOutAndMoveMaterialToNextStep(new ComplexTrackOutAndMoveMaterialToNextStepInput()
                                {
                                    Material = material,
                                    StateModel = currentTrackOutResource?.CurrentMainState?.StateModel ?? null,
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

                #endregion Execution

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
        private void LocalSendTrackOutToIoT(MaterialOutInput input, IResource resource, string materialId = null)
        {
            resource.Load();

            IAutomationControllerInstance controllerInstance = resource.GetAutomationControllerInstance();
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
                int requestTimeout = amsOSRAMUtilities.GetConfig<int>(amsOSRAMConstants.AutomationGenericRequestTimeoutConfigurationPath);

                // Send Synchronous request to automation TrackOut the Material in the Equipment
                string requestType = amsOSRAMConstants.AutomationRequestTypeTrackOut;
                object obj = controllerInstance.SendRequest(requestType, materialDataToIot.ToJsonString(), requestTimeout);

                if (obj == null)
                {
                    throw new CmfBaseException(string.Format(_localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageIoTConnectionTimeout), requestType));
                }
            }
        }

        #endregion Material

        /// <summary>
        /// Service to generate an Integration Entry based on the received Stibo message
        /// </summary>
        /// <param name="input">Input Object</param>
        /// <returns>Output Object</returns>
        /// <exception cref="CmfBaseException">If any unexpected error occurs.</exception>
        public CustomReceiveStiboMessageOutput CustomReceiveStiboMessage(CustomReceiveStiboMessageInput input)
        {
            Utilities.StartMethod(
                OBJECT_TYPE_NAME,
                CUSTOM_RECEIVE_STIBO_MESSAGE,
                new KeyValuePair<string, object>(CUSTOM_RECEIVE_STIBO_MESSAGE_INPUT, input));

            CustomReceiveStiboMessageOutput output = new CustomReceiveStiboMessageOutput();

            try
            {
                Utilities.ValidateNullInput(input);

                if (!string.IsNullOrEmpty(input.Message))
                {
                    // Set Integration Entry properties
                    IIntegrationEntry integrationEntry = _entityFactory.Create<IIntegrationEntry>();
                    integrationEntry.Name = Guid.NewGuid().ToString("N");
                    integrationEntry.EventName = amsOSRAMConstants.CustomIntegrationInboundEventName;
                    integrationEntry.SourceSystem = amsOSRAMConstants.CustomStiboSystem;
                    integrationEntry.TargetSystem = Constants.MesSystemDesignation;
                    integrationEntry.MessageType = input.MessageType;
                    integrationEntry.SystemState = IntegrationEntrySystemState.Received;
                    integrationEntry.MessageDate = DateTime.Now;

                    IIntegrationMessage integrationMessage = _entityFactory.Create<IIntegrationMessage>();
                    integrationMessage.Message = Encoding.Default.GetBytes(input.Message);

                    integrationEntry.IntegrationMessage = integrationMessage;

                    // Create Integration Entry and Set to Result
                    output.Result = _genericServiceOrchestration.CreateObject(
                        createObjectInput: new CreateObjectInput
                        {
                            Object = integrationEntry
                        }).Object as IIntegrationEntry;
                }
                else
                {
                    throw new Exception(_localizationService.Localize(amsOSRAMConstants.CustomReceiveEmptyMessage));
                }

                Utilities.EndMethod(
                    -1,
                    -1,
                    new KeyValuePair<string, object>(CUSTOM_RECEIVE_STIBO_MESSAGE_INPUT, input),
                    new KeyValuePair<string, object>(CUSTOM_RECEIVE_STIBO_MESSAGE_OUTPUT, output));
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
        /// <exception cref="CmfBaseException">If any unexpected error occurs.</exception>
        public CustomReceiveERPMessageOutput CustomReceiveERPMessage(CustomReceiveERPMessageInput input)
        {
            Utilities.StartMethod(
                OBJECT_TYPE_NAME,
                CUSTOM_RECEIVE_ERP_MESSAGE,
                new KeyValuePair<string, object>(CUSTOM_RECEIVE_ERP_MESSAGE_INPUT, input));

            CustomReceiveERPMessageOutput output = new CustomReceiveERPMessageOutput();

            try
            {
                Utilities.ValidateNullInput(input);

                if (!string.IsNullOrEmpty(input.Message))
                {
                    // create Integration Entry
                    IIntegrationEntry integrationEntry = _entityFactory.Create<IIntegrationEntry>();
                    integrationEntry.Name = Guid.NewGuid().ToString("N");
                    integrationEntry.EventName = amsOSRAMConstants.CustomIntegrationInboundEventName;
                    integrationEntry.SourceSystem = amsOSRAMConstants.CustomERPSystem;
                    integrationEntry.TargetSystem = Constants.MesSystemDesignation;
                    integrationEntry.MessageType = input.MessageType;
                    integrationEntry.SystemState = IntegrationEntrySystemState.Received;
                    integrationEntry.MessageDate = DateTime.Now;

                    IIntegrationMessage integrationMessage = _entityFactory.Create<IIntegrationMessage>();
                    integrationMessage.Message = Encoding.Default.GetBytes(input.Message);
                    integrationEntry.IntegrationMessage = integrationMessage;

                    // update output
                    output.Result = _genericServiceOrchestration.CreateObject(new CreateObjectInput
                    {
                        Object = integrationEntry
                    }).Object as IIntegrationEntry;
                }
                else
                {
                    throw new Exception(_localizationService.Localize(amsOSRAMConstants.CustomReceiveEmptyMessage));
                }

                Utilities.EndMethod(
                    -1,
                    -1,
                    new KeyValuePair<string, object>(CUSTOM_RECEIVE_ERP_MESSAGE_INPUT, input),
                    new KeyValuePair<string, object>(CUSTOM_RECEIVE_ERP_MESSAGE_OUTPUT, output));
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
        public CustomGetFlowInformationForERPOutput GetFlowInformationForERP(CustomGetFlowInformationForERPInput input)
        {
            Utilities.StartMethod(OBJECT_TYPE_NAME, GET_FLOW_INFORMATION_FOR_ERP,
                                  new KeyValuePair<string, object>(GET_FLOW_INFORMATION_FOR_ERP_INPUT, input));

            CustomGetFlowInformationForERPOutput output = new CustomGetFlowInformationForERPOutput();

            try
            {
                Utilities.ValidateNullInput(input);

                if (!string.IsNullOrWhiteSpace(input.ProductName) && !string.IsNullOrWhiteSpace(input.FlowName))
                {
                    throw new Exception(_localizationService.Localize(amsOSRAMConstants.LocalizedMessageProductNameAndFlowNameAtSameTime));
                }

                if (string.IsNullOrWhiteSpace(input.ProductName) && string.IsNullOrWhiteSpace(input.FlowName))
                {
                    throw new Exception(_localizationService.Localize(amsOSRAMConstants.LocalizedMessageProductNameOrFlowNameNotDefined));
                }

                if (string.IsNullOrWhiteSpace(input.FlowName) && !string.IsNullOrWhiteSpace(input.FlowVersion))
                {
                    throw new Exception(_localizationService.Localize(amsOSRAMConstants.LocalizedMessageFlowVersionWithoutFlowName));
                }

                // Use input FlowName by default
                string flowName = input.FlowName;

                CustomFlowInformationToERPData flowInfoData = new CustomFlowInformationToERPData();

                #region Product Info

                if (!string.IsNullOrWhiteSpace(input.ProductName))
                {
                    IProduct product = _entityFactory.Create<IProduct>();
                    product.Name = input.ProductName;
                    product.Load();

                    if (string.IsNullOrWhiteSpace(product.FlowPath))
                    {
                        throw new Exception(_localizationService.Localize(amsOSRAMConstants.LocalizedMessageProductHasNoFlowPath));
                    }

                    // Set Flow associated to the product
                    flowName = product.Flow.Name;

                    product.LoadAttributes();

                    #region Site Mapping

                    // Set Site associated to ProductLine product attribute
                    if (product.HasRelatedAttribute(amsOSRAMConstants.ProductAttributeProductionLine))
                    {
                        string productionLine = product.GetRelatedAttributeValue(amsOSRAMConstants.ProductAttributeProductionLine, true) as string;

                        if (!string.IsNullOrWhiteSpace(productionLine))
                        {
                            // Load Generic Table CustomProductionLineConversion
                            GenericTable customProdLineConversionGT = new GenericTable() { Name = amsOSRAMConstants.GenericTableCustomProductionLineConversion };
                            customProdLineConversionGT.Load();

                            // Based on ProductLine Product attribute get Site and Facility name from Generic Table
                            customProdLineConversionGT.LoadData(new Foundation.BusinessObjects.QueryObject.FilterCollection()
                                {
                                    new Foundation.BusinessObjects.QueryObject.Filter()
                                    {
                                        Name = amsOSRAMConstants.GenericTableCustomProductionLineConversionProductionLineProperty,
                                        Operator = FieldOperator.IsEqualTo,
                                        LogicalOperator = LogicalOperator.Nothing,
                                        Value = productionLine
                                    }
                                });

                            if (customProdLineConversionGT.HasData)
                            {
                                DataSet prodLineConversionDataSet = NgpDataSet.ToDataSet(customProdLineConversionGT.Data);

                                flowInfoData.Site = prodLineConversionDataSet.Tables[0].Rows[0][amsOSRAMConstants.GenericTableCustomProductionLineConversionSiteProperty].ToString();
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

                    ParameterSourceCollection productParameters = (ParameterSourceCollection)product.GetProductParameters();

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

                IFlow flow = _entityFactory.Create<IFlow>();
                flow.Name = flowName;
                flow.Version = !string.IsNullOrWhiteSpace(input.FlowVersion) ? Convert.ToInt32(input.FlowVersion) : 0;
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

                IFlowStepCollection flowSteps = flow.FlowSteps;

                if (flowSteps != null && flowSteps.Any())
                {
                    #region Area Mapping

                    flowSteps.FirstOrDefault().TargetEntity.LoadRelations(Cmf.Navigo.Common.Constants.StepArea, 1);

                    // Set Cost Center associated to the first Area of the first FlowStep
                    flowInfoData.CostCenter = flowSteps.FirstOrDefault().TargetEntity?.StepAreas?.FirstOrDefault()?.TargetEntity?.CostCenter;

                    #endregion Area Mapping

                    flowInfoData.FlowInformationData.Steps = new List<StepInformation>();

                    foreach (IFlowStep flowStep in flowSteps)
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

                #endregion Steps Mapping

                #endregion Flow Info

                #region Returned Message

                if (flowInfoData is null && flowInfoData.ProductInformationData is null && flowInfoData.FlowInformationData is null)
                {
                    throw new Exception(_localizationService.Localize(amsOSRAMConstants.LocalizedMessageCustomFlowInformationToERPDataObjectNull));
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

        /// <summary>
        /// Service to provide material attribute information to EADOS
        /// </summary>
        /// <param name="customGetMaterialAttributesInput">Input Object</param>
        /// <returns></returns>
        public CustomGetMaterialAttributesOutput CustomGetMaterialAttributes(CustomGetMaterialAttributesInput customGetMaterialAttributesInput)
        {
            Utilities.StartMethod(OBJECT_TYPE_NAME, CUSTOM_GET_MATERIAL_ATTRIBUTES,
                new KeyValuePair<string, object>(CUSTOM_GET_MATERIAL_ATTRIBUTES_INPUT, customGetMaterialAttributesInput));

            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesOutput();

            try
            {
                Utilities.ValidateNullInput(customGetMaterialAttributesInput);

                if (string.IsNullOrWhiteSpace(customGetMaterialAttributesInput.MaterialList))
                {
                    throw new MissingMandatoryFieldCmfException("MaterialList");
                }

                List<string> separatedMaterialList = customGetMaterialAttributesInput.MaterialList.Split(',').ToList();

                CustomGetMaterialAttributesData dataToXML = new CustomGetMaterialAttributesData();
                IMaterialCollection loadedMaterials = _entityFactory.CreateCollection<IMaterialCollection>();

                List<Common.DataStructures.CustomGetMaterialAttributesDataDto.Material> materialsForXML = new List<Common.DataStructures.CustomGetMaterialAttributesDataDto.Material>();
                List<string> separatedAttributeList = null;
                List<string> separatedSubMaterialList = null;

                if (!string.IsNullOrWhiteSpace(customGetMaterialAttributesInput.AttributeList))
                {
                    separatedAttributeList = customGetMaterialAttributesInput.AttributeList.Split(',').ToList();
                }
                if (!string.IsNullOrWhiteSpace(customGetMaterialAttributesInput.SubMaterialAttributeList))
                {
                    separatedSubMaterialList = customGetMaterialAttributesInput.SubMaterialAttributeList.Split(',').ToList();
                }

                foreach (string materialName in separatedMaterialList)
                {
                    IMaterial materialToAdd = _entityFactory.Create<IMaterial>();
                    materialToAdd.Name = materialName;
                    materialToAdd.Load();

                    List<Common.DataStructures.CustomGetMaterialAttributesDataDto.Attribute> mainMaterialAttributes = new List<Common.DataStructures.CustomGetMaterialAttributesDataDto.Attribute>();

                    if (separatedAttributeList != null)
                    {
                        Collection<string> mainMaterialAttributeNameCollection = new Collection<string>();
                        mainMaterialAttributeNameCollection.AddRange(separatedAttributeList);
                        materialToAdd.LoadAttributes(mainMaterialAttributeNameCollection);
                    } else
                    {
                        materialToAdd.LoadAttributes();
                    }

                    foreach (KeyValuePair<string, object> attributeOfMainMaterial in materialToAdd.Attributes)
                    {
                        if (attributeOfMainMaterial.Value != null)
                        {
                            mainMaterialAttributes.Add(new Common.DataStructures.CustomGetMaterialAttributesDataDto.Attribute()
                            {
                                Name = attributeOfMainMaterial.Key,
                                Value = attributeOfMainMaterial.Value.ToString()
                            });
                        }
                    }

                    Common.DataStructures.CustomGetMaterialAttributesDataDto.Material materialForXML = new Common.DataStructures.CustomGetMaterialAttributesDataDto.Material();

                    materialForXML.Name = materialToAdd.Name;
                    materialForXML.Form = materialToAdd.Form;
                    materialForXML.Attributes = mainMaterialAttributes;

                    if (string.IsNullOrWhiteSpace(customGetMaterialAttributesInput.IncludeSubMaterials) ||
                        customGetMaterialAttributesInput.IncludeSubMaterials.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    {
                        materialToAdd.LoadChildren();

                        List<Common.DataStructures.CustomGetMaterialAttributesDataDto.Material> subMaterialsForXML = new List<Common.DataStructures.CustomGetMaterialAttributesDataDto.Material>();

                        if (separatedSubMaterialList != null)
                        {
                            Collection<string> subMaterialAttributeNameCollection = new Collection<string>();
                            subMaterialAttributeNameCollection.AddRange(separatedSubMaterialList);
                            materialToAdd.SubMaterials.LoadAttributes(subMaterialAttributeNameCollection);
                        } else
                        {
                            materialToAdd.SubMaterials.LoadAttributes();
                        }

                        foreach (IMaterial subMat in materialToAdd.SubMaterials)
                        {
                            List<Common.DataStructures.CustomGetMaterialAttributesDataDto.Attribute> subMaterialAttributesForXML = new List<Common.DataStructures.CustomGetMaterialAttributesDataDto.Attribute>();

                            foreach (KeyValuePair<string, object> attributeOfSubMaterial in subMat.Attributes)
                            {
                                if (attributeOfSubMaterial.Value != null)
                                {
                                    subMaterialAttributesForXML.Add(new Common.DataStructures.CustomGetMaterialAttributesDataDto.Attribute() { 
                                        Name = attributeOfSubMaterial.Key, 
                                        Value = attributeOfSubMaterial.Value.ToString() 
                                    });
                                }
                            }

                            subMaterialsForXML.Add(new Common.DataStructures.CustomGetMaterialAttributesDataDto.Material()
                            {
                                Name = subMat.Name,
                                Form = subMat.Form,
                                Attributes = subMaterialAttributesForXML
                            });
                        }

                        materialForXML.SubMaterials = subMaterialsForXML;
                    }

                    materialsForXML.Add(materialForXML);
                }

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Common.DataStructures.CustomGetMaterialAttributesDataDto.Material>), new XmlRootAttribute("CustomGetMaterialAttributes"));
                string xmlToReturn = "";

                using (StringWriter stringWriter = new StringWriter())
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
                    {
                        xmlSerializer.Serialize(xmlWriter, materialsForXML);
                        xmlToReturn = stringWriter.ToString();
                    }
                }

                customGetMaterialAttributesOutput.ResultXML = xmlToReturn;

                Utilities.EndMethod(
                    -1, -1,
                    new KeyValuePair<string, object>(GET_FLOW_INFORMATION_FOR_ERP_INPUT, customGetMaterialAttributesInput),
                    new KeyValuePair<string, object>(GET_FLOW_INFORMATION_FOR_ERP_OUTPUT, customGetMaterialAttributesOutput));
            }
            catch (CmfBaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CmfBaseException(ex.Message, ex);
            }

            return customGetMaterialAttributesOutput;
        }
    }
}
