using System;
using System.Configuration;
using TIBCO.EMS;

namespace Cmf.Custom.TibcoEMS.Gateway.Logic
{
    public class Gateway : IDisposable
    {
        #region Private Properties

        private bool IsStarted = false;

        private ConnectionFactory ConnectionFactory = null;

        private Connection Connection = null;

        private Session Session = null;

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Stop();
            }
        }

        /// <summary>
        /// Starts the gateway
        /// </summary>
        public void Start()
        {
            lock (this)
            {
                try
                {
                    if (this.IsStarted)
                    {
                        this.Stop();
                    }

                    string server = ConfigurationManager.AppSettings["TibcoServer"];
                    string username = ConfigurationManager.AppSettings["TibcoUsername"];
                    string password = ConfigurationManager.AppSettings["TibcoPassword"];

                    this.ConnectionFactory = new ConnectionFactory(server);

                    this.Connection = this.ConnectionFactory.CreateConnection(username, password);

                    this.Session = this.Connection.CreateSession(false, SessionMode.AutoAcknowledge);

                    this.Connection.Start();

                    this.IsStarted = true;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Stops the gateway
        /// </summary>
        public void Stop()
        {
            lock (this)
            {
                try
                {
                    if (this.Session != null)
                    {
                        this.Session.Close();
                        this.Session = null;
                    }

                    if (this.Connection != null)
                    {
                        this.Connection.Close();
                        this.Connection = null;
                    }

                    if (this.ConnectionFactory != null)
                    {
                        this.ConnectionFactory = null;
                    }

                    this.IsStarted = false;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
        }

        #endregion
    }
}
