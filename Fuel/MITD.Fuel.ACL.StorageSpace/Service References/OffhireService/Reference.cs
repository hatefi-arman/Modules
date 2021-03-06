﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MITD.Fuel.ACL.StorageSpace.OffhireService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OffhireData", Namespace="http://schemas.datacontract.org/2004/07/OffhireService.Service")]
    [System.SerializableAttribute()]
    public partial class OffhireData : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime EndDateTimeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool HasVoucherField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LocationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private MITD.Fuel.ACL.StorageSpace.OffhireService.OffhireDataItem[] OffhireDetailsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long ReferenceNumberField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime StartDateTimeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string VesselCodeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime EndDateTime {
            get {
                return this.EndDateTimeField;
            }
            set {
                if ((this.EndDateTimeField.Equals(value) != true)) {
                    this.EndDateTimeField = value;
                    this.RaisePropertyChanged("EndDateTime");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool HasVoucher {
            get {
                return this.HasVoucherField;
            }
            set {
                if ((this.HasVoucherField.Equals(value) != true)) {
                    this.HasVoucherField = value;
                    this.RaisePropertyChanged("HasVoucher");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Location {
            get {
                return this.LocationField;
            }
            set {
                if ((object.ReferenceEquals(this.LocationField, value) != true)) {
                    this.LocationField = value;
                    this.RaisePropertyChanged("Location");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public MITD.Fuel.ACL.StorageSpace.OffhireService.OffhireDataItem[] OffhireDetails {
            get {
                return this.OffhireDetailsField;
            }
            set {
                if ((object.ReferenceEquals(this.OffhireDetailsField, value) != true)) {
                    this.OffhireDetailsField = value;
                    this.RaisePropertyChanged("OffhireDetails");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long ReferenceNumber {
            get {
                return this.ReferenceNumberField;
            }
            set {
                if ((this.ReferenceNumberField.Equals(value) != true)) {
                    this.ReferenceNumberField = value;
                    this.RaisePropertyChanged("ReferenceNumber");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime StartDateTime {
            get {
                return this.StartDateTimeField;
            }
            set {
                if ((this.StartDateTimeField.Equals(value) != true)) {
                    this.StartDateTimeField = value;
                    this.RaisePropertyChanged("StartDateTime");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string VesselCode {
            get {
                return this.VesselCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.VesselCodeField, value) != true)) {
                    this.VesselCodeField = value;
                    this.RaisePropertyChanged("VesselCode");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OffhireDataItem", Namespace="http://schemas.datacontract.org/2004/07/OffhireService.Service")]
    [System.SerializableAttribute()]
    public partial class OffhireDataItem : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private MITD.Fuel.ACL.StorageSpace.OffhireService.FuelType FuelTypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal QuantityField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TankCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private MITD.Fuel.ACL.StorageSpace.OffhireService.FuelUnitType UnitTypeCodeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public MITD.Fuel.ACL.StorageSpace.OffhireService.FuelType FuelType {
            get {
                return this.FuelTypeField;
            }
            set {
                if ((this.FuelTypeField.Equals(value) != true)) {
                    this.FuelTypeField = value;
                    this.RaisePropertyChanged("FuelType");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal Quantity {
            get {
                return this.QuantityField;
            }
            set {
                if ((this.QuantityField.Equals(value) != true)) {
                    this.QuantityField = value;
                    this.RaisePropertyChanged("Quantity");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TankCode {
            get {
                return this.TankCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.TankCodeField, value) != true)) {
                    this.TankCodeField = value;
                    this.RaisePropertyChanged("TankCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public MITD.Fuel.ACL.StorageSpace.OffhireService.FuelUnitType UnitTypeCode {
            get {
                return this.UnitTypeCodeField;
            }
            set {
                if ((this.UnitTypeCodeField.Equals(value) != true)) {
                    this.UnitTypeCodeField = value;
                    this.RaisePropertyChanged("UnitTypeCode");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FuelType", Namespace="http://schemas.datacontract.org/2004/07/OffhireService.Service")]
    public enum FuelType : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Ho = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Do = 1,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FuelUnitType", Namespace="http://schemas.datacontract.org/2004/07/OffhireService.Service")]
    public enum FuelUnitType : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Ton = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Litr = 1,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UpdateResult", Namespace="http://schemas.datacontract.org/2004/07/OffhireService.Service")]
    [System.SerializableAttribute()]
    public partial class UpdateResult : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsSucceedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long RefrenceNumberField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsSucceed {
            get {
                return this.IsSucceedField;
            }
            set {
                if ((this.IsSucceedField.Equals(value) != true)) {
                    this.IsSucceedField = value;
                    this.RaisePropertyChanged("IsSucceed");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long RefrenceNumber {
            get {
                return this.RefrenceNumberField;
            }
            set {
                if ((this.RefrenceNumberField.Equals(value) != true)) {
                    this.RefrenceNumberField = value;
                    this.RaisePropertyChanged("RefrenceNumber");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="OffhireService.IOffhireManagementService")]
    public interface IOffhireManagementService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOffhireManagementService/GetFinalizedOffhire", ReplyAction="http://tempuri.org/IOffhireManagementService/GetFinalizedOffhireResponse")]
        MITD.Fuel.ACL.StorageSpace.OffhireService.OffhireData GetFinalizedOffhire(long referenceNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOffhireManagementService/GetFinalizedOffhire", ReplyAction="http://tempuri.org/IOffhireManagementService/GetFinalizedOffhireResponse")]
        System.Threading.Tasks.Task<MITD.Fuel.ACL.StorageSpace.OffhireService.OffhireData> GetFinalizedOffhireAsync(long referenceNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOffhireManagementService/GetFinalizedOffhires", ReplyAction="http://tempuri.org/IOffhireManagementService/GetFinalizedOffhiresResponse")]
        MITD.Fuel.ACL.StorageSpace.OffhireService.OffhireData[] GetFinalizedOffhires(string vesselCode, System.Nullable<System.DateTime> fromDate, System.Nullable<System.DateTime> toDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOffhireManagementService/GetFinalizedOffhires", ReplyAction="http://tempuri.org/IOffhireManagementService/GetFinalizedOffhiresResponse")]
        System.Threading.Tasks.Task<MITD.Fuel.ACL.StorageSpace.OffhireService.OffhireData[]> GetFinalizedOffhiresAsync(string vesselCode, System.Nullable<System.DateTime> fromDate, System.Nullable<System.DateTime> toDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOffhireManagementService/SetVoucherStatus", ReplyAction="http://tempuri.org/IOffhireManagementService/SetVoucherStatusResponse")]
        MITD.Fuel.ACL.StorageSpace.OffhireService.UpdateResult SetVoucherStatus(long refrenceNumber, string vocherNo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOffhireManagementService/SetVoucherStatus", ReplyAction="http://tempuri.org/IOffhireManagementService/SetVoucherStatusResponse")]
        System.Threading.Tasks.Task<MITD.Fuel.ACL.StorageSpace.OffhireService.UpdateResult> SetVoucherStatusAsync(long refrenceNumber, string vocherNo);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IOffhireManagementServiceChannel : MITD.Fuel.ACL.StorageSpace.OffhireService.IOffhireManagementService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class OffhireManagementServiceClient : System.ServiceModel.ClientBase<MITD.Fuel.ACL.StorageSpace.OffhireService.IOffhireManagementService>, MITD.Fuel.ACL.StorageSpace.OffhireService.IOffhireManagementService {
        
        public OffhireManagementServiceClient() {
        }
        
        public OffhireManagementServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public OffhireManagementServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OffhireManagementServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OffhireManagementServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public MITD.Fuel.ACL.StorageSpace.OffhireService.OffhireData GetFinalizedOffhire(long referenceNumber) {
            return base.Channel.GetFinalizedOffhire(referenceNumber);
        }
        
        public System.Threading.Tasks.Task<MITD.Fuel.ACL.StorageSpace.OffhireService.OffhireData> GetFinalizedOffhireAsync(long referenceNumber) {
            return base.Channel.GetFinalizedOffhireAsync(referenceNumber);
        }
        
        public MITD.Fuel.ACL.StorageSpace.OffhireService.OffhireData[] GetFinalizedOffhires(string vesselCode, System.Nullable<System.DateTime> fromDate, System.Nullable<System.DateTime> toDate) {
            return base.Channel.GetFinalizedOffhires(vesselCode, fromDate, toDate);
        }
        
        public System.Threading.Tasks.Task<MITD.Fuel.ACL.StorageSpace.OffhireService.OffhireData[]> GetFinalizedOffhiresAsync(string vesselCode, System.Nullable<System.DateTime> fromDate, System.Nullable<System.DateTime> toDate) {
            return base.Channel.GetFinalizedOffhiresAsync(vesselCode, fromDate, toDate);
        }
        
        public MITD.Fuel.ACL.StorageSpace.OffhireService.UpdateResult SetVoucherStatus(long refrenceNumber, string vocherNo) {
            return base.Channel.SetVoucherStatus(refrenceNumber, vocherNo);
        }
        
        public System.Threading.Tasks.Task<MITD.Fuel.ACL.StorageSpace.OffhireService.UpdateResult> SetVoucherStatusAsync(long refrenceNumber, string vocherNo) {
            return base.Channel.SetVoucherStatusAsync(refrenceNumber, vocherNo);
        }
    }
}
