using System.Collections.Generic;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Services.Facade;

namespace MITD.Fuel.Application.Facade.Contracts.Mappers
{
    public interface IFuelReportDetailToFuelReportDetailDtoMapper : IFacadeMapper<FuelReportDetail, FuelReportDetailDto>
    {
        TransferTypeEnum MapEntityTransferTypeToDtoTransferType(
            TransferTypes? transferType);

        TransferTypes? MapDtoTransferTypeTypeToEntityTransferTypeType(
            TransferTypeEnum? transferType);

        ReceiveTypeEnum MapEntityReceiveTypeToDtoReceiveType(
            ReceiveTypes? receiveType);

        ReceiveTypes? MapDtoReceiveTypeTypeToEntityReceiveTypeType(
            ReceiveTypeEnum? receiveType);


        CorrectionTypeEnum MapEntityCorrectionTypeToDtoCorrectionType(Domain.Model.Enums.CorrectionTypes? correctionType);

        CorrectionTypes? MapDtoCorrectionTypeToEntityCorrectionType(CorrectionTypeEnum? correctionType);
    }
}
