using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cmf.Custom.Tests.Biz.Common.Utilities
{
    public static class ValidationUtilities
    {
        /// <summary>
        /// Validates if an exception is thrown and an expected message is inside the exception message.
        /// In case the exception is not thrown it will for a fail with a given message. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="expectedMessage"></param>
        /// <param name="failMessageIfNoException"></param>
        public static void ValidateThrowException(Action action, string expectedMessage, string failMessageIfNoException)
        {
            if (string.IsNullOrWhiteSpace(expectedMessage))
            {
                throw new ArgumentNullException("expectedMessage parameter is required!");
            }

            if (string.IsNullOrWhiteSpace(failMessageIfNoException))
            {
                throw new ArgumentNullException("failMessageIfNoException parameter is required!");
            }

            try
            {
                action();
                Assert.Fail(failMessageIfNoException);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.ToLower().Contains(expectedMessage.ToLower()),
                   string.Format("Expected error message does not match. [Expected Message: {0}], [Current Message: {1}]", expectedMessage, ex.Message));
            }
        }
    }
}
