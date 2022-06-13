using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.OutputObjects;
using Cmf.MessageBus.Client;
using Cmf.MessageBus.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Tibco Connection
        /// </summary>
        private Connection TibcoConnnector;

        /// <summary>
        /// Generic Table CustomTibcoEMSGatewayResolver data
        /// </summary>
        private Dictionary<string, GenericTableTibcoResolver> GTTibcoResolver;

        #endregion

        #region Constructor

        public TibcoGateway()
        {
            // Create Message Bus Configuration
            this.MessageBusConfiguration = TibcoGatewayUtilities.CreateMessageBusConfiguration();

            // Associate configuration to Message Bus
            this.MessageBus = new Transport(this.MessageBusConfiguration);

            // Create Tibco Configuration
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

                // Subscribe subjects on Message Bus
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

        /// <summary>
        /// This method is triggered when the subject contains a configuration on the Generic Table CustomTibcoEMSGatewayResolver 
        /// </summary>
        private void OnRequestReceived(string subject, MbMessage message)
        {
            if (message != null && !string.IsNullOrEmpty(message.Data))
            {
                Task.Run(() =>
                {
                    try
                    {
                        string topicName = this.GTTibcoResolver[subject].Topic;

                        string actionName = this.GTTibcoResolver[subject].Rule;

                        string messageData = message.Data;

                        if (!string.IsNullOrWhiteSpace(actionName))
                        {
                            // Execute DEE
                            ExecuteActionOutput actionOutput = TibcoGatewayUtilities.ExecuteDEE(actionName, new Dictionary<string, object>() { { "Data", message.Data } });

                            if (actionOutput.Output != null &&
                                actionOutput.Output.Any() &&
                                actionOutput.Output.ContainsKey("Result") &&
                                !string.IsNullOrWhiteSpace(actionOutput.Output["Result"].ToString()))
                            {
                                messageData = actionOutput.Output["Result"].ToString();
                            }
                        }

                        // Send message to Tibco
                        this.SendMessageToTibco(topicName, messageData);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                });
            }
        }

        /// <summary>
        /// This method is triggered when Generic Table CustomTibcoEMSGatewayResolver is changed
        /// </summary>
        private void OnRequestGenericTableChange(string subject, MbMessage message)
        {
            Task.Run(() =>
            {
                try
                {
                    this.UnsubscribeSubjects();

                    this.SubscribeSubjects();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }

        /// <summary>
        /// Subscribe topic and send message to Tibco
        /// </summary>
        private void SendMessageToTibco(string topicName, string messageData)
        {
            Session tibcoSession = this.TibcoConnnector.CreateSession(false, SessionMode.AutoAcknowledge);

            Topic tibcoTopic = tibcoSession.CreateTopic(topicName);

            MessageProducer tibcoMessageProducer = tibcoSession.CreateProducer(tibcoTopic);

            MapMessage tibcoMessage = tibcoSession.CreateMapMessage();
            tibcoMessage.SetStringProperty("field", messageData);

            tibcoMessageProducer.Send(tibcoMessage);
        }

        /// <summary>
        /// Subscribe Subjects on Message Bus based on Generic Table CustomTibcoEMSGatewayResolver
        /// </summary>
        private void SubscribeSubjects()
        {
            this.GTTibcoResolver = TibcoGatewayUtilities.GetTibcoGTResolverResults();

            foreach (KeyValuePair<string, GenericTableTibcoResolver> item in this.GTTibcoResolver)
            {
                this.MessageBus.Subscribe(item.Key, OnRequestReceived);
            }

            // Subscribe subject associated to Generic Table CustomTibcoEMSGatewayResolver triggered on invalidate cache
            this.MessageBus.Subscribe(TibcoGatewayConstants.SubjectCustomTibcoEMSGatewayResolver, OnRequestGenericTableChange);
        }

        /// <summary>
        /// Unsubscribe Subjects on Message Bus
        /// </summary>
        private void UnsubscribeSubjects()
        {
            foreach (KeyValuePair<string, GenericTableTibcoResolver> item in this.GTTibcoResolver)
            {
                this.MessageBus.Unsubscribe(item.Key);
            }

            // Unsubscribe subject associated to Generic Table CustomTibcoEMSGatewayResolver triggered on invalidate cache
            this.MessageBus.Unsubscribe(TibcoGatewayConstants.SubjectCustomTibcoEMSGatewayResolver);
        }

        #endregion
    }
}
