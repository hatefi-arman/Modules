using System;
using System.Collections.Generic;
using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Services.Application;

namespace MITD.Fuel.Application.Service.Contracts
{
    public interface IFuelReportApplicationService : IApplicationService
    {
        FuelReport GetById(long id);

        FuelReportDetail UpdateFuelReportDetail(long fuelReportId, long fuelReportDetailId, double rob, double consumption, double? receive, ReceiveTypes? receiveTypeId, double? transfer, TransferTypes? transferTypeId, double? correction, CorrectionTypes? correctionType, decimal? correctionPrice, long? currencyId, Reference transferReference, Reference receiveReference, Reference correctionReference);

        FuelReport UpdateVoyageId(long fuelReportId, long voyageId);

        void IsSetFuelReportInventoryResultPossible(long fuelReportId);

        void SetFuelReportInventoryResults(InventoryResultCommand resultBag);

        FuelReport UpdateVoyageEndOfVoyageFuelReport(long fuelReportId, DateTime newDateTime);
    }
}
