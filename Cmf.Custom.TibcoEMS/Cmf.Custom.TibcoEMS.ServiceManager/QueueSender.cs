using Cmf.Custom.TibcoEMS.ServiceManager.Common;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.OutputObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TIBCO.EMS;

namespace Cmf.Custom.TibcoEMS.ServiceManager
{
    public class QueueSender : IMessageListener
    {
        private readonly ILogger logger;

        private Session session;
        private Destination requestQueue;
        private Destination replyQueue;
        private Destination invalidQueue;

        private MessageProducer requestProducer;
        private MessageConsumer replyConsumer;
        private MessageProducer invalidProducer;

        private Message requestMessage;

        public QueueSender(ILogger logger, Session session, string requestQueueName, string replyQueueName = null, string invalidQueueName = null)
        {
            this.logger = logger;
            this.session = session;

            this.SetupQueueSender(requestQueueName, replyQueueName, invalidQueueName);
        }

        private void SetupQueueSender(string requestQueueName, string replyQueueName, string invalidQueueName)
        {
            try
            {
                requestQueue = this.session.CreateQueue(requestQueueName);
                requestProducer = this.session.CreateProducer(requestQueue);

                if (!String.IsNullOrEmpty(invalidQueueName))
                {
                    invalidQueue = session.CreateQueue(invalidQueueName); ;
                    invalidProducer = session.CreateProducer(invalidQueue);
                }

                if (!String.IsNullOrEmpty(replyQueueName))
                {
                    replyQueue = session.CreateQueue(replyQueueName); ;
                    replyConsumer = session.CreateConsumer(replyQueue);

                    this.logger.LogInformation($"Listening to {replyQueueName}...");
                    replyConsumer.MessageListener = this;
                }
            }
            catch (Exception e)
            {
                //Handle the exception appropriately
                this.logger.LogError(e.Message);
            }
        }

        public void Send(Message message)
        {
            this.logger.LogInformation("---------------------------------------------------------");

            if (replyQueue != null)
            {
                message.ReplyTo = replyQueue;
            }

            this.logger.LogInformation("Sending Message to Tibco...");

            this.logger.LogInformation("Type:       " + message.GetType());
            this.logger.LogInformation("Time:       " + DateTime.Now + " ms");
            this.logger.LogInformation("Message ID: " + message.MessageID);
            this.logger.LogInformation("Correl. ID: " + message.CorrelationID);
            this.logger.LogInformation("Reply to:   " + message.ReplyTo);

            if (message is TextMessage)
            {
                this.logger.LogInformation("Contents:   " + (message as TextMessage).Text);
            }
            else
            {
                this.logger.LogInformation("Contents:   " + message);
            }

            requestProducer.Send(message);
            requestMessage = message;

            this.logger.LogInformation("Sent with Message ID: " + message.MessageID);

            requestProducer.Close();
        }

        public void OnMessage(Message message)
        {
            if (requestMessage == null || message.CorrelationID != requestMessage.MessageID)
            {
                return;
            }

            try
            {
                this.logger.LogInformation("---------------------------------------------------------");
                
                string requestContent = "";
                string replyContent = "";

                if (requestMessage is TextMessage)
                {
                    requestContent = ((TextMessage)requestMessage).Text;
                } 
                else if (requestMessage is MapMessage)
                {
                    requestContent = ((MapMessage)requestMessage).GetString(TibcoEMSConstants.TibcoEMSPropertyMapMessageField);
                }

                if (message is TextMessage)
                {
                    TextMessage replyTextMessage = (TextMessage)message;
                    replyContent = ((TextMessage)replyTextMessage).Text;

                    this.logger.LogInformation("Received message");
                    this.logger.LogInformation("Time:       " + DateTime.Now + " ms");
                    this.logger.LogInformation("Message ID: " + replyTextMessage.MessageID);
                    this.logger.LogInformation("Correl. ID: " + replyTextMessage.CorrelationID);
                    this.logger.LogInformation("Reply to:   " + replyTextMessage.ReplyTo);
                    this.logger.LogInformation("Contents:   " + replyTextMessage.Text);
                }
                else if (message is MapMessage)
                {
                    MapMessage replyMapMessage = (MapMessage)message;
                    replyContent = replyMapMessage.GetString(TibcoEMSConstants.TibcoEMSPropertyMapMessageField);

                    this.logger.LogInformation("Received request");
                    this.logger.LogInformation("Time:       " + DateTime.Now + " ms");
                    this.logger.LogInformation("Message ID: " + replyMapMessage.MessageID);
                    this.logger.LogInformation("Correl. ID: " + replyMapMessage.CorrelationID);
                    this.logger.LogInformation("Reply to:   " + replyMapMessage.ReplyTo);
                    this.logger.LogInformation("Contents:   " + replyMapMessage);
                }
                else
                {
                    this.logger.LogInformation("Invalid message detected");
                    this.logger.LogInformation("Type:       " + message.GetType());
                    this.logger.LogInformation("Time:       " + DateTime.Now + " ms");
                    this.logger.LogInformation("Message ID: " + message.MessageID);
                    this.logger.LogInformation("Correl. ID: " + message.CorrelationID);
                    this.logger.LogInformation("Reply to:   " + message.ReplyTo);

                    if (invalidProducer != null)
                    {
                        message.CorrelationID = message.MessageID;
                        invalidProducer.Send(message);

                        this.logger.LogInformation("---------------------------------------------------------");

                        this.logger.LogInformation("Sent to invalid message queue");
                        this.logger.LogInformation("Type:       " + message.GetType());
                        this.logger.LogInformation("Time:       " + DateTime.Now + " ms");
                        this.logger.LogInformation("Message ID: " + message.MessageID);
                        this.logger.LogInformation("Correl. ID: " + message.CorrelationID);
                        this.logger.LogInformation("Reply to:   " + message.ReplyTo);
                    }
                }

                // Execute DEE
                TibcoEMSUtilities.ExecuteDEE(TibcoEMSConstants.CustomTibcoEMSReplyHandler, new Dictionary<string, object>
                {
                    { "DataRequest", requestContent },
                    { "DataReply", replyContent }
                });
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
            }
            finally
            {
                this.logger.LogInformation($"Closing consumer...");

                replyConsumer.Close();

                if (invalidProducer != null)
                {
                    invalidProducer.Close();
                }
            }
        }
    }
}