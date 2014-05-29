using System;
using System.Collections.ObjectModel;
using MITD.Core;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Application.Facade.Contracts.Mappers;
using MITD.Services.Facade;
using System.Linq;
using System.Collections.Generic;

namespace MITD.Fuel.Application.Facade.Mappers
{
    public class FuelReportToFuelReportDtoMapper : BaseFacadeMapper<FuelReport, FuelReportDto>, IFuelReportFuelReportDtoMapper
    {

        private readonly IFuelReportDetailToFuelReportDetailDtoMapper fuelReportDetailMapper;

        private readonly IVesselToVesselDtoMapper vesselMapper;

        public FuelReportToFuelReportDtoMapper(
            IFuelReportDetailToFuelReportDetailDtoMapper fuelReportDetailToFuelReportDetailDtoMapper,
            IVesselToVesselDtoMapper vesselMapper)
        {
            this.fuelReportDetailMapper = fuelReportDetailToFuelReportDetailDtoMapper;

            this.vesselMapper = vesselMapper;
        }


        public override FuelReportDto MapToModel(FuelReport entity)
        {
            var dto = new FuelReportDto();

            dto.FuelReportDetail = new ObservableCollection<FuelReportDetailDto>();

            if (entity.FuelReportDetails != null && entity.FuelReportDetails.Count > 0)
            {
                var list = fuelReportDetailMapper.MapToModel(entity.FuelReportDetails.ToList());
                dto.FuelReportDetail = new ObservableCollection<FuelReportDetailDto>(list);
            }

            base.Map(dto, entity);
            dto.FuelReportType = MapEntityFuelReportTypeToDtoFuelReportType(entity.FuelReportType);

            var vDto = new VoyageDto();
            if (entity.Voyage != null)
            {
                base.Map(vDto, entity.Voyage);
            }
            dto.Voyage = vDto;


            var vesselDto = new VesselDto();
            base.Map(vesselDto, entity.VesselInCompany);
            dto.VesselDto = vesselDto;

            var companyDto = new CompanyDto();
            base.Map(companyDto, entity.VesselInCompany.Company);
            dto.VesselDto.Company = companyDto;

            //TODO: <A.H> Review
            dto.IsNextActionFinalApprove = entity.State == States.Submitted;
            dto.IsActive = entity.IsActive();

            dto.CurrentStateName = entity.State == States.Closed ? "Closed" :
                entity.ApproveWorkFlows.Last().CurrentWorkflowStep.CurrentWorkflowStage.ToString();

            dto.UserInChargName = entity.ApproveWorkFlows.Last().CurrentWorkflowStep.ActorUser.Name;


            //            dto.IsNextActionFinalApprove =
            //                (entity.CurrentApproveWorkFlowConfig != null) &&
            //                (entity.CurrentApproveWorkFlowConfig.NextApproveWorkFlowConfig != null) &&
            //                (entity.CurrentApproveWorkFlowConfig.NextApproveWorkFlowConfig.ApproveFlowState == ApproveFlowStates.FinalApproved);


            //TODO: <A.H> From ApproveFlow to dto.UserInChargName
            //if (entity.ActionUserId != null)
            //    dto.UserInChargName = User.UserIdToUserName(entity.ActionUserId);
            //TODO: <A.H> Review
            //            if (entity.CurrentApproveWorkFlowConfig != null)
            //TODO: <A.H> There should be a mapper for ActionTypes to ActionTypeEnum.
            //TODO: <A.H> Review
            //                dto.CurrentStateName = entity.CurrentApproveWorkFlowConfig.ApproveFlowState.ToString();
            return dto;
        }

        public override IEnumerable<FuelReportDto> MapToModel(IEnumerable<FuelReport> entities)
        {
            return entities.Select(this.MapToModel);
        }

        public FuelReportTypeEnum MapEntityFuelReportTypeToDtoFuelReportType(FuelReportTypes fuelReportType)
        {
            switch (fuelReportType)
            {
                case FuelReportTypes.NoonReport:
                    return FuelReportTypeEnum.NoonReport;
                case FuelReportTypes.EndOfVoyage:
                    return FuelReportTypeEnum.EndofVoyage;
                case FuelReportTypes.ArrivalReport:
                    return FuelReportTypeEnum.ArrivalReport;
                case FuelReportTypes.DepartureReport:
                    return FuelReportTypeEnum.DepartureReport;
                case FuelReportTypes.EndOfYear:
                    return FuelReportTypeEnum.EndOfYear;
                case FuelReportTypes.EndOfMonth:
                    return FuelReportTypeEnum.EndOfMonth;
                case FuelReportTypes.CharterInEnd:
                    return FuelReportTypeEnum.CharterInEnd;
                case FuelReportTypes.CharterOutStart:
                    return FuelReportTypeEnum.CharterOutStart;
                case FuelReportTypes.DryDock:
                    return FuelReportTypeEnum.DryDock;
                case FuelReportTypes.OffHire:
                    return FuelReportTypeEnum.OffHire;
                case FuelReportTypes.LayUp:
                    return FuelReportTypeEnum.LayUp;
                default:
                    throw new ArgumentOutOfRangeException("fuelReportType");
            }
        }

        public FuelReportTypes MapDtoFuelReportTypeToEntityFuelReportType(FuelReportTypeEnum fuelReportType)
        {
            switch (fuelReportType)
            {
                case FuelReportTypeEnum.NoonReport:
                    return FuelReportTypes.NoonReport;
                case FuelReportTypeEnum.EndofVoyage:
                    return FuelReportTypes.EndOfVoyage;
                case FuelReportTypeEnum.ArrivalReport:
                    return FuelReportTypes.ArrivalReport;
                case FuelReportTypeEnum.DepartureReport:
                    return FuelReportTypes.DepartureReport;
                case FuelReportTypeEnum.EndOfYear:
                    return FuelReportTypes.EndOfYear;
                case FuelReportTypeEnum.EndOfMonth:
                    return FuelReportTypes.EndOfMonth;
                case FuelReportTypeEnum.CharterInEnd:
                    return FuelReportTypes.CharterInEnd;
                case FuelReportTypeEnum.CharterOutStart:
                    return FuelReportTypes.CharterOutStart;
                case FuelReportTypeEnum.DryDock:
                    return FuelReportTypes.DryDock;
                case FuelReportTypeEnum.OffHire:
                    return FuelReportTypes.OffHire;
                case FuelReportTypeEnum.LayUp:
                    return FuelReportTypes.LayUp;
                default:
                    throw new ArgumentOutOfRangeException("fuelReportType");
            }
        }

        public FuelReportDto MapToModel(FuelReport entity, Action<FuelReportDetail, FuelReportDetailDto> action)
        {
            var dto = this.MapToModel(entity);

            foreach (var detailDto in dto.FuelReportDetail)
            {
                action(entity.FuelReportDetails.FirstOrDefault(
                    frd => frd.Id == detailDto.Id),
                    detailDto);

            }

            return dto;
        }

        public System.Collections.Generic.IEnumerable<FuelReportDto> MapToModel(System.Collections.Generic.IEnumerable<FuelReport> entities, Action<FuelReportDetail, FuelReportDetailDto> action)
        {
            List<FuelReportDto> dtos = new List<FuelReportDto>();

            foreach (var entity in entities)
            {
                dtos.Add(MapToModel(entity, action));
            }

            return dtos;
        }
    }
}