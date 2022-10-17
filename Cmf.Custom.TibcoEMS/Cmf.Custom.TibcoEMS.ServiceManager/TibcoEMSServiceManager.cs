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

            TibcoEMSUtilities.InitialConfigurations();

            this.TibcoConnection = TibcoEMSUtilities.CreateTibcoConnection(this.TibcoConfigs);

            // Connect to Tibco
            this.TibcoConnection.Start();

            // Create Tibco session and associate to the connection 
            this.Logger.LogInformation("Creating Tibco Session...");

            this.TibcoSession = this.TibcoConnection.CreateSession(false, SessionMode.AutoAcknowledge);

            // Get Message Bus Transport Configurations
            this.Logger.LogInformation("Getting Message Bus Transport Configurations...");

            // Create Message Bus Transport
            this.Logger.LogInformation("Creating Message Bus Transport...");

            this.MessageBusTransport = new Transport(TibcoEMSUtilities.messageBusTransportConfig);

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
            if (message != null && !string.IsNullOrWhiteSpace(message.Data) && this.TibcoResolveConfigurations.ContainsKey(subject))
            {
                Task.Run(() =>
                {
                    // Topic name
                    string topicName = this.TibcoResolveConfigurations[subject].Topic;

                    // Action name
                    string actionName = this.TibcoResolveConfigurations[subject].Rule;

                    // IsQueue
                    bool isQueue = this.TibcoResolveConfigurations[subject].IsQueue;

                    // IsToCompress
                    bool isToCompress = this.TibcoResolveConfigurations[subject].IsToCompress;

                    // IsTextMessage
                    bool isTextMessage = this.TibcoResolveConfigurations[subject].IsTextMessage;

                    // Message to send
                    string messageData = message.Data;

                    // Headers to send
                    Dictionary<string, string> headersData = null;

                    if (message.Data.IsJson())
                    {
                        // Deserialize MessageBus message received to a Dictionary
                        // - Key: PropertyName
                        // - Value: PropertyValue (MessageToSend)
                        Dictionary<string, object> receivedMessage = JsonConvert.DeserializeObject<Dictionary<string, object>>(message.Data);

                        if (receivedMessage.ContainsKey("Header"))
                        {
                            // Get Headers to Send to Tibco from Json message
                            string jsonHeaders = JsonConvert.SerializeObject(receivedMessage["Header"]);
                            headersData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonHeaders);
                        }

                        // Get Message to Send to Tibco from Json message
                        messageData = receivedMessage["Message"] as string;
                    }

                    try
                    {
                        this.Logger.LogInformation(string.Format(TibcoEMSConstants.DefaultLogDataFormat, subject, topicName, string.IsNullOrWhiteSpace(actionName) ? "(null)" : actionName));

                        if (!string.IsNullOrWhiteSpace(actionName))
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
                        this.SendMessageToTibco(headersData, messageData, topicName, isQueue, isToCompress, isTextMessage);

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
        private void SendMessageToTibco(Dictionary<string, string> headersData, string messageData, string topicName, bool isQueueMessage, bool isToCompressMessage, bool isTextMessage)
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

            if (isQueueMessage)
            {
                this.Logger.LogInformation($"Create Queue with name {topicName} on Tibco Session...");

                // Create queue on Tibco session
                Queue tibcoQueue = this.TibcoSession.CreateQueue(topicName);

                // Create message produce on Tibco session
                tibcoMessageProducer = this.TibcoSession.CreateProducer(tibcoQueue);
            }
            else
            {
                this.Logger.LogInformation($"Create Topic with name {topicName} on Tibco Session...");

                // Create topic on Tibco session
                Topic tibcoTopic = this.TibcoSession.CreateTopic(topicName);

                // Create message produce on Tibco session
                tibcoMessageProducer = this.TibcoSession.CreateProducer(tibcoTopic);
            }

            if (isTextMessage)
            {
                // Create Tibco Text Message with associated message
                TextMessage tibcoTextMessage = this.TibcoSession.CreateTextMessage(messageData);

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
                tibcoMapMessage.SetString(TibcoEMSConstants.TibcoEMSPropertyMapMessageField, messageData);

                // Set Headers on MapMessage
                if (headersData != null && headersData.Any())
                {
                    foreach (KeyValuePair<string, string> header in headersData)
                    {
                        tibcoMapMessage.SetStringProperty(header.Key, header.Value);
                    }
                }

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
