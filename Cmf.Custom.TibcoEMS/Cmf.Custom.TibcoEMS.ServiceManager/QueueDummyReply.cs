using Cmf.Custom.TibcoEMS.ServiceManager.Common;
using Microsoft.Extensions.Logging;
using System;
using TIBCO.EMS;

namespace Cmf.Custom.TibcoEMS.ServiceManager
{
    public class QueueDummyReply : IMessageListener
    {
        private readonly ILogger logger;
        private readonly Session session;

        private Destination requestQueue;
        private Destination invalidQueue;

        private MessageConsumer requestConsumer;
        private MessageProducer invalidProducer;

        public QueueDummyReply(ILogger logger, Session session, string requestQueueName, string invalidQueueName = null)
        {
            this.logger = logger;
            this.session = session;

            this.SetupQueueReplier(requestQueueName, invalidQueueName);
        }

        private void SetupQueueReplier(string requestQueueName, string invalidQueueName)
        {
            try
            {
                requestQueue = session.CreateQueue(requestQueueName);
                requestConsumer = session.CreateConsumer(requestQueue);

                if (!String.IsNullOrEmpty(invalidQueueName))
                {
                    invalidQueue = session.CreateQueue(invalidQueueName);
                    invalidProducer = session.CreateProducer(invalidQueue);
                }

                requestConsumer.MessageListener = this;
            }
            catch (Exception e)
            {
                //Handle the exception appropriately
                this.logger.LogError(e.Message);
            }
        }

        public void OnMessage(Message message)
        {
            Destination replyDestination = message.ReplyTo;

            if (replyDestination == null)
            {
                return;
            }

            MessageProducer replyProducer = null;
            System.Threading.Thread.Sleep(4000);

            try
            {
                this.logger.LogInformation("---------------------------------------------------------");

                replyProducer = session.CreateProducer(replyDestination);
                string content = "<?xml version=\"1.1\" encoding=\"UTF-8\" standalone=\"no\"?><response uploadSuccess=\"true\" validationSuccess=\"false\"><samples accepted=\"true\" excluded=\"0\" parameterName=\"NZ-A8-AM-Flattening-MS-Test\" parameterUnit=\"mm\" sampleId=\"215916825\"><keys><key id=\"1\" name=\"Materialsystem\" type=\"ExKey\">InGaN</key><key id=\"2\" name=\"Equipment\" type=\"ExKey\">EQ1</key><key id=\"4\" name=\"Epi_Typ\" type=\"ExKey\">Bestseller</key><key id=\"12\" name=\"X\" type=\"DatKey\">.</key><key id=\"13\" name=\"Y\" type=\"DatKey\">.</key></keys><channelCkcIds><channelCkcId channelId=\"119178\" ckcId=\"1\"/></channelCkcIds><violations><violation id=\"1\" name=\"Raw above specification\"/> </violations></samples></response>";

                if (message is TextMessage)
                {
                    TextMessage requestMessage = (TextMessage)message;

                    this.logger.LogInformation("Received request");
                    this.logger.LogInformation("Time:       " + DateTime.Now + " ms");
                    this.logger.LogInformation("Message ID: " + requestMessage.MessageID);
                    this.logger.LogInformation("Correl. ID: " + requestMessage.CorrelationID);
                    this.logger.LogInformation("Reply to:   " + requestMessage.ReplyTo);
                    this.logger.LogInformation("Contents:   " + requestMessage.Text);

                    TextMessage replyMessage = session.CreateTextMessage();
                    replyMessage.Text = content;
                    replyMessage.CorrelationID = requestMessage.MessageID;

                    replyProducer.Send(replyMessage);

                    this.logger.LogInformation("---------------------------------------------------------");

                    this.logger.LogInformation("Sent reply");
                    this.logger.LogInformation("Time:       " + DateTime.Now + " ms");
                    this.logger.LogInformation("Message ID: " + replyMessage.MessageID);
                    this.logger.LogInformation("Correl. ID: " + replyMessage.CorrelationID);
                    this.logger.LogInformation("Reply to:   " + replyMessage.ReplyTo);
                    this.logger.LogInformation("Contents:   " + replyMessage.Text);
                }
                else if (message is MapMessage)
                {
                    MapMessage requestMessage = (MapMessage)message;

                    this.logger.LogInformation("Received request");
                    this.logger.LogInformation("Time:       " + DateTime.Now + " ms");
                    this.logger.LogInformation("Message ID: " + requestMessage.MessageID);
                    this.logger.LogInformation("Correl. ID: " + requestMessage.CorrelationID);
                    this.logger.LogInformation("Reply to:   " + requestMessage.ReplyTo);
                    this.logger.LogInformation("Contents:   " + requestMessage);

                    MapMessage replyMessage = session.CreateMapMessage();
                    replyMessage.SetString(TibcoEMSConstants.TibcoEMSPropertyMapMessageField, content);
                    replyMessage.CorrelationID = requestMessage.MessageID;

                    replyProducer.Send(replyMessage);

                    this.logger.LogInformation("---------------------------------------------------------");

                    this.logger.LogInformation("Sent reply");
                    this.logger.LogInformation("Time:       " + DateTime.Now + " ms");
                    this.logger.LogInformation("Message ID: " + replyMessage.MessageID);
                    this.logger.LogInformation("Correl. ID: " + replyMessage.CorrelationID);
                    this.logger.LogInformation("Reply to:   " + replyMessage.ReplyTo);
                    this.logger.LogInformation("Contents:   " + replyMessage);
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
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
            }
            finally
            {
                this.logger.LogInformation($"Closing consumer...");

                if (replyProducer != null)
                {
                    replyProducer.Close();
                }

                if (invalidProducer != null)
                {
                    invalidProducer.Close();
                }

                if (requestConsumer != null)
                {
                    requestConsumer.Close();
                }
            }
        }
    }
}