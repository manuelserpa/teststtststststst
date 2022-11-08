using Cmf.Custom.TibcoEMS.ServiceManager.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TIBCO.EMS;

namespace Cmf.Custom.TibcoEMS.ServiceManager
{
    public class RequestReply : IMessageListener
    {
        private readonly ILogger logger;

        private Session session;

        private MessageProducer requestProducer;
        private MessageConsumer replyConsumer;
        private MessageProducer invalidProducer;

        private Destination replyDestination;
        private Dictionary<string, object> replyContext;

        private Message requestMessage;

        public RequestReply(ILogger logger, Session session, Destination requestDestination, Destination replyDestination = null, Dictionary<string, object> replyContext = null, Destination invalidDestination = null)
        {
            this.logger = logger;
            this.session = session;
            this.replyContext = replyContext ?? new Dictionary<string, object>();
            this.replyDestination = replyDestination;

            SetupQueueSender(requestDestination, replyDestination, invalidDestination);
        }

        private void SetupQueueSender(Destination requestDestination, Destination replyDestination = null, Destination invalidDestination = null)
        {
            try
            {
                requestProducer = session.CreateProducer(requestDestination);

                if (invalidDestination != null)
                {
                    invalidProducer = session.CreateProducer(invalidDestination);
                }

                if (replyDestination != null)
                {
                    replyConsumer = session.CreateConsumer(replyDestination);

                    logger.LogInformation($"Listening to {replyDestination}...");

                    replyConsumer.MessageListener = this;
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        public void Send(Message message)
        {
            logger.LogInformation("---------------------------------------------------------");

            if (replyDestination != null)
            {
                message.ReplyTo = replyDestination;
            }

            logger.LogInformation("Sending Message to Tibco...");

            logger.LogInformation("Type:       " + message.GetType());
            logger.LogInformation("Time:       " + DateTime.Now + " ms");
            logger.LogInformation("Message ID: " + message.MessageID);
            logger.LogInformation("Correl. ID: " + message.CorrelationID);
            logger.LogInformation("Reply to:   " + message.ReplyTo);

            if (message is TextMessage)
            {
                logger.LogDebug("Contents:   " + (message as TextMessage).Text);
            }
            else
            {
                logger.LogDebug("Contents:   " + message);
            }

            requestProducer.Send(message);
            requestMessage = message;

            logger.LogInformation("Sent with Message ID: " + message.MessageID);

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
                logger.LogInformation("---------------------------------------------------------");
                
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

                    logger.LogInformation("Received message");
                    logger.LogInformation("Time:       " + DateTime.Now + " ms");
                    logger.LogInformation("Message ID: " + replyTextMessage.MessageID);
                    logger.LogInformation("Correl. ID: " + replyTextMessage.CorrelationID);
                    logger.LogInformation("Reply to:   " + replyTextMessage.ReplyTo);
                    logger.LogDebug("Contents:   " + replyTextMessage.Text);
                }
                else if (message is MapMessage)
                {
                    MapMessage replyMapMessage = (MapMessage)message;
                    replyContent = replyMapMessage.GetString(TibcoEMSConstants.TibcoEMSPropertyMapMessageField);

                    logger.LogInformation("Received request");
                    logger.LogInformation("Time:       " + DateTime.Now + " ms");
                    logger.LogInformation("Message ID: " + replyMapMessage.MessageID);
                    logger.LogInformation("Correl. ID: " + replyMapMessage.CorrelationID);
                    logger.LogInformation("Reply to:   " + replyMapMessage.ReplyTo);
                    logger.LogDebug("Contents:   " + replyMapMessage);
                }
                else
                {
                    logger.LogInformation("Invalid message detected");
                    logger.LogInformation("Type:       " + message.GetType());
                    logger.LogInformation("Time:       " + DateTime.Now + " ms");
                    logger.LogInformation("Message ID: " + message.MessageID);
                    logger.LogInformation("Correl. ID: " + message.CorrelationID);
                    logger.LogInformation("Reply to:   " + message.ReplyTo);

                    if (invalidProducer != null)
                    {
                        message.CorrelationID = message.MessageID;
                        invalidProducer.Send(message);

                        logger.LogInformation("---------------------------------------------------------");

                        logger.LogInformation("Sent to invalid message queue");
                        logger.LogInformation("Type:       " + message.GetType());
                        logger.LogInformation("Time:       " + DateTime.Now + " ms");
                        logger.LogInformation("Message ID: " + message.MessageID);
                        logger.LogInformation("Correl. ID: " + message.CorrelationID);
                        logger.LogInformation("Reply to:   " + message.ReplyTo);
                    }
                }

                // Execute DEE
                TibcoEMSUtilities.ExecuteDEE(TibcoEMSConstants.CustomTibcoEMSReplyHandler, new Dictionary<string, object>
                {
                    { "Context", replyContext },
                    { "ReplyMessage", replyContent }
                });
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
            finally
            {
                logger.LogInformation($"Closing consumer...");

                replyConsumer.Close();

                if (invalidProducer != null)
                {
                    invalidProducer.Close();
                }
            }
        }
    }
}