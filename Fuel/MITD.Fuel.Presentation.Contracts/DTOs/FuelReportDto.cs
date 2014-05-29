using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Presentation;

namespace MITD.Fuel.Presentation.Contracts.DTOs
{

    public partial class FuelReportDto
    {
        public FuelReportDto()
        {
        }

        long id;
        public long Id
        {
            get { return id; }
            set { this.SetField(p => p.Id, ref id, value); }
        }

        string code;
        [Required(AllowEmptyStrings = false, ErrorMessage = "code can't be empty")]
        public string Code
        {
            get { return code; }
            set { this.SetField(p => p.Code, ref code, value); }
        }

        string _description;
        public string Description
        {
            get { return _description; }
            set { this.SetField(p => p.Description, ref _description, value); }
        }



        string _CurrentStateName;
        public string CurrentStateName
        {
            get { return _CurrentStateName; }
            set { this.SetField(p => p.CurrentStateName, ref _CurrentStateName, value); }
        }
        string _UserInChargName;
        public string UserInChargName
        {
            get { return _UserInChargName; }
            set { this.SetField(p => p.UserInChargName, ref _UserInChargName, value); }
        }

        FuelReportTypeEnum _fuelReportType;
        public FuelReportTypeEnum FuelReportType
        {
            get { return _fuelReportType; }
            set { this.SetField(p => p.FuelReportType, ref _fuelReportType, value); }
        }

        DateTime _reportDate;
        public DateTime ReportDate
        {
            get { return _reportDate; }
            set { this.SetField(p => p.ReportDate, ref _reportDate, value); }
        }

        DateTime _eventDate;
        public DateTime EventDate
        {
            get { return _eventDate; }
            set { this.SetField(p => p.EventDate, ref _eventDate, value); }
        }

        VoyageDto _voyage;
        public VoyageDto Voyage
        {
            get { return _voyage; }
            set { this.SetField(p => p.Voyage, ref _voyage, value); }
        }

        private bool _isNextActionFinalApprove;


        public bool IsNextActionFinalApprove
        {
            get { return _isNextActionFinalApprove; }
            set { this.SetField(p => p.IsNextActionFinalApprove, ref _isNextActionFinalApprove, value); }
        }


        private bool _isActive;


        public bool IsActive
        {
            get { return _isActive; }
            set { this.SetField(p => p.IsActive, ref _isActive, value); }
        }

        private VesselDto vesselDto;
        public VesselDto VesselDto
        {
            get { return vesselDto; }
            set { this.SetField(p => p.VesselDto, ref vesselDto, value); }
        }

        private ObservableCollection<FuelReportDetailDto> fuelReportDetail;
        public ObservableCollection<FuelReportDetailDto> FuelReportDetail
        {
            get { return this.fuelReportDetail; }
            set { this.SetField(p => p.FuelReportDetail, ref this.fuelReportDetail, value); }
        }
    }
}
