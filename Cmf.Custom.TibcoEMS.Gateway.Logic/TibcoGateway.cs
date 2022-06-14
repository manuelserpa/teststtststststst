using Cmf.Custom.TibcoEMS.Gateway.Logic.Common;
using Cmf.Custom.TibcoEMS.Gateway.Logic.DataStructures;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.OutputObjects;
using Cmf.MessageBus.Client;
using Cmf.MessageBus.Messages;
using Newtonsoft.Json;
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
        /// Message Bus Transport
        /// </summary>
        private Transport MessageBusTransport;

        /// <summary>
        /// Message Bus Transport Configuration
        /// </summary>
        private TransportConfig MessageBusTransportConfiguration;

        /// <summary>
        /// Tibco Connection
        /// </summary>
        private Connection TibcoConnection;

        /// <summary>
        /// Generic Table CustomTibcoEMSGatewayResolver data
        /// </summary>
        private Dictionary<string, TibcoResolverDto> TibcoResolveConfigurations;

        #endregion

        #region Constructor

        public TibcoGateway()
        {
            // Create Message Bus Transport Configuration
            this.MessageBusTransportConfiguration = TibcoGatewayUtilities.CreateMessageBusTransportConfig();

            // Associate configuration to Message Bus
            this.MessageBusTransport = new Transport(this.MessageBusTransportConfiguration);

            // Create Tibco Configuration
            //this.TibcoConnection = TibcoGatewayUtilities.CreateTibcoConnection();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// On Start Windows Service
        /// </summary>
        public void OnStart()
        {
            // Connect to Tibco
            //this.TibcoConnection.Start();

            // Connect to Message Bus
            this.MessageBusTransport.Start();

            // Subscribe subjects on Message Bus
            this.SubscribeSubjects();
        }

        /// <summary>
        /// On Stop Windows Service
        /// </summary>
        public void OnStop()
        {
            // Close Message Bus client connection
            if (this.MessageBusTransport != null)
            {
                this.UnsubscribeSubjects();

                this.MessageBusTransport.Stop();
            }

            // Close Tibco connection
            if (this.TibcoConnection != null)
            {
                this.TibcoConnection.Close();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is triggered when the subject contains a configuration on the Generic Table CustomTibcoEMSGatewayResolver 
        /// </summary>
        private void OnMessageReceived(string subject, MbMessage message)
        {
            if (message != null && !string.IsNullOrWhiteSpace(message.Data) && this.TibcoResolveConfigurations.ContainsKey(subject))
            {
                Task.Run(() =>
                {
                    try
                    {
                        // Topic name
                        string topicName = this.TibcoResolveConfigurations[subject].Topic;

                        // Action name
                        string actionName = this.TibcoResolveConfigurations[subject].Rule;

                        // Message to send
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
                        //this.SendMessageToTibco(topicName, messageData);

                        this.MessageBusTransport.Reply(message, "Ok");
                    }
                    catch (Exception ex)
                    {
                        // Send error details to MessageBus
                        string errorResponseJSON = JsonConvert.SerializeObject(new
                        {
                            ErrorCode = string.Empty,
                            ErrorText = ex.Message
                        });

                        this.MessageBusTransport.Reply(message, errorResponseJSON);
                    }
                });
            }
        }

        /// <summary>
        /// This method is triggered when Generic Table CustomTibcoEMSGatewayResolver is changed
        /// </summary>
        private void OnInvalidateCache(string subject, MbMessage message)
        {
            //Task.Run(() =>
            //{
            try
            {
                // Dictionary with updated configurations from Generic Table
                Dictionary<string, TibcoResolverDto> newTibcoConfigurations = TibcoGatewayUtilities.GetTibcoConfigurations();

                // Dictionary with subjects to unsubscribe
                var configurationsToUnsubscribe = this.TibcoResolveConfigurations.Where(row => !newTibcoConfigurations.ContainsKey(row.Key));

                // Dictionary with subjects to subscribe
                var configurationsToSubscribe = newTibcoConfigurations.Where(row => !this.TibcoResolveConfigurations.ContainsKey(row.Key));

                if (configurationsToUnsubscribe != null && configurationsToUnsubscribe.Any())
                {
                    // Unsubscribe Subjects
                    this.UnsubscribeSubjects(configurationsToUnsubscribe);
                }

                if (configurationsToSubscribe != null && configurationsToSubscribe.Any())
                {
                    // Subscribe Subjects
                    this.SubscribeSubjects(configurationsToSubscribe);
                }

                // Set global variable with the updated configurations on Generic Table
                this.TibcoResolveConfigurations = newTibcoConfigurations;

                this.MessageBusTransport.Reply(message, "Ok");
            }
            catch (Exception ex)
            {
                // Send error details to MessageBus
                string errorResponseJSON = JsonConvert.SerializeObject(new
                {
                    ErrorCode = string.Empty,
                    ErrorText = ex.Message
                });

                this.MessageBusTransport.Reply(message, errorResponseJSON);
            }
            //});
        }

        /// <summary>
        /// Subscribe topic and send message to Tibco
        /// </summary>
        private void SendMessageToTibco(string topicName, string messageData)
        {
            Session tibcoSession = this.TibcoConnection.CreateSession(false, SessionMode.AutoAcknowledge);

            Topic tibcoTopic = tibcoSession.CreateTopic(topicName);

            MessageProducer tibcoMessageProducer = tibcoSession.CreateProducer(tibcoTopic);

            MapMessage tibcoMessage = tibcoSession.CreateMapMessage();
            tibcoMessage.SetStringProperty("field", messageData);

            tibcoMessageProducer.Send(tibcoMessage);
        }

        /// <summary>
        /// Subscribe Subjects on Message Bus
        /// </summary>
        private void SubscribeSubjects(IEnumerable<KeyValuePair<string, TibcoResolverDto>> subjects = null)
        {
            IEnumerable<KeyValuePair<string, TibcoResolverDto>> subjectsToSubscribe = subjects;

            if (subjectsToSubscribe is null)
            {
                subjectsToSubscribe = this.TibcoResolveConfigurations = TibcoGatewayUtilities.GetTibcoConfigurations();

                // Subscribe subject associated to Generic Table CustomTibcoEMSGatewayResolver triggered on invalidate cache
                this.MessageBusTransport.Subscribe(TibcoGatewayConstants.SubjectCustomTibcoEMSGatewayInvalidateCache, OnInvalidateCache);
            }

            foreach (KeyValuePair<string, TibcoResolverDto> subject in subjectsToSubscribe)
            {
                this.MessageBusTransport.Subscribe(subject.Key, OnMessageReceived);
            }
        }

        /// <summary>
        /// Unsubscribe Subjects on Message Bus
        /// </summary>
        private void UnsubscribeSubjects(IEnumerable<KeyValuePair<string, TibcoResolverDto>> subjects = null)
        {
            IEnumerable<KeyValuePair<string, TibcoResolverDto>> subjectsToUnsubscribe = subjects;

            if (subjectsToUnsubscribe is null)
            {
                subjectsToUnsubscribe = this.TibcoResolveConfigurations;

                // Unsubscribe subject associated to Generic Table CustomTibcoEMSGatewayResolver triggered on invalidate cache
                this.MessageBusTransport.Unsubscribe(TibcoGatewayConstants.SubjectCustomTibcoEMSGatewayInvalidateCache);
            }

            foreach (KeyValuePair<string, TibcoResolverDto> subject in subjectsToUnsubscribe)
            {
                this.MessageBusTransport.Unsubscribe(subject.Key);
            }
        }

        #endregion
    }
}
