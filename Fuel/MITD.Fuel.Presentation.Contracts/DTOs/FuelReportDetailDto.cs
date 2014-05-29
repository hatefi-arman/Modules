using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.Infrastructure;
using MITD.Presentation;

namespace MITD.Fuel.Presentation.Contracts.DTOs
{
    public partial class FuelReportDetailDto
    {
        private GoodDto good;
        public virtual GoodDto Good
        {
            get { return good; }
            set
            {
                this.SetField(p => p.Good, ref good, value);
            }
        }

        /// <summary>
        /// There are no comments for ReceiveTypeId in the schema.
        /// </summary>
        public virtual long Id
        {
            get
            {
                return _id;
            }
            set
            {
                this.SetField(c => c.Id, ref _id, value);
            }
        }
        private long _id;



        /// <summary>
        /// There are no comments for Consumption in the schema.
        /// </summary>
        public virtual double? Consumption
        {
            get
            {
                return _consumption;
            }
            set
            {
                this.SetField(c => c.Consumption, ref _consumption, value);
            }
        }
        private double? _consumption;


        /// <summary>
        /// There are no comments for Correction in the schema.
        /// </summary>
        public virtual double? Correction
        {
            get
            {
                return _correction;
            }
            set
            {
                this.SetField(c => c.Correction, ref _correction, value);
            }
        }
        private double? _correction;


        /// <summary>
        /// There are no comments for CorrectionPrice in the schema.
        /// </summary>
        public virtual double? CorrectionPrice
        {
            get
            {
                return _correctionPrice;
            }
            set
            {
                this.SetField(c => c.CorrectionPrice, ref _correctionPrice, value);
            }
        }
        private double? _correctionPrice;


        /// <summary>
        /// There are no comments for CorrectionType in the schema.
        /// </summary>
        public virtual CorrectionTypeEnum CorrectionType
        {
            get
            {
                return _correctionType;
            }
            set
            {
                this.SetField(c => c.CorrectionType, ref _correctionType, value);
            }
        }
        private CorrectionTypeEnum _correctionType;


        /// <summary>
        /// There are no comments for Recieve in the schema.
        /// </summary>
        public virtual double? Recieve
        {
            get
            {
                return _recieve;
            }
            set
            {
                this.SetField(c => c.Recieve, ref _recieve, value);
            }
        }
        private double? _recieve;


        /// <summary>
        /// There are no comments for ROB in the schema.
        /// </summary>


        [CustomValidation(typeof(ValidationDto), "IsGreaterZero")]
        public virtual double ROB
        {
            get
            {
                return _rob;
            }
            set
            {
                this.SetField(c => c.ROB, ref _rob, value);
            }
        }
        private double _rob;


        /// <summary>
        /// There are no comments for Transfer in the schema.
        /// </summary>
        public virtual double? Transfer
        {
            get
            {
                return _transfer;
            }
            set
            {
                this.SetField(c => c.Transfer, ref _transfer, value);
            }
        }
        private double? _transfer;


        /// <summary>
        /// There are no comments for FuelReportId in the schema.
        /// </summary>
        public virtual long FuelReportId
        {
            get
            {
                return _fuelReportId;
            }
            set
            {
                this.SetField(c => c.FuelReportId, ref _fuelReportId, value);
            }
        }
        private long _fuelReportId;


        /// <summary>
        /// There are no comments for ReceiveTypeId in the schema.
        /// </summary>
        public virtual ReceiveTypeEnum ReceiveType
        {
            get
            {
                return _receiveType;
            }
            set
            {
                this.SetField(c => c.ReceiveType, ref _receiveType, value);
            }
        }
        private ReceiveTypeEnum _receiveType;


        /// <summary>
        /// There are no comments for TransferTypeId in the schema.
        /// </summary>
        public virtual TransferTypeEnum TransferType
        {
            get
            {
                return _transferType;
            }
            set
            {
                this.SetField(c => c.TransferType, ref _transferType, value);
            }
        }
        private TransferTypeEnum _transferType;




        public virtual long? GoodId
        {
            get
            {
                return _goodId;
            }
            set
            {
                this.SetField(c => c.GoodId, ref _goodId, value);
            }
        }
        private long? _goodId;

        public virtual long? TankId
        {
            get
            {
                return _tankId;
            }
            set
            {
                this.SetField(c => c.TankId, ref _tankId, value);
            }
        }
        private long? _tankId;

        public virtual long? GoodUnitId
        {
            get
            {
                return _goodUnitId;
            }
            set
            {
                this.SetField(c => c.GoodUnitId, ref _goodUnitId, value);
            }
        }
        private long? _goodUnitId;




        private FuelReportTransferReferenceNoDto fuelReportTransferReferenceNoDto;
        public FuelReportTransferReferenceNoDto FuelReportTransferReferenceNoDto
        {
            get { return fuelReportTransferReferenceNoDto; }
            set { this.SetField(p => p.FuelReportTransferReferenceNoDto, ref fuelReportTransferReferenceNoDto, value); }
        }

        private List<FuelReportTransferReferenceNoDto> fuelReportTransferReferenceNoDtos;
        public List<FuelReportTransferReferenceNoDto> FuelReportTransferReferenceNoDtos
        {
            get { return fuelReportTransferReferenceNoDtos; }
            set { this.SetField(p => p.FuelReportTransferReferenceNoDtos, ref fuelReportTransferReferenceNoDtos, value); }
        }

        private List<FuelReportTransferReferenceNoDto> rejectedTransferReferenceNoDtos;
        public List<FuelReportTransferReferenceNoDto> RejectedTransferReferenceNoDtos
        {
            get { return rejectedTransferReferenceNoDtos; }
            set { this.SetField(p => p.RejectedTransferReferenceNoDtos, ref rejectedTransferReferenceNoDtos, value); }
        }

        private FuelReportCorrectionReferenceNoDto fuelReportCorrectionReferenceNoDto;
        public FuelReportCorrectionReferenceNoDto FuelReportCorrectionReferenceNoDto
        {
            get { return fuelReportCorrectionReferenceNoDto; }
            set { this.SetField(p => p.FuelReportCorrectionReferenceNoDto, ref fuelReportCorrectionReferenceNoDto, value); }
        }

        private List<FuelReportCorrectionReferenceNoDto> fuelReportCorrectionReferenceNoDtos;
        public List<FuelReportCorrectionReferenceNoDto> FuelReportCorrectionReferenceNoDtos
        {
            get { return fuelReportCorrectionReferenceNoDtos; }
            set { this.SetField(p => p.FuelReportCorrectionReferenceNoDtos, ref fuelReportCorrectionReferenceNoDtos, value); }
        }


        private FuelReportReciveReferenceNoDto fuelReportReciveReferenceNoDto;
        public FuelReportReciveReferenceNoDto FuelReportReciveReferenceNoDto
        {
            get { return fuelReportReciveReferenceNoDto; }
            set { this.SetField(p => p.FuelReportReciveReferenceNoDto, ref fuelReportReciveReferenceNoDto, value); }
        }

        private List<FuelReportReciveReferenceNoDto> fuelReportReciveReferenceNoDtos;
        public List<FuelReportReciveReferenceNoDto> FuelReportReciveReferenceNoDtos
        {
            get { return fuelReportReciveReferenceNoDtos; }
            set { this.SetField(p => p.FuelReportReciveReferenceNoDtos, ref fuelReportReciveReferenceNoDtos, value); }
        }

        private DTOs.CurrencyDto currencyDto;
        public CurrencyDto CurrencyDto
        {
            get { return currencyDto; }
            set { this.SetField(p => p.CurrencyDto, ref currencyDto, value); }
        }
    }
}
