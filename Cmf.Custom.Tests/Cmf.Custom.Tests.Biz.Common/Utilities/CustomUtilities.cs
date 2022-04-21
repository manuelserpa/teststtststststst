using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessOrchestration.ErpManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.QueryManagement.InputObjects;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Cmf.TestScenarios.Others;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TestScenariosUtilities = Cmf.TestScenarios.Others.Utilities;

namespace Cmf.Custom.Tests.Biz.Common.Utilities
{
    public static class CustomUtilities
    {
        #region Create
        /// <summary>
        /// Creates a Production Order with given details.
        /// </summary>
        public static ProductionOrder CreateProductionOrder(
            string name = null,
            decimal? quantity = 1,
            int priority = 1,
            string facilityName = null,
            string productName = null,
            DateTime? startTime = null,
            DateTime? endTime = null,
            bool releasePO = true,
            string flowPath = null,
            string units = null,
            CustomTearDownManager tearDownManager = null,
            string flowName = null,
            string stepName = null)
        {

            string productionOrderName = string.IsNullOrEmpty(name) ? GenerateName() : name;
            string productionOrderProduct = string.IsNullOrEmpty(productName) ? AMSOsramConstants.DefaultTestProductName : productName;
            Product product = GenericGetsScenario.GetObjectByName<Product>(productionOrderProduct);

            string productionOrderFacility = string.IsNullOrEmpty(facilityName) ? AMSOsramConstants.DefaultFacilityName : facilityName;
            Facility facility = GenericGetsScenario.GetObjectByName<Facility>(productionOrderFacility);
            flowPath = string.IsNullOrEmpty(flowPath) ? AMSOsramConstants.DefaultTestFlowPath : flowPath;
            flowName = string.IsNullOrEmpty(flowName) ? AMSOsramConstants.DefaultTestFlowName : flowName;
            stepName = string.IsNullOrEmpty(stepName) ? AMSOsramConstants.DefaultTestStepName : stepName;
            
            Flow flow = GenericGetsScenario.GetObjectByName<Flow>(flowName);
            Step step = GenericGetsScenario.GetObjectByName<Step>(stepName);

            ProductionOrder productionOrder = new ProductionOrder
            {
                Name = productionOrderName,
                Type = AMSOsramConstants.DefaultTestPOType,
                OrderNumber = productionOrderName,
                Quantity = (decimal)quantity,
                Units = string.IsNullOrEmpty(units) ? AMSOsramConstants.DefaultMaterialUnit : units,
                Product = product,
                OverDeliveryTolerance = 0,
                PlannedStartDate = startTime ?? DateTime.Now,
                PlannedEndDate = endTime ?? DateTime.Now.AddDays(1),
                DueDate = endTime ?? DateTime.Now.AddDays(1),
                Priority = priority,
                Facility = facility,
                SystemState = !releasePO ? ProductionOrderSystemState.Created : ProductionOrderSystemState.Released,
                ValidateMaterialProducts = false,
                RestrictOnComplete = false,
                FlowPath = flowPath,
                IncludeInPlanning = false,
                Flow = flow,
                Step = step
            };

            productionOrder.Create();

            if (tearDownManager != null)
            {
                tearDownManager.Push(productionOrder);
            }

            return productionOrder;
        }

        /// <summary>
        /// Creates a material with given details.
        /// </summary>
        public static Material CreateMaterial(
            string name = null,
            string type = null,
            string form = null,
            string facilityName = null,
            string productName = null,
            string flowPath = null,
            decimal? primaryQuantity = null,
            decimal? secondaryQuantity = null,
            ProductionOrder prodOrder = null,
            CustomTearDownManager tearDownManager = null,
            DateTime? expirationDate = null,
            string units = null,
            bool isTemplate = false)
        {
            const decimal defaultPrimaryQuantity = 1;

            string materialProduct = string.IsNullOrEmpty(productName) ? AMSOsramConstants.DefaultTestProductName : productName;
            Product product = GenericGetsScenario.GetObjectByName<Product>(materialProduct);

            string materialFacility = string.IsNullOrEmpty(facilityName) ? AMSOsramConstants.DefaultFacilityName : facilityName;

            string materialFlowPath = string.IsNullOrEmpty(flowPath) ? AMSOsramConstants.DefaultTestFlowPath : flowPath;
            Facility facility = GenericGetsScenario.GetObjectByName<Facility>(materialFacility);

            Material material = new Material
            {
                Name = string.IsNullOrEmpty(name) ? GenerateName() : name,
                Type = string.IsNullOrEmpty(type) ? AMSOsramConstants.DefaultMaterialType : type,
                Form = string.IsNullOrEmpty(form) ? AMSOsramConstants.DefaultMaterialFormName : form,
                Facility = facility,
                Product = product,
                FlowPath = materialFlowPath,
                PrimaryQuantity = primaryQuantity == null ? (materialFlowPath == AMSOsramConstants.DefaultTestFlowPath ? 0 : defaultPrimaryQuantity) : (decimal)primaryQuantity,
                SecondaryQuantity = secondaryQuantity,
                ProductionOrder = prodOrder,
                ExpirationDate = expirationDate,
                PossibleStartDate = prodOrder?.PlannedStartDate ?? DateTime.Now,
                PrimaryUnits = string.IsNullOrEmpty(units) ? AMSOsramConstants.DefaultMaterialUnit : units,
                IsTemplate = isTemplate
            };

            material.Create();

            if (tearDownManager != null)
            {
                tearDownManager.Push(material);
            }

            return material;
        }
        #endregion


        #region Generators
        /// <summary>
        /// Generates a name based on the test method.
        /// Format : [Prefix]_[RandomGuid]_[TestName]
        /// or
        /// [TestName]_[RandomGuid]
        /// </summary>
        /// <returns></returns>
        public static string GenerateName(string prefix = "", int guidLengthTrim = 5)
        {
            string newGuid = TestScenarios.Others.Utilities.NewGuid();

            if (guidLengthTrim > 0)
            {
                newGuid = newGuid.Substring(0, guidLengthTrim);
            }

            return string.Empty.Equals(prefix) ? string.Format("{0}_{1}", GetTestMethodName(), newGuid) : string.Format("{0}_{1}_{2}", prefix, newGuid, GetTestMethodName());
        }

        /// <summary>
        /// Gets the Test Method Name
        /// </summary>
        /// <returns></returns>
        public static string GetTestMethodName()
        {
            // for when it runs via Visual Studio locally
            StackTrace stackTrace = new StackTrace();
            foreach (StackFrame stackFrame in stackTrace.GetFrames())
            {
                MethodBase methodBase = stackFrame.GetMethod();
                object[] attributes = methodBase.GetCustomAttributes(typeof(TestMethodAttribute), false);
                if (attributes.Length >= 1)
                {
                    return methodBase.Name;
                }
            }
            return "Not called from a test method";
        }
        #endregion

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

        /// <summary>
        /// Method to get an integration entry by name
        /// </summary>
        /// <param name="name"> Name of the Integration Entry </param>
        /// <returns></returns>
        public static IntegrationEntry GetIntegrationEntry(string name)
        {
            QueryObject query = new QueryObject();
            query.Description = "";
            query.EntityTypeName = "IntegrationEntry";
            query.Name = "CustomGetIntegrationEntries";
            query.Query = new Query();
            query.Query.Distinct = true;
            query.Query.Filters = new FilterCollection() {
                new Filter()
                {
                    Name = "Name",
                    ObjectName = "IntegrationEntry",
                    ObjectAlias = "IntegrationEntry_1",
                    Operator = Cmf.Foundation.Common.FieldOperator.Contains,
                    Value = name,
                    LogicalOperator = Cmf.Foundation.Common.LogicalOperator.Nothing,
                    FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal,
                }
            };
            query.Query.Fields = new FieldCollection() {
                new Field()
                {
                    Alias = "Id",
                    ObjectName = "IntegrationEntry",
                    ObjectAlias = "IntegrationEntry_1",
                    IsUserAttribute = false,
                    Name = "Id",
                    Position = 0,
                    Sort = Cmf.Foundation.Common.FieldSort.NoSort
                },
                new Field()
                {
                    Alias = "Name",
                    ObjectName = "IntegrationEntry",
                    ObjectAlias = "IntegrationEntry_1",
                    IsUserAttribute = false,
                    Name = "Name",
                    Position = 1,
                    Sort = Cmf.Foundation.Common.FieldSort.NoSort
                }
            };
            query.Query.Relations = new RelationCollection();

            // Execute Query 
            DataSet dataSet = TestScenariosUtilities.ToDataSet(CustomUtilities.ExecuteQueryObject(query));

            IntegrationEntry integrationEntry = new IntegrationEntry();

            if (dataSet.HasData())
            {
                DataRow row = dataSet.Tables[0].Rows[0];
                integrationEntry.Name = row.Field<string>("Name");
            }

            return integrationEntry;
        }

        #endregion

        #region Query Object

        /// <summary>
        /// Executes the query object.
        /// </summary>
        /// <param name="queryObject">The query object.</param>
        /// <param name="queryParameters">The query parameters.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <returns></returns>
        public static NgpDataSet ExecuteQueryObject(QueryObject queryObject, QueryParameterCollection queryParameters = null, int pageSize = 10, int pageNumber = 1)
        {
            return new ExecuteQueryInput()
            {
                QueryObject = queryObject,
                PageSize = pageSize,
                PageNumber = pageNumber,
                QueryParameters = queryParameters
            }.ExecuteQuerySync().NgpDataSet;
        }

        #endregion

        #region Generic

        /// <summary>
        /// Get Value as nullable decimal.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <returns></returns>
        public static decimal? GetValueAsNullableDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return decimal.Parse(value.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), NumberStyles.Number | NumberStyles.AllowExponent);
        }

        /// <summary>
        /// Get Value as nullable boolean
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <returns></returns>
        public static bool? GetValueAsNullableBoolean(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            // True: Possible values 
            string[] positiveValues = { "y", "true", "yes", "1" };

            if (positiveValues.Contains(value.Trim(), StringComparer.InvariantCultureIgnoreCase))
            {
                return true;
            }

            // False: Possible values
            string[] negativeValues = { "n", "false", "no", "0" };

            if (negativeValues.Contains(value.Trim(), StringComparer.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return default(bool?);
        }

        /// <summary>
        /// Get Value as decimal.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <returns></returns>
        public static decimal GetValueAsDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return default(decimal);
            }

            return decimal.Parse(value.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), NumberStyles.Number | NumberStyles.AllowExponent);
        }

        /// <summary>
        /// Get Value as boolean.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <returns></returns>
        public static bool GetValueAsBoolean(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string[] booleanValues = { "y", "true", "yes", "1" };

            if (booleanValues.Contains(value.Trim(), StringComparer.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return default(bool);
        }

        /// <summary>
        /// Get Value as dynamic DataType.
        /// </summary>
        /// <param name="parameterDataType">Parameter Data Type.</param>
        /// <param name="value">Value to be converted.</param>
        /// <returns></returns>
        public static dynamic GetParameterValueAsDataType(ParameterDataType parameterDataType, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return default(dynamic);
            }

            switch (parameterDataType)
            {
                case ParameterDataType.Decimal:
                    return GetValueAsNullableDecimal(value);

                case ParameterDataType.Boolean:
                    return GetValueAsNullableBoolean(value);

                default:
                    return value;
            }
        }

        /// <summary>
        /// Get Value as dynamic DataType.
        /// </summary>
        /// <param name="scalarType">Scalar Type.</param>
        /// <param name="value">Value to be converted.</param>
        /// <returns></returns>
        public static dynamic GetAttributeValueAsDataType(ScalarType scalarType, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return default(dynamic);
            }

            switch (scalarType.NativeType)
            {
                case "System.Decimal":
                    return GetValueAsNullableDecimal(value);

                case "System.Boolean":
                    return GetValueAsNullableBoolean(value);

                default:
                    return value;
            }
        }

        #endregion
    }
}
