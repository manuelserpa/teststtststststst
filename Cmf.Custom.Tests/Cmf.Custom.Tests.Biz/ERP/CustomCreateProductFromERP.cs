using Cmf.Custom.Tests.Biz.Common.ERP;
using Cmf.Custom.Tests.Biz.Common.ERP.Product;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cmf.Custom.Tests.Biz.ERP
{
    [TestClass]
    public class CustomCreateProductFromERP
    {

        private ExecutionScenario _scenario;

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            _scenario = new ExecutionScenario()
            {

            };
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            if (_scenario != null)
            {
                _scenario.CompleteCleanUp();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <TestCaseID>CustomCreateProductFromERP.CustomCreateProductFromERP_SendERPMessage_CreateProducts</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomCreateProductFromERP_SendERPMessage_CreateProducts()
        {
            
        }

    }
}
