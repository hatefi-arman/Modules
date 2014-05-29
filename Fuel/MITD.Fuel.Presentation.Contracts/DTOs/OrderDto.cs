#region

using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts;
#endregion

namespace MITD.Fuel.Presentation.Contracts.DTOs
{
    // [DataContract]
    public partial class OrderDto
    {
        #region props

        private string userInChargName;
        private string code;
        private string currentStateName;
        private string description;
        private VesselDto fromVessel;
        private long id;
        private DateTime? orderDate;
        private OrderTypeEnum orderType;
        private CompanyDto _owner;
        private CompanyDto _receiver;
        private CompanyDto _supplier;
        private VesselDto _toVessel;
        private CompanyDto _transporter;
        private WorkflowStageEnum approveStatus;

        public long Id
        {
            get { return id; }
            set { this.SetField(p => p.Id, ref id, value); }
        }

        public string Code
        {
            get { return code; }
            set { this.SetField(p => p.Code, ref code, value); }
        }

        public string UserInChargName
        {
            get { return userInChargName; }
            set { this.SetField(p => p.UserInChargName, ref userInChargName, value); }
        }

        public string CurrentStateName
        {
            get { return currentStateName; }
            set { this.SetField(p => p.CurrentStateName, ref currentStateName, value); }
        }

//        [Required(AllowEmptyStrings = false, ErrorMessage = "error")]
        public string Description
        {
            get { return description; }
            set { this.SetField(p => p.Description, ref description, value); }
        }

        public WorkflowStageEnum ApproveStatus
        {
            get { return approveStatus; }
            set { this.SetField(p => p.ApproveStatus, ref approveStatus, value); }
        }
        public string ApproveStatusString
        {
            get { return approveStatus.GetDescription(); }
        }

        public OrderTypeEnum OrderType
        {
            get { return orderType; }
            set { this.SetField(p => p.OrderType, ref orderType, value); }
        }

        public string OrderTypeString
        {
            get { return OrderType.GetDescription(); }
        }

        public DateTime? OrderDate
        {
            get { return orderDate; }
            set { this.SetField(p => p.OrderDate, ref orderDate, value); }
        }

        public CompanyDto Supplier
        {
            get { return _supplier; }
            set { this.SetField(p => p.Supplier, ref _supplier, value); }
        }


        public CompanyDto Receiver
        {
            get { return _receiver; }
            set
            {
                this.SetField(p => p.Receiver, ref _receiver, value);
            }
        }

        public VesselDto FromVessel
        {
            get { return fromVessel; }
            set { this.SetField(p => p.FromVessel, ref fromVessel, value); }
        }

        public VesselDto ToVessel
        {
            get { return _toVessel; }
            set { this.SetField(p => p.ToVessel, ref _toVessel, value); }
        }

        public CompanyDto Owner
        {
            get { return _owner; }
            set { this.SetField(p => p.Owner, ref _owner, value); }
        }

        public CompanyDto Transporter
        {
            get { return _transporter; }
            set { this.SetField(p => p.Transporter, ref _transporter, value); }
        }

        #endregion

        private ObservableCollection<OrderItemDto> orderItemDto;
        private long _receiverId;

        public ObservableCollection<OrderItemDto> OrderItems
        {
            get { return orderItemDto; }
            set { this.SetField(p => p.OrderItems, ref orderItemDto, value); }
        }

    }
}