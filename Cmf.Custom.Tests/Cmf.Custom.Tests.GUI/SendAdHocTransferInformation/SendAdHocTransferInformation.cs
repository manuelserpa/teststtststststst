using Cmf.Core.Business.Controls.PageObjects.Components;
using Cmf.Core.Controls.PageObjects.Components;
using Cmf.Core.PageObjects;
using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.GUI.PageObjects.Components;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.ResourceManagement.OutputObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Settings;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Cmf.Custom.Tests.GUI.SendAdHocTransferInformation
{
    /// <summary>
    /// SendAdHocTransferInformation GUI Tests
    /// </summary>
    [TestClass]
    public class SendAdHocTransferInformationTest : amsOSRAMBaseTest
    {
        #region Variables and constants

        private const string SendAdHocTransferInformationButtonSelector = "cmf-core-controls-actionbutton[button-id='Custom.Resource.SendAdHocTransferInformationButton']";

        #endregion Variables and constants

        /// <summary>
        /// ClassInit
        /// </summary>
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            Init(context);
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        [ClassCleanup]
        public static void Cleanup()
        {
            Quit();
        }

        /// <summary>
        /// Description:
        ///     Validates the 'AdHoc Transfer' Button visibility on Resource Entity Page. If the Resource IsSorter, then Button should be visible and clickable
        ///     Logic:
        ///     - Navigate to Resource entity page
        ///     - Validate that when Resource 'IsSorter' attribute is true, 'AdHoc Transfer' Action Button is available
        ///     - Validate that when Resource 'IsSorter' attribute is false, 'AdHoc Transfer' Action Button is no longer available
        /// Acceptance Criteria:
        ///     Error message is shown to the user
        /// </summary>
        /// <TestCaseID>SendAdHocTransferInformationTest_EntityResource_ValidateButtonVisibility</TestCaseID>
        [TestMethod]
        public void SendAdHocTransferInformationTest_EntityResource_ValidateButtonVisibility()
        {
            Resource resource = InitializeResourceScenario();
            bool? teardownAttribute = resource.Attributes.ContainsKey(amsOSRAMConstants.ResourceAttributeIsSorter) ? (bool)resource.Attributes[amsOSRAMConstants.ResourceAttributeIsSorter] : null;

            try
            {
                resource.SaveAttribute(amsOSRAMConstants.ResourceAttributeIsSorter, false);

                EntityPage<Resource> resourceEntityPage = NavigateToResourceEntityPage(resource.Id);

                #region Validate AdHoc Transfer Button availability

                //Validate that AdHoc Transfer Button is not available
                ActionButton adHocTransferButton = resourceEntityPage.CreatePageObject<ActionButton>(By.CssSelector(SendAdHocTransferInformationButtonSelector));
                Assert.IsFalse(adHocTransferButton.IsEnabled, "AdHoc Transfer Button should not be available!");

                //Set IsSorter attribute to true
                resource.SaveAttribute(amsOSRAMConstants.ResourceAttributeIsSorter, true);

                //Refresh entity page
                ActionButton refreshButton = resourceEntityPage.CreatePageObject<ActionButton>(By.CssSelector("cmf-core-controls-actionbutton[button-id='Generic.Refresh']"));
                refreshButton.Click();

                WaitForLoadingStop();

                //Validate that AdHoc Transfer Button is now available
                adHocTransferButton = resourceEntityPage.CreatePageObject<ActionButton>(By.CssSelector(SendAdHocTransferInformationButtonSelector));
                Assert.IsTrue(adHocTransferButton.IsEnabled, "AdHoc Transfer Button should be available!");

                #endregion Validate AdHoc Transfer Button availability
            }
            finally
            {
                //Reset Resource IsSorter attribute
                if (teardownAttribute == null)
                {
                    resource.RemoveAttribute(amsOSRAMConstants.ResourceAttributeIsSorter);
                }
                else
                {
                    resource.LoadAttribute(amsOSRAMConstants.ResourceAttributeIsSorter);
                    resource.SaveAttribute(amsOSRAMConstants.ResourceAttributeIsSorter, teardownAttribute);
                }
            }
        }

        /// <summary>
        /// Description:
        ///     Validates the 'AdHoc Transfer' Button visibility on ResourceView. If the Resource IsSorter, then Button should be visible and clickable
        ///     Logic:
        ///     - Navigate to ResourceView
        ///     - Validate that when Resource 'IsSorter' attribute is true, 'AdHoc Transfer' Action Button is available
        ///     - Validate that when Resource 'IsSorter' attribute is false, 'AdHoc Transfer' Action Button is no longer available
        /// Acceptance Criteria:
        ///     Error message is shown to the user
        /// </summary>
        /// <TestCaseID>SendAdHocTransferInformationTest_ResourceView_ValidateButtonVisibility</TestCaseID>
        [TestMethod]
        public void SendAdHocTransferInformationTest_ResourceView_ValidateButtonVisibility()
        {
            Resource resource = InitializeResourceScenario();
            bool? teardownAttribute = resource.Attributes.ContainsKey(amsOSRAMConstants.ResourceAttributeIsSorter) ? (bool)resource.Attributes[amsOSRAMConstants.ResourceAttributeIsSorter] : null;

            try
            {
                resource.SaveAttribute(amsOSRAMConstants.ResourceAttributeIsSorter, false);

                EntityPage<Resource> resourceEntityPage = NavigateToResourceViewPage(resource.Id);

                #region Validate AdHoc Transfer Button availability

                //Validate that AdHoc Transfer Button is not available
                ActionButton adHocTransferButton = resourceEntityPage.CreatePageObject<ActionButton>(By.CssSelector(SendAdHocTransferInformationButtonSelector));
                Assert.IsFalse(adHocTransferButton.IsEnabled, "AdHoc Transfer Button should not be available!");

                //Set IsSorter attribute to true
                resource.SaveAttribute(amsOSRAMConstants.ResourceAttributeIsSorter, true);

                //Refresh entity page
                ActionButton refreshButton = resourceEntityPage.CreatePageObject<ActionButton>(By.CssSelector("cmf-core-controls-actionbutton[button-id='Generic.Refresh']"));
                refreshButton.Click();

                WaitForLoadingStop();

                //Validate that AdHoc Transfer Button is now available
                adHocTransferButton = resourceEntityPage.CreatePageObject<ActionButton>(By.CssSelector(SendAdHocTransferInformationButtonSelector));
                Assert.IsTrue(adHocTransferButton.IsEnabled, "AdHoc Transfer Button should be available!");

                #endregion Validate AdHoc Transfer Button availability
            }
            finally
            {
                //Reset Resource IsSorter attribute
                if (teardownAttribute == null)
                {
                    resource.RemoveAttribute(amsOSRAMConstants.ResourceAttributeIsSorter);
                }
                else
                {
                    resource.LoadAttribute(amsOSRAMConstants.ResourceAttributeIsSorter);
                    resource.SaveAttribute(amsOSRAMConstants.ResourceAttributeIsSorter, teardownAttribute);
                }
            }
        }

        /// <summary>
        /// Description:
        ///     Validates the Wizard StepSorterProcess (AdHoc Transfer)
        ///     - The Source Load Port list should list all the load ports of a given resource
        ///     - The Product should list all the products
        ///     - Sorter Process should list all the values inside the LookupTable
        ///     - The Quantity should be filled automatically after choosing a Product
        ///         - This value should be the result of the source quantity of the SmartTable resolution
        ///     - On Transfer should call the DEE
        /// </summary>
        /// <TestCaseID>SendAdHocTransferInformationTest_WaferReception_HappyPath</TestCaseID>
        [TestMethod]
        public void SendAdHocTransferInformationTest_WaferReception_HappyPath()
        {
            Resource resource = InitializeResourceScenario();
            bool? teardownAttribute = resource.Attributes.ContainsKey(amsOSRAMConstants.ResourceAttributeIsSorter) ? (bool)resource.Attributes[amsOSRAMConstants.ResourceAttributeIsSorter] : null;
            SmartTableManager smartTableManager = new SmartTableManager();

            try
            {
                #region Resource and SmartTable setup

                resource.SaveAttribute(amsOSRAMConstants.ResourceAttributeIsSorter, true);

                string productName = amsOSRAMConstants.DefaultTestProductName;
                string sourceCapacity = "20";

                smartTableManager.ClearSmartTable(amsOSRAMConstants.CustomProductContainerCapacitiesSmartTable);
                smartTableManager.SetSmartTableData(amsOSRAMConstants.CustomProductContainerCapacitiesSmartTable, new Dictionary<string, string>
                {
                    { "Product", productName },
                    { "SourceCapacity", sourceCapacity },
                    { "TargetCapacity", "10" }
                });

                #endregion Resource and SmartTable setup

                #region Open wizard

                EntityPage<Resource> resourceEntityPage = NavigateToResourceEntityPage(resource.Id);

                ActionButton adHocTransferButton = resourceEntityPage.CreatePageObject<ActionButton>(By.CssSelector(SendAdHocTransferInformationButtonSelector));
                adHocTransferButton.Click();

                WaitForLoadingStop();

                //Create new version for DEE CustomSendAdHocTransferInformationToIoT and set as effetive
                //On Cleanup set the previous version as effetive and delete the version created here
                WizardStepSorterProcess wizardStepSorterProcess = CreatePageObject<WizardStepSorterProcess>(WizardStepSorterProcess.Selector);

                #endregion Open wizard

                #region Validate Parent Resource

                // Set the LoadPorts and Sorter Process fields
                Resource parentResource = wizardStepSorterProcess.Resource.Value;
                Assert.AreEqual(parentResource.Name, resource.Name, $"Should have the {resource.Name} displayed instead of {parentResource.Name}");
                parentResource.Load();

                #endregion Validate Parent Resource

                #region Validate Load Ports

                GetDescendentResourcesOutput getDescendentResources = new GetDescendentResourcesInput()
                {
                    Resource = parentResource,
                    Depth = 1
                }.GetDescendentResourcesSync();

                IEnumerable<Resource> loadPortsList = getDescendentResources.DescendentResources
                    .Where(w => w.ChildResource.ProcessingType == ProcessingType.LoadPort)
                    .Select(s => s.ChildResource);

                ResourceCollection loadPorts = new ResourceCollection();
                loadPorts.AddRange(loadPortsList);

                FindEntity<Resource> loadPortsFindEntity = wizardStepSorterProcess.SourceLoadPortPropertyEditor.Editor as FindEntity<Resource>;
                List<string> loadPortsNameList = loadPortsFindEntity.GetItems().Select(s => s.Text).ToList();

                Assert.AreEqual(loadPorts.Count, loadPortsNameList.Count, $"Should have {loadPorts.Count} load ports");
                Assert.AreEqual(loadPorts.Select(s => s.Name).Intersect(loadPortsNameList).ToList().Count, loadPortsNameList.Count, $"Load ports of the parent resource do not match the ones of the list");

                wizardStepSorterProcess.SourceLoadPort = loadPorts.FirstOrDefault();

                #endregion Validate Load Ports

                #region Validate Product

                Assert.IsTrue(wizardStepSorterProcess.Quantity == null || wizardStepSorterProcess.Quantity.ToString() == String.Empty, "Quantity should be empty");

                FindEntity<Product> productFindEntity = wizardStepSorterProcess.ProductPropertyEditor.Editor as FindEntity<Product>;
                productFindEntity.GetItems();

                Product product = new Product();
                product.Name = productName;

                wizardStepSorterProcess.Product = product;

                #endregion Validate Product

                #region Validate Sorter Process

                //Load SorterProcess LookupTable
                LookupTable sorterProcess = new LookupTable { Name = amsOSRAMConstants.CustomSorterProcessLookupTable };
                sorterProcess.Load();

                List<string> sorterProcessList = sorterProcess.Values.Where(w => w.IsEnabled.HasValue && w.IsEnabled.Value).Select(s => s.Value).ToList();
                LookupComboBox sorterProcessComboBox = wizardStepSorterProcess.SorterProcessPropertyEditor.Editor as LookupComboBox;
                List<string> sorterProcessComboBoxItems = sorterProcessComboBox.BusinessComboBox.Items.Select(s => s.Replace("\r\n", "").Trim().Split().FirstOrDefault()).ToList();

                Assert.AreEqual(sorterProcessComboBoxItems.Count, sorterProcessList.Count, $"Should have {loadPorts.Count} sorter process");
                Assert.AreEqual(sorterProcessComboBoxItems.Intersect(sorterProcessList).ToList().Count, sorterProcessList.Count, $"Sorter Process values from Lookup Table do not match the ones on the list");

                wizardStepSorterProcess.SorterProcess = sorterProcess.Values.FirstOrDefault(f => f.Value == amsOSRAMConstants.CustomSorterProcessWaferReception);

                #endregion Validate Sorter Process

                #region Validate Quantity

                Assert.AreEqual(wizardStepSorterProcess.Quantity, sourceCapacity, $"Quantity should be {sourceCapacity} but is {wizardStepSorterProcess.Quantity}");

                #endregion Validate Quantity
            }
            finally
            {
                if (teardownAttribute == null)
                {
                    resource.RemoveAttribute(amsOSRAMConstants.ResourceAttributeIsSorter);
                }
                else
                {
                    resource.LoadAttribute(amsOSRAMConstants.ResourceAttributeIsSorter);
                    resource.SaveAttribute(amsOSRAMConstants.ResourceAttributeIsSorter, teardownAttribute);
                }

                smartTableManager.UndoSmartTableChanges(amsOSRAMConstants.CustomProductContainerCapacitiesSmartTable);
            }
        }

        #region Help methods

        /// <summary>
        /// Description:
        ///     Logic for going to a Resource Entity Page:
        ///         - Navigates to the desired Resource Entity Page
        ///         - Returns EntityPage<Resource>
        /// </summary>
        private EntityPage<Resource> NavigateToResourceEntityPage(long resourceId)
        {
            //Navigates to Resource page
            NavigateTo(string.Format("Entity/Resource/{0}", resourceId));

            EntityPage<Resource> resourceEntityPage = CreatePageObject<EntityPage<Resource>>();
            WaitForLoadingStop();

            return resourceEntityPage;
        }

        /// <summary>
        /// Description:
        ///     Logic for going to a Resource Entity Page:
        ///         - Navigates to the desired Resource Entity Page
        ///         - Returns EntityPage<Resource>
        /// </summary>
        private EntityPage<Resource> NavigateToResourceViewPage(long resourceId)
        {
            NavigateToResourceEntityPage(resourceId);

            ActionButtonGroup viewsButton = CreatePageObject<ActionButtonGroup>("cmf-core-controls-actionbuttongroup[button-id='Generic.Views']");
            viewsButton.Click();
            WaitForLoadingStop();

            ActionButtonGroupButton resourceViewButton = CreatePageObject<ActionButtonGroupButton>("cmf-core-controls-actionbuttongroupbutton[data-tag='cmf.mes.resource.view.resourceview']");
            resourceViewButton.Click();
            WaitForLoadingStop();

            EntityPage<Resource> resourceViewPage = CreatePageObject<EntityPage<Resource>>();
            WaitForLoadingStop();

            return resourceViewPage;
        }

        /// <summary>
        /// Setup Resource Scenario
        /// </summary>
        private Resource InitializeResourceScenario()
        {
            Resource resource = new Resource { Name = "5FASH1" };

            // Load Resource which is not a Sorter
            resource.Load();
            resource.LoadAttribute(amsOSRAMConstants.ResourceAttributeIsSorter);

            return resource;
        }

        #endregion Help methods
    }
}