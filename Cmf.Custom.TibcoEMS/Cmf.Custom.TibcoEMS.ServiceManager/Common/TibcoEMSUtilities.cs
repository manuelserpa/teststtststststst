using Cmf.Custom.TibcoEMS.ServiceManager.DataStructures;
using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessOrchestration.ApplicationSettingManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.OutputObjects;
using Cmf.Foundation.BusinessOrchestration.TableManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.Utilities.InputObjects;
using Cmf.Foundation.Common;
using Cmf.LightBusinessObjects.Infrastructure;
using Cmf.MessageBus.Client;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
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
        /// LBOs Client Configuration
        /// </summary>
        private static ClientConfiguration config = null;

        /// <summary>
        /// Custom Transport configuration
        /// </summary>
        public static TransportConfig messageBusTransportConfig = null;

        public static void InitialConfigurations()
        {
            ClientConfigurationProvider.ConfigurationFactory = () =>
            {
                if (config == null)
                {
                    config = new ClientConfiguration()
                    {
                        HostAddress = ConfigurationManager.AppSettings["HostAddress"],
                        ClientTenantName = ConfigurationManager.AppSettings["ClientTenantName"],
                        ApplicationName = ConfigurationManager.AppSettings["ApplicationName"],
                        UseSsl = bool.Parse(ConfigurationManager.AppSettings["UseSsl"] ?? "false"),
                        IsUsingLoadBalancer = bool.Parse(ConfigurationManager.AppSettings["IsUsingLoadBalancer"] ?? "false"),
                    };

                    string userName = ConfigurationManager.AppSettings.GetValue("UserName", "");
                    
                    if (String.IsNullOrEmpty(userName))
                    {
                        config.SecurityPortalBaseAddress = new Uri(ConfigurationManager.AppSettings["SecurityPortalBaseAddress"]);
                        config.SecurityAccessToken = ConfigurationManager.AppSettings["SecurityAccessToken"];
                    } 
                    else
                    {
                        config.UserName = userName;
                        config.Password = ConfigurationManager.AppSettings["Password"];
                    }
                }
                return config;
            };

            if (messageBusTransportConfig == null)
            {
                string transportConfigString = new GetApplicationBootInformationInput().GetApplicationBootInformationSync().TransportConfig;

                TransportConfig transportConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<TransportConfig>(transportConfigString);
                transportConfig.ApplicationName = ConfigurationManager.AppSettings["ApplicationName"];
                transportConfig.TenantName = ConfigurationManager.AppSettings["ClientTenantName"];

                messageBusTransportConfig = transportConfig;
            }
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
                    Name = TibcoEMSConstants.GenericTableCustomTibcoEMSGatewayResolverIsEnabledProperty,
                    Operator = FieldOperator.IsEqualTo,
                    Value = true,
                    LogicalOperator = LogicalOperator.Nothing
                }
            };

            // Execute service to get Generic Table results
            GenericTable genericTable = new GetGenericTableByNameWithFilterInput()
            {
                Name = TibcoEMSConstants.GenericTableCustomTibcoEMSGatewayResolver,
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

                output = dt.AsEnumerable().ToDictionary(row => row.Field<string>(TibcoEMSConstants.GenericTableCustomTibcoEMSGatewayResolverSubjectProperty),
                                                        row => new TibcoResolverDto
                                                        {
                                                            Subject = row.Field<string>(TibcoEMSConstants.GenericTableCustomTibcoEMSGatewayResolverSubjectProperty),
                                                            Topic = row.Field<string>(TibcoEMSConstants.GenericTableCustomTibcoEMSGatewayResolverTopicProperty),
                                                            ReplyTo = row.Field<string>(TibcoEMSConstants.GenericTableCustomTibcoEMSGatewayResolverReplyToProperty),
                                                            Rule = row.Table.Columns.Contains(TibcoEMSConstants.GenericTableCustomTibcoEMSGatewayResolverRuleProperty) ?
                                                                   row.Field<string>(TibcoEMSConstants.GenericTableCustomTibcoEMSGatewayResolverRuleProperty) :
                                                                   string.Empty,
                                                            IsQueue = row.Field<bool>(TibcoEMSConstants.GenericTableCustomTibcoEMSGatewayResolverQueueMessageProperty),
                                                            IsTextMessage = row.Field<bool>(TibcoEMSConstants.GenericTableCustomTibcoEMSGatewayResolverTextMessageProperty),
                                                            IsToCompress = row.Field<bool>(TibcoEMSConstants.GenericTableCustomTibcoEMSGatewayResolverCompressMessageProperty)
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

        #endregion Public Methods

        public static TextMessage CreateTextMessage(Session session, string messageData, Dictionary<string, string> headersData, bool isToCompressMessage)
        {
            // Create Tibco Text Message with associated message
            TextMessage tibcoTextMessage = session.CreateTextMessage(messageData);

            // Set Headers on TextMessage
            if (headersData != null && headersData.Any())
            {
                foreach (KeyValuePair<string, string> header in headersData)
                {
                    tibcoTextMessage.SetStringProperty(header.Key, header.Value);
                }
            }

            // Set property to compress Text Message only if it is defined on GT with value "true"
            if (isToCompressMessage)
            {
                tibcoTextMessage.SetBooleanProperty(TibcoEMSConstants.TibcoEMSPropertyCompressTextMessage, isToCompressMessage);
            }

            return tibcoTextMessage;
        }

        public static MapMessage CreateMapMessage(Session session, string messageData, Dictionary<string, string> headersData)
        {
            MapMessage tibcoMapMessage = session.CreateMapMessage();
            tibcoMapMessage.SetString(TibcoEMSConstants.TibcoEMSPropertyMapMessageField, messageData);

            // Set Headers on MapMessage
            if (headersData != null && headersData.Any())
            {
                foreach (KeyValuePair<string, string> header in headersData)
                {
                    tibcoMapMessage.SetStringProperty(header.Key, header.Value);
                }
            }

            return tibcoMapMessage;
        }
    }
}
