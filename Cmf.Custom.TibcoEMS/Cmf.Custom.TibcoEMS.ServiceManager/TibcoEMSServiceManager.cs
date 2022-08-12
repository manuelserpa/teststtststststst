using Cmf.Custom.TibcoEMS.ServiceManager.Common;
using Cmf.Custom.TibcoEMS.ServiceManager.DataStructures;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.OutputObjects;
using Cmf.MessageBus.Client;
using Cmf.MessageBus.Messages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using TIBCO.EMS;

namespace Cmf.Custom.TibcoEMS.ServiceManager
{
    public class TibcoEMSServiceManager
    {
        #region Private Variables

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger Logger;

        /// <summary>
        /// Tibco Configs
        /// </summary>
        private readonly NameValueCollection TibcoConfigs;

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
        /// Tibco Session
        /// </summary>
        private Session TibcoSession;

        /// <summary>
        /// TibcoResolveConfigurations
        /// </summary>
        private Dictionary<string, TibcoResolverDto> TibcoResolveConfigurations;

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="TibcoEMSServiceManager"/> class.
        /// </summary>
        public TibcoEMSServiceManager(ILogger logger, NameValueCollection tibcoConfigs)
        {
            // Set Logger
            this.Logger = logger;

            // Set TibcoConfigs
            this.TibcoConfigs = tibcoConfigs;
        }

        /// <summary>
        /// On Start Windows Service
        /// </summary>
        public void OnStart()
        {
            // Create Tibco connection
            this.Logger.LogInformation("Creating Tibco Connection...");

            this.TibcoConnection = TibcoEMSUtilities.CreateTibcoConnection(this.TibcoConfigs);

            // Connect to Tibco
            this.TibcoConnection.Start();

            // Create Tibco session and associate to the connection 
            this.Logger.LogInformation("Creating Tibco Session...");

            this.TibcoSession = this.TibcoConnection.CreateSession(false, SessionMode.AutoAcknowledge);

            // Get Message Bus Transport Configurations
            this.Logger.LogInformation("Getting Message Bus Transport Configurations...");

            this.MessageBusTransportConfiguration = TibcoEMSUtilities.CreateMessageBusTransportConfig();

            // Create Message Bus Transport
            this.Logger.LogInformation("Creating Message Bus Transport...");

            this.MessageBusTransport = new Transport(this.MessageBusTransportConfiguration);

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

            // Close Tibco session
            if (this.TibcoSession != null)
            {
                this.TibcoSession.Close();
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
            if (message != null && !String.IsNullOrWhiteSpace(message.Data) && this.TibcoResolveConfigurations.ContainsKey(subject))
            {
                Task.Run(() =>
                {
                    // Topic name
                    string topicName = this.TibcoResolveConfigurations[subject].Topic;

                    // Action name
                    string actionName = this.TibcoResolveConfigurations[subject].Rule;

                    // QueueMessage
                    bool queueMessage = this.TibcoResolveConfigurations[subject].QueueMessage;

                    // CompressMessage
                    bool compressMessage = this.TibcoResolveConfigurations[subject].CompressMessage;

                    // TextMessage
                    bool textMessage = this.TibcoResolveConfigurations[subject].TextMessage;

                    // Message to send
                    string messageData = message.Data;

                    if (message.Data.IsJson())
                    {
                        // Deserialize MessageBus message received to a Dictionary
                        // - Key: PropertyName
                        // - Value: PropertyValue (MessageToSend)
                        Dictionary<string, string> receivedMessage = JsonConvert.DeserializeObject<Dictionary<string, string>>(message.Data);

                        // Get Message to Send to Tibco from Json message
                        messageData = receivedMessage["Message"];
                    }

                    try
                    {
                        this.Logger.LogInformation(String.Format(TibcoEMSConstants.DefaultLogDataFormat, subject, topicName, String.IsNullOrWhiteSpace(actionName) ? "(null)" : actionName));

                        if (!String.IsNullOrWhiteSpace(actionName))
                        {
                            // Execute DEE
                            ExecuteActionOutput actionOutput = TibcoEMSUtilities.ExecuteDEE(actionName, new Dictionary<string, object>() { { "Data", message.Data } });

                            if (actionOutput.Output != null &&
                                actionOutput.Output.Any() &&
                                actionOutput.Output.ContainsKey("Result") &&
                                !String.IsNullOrWhiteSpace(actionOutput.Output["Result"].ToString()))
                            {
                                messageData = actionOutput.Output["Result"].ToString();
                            }
                        }

                        this.Logger.LogInformation("Sending message to Tibco...");

                        this.Logger.LogInformation($"MessageBus MessageID: {message.Id}");

                        // Send Message to Tibco
                        this.SendMessageToTibco(messageData, topicName, queueMessage, compressMessage, textMessage);

                        this.MessageBusTransport.Reply(message, "Ok");

                        this.Logger.LogInformation("Message sent.");
                    }
                    catch (Exception ex)
                    {
                        this.Logger.LogError(ex, String.Format(TibcoEMSConstants.DefaultLogDataFormat, subject, topicName, String.IsNullOrWhiteSpace(actionName) ? "(null)" : actionName), null);

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
            Task.Run(() =>
            {
                try
                {
                    this.Logger.LogInformation($"Invalidate Cache Message Received >> SUBJECT: {subject}");

                    // Dictionary with updated configurations from Generic Table
                    Dictionary<string, TibcoResolverDto> newTibcoConfigurations = TibcoEMSUtilities.GetTibcoConfigurations();

                    // Dictionary with subjects to unsubscribe
                    var configurationsToUnsubscribe = this.TibcoResolveConfigurations.Where(row => !newTibcoConfigurations.ContainsKey(row.Key));

                    // Dictionary with subjects to subscribe
                    var configurationsToSubscribe = newTibcoConfigurations.Where(row => !this.TibcoResolveConfigurations.ContainsKey(row.Key));

                    if (configurationsToUnsubscribe != null && configurationsToUnsubscribe.Any())
                    {
                        this.Logger.LogInformation("Unsubscribing deprecated subjects...");

                        // Unsubscribe Subjects
                        this.UnsubscribeSubjects(configurationsToUnsubscribe);
                    }

                    if (configurationsToSubscribe != null && configurationsToSubscribe.Any())
                    {
                        this.Logger.LogInformation("Subscribing new subjects...");

                        // Subscribe Subjects
                        this.SubscribeSubjects(configurationsToSubscribe);
                    }

                    // Set global variable with the updated configurations on Generic Table
                    this.TibcoResolveConfigurations = newTibcoConfigurations;

                    this.MessageBusTransport.Reply(message, "Ok");

                    this.Logger.LogInformation("Invalidate Cache Completed.");
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, null, null);

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

        /// <summary>
        /// Subscribe topic or queue and send message to Tibco
        /// </summary>
        private void SendMessageToTibco(string messageData, string topicName, bool queueMessage, bool compressMessage, bool textMessage)
        {
            this.Logger.LogInformation("Checking connection to Tibco...");

            // Check if Tibco is disconnected
            if (this.TibcoConnection != null && this.TibcoConnection.IsDisconnected())
            {
                this.Logger.LogInformation("Tibco is disconnected!");

                // Create Tibco connection
                this.Logger.LogInformation("Creating Tibco Connection...");

                this.TibcoConnection = TibcoEMSUtilities.CreateTibcoConnection(this.TibcoConfigs);

                // Connect to Tibco
                this.TibcoConnection.Start();

                // Create Tibco session and associate to the connection 
                this.Logger.LogInformation("Creating Tibco Session...");
                this.TibcoSession = this.TibcoConnection.CreateSession(false, SessionMode.AutoAcknowledge);
            }

            this.Logger.LogInformation("Tibco is connected...");

            // Tibco Message Producer
            MessageProducer tibcoMessageProducer;

            // Tibco Queue
            Queue tibcoQueue;

            // Tibco Topic
            Topic tibcoTopic;

            if (queueMessage || textMessage)
            {
                this.Logger.LogInformation($"Create Queue with name {topicName} on Tibco Session...");

                // Create queue on Tibco session
                tibcoQueue = this.TibcoSession.CreateQueue(topicName);

                // Create message produce on Tibco session
                tibcoMessageProducer = this.TibcoSession.CreateProducer(tibcoQueue);
            }
            else
            {
                this.Logger.LogInformation($"Create Topic with name {topicName} on Tibco Session...");

                // Create topic on Tibco session
                tibcoTopic = this.TibcoSession.CreateTopic(topicName);

                // Create message produce on Tibco session
                tibcoMessageProducer = this.TibcoSession.CreateProducer(tibcoTopic);
            }

            if (textMessage)
            {
                // Create Tibco Text Message
                TextMessage tibcoTextMessage = this.TibcoSession.CreateTextMessage();
                tibcoTextMessage.SetBooleanProperty("JMS_TIBCO_COMPRESS", compressMessage);
                tibcoTextMessage.SetStringProperty("field", messageData);

                this.Logger.LogInformation("Sending Text Message to Tibco...");

                // Send Message to Tibco
                tibcoMessageProducer.Send(tibcoTextMessage);

                // Log MessageID
                this.Logger.LogInformation($"TextMessageID: {tibcoTextMessage.MessageID}");
            }
            else
            {
                // Create Tibco Map Message
                MapMessage tibcoMapMessage = this.TibcoSession.CreateMapMessage();
                tibcoMapMessage.SetStringProperty("field", messageData);

                this.Logger.LogInformation("Sending Map Message to Tibco...");

                // Send Message to Tibco
                tibcoMessageProducer.Send(tibcoMapMessage);

                // Log MessageID
                this.Logger.LogInformation($"MapMessageID: {tibcoMapMessage.MessageID}");
            }

            // Close Message Producer after send Message to Tibco
            tibcoMessageProducer.Close();
        }

        /// <summary>
        /// Subscribe Subjects on Message Bus
        /// </summary>
        private void SubscribeSubjects(IEnumerable<KeyValuePair<string, TibcoResolverDto>> subjects = null)
        {
            IEnumerable<KeyValuePair<string, TibcoResolverDto>> subjectsToSubscribe = subjects;

            if (subjectsToSubscribe is null)
            {
                subjectsToSubscribe = this.TibcoResolveConfigurations = TibcoEMSUtilities.GetTibcoConfigurations();

                // Subscribe subject associated to Generic Table CustomTibcoEMSGatewayResolver triggered on invalidate cache
                this.MessageBusTransport.Subscribe(TibcoEMSConstants.SubjectCustomTibcoEMSGatewayInvalidateCache, OnInvalidateCache);
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
                this.MessageBusTransport.Unsubscribe(TibcoEMSConstants.SubjectCustomTibcoEMSGatewayInvalidateCache);
            }

            foreach (KeyValuePair<string, TibcoResolverDto> subject in subjectsToUnsubscribe)
            {
                this.MessageBusTransport.Unsubscribe(subject.Key);
            }
        }

        #endregion
    }
}
