using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.OutputObjects;
using Cmf.Foundation.BusinessOrchestration.ErpManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.OutputObjects;
using Cmf.Foundation.BusinessOrchestration.LocalizationManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.QueryManagement.InputObjects;
using Cmf.Navigo.BusinessObjects;
using Cmf.TestScenarios.Others;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
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
            string productionOrderProduct = string.IsNullOrEmpty(productName) ? amsOSRAMConstants.DefaultTestProductName : productName;
            Product product = GenericGetsScenario.GetObjectByName<Product>(productionOrderProduct);

            string productionOrderFacility = string.IsNullOrEmpty(facilityName) ? amsOSRAMConstants.DefaultFacilityName : facilityName;
            Facility facility = GenericGetsScenario.GetObjectByName<Facility>(productionOrderFacility);
            flowPath = string.IsNullOrEmpty(flowPath) ? amsOSRAMConstants.DefaultTestFlowPath : flowPath;
            flowName = string.IsNullOrEmpty(flowName) ? amsOSRAMConstants.DefaultTestFlowName : flowName;
            stepName = string.IsNullOrEmpty(stepName) ? amsOSRAMConstants.DefaultTestStepName : stepName;
            
            Flow flow = GenericGetsScenario.GetObjectByName<Flow>(flowName);
            Step step = GenericGetsScenario.GetObjectByName<Step>(stepName);

            ProductionOrder productionOrder = new ProductionOrder
            {
                Name = productionOrderName,
                Type = amsOSRAMConstants.DefaultTestPOType,
                OrderNumber = productionOrderName,
                Quantity = (decimal)quantity,
                Units = string.IsNullOrEmpty(units) ? amsOSRAMConstants.DefaultMaterialUnit : units,
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

            string materialProduct = string.IsNullOrEmpty(productName) ? amsOSRAMConstants.DefaultTestProductName : productName;
            Product product = GenericGetsScenario.GetObjectByName<Product>(materialProduct);

            string materialFacility = string.IsNullOrEmpty(facilityName) ? amsOSRAMConstants.DefaultFacilityName : facilityName;

            string materialFlowPath = string.IsNullOrEmpty(flowPath) ? amsOSRAMConstants.DefaultTestFlowPath : flowPath;
            Facility facility = GenericGetsScenario.GetObjectByName<Facility>(materialFacility);

            Material material = new Material
            {
                Name = string.IsNullOrEmpty(name) ? GenerateName() : name,
                Type = string.IsNullOrEmpty(type) ? amsOSRAMConstants.DefaultMaterialType : type,
                Form = string.IsNullOrEmpty(form) ? amsOSRAMConstants.DefaultMaterialFormName : form,
                Facility = facility,
                Product = product,
                FlowPath = materialFlowPath,
                PrimaryQuantity = primaryQuantity == null ? (materialFlowPath == amsOSRAMConstants.DefaultTestFlowPath ? 0 : defaultPrimaryQuantity) : (decimal)primaryQuantity,
                SecondaryQuantity = secondaryQuantity,
                ProductionOrder = prodOrder,
                ExpirationDate = expirationDate,
                PossibleStartDate = prodOrder?.PlannedStartDate ?? DateTime.Now,
                PrimaryUnits = string.IsNullOrEmpty(units) ? amsOSRAMConstants.DefaultMaterialUnit : units,
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
            query.Query.Top = 1;
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
                },
                new Field()
                {
                    Alias = "ModifiedOn",
                    ObjectName = "IntegrationEntry",
                    ObjectAlias = "IntegrationEntry_1",
                    IsUserAttribute = false,
                    Name = "ModifiedOn",
                    Position = 2,
                    Sort = Cmf.Foundation.Common.FieldSort.Descending
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

        /// <summary>
        /// Query to retrieve the last created Integration Entry filtering by message type 
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public static IntegrationEntry GetLastIntegrationEntry(string messageType)
        {
            QueryObject query = new QueryObject();
            query.Description = "";
            query.EntityTypeName = "IntegrationEntry";
            query.Name = "CustomGetLastIE";
            query.Query = new Query();
            query.Query.Distinct = true;
            query.Query.Filters = new FilterCollection() {
                new Filter()
                {
                    Name = "MessageType",
                    ObjectName = "IntegrationEntry",
                    ObjectAlias = "IntegrationEntry_1",
                    Operator = Cmf.Foundation.Common.FieldOperator.Contains,
                    Value = messageType,
                    LogicalOperator = Cmf.Foundation.Common.LogicalOperator.AND,
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
                },
                new Field()
                {
                    Alias = "CreatedOn",
                    ObjectName = "IntegrationEntry",
                    ObjectAlias = "IntegrationEntry_1",
                    IsUserAttribute = false,
                    Name = "CreatedOn",
                    Position = 2,
                    Sort = Cmf.Foundation.Common.FieldSort.Descending
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
            integrationEntry.Load();
            return integrationEntry;
        }

        /// <summary>
        /// Wait for integration entry to be either failed or processed
        /// </summary>
        /// <param name="integrationEntry"></param>
        /// <param name="numberOfTries"></param>
        /// <param name="secondsBetweenAttempts"></param>
        public static void WaitForIntegrationEntryProcessFinish(
            IntegrationEntry integrationEntry, int numberOfTries = 15, int secondsBetweenAttempts = 1)
        {
            Int32 loops = 0;

            integrationEntry.Load();
            while ((int)integrationEntry.SystemState <= 1 && loops <= numberOfTries)
            {
                loops++;
                System.Threading.Thread.Sleep(secondsBetweenAttempts * 1000);
                integrationEntry.Load();
            }
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

        #region XML

        /// <summary>
        /// Deserialize Xml To Object
        /// </summary>
        /// <typeparam name="T">Serializable Class</typeparam>
        /// <param name="xml">XML</param>
        /// <returns>Object</returns>
        public static T DeserializeXmlToObject<T>(string xml)
        {
            T output;
            // Construct an instance of the XmlSerializer with the type
            // of object that is being deserialized.
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(xml))
            {
                // Call the Deserialize method and cast to the object type.
                output = (T)serializer.Deserialize(reader);
            }

            return output;
        }

        /// <summary>
        /// Serialize Object to XML
        /// </summary>
        /// <typeparam name="T">Serializable Type</typeparam>
        /// <param name="value">Object to be serialized</param>
        /// <returns></returns>
        public static string SerializeToXML<T>(this T value)
        {
            string output = string.Empty;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, value);
                output = writer.ToString();
            }

            return output;
        }

        #endregion

        #region LocalizedMessage Helper Methods

        /// <summary>
        /// Get Localized message by Key
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetLocalizedMessageByName(string name, params object[] paramsValue)
        {
            string message = new GetLocalizedMessageByNameInput
            {
                Name = name,
                Culture = new Culture(),
                IgnoreLastServiceId = true
            }.GetLocalizedMessageByNameSync().LocalizedMessage.MessageText;

            if (paramsValue != null && paramsValue.Any())
            {
                message = string.Format(message, paramsValue);
            }

            return message;
        }

        #endregion

        #region DEE

        public static Foundation.Common.DynamicExecutionEngine.Action UpdateOrCreateDEE(Foundation.Common.DynamicExecutionEngine.Action action, string codeToReplace = null)
        {
            if (action.Id <= 0 && !String.IsNullOrEmpty(action.Name))
            {
                try
                {
                    GetActionByNameOutput actionOutput = new GetActionByNameInput
                    {
                        Name = action.Name
                    }.GetActionByNameSync();

                    action = actionOutput.Action;
                } catch
                {

                }
            }

            Assert.IsNotNull(action, $"The DEE {action.Name} is missing");

            string actionCodeToSave = action.ActionCode;

            if (action.Name == null || action.Name == String.Empty || actionCodeToSave == null && actionCodeToSave == String.Empty)
            {
                throw new Exception("The DEE action must have a name and code");
            }

            if (codeToReplace != null && codeToReplace != String.Empty)
            {
                actionCodeToSave = actionCodeToSave.Replace("// DO NOT DELETE: This is a hook for test purposes", codeToReplace);
            }

            action.ActionCode = actionCodeToSave;

            if (action.Id <= 0)
            {
                action = new CreateActionInput()
                {
                    Action = action
                }.CreateActionSync().Action;

                action = new MakeActionVersionEffectiveInput()
                {
                    Action = action
                }.MakeActionVersionEffectiveSync().Action;
            } 
            else
            {
                action = new CompileAndSaveWithNewVersionInput()
                {
                    Action = action,
                    IsNewVersion = true,
                    IsToMakeNewVersionEffective = true
                }.CompileAndSaveWithNewVersionSync().Action;
            }

            return action;
        }

        public static Foundation.Common.DynamicExecutionEngine.Action RollbackDEE(long actionId)
        {
            GetActionByIdOutput getActionByNameOutput = new GetActionByIdInput()
            {
                Id = actionId
            }.GetActionByIdSync();

            return new MakeActionVersionEffectiveInput
            {
                Action = getActionByNameOutput.Action
            }.MakeActionVersionEffectiveSync().Action;
        }

        public static object RunDEEForTests(Foundation.Common.DynamicExecutionEngine.Action action, Dictionary<string, object> input)
        {
            if (action == null || action.Name == null || action.Name == String.Empty)
            {
                throw new Exception("The DEE action must have a name");
            }

            // Check if needs load
            if (action.ActionCode == null || action.ActionCode == String.Empty)
            {
                try
                {
                    action = new GetActionByNameInput()
                    {
                        Name = action.Name
                    }.GetActionByNameSync().Action;
                }
                catch
                {
                    throw new Exception($"The DEE with the name {action.Name} does not exist");
                }
            }

            ExecuteActionOutput deeOutput = new ExecuteActionInput()
            {
                Action = action,
                Input = input
            }.ExecuteActionSync();

            return deeOutput.Output.GetValueOrDefault("Result", null);
        }

        #endregion

        public static string GetEnvironmentName()
        {
            string ruleCode = @"string environmentMachineName = Environment.MachineName; Input[""Result""] = environmentMachineName.ToString();";

            Cmf.Foundation.Common.DynamicExecutionEngine.Action action = new Cmf.Foundation.Common.DynamicExecutionEngine.Action()
            {
                Name = "CustomGetEnvironmentMachineName",
                ActionCode = ruleCode,
                IsEnabled = true
            };

            action = UpdateOrCreateDEE(action);

            return (string)RunDEEForTests(action, new Dictionary<string, object>());
        }

        public static void WaitFor(this Func<bool> condition, int maxNumberOfRetries = 10, int msSleepTime = 500)
        {
            int retryCount = 0;
            do
            {
                if (condition?.Invoke() ?? true)
                {
                    break;
                }
                System.Threading.Thread.Sleep(msSleepTime);
            } while (retryCount++ < maxNumberOfRetries);
        }
    }
}
