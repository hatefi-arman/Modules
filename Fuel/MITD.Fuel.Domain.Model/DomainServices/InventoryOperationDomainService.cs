using System;
using System.Collections.Generic;
using System.Linq;
using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Repositories;
using MITD.Fuel.Domain.Model.Specifications;

namespace MITD.Fuel.Domain.Model.DomainServices
{
    public class InventoryOperationDomainService : IInventoryOperationDomainService
    {
        private readonly IInventoryOperationRepository inventoryOperationRepository;
        private readonly IScrapDomainService scrapDomainService;

        public InventoryOperationDomainService(
            IInventoryOperationRepository inventoryOperationRepository,
            IScrapDomainService scrapDomainService)
        {
            this.inventoryOperationRepository = inventoryOperationRepository;
            this.scrapDomainService = scrapDomainService;
        }

        public List<InventoryOperation> GetFuelReportInventoryOperations(FuelReport fuelReport)
        {
            fuelReport.CheckToBeNotCancelled();

            return fuelReport.FuelReportDetails.SelectMany(frd => frd.InventoryOperations).ToList();

            //return inventoryOperationRepository.Find(
            //    inv =>
            //        fuelReport.FuelReportDetails
            //            .SelectMany(frd => frd.InventoryOperations)
            //            .Select(frdinv => frdinv.Id)
            //            .Contains(inv.Id)
            //).ToList();
        }



        public List<InventoryOperation> GetFuelReportDetailInventoryOperations(long fuelReportId, long fuelReportDetailId)
        {
            return this.inventoryOperationRepository.Find(
                inv =>
                    inv.FuelReportDetail.FuelReportId == fuelReportId &&
                    inv.FuelReportDetail.Id == fuelReportDetailId)
                .Where(inv => inv.FuelReportDetail.FuelReport.IsActive()).ToList();
        }

        public List<InventoryOperation> GetVoyageRegisteredInventoryOperations(long voyageId)
        {
            return this.inventoryOperationRepository.Find(
                inv =>
                    inv.FuelReportDetail.FuelReport.VoyageId == voyageId)
                .Where(inv => inv.FuelReportDetail.FuelReport.IsActive()).ToList();
        }


        public bool HasOrderAnyReceipt(long orderId)
        {
            return this.inventoryOperationRepository.Count(
                    inv =>
                        inv.ActionType == InventoryActionType.Receipt &&
                            inv.FuelReportDetail.ReceiveReference != null &&
                            inv.FuelReportDetail.ReceiveReference.ReferenceType == ReferenceType.Order &&
                            inv.FuelReportDetail.ReceiveReference.ReferenceId == orderId) != 0;
        }

        public PageResult<InventoryOperation> GetScrapInventoryOperations(long scrapId, int? pageSize, int? pageIndex)
        {
            //var scrapDetail = scrapDomainService.GetScrapDetail(scrapId, scrapDetailId);

            //return new PageResult<InventoryOperation>()
            //       {
            //           Result = scrapDetail.InventoryOperations
            //       };

            var scrap = scrapDomainService.Get(scrapId);

            return new PageResult<InventoryOperation>()
            {
                Result = scrap.InventoryOperations
            };
        }
    }
}
