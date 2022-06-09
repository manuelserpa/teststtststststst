using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.OutputObjects;
using Cmf.MessageBus.Client;
using Cmf.MessageBus.Messages;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private Dictionary<string, KeyValuePair<string, string>> GTTibcoResolver;

        #endregion

        #region Constructor

        public TibcoGateway()
        {
            this.MessageBusConfiguration = TibcoGatewayUtilities.CreateMessageBusConfiguration();
            this.MessageBus = new Transport(this.MessageBusConfiguration);

            //this.TibcoConnnector = TibcoGatewayUtilities.CreateTibcoConfiguration();

            this.GTTibcoResolver = TibcoGatewayUtilities.GetTibcoGTResolverResults();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Subscribe Cmf Message Bus
        /// </summary>
        public void Start()
        {
            try
            {
                // Connect to Message Bus
                this.MessageBus.Start();

                // Block until the client has connected to the Message Bus
                //if (!this.ConnectedSignalEvent.WaitOne(this.ConnectTimeout))
                //{
                //    // Failed to connect in the set interval window
                //    throw new Exception("Failed to connect to MessageBus");
                //}

                // Connect to Tibco
                //this.TibcoConnnector.Start();

                foreach (KeyValuePair<string, KeyValuePair<string, string>> item in this.GTTibcoResolver)
                {
                    this.MessageBus.Subscribe(item.Key, OnRequestReceived);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Unsubscribe Cmf Message Bus
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
            if (message != null && !String.IsNullOrEmpty(message.Data))
            {
                Task.Run(() =>
                {
                    try
                    {
                        // Tibco Topic
                        string topic = this.GTTibcoResolver[subject].Key;

                        // Action name
                        string actionName = this.GTTibcoResolver[subject].Value;

                        // Execute DEE
                        ExecuteActionOutput actionOutput = TibcoGatewayUtilities.ExecuteDEE(actionName, new Dictionary<string, object>());

                        if (!string.IsNullOrEmpty(topic) && actionOutput.Output.Count > 0 && actionOutput.Output != null)
                        {
                            // Send message to Tibco
                            this.SendMessageToTibco(topic, message.Data);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                });
            }
        }

        private void SendMessageToTibco(string subject, string message)
        {
            Session tibcoSession = this.TibcoConnnector.CreateSession(false, SessionMode.AutoAcknowledge);

            Topic tibcoTopic = tibcoSession.CreateTopic(subject);

            MessageProducer tibcoMessageProducer = tibcoSession.CreateProducer(tibcoTopic);

            MapMessage tibcoMessage = tibcoSession.CreateMapMessage();
            tibcoMessage.SetStringProperty("field", message);

            tibcoMessageProducer.Send(tibcoMessage);
        }

        #endregion
    }
}
