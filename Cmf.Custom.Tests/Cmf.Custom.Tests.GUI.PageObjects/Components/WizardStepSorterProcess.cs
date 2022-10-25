using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using Cmf.Core.Business.Controls.PageObjects.Components;
using Cmf.Core.Business.Controls.PageObjects.Util;
using Cmf.Core.PageObjects;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using OpenQA.Selenium;

namespace Cmf.Custom.Tests.GUI.PageObjects.Components
{
    public class WizardStepSorterProcess : TransactionWizard
    {
        #region Properties

        /// <summary>
        /// Page Selector
        /// </summary>
        public static new string Selector
        {
            get
            {
                return "customization-amsosram-resource-customwizardadhoctransfer";
            }
        }

        /// <summary>
        /// Source Load Port field
        /// </summary>
        public PropertyViewer<Resource> Resource
        {
            get
            {
                return PropertyViewerUtil.GetItem<Resource>(this, PropertyViewerUtil.getSelector("Main Resource", "data-label"));
            }
        }

        /// <summary>
        /// Source Load Port editor
        /// </summary>
        public PropertyEditor<Resource> SourceLoadPortPropertyEditor
        {
            get
            {
                return PropertyEditorFormUtil.GetItem<Resource>(this, PropertyEditorFormUtil.getSelector("Source Load Port", "data-label"));
            }
        }

        /// <summary>
        /// Source Load Port field
        /// </summary>
        public Resource SourceLoadPort
        {
            get
            {
                return this.SourceLoadPortPropertyEditor.Value;
            }
            set
            {
                this.SourceLoadPortPropertyEditor.Value = value;
            }
        }

        /// <summary>
        /// Product PropertyEditor
        /// </summary>
        public PropertyEditor<Product> ProductPropertyEditor
        {
            get
            {
                return PropertyEditorFormUtil.GetItem<Product>(this, PropertyEditorFormUtil.getSelector("Product", "data-label"));
            }
        }

        /// <summary>
        /// Product field
        /// </summary>
        public Product Product
        {
            get
            {
                return this.ProductPropertyEditor.Value;
            }
            set
            {
                this.ProductPropertyEditor.Value = value;
            }
        }

        /// <summary>
        /// Sorter Process PropertyEditor
        /// </summary>
        public PropertyEditor<LookupTableValue> SorterProcessPropertyEditor
        {
            get
            {
                return PropertyEditorFormUtil.GetItem<LookupTableValue>(this, PropertyEditorFormUtil.getSelector("Sorter Process", "data-label"));
            }
        }

        /// <summary>
        /// Sorter Process field
        /// </summary>
        public LookupTableValue SorterProcess
        {
            get
            {
                return this.SorterProcessPropertyEditor.Value;
            }
            set
            {
                this.SorterProcessPropertyEditor.Value = value;
            }
        }

        /// <summary>
        /// Quantity PropertyEditor
        /// </summary>
        public PropertyEditor<string> QuantityPropertyEditor
        {
            get
            {
                return PropertyEditorFormUtil.GetItem<string>(this, PropertyEditorFormUtil.getSelector("Quantity", "data-label"));
            }
        }

        /// <summary>
        /// Quantity field
        /// </summary>
        public string Quantity
        {
            get
            {
                return this.QuantityPropertyEditor.Value;
            }
            set
            {
                this.QuantityPropertyEditor.Value = value;
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public WizardStepSorterProcess(PageContext context) : base(context)
        {
            WaitForLoadingStop();
        }

        #endregion Constructors
    }
}
