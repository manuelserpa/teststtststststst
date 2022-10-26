using Cmf.Custom.TibcoEMS.ServiceManager.Common;
using Cmf.Custom.TibcoEMS.ServiceManager.DataStructures;
using Cmf.Custom.TibcoEMS.ServiceManager.Mock;
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

        #endregion Private Variables

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
            TibcoEMSUtilities.InitialConfigurations();

            this.CreateTibcoConnection();

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

        #endregion Public Methods

        #region Private Methods

        private void CreateTibcoConnection()
        {
            // Create Tibco connection
            this.Logger.LogInformation("Creating Tibco Connection...");

            this.TibcoConnection = TibcoEMSUtilities.CreateTibcoConnection(this.TibcoConfigs);

            // Connect to Tibco
            this.Logger.LogInformation("StartingTibco Connection...");

            this.TibcoConnection.Start();

            // Create Tibco session and associate to the connection
            this.Logger.LogInformation("Creating Tibco Session...");

            this.TibcoSession = this.TibcoConnection.CreateSession(false, SessionMode.AutoAcknowledge);
        }

        /// <summary>
        /// This method is triggered when the subject contains a configuration on the Generic Table CustomTibcoEMSGatewayResolver
        /// </summary>
        private void OnMessageReceived(string subject, MbMessage message)
        {
            if (message != null && !string.IsNullOrWhiteSpace(message.Data) && this.TibcoResolveConfigurations.ContainsKey(subject))
            {
                Task.Run(() =>
                {
                    // Topic/Queue name
                    string topicName = this.TibcoResolveConfigurations[subject].Topic;

                    // Topic name
                    string replyTo = this.TibcoResolveConfigurations[subject].ReplyTo;

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
                        this.SendMessageToTibco(messageData, topicName, replyTo, isQueue, isToCompress, isTextMessage);

                        this.Logger.LogInformation("Replying to MessageBus...");
                        this.MessageBusTransport.Reply(message, "Ok");
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
        private void SendMessageToTibco(string message, string requestDestinationName, string replyDestinationName, bool isQueueMessage, bool isToCompressMessage, bool isTextMessage)
        {
            this.Logger.LogInformation("Checking connection to Tibco...");

            // Check if Tibco is disconnected
            if (this.TibcoConnection != null && this.TibcoConnection.IsDisconnected())
            {
                this.Logger.LogInformation("Tibco is disconnected!");

                this.CreateTibcoConnection();
            }

            this.Logger.LogInformation("Tibco is connected...");

            Dictionary<string, string> headersData = null;
            string messageData = null;

            this.Logger.LogInformation("Parsing message...");
            Dictionary<string, object> context = new Dictionary<string, object>();

            if (message.IsJson())
            {
                // Deserialize MessageBus message received to a Dictionary
                // - Key: PropertyName
                // - Value: PropertyValue (MessageToSend)
                Dictionary<string, object> receivedMessage = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);

                if (receivedMessage.ContainsKey("Header"))
                {
                    // Get Headers to Send to Tibco from Json message
                    headersData = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(receivedMessage["Header"]));
                }

                if (receivedMessage.ContainsKey("Context"))
                {
                    // Get Headers to Send to Tibco from Json message
                    context = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(receivedMessage["Context"]));
                }

                // Get Message to Send to Tibco from Json message
                messageData = receivedMessage["Message"] as string;
            }

            Message messageToSend;

            if (isTextMessage)
            {
                messageToSend = TibcoEMSUtilities.CreateTextMessage(this.TibcoSession, messageData, headersData, isToCompressMessage);
            }
            else
            {
                messageToSend = TibcoEMSUtilities.CreateMapMessage(this.TibcoSession, messageData, headersData);
            }

            if (isQueueMessage)
            {
                this.Logger.LogInformation($"Create Queue with name {requestDestinationName} on Tibco Session...");

                // TODO: This line should be removed or commented
                new QueueSpaceReply(this.Logger, this.TibcoSession, requestDestinationName);

                Queue requestQueue = this.TibcoSession.CreateQueue(requestDestinationName);
                Queue replyQueue = this.TibcoSession.CreateQueue(replyDestinationName);

                RequestReply requestor = new RequestReply(this.Logger, this.TibcoSession, requestQueue, replyQueue, context);
                requestor.Send(messageToSend);
            }
            else
            {
                this.Logger.LogInformation($"Create Topic with name {requestDestinationName} on Tibco Session...");

                Topic requestTopic = this.TibcoSession.CreateTopic(requestDestinationName);
                Topic replyTopic = this.TibcoSession.CreateTopic(replyDestinationName);

                RequestReply requestor = new RequestReply(this.Logger, this.TibcoSession, requestTopic, replyTopic, context);
                requestor.Send(messageToSend);
            }
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

        #endregion Private Methods
    }
}