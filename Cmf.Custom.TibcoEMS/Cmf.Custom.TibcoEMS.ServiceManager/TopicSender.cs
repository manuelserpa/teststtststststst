using Microsoft.Extensions.Logging;
using System;
using TIBCO.EMS;

namespace Cmf.Custom.TibcoEMS.ServiceManager
{
    public class TopicSender : IMessageListener
    {
        private readonly ILogger logger;

        private Session session;
        private Destination requestTopic;
        private Destination replyTopic;
        private Destination invalidTopic;

        private MessageProducer requestProducer;
        private MessageConsumer replyConsumer;
        private MessageProducer invalidProducer;

        private Message requestMessage;

        public TopicSender(ILogger logger, Session session, string requestTopicName, string replyTopicName = null, string invalidTopicName = null)
        {
            this.logger = logger;
            this.session = session;

            this.SetupTopicSender(requestTopicName, replyTopicName, invalidTopicName);
        }

        private void SetupTopicSender(string requestTopicName, string replyTopicName, string invalidTopicName)
        {
            try
            {
                requestTopic = this.session.CreateTopic(requestTopicName);
                requestProducer = this.session.CreateProducer(requestTopic);

                if (!String.IsNullOrEmpty(invalidTopicName))
                {
                    invalidTopic = session.CreateTopic(invalidTopicName); ;
                    invalidProducer = session.CreateProducer(invalidTopic);
                }

                if (!String.IsNullOrEmpty(replyTopicName))
                {
                    replyTopic = session.CreateTopic(replyTopicName); ;
                    replyConsumer = session.CreateConsumer(replyTopic);

                    this.logger.LogInformation($"Listening to {replyTopicName}...");
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

            if (replyTopic != null)
            {
                message.ReplyTo = replyTopic;
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

                if (message is TextMessage)
                {
                    TextMessage requestTextMessage = (TextMessage)message;

                    this.logger.LogInformation("Received message");
                    this.logger.LogInformation("Time:       " + DateTime.Now + " ms");
                    this.logger.LogInformation("Message ID: " + requestTextMessage.MessageID);
                    this.logger.LogInformation("Correl. ID: " + requestTextMessage.CorrelationID);
                    this.logger.LogInformation("Reply to:   " + requestTextMessage.ReplyTo);
                    this.logger.LogInformation("Contents:   " + requestTextMessage.Text);
                }
                else if (message is MapMessage)
                {
                    MapMessage requestMapMessage = (MapMessage)message;

                    this.logger.LogInformation("Received request");
                    this.logger.LogInformation("Time:       " + DateTime.Now + " ms");
                    this.logger.LogInformation("Message ID: " + requestMapMessage.MessageID);
                    this.logger.LogInformation("Correl. ID: " + requestMapMessage.CorrelationID);
                    this.logger.LogInformation("Reply to:   " + requestMapMessage.ReplyTo);
                    this.logger.LogInformation("Contents:   " + requestMapMessage);
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

                        this.logger.LogInformation("Sent to invalid message Topic");
                        this.logger.LogInformation("Type:       " + message.GetType());
                        this.logger.LogInformation("Time:       " + DateTime.Now + " ms");
                        this.logger.LogInformation("Message ID: " + message.MessageID);
                        this.logger.LogInformation("Correl. ID: " + message.CorrelationID);
                        this.logger.LogInformation("Reply to:   " + message.ReplyTo);
                    }
                }
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