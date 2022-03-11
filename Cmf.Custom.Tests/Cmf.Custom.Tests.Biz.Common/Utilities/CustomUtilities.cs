using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.ErpManagement.InputObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Cmf.Custom.Tests.Biz.Common.Utilities
{
    public static class CustomUtilities
    {
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


        #region Integration Entries
        /// <summary>
        /// Dispatch Integration Entries
        /// </summary>
        /// <param name="integrationEntries"></param>
        public static void DispatchIntegrationEntries(IntegrationEntryCollection integrationEntries)
        {
            new DispatchIntegrationEntriesInput()
            {
                IntegrationEntries = integrationEntries
            }.DispatchIntegrationEntriesSync();
        } 
        #endregion
    }
}
