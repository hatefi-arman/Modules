using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using MITD.Domain.Repository;
using MITD.Fuel.Application.Facade.Contracts.Mappers;
using MITD.Fuel.Application.Service.Contracts;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Presentation.Contracts;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;

namespace MITD.Fuel.Application.Facade
{
   [Interceptor(typeof(SecurityInterception))]
    public class FuelReportFacadeService : IFuelReportFacadeService
    {
        private readonly IFuelReportApplicationService appService;
        private readonly IFuelReportFuelReportDtoMapper fuelReportMapper;
        private readonly IFuelReportDetailToFuelReportDetailDtoMapper fuelReportDetailToFuelReportDetailDtoMapper;
        private readonly ICurrencyToCurrencyDtoMapper currencyToCurrencyDtoMapper;
        private readonly ICurrencyDomainService currencyDomainService;
        private readonly IFuelReportDomainService fuelReportDomainService;
        private readonly IInventoryOperationDomainService inventoryOperationDomainService;
        private readonly IInventoryOperationToInventoryOperationDtoMapper inventoryOperationMapper;


        //================================================================================

        public FuelReportFacadeService(
            IFuelReportApplicationService appService,
            IFuelReportFuelReportDtoMapper fuelReportMapper,
            IFuelReportDetailToFuelReportDetailDtoMapper fuelReportDetailToFuelReportDetailDtoMapper,
            ICurrencyDomainService currencyDomainService,
            ICurrencyToCurrencyDtoMapper currencyToCurrencyDtoMapper,
            IFuelReportDomainService fuelReportDomainService,
            IInventoryOperationDomainService inventoryOperationDomainService,
            IInventoryOperationToInventoryOperationDtoMapper inventoryOperationMapper)
        {
            this.appService = appService;
            this.fuelReportMapper = fuelReportMapper;
            this.fuelReportDetailToFuelReportDetailDtoMapper = fuelReportDetailToFuelReportDetailDtoMapper;
            this.currencyToCurrencyDtoMapper = currencyToCurrencyDtoMapper;
            this.fuelReportDomainService = fuelReportDomainService;
            this.inventoryOperationDomainService = inventoryOperationDomainService;
            this.inventoryOperationMapper = inventoryOperationMapper;
            this.currencyDomainService = currencyDomainService;
        }

        //================================================================================

        public FuelReportDto Add(FuelReportDto data)
        {
            throw new NotImplementedException();
        }

        //================================================================================

        public FuelReportDto GetById(long id, bool includeReferencesLookup)
        {
            var entity = fuelReportDomainService.Get(id);

            FuelReportDto dto = null;
            if (includeReferencesLookup)
                dto = fuelReportMapper.MapToModel(entity, populateFuelReportDetailDtoReferencesLookup);
            else
                dto = fuelReportMapper.MapToModel(entity);

            return dto;
        }

        //================================================================================

        public PageResultDto<FuelReportDto> GetAll(int pageSize, int pageIndex, bool includeReferencesLookup)
        {
            List<FuelReport> data = fuelReportDomainService.GetAll();

            List<FuelReportDto> mapped = null;

            if (includeReferencesLookup)
                mapped = fuelReportMapper.MapToModel(data, populateFuelReportDetailDtoReferencesLookup).ToList();
            else
                mapped = fuelReportMapper.MapToModel(data).ToList();

            var result = new PageResultDto<FuelReportDto>
            {
                CurrentPage = 1,
                PageSize = 1,
                Result = mapped,
                TotalCount = 1,
                TotalPages = 1
            };

            return result;
        }

        //================================================================================

        public PageResultDto<FuelReportDto> GetByFilter(long companyId, long vesselInCompanyId, int pageSize, int pageIndex, bool includeReferencesLookup)
        {
            PageResult<FuelReport> data = fuelReportDomainService.GetByFilter(companyId, vesselInCompanyId, pageSize, pageIndex);

            List<FuelReportDto> mapped = null;

            if (includeReferencesLookup)
                mapped = fuelReportMapper.MapToModel(data.Result, populateFuelReportDetailDtoReferencesLookup).ToList();
            else
                mapped = fuelReportMapper.MapToModel(data.Result).ToList();

            var result = new PageResultDto<FuelReportDto>
                             {
                                 CurrentPage = data.CurrentPage,
                                 PageSize = data.PageSize,
                                 Result = mapped,
                                 TotalCount = data.TotalCount,
                                 TotalPages = data.TotalPages
                             };

            return result;
        }

        //================================================================================

        public FuelReportDetailDto UpdateFuelReportDetail(long fuelReportId, FuelReportDetailDto fuelReportDetailDto)
        {
            var transferReference = Reference.Empty;
            var receiveReference = Reference.Empty;
            var correctionReference = Reference.Empty;

            if (fuelReportDetailDto.FuelReportTransferReferenceNoDto != null)
            {
                transferReference = new Reference()
                                    {
                                        Code = fuelReportDetailDto.FuelReportTransferReferenceNoDto.Code,
                                        ReferenceType = (ReferenceType)fuelReportDetailDto.FuelReportTransferReferenceNoDto.ReferenceType,
                                        ReferenceId = fuelReportDetailDto.FuelReportTransferReferenceNoDto.Id
                                    };
            }

            if (fuelReportDetailDto.FuelReportReciveReferenceNoDto != null)
            {
                receiveReference = new Reference()
                {
                    Code = fuelReportDetailDto.FuelReportReciveReferenceNoDto.Code,
                    ReferenceType = (ReferenceType)fuelReportDetailDto.FuelReportReciveReferenceNoDto.ReferenceType,
                    ReferenceId = fuelReportDetailDto.FuelReportReciveReferenceNoDto.Id
                };
            }


            if (fuelReportDetailDto.FuelReportCorrectionReferenceNoDto != null)
            {
                correctionReference = new Reference()
                {
                    Code = fuelReportDetailDto.FuelReportCorrectionReferenceNoDto.Code,
                    ReferenceType = (ReferenceType)fuelReportDetailDto.FuelReportCorrectionReferenceNoDto.ReferenceType,
                    ReferenceId = fuelReportDetailDto.FuelReportCorrectionReferenceNoDto.Id
                };
            }



            var updatedEntity = this.appService.UpdateFuelReportDetail(
                fuelReportDetailDto.FuelReportId,
                fuelReportDetailDto.Id,
                fuelReportDetailDto.ROB,
                fuelReportDetailDto.Consumption.HasValue ? fuelReportDetailDto.Consumption.Value : 0,
                fuelReportDetailDto.Recieve,
                this.fuelReportDetailToFuelReportDetailDtoMapper.MapDtoReceiveTypeTypeToEntityReceiveTypeType(fuelReportDetailDto.ReceiveType),
                fuelReportDetailDto.Transfer,
                this.fuelReportDetailToFuelReportDetailDtoMapper.MapDtoTransferTypeTypeToEntityTransferTypeType(fuelReportDetailDto.TransferType),
                fuelReportDetailDto.Correction,
                this.fuelReportDetailToFuelReportDetailDtoMapper.MapDtoCorrectionTypeToEntityCorrectionType(fuelReportDetailDto.CorrectionType),
                fuelReportDetailDto.CorrectionPrice.HasValue ? (decimal?) new decimal(fuelReportDetailDto.CorrectionPrice.Value) : null,
                fuelReportDetailDto.CurrencyDto == null ? null : (long?)fuelReportDetailDto.CurrencyDto.Id,
                transferReference,
                receiveReference,
                correctionReference);

            var dto = fuelReportDetailToFuelReportDetailDtoMapper.MapToModel(updatedEntity);

            populateFuelReportDetailDtoReferencesLookup(updatedEntity, dto);

            return dto;
        }

        //================================================================================

        public List<CurrencyDto> GetAllCurrency()
        {
            var allCurrencies = this.currencyDomainService.GetAll();

            return currencyToCurrencyDtoMapper.MapEntityToDto(allCurrencies);
        }

        //================================================================================

        public FuelReportDto Update(FuelReportDto fuelReportDto)
        {
            if (fuelReportDto.Voyage == null)
                throw new InvalidArgument("fuelReportDto.Voyage");

            var updatedFuelReport = this.appService.UpdateVoyageId(
                fuelReportDto.Id,
                fuelReportDto.Voyage.Id);

            var updatedDto = fuelReportMapper.MapToModel(updatedFuelReport);

            return updatedDto;
        }

        //================================================================================

        private void populateFuelReportDetailDtoReferencesLookup(FuelReportDetail fuelReportDetail, FuelReportDetailDto fuelReportDetailDto)
        {
            var correctionReferences = fuelReportDomainService.GetFuelReportDetailCorrectionReferences(fuelReportDetail);
            var transferReferences = fuelReportDomainService.GetFuelReportDetailTransferReferences(fuelReportDetail);
            var receiveReferences = fuelReportDomainService.GetFuelReportDetailReceiveReferences(fuelReportDetail);
            var rejectedReferences = fuelReportDomainService.GetFuelReportDetailRejectedTransferReferences(fuelReportDetail);


            fuelReportDetailDto.FuelReportCorrectionReferenceNoDtos = new List<FuelReportCorrectionReferenceNoDto>();

            if (correctionReferences != null)
            {
                fuelReportDetailDto.FuelReportCorrectionReferenceNoDtos.AddRange(
                    correctionReferences.Select(r => new FuelReportCorrectionReferenceNoDto()
                        {
                            Id = r.ReferenceId.Value,
                            Code = r.Code,
                            ReferenceType = (ReferenceTypeEnum)(int)r.ReferenceType.Value
                        }));
            }

            fuelReportDetailDto.FuelReportTransferReferenceNoDtos = new List<FuelReportTransferReferenceNoDto>();

            if (transferReferences != null)
            {
                fuelReportDetailDto.FuelReportTransferReferenceNoDtos.AddRange(transferReferences.Select(r => new FuelReportTransferReferenceNoDto()
                {
                    Id = r.ReferenceId.Value,
                    Code = r.Code,
                    ReferenceType = (ReferenceTypeEnum)(int)r.ReferenceType.Value
                }));
            }


            fuelReportDetailDto.FuelReportReciveReferenceNoDtos = new List<FuelReportReciveReferenceNoDto>();

            if (receiveReferences != null)
            {
                fuelReportDetailDto.FuelReportReciveReferenceNoDtos.AddRange(receiveReferences.Select(r => new FuelReportReciveReferenceNoDto()
                {
                    Id = r.ReferenceId.Value,
                    Code = r.Code,
                    ReferenceType = (ReferenceTypeEnum)(int)r.ReferenceType.Value
                }));
            }

            fuelReportDetailDto.RejectedTransferReferenceNoDtos = new List<FuelReportTransferReferenceNoDto>();

            if (rejectedReferences != null)
            {
                fuelReportDetailDto.RejectedTransferReferenceNoDtos.AddRange(rejectedReferences.Select(r => new FuelReportTransferReferenceNoDto()
                {
                    Id = r.ReferenceId.Value,
                    Code = r.Code,
                    ReferenceType = (ReferenceTypeEnum)(int)r.ReferenceType.Value
                }));
            }
        }

        //================================================================================

        public List<FuelReportInventoryOperationDto> GetInventoryOperations(long id, long detailId)
        {
            var result = inventoryOperationMapper.MapToModel(
                inventoryOperationDomainService.GetFuelReportDetailInventoryOperations(id, detailId)).ToList();

            var detailDto = GetById(id, false).FuelReportDetail.First(frd => frd.Id == detailId);

            result.ForEach(invDto => invDto.Good = detailDto.Good);

            return result;
        }

        //================================================================================

    }
}
