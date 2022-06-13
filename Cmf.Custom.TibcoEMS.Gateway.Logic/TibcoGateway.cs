using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.OutputObjects;
using Cmf.MessageBus.Client;
using Cmf.MessageBus.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TIBCO.EMS;

namespace Cmf.Custom.TibcoEMS.Gateway.Logic
{
    public class TibcoGateway
    {
        #region Private Variables

        /// <summary>
        /// Message Bus
        /// </summary>
        private Transport MessageBus;

        /// <summary>
        /// Message Bus Configuration
        /// </summary>
        private TransportConfig MessageBusConfiguration;

        /// <summary>
        /// Manual Reset Event
        /// </summary>
        private ManualResetEvent ConnectedSignalEvent = new ManualResetEvent(false);

        /// <summary>
        /// Tibco Configuration
        /// </summary>
        private Connection TibcoConnnector;

        private int _connectTimeout;

        /// <summary>
        /// Connect Timeout
        /// </summary>
        private int ConnectTimeout
        {
            get => int.TryParse(ConfigurationManager.AppSettings["MessageBus.ConnectTimeout"], out _connectTimeout) ? _connectTimeout : 10000;
        }

        private int _requestTimeout;

        /// <summary>
        /// Request Timeout
        /// </summary>
        private int RequestTimeout
        {
            get => int.TryParse(ConfigurationManager.AppSettings["MessageBus.RequestTimeout"], out _requestTimeout) ? _requestTimeout : 10000;
        }

        /// <summary>
        /// GenericTable Tibco Resolver
        /// </summary>
        private Dictionary<string, GenericTableTibcoResolver> GTTibcoResolver;

        #endregion

        #region Constructor

        public TibcoGateway()
        {
            this.MessageBusConfiguration = TibcoGatewayUtilities.CreateMessageBusConfiguration();
            this.MessageBus = new Transport(this.MessageBusConfiguration);

            //this.TibcoConnnector = TibcoGatewayUtilities.CreateTibcoConfiguration();

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// On Start Windows Service
        /// </summary>
        public void Start()
        {
            try
            {
                // Connect to Message Bus
                this.MessageBus.Start();

                // Connect to Tibco
                //this.TibcoConnnector.Start();

                this.SubscribeSubjects();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// On Stop Windows Service
        /// </summary>
        public void Stop()
        {
            try
            {
                // Close Tibco connection
                if (this.TibcoConnnector != null)
                {
                    this.TibcoConnnector.Close();
                }

                // Close Message Bus client connection
                if (this.MessageBus != null)
                {
                    this.UnsubscribeSubjects();

                    this.MessageBus.Stop();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Private Methods

        private void OnRequestReceived(string subject, MbMessage message)
        {
            if (message != null && !string.IsNullOrEmpty(message.Data))
            {
                Task.Run(() =>
                {
                    try
                    {
                        switch (subject)
                        {
                            case TibcoGatewayConstants.SubjectCustomTibcoEMSGatewayResolver:
                                {
                                    this.UnsubscribeSubjects();

                                    this.SubscribeSubjects();

                                    break;
                                }
                            default:
                                {
                                    string topicName = this.GTTibcoResolver[subject].Topic;

                                    string actionName = this.GTTibcoResolver[subject].Rule;

                                    string messageData = message.Data;

                                    if (!string.IsNullOrWhiteSpace(actionName))
                                    {
                                        // Execute DEE
                                        ExecuteActionOutput actionOutput = TibcoGatewayUtilities.ExecuteDEE(actionName, JsonConvert.DeserializeObject<Dictionary<string, object>>(message.Data));

                                        if (actionOutput.Output != null &&
                                            actionOutput.Output.Any() &&
                                            actionOutput.Output.ContainsKey("Message") &&
                                            !string.IsNullOrWhiteSpace(actionOutput.Output["Message"].ToString()))
                                        {
                                            messageData = actionOutput.Output["Message"].ToString();
                                        }
                                    }

                                    // Send message to Tibco
                                    this.SendMessageToTibco(topicName, messageData);

                                    break;
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                });
            }
        }

        private void SendMessageToTibco(string topicName, string messageData)
        {
            Session tibcoSession = this.TibcoConnnector.CreateSession(false, SessionMode.AutoAcknowledge);

            Topic tibcoTopic = tibcoSession.CreateTopic(topicName);

            MessageProducer tibcoMessageProducer = tibcoSession.CreateProducer(tibcoTopic);

            MapMessage tibcoMessage = tibcoSession.CreateMapMessage();
            tibcoMessage.SetStringProperty("field", messageData);

            tibcoMessageProducer.Send(tibcoMessage);
        }

        private void SubscribeSubjects()
        {
            this.GTTibcoResolver = TibcoGatewayUtilities.GetTibcoGTResolverResults();

            foreach (KeyValuePair<string, GenericTableTibcoResolver> item in this.GTTibcoResolver)
            {
                this.MessageBus.Subscribe(item.Key, OnRequestReceived);
            }

            this.MessageBus.Subscribe(TibcoGatewayConstants.SubjectCustomTibcoEMSGatewayResolver, OnRequestReceived);
        }

        private void UnsubscribeSubjects()
        {
            foreach (KeyValuePair<string, GenericTableTibcoResolver> item in this.GTTibcoResolver)
            {
                this.MessageBus.Unsubscribe(item.Key);
            }

            this.MessageBus.Unsubscribe(TibcoGatewayConstants.SubjectCustomTibcoEMSGatewayResolver);
        }

        #endregion
    }
}
