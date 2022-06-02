using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessOrchestration.TableManagement.InputObjects;
using Cmf.Foundation.Common;
using Cmf.MessageBus.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using TIBCO.EMS;

namespace Cmf.Custom.TibcoEMS.Gateway.Logic
{
    public static class TibcoGatewayUtilities
    {
        /// <summary>
        /// Create Message Bus configuration
        /// </summary>
        public static TransportConfig CreateMessageBusConfiguration()
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
        /// Create Tibco configuration
        /// </summary>
        public static Connection CreateTibcoConfiguration()
        {
            string host = ConfigurationManager.AppSettings["Tibco.Host"];
            string username = ConfigurationManager.AppSettings["Tibco.Username"];
            string password = ConfigurationManager.AppSettings["Tibco.Password"];

            ConnectionFactory factory = new ConnectionFactory(host);

            Connection connection = factory.CreateConnection(username, password);

            return connection;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<string, KeyValuePair<string, string>> GetTibcoGTResolverResults()
        {
            Dictionary<string, KeyValuePair<string, string>> output = null;

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
                Name = TibcoGatewayConstants.GTCustomTibcoEMSGatewayResolver,
                Filters = filters
            }.GetGenericTableByNameWithFilterSync().GenericTable;

            if (genericTable.HasData)
            {
                DataSet filteredResults = NgpDataSet.
                if (genericTable.Data.)
                {

                }
            }

            return output;
        }
    }
}
