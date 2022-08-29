using System;
using System.Collections.Generic;
using System.Text;
using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Navigo.BusinessObjects;
using Cmf.TestScenarios.ContainerManagement.ContainerScenarios;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cmf.Custom.Tests.Biz.Containers
{
    [TestClass]
    public class CustomUndockContainerValidationTest
    {
        private CustomMaterialScenario materialScenario = null;
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

            #region Container setup

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

            #endregion Container setup
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            // Restore ContainerTypeUndockNotAllowed Configuration to original value
            ConfigUtilities.SetConfigValue(AMSOsramConstants.DefaultVendorContainerTypesConfig, originalVendorContainerTypes);

            // MaterialScenario teardown
            if (materialScenario != null)
            {
                materialScenario.Entity.Load();
                materialScenario.TearDown();
            }
        }

        /// <summary>
        /// Description:
        ///     - When trying to Undock a Container which type is configured at /AMSOsram/Container/ContainerTypeUndockNotAllowed/ and that has positions occupied with
        ///     materials, the Undock operation must not be allowed and an exception should be thrown.
        /// </summary>
        /// <TestCaseID>CustomUndockContainerValidationTest_UndockContainerWithUsedPositions</TestCaseID>
        [TestMethod]
        public void CustomUndockContainerValidationTest_UndockContainerWithUsedPositions()
        {
            //Validates if a exception is thrown after undock the container
            ValidationUtilities.ValidateThrowException(() =>
                 new Cmf.Navigo.BusinessOrchestration.ContainerManagement.InputObjects.UndockContainerInput
                 {
                     Container = materialScenario.ContainerScenario.Entity,
                     IgnoreLastServiceId = true
                 }.UndockContainerSync(),
                $"The Container { materialScenario.ContainerScenario.Entity.Name } cannot be undocked due to configuration",
                $"Undock operation should not be allowed due to configuration at { AMSOsramConstants.DefaultVendorContainerTypesConfig }!");

            materialScenario.ContainerScenario.Entity.LoadRelation("ContainerResource");

            // Validate that the Container is still docked
            Assert.IsTrue(materialScenario.ContainerScenario.Entity.ContainerResourceRelations != null && 
                materialScenario.ContainerScenario.Entity.ContainerResourceRelations.Count > 0, 
                $"The container { materialScenario.ContainerScenario.Entity.Name} should not have been undocked due to configuration at { AMSOsramConstants.DefaultVendorContainerTypesConfig }!");
        }

        /// <summary>
        /// Description:
        ///     - When trying to Undock a Container which type is configured at /AMSOsram/Container/ContainerTypeUndockNotAllowed/ and that has no positions occupied,
        ///     the Undock operation must be allowed and at the end the container should be terminated.
        /// </summary>
        /// <TestCaseID>CustomUndockContainerValidationTest_UndockContainerWithoutUsedPositions</TestCaseID>
        [TestMethod]
        public void CustomUndockContainerValidationTest_UndockContainerWithoutUsedPositions()
        {
            // Empty the container (ensure that at this point the container is docked, otherwise it will be terminated before processing the undock!)
            materialScenario.ContainerScenario.Entity.Empty();

            Assert.AreEqual(0, materialScenario.ContainerScenario.Entity.UsedPositions,
                $"The container { materialScenario.ContainerScenario.Entity.Name } should not have positions occupied!");

            // Undock container
            new Cmf.Navigo.BusinessOrchestration.ContainerManagement.InputObjects.UndockContainerInput
            {
                Container = materialScenario.ContainerScenario.Entity,
                IgnoreLastServiceId = true
            }.UndockContainerSync();

            // Validate that the Container was undocked
            materialScenario.ContainerScenario.Entity.LoadRelation("ContainerResource");

            Assert.IsTrue(materialScenario.ContainerScenario.Entity.ContainerResourceRelations == null ||
                materialScenario.ContainerScenario.Entity.ContainerResourceRelations.Count == 0,
                $"The container { materialScenario.ContainerScenario.Entity.Name } should have been undocked since it has no positions being used!");

            // Reload container
            materialScenario.ContainerScenario.Entity.Load();

            // Validate that the container was terminated
            Assert.AreEqual(Foundation.Common.Base.UniversalState.Terminated, materialScenario.ContainerScenario.Entity.UniversalState,
                $"The container { materialScenario.ContainerScenario.Entity.Name } should have been terminated!");
        }

        /// <summary>
        /// Description:
        ///     - When trying to Undock a Container which type is not configured at /AMSOsram/Container/ContainerTypeUndockNotAllowed/,
        ///     the Undock operation must be allowed.
        /// </summary>
        /// <TestCaseID>CustomUndockContainerValidationTest_ContainerTypeNotConfigured</TestCaseID>
        [TestMethod]
        public void CustomUndockContainerValidationTest_ContainerTypeNotConfigured()
        {
            // Clear configuration
            ConfigUtilities.SetConfigValue(AMSOsramConstants.DefaultVendorContainerTypesConfig, string.Empty);

            // Undock the container
            new Cmf.Navigo.BusinessOrchestration.ContainerManagement.InputObjects.UndockContainerInput
            {
                Container = materialScenario.ContainerScenario.Entity,
                IgnoreLastServiceId = true
            }.UndockContainerSync();

            materialScenario.ContainerScenario.Entity.LoadRelation("ContainerResource");

            // Validate that the container was undocked
            Assert.IsTrue(materialScenario.ContainerScenario.Entity.ContainerResourceRelations == null || 
                materialScenario.ContainerScenario.Entity.ContainerResourceRelations.Count == 0, 
                $"The container { materialScenario.ContainerScenario.Entity.Name } should have been undocked since there is no configuration at { AMSOsramConstants.DefaultVendorContainerTypesConfig }!");
        }
    }
}
