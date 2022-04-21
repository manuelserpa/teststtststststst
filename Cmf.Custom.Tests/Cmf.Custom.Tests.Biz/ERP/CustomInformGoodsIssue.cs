using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.TestUtilities;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmf.Custom.Tests.Biz.ERP
{
    [TestClass]
    public class CustomInformGoodsIssue
    {
        private ExecutionScenario _scenario;
        private CustomTearDownManager customTeardownManager = null;

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            _scenario = new ExecutionScenario();
            customTeardownManager = new CustomTearDownManager();
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            if (customTeardownManager != null)
                customTeardownManager.TearDownSequentially();

            if (_scenario != null)
            {
                _scenario.CompleteCleanUp();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <TestCaseID>CustomInformGoodsIssue.CustomInformGoodsIssue_TrackOutOnFirstStepOfPO_CreateIntegrationEntry</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomInformGoodsIssue_TrackOutOnFirstStepOfPO_CreateIntegrationEntry()
        {
            ///<Step> Create a Production Order, a Lot and its wafers </Step>
            _scenario.IsToAssingPOsToMaterials = true;
            _scenario.NumberOfProductionOrdersToGenerate = 1;
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.NumberOfChildMaterialsToGenerate = 5;
            _scenario.Setup();

            Material material = _scenario.GeneratedLots.FirstOrDefault();

            // dispatch and Trackin
            Resource resource = new Resource
            {
                Name = AMSOsramConstants.DefaultTestResourceName
            };

            resource.Load();

            material.ComplexDispatchAndTrackIn(resource);

            material.TrackOut();

            // check IE
        }
    }
}
