using System;
using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.TestUtilities;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.ContainerManagement.InputObjects;
using Cmf.TestScenarios.ContainerManagement.ContainerScenarios;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cmf.Custom.Tests.Biz.Containers
{
    [TestClass]
    public class CustomTerminateContainerTest
    {
        private CustomMaterialScenario materialScenario = null;
        private ContainerScenario containerScenario = null;
        private string originalVendorContainerTypes = null;

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            #region Material Scenario Setup

            //Creation Custom Material Scenario
            materialScenario = new CustomMaterialScenario(false)
            {
                NumberOfSubMaterials = 2,
                AssociateSubMaterialsToContainer = true
            };

            materialScenario.Setup();

            #endregion Material Scenario Setup

            #region Configurations

            // Get original config value for ContainerTypeUndockNotAllowed to restore later
            originalVendorContainerTypes = (string)ConfigUtilities.GetConfigValue(AMSOsramConstants.DefaultVendorContainerTypesConfig);

            // Set configuration with current ContainerType
            ConfigUtilities.SetConfigValue(AMSOsramConstants.DefaultVendorContainerTypesConfig,
                $"{ AMSOsramConstants.ContainerPeekCassete },{ materialScenario.ContainerScenario.Entity.Type }");

            #endregion Configurations
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            // Restore ContainerTypeUndockNotAllowed Configuration to original value
            ConfigUtilities.SetConfigValue(AMSOsramConstants.DefaultVendorContainerTypesConfig, originalVendorContainerTypes);

            // ContainerScenario teardown
            if (containerScenario != null)
            {
                containerScenario.Entity.Load();
                containerScenario.TearDown();
            }

            // MaterialScenario teardown
            if (materialScenario != null)
            {
                materialScenario.Entity.Load();
                materialScenario.TearDown();
            }
        }

        /// <summary>
        /// Description:
        ///     - When a Container which type is not configured at /AMSOsram/Container/ContainerTypeUndockNotAllowed/ is emptied,
        ///     then the container should not be terminated (regardless it is docked or not).
        /// </summary>
        /// <TestCaseID>CustomUndockContainerValidationTest_ContainerTypeNoConfiguration</TestCaseID>
        [TestMethod]
        public void CustomTerminateContainerTest_ContainerTypeNotConfigured()
        {
            // Clear configuration
            ConfigUtilities.SetConfigValue(AMSOsramConstants.DefaultVendorContainerTypesConfig, string.Empty);

            // Empty container
            materialScenario.ContainerScenario.Entity.Empty();

            // Reload container
            materialScenario.ContainerScenario.Entity.Load();

            // Validate that the container was not terminated
            Assert.AreNotEqual(Foundation.Common.Base.UniversalState.Terminated, materialScenario.ContainerScenario.Entity.UniversalState,
                $"The container { materialScenario.ContainerScenario.Entity.Name } should not have been terminated, since the ContainerType is not " +
                $"configured at { AMSOsramConstants.DefaultVendorContainerTypesConfig }!");
        }

        /// <summary>
        /// Description:
        ///     - When removing all the materials from a container which type is configured at /AMSOsram/Container/ContainerTypeUndockNotAllowed/,
        ///     if the container is docked, it should not be terminated.
        /// </summary>
        /// <TestCaseID>CustomTerminateContainerTest_ContainerDockedDisassociateMaterials</TestCaseID>
        [TestMethod]
        public void CustomTerminateContainerTest_ContainerDockedDisassociateMaterials()
        {
            #region Dock Container

            Resource resource = new Resource() { Name = "PDSP0101.AL" };
            resource.Load();

            // Dock Container
            resource.DockContainer(materialScenario.ContainerScenario.Entity);

            // Load ContainerResource relation
            materialScenario.ContainerScenario.Entity.LoadRelation("ContainerResource");

            // Validate that the container is docked
            Assert.IsTrue(materialScenario.ContainerScenario.Entity.ContainerResourceRelations != null && 
                materialScenario.ContainerScenario.Entity.ContainerResourceRelations.Count > 0, 
                $"The container { materialScenario.ContainerScenario.Entity.Name } should be docked!");

            #endregion

            #region Disassociate materials from container

            // Load MaterialContainer relation
            materialScenario.ContainerScenario.Entity.LoadRelation("MaterialContainer");

            // Disassociate materials from container
            DisassociateMaterialsFromContainer(materialScenario.ContainerScenario.Entity, materialScenario.ContainerScenario.Entity.ContainerMaterials);

            #endregion Disassociate materials from container            

            // Validate that the container was not terminated due being docked
            Assert.AreNotEqual(Foundation.Common.Base.UniversalState.Terminated, materialScenario.ContainerScenario.Entity.UniversalState,
                $"The container { materialScenario.ContainerScenario.Entity.Name } should not have been terminated, since it is docked!");
        }

        /// <summary>
        /// Description:
        ///     - When  a container which type is configured at /AMSOsram/Container/ContainerTypeUndockNotAllowed/ is emptied,
        ///     if the container is not docked, then it should be terminated.
        /// </summary>
        /// <TestCaseID>CustomTerminateContainerTest_ContainerUndockedEmptyContainer</TestCaseID>
        [TestMethod]
        public void CustomTerminateContainerTest_ContainerUndockedEmptyContainer()
        {
            // Empty container
            materialScenario.ContainerScenario.Entity.Empty();

            // Reload container
            materialScenario.ContainerScenario.Entity.Load();

            // Validate that the container was terminated
            Assert.AreEqual(Foundation.Common.Base.UniversalState.Terminated, materialScenario.ContainerScenario.Entity.UniversalState,
                $"The container { materialScenario.ContainerScenario.Entity.Name } should have been terminated, since it is undocked!");
        }

        /// <summary>
        /// Description:
        ///     - When removing all the materials from a container which type is configured at /AMSOsram/Container/ContainerTypeUndockNotAllowed/,
        ///     if the container is not docked, then it should be terminated.
        /// </summary>
        /// <TestCaseID>CustomTerminateContainerTest_ContainerUndockedDisassociateMaterials</TestCaseID>
        [TestMethod]
        public void CustomTerminateContainerTest_ContainerUndockedDisassociateMaterials()
        {
            #region Disassociate materials from container

            // Load MaterialContainer relation
            materialScenario.ContainerScenario.Entity.LoadRelation("MaterialContainer");

            // Disassociate materials from container
            DisassociateMaterialsFromContainer(materialScenario.ContainerScenario.Entity, materialScenario.ContainerScenario.Entity.ContainerMaterials);

            #endregion Disassociate materials from container

            // Reload container
            materialScenario.ContainerScenario.Entity.Load();

            // Validate that the container was terminated
            Assert.AreEqual(Foundation.Common.Base.UniversalState.Terminated, materialScenario.ContainerScenario.Entity.UniversalState,
                $"The container { materialScenario.ContainerScenario.Entity.Name } should have been terminated, since it is undocked!");
        }

        /// <summary>
        /// Description:
        ///     - When all the materials are transfered to another container and the source container type is configured
        ///     at /AMSOsram/Container/ContainerTypeUndockNotAllowed/, if the container is not docked, then it should be terminated.
        /// </summary>
        /// <TestCaseID>CustomTerminateContainerTest_ContainerUndockedTransferMaterialBetweenContainers</TestCaseID>
        [TestMethod]
        public void CustomTerminateContainerTest_ContainerUndockedTransferMaterialBetweenContainers()
        {
            #region Create target container

            // Create one Container to put the Wafers
            containerScenario = new ContainerScenario();
            containerScenario.Entity.IsAutoGeneratePositionEnabled = false;
            containerScenario.Entity.Name = "Container_" + DateTime.Now.Ticks.ToString();
            containerScenario.Entity.Type = materialScenario.ContainerScenario.Entity.Type;
            containerScenario.Entity.PositionUnitType = ContainerPositionUnitType.Material;
            containerScenario.Entity.Facility = materialScenario.Entity.Facility;
            containerScenario.Entity.CapacityUnits = AMSOsramConstants.UnitWafers;
            containerScenario.Entity.CapacityPerPosition = 1;
            containerScenario.Entity.TotalPositions = AMSOsramConstants.ContainerTotalPosition;
            containerScenario.Setup();

            #endregion  Create target container

            #region Transfer materials

            // Load Sub-Materials
            materialScenario.Entity.LoadChildren();

            MaterialContainerCollection materialContainers = new MaterialContainerCollection();

            for (int i = 0; i < materialScenario.Entity.SubMaterialCount; i++)
            {
                MaterialContainer materialContainer = new MaterialContainer()
                {
                    SourceEntity = materialScenario.Entity.SubMaterials[i],
                    TargetEntity = containerScenario.Entity,
                    Position = i + 1
                };

                materialContainers.Add(materialContainer);
            }

            // Transfer materials from current container to containerScenario
            new TransferMaterialBetweenContainersInput()
            {
                Container = materialScenario.ContainerScenario.Entity,
                MaterialContainerRelations = materialContainers
            }.TransferMaterialBetweenContainersSync();          

            #endregion Transfer materials

            // Reload container
            materialScenario.ContainerScenario.Entity.Load();

            // Validate that the container was terminated
            Assert.AreEqual(Foundation.Common.Base.UniversalState.Terminated, materialScenario.ContainerScenario.Entity.UniversalState,
                $"The container { materialScenario.ContainerScenario.Entity.Name } should have been terminated, since it is undocked!");
        }

        #region Help methods

        /// <summary>
        /// Disassociate materials from a container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="materialContainersToRemove"></param>
        private void DisassociateMaterialsFromContainer(Container container, MaterialContainerCollection materialContainersToRemove)
        {
            // Remove all the Sub-Materials from Container
            new UpdateContainerMaterialPositionsInput()
            {
                Container = container,
                MaterialContainerRelationsToRemove = materialContainersToRemove
            }.UpdateContainerMaterialPositionsSync();

            // Reload container
            container.Load();

            Assert.IsTrue(container.ContainerMaterials == null || container.ContainerMaterials.Count == 0, $"The container { container.Name } should be empty!");
        }

        #endregion Help methods
    }
}
