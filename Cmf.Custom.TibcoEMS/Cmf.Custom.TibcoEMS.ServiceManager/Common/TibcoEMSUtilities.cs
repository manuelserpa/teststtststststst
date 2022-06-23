using Cmf.Custom.TibcoEMS.ServiceManager.DataStructures;
using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessOrchestration.ConfigurationManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.OutputObjects;
using Cmf.Foundation.BusinessOrchestration.TableManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.Utilities.InputObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;
using Cmf.MessageBus.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TIBCO.EMS;

namespace Cmf.Custom.TibcoEMS.ServiceManager.Common
{
    public static class TibcoEMSUtilities
    {
        #region Private Variables

        /// <summary>
        /// The string a
        /// </summary>
        private static readonly string stringA = "EV3x0mnju+ceu2Lt528h3Tk{Wl@3@F";

        /// <summary>
        /// The string b
        /// </summary>
        private static readonly string stringB = "153A926C2CFCF4BAFD39F21256789";

        /// <summary>
        /// The string c
        /// </summary>
        private static readonly string stringC = "ZJ9984nM19O3A22K";

        #endregion

        #region Public Methods

        /// <summary>
        /// Create Message Bus configuration
        /// </summary>
        public static TransportConfig CreateMessageBusTransportConfig()
        {
            string host = ConfigurationManager.AppSettings["MessageBus.Host"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["MessageBus.Port"]);
            string securityToken = ConfigurationManager.AppSettings["MessageBus.SecurityToken"];

            string tenantName = ConfigurationManager.AppSettings["ClientTenantName"];
            string applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            string externalAddress = ConfigurationManager.AppSettings["MessageBus.ExternalAddress"];

            bool useLoadBalancing = bool.TryParse(ConfigurationManager.AppSettings["MessageBus.UseLoadBalancing"], out useLoadBalancing) ? useLoadBalancing : false;

            TransportConfig messageBusConfiguration = new TransportConfig()
            {
                GatewaysConfig = new List<GatewayConfig>
                {
                    new GatewayConfig()
                    {
                        Address = host,
                        Port = port,
                        ExternalAddress = externalAddress,
                    }
                },
                UseLoadBalancing = false,
                ApplicationName = applicationName,
                TenantName = tenantName,
                SecurityToken = securityToken,
                UseGatewayExternalAddress = externalAddress.Length > 0
            };

            if (useLoadBalancing)
            {
                string lbHost = ConfigurationManager.AppSettings["MessageBus.LoadBalancing.Host"];
                int lbPort = Convert.ToInt32(ConfigurationManager.AppSettings["MessageBus.LoadBalancing.Port"]);
                string lbSecurityToken = ConfigurationManager.AppSettings["MessageBus.LoadBalancing.SecurityToken"];

                bool lbEnableSsl = bool.TryParse(ConfigurationManager.AppSettings["MessageBus.LoadBalancing.UseSSL"], out lbEnableSsl) ? lbEnableSsl : false;

                messageBusConfiguration.UseLoadBalancing = true;
                messageBusConfiguration.LoadBalancerConfig = new LoadBalancerConfig
                {
                    Address = lbHost,
                    Port = lbPort,
                    EnableSsl = lbEnableSsl,
                    SecurityToken = lbSecurityToken
                };
            }

            return messageBusConfiguration;
        }

        /// <summary>
        /// Create Tibco connection
        /// </summary>
        public static Connection CreateTibcoConnection(NameValueCollection configurations)
        {
            string host = configurations["Host"];
            string username = configurations["Username"];
            string password = Decrypt(configurations["Password"], stringA, stringB, 3, stringC, 256);

            ConnectionFactory factory = new ConnectionFactory(host);

            Connection connection = factory.CreateConnection(username, password);

            return connection;
        }

        /// <summary>
        /// Execute DEE Action
        /// </summary>
        public static ExecuteActionOutput ExecuteDEE(string actionName, Dictionary<string, object> input)
        {
            ExecuteActionOutput output = new ExecuteActionInput
            {
                Action = new Foundation.Common.DynamicExecutionEngine.Action { Name = actionName },
                Input = input
            }.ExecuteActionSync();

            return output;
        }

        /// <summary>
        /// Get data "IsEnabled" from Generic Table related to topics to be subscribed by Tibco 
        /// </summary>
        public static Dictionary<string, TibcoResolverDto> GetTibcoConfigurations()
        {
            Dictionary<string, TibcoResolverDto> output = null;

            // Filter Generic Table by "IsEnabled" field
            FilterCollection filters = new FilterCollection()
            {
                new Filter()
                {
                    Name = "IsEnabled",
                    Operator = FieldOperator.IsEqualTo,
                    Value = true,
                    LogicalOperator = LogicalOperator.Nothing
                }
            };

            // Execute service to get Generic Table results
            GenericTable genericTable = new GetGenericTableByNameWithFilterInput()
            {
                Name = TibcoEMSConstants.GTCustomTibcoEMSGatewayResolver,
                Filters = filters
            }.GetGenericTableByNameWithFilterSync().GenericTable;

            DataSet ds = new ToDataSetInput()
            {
                NgpDataSet = genericTable.Data
            }.ToDataSetSync().DataSet;

            // Transform DataSet table to a Dictionary
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];

                output = dt.AsEnumerable().ToDictionary(row => row.Field<string>("Subject"),
                                                        row => new TibcoResolverDto
                                                        {
                                                            Subject = row.Field<string>("Subject"),
                                                            Topic = row.Field<string>("Topic"),
                                                            Rule = row.Table.Columns.Contains("Rule") ? row.Field<string>("Rule") : String.Empty
                                                        });
            }

            return output;
        }

        /// <summary>
        /// Get Custom Configs from MES
        /// </summary>
        public static NameValueCollection GetChildConfigsByPath(string path)
        {
            NameValueCollection output = new NameValueCollection();

            ConfigCollection configs = new GetChildConfigsByPathInput()
            {
                Path = path
            }.GetChildConfigsByPathSync().Configs;

            if (configs != null && configs.Any())
            {
                foreach (Config config in configs)
                {
                    string value = config.Value != null ? config.Value.ToString() : string.Empty;

                    output.Add(config.Name, value);
                }
            }

            return output;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Decrypts specified ciphertext using Rijndael symmetric key algorithm.
        /// </summary>
        /// <param name="cipherText">
        /// Base64-formatted ciphertext value.
        /// </param>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived. The
        /// derived password will be used to generate the encryption key.
        /// Passphrase can be any string.
        /// </param>
        /// <param name="saltValue">
        /// Salt value used along with passphrase to generate password. Salt can
        /// be any string.
        /// </param>
        /// <param name="passwordIterations">
        /// Number of iterations used to generate password.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (or IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long.
        /// </param>
        /// <param name="keySize">
        /// Size of encryption key in bits. Allowed values are: 128, 192, and 256.
        /// Longer keys are more secure than shorter keys.
        /// </param>
        /// <returns>
        /// Decrypted string value.
        /// </returns>
        /// <remarks>
        /// Most of the logic in this function is similar to the Encrypt
        /// logic. In order for decryption to work, all parameters of this function
        /// - except cipherText value - must match the corresponding parameters of
        /// the Encrypt function which was called to generate the
        /// ciphertext.
        /// </remarks>
        private static string Decrypt(string cipherText, string passPhrase, string saltValue, int passwordIterations, string initVector, int keySize)
        {
            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            Byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            Byte[] saltValueBytes = Encoding.UTF8.GetBytes(saltValue);

            // Convert our ciphertext into a byte array.
            Byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            // First, we must create a password, from which the key will be
            // derived. This password will be generated from the specified
            // passphrase and salt value. Password creation can be done in
            // several iterations.
            Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltValueBytes, passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            Byte[] keyBytes = password.GetBytes(keySize / 8);

            // Create uninitialized AES encryption object.
            Aes symmetricKey = Aes.Create();

            // Generate decryptor from the existing key bytes and initialization
            // vector. Key size will be defined based on the number of the key
            // bytes.
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // Define cryptographic stream (always use Read mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            // Since at this point we don't know what the size of decrypted data
            // plaintext is never longer than ciphertext.
            Byte[] plainTextBytes = new Byte[cipherTextBytes.Length];

            // Start decrypting.
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            byte[] finalBytes = decryptor.TransformFinalBlock(plainTextBytes, 0, 0);

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert decrypted data into a string.
            // Let us assume that the original plaintext string was UTF8-encoded.
            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            string final = Encoding.UTF8.GetString(finalBytes, 0, finalBytes.Length);

            var result = plainText + final;

            // Return decrypted string.
            return result;
        }

        #endregion
    }
}
