using System;
using System.Collections.Generic;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Factories;

namespace MITD.Fuel.Domain.Model.DomainObjects.Factories
{
    public interface IFuelReportFactory : IFactory
    {
        FuelReport CreateFuelReport(
            string code,
            string description,
            DateTime eventDate,
            DateTime reportDate,
            long vesselInCompanyId,
            long? voyageId,
            FuelReportTypes fuelReportType,
            States state);

        FuelReportDetail CreateFuelReportDetail(
            long fuelReportId,
            double rob,
            string robUOM,
            double consumption,
            double? receive,
            ReceiveTypes? receiveType,
            double? transfer,
            TransferTypes? transferType,
            double? correction,
            CorrectionTypes? correctionType,
            decimal? correctionPrice,
            string correctionPriceCurrencyISOCode,
            long? correctionPriceCurrencyId,
            long fuelTypeId,
            long measuringUnitId,
            long tankId);
    }
}