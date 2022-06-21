using Cmf.Custom.TibcoEMS.Service.DataStructures;
using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessOrchestration.ConfigurationManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.ConfigurationManagement.OutputObjects;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.OutputObjects;
using Cmf.Foundation.BusinessOrchestration.TableManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.Utilities.InputObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;
using Cmf.MessageBus.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using TIBCO.EMS;

namespace Cmf.Custom.TibcoEMS.Service.Common
{
    public static class TibcoEMSUtilities
    {
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
            string userName = ConfigurationManager.AppSettings["UserName"];
            string externalAddress = ConfigurationManager.AppSettings["MessageBus.ExternalAddress"];

            bool useLoadBalancing = bool.TryParse(ConfigurationManager.AppSettings["MessageBus.UseLoadBalancing"], out useLoadBalancing) ? useLoadBalancing : false;

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

            if (useLoadBalancing)
            {
                string lbHost = ConfigurationManager.AppSettings["MessageBus.LoadBalancing.Host"];
                int lbPort = Convert.ToInt32(ConfigurationManager.AppSettings["MessageBus.LoadBalancing.Port"]);
                string lbSecurityToken = ConfigurationManager.AppSettings["MessageBus.LoadBalancing.SecurityToken"];

                bool lbEnableSsl = bool.TryParse(ConfigurationManager.AppSettings["MessageBus.LoadBalancing.UseSSL"], out lbEnableSsl) ? lbEnableSsl : false;

                messageBusConfiguration.UseLoadBalancing = true;
                messageBusConfiguration.LoadBalancerConfig = new LoadBalancerConfig
                {
                    Address = lbHost,
                    Port = lbPort,
                    EnableSsl = lbEnableSsl,
                    SecurityToken = lbSecurityToken
                };
            }

            return messageBusConfiguration;
        }

        /// <summary>
        /// Create Tibco connection
        /// </summary>
        public static Connection CreateTibcoConnection()
        {
            string host = ConfigurationManager.AppSettings["Tibco.Host"];
            string username = ConfigurationManager.AppSettings["Tibco.Username"];
            string password = ConfigurationManager.AppSettings["Tibco.Password"];

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

        public static ConfigCollection GetTibcoConfigs()
        {
            ConfigCollection result = new GetConfigsForPathInput()
            {
                Path = "/AMSOsram/TibcoEMS/"
            }.GetConfigsForPathSync().Configs;

            return result;
        }
    }
}
