using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessOrchestration;
using Cmf.Foundation.BusinessOrchestration.QueryManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.QueryManagement.OutputObjects;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.DispatchManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.FacilityManagement.FlowManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;
using Cmf.Navigo.BusinessOrchestration.OrderManagement.InputObjects;
using Cmf.TestScenarios.Others;
//using amsOSRAMEIAutomaticTests.Objects.TestUtilities;
using amsOSRAMEIAutomaticTests.Objects.Utilities;
using EIUtilities = cmConnect.TestFramework.Common.Utilities;
using Cmf.Custom.TestUtilities;

namespace amsOSRAMEIAutomaticTests.Objects.Extensions
{
    /// <summary>
    /// Extension Methods Class that adapts material services as Business Objects extension methods or adds functionality need for testing
    /// </summary>
    public static partial class Extensions
    {
        #region Material

        #region Material Management Services

        #region Genealogy

        /// <summary>
        /// Merges a given material into the current material
        /// </summary>
        /// <param name="instance">material into which materialsToMerge will be merged</param>
        /// <param name="materialToMerge">Material to be merged into current Material instance</param>
        /// <param name="copyFutureHolds">Indicates if merged material's future holds are to be copied into current Material instance</param>
        public static MergeMaterialsOutput Merge(this Material instance, Material materialToMerge, bool copyFutureHolds = false)
        {
            MergeMaterialsOutput returnObj = instance.Merge(new MaterialCollection() { materialToMerge }, copyFutureHolds: copyFutureHolds);
            return returnObj;
        }

        /// <summary>
        /// Merges a given material collection into the current material
        /// </summary>
        /// <param name="instance">material into which materialsToMerge will be merged</param>
        /// <param name="materialsToMerge">Materials to be merged into current Material instance</param>
        /// <param name="copyFutureHolds">Indicates if merged materials' future holds are to be copied into current Material instance</param>
        public static MergeMaterialsOutput Merge(this Material instance, MaterialCollection materialsToMerge, bool copyFutureHolds = false)
        {
            MergeMaterialsOutput returnObj = null;

            if (instance != null && materialsToMerge != null && materialsToMerge.Count > 0)
            {
                // prepare merge dictionary
                Dictionary<Material, MergeMaterialParameters> mergeParams = materialsToMerge.ToDictionary(E => E, E => new MergeMaterialParameters() { new MergeMaterialParameter() { SubMaterial = E } });
                returnObj = instance.Merge(mergeParams);
            }

            return returnObj;
        }

        /// <summary>
        /// Merges a given material collection into the current material
        /// </summary>
        /// <param name="instance">material into which materialsToMerge will be merged</param>
        /// <param name="mergeSettings">Merge materials and settings to be applied</param>
        /// <param name="copyFutureHolds">Indicates if merged materials' future holds are to be copied into current Material instance</param>
        public static MergeMaterialsOutput Merge(this Material instance, Dictionary<Material, MergeMaterialParameters> mergeSettings, bool copyFutureHolds = false)
        {
            MergeMaterialsOutput returnObj = null;

            // only proceed if actually eligible
            if (instance != null && mergeSettings != null && mergeSettings.Count > 0)
            {
                // perform merge
                returnObj = new MergeMaterialsInput()
                {
                    MainMaterial = instance
                    ,
                    ChildMaterials = mergeSettings
                    ,
                    ToCopyFutureHolds = copyFutureHolds
                }.MergeMaterialsSync();

                // refresh current instance with service output
                instance.CopyFrom(returnObj.Material);
            }

            return returnObj;
        }

        /// <summary>
        /// Expands a material into a set of submaterials based on primary quantity
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="expandForm"></param>
        public static void Expand(this Material instance, string expandForm)
        {
            if (instance != null && instance.Exists() && instance.PrimaryQuantity > 0)
            {
                int numberOfSubmaterials = (int)Math.Ceiling(instance.PrimaryQuantity.GetValueOrDefault());
                decimal? secondaryQuantityPerChild = instance.SecondaryQuantity != null ? instance.SecondaryQuantity / numberOfSubmaterials : null;

                // create n sub materials
                MaterialCollection subMaterials = new MaterialCollection();
                for (int iPos = 1; iPos <= numberOfSubmaterials; iPos++)
                {
                    subMaterials.Add(new Material()
                    {
                        Name = String.Format("{0}-{1}", instance.Name, iPos.ToString().PadLeft(2, '0'))
                        ,
                        PrimaryQuantity = 1
                        ,
                        SecondaryQuantity = secondaryQuantityPerChild
                    });


                }

                ExpandMaterialOutput expandMaterialOutput = new ExpandMaterialInput()
                {
                    Material = instance
                    ,
                    SubMaterials = subMaterials
                    ,
                    Form = expandForm
                }.ExpandMaterialSync();

                // perform expand
                instance.CopyFrom(expandMaterialOutput.Material);

                instance.SubMaterials = expandMaterialOutput.ExpandedSubMaterials;
            }
        }

        /// <summary>
        /// Collapse  submaterials into the main material
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="collapseForm"></param>
        public static void CollapseAll(this Material instance)
        {
            if (instance != null && instance.Exists())
            {
                // perform collapseAll
                instance.CopyFrom(new CollapseAllSubMaterialsInput()
                {
                    Material = instance
                }.CollapseAllSubMaterialsSync().Material);
            }
        }

        /*
        /// <summary>
        /// Loads a given Material's children
        /// </summary>
        /// <param name="instance">Material instance for which children will be loaded</param>
        /// <param name="levelsToLoad">Levels to load</param>
        /// <returns>Current instance submaterial collecton</returns>
        public static MaterialCollection LoadChildren(this Material instance, int levelsToLoad = 1)
        {
            MaterialCollection subMaterialsLoaded = null;

            if (instance != null)
            {
                instance.CopyFrom(new LoadMaterialChildrenInput() { Material = instance, LevelsToLoad = levelsToLoad }.LoadMaterialChildrenSync().Material);
                subMaterialsLoaded = instance.SubMaterials;
            }

            return subMaterialsLoaded;
        }
        */
        /// <summary>
        /// Attaches provided collection of materials to current Material instance
        /// </summary>
        /// <param name="instance">Current Material instance to which materials will be attached</param>
        /// <param name="materialsToAttach">Materials to be attached to the current instance</param>
        /// <param name="copyFutureHolds">Indication on whether or not Future holds (if existing) are to be copied into current material instance</param>
        public static void Attach(this Material instance, MaterialCollection materialsToAttach, bool copyFutureHolds = false)
        {
            if (instance != null && materialsToAttach != null && materialsToAttach.Count > 0)
            {
                // perform attach and refresh target material instance
                instance.CopyFrom(new Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects.AttachMaterialsInput()
                {
                    Material = instance
                    ,
                    SubMaterials = materialsToAttach
                    ,
                    ToCopyFutureHolds = copyFutureHolds
                }.AttachMaterialsSync().Material);

                // reload materials to be attached
                materialsToAttach.Load();
            }
        }

        /// <summary>
        /// Detatches a given set of materials from the current Material instance
        /// </summary>
        /// <param name="instance">Material instance from which materials will be detached.</param>
        /// <param name="materialsToDetach">Materials to be detached from current material instance</param>
        /// <param name="copyReworks">Indication on whether rework information is to be copied into detached materials. Defaults to false</param>
        /// <param name="copyFutureHolds">Indication on whether future holds are to be copied into detached materials. Defaults to false</param>
        /// <param name="terminateParentMaterial">Indication on whether parent material is to be terminated in case of full detach. Defaults to false</param>
        public static void Detach(this Material instance, MaterialCollection materialsToDetach, bool copyReworks = false, bool copyFutureHolds = false, bool terminateParentMaterial = false)
        {
            // only proceed if current material instance is set and a list of materials to detach is valid
            if (instance != null && materialsToDetach != null && materialsToDetach.Count > 0)
            {
                // invoke detach service and refresh current instance
                instance.CopyFrom(new Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects.DetachMaterialsInput()
                {
                    Material = instance
                    ,
                    SubMaterialsToDetach = materialsToDetach
                    ,
                    CopyReworks = copyReworks
                    ,
                    ToCopyFutureHolds = copyFutureHolds
                    ,
                    TerminateParentMaterial = terminateParentMaterial
                }.DetachMaterialsSync().Material);

                // reload detached materials
                materialsToDetach.Load();
            }
        }
        /*
        /// <summary>
        /// Detatches all existing sub materials from the current Material instance
        /// </summary>
        /// <param name="instance">Material instance from which all materials will be detached.</param>
        /// <param name="copyReworks">Indication on whether rework information is to be copied into detached materials. Defaults to false</param
        /// <param name="terminateParentMaterial">Indication on whether parent material is to be terminated in case of full detach. Defaults to false</param>
        /// <returns>All detached materials</returns>
        public static MaterialCollection DetachAll(this Material instance, bool copyReworks = false, bool terminateParentMaterial = false)
        {
            MaterialCollection detachedMaterials = null;

            // only proceed if current instance is not null
            if (instance != null)
            {
                // get all materials to be detached
                detachedMaterials = instance.LoadChildren();

                // if any materials found, then proceed with the detach
                if (detachedMaterials != null && detachedMaterials.Count > 0)
                {
                    // perform detach and update current instance
                    instance.CopyFrom(new Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects.DetachAllMaterialsInput()
                    {
                        Material = instance
                        ,
                        CopyReworks = copyReworks
                        ,
                        TerminateParentMaterial = terminateParentMaterial
                    }.DetachAllMaterialsSync().Material);

                    // refresh detached objects so they are returned updated.
                    detachedMaterials.Load();
                }
            }

            // return empty list if nothing done
            detachedMaterials = detachedMaterials ?? new MaterialCollection();

            return detachedMaterials;
        }
        */
        /// <summary>
        /// Transfer a given set of materials into the current Material instance
        /// </summary>
        /// <param name="instance">Material instance to which sub materials will be transfered</param>
        /// <param name="subMaterialsToTransfer">Sub Materials to be transfered</param>
        public static void Transfer(this Material instance, MaterialCollection subMaterialsToTransfer)
        {
            // only proceed if instance is not null and list of materials to transfer has elements
            if (instance != null && subMaterialsToTransfer != null && subMaterialsToTransfer.Count > 0)
            {
                // invoke transfer service and refresh current material instance
                instance.CopyFrom(new Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects.TransferSubMaterialsInput()
                {
                    NewParentMaterial = instance
                    ,
                    SubMaterials = subMaterialsToTransfer
                }.TransferSubMaterialsSync().NewParentMaterial);

                // reload sub materials transfered
                subMaterialsToTransfer.Load();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="splitSettings"></param>
        /// <param name="serviceComments"></param>
        /// <returns></returns>
        public static MaterialCollection SplitMaterial(this Material instance, SplitInputParametersCollection splitSettings, String serviceComments = null, bool updateInstance = false)
        {
            SplitMaterialOutput splitMaterialOutput = new SplitMaterialInput()
            {
                Material = instance,
                ServiceComments = serviceComments ?? "Split performed by test",
                ChildMaterials = splitSettings
            }.SplitMaterialSync();

            if (updateInstance)
                instance.CopyFrom(splitMaterialOutput.Material);

            return splitMaterialOutput.ChildMaterials;
        }

        #endregion Genealogy


        /// <summary>
        /// Retrieves list of possible resources for dispatch of current material instance
        /// </summary>
        /// <param name="instance">Material instance for which resource dispatch list will be provided...</param>
        /// <param name="dispatchType">Type of dispatch to be made. Defaults to ProcessingType = Process</param>
        /// <returns>Resource Collection of possible resources for dispatch</returns>
        public static ResourceCollection GetDispatchList(this Material instance, ProcessingType dispatchType = ProcessingType.Process)
        {
            ResourceCollection possibleResources = null;

            // only proceed if instance is set.
            if (instance != null)
            {
                // invoke service and assign result to return variable
                possibleResources = new GetDispatchListForMaterialInput()
                {
                    Material = instance
                    ,
                    DispatchType = ProcessingType.Process
                }.GetDispatchListForMaterialSync().Resources;

            }

            return possibleResources;
        }

        /// <summary>
        /// Change materials flow and step
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="flow"></param>
        /// <param name="step"></param>
        /// <param name="flowPath"></param>
        public static void ChangeFlowAndStep(this Material instance, Flow flow, Step step, String flowPath)
        {
            if (((instance.Step != null && step != null && instance.Step.Name != step.Name) || (instance.Flow != null && flow != null && instance.Flow.Name != flow.Name) || instance.FlowPath != flowPath))
            {
                instance.CopyFrom(new ChangeMaterialFlowAndStepInput()
                {
                    Flow = flow,
                    FlowPath = flowPath,
                    Material = instance,
                    Step = step
                }.ChangeMaterialFlowAndStepSync().Material);
            }
        }

        #region Shipping

        /// <summary>
        /// Ships a material to the provided facility
        /// </summary>
        /// <param name="instance">Material instance to ship</param>
        /// <param name="destinationFacility">Facility to which material is to be shipped</param>
        /// <param name="releaseContainers">Indication on whether current containers (if existing) are to be disassociated from shipped materials</param>
        /// <param name="shippingLabel">Shipping label</param>
        public static void Ship(this Material instance, Facility destinationFacility, bool releaseContainers = false, string shippingLabel = null)
        {
            if (instance != null && destinationFacility != null)
            {

                MaterialCollection shippedMaterials = null;

                if (destinationFacility.Site != null)
                {
                    shippedMaterials = new RemoteShipMaterialsInput()
                    {
                        Materials = new MaterialCollection() { instance }
                        ,
                        DestinationFacility = destinationFacility
                        ,
                        ReleaseContainers = releaseContainers
                        ,
                        ShippingLabel = shippingLabel
                    }.RemoteShipMaterialsSync().Materials;
                }
                else
                {
                    shippedMaterials = new ShipMaterialsInput()
                    {
                        Materials = new MaterialCollection() { instance }
                        ,
                        DestinationFacility = destinationFacility
                        ,
                        ReleaseContainers = releaseContainers
                        ,
                        ShippingLabel = shippingLabel
                    }.ShipMaterialsSync().Materials;
                }

                if (shippedMaterials != null)
                {
                    Material shippedMaterial = shippedMaterials.FirstOrDefault(E => E.Id == instance.Id);
                    if (shippedMaterials != null)
                    {
                        shippedMaterial = shippedMaterials.FirstOrDefault(E => E.Id == instance.Id);
                        if (shippedMaterial != null)
                            instance.CopyFrom(shippedMaterial);
                    }
                }
            }
        }

        /// <summary>
        /// Ships a material to the provided facility
        /// </summary>
        /// <param name="instance">Material instance to ship</param>
        /// <param name="destinationFacility">Facility to which material is to be shipped</param>
        /// <param name="releaseContainers">Indication on whether current containers (if existing) are to be disassociated from shipped materials</param>
        /// <param name="shippingLabel">Shipping label</param>
        public static void Ship(this MaterialCollection instance, Facility destinationFacility, bool releaseContainers = false, string shippingLabel = null)
        {
            if (instance != null && destinationFacility != null)
            {

                MaterialCollection shippedMaterials = null;

                if (destinationFacility.Site != null)
                {
                    shippedMaterials = new RemoteShipMaterialsInput()
                    {
                        Materials = instance
                        ,
                        DestinationFacility = destinationFacility
                        ,
                        ReleaseContainers = releaseContainers
                        ,
                        ShippingLabel = shippingLabel
                    }.RemoteShipMaterialsSync().Materials;
                }
                else
                {
                    shippedMaterials = new ShipMaterialsInput()
                    {
                        Materials = instance
                        ,
                        DestinationFacility = destinationFacility
                        ,
                        ReleaseContainers = releaseContainers
                        ,
                        ShippingLabel = shippingLabel
                    }.ShipMaterialsSync().Materials;
                }

                if (shippedMaterials != null && shippedMaterials.Count > 0)
                {
                    shippedMaterials.Load();
                }
            }
        }

        public static void UnShip(this Material instance)
        {
            if (instance != null)
            {
                MaterialCollection shippedMaterials = null;

                shippedMaterials = new UnshipMaterialsInput()
                {
                    Materials = new MaterialCollection() { instance }

                }.UnshipMaterialsSync().Materials;

                if (shippedMaterials != null)
                {
                    Material shippedMaterial = shippedMaterials.FirstOrDefault(E => E.Id == instance.Id);
                    if (shippedMaterials != null)
                    {
                        shippedMaterial = shippedMaterials.FirstOrDefault(E => E.Id == instance.Id);
                        if (shippedMaterial != null)
                            instance.CopyFrom(shippedMaterial);
                    }
                }
            }
        }


        #endregion Shipping

        #region Dispatch & Track In

        /// <summary>
        /// Performs a complex dispatch and track in of a given material instance based on provided parameters
        /// </summary>
        /// <param name="instance">Current material instance to be dispatched and tracked in</param>
        /// <param name="resource">Resource to which current material instance is to be dispatched and tracked in</param>
        /// <param name="dataCollectionPointsXml">Data Collection Points to post, if existing, in XML format</param>
        /// <param name="recipe">Recipe to use, if defined</param>
        /// <param name="setStateModelTransition">Indicates if a state change should be performed based on default transitions from the current state and operation being made</param>
        /// <param name="checkListParameters">check list parameters completed information, if existing</param>
        /// <param name="skipDCValidation">Indicates if DataCollection validations are to be skipped</param>
        /// <param name="serviceComments">Optional service commments</param>
        public static void ComplexDispatchAndTrackIn(this Material instance, Resource resource, String dataCollectionPointsXml = null, Recipe recipe = null, Boolean setStateModelTransition = true, PerformImmediateInputParametersCollection checkListParameters = null, Boolean skipDCValidation = false, String serviceComments = null)
        {
            // load resource if set and not yet loaded
            resource.SpecialLoad();

            // load current material step if not yet done
            instance.Step.SpecialLoad();

            #region Check for necessary state model changes

            StateModel stateModel = null;
            StateModelTransition stateModelTransition = null;

            // get default transition if set for transition change
            if (setStateModelTransition && resource.CurrentMainState != null && resource.CurrentMainState.CurrentState != null)
            {
                if (String.Equals(resource.CurrentMainState.CurrentState.Name.ToLower(), Constants.ResourceDefaultStateModelStandby.ToLower(), StringComparison.InvariantCultureIgnoreCase))
                {
                    GetDataForTrackInOperation operation = GetDataForTrackInOperation.DispatchAndTrackIn;
                    if (instance.SystemState == MaterialSystemState.Dispatched)
                        operation = GetDataForTrackInOperation.TrackIn;

                    ResourcePossibleTransitions transitions = new GetDataForTrackInWizardInput() { Resource = resource, Material = instance, Operation = operation }.GetDataForTrackInWizardSync().ResourcePossibleTransitions;

                    if (transitions != null)
                    {
                        stateModel = resource.CurrentMainState.StateModel;
                        stateModelTransition = transitions.DefaultTransition;
                    }
                }
            }

            #endregion

            // use collection service
            MaterialCollection mats = new MaterialCollection();
            mats.Add(instance);

            resource.Load();
            ComplexDispatchAndTrackInMaterialsInput dispatchInput = new ComplexDispatchAndTrackInMaterialsInput()
            {
                MaterialCollection = new Dictionary<Material, DispatchMaterialParameters>()
                 {
                     {  instance, new DispatchMaterialParameters() { Resource = resource } }
                 }
                 ,
                DataCollectionPointsXml = new Dictionary<Material, string>() { { instance, dataCollectionPointsXml } }
                 ,
                Recipe = recipe
                 ,
                StateModel = stateModel
                 ,
                StateModelTransition = stateModelTransition
                 ,
                ChecklistParameters = new Dictionary<Material, PerformImmediateInputParametersCollection>() { { instance, checkListParameters } }
                 ,
                SkipDCValidation = skipDCValidation
                 ,
                ServiceComments = serviceComments
            };


            ComplexDispatchAndTrackInMaterialsOutput output = null;

            EIUtilities.TestUtilities.WaitFor(30, String.Format("Error on ComplexDispatchAndTrackInMaterialsSync"), () =>
            {
                try
                {
                    dispatchInput.MaterialCollection[instance].Resource.Load();
                    output = dispatchInput.ComplexDispatchAndTrackInMaterialsSync();
                    return true;
                }
                catch
                {
                    return false;
                }


            });


            instance.CopyFrom(output.Materials.First());
            resource.CopyFrom(output.Resource);

        }

        /// <summary>
        /// Performs a complex trackin  of a given material instance
        /// </summary>
        /// <param name="instance">Current material instance to be tracked in</param>
        /// <param name="resource">Resource to which current material instance is to be tracked in</param>
        /// <param name="recipe">Recipe to use, if defined</param>
        /// <param name="recipeParameterValues">Recipe parameter values to use, if defined</param>
        /// <param name="dataCollectionPointsXml">Data Collection Points to be posted in xml format, if defined</param>
        /// <param name="currentDataCollectionPoints">Data Collection points in object type format</param>
        /// <param name="skipDCValidation">Indication on whether Data Collection validations should be skipped</param>
        /// <param name="setStateModelTransition">Resource state model transition to be used if defined. If not, will be automatically determined.</param>
        /// <param name="serviceComments">Optional service comments to be stored with the service execution</param>
        public static void ComplexTrackIn(this Material instance
                                                , Resource resource = null
                                                , Recipe recipe = null
                                                , Dictionary<Parameter, Object> recipeParameterValues = null
                                                , String dataCollectionPointsXml = null
                                                , DataCollectionPointCollection currentDataCollectionPoints = null
                                                , PerformImmediateInputParametersCollection checklistParameters = null
                                                , Boolean skipDCValidation = false
                                                , Boolean setStateModelTransition = true
                                                , String serviceComments = null
                                                , Dictionary<String, Object> operationParameters = null)
        {
            Boolean isResourceDefined = (resource != null);

            #region Check for necessary state model changes

            StateModel stateModel = null;
            StateModelTransition stateModelTransition = null;

            if (resource != null)
            {
                resource.SpecialLoad();
                instance.Step.SpecialLoad();

                if (String.Equals(resource.CurrentMainState.CurrentState.Name, Constants.ResourceDefaultStateModelStandby.ToLower(), StringComparison.InvariantCultureIgnoreCase))
                {
                    GetDataForTrackInOperation operation = GetDataForTrackInOperation.DispatchAndTrackIn;
                    if (instance.SystemState == MaterialSystemState.Dispatched)
                        operation = GetDataForTrackInOperation.TrackIn;

                    ResourcePossibleTransitions transitions = new GetDataForTrackInWizardInput() { Resource = resource, Material = instance, Operation = operation }.GetDataForTrackInWizardSync().ResourcePossibleTransitions;

                    if (transitions != null)
                    {
                        stateModel = resource.CurrentMainState.StateModel;
                        stateModelTransition = transitions.DefaultTransition;
                    }
                }
            }



            #endregion
            resource.Load();
            ComplexTrackInMaterialInput input = new ComplexTrackInMaterialInput()
            {
                Material = instance
                ,
                Resource = resource
                ,
                Recipe = recipe
                ,
                RecipeParameterValues = recipeParameterValues
                ,
                DataCollectionPointsXml = dataCollectionPointsXml
                ,
                CurrentDataCollectionPoints = currentDataCollectionPoints
                ,
                ChecklistParameters = checklistParameters
                ,
                StateModel = stateModel
                ,
                StateModelTransition = stateModelTransition
                ,
                SkipDCValidation = skipDCValidation
                ,
                ServiceComments = serviceComments
            };



            ComplexTrackInMaterialOutput output = null;

            EIUtilities.TestUtilities.WaitFor(30, String.Format("Error on ComplexTrackInMaterialSync"), () =>
            {
                try
                {
                    input.Resource.Load();
                    output = input.ComplexTrackInMaterialSync();
                    return true;
                }
                catch
                {
                    return false;
                }


            });





            instance.CopyFrom(output.Material);

            if (isResourceDefined) resource.CopyFrom(output.Resource);
        }


        /// <summary>
        /// Dispatches a given material to a given resource
        /// </summary>
        /// <param name="instance">Material to be dispatched</param>
        /// <param name="resource">Resource into which material will be dispatched...</param>
        /// <param name="serviceComments">Optional service comments</param>
        /// <returns>Feedback Messages</returns>
        public static Collection<FeedbackMessage> Dispatch(this Material instance, Resource resource, String serviceComments = null)
        {
            // only proceed if material is set
            if (instance != null)
            {
                resource.Load();
                bool resourceSet = (resource != null);

                DispatchMaterialsInput input = new DispatchMaterialsInput()
                {
                    IgnoreLastServiceId = true,
                    Materials = new Dictionary<Material, DispatchMaterialParameters>()
                    {
                        { instance, new DispatchMaterialParameters() { Resource = resource } }
                    }
                    ,
                    ServiceComments = serviceComments
                };




                DispatchMaterialsOutput output = null;

                EIUtilities.TestUtilities.WaitFor(30, String.Format("Error on Dispacth"), () =>
                {
                    try
                    {
                        input.Materials[instance].Resource.Load();
                        output = input.DispatchMaterialsSync();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }


                });




                instance.CopyFrom(output.Materials.First());

                if (resourceSet)
                {
                    resource.Load();
                    resource.CopyFrom(output.Resources.First());
                }

                return output.FeedbackMessages;
            }

            return null;
        }

        /// <summary>
        /// Undispatches a given material
        /// </summary>
        /// <param name="instance">Material to be undispatched</param>
        /// <param name="serviceComments">Optional service comments</param>
        public static void Undispatch(this Material instance, String serviceComments = null)
        {
            if (instance != null)
            {
                MaterialCollection materials = new MaterialCollection();

                materials.Add(instance);

                materials.Undispatch();
            }
        }

        /// <summary>
        /// Undispatches materials
        /// </summary>
        /// <param name="instance">Materials to be undispatched</param>
        /// <param name="serviceComments">Optional service comments</param>
        public static void Undispatch(this MaterialCollection instance, String serviceComments = null)
        {
            // only proceed if material is set
            if (instance != null && instance.Count > 0)
            {
                UndispatchMaterialsOutput res = new UndispatchMaterialsInput()
                {
                    Materials = instance,
                    ServiceComments = serviceComments
                }.UndispatchMaterialsSync();
            }
        }

        #endregion Material Management Services

        #region TrackOut and Change of Step

        /// <summary>
        /// Moves material to next step
        /// </summary>
        /// <param name="instance">Material instance being moved to the next step</param>
        /// <param name="serviceComments">Service comments to add to the service execution</param>
        public static void ComplexMoveNext(this Material instance, String serviceComments = null)
        {
            // only proceed if instance is set
            if (instance != null)
            {

                // load material if not yet loaded
                instance.SpecialLoad();

                // Load step info if not yet loaded
                instance.Step.SpecialLoad();

                // If material is not Processed, this should not do anything
                if (instance.SystemState != MaterialSystemState.Processed && !instance.Step.IsPassThrough.Value) return;

                // get next steps for material
                var nextSteps = new GetNextStepsForMaterialInput() { Material = instance }.GetNextStepsForMaterialSync().NextStepsResults;
                if (nextSteps == null || nextSteps.Count == 0)
                {
                    throw new Exception("No possible next steps for material after step " + instance.Step.Name);
                }

                // perform move next
                instance.CopyFrom(new ComplexMoveMaterialToNextStepInput()
                {
                    Material = instance
                    ,
                    FlowPath = nextSteps[0].FlowPath
                    ,
                    ServiceComments = serviceComments
                }.ComplexMoveMaterialToNextStepSync().Material);
            }
        }

        public static void ComplexMoveNext(this MaterialCollection instances, String flowPath = null, String serviceComments = null)
        {
            // only proceed if instance is set
            if (instances != null && instances.Count > 0)
            {
                // load materials if not yet loaded
                foreach (Material instance in instances)
                {
                    // load material if not yet loaded
                    instance.SpecialLoad();
                }

                // initialize incoming materials dictionary
                Dictionary<long, Material> incomingMaterials = instances.ToDictionary(E => E.Id, E => E);

                // prepare move next settings dictionary per material
                Dictionary<long, string> materialSettings = instances.ToDictionary(E => E.Id, E => flowPath);

                // if no flowpath provided, obtain it from first material instance
                if (String.IsNullOrWhiteSpace(flowPath))
                {
                    foreach (Material instance in instances)
                    {
                        // Load step info if not yet loaded
                        instance.Step.SpecialLoad();

                        // If material is not Processed, this should not do anything
                        if (instance.SystemState != MaterialSystemState.Processed && !instance.Step.IsPassThrough.Value) continue;

                        // get next steps for material
                        var nextSteps = new GetNextStepsForMaterialInput() { Material = instance }.GetNextStepsForMaterialSync().NextStepsResults;
                        if (nextSteps == null || nextSteps.Count == 0)
                        {
                            throw new Exception("No possible next steps for material after step " + instance.Step.Name);
                        }

                        materialSettings[instance.Id] = nextSteps[0].FlowPath;
                    }

                    // remove all materials whose instance does not have a flow path
                    materialSettings = materialSettings.Where(E => !String.IsNullOrWhiteSpace(E.Value)).ToDictionary(E => E.Key, E => E.Value);
                }

                // transform prepared dictionary with material objects
                Dictionary<Material, string> transformedDictionary = materialSettings.ToDictionary(E => incomingMaterials[E.Key], E => E.Value);

                // perform move next
                ComplexMoveMaterialsToNextStepOutput output = new ComplexMoveMaterialsToNextStepInput()
                {
                    Materials = transformedDictionary
                    ,
                    ServiceComments = serviceComments
                }.ComplexMoveMaterialsToNextStepSync();

                // refresh materials moved...
                foreach (Material outputMaterial in output.Materials)
                {
                    incomingMaterials[outputMaterial.Id].CopyFrom(outputMaterial);
                }
            }

        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="instance"></param>
        ///// <param name="flow"></param>
        ///// <param name="step"></param>
        ///// <param name="flowPath"></param>
        //public static void ChangeFlowAndStep(this Material instance, Flow flow, Step step)
        //{
        //    flow.SpecialLoad();
        //    step.SpecialLoad();

        //    ChangeMaterialFlowAndStepInput changeMaterialFlowStepInput = new ChangeMaterialFlowAndStepInput();
        //    changeMaterialFlowStepInput.Material = instance;
        //    changeMaterialFlowStepInput.FlowPath = flow.GetStepFlowPath(step);
        //    changeMaterialFlowStepInput.Flow = flow;
        //    changeMaterialFlowStepInput.Step = step;

        //    instance.CopyFrom(changeMaterialFlowStepInput.ChangeMaterialFlowAndStepSync().Material);
        //}

        /// <summary>
        /// Performs a complex trackout on the provided material
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="materialGradeBinningQuantityCollection"></param>
        /// <param name="bonusReasons"></param>
        /// <param name="performImmediateInputParametersCollection"></param>
        /// <param name="currentDataCollectionPoints"></param>
        /// <param name="dataCollectionPointsXml"></param>
        /// <param name="lossReasons"></param>
        /// <param name="subMaterialLosses"></param>
        /// <param name="isMoveNext"></param>
        /// <param name="terminateOnZeroQuantity"></param>
        /// <param name="isPassScenario"></param>
        /// <param name="serviceComments"></param>
        /// <returns></returns>
        public static Material ComplexTrackOutMaterial(this Material instance
                                                        , MaterialGradeBinningQuantityCollection materialGradeBinningQuantityCollection = null
                                                        , LossBonusAffectedQuantityCollection bonusReasons = null
                                                        , PerformImmediateInputParametersCollection performImmediateInputParametersCollection = null
                                                        , DataCollectionPointCollection currentDataCollectionPoints = null
                                                        , String dataCollectionPointsXml = null
                                                        , LossBonusAffectedQuantityCollection lossReasons = null
                                                        , Dictionary<Material, LossBonusAffectedQuantityCollection> subMaterialLosses = null
                                                        , Boolean terminateOnZeroQuantity = false
                                                        , String serviceComments = null, bool updateInstance = true)
        {
            Material returnMaterial = null;

            // only makes sense to proceed if material instance is set
            if (instance != null)
            {

                // get last known resource
                Resource materialResource = instance.LastProcessedResource.SpecialLoad();

                // Determine state transition for resource
                StateModelTransitionSetting smtsTransition = materialResource.GetEligibleTransition(instance, "TrackOut") ?? new StateModelTransitionSetting();

                ComplexTrackOutMaterialsOutput output = new ComplexTrackOutMaterialsInput()
                {
                    Material = new Dictionary<Material, ComplexTrackOutParameters>()
                    {
                        { instance, new ComplexTrackOutParameters()
                                        {
                                             BinQuantities = materialGradeBinningQuantityCollection
                                             , BonusReasons = bonusReasons
                                             , ChecklistParameters = performImmediateInputParametersCollection
                                             , CurrentDataCollectionPoints = currentDataCollectionPoints
                                             , DataCollectionPointsXml = dataCollectionPointsXml
                                             , LossReasons = lossReasons
                                             , SubMaterialLosses = subMaterialLosses
                                             , TerminateOnZeroQuantity = terminateOnZeroQuantity
                                        }
                        }
                    }
                    ,
                    ServiceComments = serviceComments
                }.ComplexTrackOutMaterialsSync();

                if (updateInstance)
                {
                    instance.CopyFrom(output.Materials.First().Key);
                }

                returnMaterial = output.Materials.First().Key;




                //// prepare and invoke operation
                //ComplexTrackOutMaterialOutput output = new ComplexTrackOutMaterialInput()
                //{
                //    Material = instance,
                //    BinQuantities = materialGradeBinningQuantityCollection,
                //    BonusReasons = bonusReasons,
                //    ChecklistParameters = performImmediateInputParametersCollection,
                //    CurrentDataCollectionPoints = currentDataCollectionPoints,
                //    DataCollectionPointsXml = dataCollectionPointsXml,
                //    LossReasons = lossReasons,
                //    SubMaterialLosses = subMaterialLosses,
                //    StateModelTransition = smtsTransition.StateModelTransition,
                //    StateModel = smtsTransition.StateModelTransition == null ? null : smtsTransition.StateModel,
                //    StateModelTransitionReason = smtsTransition.StateModelTransition == null ? null : smtsTransition.StateModelTransition.ReasonDefaultValue,
                //    TerminateOnZeroQuantity = terminateOnZeroQuantity,

                //    ServiceComments = serviceComments
                //}.ComplexTrackOutMaterialSync();

                // update material instance
                //if (updateInstance)
                //{
                //    instance.CopyFrom(output.Material);
                //}

                //returnMaterial = output.Material;
            }

            return returnMaterial;
        }


        #endregion TrackOut and Change of Step

        #region Time Constraints

        /// <summary>
        /// Retrieves all time constraints found for the material in question and with the provided filters
        /// </summary>
        /// <param name="instance">Material instance for which the time constraints will be retrieved...</param>
        /// <param name="fromStep">starting step in the constraint. defaults to null</param>
        /// <param name="toStep">ending step in the constraint. defaults to null</param>
        /// <returns>MaterialTimeConstraints found for the material and given conditions</returns>
        public static MaterialTimeConstraintCollection GetMaterialTimeConstraints(this Material instance, String fromStep = null, String toStep = null)
        {
            MaterialTimeConstraintCollection matTCs = new MaterialTimeConstraintCollection();

            if (instance.SpecialLoad() != null)
            {

                QueryObject outputQuery = new QueryObject();
                outputQuery.EntityTypeName = "MaterialTimeConstraint";
                outputQuery.Name = "GetMatTCs";
                outputQuery.Query = new Query();
                outputQuery.Query.EntityFilter = new EntityFilterCollection(){
                   new EntityFilter()
                   {
                         Alias = "MaterialTimeConstraint_1",
                         EntityType = "MaterialTimeConstraint",
                         Filter = new FilterCollection() {
                                      new Filter()
                                      {
                                             Name = "UniversalState",
                                             Operator = Cmf.Foundation.Common.FieldOperator.IsEqualTo,
                                             Value = 2,
                                             LogicalOperator = Cmf.Foundation.Common.LogicalOperator.AND
                                      }
                                },
                                Properties = new FieldCollection()
                                {
                                      new Field()
                                      {
                                             Alias = "Id",
                                             IsUserAttribute = false,
                                             Name = "Id",
                                             Position = 0,
                                             Sort = Cmf.Foundation.Common.FieldSort.NoSort
                                      }
                                }
                   },
                   new EntityFilter()
                   {
                         Alias = "MaterialTimeConstraint_Material_2",
                         EntityType = "Material",
                         Filter = new FilterCollection() {
                                      new Filter()
                                      {
                                             Name = "Name",
                                             Operator = Cmf.Foundation.Common.FieldOperator.IsEqualTo,
                                             Value = instance.Name,
                                             LogicalOperator = Cmf.Foundation.Common.LogicalOperator.AND
                                      }
                                },
                                Properties = new FieldCollection()
                   }
            };
                if (toStep != null)
                {
                    outputQuery.Query.EntityFilter.Add(new EntityFilter()
                    {
                        Alias = "MaterialTimeConstraint_ToStep_2",
                        EntityType = "Step",
                        Filter = new FilterCollection() {
                                      new Filter()
                                      {
                                             Name = "Name",
                                             Operator = Cmf.Foundation.Common.FieldOperator.IsEqualTo,
                                             Value = toStep,
                                             LogicalOperator = Cmf.Foundation.Common.LogicalOperator.AND
                                      }
                                },
                        Properties = new FieldCollection()
                    });
                }
                if (fromStep != null)
                {
                    outputQuery.Query.EntityFilter.Add(new EntityFilter()
                    {
                        Alias = "MaterialTimeConstraint_FromStep_2",
                        EntityType = "Step",
                        Filter = new FilterCollection() {
                                    new Filter()
                                    {
                                            Name = "Name",
                                            Operator = Cmf.Foundation.Common.FieldOperator.IsEqualTo,
                                            Value = fromStep,
                                            LogicalOperator = Cmf.Foundation.Common.LogicalOperator.Nothing
                                    }
                            },
                        Properties = new FieldCollection()
                    });
                }

                outputQuery.Query.Relations = new RelationCollection() {
                   new Relation()
                   {
                         Alias = "",
                         Filter = new FilterCollection(),
                         Properties = new FieldCollection(),
                         IsRelation = false,
                         Name = "",
                         SourceEntity = "MaterialTimeConstraint",
                         SourceEntityAlias = "MaterialTimeConstraint_1",
                         SourceJoinType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.JoinType.InnerJoin,
                         SourceProperty = "MaterialId",
                         TargetEntity = "Material",
                         TargetEntityAlias = "MaterialTimeConstraint_Material_2",
                         TargetJoinType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.JoinType.InnerJoin,
                         TargetProperty = "Id"
                   } };

                if (toStep != null)
                {
                    outputQuery.Query.Relations.Add(
                        new Relation()
                        {
                            Alias = "",
                            Filter = new FilterCollection(),
                            Properties = new FieldCollection(),
                            IsRelation = false,
                            Name = "",
                            SourceEntity = "MaterialTimeConstraint",
                            SourceEntityAlias = "MaterialTimeConstraint_1",
                            SourceJoinType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.JoinType.InnerJoin,
                            SourceProperty = "ToStepId",
                            TargetEntity = "Step",
                            TargetEntityAlias = "MaterialTimeConstraint_ToStep_2",
                            TargetJoinType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.JoinType.InnerJoin,
                            TargetProperty = "Id"
                        }
                    );
                }

                if (fromStep != null)
                {
                    outputQuery.Query.Relations.Add(
                       new Relation()
                       {
                           Alias = "",
                           Filter = new FilterCollection(),
                           Properties = new FieldCollection(),
                           IsRelation = false,
                           Name = "",
                           SourceEntity = "MaterialTimeConstraint",
                           SourceEntityAlias = "MaterialTimeConstraint_1",
                           SourceJoinType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.JoinType.InnerJoin,
                           SourceProperty = "FromStepId",
                           TargetEntity = "Step",
                           TargetEntityAlias = "MaterialTimeConstraint_FromStep_2",
                           TargetJoinType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.JoinType.InnerJoin,
                           TargetProperty = "Id"
                       }
                    );
                }

                var executeInput = new ExecuteQueryInput();
                executeInput.QueryObject = outputQuery;
                ExecuteQueryOutput executeOutput = executeInput.ExecuteQuerySync();

                DataSet ds = Cmf.TestScenarios.Others.Utilities.ToDataSet(executeOutput.NgpDataSet);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        long id = (long)ds.Tables[0].Rows[i]["Id"];
                        MaterialTimeConstraint mtc = Extensions.GetEntity<MaterialTimeConstraint>(id, 1);
                        matTCs.Add(mtc);
                    }
                }

            }

            return matTCs;
        }

        /// <summary>
        /// Manages material time constraints for a given material
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="materialTimeConstraintsToAdd"></param>
        /// <param name="materialTimeConstraintsToUpdate"></param>
        /// <param name="materialTimeConstraintsToRemove"></param>
        public static void ManageMaterialTimeConstrains(this Material instance
                                                                , MaterialTimeConstraintCollection materialTimeConstraintsToAdd
                                                                , MaterialTimeConstraintCollection materialTimeConstraintsToUpdate
                                                                , MaterialTimeConstraintCollection materialTimeConstraintsToRemove)
        {
            instance.CopyFrom(new ManageMaterialTimeConstraintsInput()
            {
                Material = instance
                ,
                MaterialTimeConstraintsToAdd = materialTimeConstraintsToAdd
                ,
                MaterialTimeConstraintsToUpdate = materialTimeConstraintsToUpdate
                ,
                MaterialTimeConstraintsToRemove = materialTimeConstraintsToRemove
            }.ManageMaterialTimeConstraintsSync().Material);
        }

        #endregion

        #region Hold & Release

        /// <summary>
        /// Releases a given material from the provided material hold reasons. if none provided, will release them all
        /// </summary>
        /// <param name="instance">Material instance subject to release</param>
        /// <param name="reasonsToRelease">Material hold reasons to remove. if none provided, will release them all</param>
        /// <returns>Updated material instance</returns>
        public static Material Release(this Material instance, MaterialHoldReasonCollection reasonsToRelease = null)
        {
            if (reasonsToRelease == null)
            {
                instance.LoadRelation("MaterialHoldReason");
                if (instance.RelationCollection.ContainsKey("MaterialHoldReason"))
                {
                    reasonsToRelease = new MaterialHoldReasonCollection();
                    reasonsToRelease.AddRange(instance.RelationCollection["MaterialHoldReason"].Select(E => E as MaterialHoldReason));
                }

                instance.CopyFrom(new ReleaseMaterialInput() { MaterialHoldReasonCollection = reasonsToRelease, Material = instance }.ReleaseMaterialSync().Material);
            }

            return instance;
        }


        /// <summary>
		/// Performs a complete release if material is on hold
		/// </summary>
		/// <param name="instance"></param>
		public static void CompleteRelease(this Material instance)
        {
            if (instance.HoldCount > 0)
            {
                instance.LoadRelation("MaterialHoldReason");

                instance.CopyFrom(new ReleaseMaterialInput()
                {
                    Material = instance,
                    MaterialHoldReasonCollection = instance.MaterialHoldReasons
                }.ReleaseMaterialSync().Material);
            }
        }

        #endregion Hold & Release

        #region Change Material Type

        public static void ChangeMaterialType(this Material instance, string materialType)
        {
            instance.Type = materialType;

            instance = new ChangeMaterialTypeInput
            {
                ApplyToAllSubMaterials = true,
                Material = instance
            }.ChangeMaterialTypeSync().Material;

        }

        #endregion

        ///// <summary>
        ///// Perform Operation All the Checklist Items of a material
        ///// </summary>
        ///// <param name="material">Material</param>
        ///// <param name="checklistOperation">Checklist Operation</param>
        //public static void PerformMaterialChecklistItem(this Material material, PerformChecklistOperation checklistOperation)
        //{
        //    if (material.CurrentChecklistInstance != null)
        //    {
        //        material.CurrentChecklistInstance = TestUtilities.TestUtilities.LoadChecklistInstanceItems(material.CurrentChecklistInstance);

        //        List<PerformMaterialChecklistItemInput> checklistItemInputs = new List<PerformMaterialChecklistItemInput>();

        //        foreach (var item in material.CurrentChecklistInstance.Items)
        //        {
        //            checklistItemInputs.Add(new PerformMaterialChecklistItemInput()
        //            {
        //                Material = material,
        //                ChecklistItemInstance = item,
        //                PerformChecklistOperation = checklistOperation
        //            });
        //        }

        //        PerformMaterialChecklistItemsOutput output = new PerformMaterialChecklistItemsInput()
        //        {
        //            ItemsAndParameters = checklistItemInputs
        //        }.PerformMaterialChecklistItemsSync();

        //        material.CopyFrom(output.Material);
        //    }
        //}

        #region Change Material Quantity

        /// <summary>
        /// Change Material Quantity
        /// </summary>
        /// <param name="instance">Material to change</param>
        /// <param name="primaryQuantity">New Primary quantity</param>
        /// <param name="secondaryQuantity">New Secondary quantity</param>
        public static void ChangeMaterialQuantity(this Material instance, decimal primaryQuantity, decimal secondaryQuantity)
        {
            MaterialQuantityChange materialQuantity = new MaterialQuantityChange()
            {
                Material = instance,
                NewPrimaryQuantity = primaryQuantity,
                NewSecondaryQuantity = secondaryQuantity
            };

            instance.CopyFrom(new ChangeMaterialQuantityInput
            {
                MaterialQuantityChange = materialQuantity
            }.ChangeMaterialQuantitySync().Material);
        }

        #endregion

        #endregion

        #region Custom

        /// <summary>
        /// Retrieves the current leaf flow for a given material instance
        /// </summary>
        /// <param name="instance">material instance for which the flow will be retrieved</param>
        /// <returns></returns>
        public static Flow GetCurrentFlow(this Material instance)
        {
            Flow returnFlow = null;

            if (instance != null && !String.IsNullOrEmpty(instance.FlowPath))
            {
                string[] splitTokens = instance.FlowPath.Split('/');
                if (splitTokens.Length >= 2)
                {
                    string[] lastFlowTokens = splitTokens[splitTokens.Length - 2].Split(':');
                    if (lastFlowTokens.Length == 2)
                    {
                        string flowName = lastFlowTokens[0];

                        returnFlow = new Flow();
                        returnFlow.Load(flowName);
                    }
                }
            }

            return returnFlow;
        }

        ///// <summary>
        ///// Retrieves the current material's leaf flow ERP operation code
        ///// </summary>
        ///// <param name="instance"></param>
        ///// <returns></returns>
        //public static string GetCurrentERPOperationCode(this Material instance)
        //{
        //    string returnValue = null;

        //    if (instance != null)
        //    {
        //        Flow currentFlow = instance.GetCurrentFlow();
        //        if (currentFlow != null)
        //        {
        //            returnValue = currentFlow.GetERPOperationCode();
        //        }
        //    }

        //    return returnValue;
        //}

        /// <summary>
        /// Puts a given material in the first non pass through step in the current flow.
        /// </summary>
        /// <param name="instance">Material instance being moved to the next non pass through step</param>
        public static void PutInNextProcessStep(this Material instance)
        {
            if (instance != null)
            {
                while (instance.Step.SpecialLoad().IsPassThrough.GetValueOrDefault())
                {
                    instance.ComplexMoveNext();
                }
            }
        }

        /// <summary>
        /// Tries to provide an eligible resource to which material can be dispatched
        /// </summary>
        /// <param name="instance">Material instance for which and eligible resource will be retrieved</param>
        /// <returns>First eligible resource if found</returns>
        public static Resource GetResource(this Material instance)
        {
            Resource returnObject = null;

            if (instance.SpecialLoad() != null)
            {
                ResourceCollection res = instance.GetDispatchList();
                if (res != null)
                {
                    returnObject = res.FirstOrDefault();
                }
            }

            return returnObject;
        }

        /// <summary>
        /// Retrieves all possible next steps for a given material
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static NextStepsResultCollection GetNextStepsForMaterial(this Material instance)
        {
            NextStepsResultCollection returnObj = null;

            if (instance != null)
            {
                var result = new GetNextStepsForMaterialInput()
                {
                    Material = instance
                }.GetNextStepsForMaterialSync();

                if (result != null)
                    returnObj = result.NextStepsResults;
            }

            return returnObj;
        }

        /// <summary>
        /// Retrieves all possible next steps for a given material
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static NextStepsResultCollection GetDataForMultipleMoveNextWizard(this Material instance)
        {
            NextStepsResultCollection returnObj = null;

            if (instance != null)
            {
                returnObj = new MaterialCollection() { instance }.GetDataForMultipleMoveNextWizard();
            }

            return returnObj;
        }

        /// <summary>
        /// Retrieves all possible next steps for a given set of materials
        /// </summary>
        /// <param name="instances"></param>
        /// <returns></returns>
        public static NextStepsResultCollection GetDataForMultipleMoveNextWizard(this MaterialCollection instances)
        {
            NextStepsResultCollection returnObj = null;

            if (instances != null)
            {
                GetDataForMultipleMoveNextWizardOutput result = new GetDataForMultipleMoveNextWizardInput()
                {
                    Materials = instances
                }.GetDataForMultipleMoveNextWizardSync();

                if (result != null)
                    returnObj = result.NextStepsResults;
            }

            return returnObj;
        }


        ///// <summary>
        ///// Performs a complete unit invoking trackout service and passing number of units completed
        ///// </summary>
        ///// <param name="instance"></param>
        ///// <param name="quantityCompleted"></param>
        //public static Material CompleteUnit(this Material instance, decimal quantityCompleted)
        //{
        //    // return value that will hold completed unit material coming from service
        //    Material completedUnit = null;

        //    if (instance != null)
        //    {
        //        // prepare quantity completed datacollection
        //        DataCollectionPointCollection dataCollectionPoints = TestUtilities.TestUtilities.DataCollectionPointCollection_ForTrackOut(quantity: quantityCompleted);

        //        // invoke complex trackout
        //        completedUnit = instance.ComplexTrackOutMaterial(currentDataCollectionPoints: dataCollectionPoints, terminateOnZeroQuantity: true, updateInstance: false);
        //        instance.Load();
        //    }

        //    // return completed unit material
        //    return completedUnit;
        //}

        #endregion Custom

        /// <summary>
        /// Hold Material
        /// </summary>
        /// <param name="instance"></param>     
        /// <param name="materialHoldReasonCollection"></param>
        /// /// <param name="serviceComments"></param>
        public static void Hold(this Material instance, MaterialHoldReasonCollection materialHoldReasonCollection, String serviceComments = null)
        {
            instance.CopyFrom(
                new HoldMaterialInput()
                {
                    IgnoreLastServiceId = true,
                    Material = instance,
                    MaterialHoldReasonCollection = materialHoldReasonCollection,
                    ServiceComments = serviceComments
                }.HoldMaterialSync().Material);
        }

        /// <summary>
		/// 
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="serviceComments"></param>
		public static void Abort(this Material instance, String serviceComments = null)
        {
            instance.CopyFrom(
                new AbortMaterialProcessInput()
                {
                    IgnoreLastServiceId = true,
                    Material = instance,
                    ServiceComments = serviceComments
                }.AbortMaterialProcessSync().Material);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="serviceComments"></param>
        public static void TerminateMaterial(this Material instance, Reason reasonToUse = null, LossReasonClassification classifications = null, String serviceComments = null)
        {
            // check if a load is required...
            instance.SpecialLoad();

            // Determine step and loss reason to use...
            Step currentStep = instance.Step.SpecialLoad();

            // if no reason is given, pick first eligible loss found
            if (reasonToUse == null)
                reasonToUse = currentStep.GetRandomReason();

            if (reasonToUse == null)
                throw new Exception("Could not find suitable loss reason.");

            // determine loss reason classifications for the terminate
            if (classifications == null && (!String.IsNullOrEmpty(currentStep.LossClassificationName1)
                                            || !String.IsNullOrEmpty(currentStep.LossClassificationName2)
                                            || !String.IsNullOrEmpty(currentStep.LossClassificationName3)
                                            || !String.IsNullOrEmpty(currentStep.LossClassificationName4)))
            {
                string classification1 = null;
                string classification2 = null;
                string classification3 = null;
                string classification4 = null;

                if (currentStep.LossClassificationLookupTable1 != null)
                    classification1 = LookupTableScenario.GetLookupTableByName(currentStep.LossClassificationLookupTable1).Values[0].Value;
                if (currentStep.LossClassificationLookupTable2 != null)
                    classification2 = LookupTableScenario.GetLookupTableByName(currentStep.LossClassificationLookupTable2).Values[0].Value;
                if (currentStep.LossClassificationLookupTable3 != null)
                    classification3 = LookupTableScenario.GetLookupTableByName(currentStep.LossClassificationLookupTable3).Values[0].Value;
                if (currentStep.LossClassificationLookupTable4 != null)
                    classification4 = LookupTableScenario.GetLookupTableByName(currentStep.LossClassificationLookupTable4).Values[0].Value;


                classifications = new LossReasonClassification();
                classifications.LossClassificationName1 = currentStep.LossClassificationName1;
                classifications.LossClassificationName2 = currentStep.LossClassificationName2;
                classifications.LossClassificationName3 = currentStep.LossClassificationName3;
                classifications.LossClassificationName4 = currentStep.LossClassificationName4;
                classifications.LossClassificationValue1 = classification1;
                classifications.LossClassificationValue2 = classification2;
                classifications.LossClassificationValue3 = classification3;
                classifications.LossClassificationValue4 = classification4;
            }


            // perform terminate
            instance.CopyFrom(new TerminateMaterialInput()
            {
                Material = instance as Material
                ,
                LossReason = reasonToUse
                ,
                LossReasonClassification = classifications
                ,
                ServiceComments = serviceComments
            }.TerminateMaterialSync().Material);

        }

        /// <summary>
        /// Terminates all material relations except MaterialResource
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="relationName"></param>
        public static void TerminateAllMaterialRelations(this Material instance, string relationName = null)
        {
            List<string> lstRels = new List<string>();

            if (relationName == null)
            {
                instance.LoadRelations();
                lstRels = instance.RelationCollection.Select(x => x.Key).ToList();
            }
            else
            {
                lstRels = new List<string> { relationName };
            }

            foreach (string rel in lstRels)
            {
                if (rel.Equals(Constants.RelationMaterialResource) || rel.Equals(Constants.RelationMaterialProductionOrder) || rel.Equals("MaterialHoldReason")) continue;
                if (!instance.HasRelation(rel)) continue;

                instance.LoadRelation(rel);
                instance.RemoveRelations(instance.RelationCollection[rel]);
            }
        }
              

        #endregion Material

        #region ProductionOrder

        /// <summary>
        /// Loads ProductionOrder Materials
        /// </summary>
        /// <param name="instance">ProductionOrder</param>
        public static void LoadMaterials(this ProductionOrder instance)
        {
            instance.CopyFrom(new LoadMaterialsForProductionOrdersInput
            {
                ProductionOrders = new ProductionOrderCollection { instance }
            }.LoadMaterialsForProductionOrdersSync().ProductionOrders.First());
        }

        #endregion

        #region Verifications
        public static StateModelState GetCurrentMainStateModelState(this Material instance)
        {
            instance.Load();
            return instance.CurrentMainState.CurrentState;
        }

        public static MaterialSystemState GetCurrentSystemStateModelState(this Material instance)
        {
            instance.Load();
            return instance.SystemState;
        }
        #endregion

      
    }
}
