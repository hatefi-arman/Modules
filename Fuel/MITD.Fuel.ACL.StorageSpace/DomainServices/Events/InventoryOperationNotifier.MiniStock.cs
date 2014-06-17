using System;
using System.Collections.Generic;
using System.Diagnostics;
using MITD.Fuel.ACL.StorageSpace.DomainServices.Events.Inventory.DTOs;
using MITD.Fuel.ACL.StorageSpace.Mappers.Inventory.Contracts;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;
using MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.IDomainServices.Events.InventoryOperations;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Core;
using MITD.Fuel.Integration.Inventory;

namespace MITD.Fuel.ACL.StorageSpace.DomainServices.Events
{
    public class InventoryOperationNotifier : IInventoryOperationNotifier
    {
        private readonly IFuelReportFuelReportDtoMapper fuelReportDtoMapper;
        private IFuelReportDetailToFuelReportDetailDtoMapper fuelReportDetailDtoMapper;
        private readonly IInvoiceToDtoMapper invoiceToDtoMapper;

        private readonly IInventoryOperationManagement inventoryOperationManagement;

        public InventoryOperationNotifier(
            //IFuelReportFuelReportDtoMapper fuelReportDtoMapper,
            //IInvoiceToDtoMapper invoiceToDtoMapper,
            //IFuelReportDetailToFuelReportDetailDtoMapper fuelReportDetailDtoMapper
            )
        {
            inventoryOperationManagement = new InventoryOperationManagement();

            //this.fuelReportDtoMapper = fuelReportDtoMapper;
            //this.invoiceToDtoMapper = invoiceToDtoMapper;
            //this.fuelReportDetailDtoMapper = fuelReportDetailDtoMapper;
        }

        public List<InventoryOperation> NotifySubmittingFuelReportDetail(FuelReportDetail source)
        {
            try
            {
                return inventoryOperationManagement.ManageFuelReportDetail(source,
                    //TODO: Fake ActorId
                    1101);
            }
            catch (Exception)
            {

                throw;
            }

            var dto = fuelReportDetailDtoMapper.MapToModel(source);
            dto.FuelReport = fuelReportDtoMapper.MapToModel(source.FuelReport);

            setConsumption(dto, source);

            return new List<InventoryOperation>(new InventoryOperation[]
                    {
                     new InventoryOperation(
                        "INV# - " +DateTime.Now.Ticks,
                        DateTime.Now,
                        InventoryActionType.Issue,
                        (long? )null,
                        (long? )null)
                    });
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
                return inventoryOperationManagement.ManageOrderItemBalance(orderItemBalance,
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
                return inventoryOperationManagement.ManageCharterInStart(charterInStart,
                    //TODO: Fake ActorId
                    1101);
            }
            catch (Exception)
            {
                
                throw;
            }
            

            return new List<InventoryOperation>(new InventoryOperation[]
                    {
                     new InventoryOperation(
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
                     "INV# - " +DateTime.Now.Ticks,
                        DateTime.Now,
                        InventoryActionType.Issue,
                        (long? )null,
                        (long? )null)});
        }

        public List<InventoryOperation> NotifySubmittingCharterOutStart(CharterOut charterOutStart)
        {
            return new List<InventoryOperation>(new InventoryOperation[]
                    {
                     new InventoryOperation(
                     "INV# - " +DateTime.Now.Ticks,
                        DateTime.Now,
                        InventoryActionType.Issue,
                        (long? )null,
                        (long? )null)});
        }

        public List<InventoryOperation> NotifySubmittingCharterOutEnd(CharterOut charterOutEnd)
        {
            return new List<InventoryOperation>(new InventoryOperation[]
                    {
                     new InventoryOperation(
                     "INV# - " +DateTime.Now.Ticks,
                        DateTime.Now,
                        InventoryActionType.Issue,
                        (long? )null,
                        (long? )null)});
        }
    }


}
