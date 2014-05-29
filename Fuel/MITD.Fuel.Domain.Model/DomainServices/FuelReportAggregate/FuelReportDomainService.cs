using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Repositories;
using MITD.Fuel.Domain.Model.Specifications;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Factories;
using MITD.Domain.Model;

namespace MITD.Fuel.Domain.Model.DomainServices
{
    public class FuelReportDomainService : IFuelReportDomainService
    {
        private readonly IFuelReportRepository fuelReportRepository;
        private readonly IOrderDomainService orderDomainService;
        private readonly IInventoryManagementDomainService inventoryManagementDomainService;
        private readonly ICharteringDomainService charteringDomainService;
        private readonly IVoyageDomainService voyageDomainService;
        private readonly IInventoryOperationDomainService inventoryOperationDomainService;
        private readonly IInventoryOperationRepository inventoryOperationRepository;
        private readonly IInventoryOperationFactory inventoryOperationFactory;

        private readonly IsFuelReportIssued isFuelReportIssued;
        private readonly IsFuelReportClosed isFuelReportInFinalApprovedState;
        private readonly IsFuelReportOperational isFuelReportOperational;
        private readonly IsFuelReportNotCancelled isFuelReportNotCancelled;
        private readonly IsFuelReportSubmitted isFuelReportSubmitted;

        public FuelReportDomainService(
            IFuelReportRepository fuelReportRepository,
            IVoyageDomainService voyageDomainService,
            IInventoryOperationDomainService inventoryOperationDomainService,
            IInventoryOperationRepository inventoryOperationRepository,
            IInventoryOperationFactory inventoryOperationFactory,
            IOrderDomainService orderDomainService,
            IInventoryManagementDomainService inventoryManagementDomainService,
            ICharteringDomainService charteringDomainService)
        {
            this.fuelReportRepository = fuelReportRepository;
            this.voyageDomainService = voyageDomainService;
            this.inventoryOperationDomainService = inventoryOperationDomainService;
            this.inventoryOperationRepository = inventoryOperationRepository;
            this.inventoryOperationFactory = inventoryOperationFactory;
            this.orderDomainService = orderDomainService;
            this.inventoryManagementDomainService = inventoryManagementDomainService;
            this.charteringDomainService = charteringDomainService;
            this.isFuelReportNotCancelled = new IsFuelReportNotCancelled();
            this.isFuelReportIssued = new IsFuelReportIssued(inventoryOperationDomainService);
            this.isFuelReportInFinalApprovedState = new IsFuelReportClosed();
            this.isFuelReportOperational = new IsFuelReportOperational();
            isFuelReportSubmitted = new IsFuelReportSubmitted();
        }

        //================================================================================

        public FuelReport Get(long id)
        {
            var fetchStrategy = new SingleResultFetchStrategy<FuelReport>();
            fetchStrategy.Include(f => f.Voyage).Include(f => f.FuelReportDetails);

            return fuelReportRepository.First(
                Extensions.And(this.isFuelReportNotCancelled.Predicate, fr => fr.Id == id),
                fetchStrategy);

        }

        //================================================================================

        public List<FuelReport> GetAll()
        {
            return fuelReportRepository.Find(this.isFuelReportNotCancelled.Predicate).ToList();
        }

        //================================================================================

        public PageResult<FuelReport> GetByFilter(long companyId, long vesselInCompanyId, int pageSize, int pageIndex)
        {
            var fetchStrategy = new ListFetchStrategy<FuelReport>(nolock: true)
                .WithPaging(pageSize, pageIndex + 1)
                .Include(c => c.FuelReportDetails)
                .OrderBy(c => c.EventDate);

            var data = this.fuelReportRepository.Find(
                                                f =>
                                                    f.VesselInCompanyId == vesselInCompanyId &&
                                                    f.VesselInCompany.CompanyId == companyId
                                    , fetchStrategy);

            return fetchStrategy.PageCriteria.PageResult;
        }

        //================================================================================

        public List<FuelReport> GetPreviousNotFinalApprovedReports(FuelReport fuelReport)
        {
            var fetchStrategy = new ListFetchStrategy<FuelReport>().OrderBy(c => c.EventDate);

            var result = this.fuelReportRepository.Find(
                                    Extensions.And(this.isFuelReportInFinalApprovedState.Not().Predicate, Extensions.And(this.isFuelReportOperational.Predicate, f => f.EventDate < fuelReport.EventDate)));

            return result.ToList();
        }

        //================================================================================

        public List<FuelReport> GetVoyagesValidEndOfVoyageFuelReports(List<Voyage> voyages)
        {
            var result = this.fuelReportRepository
                .Find(Extensions.And(this.isFuelReportNotCancelled.Predicate,
                    fr => fr.FuelReportType == Enums.FuelReportTypes.EndOfVoyage))
                .Join(voyages, report => report.VoyageId, v => v.Id, (fr, v) => fr);
            //.Where(fr => voyages.Any(v => v.Id == fr.VoyageId));

            return result.ToList();
        }

        //================================================================================

        public FuelReport GetVoyageValidEndOfVoyageFuelReport(long voyageId)
        {
            var endOfVoyageFuelReport = this.fuelReportRepository.First(
                Extensions.And(this.isFuelReportNotCancelled.Predicate, fr => fr.FuelReportType == Enums.FuelReportTypes.EndOfVoyage &&
                        voyageId == fr.VoyageId));

            if (endOfVoyageFuelReport == null)
                throw new ObjectNotFound("Voyage EOV FuelReport", voyageId);

            return endOfVoyageFuelReport;
        }

        //================================================================================

        public FuelReport GetVoyageValidEndOfVoyageFuelReport(Voyage voyage)
        {
            if (voyage == null) return null;

            return GetVoyageValidEndOfVoyageFuelReport(voyage.Id);
        }

        //================================================================================

        public List<FuelReport> GetVesselFuelReports(VesselInCompany vesselInCompany, DateTime? startDateTime, DateTime? endDateTime, bool ignoreTime = false)
        {
            var lisFetchStrategy = new ListFetchStrategy<FuelReport>(nolock: true);


            var startDateToSearch = DateTime.MinValue;
            if (startDateTime.HasValue)
            {
                if (ignoreTime)
                {
                    startDateToSearch = startDateTime.Value.Date;
                }
                else
                {
                    startDateToSearch = startDateTime.Value;
                }
            }


            var endDateToSearch = DateTime.MaxValue;
            if (endDateTime.HasValue)
            {
                if (ignoreTime)
                {
                    endDateToSearch = endDateTime.Value.Date.AddDays(1).AddMilliseconds(-1);
                }
                else
                {
                    endDateToSearch = endDateTime.Value;
                }
            }

            var result = this.fuelReportRepository.Find(
                Extensions.And(this.isFuelReportNotCancelled.Predicate, fr =>
                    fr.VesselInCompanyId == vesselInCompany.Id &&
                    fr.EventDate >= startDateToSearch &&
                    fr.EventDate <= endDateToSearch));

            return result.ToList();
        }

        //================================================================================

        public List<Reference> GetFuelReportDetailTransferReferences(FuelReportDetail fuelReportDetail)
        {
            List<Reference> result = null;

            //if (fuelReportDetail.TransferType.HasValue)
            //{
            //    switch (fuelReportDetail.TransferType.Value)
            //    {
            //        case TransferTypes.TransferSale:

            //            var finalApprovedTransferOrder = orderDomainService.GetFinalApprovedTransferOrders(fuelReportDetail.FuelReport.Vessel.CompanyId);

            //            result = new List<Reference>(
            //                finalApprovedTransferOrder.Select(
            //                    o => new Reference
            //                        {
            //                            ReferenceType = ReferenceType.Order,
            //                            ReferenceId = o.Id,
            //                            Code = o.Code
            //                        }));
            //            break;

            //        case TransferTypes.InternalTransfer:

            //            var finalApprovedInternalTransferOrder = orderDomainService.GetFinalApprovedInternalTransferOrders(fuelReportDetail.FuelReport.Vessel.CompanyId);

            //            result = new List<Reference>(
            //                finalApprovedInternalTransferOrder.Select(
            //                o => new Reference
            //                    {
            //                        ReferenceType = ReferenceType.Order,
            //                        ReferenceId = o.Id,
            //                        Code = o.Code
            //                    }));

            //            break;
            //    }
            //}


            result = new List<Reference>();

            var finalApprovedTransferOrder = orderDomainService.GetFinalApprovedTransferOrders(fuelReportDetail.FuelReport.VesselInCompany.CompanyId);

            result.AddRange(
                finalApprovedTransferOrder.Select(
                    o => new Reference
                    {
                        ReferenceType = ReferenceType.Order,
                        ReferenceId = o.Id,
                        Code = o.Code
                    }));

            var finalApprovedInternalTransferOrder = orderDomainService.GetFinalApprovedInternalTransferOrders(fuelReportDetail.FuelReport.VesselInCompany.CompanyId);

            result.AddRange(
                finalApprovedInternalTransferOrder.Select(
                o => new Reference
                {
                    ReferenceType = ReferenceType.Order,
                    ReferenceId = o.Id,
                    Code = o.Code
                }));

            return result;
        }

        //================================================================================

        public List<Reference> GetFuelReportDetailReceiveReferences(FuelReportDetail fuelReportDetail)
        {
            List<Reference> result = null;

            //if (fuelReportDetail.ReceiveType.HasValue)
            //{
            //    switch (fuelReportDetail.ReceiveType.Value)
            //    {
            //        case ReceiveTypes.Purchase:

            //            var finalApprovedPrurchaseOrders = orderDomainService.GetFinalApprovedPrurchaseOrders(fuelReportDetail.FuelReport.Vessel.CompanyId);

            //            result = new List<Reference>(
            //                finalApprovedPrurchaseOrders.Select(
            //                o => new Reference
            //                {
            //                    ReferenceType = ReferenceType.Order,
            //                    ReferenceId = o.Id,
            //                    Code = o.Code
            //                }));

            //            break;

            //        case ReceiveTypes.TransferPurchase:

            //            var finalApprovedPrurchaseTransferOrders = orderDomainService.GetBuyerFinalApprovedPurchaseTransferOrders(fuelReportDetail.FuelReport.Vessel.CompanyId);

            //            result = new List<Reference>(
            //               finalApprovedPrurchaseTransferOrders.Select(
            //               o => new Reference
            //               {
            //                   ReferenceType = ReferenceType.Order,
            //                   ReferenceId = o.Id,
            //                   Code = o.Code
            //               }));

            //            break;

            //        case ReceiveTypes.InternalTransfer:

            //            var finalApprovedInternalTransferOrders = orderDomainService.GetFinalApprovedInternalTransferOrders(fuelReportDetail.FuelReport.Vessel.CompanyId);

            //            result = new List<Reference>(
            //               finalApprovedInternalTransferOrders.Select(
            //               o => new Reference
            //               {
            //                   ReferenceType = ReferenceType.Order,
            //                   ReferenceId = o.Id,
            //                   Code = o.Code
            //               }));

            //            break;
            //    }
            //}


            result = new List<Reference>();

            var finalApprovedPrurchaseOrders = orderDomainService.GetFinalApprovedPrurchaseOrders(fuelReportDetail.FuelReport.VesselInCompany.CompanyId);

            result.AddRange(
                finalApprovedPrurchaseOrders.Select(
                o => new Reference
                {
                    ReferenceType = ReferenceType.Order,
                    ReferenceId = o.Id,
                    Code = o.Code
                }));

            var finalApprovedPrurchaseTransferOrders = orderDomainService.GetBuyerFinalApprovedPurchaseTransferOrders(fuelReportDetail.FuelReport.VesselInCompany.CompanyId);

            result.AddRange(
               finalApprovedPrurchaseTransferOrders.Select(
               o => new Reference
               {
                   ReferenceType = ReferenceType.Order,
                   ReferenceId = o.Id,
                   Code = o.Code
               }));

            var finalApprovedInternalTransferOrders = orderDomainService.GetFinalApprovedInternalTransferOrders(fuelReportDetail.FuelReport.VesselInCompany.CompanyId);

            result.AddRange(
               finalApprovedInternalTransferOrders.Select(
               o => new Reference
               {
                   ReferenceType = ReferenceType.Order,
                   ReferenceId = o.Id,
                   Code = o.Code
               }));

            return result;
        }

        //================================================================================

        public List<Reference> GetFuelReportDetailCorrectionReferences(FuelReportDetail fuelReportDetail)
        {
            List<Reference> result = null;

            var lastIssuedEOVFuelReportsOfPreviousVoyages = GetLastIssuedEOVFuelReportOfPreviousVoyages(fuelReportDetail.FuelReport);

            if (lastIssuedEOVFuelReportsOfPreviousVoyages != null)
            {
                result = new List<Reference>(){
                    new Reference(){
                        ReferenceType= ReferenceType.Voyage,
                        ReferenceId = lastIssuedEOVFuelReportsOfPreviousVoyages.Voyage.Id,
                        Code = lastIssuedEOVFuelReportsOfPreviousVoyages.Voyage.VoyageNumber
                    }};
            }

            return result;
        }

        //================================================================================

        public FuelReport GetLastIssuedEOVFuelReportOfPreviousVoyages(FuelReport fuelReport)
        {
            var previousEOVFuelReports = getPreviousEOVFuelReports(fuelReport.EventDate);

            var lastIssuedEOVFuelReportOfPreviousVoyages =
                previousEOVFuelReports
                    .FindAll(eovfr => isFuelReportIssued.IsSatisfiedBy(eovfr))
                    .OrderBy(fr => fr.EventDate).FirstOrDefault();

            return lastIssuedEOVFuelReportOfPreviousVoyages;
        }

        //================================================================================

        public List<FuelReport> GetNotIssuedEOVFuelReportsOfPreviousVoyages(FuelReport fuelReport)
        {
            var previousEOVFuelReports = getPreviousEOVFuelReports(fuelReport.EventDate);

            var notIssuedEOVFuelReportsOfPreviousVoyages =
                previousEOVFuelReports
                    .FindAll(eovfr => !isFuelReportIssued.IsSatisfiedBy(eovfr));

            return notIssuedEOVFuelReportsOfPreviousVoyages;
        }

        //================================================================================

        private List<FuelReport> getPreviousEOVFuelReports(DateTime comparingDate)
        {
            var voyagesEndedBefore = voyageDomainService.GetVoyagesEndedBefore(comparingDate);

            var endOfVoyageFuelReportsOfPreviousVoyages = this.GetVoyagesValidEndOfVoyageFuelReports(voyagesEndedBefore);

            return endOfVoyageFuelReportsOfPreviousVoyages;
        }

        //================================================================================

        public List<FuelReport> GetFuelReportsFromYesterday(FuelReport fuelReport)
        {
            DateTime previousDayDate = fuelReport.EventDate.Date.AddDays(-1);

            return GetVesselFuelReports(fuelReport.VesselInCompany, previousDayDate, fuelReport.EventDate.AddMilliseconds(-1));
        }

        //================================================================================

        public FuelReport GetLastIssuedFuelReportBefore(FuelReport fuelReport)
        {
            var previousFuelReports = this.fuelReportRepository.Find(
                fr =>
                    (fr.FuelReportType == Enums.FuelReportTypes.EndOfVoyage ||
                    fr.FuelReportType == Enums.FuelReportTypes.EndOfMonth ||
                    fr.FuelReportType == Enums.FuelReportTypes.EndOfYear) &&
                    fr.EventDate < fuelReport.EventDate
                ).ToList();

            var lastIssuedFuelReport =
                    previousFuelReports
                        .FindAll(fr => isFuelReportIssued.IsSatisfiedBy(fr))
                        .OrderBy(fr => fr.EventDate).FirstOrDefault();

            return lastIssuedFuelReport;
        }

        //================================================================================

        public void SetFuelReportInventoryResults(InventoryResultCommand resultCommand, FuelReport fuelReport)
        {
            foreach (var item in resultCommand.Items)
            {
                var fuelReportDetail = fuelReport.FuelReportDetails.FirstOrDefault(frd => frd.Id == item.FuelReportDetailId);
                if (fuelReportDetail == null)
                {
                    throw new ObjectNotFound("resultBag.Item.DetailId");
                }

                var inventoryOperationToPersist = inventoryOperationFactory.Create(fuelReportDetail, item.ActionNumber, item.ActionType, item.ActionDate);

                inventoryOperationRepository.Add(inventoryOperationToPersist);

                //if (fuelReportDetail.TransferReferenceOrderId.HasValue)
                //{
                //     orderDomainService.CloseOrder(fuelReportDetail.TransferReferenceOrderId.Value);
                //}

                //if (fuelReportDetail.ReceiveReferenceOrderId.HasValue)
                //{
                //      orderDomainService.CloseOrder(fuelReportDetail.ReceiveReferenceOrderId.Value);
                //}
            }

            //IFuelReportStateFactory stateFactory = ServiceLocator.Current.GetInstance<IFuelReportStateFactory>();

            //fuelReport.Close(stateFactory.CreateClosedState());
        }

        //================================================================================

        //Commented due to be unusable
        //public void InvalidateAllPreviousEndOfVoyageFuelReports(FuelReport fuelReport)
        //{
        //    if (fuelReport.FuelReportType != FuelReportTypes.EndOfVoyage)
        //        throw new ArgumentException("FuelReport is not of Type EndOfVoyage.");

        //    var previousFuelReports = this.fuelReportRepository.Find(
        //                                    Extensions.And(this.isFuelReportOperational.Predicate, fr =>
        //                                            fr.FuelReportType == Enums.FuelReportTypes.EndOfVoyage &&
        //                                            fr.ReportDate < fuelReport.ReportDate &&
        //                                            fr.VoyageId == fuelReport.VoyageId)
        //        );

        //    IFuelReportStateFactory stateFactory = ServiceLocator.Current.GetInstance<IFuelReportStateFactory>();

        //    foreach (var previousFuelReport in previousFuelReports)
        //    {
        //        previousFuelReport.Invalidate(stateFactory.CreateInvalidState());
        //    }
        //}

        //================================================================================

        public List<FuelReport> GetVoyageAllEndOfVoyageFuelReport(Voyage voyage)
        {
            return this.fuelReportRepository.Find(
                fr =>
                    fr.FuelReportType == Enums.FuelReportTypes.EndOfVoyage &&
                    voyage.Id == fr.VoyageId
                ).ToList();
        }

        //================================================================================

        public string[] GetVoyageConsumptionIssueNumber(long voyageId)
        {
            var endOfVoyageFuelReport = this.GetVoyageValidEndOfVoyageFuelReport(voyageId);

            var endOfVoyageInventoryOperations = inventoryOperationDomainService.GetFuelReportInventoryOperations(endOfVoyageFuelReport);

            var firstInventoryOperation = endOfVoyageInventoryOperations.FirstOrDefault();

            if (firstInventoryOperation == null)
                throw new ObjectNotFound("VoyageConsumptionIssueNumber");

            return new string[] { firstInventoryOperation.ActionNumber };
        }

        //================================================================================

        public ChangingFuelReportDateData GetChangingFuelReportDateData(long fuelReportId, DateTime newDateTime)
        {
            var data = new ChangingFuelReportDateData();

            var changingFuelReport = this.Get(fuelReportId);
            data.ChangingFuelReport = changingFuelReport;

            var ascendingFetchStrategy = new SingleResultFetchStrategy<FuelReport>().OrderBy(fr => fr.EventDate);

            var nextFuelReportBeforeChangingDate = fuelReportRepository.First(
                        Extensions.And(isFuelReportNotCancelled.Predicate,
                        fr => fr.VesselInCompanyId == changingFuelReport.VesselInCompanyId &&
                            fr.EventDate > changingFuelReport.EventDate),
                        ascendingFetchStrategy);

            if (nextFuelReportBeforeChangingDate != null)
                data.NextFuelReportBeforeChangeDate = this.Get(nextFuelReportBeforeChangingDate.Id);

            var nextFuelReportAfterChangingDate = fuelReportRepository.First(
                        Extensions.And(isFuelReportNotCancelled.Predicate,
                        fr => fr.VesselInCompanyId == changingFuelReport.VesselInCompanyId &&
                             fr.EventDate > newDateTime),
                        ascendingFetchStrategy);

            if (nextFuelReportAfterChangingDate != null)
                if ((nextFuelReportBeforeChangingDate == null) || (nextFuelReportAfterChangingDate.Id != nextFuelReportBeforeChangingDate.Id))
                    data.NextFuelReportAfterChangeDate = this.Get(nextFuelReportAfterChangingDate.Id);

            return data;
        }

        //================================================================================

        public List<Reference> GetFuelReportDetailRejectedTransferReferences(FuelReportDetail fuelReportDetail)
        {
            return inventoryManagementDomainService.GetVesselPurchaseReceiptNumbers(fuelReportDetail.FuelReport.VesselInCompany.CompanyId, fuelReportDetail.FuelReport.VesselInCompanyId);
        }

        //================================================================================

        public decimal CalculateReportingConsumption(FuelReportDetail fuelReportDetail)
        {
            var processingFuelReport = fuelReportDetail.FuelReport;

            if (!(processingFuelReport.FuelReportType == FuelReportTypes.EndOfMonth ||
                processingFuelReport.FuelReportType == FuelReportTypes.EndOfVoyage ||
                processingFuelReport.FuelReportType == FuelReportTypes.EndOfYear))
            {
                throw new BusinessRuleException("", "FuelReport is not of type EOV, EOM or EOY.");
            }


            var startDateOfVesselActivation = DateTime.MinValue;

            switch (processingFuelReport.VesselInCompany.VesselStateCode)
            {
                case VesselStates.Inactive:
                case VesselStates.CharterOut:
                case VesselStates.Scrapped:
                    throw new BusinessRuleException("", "The vessel is in an incorrect state.");

                case VesselStates.CharterIn:

                    var charterInStart = charteringDomainService.GetCharterInStart(processingFuelReport.VesselInCompany.Company, processingFuelReport.VesselInCompany, processingFuelReport.EventDate);

                    if (charterInStart == null)
                        throw new BusinessRuleException("", "The proper Charter-In Start record not found.");

                    startDateOfVesselActivation = charterInStart.ActionDate;

                    break;

                case VesselStates.Owned:

                    var charterOutEnd = charteringDomainService.GetCharterOutEnd(processingFuelReport.VesselInCompany.Company, processingFuelReport.VesselInCompany, processingFuelReport.EventDate);

                    if (charterOutEnd != null)
                        startDateOfVesselActivation = charterOutEnd.ActionDate;

                    break;

                default:
                    throw new InvalidArgument("VesselStateCode is invalid.", "VesselStateCode");
            }

            var singleFetchStrategy = new SingleResultFetchStrategy<FuelReport>(nolock: true).OrderByDescending(fr => fr.EventDate);

            var lastConsumptionIssuingFuelReport = fuelReportRepository.First(
                    fr =>
                        fr.EventDate >= startDateOfVesselActivation &&
                        fr.EventDate < processingFuelReport.EventDate &&
                        fr.VesselInCompany.Id == processingFuelReport.VesselInCompanyId &&
                            (fr.FuelReportType == FuelReportTypes.EndOfMonth ||
                            fr.FuelReportType == FuelReportTypes.EndOfMonth ||
                            fr.FuelReportType == FuelReportTypes.EndOfYear),
                    singleFetchStrategy);

            if (lastConsumptionIssuingFuelReport != null && !isFuelReportIssued.IsSatisfiedBy(lastConsumptionIssuingFuelReport))
                throw new BusinessRuleException("", "Previous EOV, EOM or EOY FuelReport is not issued yet.");

            var startDateToCalculateConsumption = (lastConsumptionIssuingFuelReport != null) 
                ? lastConsumptionIssuingFuelReport.EventDate 
                : startDateOfVesselActivation;

            Expression<Func<FuelReport , bool>> queryToFindPreviousFuelReportsForCalculation = 
                fr =>
                    fr.VesselInCompanyId == processingFuelReport.VesselInCompanyId &&
                    fr.EventDate > startDateToCalculateConsumption &&
                    fr.EventDate < processingFuelReport.EventDate;


            var notSubmittedFuelReportsCount = fuelReportRepository.Count(
                isFuelReportSubmitted.Not().Predicate.And(queryToFindPreviousFuelReportsForCalculation));

            if(notSubmittedFuelReportsCount != 0)
                throw new BusinessRuleException("","There are some not approved FuelReports.");

            var calculatedPreviousConsumptions = 
                fuelReportRepository.Find(queryToFindPreviousFuelReportsForCalculation)
                    .SelectMany(fr => fr.FuelReportDetails.Where(frd => frd.GoodId == fuelReportDetail.GoodId))
                    .Sum(frd => frd.Consumption);

            return new decimal(calculatedPreviousConsumptions + fuelReportDetail.Consumption);
        }

        //================================================================================
    }
}

