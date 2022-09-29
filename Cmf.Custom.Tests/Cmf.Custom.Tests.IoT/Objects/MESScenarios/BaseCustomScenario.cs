using amsOSRAMEIAutomaticTests.Objects.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Reflection;


namespace Cmf.Custom.Tests.Biz.Common.Scenarios
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseCustomScenario
    {
        /// <summary>
        /// The tear down manager
        /// </summary>
        protected CustomTeardownManager TearDownManager = new CustomTeardownManager();

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
                this.TestName = GetTestMethodName();
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
                    // TearDown();
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
        /// Gets the Test Method Name
        /// </summary>
        /// <returns></returns>
        public static string GetTestMethodName()
        {
            // for when it runs via Visual Studio locally
            var stackTrace = new StackTrace();
            foreach (var stackFrame in stackTrace.GetFrames())
            {
                MethodBase methodBase = stackFrame.GetMethod();
                Object[] attributes = methodBase.GetCustomAttributes(typeof(TestMethodAttribute), false);
                if (attributes.Length >= 1)
                {
                    return methodBase.Name;
                }
            }
            return "Not called from a test method";
        }
    }
}
