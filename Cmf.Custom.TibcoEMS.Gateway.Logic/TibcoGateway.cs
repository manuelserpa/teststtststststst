using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessOrchestration.TableManagement.InputObjects;
using Cmf.Foundation.Common;
using Cmf.MessageBus.Client;
using System;
using System.Threading;

namespace Cmf.Custom.TibcoEMS.Gateway.Logic
{
    public class TibcoGateway
    {
        #region Private Properties

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
        /// ConnectTimeout
        /// </summary>
        private int ConnectTimeout = 10000;
        
        /// <summary>
        /// Request Timeout
        /// </summary>
        private int RequestTimeout = 10000;

        #endregion

        #region Constructor

        public TibcoGateway()
        {
            this.MessageBusConfiguration = TibcoGatewayUtilities.CreateMessageBusConfiguration();
            this.MessageBus = new Transport(this.MessageBusConfiguration);

            this.MessageBus.Exception += OnException;
            this.MessageBus.Connected += OnConnected;
            this.MessageBus.Disconnected += OnDisconnected;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Subscribe Cmf Message Bus
        /// </summary>
        public void SubscribeMessageBus()
        {
            // Filter Generic Table by "IsEnabled" field
            FilterCollection filters = new FilterCollection()
            {
                new Filter()
                {
                    Name ="IsEnabled",
                    Operator = FieldOperator.IsEqualTo,
                    Value = true,
                    LogicalOperator = LogicalOperator.Nothing
                }
            };

            // Execute service to get Generic Table results
            GenericTable genericTable = new GetGenericTableByNameWithFilterInput()
            {
                Name = TibcoGatewayConstants.GTCustomTibcoEMSGatewayResolver,
                Filters = filters
            }.GetGenericTableByNameWithFilterSync().GenericTable;

            if (genericTable != null && genericTable.HasData)
            {
                // Connect to Message Bus
                this.MessageBus.Start();

                // Block until the client has connected to the Message Bus
                if (!this.ConnectedSignalEvent.WaitOne(this.ConnectTimeout))
                {
                    // Failed to connect in the set interval window
                    throw new Exception("Failed to connect to MessageBus");
                }

                //foreach (var item in genericTable.Data)
                //{
                //    // Subscribe to event
                //    this.MessageBus.Subscribe(subject, onEvent);
                //}
            }
        }

        /// <summary>
        /// Unsubscribe Cmf Message Bus
        /// </summary>
        public void UnsubscribeMessageBus()
        {
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

        #endregion
    }
}
