
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.StateModelsManagement.InputObjects;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.DispatchManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.LaborManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects;
using Cmf.TestScenarios.Others;
using AMSOsramEIAutomaticTests.Objects.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AMSOsramEIAutomaticTests.Objects.Extensions
{
    /// <summary>
    /// Extension Methods Class that adapts resource services as Business Objects extension methods or adds functionality need for testing
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Retrieves all parent resources
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static ResourceHierarchy GetAscendantResources(this Resource instance)
        {
            ResourceHierarchy parentResources = null;

            if (instance != null)
            {
                parentResources = new Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects.GetAscendentResourcesInput()
                {
                    Resource = instance
                    ,
                    Depth = 1
                }.GetAscendentResourcesSync().AscendentResources;
            }

            return parentResources;
        }

        /// <summary>
        /// Retrieves all descendent resources
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static ResourceHierarchy GetDescendentResources(this Resource instance, short depth = 1)
        {
            ResourceHierarchy childResources = null;

            if (instance != null)
            {
                childResources = new Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects.GetDescendentResourcesInput()
                {
                    Resource = instance
                    ,
                    Depth = depth
                }.GetDescendentResourcesSync().DescendentResources;
            }

            return childResources;
        }

        /// <summary>
        /// Adjusts the State of a given Resource
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static void AdjustState(this Resource instance, string state)
        {
            if (instance != null)
            {
                new Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects.AdjustResourceStateInput()
                {
                    Resource = instance,
                    StateModel = instance.CurrentMainState.StateModel,
                    StateModelStateName = state
                }.AdjustResourceStateSync();
            }
        }


        /// <summary>
        /// Determines default transition for operation being performed
        /// </summary>
        /// <param name="instance">Resource instance where transition will be determined</param>
        /// <param name="material">Material instance causing the need for obtaining a state transition suggestion</param>
        /// <param name="operation">operation being performed (TrackIn, TrackOut)</param>
        /// <returns>StateModelTransitionSetting</returns>
        public static StateModelTransitionSetting GetEligibleTransition(this Resource instance, Material material, string operation = "TrackIn")
        {
            StateModelTransitionSetting returnSetting = new StateModelTransitionSetting();

            if (instance != null && instance.CurrentMainState != null && instance.CurrentMainState.StateModel != null && instance.CurrentMainState.StateModel.Id > 0)
            {
                returnSetting.StateModel = Extensions.GetEntity<StateModel>(instance.CurrentMainState.StateModel.Id);

                switch (operation.ToUpper())
                {
                    case "TRACKIN":

                        if (String.Equals(instance.CurrentMainState.CurrentState.Name, Constants.ResourceDefaultStateModelStandby.ToLower(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            GetDataForTrackInOperation operationToUse = GetDataForTrackInOperation.DispatchAndTrackIn;
                            if (material.SystemState == MaterialSystemState.Dispatched)
                                operationToUse = GetDataForTrackInOperation.TrackIn;

                            ResourcePossibleTransitions transitions = new GetDataForTrackInWizardInput() { Resource = instance, Material = material, Operation = operationToUse }.GetDataForTrackInWizardSync().ResourcePossibleTransitions;

                            if (transitions != null)
                            {
                                returnSetting.StateModel = instance.CurrentMainState.StateModel;
                                returnSetting.StateModelTransition = transitions.DefaultTransition;
                            }
                        }


                        break;
                    case "TRACKOUT":

                        if (instance.MaterialsInProcessCount == 1)
                        {
                            string currentState = instance.CurrentMainState.CurrentState.Name;
                            returnSetting.StateModelTransition = returnSetting.StateModel.StateTransitions.FirstOrDefault(E => E.FromState.Name == currentState && String.Equals(E.ToState.Name, Constants.ResourceDefaultStateModelStandby.ToLower(), StringComparison.InvariantCultureIgnoreCase));
                        }

                        break;


                }

            }

            return returnSetting;
        }

        /// <summary>
        /// Retrieves the list of materials dispatchable to a given resource
        /// </summary>
        /// <param name="instance">Resource for which dispatch list will be retrieved</param>
        /// <returns></returns>
        public static MaterialCollection GetDispatchList(this Resource instance)
        {
            MaterialCollection returbObj = null;

            if (instance != null)
            {
                returbObj = new GetDispatchListForResourceInput()
                {
                    Resource = instance
                }.GetDispatchListForResourceSync().Materials;
            }

            return returbObj;
        }

        ///// <summary>
        ///// Checks in the Employee for the current user
        ///// </summary>
        ///// <param name="instance">The Resource</param>
        ///// <returns></returns>
        //public static void CheckIn(this Resource instance, Employee employee = null, Dictionary<Resource, Employee> employeeToCheckOut = null, string userName = null)
        //{
        //    if (null == employee)
        //    {
        //        string user = string.IsNullOrEmpty(userName) ? SecurityScenario.GetUser(ConfigurationManager.AppSettings["UserName"]).UserAccount : userName;

        //        employee = new GetEmployeeByUserAccountInput()
        //        {
        //            UserAccount = user
        //        }.GetEmployeeByUserAccountSync().Employee;
        //    }
        //    Dictionary<Resource, Certification> resourceCertification = new Dictionary<Resource, Certification>();
        //    //resourceCertification.Add(instance, null);
        //    resourceCertification.Add(instance, new Certification());
        //    Dictionary<Resource, Employee> resourceEmployee = new Dictionary<Resource, Employee>();
        //    resourceEmployee.Add(instance, employee);

        //    Dictionary<Employee, CheckInEmployeeParameters> dictionaryEmployee = new Dictionary<Employee, CheckInEmployeeParameters>();
        //    dictionaryEmployee.Add(employee,
        //            new CheckInEmployeeParameters
        //            {
        //                EmployeeToCheckOut = employeeToCheckOut,
        //                ResourcesCertification = resourceCertification
        //            }
        //    );


        //    new CheckInEmployeesInput()
        //    {
        //        Employees = dictionaryEmployee
        //    }.CheckInEmployeesSync();
        //    instance.Load();
        //    instance.CopyFrom(instance);

        //}

        ///// <summary>
        ///// Checks out the Employee for the current user
        ///// </summary>
        ///// <param name="instance">The Resource</param>
        ///// <returns></returns>
        //public static void CheckOut(this Resource instance, Employee employee = null, string userName = null)
        //{

        //    Dictionary<Resource, EmployeeCollection> resourceEmployees = new Dictionary<Resource, EmployeeCollection>();
        //    EmployeeCollection employeeCollection = new EmployeeCollection();

        //    if (null == employee)
        //    {
        //        string user = string.IsNullOrEmpty(userName) ? SecurityScenario.GetUser(ConfigurationManager.AppSettings["UserName"]).UserAccount : userName;

        //        employee = new GetEmployeeByUserAccountInput()
        //        {
        //            UserAccount = user
        //        }.GetEmployeeByUserAccountSync().Employee;
        //    }
        //    employeeCollection.Add(employee);
        //    resourceEmployees.Add(instance, employeeCollection);

        //    try
        //    {
        //        new CheckOutEmployeesInput()
        //        {
        //            ResourceEmployees = resourceEmployees
        //        }.CheckOutEmployeesSync();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (!ex.Message.Contains(" is not checked in"))
        //        {
        //            throw ex;
        //        }
        //    }

        //    instance.Load();
        //    instance.CopyFrom(instance);
        //}

        /// <summary>
        /// Change resource state
        /// </summary>
        /// <param name="instance">The Resource</param>
        /// <param name="state">the state to go to</param>
        /// <returns></returns>
        public static void ChangeState(this Resource instance, string state)
        {
            StateModelTransitionCollection possibleTransitions = new GetPossibleTransitionsForStateInput() { StateModel = instance.CurrentMainState.StateModel, StateModelState = instance.CurrentMainState.CurrentState }.GetPossibleTransitionsForStateSync().StateModelTransitions;
            StateModelTransition smTransition = null;
            if (possibleTransitions != null)
                smTransition = possibleTransitions.FirstOrDefault(E => String.Equals(E.ToState.Name, state, StringComparison.InvariantCultureIgnoreCase));

            new ComplexLogResourceEventInput()
            {
                Resource = instance,
                StateModel = instance.CurrentMainState.StateModel,
                StateModelTransition = smTransition
            }.ComplexLogResourceEventSync();
        }

        /// <summary>
        /// Changes the state of the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="toStateName">Name of to state.</param>
        public static void ChangeResourceState(this Resource resource, string toStateName)
        {
            resource = GenericGetsScenario.GetObjectById<Resource>(resource.Id);

            StateModel stateModel = new StateModel();
            stateModel = GenericGetsScenario.GetObjectByName<StateModel>(resource.CurrentMainState.StateModel.Name);
            StateModelTransition transition = stateModel.StateTransitions.Where(t => t.FromState.Name.Equals(resource.CurrentMainState.CurrentState.Name)
                                                                                && t.ToState.Name.Equals(toStateName)).FirstOrDefault();

            if (transition != null)
            {
                resource.CopyFrom(new LogResourceEventInput()
                {
                    Resource = resource,
                    StateModel = stateModel,
                    StateModelTransition = transition
                }.LogResourceEventSync().Resource);
            }
        }

        public static void SetResourceDispachable(this Resource resource)
        {
            resource = GenericGetsScenario.GetObjectById<Resource>(resource.Id);
            resource = new Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects.SetorUnsetResourceDispatchableInput()
            {
                Resource = resource,
                IsDispatchable = true
            }.SetorUnsetResourceDispatchableSync().Resource;
        }

        public static StateModelState GetCurrentMainStateModelState(this Resource instance)
        {
            instance.Load();
            return instance.CurrentMainState.CurrentState;
        }

        public static ResourceSystemState GetCurrentSystemStateModelState(this Resource instance)
        {
            instance.Load();
            return instance.SystemState;
        }

        public static void DetachMaterials(this Resource resource, MaterialCollection materials)
        {
            if(materials.Count == 0)
            {
                return;
            }
            resource = GenericGetsScenario.GetObjectById<Resource>(resource.Id);
            resource = new Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects.DetachConsumablesFromResourceInput()
            {
                Resource = resource,
                MaterialCollection = materials
            }.DetachConsumablesFromResourceSync().Resource;
        }

        //public static void DetachAllMaterials(this Resource resource)
        //{
        //    DetachMaterials(resource, resource.GetAttachedMaterials());
        //}

    }


    public class StateModelTransitionSetting
    {
        public StateModel StateModel { get; set; }
        public StateModelTransition StateModelTransition { get; set; }
    }

}
