//--------------------------------------------------------------------------------
//<FileInfo>
//  <copyright file="BaseContext.cs" company="Critical Manufacturing, SA">
//        <![CDATA[Copyright © Critical Manufacturing SA. All rights reserved.]]>
//  </copyright>
//  <Author>João Brandão</Author>
//</FileInfo>
//--------------------------------------------------------------------------------

#region Using Directives

using Cmf.Foundation.BusinessOrchestration.ApplicationSettingManagement.InputObjects;
using Cmf.LightBusinessObjects.Infrastructure;
using Cmf.LoadBalancing;
using Cmf.MessageBus.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

#endregion Using Directives

namespace Settings
{
    /// <summary>
    /// Represents the test base class
    /// </summary>
    public class BaseContext
    {
        #region Private Variables

        /// <summary>
        /// The configuration
        /// </summary>
        private static ClientConfiguration config = null;

        /// <summary>
        /// Custom Transport configuration
        /// </summary>
        private static TransportConfig messageBusTransportConfig = null;

        /// <summary>
        /// Custom Test context
        /// </summary>
        private static TestContext testContext = null;

        #endregion

        #region Public Variables
        #endregion

        #region Properties

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public static string Password
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public static string UserName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the user role.
        /// </summary>
        /// <value>
        /// The user role.
        /// </value>
        public static string UserRole
        {
            get;
            private set;
        }

        /// <summary>
        /// IoT Tests Mode
        /// </summary>
        /// <value>
        /// Enum with modes
        /// </value>
        public static IoTModes Mode
        {
            get;
            private set;
        } = IoTModes.Local;

        /// <summary>
        /// Absolute Path to RunSettings File
        /// </summary>
        /// <value>
        /// Absolute Path to RunSettings File
        /// </value>
        public static string FilePath
        {
            get;
            private set;
        }

        #endregion

        #region Constructors
        #endregion

        #region Public Methods

        /// <summary>
        /// Ends this instance.
        /// </summary>
        public static void BaseEnd()
        {
            // Assembly clean up
        }

        /// <summary>
        /// Initializes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void BaseInit(TestContext context)
        {
            // Set custom test context
            LoadEnv(ref context);
            BaseContext.testContext = context;

            ClientConfigurationProvider.ConfigurationFactory = () =>
            {
                if (config == null)
                {
                    config = new ClientConfiguration()
                    {
                        HostAddress = System.IO.Directory.Exists(GetString(context, "hostAdress")) ? GetString(context, "hostAdress") : string.Format("{0}:{1}", GetString(context, "hostAdress"), int.Parse(GetString(context, "hostPort"))),
                        ClientTenantName = GetString(context, "clientTenantName"),
                        UseSsl = context.Properties.Contains("hostUseSSL") ? bool.Parse(GetString(context, "hostUseSSL")) : false,
                        ApplicationName = GetString(context, "applicationName"),
                        IsUsingLoadBalancer = context.Properties.Contains("useLoadBalancer") ? bool.Parse(GetString(context, "useLoadBalancer")) : false,
                        ThingsToDoAfterInitialize = null,
                        RequestTimeout = GetString(context, "requestTimeout")
                    };

                    if (context.Properties.Contains("userName"))
                    {
                        config.UserName = GetString(context, "userName");
                    }

                    if (context.Properties.Contains("password"))
                    {
                        config.Password = GetString(context, "password");
                    }
                    
                    config.SecurityPortalBaseAddress = new Uri(GetString(context, "securityPortalBaseAddress"));
                    config.SecurityAccessToken = GetString(context, "securityPortalAccessToken");
                }
                return config;
            };

            UserRole = context.Properties.Contains("userRole") ? GetString(context, "userRole") : "Almost Admin";

            // Handle Culture
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(GetString(context, "culture"));

            #region Set DB Connections
            //if (context.Properties.Contains("OnlineConnection"))
            //{
            //    Utilities.OnlineConnection = new SqlConnection(GetString(context, "OnlineConnection"));
            //}
            //if (context.Properties.Contains("ODSConnection"))
            //{
            //    Utilities.ODSConnection = new SqlConnection(GetString(context, "ODSConnection"));
            //}
            //if (context.Properties.Contains("DWHConnection"))
            //{
            //    Utilities.DWHConnection = new SqlConnection(GetString(context, "DWHConnection"));
            //}
            #endregion

            #region Connect IoT

            if (context.Properties.Contains("mode"))
            {
                IoTModes ioTMode;
                Enum.TryParse(GetString(context, "mode"), out ioTMode);

                Mode = ioTMode;
            }

            if (GetString(context, "TestRunDirectory").Contains("TestExecution"))
            {
                FilePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), GetString(context, "filePathRemote")));
            } else
            {
                FilePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), GetString(context, "filePathLocal")));
            }

            #endregion Connect IoT
        }

        /// <summary>
        /// Retries a given function after a period of time to avoid race condition
        /// </summary>
        public static void RetryRun(Action retryRun, int retryCount = 3, TimeSpan? span = null)
        {
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    retryRun();
                    break;
                }
                catch
                {
                    if (span == null)
                    {
                        Random random = new Random();
                        span = new TimeSpan(0, 0, random.Next(5, 15));
                    }

                    if (i + 1 == retryCount)
                    {
                        throw;
                    }
                    else
                    {
                        Thread.Sleep(span.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Custom: Get Message Bus Transport Configuration
        /// </summary>
        /// <returns></returns>
        public static TransportConfig GetMessageBusTransportConfiguration()
        {
            if (messageBusTransportConfig == null)
            {
                string transportConfigString = new GetApplicationBootInformationInput().GetApplicationBootInformationSync().TransportConfig;
            
                TransportConfig transportConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<TransportConfig>(transportConfigString);
                transportConfig.ApplicationName = GetString(testContext, "applicationName");
                transportConfig.TenantName = GetString(testContext, "clientTenantName");

                messageBusTransportConfig = transportConfig;
            }

            return messageBusTransportConfig;
        }

        public enum IoTModes
        {
            Local,
            RemoteDownload,
            RemoteService
        }

        public static void LoadEnv(ref TestContext context)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            string fileName = ".env";

            while (!File.Exists(Path.Combine(dirInfo.FullName, fileName)) && dirInfo.Parent != null)
            {
                dirInfo = dirInfo.Parent;
            }

            string filePath = Path.Combine(dirInfo.FullName, fileName);

            if (!File.Exists(filePath))
            {
                return;
            }

            Dictionary<string, string> env = new Dictionary<string, string>();

            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] parts = line.Split(
                    '=',
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    continue;

                env.Add(parts[0], parts[1]);
            }

            if (env.ContainsKey("USERNAME"))
            {
                context.Properties["userName"] = env.GetValueOrDefault("USERNAME");
            }

            if (env.ContainsKey("PASSWORD") && context.Properties.Contains("password"))
            {
                context.Properties["password"] = env.GetValueOrDefault("PASSWORD");
            }
        }

        #endregion

        #region Private & Internal Methods

        /// <summary>
        /// Gets a string from the TestContext properties
        /// </summary>
        /// <param name="context">Test Context</param>
        /// <param name="property">Property to find</param>
        /// <returns>A string</returns>
        private static string GetString(TestContext context, string property)
        {
            if (context.Properties[property] == null)
            {
                throw new ArgumentException($"Property does not exist, does not have a value, or a test setting is not selected.", property);
            }
            else
            {
                return context.Properties[property].ToString();
            }
        }

        #endregion

        #region Event handling Methods
        #endregion
    }
}
