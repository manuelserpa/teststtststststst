using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessOrchestration.TableManagement.InputObjects;
using Cmf.Foundation.Common;
using Cmf.MessageBus.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using TIBCO.EMS;

namespace Cmf.Custom.TibcoEMS.Gateway.Logic
{
    public static class TibcoGatewayUtilities
    {
        /// <summary>
        /// Create Message Bus configuration
        /// </summary>
        public static TransportConfig CreateMessageBusConfiguration()
        {
            string host = ConfigurationManager.AppSettings["MessageBus.Host"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["MessageBus.Port"]);
            string securityToken = ConfigurationManager.AppSettings["MessageBus.SecurityToken"];

            string tenantName = ConfigurationManager.AppSettings["ClientTenantName"];
            string applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            string userName = ConfigurationManager.AppSettings["UserName"];
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
        /// Create Tibco configuration
        /// </summary>
        public static Connection CreateTibcoConfiguration()
        {
            string host = ConfigurationManager.AppSettings["Tibco.Host"];
            string username = ConfigurationManager.AppSettings["Tibco.Username"];
            string password = ConfigurationManager.AppSettings["Tibco.Password"];

            ConnectionFactory factory = new ConnectionFactory(host);

            Connection connection = factory.CreateConnection(username, password);

            return connection;
        }

        /// <summary>
        /// Get Generic Table Data related to topics to be subscribed by Tibco 
        /// </summary>
        public static Dictionary<string, KeyValuePair<string, string>> GetTibcoGTResolverResults()
        {
            Dictionary<string, KeyValuePair<string, string>> output = null;

            // Filter Generic Table by "IsEnabled" field
            FilterCollection filters = new FilterCollection()
            {
                new Filter()
                {
                    Name = "IsEnabled",
                    Operator = FieldOperator.IsEqualTo,
                    Value = true,
                    LogicalOperator = LogicalOperator.AND
                },
                new Filter()
                {
                    Name = "Subject",
                    Operator = FieldOperator.IsNotNull,
                    LogicalOperator = LogicalOperator.Nothing
                }
            };

            // Execute service to get Generic Table results
            GenericTable genericTable = new GetGenericTableByNameWithFilterInput()
            {
                Name = TibcoGatewayConstants.GTCustomTibcoEMSGatewayResolver,
                Filters = filters
            }.GetGenericTableByNameWithFilterSync().GenericTable;

            DataSet ds = ToDataSet(genericTable.Data);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                output = ds.Tables[0].AsEnumerable().ToDictionary(row => row.Field<string>("Subject"), row => new KeyValuePair<string, string>(row.Field<string>("Topic"), row.Field<string>("Rule")));
            }

            return output;
        }

        /// <summary>
        /// Convert a NgpDataSet to a DataSet
        /// </summary>
        /// <param name="dsd">NgpDataSet to convert</param>
        /// <returns>Returns a DataSet with all information of the NgpDataSet</returns>
        public static DataSet ToDataSet(NgpDataSet dsd)
        {
            DataSet ds = new DataSet();

            if (dsd == null || (string.IsNullOrWhiteSpace(dsd.XMLSchema) && string.IsNullOrWhiteSpace(dsd.DataXML)))
            {
                dsd = FromDataSet(ds);
            }

            //Insert schema
            TextReader a = new StringReader(dsd.XMLSchema);
            XmlReader readerS = new XmlTextReader(a);
            ds.ReadXmlSchema(readerS);
            XDocument xdS = XDocument.Parse(dsd.XMLSchema);

            //Insert data
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(dsd.DataXML);
            MemoryStream stream = new MemoryStream(byteArray);

            XmlReader reader = new XmlTextReader(stream);
            ds.ReadXml(reader);
            XDocument xd = XDocument.Parse(dsd.DataXML);

            foreach (DataTable dt in ds.Tables)
            {
                var rs = from row in xd.Descendants(dt.TableName)
                         select row;

                int i = 0;
                foreach (var r in rs)
                {
                    DataRowState state = DataRowState.Added;
                    if (r.Attribute("RowState") != null)
                    {
                        state = (DataRowState)Enum.Parse(typeof(DataRowState), r.Attribute("RowState").Value);
                    }

                    DataRow dr = dt.Rows[i];
                    dr.AcceptChanges();

                    if (state == DataRowState.Deleted)
                    {
                        dr.Delete();
                    }
                    else if (state == DataRowState.Added)
                    {
                        dr.SetAdded();
                    }
                    else if (state == DataRowState.Modified)
                    {
                        dr.SetModified();
                    }

                    i++;
                }
            }

            return ds;
        }

        /// <summary>
        /// Convert a DataSet to a NgpDataSet
        /// </summary>
        /// <param name="ds">The DataSet</param>
        /// <returns>Returns the DataSet converted in a NgpDataSet</returns>
        public static NgpDataSet FromDataSet(DataSet ds)
        {
            List<string> columnsToIgnore = new List<string>();

            NgpDataSet dsd = new NgpDataSet();
            dsd.Tables = new ObservableCollection<NgpDataTableInfo>();

            foreach (DataTable t in ds.Tables)
            {
                NgpDataTableInfo tableInfo = new NgpDataTableInfo
                {
                    TableName = t.TableName
                };

                dsd.Tables.Add(tableInfo);
                tableInfo.Columns = new ObservableCollection<NgpDataColumnInfo>();
                foreach (DataColumn c in t.Columns)
                {
                    if (columnsToIgnore == null || (columnsToIgnore != null && !columnsToIgnore.Contains(c.ColumnName)))
                    {
                        NgpDataColumnInfo col = new NgpDataColumnInfo
                        {
                            ColumnName = c.ColumnName,
                            ColumnTitle = c.ColumnName,
                            DataTypeName = c.DataType.FullName,
                            MaxLength = c.MaxLength,
                            IsKey = c.Unique,
                            IsReadOnly = (c.Unique || c.ReadOnly),
                            IsRequired = !c.AllowDBNull
                        };

                        if (c.DataType == typeof(Guid))
                        {
                            col.IsReadOnly = true;
                            col.DisplayIndex = -1;
                        }
                        tableInfo.Columns.Add(col);
                    }
                }
            }

            dsd.DataXML = ds.GetXml();
            dsd.XMLSchema = ds.GetXmlSchema();

            return dsd;
        }
    }
}
