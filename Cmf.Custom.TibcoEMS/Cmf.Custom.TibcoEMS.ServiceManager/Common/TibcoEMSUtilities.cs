using Cmf.Custom.TibcoEMS.ServiceManager.DataStructures;
using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.OutputObjects;
using Cmf.Foundation.BusinessOrchestration.TableManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.Utilities.InputObjects;
using Cmf.Foundation.Common;
using Cmf.MessageBus.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using TIBCO.EMS;

namespace Cmf.Custom.TibcoEMS.ServiceManager.Common
{
    public static class TibcoEMSUtilities
    {
        #region Public Methods

        /// <summary>
        /// Create Message Bus configuration
        /// </summary>
        public static TransportConfig CreateMessageBusTransportConfig()
        {
            string host = ConfigurationManager.AppSettings["MessageBus.Host"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["MessageBus.Port"]);
            string securityToken = ConfigurationManager.AppSettings["MessageBus.SecurityToken"];

            string tenantName = ConfigurationManager.AppSettings["ClientTenantName"];
            string applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            string externalAddress = ConfigurationManager.AppSettings["MessageBus.ExternalAddress"];

            TransportConfig messageBusConfiguration = new TransportConfig()
            {
                GatewaysConfig = new List<GatewayConfig>
                {
                    new GatewayConfig()
                    {
                        Address = host,
                        Port = port,
                        ExternalAddress = externalAddress,
                    }
                },
                UseLoadBalancing = false,
                ApplicationName = applicationName,
                TenantName = tenantName,
                SecurityToken = securityToken,
                UseGatewayExternalAddress = externalAddress.Length > 0
            };

            return messageBusConfiguration;
        }

        /// <summary>
        /// Create Tibco connection
        /// </summary>
        public static Connection CreateTibcoConnection(NameValueCollection configurations)
        {
            string host = configurations["Host"];
            string username = configurations["Username"];
            string password = configurations["Password"];

            ConnectionFactory factory = new ConnectionFactory(host);

            Connection connection = factory.CreateConnection(username, password);

            return connection;
        }

        /// <summary>
        /// Execute DEE Action
        /// </summary>
        public static ExecuteActionOutput ExecuteDEE(string actionName, Dictionary<string, object> input)
        {
            ExecuteActionOutput output = new ExecuteActionInput
            {
                Action = new Foundation.Common.DynamicExecutionEngine.Action { Name = actionName },
                Input = input
            }.ExecuteActionSync();

            return output;
        }

        /// <summary>
        /// Get data "IsEnabled" from Generic Table related to topics to be subscribed by Tibco 
        /// </summary>
        public static Dictionary<string, TibcoResolverDto> GetTibcoConfigurations()
        {
            Dictionary<string, TibcoResolverDto> output = null;

            // Filter Generic Table by "IsEnabled" field
            FilterCollection filters = new FilterCollection()
            {
                new Filter()
                {
                    Name = "IsEnabled",
                    Operator = FieldOperator.IsEqualTo,
                    Value = true,
                    LogicalOperator = LogicalOperator.Nothing
                }
            };

            // Execute service to get Generic Table results
            GenericTable genericTable = new GetGenericTableByNameWithFilterInput()
            {
                Name = TibcoEMSConstants.GTCustomTibcoEMSGatewayResolver,
                Filters = filters
            }.GetGenericTableByNameWithFilterSync().GenericTable;

            DataSet ds = new ToDataSetInput()
            {
                NgpDataSet = genericTable.Data
            }.ToDataSetSync().DataSet;

            // Transform DataSet table to a Dictionary
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];

                output = dt.AsEnumerable().ToDictionary(row => row.Field<string>("Subject"),
                                                        row => new TibcoResolverDto
                                                        {
                                                            Subject = row.Field<string>("Subject"),
                                                            Topic = row.Field<string>("Topic"),
                                                            Rule = row.Table.Columns.Contains("Rule") ? row.Field<string>("Rule") : String.Empty
                                                        });
            }

            return output;
        }

        /// <summary>
        /// Get Tibco Configs from MES
        /// </summary>
        public static NameValueCollection GetTibcoConfigs()
        {
            NameValueCollection output = new NameValueCollection();

            ExecuteActionOutput actionResult = ExecuteDEE(TibcoEMSConstants.CustomGetTibcoConfigurations, new Dictionary<string, object>());

            if (actionResult != null && actionResult.Output.Any())
            {
                foreach (var config in actionResult.Output)
                {
                    output.Add(config.Key, config.Value.ToString());
                }
            }

            return output;
        }

        /// <summary>
        /// Check if a string is a valid JSON document.
        ///
        /// This method accepts strings that specify either an JSON
        /// object or an JSON array.
        /// </summary>
        /// <param name="value">
        /// The string to check.
        /// </param>
        /// <returns>
        /// Returns "true" if the string is a JSON document, "false"
        /// otherwise.
        /// </returns>
        public static bool IsJson(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            try
            {
                JsonConvert.DeserializeObject(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
