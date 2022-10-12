﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERPWebService
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://osram-os.com/rbg/pp/sfc/sapbookings/xi103", ConfigurationName="ERPWebService.MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUT")]
    public interface MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUT
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://sap.com/xi/WebService/soap1.1", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<ERPWebService.MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTResponse> MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTAsync(ERPWebService.MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class GoodsIssueRow
    {
        
        private string productionOrderNrField;
        
        private string costCenterField;
        
        private string lotNumberField;
        
        private string materialNrField;
        
        private string quantityField;
        
        private string quantityUnitField;
        
        private string sapStoreField;
        
        private string siteField;
        
        private string movementTypeField;
        
        private string sapToStoreField;
        
        private string batchField;
        
        private string matRecNrField;
        
        private string matCalYearField;
        
        private string idField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string ProductionOrderNr
        {
            get
            {
                return this.productionOrderNrField;
            }
            set
            {
                this.productionOrderNrField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string CostCenter
        {
            get
            {
                return this.costCenterField;
            }
            set
            {
                this.costCenterField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string LotNumber
        {
            get
            {
                return this.lotNumberField;
            }
            set
            {
                this.lotNumberField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string MaterialNr
        {
            get
            {
                return this.materialNrField;
            }
            set
            {
                this.materialNrField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string Quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string QuantityUnit
        {
            get
            {
                return this.quantityUnitField;
            }
            set
            {
                this.quantityUnitField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=6)]
        public string SapStore
        {
            get
            {
                return this.sapStoreField;
            }
            set
            {
                this.sapStoreField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=7)]
        public string Site
        {
            get
            {
                return this.siteField;
            }
            set
            {
                this.siteField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=8)]
        public string MovementType
        {
            get
            {
                return this.movementTypeField;
            }
            set
            {
                this.movementTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=9)]
        public string SapToStore
        {
            get
            {
                return this.sapToStoreField;
            }
            set
            {
                this.sapToStoreField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=10)]
        public string Batch
        {
            get
            {
                return this.batchField;
            }
            set
            {
                this.batchField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=11)]
        public string MatRecNr
        {
            get
            {
                return this.matRecNrField;
            }
            set
            {
                this.matRecNrField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=12)]
        public string MatCalYear
        {
            get
            {
                return this.matCalYearField;
            }
            set
            {
                this.matCalYearField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class GoodsIssueReplyRow
    {
        
        private string productionOrderNrField;
        
        private string lotNumberField;
        
        private string materialNrField;
        
        private string errorMessageField;
        
        private string sapTransferDateField;
        
        private string matRecNrField;
        
        private string matCalYearField;
        
        private string idField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string ProductionOrderNr
        {
            get
            {
                return this.productionOrderNrField;
            }
            set
            {
                this.productionOrderNrField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string LotNumber
        {
            get
            {
                return this.lotNumberField;
            }
            set
            {
                this.lotNumberField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string MaterialNr
        {
            get
            {
                return this.materialNrField;
            }
            set
            {
                this.materialNrField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string ErrorMessage
        {
            get
            {
                return this.errorMessageField;
            }
            set
            {
                this.errorMessageField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string SapTransferDate
        {
            get
            {
                return this.sapTransferDateField;
            }
            set
            {
                this.sapTransferDateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string MatRecNr
        {
            get
            {
                return this.matRecNrField;
            }
            set
            {
                this.matRecNrField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=6)]
        public string MatCalYear
        {
            get
            {
                return this.matCalYearField;
            }
            set
            {
                this.matCalYearField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Row", IsNullable=false)]
        public ERPWebService.GoodsIssueRow[] GoodsIssue;
        
        public MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTRequest()
        {
        }
        
        public MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTRequest(ERPWebService.GoodsIssueRow[] GoodsIssue)
        {
            this.GoodsIssue = GoodsIssue;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Row", IsNullable=false)]
        public ERPWebService.GoodsIssueReplyRow[] GoodsIssueReply;
        
        public MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTResponse()
        {
        }
        
        public MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTResponse(ERPWebService.GoodsIssueReplyRow[] GoodsIssueReply)
        {
            this.GoodsIssueReply = GoodsIssueReply;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public interface MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUTChannel : ERPWebService.MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUT, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public partial class MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUTClient : System.ServiceModel.ClientBase<ERPWebService.MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUT>, ERPWebService.MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUT
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUTClient(EndpointConfiguration endpointConfiguration) : 
                base(MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUTClient.GetBindingForEndpoint(endpointConfiguration), MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUTClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUTClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUTClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUTClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUTClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUTClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ERPWebService.MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTResponse> ERPWebService.MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUT.MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTAsync(ERPWebService.MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTRequest request)
        {
            return base.Channel.MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTAsync(request);
        }
        
        public System.Threading.Tasks.Task<ERPWebService.MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTResponse> MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTAsync(ERPWebService.GoodsIssueRow[] GoodsIssue)
        {
            ERPWebService.MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTRequest inValue = new ERPWebService.MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTRequest();
            inValue.GoodsIssue = GoodsIssue;
            return ((ERPWebService.MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUT)(this)).MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTAsync(inValue);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.HTTP_Port))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.HTTPS_Port))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                result.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.HTTP_Port))
            {
                return new System.ServiceModel.EndpointAddress(@"http://webx9d.int.osram-light.com/XISOAPAdapter/MessageServlet?senderParty=&senderService=BS_NON_T_CMFMES&receiverParty=&receiverService=&interface=MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUT&interfaceNamespace=http%3A%2F%2Fosram-os.com%2Frbg%2Fpp%2Fsfc%2Fsapbookings%2Fxi103");
            }
            if ((endpointConfiguration == EndpointConfiguration.HTTPS_Port))
            {
                return new System.ServiceModel.EndpointAddress(@"https://webx9d.int.osram-light.com/XISOAPAdapter/MessageServlet?senderParty=&senderService=BS_NON_T_CMFMES&receiverParty=&receiverService=&interface=MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUT&interfaceNamespace=http%3A%2F%2Fosram-os.com%2Frbg%2Fpp%2Fsfc%2Fsapbookings%2Fxi103");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        public enum EndpointConfiguration
        {
            
            HTTP_Port,
            
            HTTPS_Port,
        }
    }
}