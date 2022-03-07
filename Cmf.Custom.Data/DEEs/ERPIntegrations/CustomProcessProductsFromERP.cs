using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.ERP;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common.Base;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmf.Custom.AMSOsram.Actions.ERPIntegrations
{
    public class CustomProcessProductsFromERP : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---   

            #region Info
            /// <summary>
            /// Summary text
            ///     
            /// Action Groups:
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion

            IntegrationEntry integrationEntry = AMSOsramUtilities.GetInputItem<IntegrationEntry>(Input, "IntegrationEntry");

            if (integrationEntry is null || integrationEntry.IntegrationMessage is null || integrationEntry.IntegrationMessage.Message is null || integrationEntry.IntegrationMessage.Message.Length <= 0)
            {
                return false;
            }

            return true;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            //System
            UseReference("", "System");
            UseReference("", "System.Collections.Generic");
            UseReference("", "System.Linq");
            UseReference("", "System.Text");

            //Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.Common.Base.dll", "Cmf.Foundation.Common.Base");

            //Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");

            //Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.ERP");

            // Load Integration Entry
            IntegrationEntry integrationEntry = AMSOsramUtilities.GetInputItem<IntegrationEntry>(Input, "IntegrationEntry");

            // Cast Integation Entry Message to string
            string message = Encoding.UTF8.GetString(integrationEntry.IntegrationMessage.Message);

            // Deserialize XML Message to an object
            ProcessProductDataOutput processProduct = AMSOsramUtilities.DeserializeXmlToObject<ProcessProductDataOutput>(message);

            #region Product

            // Collection used to store new Products
            ProductCollection nonExistentProducts = new ProductCollection();

            // Collection used to store existing Products
            ProductCollection existingProducts = new ProductCollection();

            // ChangeSet to create/update Products
            ChangeSet productsChangeSet = new ChangeSet();
            {
                productsChangeSet.Name = Guid.NewGuid().ToString("N");
                productsChangeSet.Description = $"ChangeSet to create/update Products.";
                productsChangeSet.Type = $"General";
                productsChangeSet.MakeEffectiveOnApproval = true;
            }

            productsChangeSet.Create();

            // Create Product context using data received from Integration Entry
            foreach (ProcessProductData productData in processProduct.ProcessProductsData)
            {
                Product product = new Product();
                product.Name = productData.Name;
                product.Description = productData.Description;
                product.Type = productData.Type;
                product.ProductType = ProductType.FinishedGood; //productData.ProductType
                product.DefaultUnits = productData.DefaultUnits;
                product.IsEnabled = productData.IsEnabled.ToUpper() == "Y" ? true : false;
                product.Yield = Convert.ToDecimal(productData.Yield) / 100; // How to convert the value?

                ProductGroup productGroup = new ProductGroup();
                {
                    productGroup.Name = productData.ProductGroup;
                }

                if (!productGroup.ObjectExists())
                {
                    // TO DO
                }
                else
                {
                    productGroup.Load();
                }

                product.ProductGroup = productGroup;
                product.MaximumMaterialSize = Convert.ToDecimal(productData.MaximumMaterialSize);

                if (product.ObjectExists())
                {
                    existingProducts.Add(product);
                }
                else
                {
                    nonExistentProducts.Add(product);
                }
            }

            // Update Existing Products
            if (existingProducts != null && existingProducts.Any())
            {
                existingProducts.Load();

                existingProducts.CreateVersion(false, productsChangeSet, string.Empty);
            }

            // Create Non-Existing Products
            if (nonExistentProducts != null && nonExistentProducts.Any())
            {
                nonExistentProducts.CreateVersion(true, productsChangeSet, string.Empty);
            }

            #endregion

            #region Product/Parameters

            // Disassociate or associate relation between Product and Parameters
            foreach (ProcessProductData productData in processProduct.ProcessProductsData)
            {
                // Load Product
                Product product = new Product();
                {
                    product.Name = productData.Name;

                    product.Load();
                }

                #region Desassociate Parameters

                // Disassociate relation between Product and removed Parameters
                ParameterSourceCollection parametersToRemoveRelations = new ParameterSourceCollection();
                parametersToRemoveRelations.AddRange(product.GetProductParameters().Where(w => !productData.ProcessProductParameters.Any(a => w.Parameter.Name == a.Name)));

                if (parametersToRemoveRelations != null && parametersToRemoveRelations.Any())
                {
                    ProductParameterCollection productParametersToRemove = new ProductParameterCollection();

                    foreach (ParameterSource parameterToRemove in parametersToRemoveRelations)
                    {
                        ProductParameter productParameter = new ProductParameter();
                        {
                            productParameter.SourceEntity = product;
                            productParameter.TargetEntity = parameterToRemove.Parameter;

                            productParametersToRemove.Add(productParameter);
                        }
                    }

                    if (productParametersToRemove != null && productParametersToRemove.Any())
                    {
                        // Remove relations between Product and Parameters
                        product.RemoveRelations(productParametersToRemove);
                    }
                }

                #endregion

                #region Associate Parameters

                ProductParameterCollection productParametersCollection = new ProductParameterCollection();

                // Associate relation between Product to Parameter
                foreach (ProcessProductParametersData parameterData in productData.ProcessProductParameters)
                {
                    Parameter parameter = new Parameter();
                    {
                        parameter.Name = parameterData.Name;
                    }

                    if (parameter.ObjectExists())
                    {
                        parameter.Load();
                    }
                    else
                    {
                        parameter.Description = parameterData.Name;
                        parameter.Type = "Production";
                        parameter.DisplayName = parameter.Name;
                        parameter.ParameterScope = ParameterScope.EDC_SPC;
                        parameter.DataType = ParameterDataType.String;

                        parameter.Create();
                    }

                    ProductParameter productParameter = new ProductParameter();
                    {
                        productParameter.SourceEntity = product;
                        productParameter.TargetEntity = parameter;
                        productParameter.Value = parameterData.Value;

                        productParametersCollection.Add(productParameter);
                    }
                }

                if (productParametersCollection != null && productParametersCollection.Any())
                {
                    // Add relation between Product and Parameters
                    product.AddRelations(productParametersCollection);
                }

                #endregion
            }

            productsChangeSet.Load();

            productsChangeSet.RequestApproval();

            if (productsChangeSet.SystemState == ChangeSetSystemState.InApproval)
            {
                productsChangeSet.Approve();
            }

            if (productsChangeSet.UniversalState == UniversalState.Active)
            {
                productsChangeSet.MakeObjectsEffective();
            }

            if (productsChangeSet.UniversalState != UniversalState.Terminated)
            {
                productsChangeSet.Terminate();
            }

            #endregion

            //---End DEE Code---

            return Input;
        }
    }
}
