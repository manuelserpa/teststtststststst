using Cmf.Custom.Tests.Biz.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Cmf.Custom.Tests.Biz.Common.Scenarios
{
    /// <summary>
    /// Base Custom Scenario
    /// </summary>
    public abstract class CustomBaseScenario
    {
        /// <summary>
        /// The tear down manager
        /// </summary>
        public TestUtilities.CustomTearDownManager TearDownManager = new Cmf.Custom.TestUtilities.CustomTearDownManager();

        /// <summary>
        /// The test name
        /// </summary>
        protected string TestName = string.Empty;

        /// <summary>
        /// Base method that adds the Setup and TearDown logic to every test
        /// </summary>
        /// <param name="testToRun"></param>
        public void RunScenarioTest(Action testToRun)
        {
            Exception testFailed = null;
            try
            {
                this.TestName = CustomUtilities.GetTestMethodName();
                this.Setup();
                testToRun();
            }
            catch (Exception e)
            {
                testFailed = e;
                if (e.Message.Equals("The method or operation is not implemented."))
                {
                    Assert.Inconclusive(e.Message);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                #region Tear Down

                try
                {
                    CompleteCleanUp();
                    TearDown();
                }
                catch (Exception ex)
                {
                    if (testFailed == null)
                    {
                        Assert.Inconclusive("Error on Tear Down : " + ex.Message);
                    }
                }

                #endregion
            }
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        public abstract void Setup();

        /// <summary>
        /// Completes the clean up.
        /// </summary>
        public abstract void CompleteCleanUp();

        /// <summary>
        /// Tears down.
        /// </summary>
        public virtual void TearDown()
        {
            TearDownManager.TearDownSequentially();
        }
    }
}
