﻿using MITD.Domain.Repository;
using MITD.Fuel.Application.Facade.Contracts.Mappers;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Presentation.Contracts;
using MITD.Services.Facade;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CharterType = MITD.Fuel.Domain.Model.Enums.CharterType;
using OffHirePricingType = MITD.Fuel.Domain.Model.Enums.OffHirePricingType;

namespace MITD.Fuel.Application.Facade.Mappers
{
   public class CharterOutToDtoMapper : BaseFacadeMapper<CharterOut, CharterDto>, ICharterOutToDtoMapper
    {
       public CharterDto MapToDtoModel(CharterOut charterOut)
        {
            var res = new CharterDto()
            {
                Id = charterOut.Id,
                Owner = (charterOut.Owner != null) ? base.Map(new CompanyDto(), charterOut.Owner) as CompanyDto : null,
                Charterer = (charterOut.Charterer != null) ? base.Map(new CompanyDto(), charterOut.Charterer) as CompanyDto : null,
                Vessel = (charterOut.VesselInCompany != null) ? base.Map(new VesselDto(), charterOut.VesselInCompany) as VesselDto : null,
                Currency = (charterOut.Currency != null) ? base.Map(new CurrencyDto(), charterOut.Currency) as CurrencyDto : null,
                OffHirePricingType = OfHreToDtoConvertor(charterOut.OffHirePricingType),
                CurrentStateName = GetStateName(charterOut.CurrentState),
                IsFinalApproveVisiblity = (charterOut.CurrentState != States.Submitted)
                
            };

            if (charterOut.CharterType == CharterType.Start)
            {
                res.StartDate = charterOut.ActionDate;

            }
            else
            {
                res.EndDate = charterOut.ActionDate;
                res.CharterEndType = CharterEndTypeConvertor(charterOut.CharterEndType);
            }

            res.CharterItems = new ObservableCollection<CharterItemDto>();
            res.InventoryOperationDtos = new ObservableCollection<FuelReportInventoryOperationDto>();

            return res;
        }

        public PageResultDto<CharterDto> MapToDtoModels(PageResult<CharterOut> charterIns)
        {
            var dtos = new PageResultDto<CharterDto>();
            dtos.Result = new List<CharterDto>();
            charterIns.Result.ToList().ForEach(c => dtos.Result.Add(MapToDtoModel(c)));

            dtos.TotalCount = charterIns.TotalCount;
            dtos.TotalPages = charterIns.TotalPages;
            dtos.CurrentPage = charterIns.CurrentPage;
            return dtos;

        }


        public CharterEndTypeEnum CharterEndTypeConvertor(CharterEndType charterEndType)
        {
            var res = CharterEndTypeEnum.None;
            switch (charterEndType)
            {
                case CharterEndType.None:
                    res = CharterEndTypeEnum.None;
                    break;
                case CharterEndType.DryDock:
                    res = CharterEndTypeEnum.DryDock;
                    break;
                case CharterEndType.CharterInEnd:
                    res = CharterEndTypeEnum.CharterInEnd;
                    break;
               
                case CharterEndType.LayUp:
                    res = CharterEndTypeEnum.LayUp;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("charterEndType");
            }
            return res;
        }




        public Presentation.Contracts.Enums.OffHirePricingType OfHreToDtoConvertor(Domain.Model.Enums.OffHirePricingType pricingType)
        {
            if (pricingType == OffHirePricingType.CharterPartyBase)
            {
                return Presentation.Contracts.Enums.OffHirePricingType.CharterPartyBase;
            }
            else
            {
                return Presentation.Contracts.Enums.OffHirePricingType.IssueBase;
            }
        }


        public Domain.Model.Enums.OffHirePricingType DtoToOfHreConvertor(Presentation.Contracts.Enums.OffHirePricingType pricingType)
        {
            if (pricingType == Presentation.Contracts.Enums.OffHirePricingType.CharterPartyBase)
            {
                return Domain.Model.Enums.OffHirePricingType.CharterPartyBase;
            }
            else
            {
                return Domain.Model.Enums.OffHirePricingType.IssueBase;
            }
        }


        public CharterEndType CharterEndTypeEnumConvertor(CharterEndTypeEnum charterEndTypeEnum)
        {
            var res = CharterEndType.None;
            switch (charterEndTypeEnum)
            {
                case CharterEndTypeEnum.None:
                    res = CharterEndType.None;
                    break;
                case CharterEndTypeEnum.DryDock:
                    res = CharterEndType.DryDock;
                    break;
                case CharterEndTypeEnum.CharterInEnd:
                    res = CharterEndType.CharterInEnd;
                    break;
               
                case CharterEndTypeEnum.LayUp:
                    res = CharterEndType.LayUp;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("charterEndTypeEnum");
            }
            return res;
        }

        public PageResultDto<FuelReportInventoryOperationDto> MapToInvDtoModels(PageResult<InventoryOperation> entities)
        {
            var dtos = new PageResultDto<FuelReportInventoryOperationDto>();
            dtos.Result = entities.Result.Select(MapToInvDtoModel).ToList();

            return dtos;
        }

        public FuelReportInventoryOperationDto MapToInvDtoModel(InventoryOperation entity)
        {
            var dto = new FuelReportInventoryOperationDto()
            {
                Id = entity.Id,
                Code = entity.ActionNumber,
                ActionType = entity.ActionType.ToString(),
                ActionDate = entity.ActionDate,

            };

            return dto;
        }

        public string GetStateName(States states)
        {
            var res = "ثبت";
            switch (states)
            {
                case States.Open:
                    break;
                case States.Submitted:
                    res = "تایید نهایی";
                    break;
                case States.Closed:
                    break;
                case States.Cancelled:
                    break;
                case States.SubmitRejected:
                    res = "برگشت از تایید نهایی";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("states");
            }
            return res;
        }
    }
}
