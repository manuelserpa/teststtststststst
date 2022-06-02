using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessOrchestration.TableManagement.InputObjects;
using Cmf.Foundation.Common;
using Cmf.MessageBus.Client;
using Cmf.MessageBus.Messages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
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

            this.MessageBus.Exception += OnException;
            this.MessageBus.Connected += OnConnected;
            this.MessageBus.Disconnected += OnDisconnected;

            this.GTTibcoResolver = TibcoGatewayUtilities.GetTibcoGTResolverResults();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Subscribe Cmf Message Bus
        /// </summary>
        public void SubscribeMessageBus()
        {
            try
            {
                // Connect to Message Bus
                this.MessageBus.Start();

                // Block until the client has connected to the Message Bus
                if (!this.ConnectedSignalEvent.WaitOne(this.ConnectTimeout))
                {
                    // Failed to connect in the set interval window
                    throw new Exception("Failed to connect to MessageBus");
                }

                // Connect to Tibco
                this.TibcoConnnector.Start();

                this.MessageBus.Subscribe("CustomReportEDCToSpace", CreateMessage);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Unsubscribe Cmf Message Bus
        /// </summary>
        public void UnsubscribeMessageBus()
        {
            this.TibcoConnnector.Close();

            this.MessageBus.Stop();
        }

        #endregion

        #region Private Methods

        private void OnException(string ex)
        {
            throw new Exception(ex);
        }

        private void OnConnected()
        {
            ConnectedSignalEvent.Set();

            if (Environment.UserInteractive)
            {
                Console.WriteLine("Connected to MessageBus");
            }
        }

        private void OnDisconnected()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine("Disconnected from MessageBus");
            }
        }

        private void CreateMessage(string subject, MbMessage message)
        {
            if (message != null && !String.IsNullOrEmpty(message.Data))
            {
                if (this.GTTibcoResolver != null && this.GTTibcoResolver.Any())
                {
                    if (this.GTTibcoResolver.Keys.Equals(message.Subject))
                    {

                    }
                }
            }
        }

        private void SendMessage(string subject, string message)
        {
            if (!string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(message))
            {
                Session tibcoSession = this.TibcoConnnector.CreateSession(false, SessionMode.AutoAcknowledge);

                Topic tibcoTopic = tibcoSession.CreateTopic(subject);

                MessageProducer tibcoMessageProducer = tibcoSession.CreateProducer(tibcoTopic);

                MapMessage tibcoMessage = tibcoSession.CreateMapMessage();
                tibcoMessage.SetStringProperty("field", message);

                tibcoMessageProducer.Send(tibcoMessage);
            }
        }

        #endregion
    }
}
