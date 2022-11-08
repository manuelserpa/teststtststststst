//--------------------------------------------------------------------------------
//<FileInfo>
//  <copyright file="Settings.cs" company="Critical Manufacturing, SA">
//        <![CDATA[Copyright © Critical Manufacturing SA. All rights reserved.]]>
//  </copyright>
//  <Author>João Brandão</Author>
//</FileInfo>
//--------------------------------------------------------------------------------

#region Using Directives

using Cmf.Core.PageObjects;
using Cmf.Foundation.BusinessOrchestration.ConfigurationManagement.InputObjects;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;

#endregion Using Directives

namespace Settings
{
    /// <summary>
    /// Represents the text initialization class.
    /// </summary>
    /// <seealso cref="BaseContext" />
    [TestClass]
    public class amsOSRAMBaseTest : BaseTestClass
    {
        #region Private Variables
        #endregion

        #region Public Variables
        #endregion

        #region Properties
        #endregion

        #region Constructors
        #endregion

        #region Public Methods

        public static void NavigateTo(string path, NavigateToOptions mode = NavigateToOptions.Maximize | NavigateToOptions.Authenticate, string query = "")
        {
            BaseTestClass.NavigateTo(path, mode, query); 
            
            // Skip Keep user sing in
            if (Driver.Url.Contains("kmsi"))
            {
                IWebElement authComponent = Driver.FindElement(By.XPath("//cmf-core-controls-auth"));

                if (authComponent.Text.Contains("Remain signed in?"))
                {
                    IWebElement button = authComponent.FindElement(By.CssSelector(".cmf-btn.cmf-btn-primary"));
                    button.Click();

                    WaitForLoadingStop();
                }
            }
        }

        #endregion

        #region Private & Internal Methods
        #endregion

        #region Event handling Methods
        #endregion
    }
}
