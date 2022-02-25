//<FileInfo>
//  <copyright file="SendTrackInInformationToIoT.cs" company="Critical Manufacturing, SA">
//        <![CDATA[Copyright © Critical Manufacturing SA. All rights reserved.]]>
//  </copyright>
//  <Author>Davi Figueiredo</Author>
//</FileInfo>

using Cmf.Custom.Tests.Biz.Common.Extensions;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.DispatchManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Cmf.TestScenarios.ContainerManagement.ContainerScenarios;
using Cmf.TestScenarios.MaterialManagement.MaterialScenarios;
using Cmf.TestScenarios.Others;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;

namespace Cmf.Custom.Tests.Biz.Common.Scenarios
{
    /// <summary>
    /// CustomMaterialScenario
    /// </summary>
    public class CustomMaterialScenario : MaterialScenario
    {
        #region constants

        private const int DefaultNumberOfWafers = 13;

        #endregion

        #region Resource
        private class OriginalResourceData
        {
            public Resource Resource { get; set; }
            public ResourceAutomationMode AutomationMode { get; set; }
            public string AutomationAddress { get; set; }

        }

        /// <summary>
        /// Modified Resource (AutomationMode changed)
        /// </summary>
        private Dictionary<string, OriginalResourceData> ModifiedResources = new Dictionary<string, OriginalResourceData>();

        /// <summary>
        /// Control variable isResourceOnlineModeChanged
        /// </summary>
        private bool isResourceOnlineModeChanged = false;
        /// <summary>
        /// Original AutomationMode
        /// </summary>
        private ResourceAutomationMode OriginalAutomationMode { get; set; }
        /// <summary>
        /// Original AutomationAddress
        /// </summary>
        private string OriginalAutomationAddress { get; set; }

        /// <summary>
        /// Modified Resource (Material TrackInOut)
        /// </summary>
        public Resource ResourceMaterialInOut { get; set; }
        /// <summary>
        /// Control variable isSubMaterialTrackingEnabledOriginalValue
        /// </summary>
        private bool? isSubMaterialTrackingEnabledOriginalValue = null;
        /// <summary>
        /// Control variable isSubMaterialTrackingEnabledChanged
        /// </summary>
        private bool isSubMaterialTrackingEnabledChanged = false;
        /// <summary>
        /// Set the Resource online. Default should be true for IoT tests to work.
        /// </summary>
        public bool SetResourceOnline { get; private set; }
        /// <summary>
        /// Indicates if the Dispatch must be performed during the setup.
        /// Default is true.
        /// </summary>
        public bool PerformDispatchOnSetup { get; set; } = true;

        public Flow Flow = null;
        public Step Step = null;
        public Facility Facility = null;
        #endregion

        #region Step 

        /// <summary>
        /// Modified Step
        /// </summary>
        private Step step = null;
        /// <summary>
        /// Control variable subMaterialTrackStateDeptOriginalValue
        /// </summary>
        private int? subMaterialTrackStateDeptOriginalValue = null;
        /// <summary>
        /// Control variable subMaterialTrackStateDeptChanged
        /// </summary>
        private bool subMaterialTrackStateDeptChanged = false;

        #endregion
        /// <summary>
        /// Container Scenario
        /// </summary>
        public ContainerScenario ContainerScenario { get; private set; }
        public Collection<MaterialScenario> waferScenarios = new Collection<MaterialScenario>();
        public MaterialCollection SubMaterials = new MaterialCollection();
        public Resource Resource = null;

        public string FlowName = null;
        public string StepName = null;

        #region Material

        /// <summary>
        /// Setup the Number of SubMaterials
        /// </summary>
        public int? NumberOfSubMaterials { get; set; }

        /// <summary>
        /// Setup the Material Type
        /// </summary>
        public string MaterialType = null;

        /// <summary>
        /// Define if sub materials should be assigned to containers. Default is true.
        /// </summary>
        /// <remarks>
        /// Set this flag to false if your test does not require container validation.
        /// </remarks>
        public bool AssociateSubMaterialsToContainer = true;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// Default value for setResourceOnline should be true for IoT tests to work.
        /// </summary>
        public CustomMaterialScenario(bool setResourceOnline = true)
        {
            this.SetResourceOnline = setResourceOnline;
        }

        #endregion
        /// <summary>
        /// Setup the Scenario
        /// </summary>
        public void Setup(bool isMaterialInOut = false,
            bool isToSetAutomationModeOnline = true,
            bool automaticContainerPositions = true,
            string containerType = AMSOsramConstants.ContainerSMIFPod,
            string productName = AMSOsramConstants.TestProduct,
            string subMaterialProductName = null)
        {
            Cmf.TestScenarios.Others.Utilities.RunTearDown = true;

            #region Material Setup

            // Facility
            if (Facility == null)
            {
                Facility = new Facility() { Name = AMSOsramConstants.TestFacility };
                Facility.Load();
            }

            // Product
            Product product = new Product() { Name = productName };
            product.Load();

            //StateModel stateModel = GenericGetsScenario.GetObjectByName<StateModel>("CustomMaterialStateModel");

            // Number of Wafers = Number of Container Positions = Number of sub materials
            int numberOfWafers = DefaultNumberOfWafers;
            if (NumberOfSubMaterials.HasValue)
            {
                numberOfWafers = NumberOfSubMaterials.Value;
            }

            string materialType = AMSOsramConstants.MaterialTypeProduction;
            if (MaterialType != null)
            {
                materialType = MaterialType;
            }

            if (this.Flow == null && !String.IsNullOrEmpty(this.FlowName))
            {
                this.Flow = new Flow() { Name = this.FlowName };
                this.Flow.Load();
            }

            if (this.Step == null && !String.IsNullOrEmpty(this.StepName))
            {
                this.Step = new Step() { Name = this.StepName };
                this.Step.Load();
            }

            // Top Most Material - Lot
            this.Entity.Name = "MESTest_Material_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            this.Entity.Facility = this.Facility;

            // If the Flow and Step are available then set them in the entity including respective flowpath
            // Otherwise the system will use the Product's default FlowPath
            if (this.Flow != null && this.Step != null)
            {
                this.Entity.Flow = this.Flow;
                this.Entity.Step = this.Step;
                // this.Entity.FlowPath = this.Flow.GetFlowPath(); >>> !!!! This extension method from Cmf.Custom.TestUtilities does not work for sub flows
                this.Entity.FlowPath = FlowExtensionMethods.CustomGetFlowPath(this.Flow, this.Step.Name);
            }
            this.Entity.Product = product;
            this.Entity.Form = AMSOsramConstants.FormLot;
            this.Entity.Type = materialType;
            this.Entity.PrimaryUnits = AMSOsramConstants.UnitWafers;
            this.Entity.PrimaryQuantity = 0;
            //this.Entity.CurrentMainState = new CurrentEntityState()
            //{
            //    StateModel = stateModel,
            //    CurrentState = stateModel.States.FirstOrDefault(s => s.Name == "Queued")
            //};

            // Setup the SubMaterial info (Wafers)
            this.MainForm = AMSOsramConstants.FormLot;
            this.SubForm = AMSOsramConstants.FormWafer;
            this.AddServiceContexts = false;

            base.Setup();

            if (numberOfWafers > 0)
            {
                Product waferProduct = product;
                if (!string.IsNullOrWhiteSpace(subMaterialProductName))
                {
                    waferProduct = new Product()
                    {
                        Name = subMaterialProductName
                    };

                    if (!waferProduct.ObjectExists())
                    {
                        throw new ArgumentNullException("subMaterialProductName", $"Product '{subMaterialProductName}' does not exist in the system!");
                    }

                    waferProduct.Load();
                }

                for (int i = 0; i < numberOfWafers; i++)
                {
                    MaterialScenario ms = new MaterialScenario();
                    //ms.Entity.Name = "MESTest_Material_" + DateTime.Now.ToString("yyyyMMdd_HHmmssffffff");
                    ms.Entity.Facility = this.Facility;
                    ms.Entity.Flow = this.Entity.Flow;
                    ms.Entity.Step = this.Entity.Step;
                    ms.Entity.FlowPath = this.Entity.FlowPath;
                    ms.Entity.Product = waferProduct;
                    ms.Entity.Form = AMSOsramConstants.FormWafer;
                    ms.Entity.Type = materialType;
                    ms.Entity.PrimaryUnits = AMSOsramConstants.UnitWafers;
                    ms.Entity.PrimaryQuantity = 1;
                    ms.AddServiceContexts = false;
                    ms.Setup();
                    waferScenarios.Add(ms);
                }

                this.SubMaterials = new MaterialCollection();
                this.SubMaterials.AddRange(waferScenarios.Select(M => M.Entity));

                new Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects.AttachMaterialsInput
                {
                    Material = this.Entity,
                    SubMaterials = this.SubMaterials
                }.AttachMaterialsSync();

                this.Refresh();

                for (int i = 0; i < numberOfWafers; i++)
                {
                    waferScenarios[i].Refresh();
                }

                this.SubMaterials = new MaterialCollection();
                this.SubMaterials.AddRange(waferScenarios.Select(M => M.Entity));
            }

            #endregion

            Resource resourceToUse = Resource;

            this.SetResourceAutomationModeOnline(isToSetAutomationModeOnline);

            #region Container creation and Sub-Material (Wafers) Association

            if (numberOfWafers > 0 && AssociateSubMaterialsToContainer)
            {
                // Create one Container to put the Wafers
                this.ContainerScenario = new ContainerScenario();
                this.ContainerScenario.Entity.IsAutoGeneratePositionEnabled = automaticContainerPositions;
                this.ContainerScenario.Entity.Name = "Container_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
                this.ContainerScenario.Entity.Type = containerType;
                this.ContainerScenario.Entity.PositionUnitType = ContainerPositionUnitType.Material;
                this.ContainerScenario.Entity.Facility = this.Entity.Facility;
                this.ContainerScenario.Entity.CapacityUnits = AMSOsramConstants.UnitWafers;
                //this.ContainerScenario.Entity.CapacityUnits = "CARRIER";
                this.ContainerScenario.Entity.CapacityPerPosition = 1;
                this.ContainerScenario.Entity.TotalPositions = AMSOsramConstants.ContainerTotalPosition;
                this.ContainerScenario.Setup();

                // Associate the Wafers to Container
                this.ContainerScenario.AssociateMaterials(this.SubMaterials);
            }

            #endregion

            Flow flow = null;
            string flowPath = String.Empty;
            if (!String.IsNullOrEmpty(FlowName) && !String.IsNullOrEmpty(StepName))
            {
                flow = new Flow() { Name = FlowName };
                flow.Load();
                step = new Step() { Name = StepName };
                step.Load();

                flowPath = FlowExtensionMethods.CustomGetFlowPath(flow, step.Name);
            }

            #region MaterialInOut

            if (isMaterialInOut)
            {
                if (flow == null)
                {
                    flow = new Flow() { Id = this.Entity.Flow.Id };
                    flow.Load();

                    step = new Step()
                    {
                        Name = AMSOsramConstants.TestM3MTZnOSputterCluster6in00126F008_E
                    };

                    step.Load();

                    // Change Flow and Step to a step that allows SubMaterial TrackIn
                    flowPath = FlowExtensionMethods.CustomGetFlowPath(flow, step.Name);
                }
                this.ChangeFlowAndStep(flow, step, flowPath);

                if (PerformDispatchOnSetup)
                {
                    if (Resource != null)
                    {
                        this.Dispath(resourceToUse);
                    }
                    else
                    {
                        this.Dispath();
                    }
                }

                this.LoadRelations(new Collection<string>() { "MaterialResource" });
                MaterialResource materialResource = ((MaterialResource)this.Entity.RelationCollection.First().Value.First());

                materialResource.TargetEntity.Load();
                ResourceMaterialInOut = materialResource.TargetEntity;

                if (step.SubMaterialTrackStateDepth.GetValueOrDefault() < 1)
                {
                    subMaterialTrackStateDeptChanged = true;
                    subMaterialTrackStateDeptOriginalValue = step.SubMaterialTrackStateDepth;
                    step.SubMaterialTrackStateDepth = 1;
                    step.Save();
                }

                if (!materialResource.TargetEntity.IsSubMaterialTrackingEnabled.GetValueOrDefault())
                {
                    isSubMaterialTrackingEnabledChanged = true;
                    isSubMaterialTrackingEnabledOriginalValue = ResourceMaterialInOut.IsSubMaterialTrackingEnabled;
                    ResourceMaterialInOut.IsSubMaterialTrackingEnabled = true;
                    ResourceMaterialInOut.Save();
                }
            }
            else if (flow != null)
            {
                this.ChangeFlowAndStep(flow, step, flowPath);
                if (PerformDispatchOnSetup)
                {
                    this.Dispath(resourceToUse);
                }
            }
            #endregion

            base.Refresh();
        }

        /// <summary>
        /// Change Flow and Step
        /// </summary>
        public void ChangeFlowAndStep(string flowName, string stepName)
        {
            if (string.IsNullOrWhiteSpace(flowName))
            {
                throw new ArgumentNullException("flowName", "flowName should not be empty!");
            }

            if (string.IsNullOrWhiteSpace(stepName))
            {
                throw new ArgumentNullException("stepName", "stepName should not be empty!");
            }

            Flow flow = new Flow() { Name = flowName };
            if (!flow.ObjectExists())
            {
                throw new ArgumentException(string.Format("Flow {0} does not exist!", flowName));
            }

            Step step = new Step()
            {
                Name = stepName
            };

            if (!step.ObjectExists())
            {
                throw new ArgumentException(string.Format("Step {0} does not exist!", stepName));
            }

            flow.Load();
            step.Load();

            if (base.Entity.Flow.DefinitionId != flow.DefinitionId || base.Entity.Step.Id != step.Id)
            {

                var flowPath = FlowExtensionMethods.CustomGetFlowPath(flow, step.Name);
                base.Entity.ChangeFlowAndStep(flow, step, flowPath);
            }
        }


        /// <summary>
        /// Change the resource AutomationMode to Online for testing
        /// </summary>
        public void SetResourceAutomationModeOnline(bool isToSetAutomationModeOnline = true)
        {
            if (SetResourceOnline) // Assuming that ResourceAutomationMode is not Online by default
            {
                #region Resource and AutomationMode

                Resource resourceToUse = this.Resource;
                if (resourceToUse == null)
                {
                    // Get the possible resource and set the AutomationMode to Online so the DEEs can be triggered
                    GetDispatchListForMaterialInput serviceInput = new GetDispatchListForMaterialInput
                    {
                        Material = base.Entity,
                        DispatchType = ProcessingType.Process
                    };
                    var serviceOutput = serviceInput.GetDispatchListForMaterialSync();

                    if (serviceOutput.Resources == null || serviceOutput.Resources.Count == 0)
                    {
                        throw new Exception("Missing resource for testing!");
                    }

                    resourceToUse = serviceOutput.Resources[0];
                }
                resourceToUse.Load();

                if (resourceToUse.AutomationMode != ResourceAutomationMode.Online && isToSetAutomationModeOnline)
                {
                    if (!ModifiedResources.ContainsKey(resourceToUse.Name))
                    {
                        OriginalResourceData originalData = new OriginalResourceData()
                        {
                            Resource = resourceToUse,
                            AutomationMode = resourceToUse.AutomationMode,
                            AutomationAddress = resourceToUse.AutomationAddress
                        };

                        // change the values to test
                        resourceToUse.AutomationMode = ResourceAutomationMode.Online;
                        resourceToUse.AutomationAddress = "127.0.0.1";
                        resourceToUse.Save();

                        isResourceOnlineModeChanged = true;

                        ModifiedResources.Add(resourceToUse.Name, originalData);
                    }
                }
            }

            #endregion
        }

        public void DockContainer(Resource loadPort)
        {
            if (this.ContainerScenario != null && loadPort != null)
            {
                // Undock if docked first
                loadPort.LoadRelation("ContainerResource");
                if (loadPort.ResourceContainers != null && loadPort.ResourceContainers.Count > 0)
                {
                    var containerToUndock = loadPort.ResourceContainers[0].SourceEntity;
                    containerToUndock.Load();

                    var undock = new Cmf.Navigo.BusinessOrchestration.ContainerManagement.InputObjects.UndockContainerInput()
                    {
                        Container = containerToUndock,
                        NumberOfRetries = 3
                    };
                    undock.UndockContainerSync();

                }

                //
                this.ContainerScenario.Entity.Load();
                this.ContainerScenario.Entity.LoadRelation("ResourceContainer");
                loadPort.Load();
                var dock = new Cmf.Navigo.BusinessOrchestration.ContainerManagement.InputObjects.DockContainerInput()
                {
                    Container = this.ContainerScenario.Entity,
                    Resource = loadPort,
                    IgnoreLastServiceId = true,
                    NumberOfRetries = 10,
                }.DockContainerSync();

                this.ContainerScenario.Entity.LoadRelations(new Collection<String>() { "ContainerResource" });
            }
        }

        /// <summary>
        /// Tear down the created objects
        /// </summary>
        /// <param name="releaseMaterial">true: validates if material is OnHold anb release it before terminating; false otherwise</param>
        public void TearDown(bool releaseMaterial)
        {
            if (this.Entity != null && releaseMaterial)
            {
                ReleaseMaterial();
            }

            this.TearDown();
        }

        /// <summary>
        /// Releases the material.
        /// </summary>
        public void ReleaseMaterial()
        {
            this.Entity.Load();
            if (this.Entity.HoldCount > 0)
            {
                MaterialHoldReasonUserInformationCollection getHoldReasons = new GetMaterialHoldReasonsWithUserInformationInput()
                {
                    Material = this.Entity,
                    LevelsToLoad = 1
                }.GetMaterialHoldReasonsWithUserInformationSync().MaterialHoldReasonUserInformationCollection;

                if (getHoldReasons != null)
                {
                    MaterialHoldReasonCollection holdReasons = new MaterialHoldReasonCollection();
                    getHoldReasons.ForEach(a => holdReasons.Add(a.MaterialHoldReason));

                    new ReleaseMaterialInput()
                    {
                        Material = this.Entity,
                        IgnoreLastServiceId = true,
                        MaterialHoldReasonCollection = holdReasons
                    }.ReleaseMaterialSync();
                }

            }
        }

        /// <summary>
        /// Special Release the material.
        /// </summary>
        public void SpecialReleaseMaterial()
        {
            this.Entity.Load();
            if (this.Entity.HoldCount > 0)
            {
                MaterialHoldReasonUserInformationCollection getHoldReasons = new GetMaterialHoldReasonsWithUserInformationInput()
                {
                    Material = this.Entity,
                    LevelsToLoad = 1
                }.GetMaterialHoldReasonsWithUserInformationSync().MaterialHoldReasonUserInformationCollection;

                if (getHoldReasons != null)
                {
                    MaterialHoldReasonCollection holdReasons = new MaterialHoldReasonCollection();
                    getHoldReasons.ForEach(a => holdReasons.Add(a.MaterialHoldReason));

                    new SpecialReleaseMaterialInput()
                    {
                        Material = this.Entity,
                        IgnoreLastServiceId = true,
                        MaterialHoldReasonCollection = holdReasons
                    }.SpecialReleaseMaterialSync();
                }

            }
        }

        /// <summary>
        /// Tear down the created objects
        /// </summary>
        public new void TearDown()
        {
            if (this.Entity != null)
            {
                this.Entity.Load();
                if (this.Entity.UniversalState != Foundation.Common.Base.UniversalState.Terminated &&
                    this.Entity.SystemState == MaterialSystemState.InProcess)
                {
                    //this will abort the material, clearing the iot persistence in case of errors during the test
                    //calling abort with retry to try to avoid concurrency issues
                    AbortMaterialProcessInput abortMaterialProcessInput = new AbortMaterialProcessInput()
                    {
                        Material = this.Entity,
                        NumberOfRetries = 5
                    };
                    abortMaterialProcessInput.AbortMaterialProcessSync();

                    ReleaseMaterial();
                }
            }

            base.TearDown();

            if (this.ContainerScenario != null)
            {
                this.ContainerScenario.TearDown();
            }

            if (isResourceOnlineModeChanged && ModifiedResources.Count > 0)
            {
                foreach (var item in ModifiedResources.Values)
                {
                    item.Resource.Load();
                    item.Resource.AutomationMode = item.AutomationMode;
                    item.Resource.AutomationAddress = item.AutomationAddress;
                    item.Resource.Save();
                }
            }

            if (isSubMaterialTrackingEnabledChanged && ResourceMaterialInOut != null)
            {
                ResourceMaterialInOut.Load();
                ResourceMaterialInOut.IsSubMaterialTrackingEnabled = isSubMaterialTrackingEnabledOriginalValue;
                ResourceMaterialInOut.Save();
            }

            if (subMaterialTrackStateDeptChanged && step != null)
            {
                step.Load();
                step.SubMaterialTrackStateDepth = subMaterialTrackStateDeptOriginalValue;
                step.Save();
            }
        }
    }
}
