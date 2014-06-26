using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MITD.Fuel.ACL.Contracts.AutomaticVoucher;
using MITD.Fuel.ACL.StorageSpace.DomainServices.Events.Inventory.DTOs;
using MITD.Fuel.ACL.StorageSpace.Mappers.Inventory.Contracts;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;
using MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate;
using MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.IDomainServices.Events.InventoryOperations;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Core;
using MITD.Fuel.Integration.Inventory;
using ReferenceType = MITD.Fuel.Domain.Model.Enums.ReferenceType;

namespace MITD.Fuel.ACL.StorageSpace.DomainServices.Events
{
    public class InventoryOperationNotifier : IInventoryOperationNotifier
    {
        private readonly IFuelReportFuelReportDtoMapper fuelReportDtoMapper;
        private readonly IFuelReportDetailToFuelReportDetailDtoMapper fuelReportDetailDtoMapper;
        private readonly IInvoiceToDtoMapper invoiceToDtoMapper;
        private readonly IAddCharterInStartReceiptVoucher _addCharterInStartReceiptVoucher;
        private readonly IInventoryOperationManager inventoryOperationManager;

        public InventoryOperationNotifier(IAddCharterInStartReceiptVoucher addCharterInStartReceiptVoucher
            //IFuelReportFuelReportDtoMapper fuelReportDtoMapper,
            //IInvoiceToDtoMapper invoiceToDtoMapper,
            //IFuelReportDetailToFuelReportDetailDtoMapper fuelReportDetailDtoMapper
            )
        {
            this.inventoryOperationManager = new InventoryOperationManager();
            _addCharterInStartReceiptVoucher = addCharterInStartReceiptVoucher;
            //this.fuelReportDtoMapper = fuelReportDtoMapper;
            //this.invoiceToDtoMapper = invoiceToDtoMapper;
            //this.fuelReportDetailDtoMapper = fuelReportDetailDtoMapper;
        }

        public InventoryOperation NotifySubmittingFuelReportConsumption(FuelReport fuelReport)
        {
            try
            {
                return this.inventoryOperationManager.ManageFuelReportConsumption(fuelReport,
                    //TODO: Fake ActorId
                    1101);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<InventoryOperation> NotifySubmittingFuelReportDetail(FuelReportDetail source, IFuelReportDomainService fuelReportDomainService)
        {
            var result = new List<InventoryOperation>();

            try
            {
                if (source.Correction.HasValue && source.CorrectionType.HasValue)
                {
                    if (source.CorrectionType.Value == CorrectionTypes.Plus)
                    {
                        if (source.CorrectionReference.IsEmpty())
                        {
                            if (source.IsCorrectionPriceEmpty())
                            {
                                var lastReceiveFuelReportDetailBefore = fuelReportDomainService.GetLastReceiveFuelReportDetailBefore(source);

                                var lastReceiveInventoryOperationId = inventoryOperationManager.GetFueReportDetailReceiveOperationReference(lastReceiveFuelReportDetailBefore).OperationId;

                                result.AddRange(this.inventoryOperationManager.ManageFuelReportDetailIncrementalCorrectionUsingReferencePricing(source, lastReceiveInventoryOperationId, "PricingByLastReceipt",
                                    //TODO: Fake ActorId
                                    1101));
                            }
                            else
                            {
                                result.AddRange(this.inventoryOperationManager.ManageFuelReportDetailIncrementalCorrectionDirectPricing(source,
                                    //TODO: Fake ActorId
                                    1101));
                            }
                        }
                        else
                        {
                            if (source.CorrectionReference.ReferenceType.Value == ReferenceType.Voyage)
                            {
                                //var eovFuelReport = fuelReportDomainService.GetVoyageValidEndOfVoyageFuelReport(source.CorrectionReference.ReferenceId.Value);

                                //var consumptionInventoryOperationId = eovFuelReport.ConsumptionInventoryOperations.Last().InventoryOperationId;

                                var consumptionInventoryOperationId = fuelReportDomainService.GetVoyageConsumptionIssueOperation(source.CorrectionReference.ReferenceId.Value).InventoryOperationId;

                                result.AddRange(this.inventoryOperationManager.ManageFuelReportDetailIncrementalCorrectionUsingReferencePricing(source, consumptionInventoryOperationId, "PricingByIssuedVoyage",
                                    //TODO: Fake ActorId
                                    1101));
                            }
                        }
                    }
                    else
                    {
                        result.AddRange(this.inventoryOperationManager.ManageFuelReportDetailDecrementalCorrection(source,
                            //TODO: Fake ActorId
                                             1101));
                    }
                }

                result.AddRange(this.inventoryOperationManager.ManageFuelReportDetail(source,
                    //TODO: Fake ActorId
                    1101));
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        private void setConsumption(FuelReportDetailDto dto, FuelReportDetail source)
        {
            if (!(source.FuelReport.FuelReportType == FuelReportTypes.EndOfMonth ||
                source.FuelReport.FuelReportType == FuelReportTypes.EndOfVoyage ||
                source.FuelReport.FuelReportType == FuelReportTypes.EndOfYear))
            {
                dto.Consumption = 0;
            }
            else
            {
                var fuelReportDomainService = ServiceLocator.Current.GetInstance<IFuelReportDomainService>();

                var reportingConsumption = fuelReportDomainService.CalculateReportingConsumption(source);

                dto.Consumption = (double?)reportingConsumption;
            }
        }

        public List<InventoryOperation> NotifySubmittingScrap(Scrap source)
        {
            return new List<InventoryOperation>(new InventoryOperation[]
                    {
                     new InventoryOperation(
                         312,
                     "INV# - " +DateTime.Now.Ticks,
                        DateTime.Now,
                        InventoryActionType.Issue,
                        (long? )null,
                        (long? )null)});
        }

        public List<InventoryOperation> NotifySubmittingInvoice(Invoice source)
        {
            var invoiceDto = invoiceToDtoMapper.MapToModel(source);

            return new List<InventoryOperation>(new InventoryOperation[]
                    {
                     new InventoryOperation(
                         312,
                     "INV# - " +DateTime.Now.Ticks,
                        DateTime.Now,
                        InventoryActionType.Issue,
                        (long? )null,
                        (long? )null)});
        }

        public InventoryOperation NotifySubmittingOrderItemBalance(OrderItemBalance orderItemBalance)
        {
            try
            {
                return this.inventoryOperationManager.ManageOrderItemBalance(orderItemBalance,
                    //TODO: Fake ActorId
                    1101);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<InventoryOperation> NotifySubmittingCharterInStart(CharterIn charterInStart)
        {
            try
            {
                //var res= this.inventoryOperationManager.ManageCharterInStart(charterInStart,
                //    //TODO: Fake ActorId
                //    1101);

                _addCharterInStartReceiptVoucher.Execute(charterInStart,new List<Receipt>()
                                                                        {
                                                                          new Receipt(1,40,2500,1.5m)
                                                                        },"001","050");


                return null;
            }
            catch (Exception)
            {

                throw;
            }


            return new List<InventoryOperation>(new InventoryOperation[]
                    {
                     new InventoryOperation(
                         312,
                     "INV# - " +DateTime.Now.Ticks,
                        DateTime.Now,
                        InventoryActionType.Issue,
                        (long? )null,
                        (long? )null)});
        }

        public List<InventoryOperation> NotifySubmittingCharterInEnd(CharterIn charterInEnd)
        {
            return new List<InventoryOperation>(new InventoryOperation[]
                    {
                     new InventoryOperation(
                         312,
                     "INV# - " +DateTime.Now.Ticks,
                        DateTime.Now,
                        InventoryActionType.Issue,
                        (long? )null,
                        (long? )null)});
        }

        public List<InventoryOperation> NotifySubmittingCharterOutStart(CharterOut charterOutStart)
        {
            try
            {
                return new List<InventoryOperation>()
                       {
                           this.inventoryOperationManager.ManageCharterOutStart(charterOutStart,
                                        //TODO: Fake ActorId
                                        1101)
                       };
            }
            catch
            {
                throw;
            }
        }

        public List<InventoryOperation> NotifySubmittingCharterOutEnd(CharterOut charterOutEnd)
        {
            return new List<InventoryOperation>(new InventoryOperation[]
                    {
                     new InventoryOperation(
                         312,
                     "INV# - " +DateTime.Now.Ticks,
                        DateTime.Now,
                        InventoryActionType.Issue,
                        (long? )null,
                        (long? )null)});
        }
    }


}
