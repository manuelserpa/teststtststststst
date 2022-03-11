using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common.Integration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cmf.Custom.Tests.Biz.Common.Extensions
{
    public static class IntegrationEntryExtensions
    {
        /// <summary>
        /// Determines whether [is integration entry processed] [the specified number of tries].
        /// </summary>
        /// <param name="numberOfTries">The number of tries.</param>
        /// <param name="secondsBetweenAttempts">The seconds between attempts.</param>
        /// <returns><c>true</c> if [is integration entry processed] [the specified number of tries]; otherwise, <c>false</c>.</returns>
        public static bool IsIntegrationEntryProcessed(this IntegrationEntry instance, int numberOfTries = 60, int secondsBetweenAttempts = 1)
        {
            instance.WaitForIntegrationEntryDispatch(numberOfTries, secondsBetweenAttempts);

            // Integration Entry must be correctly processed
            if (instance.SystemState != IntegrationEntrySystemState.Processed)
                return false;

            return true;
        }

        /// <summary>
        /// Waits for integration entry dispatch.
        /// </summary>
        /// <param name="numberOfTries">The number of tries.</param>
        /// <param name="secondsBetweenAttempts">The seconds between attempts.</param>
        public static void WaitForIntegrationEntryDispatch(this IntegrationEntry instance, int numberOfTries = 60, int secondsBetweenAttempts = 10)
        {
            // reload entry to check state and wait for the Integration Entry to be processed, at a maximum of 60 check loops
            Int32 loops = 0;
            while ((instance.SystemState != IntegrationEntrySystemState.Processed && instance.SystemState != IntegrationEntrySystemState.Failed) && loops <= numberOfTries)
            {
                instance.Load();
                loops++;
                System.Threading.Thread.Sleep(secondsBetweenAttempts * 1000);
            }
        }
    }
}
