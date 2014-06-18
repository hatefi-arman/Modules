using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MITD.CurrencyAndMeasurement.Domain.Contracts;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Specifications;
using MITD.Fuel.Domain.Model.IDomainServices.Events.InventoryOperations;
using MITD.Fuel.Domain.Model.DomainObjects.FuelReportAggregate.FuelReportStates;

namespace MITD.Fuel.Domain.Model.DomainObjects
{
    public class FuelReport
    {
        #region Properties

        public long Id { get; private set; }

        public string Code { get; private set; }

        public string Description { get; private set; }

        public DateTime EventDate { get; private set; }

        public DateTime ReportDate { get; private set; }

        public long VesselInCompanyId { get; private set; }

        public virtual VesselInCompany VesselInCompany { get; private set; }

        public long? VoyageId { get; private set; }

        public virtual Voyage Voyage { get; private set; }

        public FuelReportTypes FuelReportType { get; private set; }

        public virtual List<FuelReportWorkflowLog> ApproveWorkFlows { get; private set; }

        public byte[] TimeStamp { get; private set; }

        public virtual ICollection<FuelReportDetail> FuelReportDetails { get; private set; }

        public States State { get; private set; }

        public FuelReportState EntityState { get; private set; }

        public virtual List<InventoryOperation> ConsumptionInventoryOperations { get; set; }

        private readonly IsFuelReportClosed isFuelReportClosed;

        private readonly IsFuelReportOpen isFuelReportOpen;

        private readonly IsFuelReportWaitingToBeClosed isFuelReportWaitingToBeClosed;

        private readonly IsFuelReportOperational isFuelReportOperational;

        private readonly IsFuelReportNotCancelled isFuelReportNotCancelled;

        private readonly IsFuelReportSubmitRejected isFuelReportSubmitRejected;

        private readonly IsFuelReportSubmitted isFuelReportSubmitted;


        #endregion

        #region Constructors

        public FuelReport()
        {
            this.isFuelReportClosed = new IsFuelReportClosed();

            this.isFuelReportOpen = new IsFuelReportOpen();

            this.isFuelReportWaitingToBeClosed = new IsFuelReportWaitingToBeClosed();

            this.isFuelReportOperational = new IsFuelReportOperational();
            this.isFuelReportNotCancelled = new IsFuelReportNotCancelled();
            this.isFuelReportSubmitRejected = new IsFuelReportSubmitRejected();
            this.isFuelReportSubmitted = new IsFuelReportSubmitted();

            this.ApproveWorkFlows = new List<FuelReportWorkflowLog>();
            this.FuelReportDetails = new Collection<FuelReportDetail>();
        }

        internal FuelReport(
            long id,
            string code,
            string description,
            DateTime eventDate,
            DateTime reportDate,
            long vesselInCompanyId,
            long? voyageId,
            FuelReportTypes fuelReportType,
            States state)
            : this()
        {
            //Id = id;

            Code = code;

            Description = description;

            EventDate = eventDate;

            ReportDate = reportDate;

            VesselInCompanyId = vesselInCompanyId;

            VoyageId = voyageId;

            FuelReportType = fuelReportType;

            State = state;
        }

        internal FuelReport(
            long id,
            string code,
            string description,
            DateTime eventDate,
            DateTime reportDate,
            VesselInCompany vesselInCompany,
            long? voyageId,
            FuelReportTypes fuelReportType,
            States state)
            : this()
        {
            //Id = id;

            Code = code;

            Description = description;

            EventDate = eventDate;

            ReportDate = reportDate;

            VesselInCompanyId = vesselInCompany.Id;
            VesselInCompany = vesselInCompany;

            VoyageId = voyageId;

            FuelReportType = fuelReportType;

            State = state;
        }

        #endregion

        #region Methods

        //===================================================================================

        public void UpdateVoyageId(long? voyageId, IVoyageDomainService voyageDomainService)
        {
            this.CheckToBeOperational();

            //BR_FR1
            this.validateToBeInOpenState();

            validateVoyageId(voyageId, voyageDomainService);

            VoyageId = voyageId;
        }

        //===================================================================================

        private void validateVoyageId(long? voyageId, IVoyageDomainService voyageDomainService)
        {
            //BR_FR2
            validateVoyageValue(voyageId, voyageDomainService);

            //BR_FR35
            validateVoyageForEndOfVoyageCondition(voyageId);

            //BR_FR36
            validateVoyageEndDateForEndOfVoyageReportType(voyageId, voyageDomainService);
        }

        //===================================================================================

        #endregion

        #region FuelReportDetail Operations

        //===================================================================================

        public FuelReportDetail UpdateFuelReportDetail(
            long fuelReportDetailId,
            double rob,
            double consumption,
            double? receive,
            ReceiveTypes? receiveType,
            double? transfer,
            TransferTypes? transferType,
            double? correction,
            CorrectionTypes? correctionType,
            decimal? correctionPrice,
            long? currencyId,
            Reference transferReference,
            Reference receiveReference,
            Reference correctionReference,
            IFuelReportDomainService fuelReportDomainService,
            ICurrencyDomainService currencyDomainService)
        {
            this.CheckToBeOperational();

            //BR_FR1
            validateToBeOpenOrSubmitRejected();

            FuelReportDetail updatingFuelReportDetail = FuelReportDetails.FirstOrDefault(c => c.Id == fuelReportDetailId);

            if (updatingFuelReportDetail == null) throw new ObjectNotFound("FuelReportDetail", fuelReportDetailId);


            validatePreviousFuelReportsToBeFinalApproved(fuelReportDomainService);



            //IOrderedEnumerable<FuelReport> fuelReportsOfYesterday =
            //    fuelReportDomainService
            //        .GetYesterdayFuelReports(this)
            //            .OrderBy(fr => fr.ReportDate);//The ordering is used to find the last report in previous day.

            //FuelReport previousFuelReport = null;

            //bool isTheFirstReport = false;

            //if (fuelReportsOfYesterday.Count() == 0)
            //{
            //    //If nothing found for previous day...

            //    isTheFirstReport = isCurrentFuelReportTheFirstOne(fuelReportDomainService);
            //}
            //else
            //{
            //    //Retrieve the last Fuel Report of yesterday. 
            //    previousFuelReport = fuelReportsOfYesterday.LastOrDefault();
            //}


            //FuelReportDetail fuelReportDetailOfYesterdayForRelevantGood = null;

            //if (!isTheFirstReport)
            //{
            //    //TODO: The validation of fuel types against Yesterday valid fuel types must be revised.
            //    fuelReportDetailOfYesterdayForRelevantGood =
            //        previousFuelReport.FuelReportDetails.FirstOrDefault(c => c.GoodId == updatingFuelReportDetail.GoodId);

            //    if (fuelReportDetailOfYesterdayForRelevantGood == null)
            //        //Because current Fuel Report is not the first one, 
            //        //its relevant Fuel Report Detail of yesterday must be found.
            //        throw new ObjectNotFound("FuelReportDetailOfYesterdayForRelevantGood");
            //}

            bool isTheFirstReport = isCurrentFuelReportTheFirstOne(fuelReportDomainService);

            FuelReport previousFuelReport = getYesterdayLastFuelReport(fuelReportDomainService);

            validateFuelReportOfTheDayBefore(previousFuelReport, isTheFirstReport);

            FuelReportDetail fuelReportDetailOfYesterdayForRelevantGood =
                getGoodRelevantFuelReportDetailOfYesterday(
                    updatingFuelReportDetail.GoodId,
                    isTheFirstReport,
                    previousFuelReport);


            if (this.State == States.Open)
            {
                updatingFuelReportDetail.Update(rob, consumption, receive, receiveType, transfer, transferType,
                                                correction, correctionType, correctionPrice, currencyId,
                                                transferReference,
                                                receiveReference,
                                                correctionReference,
                                                fuelReportDetailOfYesterdayForRelevantGood,
                                                isTheFirstReport,
                                                currencyDomainService);
            }
            else if (this.State == States.SubmitRejected)
            {
                updatingFuelReportDetail.Update(rob, consumption, fuelReportDetailOfYesterdayForRelevantGood, isTheFirstReport, currencyDomainService);
            }
            else
            {
                throw new InvalidOperation("UpdateFuelReportDetail", "The Fuel Report is in an invalid state.");
            }

            return updatingFuelReportDetail;
        }

        //===================================================================================

        private FuelReportDetail getGoodRelevantFuelReportDetailOfYesterday(long goodId, bool isTheFirstReport, FuelReport previousFuelReport)
        {
            FuelReportDetail fuelReportDetailOfYesterdayForRelevantGood = null;

            if (!isTheFirstReport)
            {
                //TODO: The validation of fuel types against Yesterday valid fuel types must be revised.
                fuelReportDetailOfYesterdayForRelevantGood =
                    previousFuelReport.FuelReportDetails.FirstOrDefault(c => c.GoodId == goodId);

                if (fuelReportDetailOfYesterdayForRelevantGood == null)
                    //Because current Fuel Report is not the first one, 
                    //its relevant Fuel Report Detail of yesterday must be found.
                    throw new ObjectNotFound("FuelReportDetailOfYesterdayForRelevantGood", goodId);
            }

            return fuelReportDetailOfYesterdayForRelevantGood;
        }

        //===================================================================================

        private FuelReport getYesterdayLastFuelReport(IFuelReportDomainService fuelReportDomainService)
        {
            IOrderedEnumerable<FuelReport> fuelReportsOfYesterday =
                fuelReportDomainService.GetFuelReportsFromYesterday(this)
                    .OrderBy(fr => fr.EventDate);//The ordering is used to find the last report from previous day till current Report.

            return fuelReportsOfYesterday.LastOrDefault();
        }

        //===================================================================================

        private void validateToBeOpenOrSubmitRejected()
        {
            if (!this.isFuelReportOpen.IsSatisfiedBy(this) && !this.isFuelReportSubmitRejected.IsSatisfiedBy(this))
                throw new BusinessRuleException("", "Fuel Report state is invalid.");
        }

        //===================================================================================

        private bool isCurrentFuelReportTheFirstOne(IFuelReportDomainService fuelReportDomainService)
        {
            var isTheFirstOne = false;

            var oneDayBefore = this.EventDate.AddDays(-1).Date;

            long fuelReportsCountBeforeCurrentDay =
                fuelReportDomainService
                    .GetVesselFuelReports(this.VesselInCompany, null, oneDayBefore, true)
                        .Count();

            isTheFirstOne = (fuelReportsCountBeforeCurrentDay == 0);

            return isTheFirstOne;
        }

        //===================================================================================

        #endregion

        #region BusinessRules

        //===================================================================================

        /// <summary>
        /// BR_FR2
        /// </summary>
        private void validateVoyageValue(long? voyageId, IVoyageDomainService voyageDomainService)
        {
            if (voyageId.HasValue &&
                !(
                    voyageDomainService.IsVoyageAvailable(voyageId.Value) &&
                    isVoyageDurationAndVesselValid(voyageId, voyageDomainService)
                )
            )
                throw new BusinessRuleException("BR_FR2", "Voyage is not Valid.");
        }

        //===================================================================================

        /// <summary>
        /// This validation is part of validateVoyageValue
        /// </summary>
        private bool isVoyageDurationAndVesselValid(long? voyageId, IVoyageDomainService voyageDomainService)
        {
            if (voyageId.HasValue)
            {
                var givenVoyage = voyageDomainService.Get(voyageId.Value);

                if (!(givenVoyage.VesselInCompany.Id == this.VesselInCompanyId &&
                    (givenVoyage.StartDate <= EventDate &&
                        EventDate <= givenVoyage.EndDate))
                    )
                {
                    return false;
                }
            }

            return true;
        }

        //===================================================================================

        private void validateFuelReportOfTheDayBefore(
            FuelReport fuelReportOfTheDayBefore,
            bool isCurrentFuelReportTheFirst)
        {
            if (fuelReportOfTheDayBefore == null && !isCurrentFuelReportTheFirst)
                //This means that current Fuel Report is not the first one in the whole system and 
                //has no Fuel Report in yesterday.
                throw new ObjectNotFound("FuelReportOfTheDayBefore"); //"No fuel report found for previous day.";


            if (fuelReportOfTheDayBefore != null)
            {
                //Checking FuelReport of the Day Before for final approval state.
                //TODO: The business rule code must be indicated.
                if (!isFuelReportClosed.IsSatisfiedBy(fuelReportOfTheDayBefore))
                    throw new BusinessRuleException("XXXXXX", "The previous Fuel Report must be Final Approved.");
            }
        }

        //===================================================================================

        /// <summary>
        /// BR_FR35
        /// </summary>
        private void validateVoyageForEndOfVoyageCondition(long? voyageId)
        {
            if (FuelReportType == FuelReportTypes.EndOfVoyage &&
                !voyageId.HasValue)
                throw new BusinessRuleException("BR_FR35", "Voyage is not specified for End Of Voyage Fuel Report.");
        }

        //===================================================================================

        //private void validateVoyageExistance(long? voyageId)
        //{
        //    if (FuelReportType == FuelReportTypes.EndOfVoyage &&
        //        !voyageId.HasValue)
        //        throw new BusinessRuleException("BR_FR35", "Voyage is mandatory for EOV Fuel Report.");
        //}

        //===================================================================================

        /// <summary>
        /// BR_FR36
        /// </summary>
        private void validateVoyageEndDateForEndOfVoyageReportType(
            long? voyageId,
            IVoyageDomainService voyageDomainService)
        {
            if (FuelReportType == FuelReportTypes.EndOfVoyage &&
                voyageId.HasValue)
            {
                var givenVoyage = voyageDomainService.Get(voyageId.Value);

                if (!givenVoyage.EndDate.HasValue)
                    throw new BusinessRuleException("", "Voyage has not ended or its EndDate is not reported yet.");

                if (!(
                        givenVoyage.VesselInCompany.Id == this.VesselInCompanyId && //This is already checked in BR_FR2, but implemented due to Analysis indication.
                        (//Compare found voyage End Date with current fuel Report Date with the resolution of hour.
                        //TODO: value
                            EventDate.Date == givenVoyage.EndDate.Value.Date &&
                            EventDate.Hour == givenVoyage.EndDate.Value.Hour
                        )
                    )
                )
                {
                    throw new BusinessRuleException("BR_FR36", "Given Voyage is not match with Fuel Report Date and Vessel.");
                }
            }
        }

        //===================================================================================

        /// <summary>
        /// BR_FR27
        /// </summary>
        private void validateToBeInOpenState()
        {
            if (!this.isFuelReportOpen.IsSatisfiedBy(this))
                throw new BusinessRuleException("BR_FR27", "Entity is not in Mid-Approve state.");
        }

        //===================================================================================

        #endregion

        #region Approve Workflow

        //===================================================================================

        private void validateSubmittingState(IVoyageDomainService voyageDomainService,
            IFuelReportDomainService fuelReportDomainService, IInventoryOperationDomainService inventoryOperationDomainService, IGoodDomainService goodDomainService,
            IOrderDomainService orderDomainService, ICurrencyDomainService currencyDomainService,
            IInventoryManagementDomainService inventoryManagementDomainService)
        {
            validateToBeOpenOrSubmitRejected();

            validateVoyageId(this.VoyageId, voyageDomainService);

            //BR_FR37
            validatePreviousEndedVoyagesToBeIssued(
                voyageDomainService,
                fuelReportDomainService,
                inventoryOperationDomainService);

            //BR_FR38
            validatePreviousFuelReportsToBeFinalApproved(fuelReportDomainService);

            //BR_FR39            
            validateFuelReportsOfPreviousDayForMandatoryType(
                fuelReportDomainService,
                goodDomainService);


            var isTheFirstReport = isCurrentFuelReportTheFirstOne(fuelReportDomainService);

            FuelReport previousFuelReport = getYesterdayLastFuelReport(fuelReportDomainService);

            validateFuelReportOfTheDayBefore(previousFuelReport, isTheFirstReport);

            foreach (var detailItem in this.FuelReportDetails)
            {
                FuelReportDetail fuelReportDetailOfYesterdayForRelevantGood =
                    getGoodRelevantFuelReportDetailOfYesterday(
                        detailItem.GoodId,
                        isTheFirstReport,
                        previousFuelReport);

                //All Edit Rules must be checked for details.
                detailItem.ValidateCurrentValues(
                    fuelReportDetailOfYesterdayForRelevantGood,
                    isTheFirstReport,
                    currencyDomainService);

                detailItem.ValidateTransferReferences(orderDomainService, inventoryManagementDomainService);
                detailItem.ValidateReceiveReferences(orderDomainService);
                detailItem.ValidateCorrectionReferences(isTheFirstReport, voyageDomainService, fuelReportDomainService);
            }
        }

        //===================================================================================

        /// <summary>
        /// BR_FR37
        /// </summary>
        private void validatePreviousEndedVoyagesToBeIssued(
            IVoyageDomainService voyageDomainService,
            IFuelReportDomainService fuelReportDomainService,
            IInventoryOperationDomainService inventoryOperationDomainService)
        {
            var previousEOVFuelReports = fuelReportDomainService.GetNotIssuedEOVFuelReportsOfPreviousVoyages(this);

            var isFuelReportIssuedSpec = new IsFuelReportIssued(inventoryOperationDomainService);

            var notIssuedEOVFuelReportsOfPreviousVoyages =
                previousEOVFuelReports.FindAll(
                    eovfr => !isFuelReportIssuedSpec.IsSatisfiedBy(eovfr));

            if (notIssuedEOVFuelReportsOfPreviousVoyages.Count != 0)
                throw new BusinessRuleException("BR_FR37", "There are some ended voyages before current Fuel Report with no issue for End Of Voyage Fuel Report.");
        }

        //===================================================================================

        /// <summary>
        /// BR_FR38
        /// </summary>
        private void validatePreviousFuelReportsToBeFinalApproved(IFuelReportDomainService fuelReportDomainService)
        {
            var previousNotFinalApprovedReports = fuelReportDomainService.GetPreviousNotFinalApprovedReports(this);

            if (previousNotFinalApprovedReports.Count() != 0)
                throw new BusinessRuleException("BR_FR38", "There are some not final approved Fuel Reports before current FuelReport.");
        }

        //===================================================================================

        /// <summary>
        /// BR_FR39
        /// </summary>
        private void validateFuelReportsOfPreviousDayForMandatoryType(
            IFuelReportDomainService fuelReportDomainService,
            IGoodDomainService goodDomainService)
        {
            return;
            var fuelReportsOfYesterday = fuelReportDomainService.GetFuelReportsFromYesterday(this);

            foreach (var yesterdayFuelReport in fuelReportsOfYesterday)
            {
                var mandatoryGoods = goodDomainService.GetMandatoryVesselGoods(this.VesselInCompanyId, yesterdayFuelReport.EventDate);

                //Validate the availability of all mandatory goods for previous day fuel report.
                if (mandatoryGoods.Count(
                    mg =>
                        !yesterdayFuelReport.FuelReportDetails.
                            Select(frd => frd.Good.Id).Contains(mg.Id)) > 0                    )
                    throw new BusinessRuleException("BR_FR39", "Some mandatory fuels not found in fuel report of yesterday.");
            }
        }

        //===================================================================================

        #endregion

        //===================================================================================

        public void IsWaitingToBeClosed()
        {
            if (!isFuelReportWaitingToBeClosed.IsSatisfiedBy(this))
                throw new BusinessRuleException("FR_Close", this.State + " : Fuel Report could not accept Inventory Results.");
        }

        //===================================================================================

        public void Close(FuelReportState entityNewState)
        {
            this.CheckToBeOperational();

            IsWaitingToBeClosed();

            setEntityState(entityNewState);
        }

        //===================================================================================

        public void Submit(
            FuelReportState entityNewState,
            IVoyageDomainService voyageDomainService,
            IFuelReportDomainService fuelReportDomainService,
            IInventoryOperationDomainService inventoryOperationDomainService,
            IGoodDomainService goodDomainService,
            IOrderDomainService orderDomainService,
            ICurrencyDomainService currencyDomainService,
            IBalanceDomainService balanceDomainService,
            IInventoryManagementDomainService inventoryManagementDomainService,
            IInventoryOperationNotifier inventoryOperationNotifier)
        {
            this.CheckToBeOperational();

            validateSubmittingState(
                    voyageDomainService,
                    fuelReportDomainService,
                    inventoryOperationDomainService,
                    goodDomainService,
                    orderDomainService,
                    currencyDomainService,
                    inventoryManagementDomainService);

            sendDataToOrderDomainService(balanceDomainService);

            if (this.FuelReportType == FuelReportTypes.EndOfVoyage ||
                this.FuelReportType == FuelReportTypes.EndOfYear ||
                this.FuelReportType == FuelReportTypes.EndOfMonth)
            {
                var inventoryResult = inventoryOperationNotifier.NotifySubmittingFuelReportConsumption(this);

                this.ConsumptionInventoryOperations.Add(inventoryResult);
            }
            else
            {
                foreach (var fuelReportDetail in this.FuelReportDetails)
                {
                    var inventoryResult = inventoryOperationNotifier.NotifySubmittingFuelReportDetail(fuelReportDetail);

                    if (inventoryResult != null)
                        fuelReportDetail.InventoryOperations.AddRange(inventoryResult);
                }
                
            }

            //var notificationData = buildNotificationData(fuelReportDomainService);

            //fuelReportDomainService.SetFuelReportInventoryResults(buildFakeResult(notificationData), this);

            setEntityState(entityNewState);
        }

        //===================================================================================

        private void sendDataToOrderDomainService(IBalanceDomainService balanceDomainService)
        {
            foreach (var fuelReportDetail in FuelReportDetails)
            {
                if (fuelReportDetail.Receive.HasValue && fuelReportDetail.ReceiveReference != null && fuelReportDetail.ReceiveReference.ReferenceId.HasValue)
                    balanceDomainService.SetReceivedData(fuelReportDetail.ReceiveReference.ReferenceId.Value, fuelReportDetail.Id, fuelReportDetail.GoodId, fuelReportDetail.MeasuringUnitId, (decimal)fuelReportDetail.Receive.Value);
            }
        }

        //===================================================================================

        private FuelReportNotificationData buildNotificationData(IFuelReportDomainService fuelReportDomainService)
        {
            var notificationData = new FuelReportNotificationData();

            notificationData.FuelReportId = this.Id;

            notificationData.Items = buildNotificationDataItems(fuelReportDomainService);

            return notificationData;
        }

        //===================================================================================

        private List<FuelReportNotificationDataItem> buildNotificationDataItems(IFuelReportDomainService fuelReportDomainService)
        {
            List<FuelReportNotificationDataItem> dataItems = new List<FuelReportNotificationDataItem>();

            foreach (var detail in this.FuelReportDetails)
            {
                dataItems.Add(detail.GetNotificationDataItem(fuelReportDomainService));
            }

            return dataItems;
        }

        //===================================================================================

        private InventoryResultCommand buildFakeResult(FuelReportNotificationData notificationData)
        {

            var bag = new InventoryResultCommand
                      {
                          FuelReportId = notificationData.FuelReportId,
                          Items = new List<InventoryResultCommandItem>()
                      };

            foreach (var item in notificationData.Items)
            {
                bag.Items.AddRange(getFuelReportInventoryResultItem(item));
            }

            return bag;
        }

        //===================================================================================

        private List<InventoryResultCommandItem> getFuelReportInventoryResultItem(FuelReportNotificationDataItem dataItem)
        {
            List<InventoryResultCommandItem> items = new List<InventoryResultCommandItem>();

            if (dataItem.EndOfVoyage != null)
            {
                InventoryResultCommandItem item = new InventoryResultCommandItem();

                item.FuelReportDetailId = dataItem.FuelReportDetailId;
                item.ActionDate = DateTime.Now;
                item.ActionNumber = "INV" + DateTime.Now.ToString("yyyyMMddHHmmss");
                item.ActionType = InventoryActionType.Issue;

                items.Add(item);
            }

            if (dataItem.EndOfMonth != null)
            {
                InventoryResultCommandItem item = new InventoryResultCommandItem();

                item.FuelReportDetailId = dataItem.FuelReportDetailId;
                item.ActionDate = DateTime.Now;
                item.ActionNumber = "INV" + DateTime.Now.ToString("yyyyMMddHHmmss");
                item.ActionType = InventoryActionType.Issue;

                items.Add(item);
            }


            if (dataItem.EndOfYear != null)
            {
                InventoryResultCommandItem item = new InventoryResultCommandItem();

                item.FuelReportDetailId = dataItem.FuelReportDetailId;
                item.ActionDate = DateTime.Now;
                item.ActionNumber = "INV" + DateTime.Now.ToString("yyyyMMddHHmmss");
                item.ActionType = InventoryActionType.Issue;

                items.Add(item);
            }

            if (dataItem.NegativeCorrection != null)
            {
                InventoryResultCommandItem item = new InventoryResultCommandItem();

                item.FuelReportDetailId = dataItem.FuelReportDetailId;
                item.ActionDate = DateTime.Now;
                item.ActionNumber = "INV" + DateTime.Now.ToString("yyyyMMddHHmmss");
                item.ActionType = InventoryActionType.Issue;

                items.Add(item);
            }


            if (dataItem.Transfer != null)
            {
                InventoryResultCommandItem item = new InventoryResultCommandItem();

                item.FuelReportDetailId = dataItem.FuelReportDetailId;
                item.ActionDate = DateTime.Now;
                item.ActionNumber = "INV" + DateTime.Now.ToString("yyyyMMddHHmmss");
                item.ActionType = InventoryActionType.Issue;

                items.Add(item);
            }


            if (dataItem.Receive != null)
            {
                InventoryResultCommandItem item = new InventoryResultCommandItem();

                item.FuelReportDetailId = dataItem.FuelReportDetailId;
                item.ActionDate = DateTime.Now;
                item.ActionNumber = "INV" + DateTime.Now.ToString("yyyyMMddHHmmss");
                item.ActionType = InventoryActionType.Receipt;

                items.Add(item);
            }


            if (dataItem.NegativeCorrection != null)
            {
                InventoryResultCommandItem item = new InventoryResultCommandItem();

                item.FuelReportDetailId = dataItem.FuelReportDetailId;
                item.ActionDate = DateTime.Now;
                item.ActionNumber = "INV" + DateTime.Now.ToString("yyyyMMddHHmmss");
                item.ActionType = InventoryActionType.Receipt;

                items.Add(item);
            }

            return items;
        }

        //===================================================================================

        public void Invalidate(FuelReportState entityNewState)
        {
            setEntityState(entityNewState);
        }

        //===================================================================================

        public void CheckToBeOperational()
        {
            if (!IsActive())
                throw new InvalidOperation("CheckToBeOperational", "The Fuel Report is not Operational.");
        }

        //===================================================================================

        public void CheckToBeNotCancelled()
        {
            if (!IsFuelReportNotCancelled())
                throw new InvalidOperation("CheckToNotBeCancelled", "The Fuel Report is cancelled.");
        }

        //===================================================================================

        public bool IsActive()
        {
            return isFuelReportOperational.IsSatisfiedBy(this);
        }

        //===================================================================================

        public bool IsFuelReportNotCancelled()
        {
            return isFuelReportNotCancelled.IsSatisfiedBy(this);
        }

        //===================================================================================

        private void setEntityState(FuelReportState entityNewState)
        {
            this.EntityState = entityNewState;

            this.State = entityNewState.State;
        }

        //===================================================================================

        internal void Configure(FuelReportAggregate.Factories.IFuelReportStateFactory fuelReportStateFactory)
        {
            EntityState = fuelReportStateFactory.CreatState(this.State);

            foreach (var fuelReportDetail in FuelReportDetails)
            {
                fuelReportDetail.ROBQuantity = new Quantity((decimal)fuelReportDetail.ROB, new UnitOfMeasure(fuelReportDetail.ROBUOM));

                if (fuelReportDetail.CorrectionPrice.HasValue && !string.IsNullOrWhiteSpace(fuelReportDetail.CorrectionPriceCurrencyISOCode))
                    fuelReportDetail.CorrectionPriceMoney = new Money(fuelReportDetail.CorrectionPrice.Value, new CurrencyAndMeasurement.Domain.Contracts.Currency(fuelReportDetail.CorrectionPriceCurrencyISOCode));
            }
        }

        //===================================================================================

        public void UpdateEventDate(DateTime eventDateTime)
        {
            CheckToBeOperational();

            validateToBeSubmitRejected();

            validateToBeEndOfVoyageFuelReport();

            this.EventDate = eventDateTime;
        }

        //===================================================================================

        private void validateToBeEndOfVoyageFuelReport()
        {
            if (this.FuelReportType != FuelReportTypes.EndOfVoyage)
                throw new BusinessRuleException("", "Fuel Report is not End of Voyage Fuel Report.");
        }

        //===================================================================================

        private void validateToBeSubmitRejected()
        {
            if (!this.isFuelReportSubmitRejected.IsSatisfiedBy(this))
                throw new BusinessRuleException("", "The Fuel Report is not submit rejected.");
        }

        //===================================================================================

        private void validateToBeInSubmittedState()
        {
            if (!this.isFuelReportSubmitted.IsSatisfiedBy(this))
                throw new BusinessRuleException("", "The Fuel Report is not submit rejected.");
        }

        //===================================================================================

        public void SubmittedReject(FuelReportState entityNewState)
        {
            //validateToBeEndOfVoyageFuelReport();

            CheckToBeOperational();

            validateToBeInSubmittedState();

            setEntityState(entityNewState);
        }

        //===================================================================================

        public void UpdateConsumption(IFuelReportDomainService fuelReportDomainService)
        {
            //validatePreviousFuelReportsToBeFinalApproved(fuelReportDomainService);

            var previousFuelReports = fuelReportDomainService.GetFuelReportsFromYesterday(this);

            var previousFuelReport = previousFuelReports.LastOrDefault();

            if (previousFuelReport != null)
            {
                foreach (var fuelReportDetail in this.FuelReportDetails)
                {
                    var relevantFeulReportDetailOfPreviousDay = previousFuelReport.FuelReportDetails.FirstOrDefault(frd => frd.GoodId == fuelReportDetail.GoodId);

                    if (relevantFeulReportDetailOfPreviousDay != null)
                    {
                        fuelReportDetail.UpdateConsumption(relevantFeulReportDetailOfPreviousDay);
                    }
                }
            }
        }

        //===================================================================================

    }
}