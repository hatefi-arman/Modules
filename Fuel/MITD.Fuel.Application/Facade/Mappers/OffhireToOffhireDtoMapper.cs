using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MITD.Fuel.Application.Facade.Contracts.Mappers;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Services.Facade;

namespace MITD.Fuel.Application.Facade.Mappers
{
    public class OffhireToOffhireDtoMapper : BaseFacadeMapper<Offhire, OffhireDto>, IOffhireToOffhireDtoMapper
    {
        private readonly IVesselToVesselDtoMapper vesselDtoMapper;
        private readonly IFacadeMapper<Company, CompanyDto> companyDtoMapper;
        private readonly IOffhireDetailToOffhireDetailDtoMapper offhireDetailMapper;
        private readonly ITankToTankDtoMapper tankDtoMapper;
        private readonly IFacadeMapper<User, UserDto> userDtoMapper;
        private readonly IActivityLocationToActivityLocationDtoMapper activityLocationDtoMapper;
        private readonly ICurrencyToCurrencyDtoMapper currencyDtoMapper;
        private readonly IVoyageToVoyageDtoMapper voyageDtoMapper;

        public OffhireToOffhireDtoMapper(
            IVesselToVesselDtoMapper vesselDtoMapper,
            IFacadeMapper<Company, CompanyDto> companyDtoMapper,
            IOffhireDetailToOffhireDetailDtoMapper offhireDetailMapper,
            ITankToTankDtoMapper tankDtoMapper,
            IFacadeMapper<User, UserDto> userDtoMapper,
            IActivityLocationToActivityLocationDtoMapper activityLocationDtoMapper,
            ICurrencyToCurrencyDtoMapper currencyDtoMapper, IVoyageToVoyageDtoMapper voyageDtoMapper)
        {
            this.vesselDtoMapper = vesselDtoMapper;
            this.companyDtoMapper = companyDtoMapper;
            this.offhireDetailMapper = offhireDetailMapper;
            this.tankDtoMapper = tankDtoMapper;
            this.userDtoMapper = userDtoMapper;
            this.activityLocationDtoMapper = activityLocationDtoMapper;
            this.currencyDtoMapper = currencyDtoMapper;
            this.voyageDtoMapper = voyageDtoMapper;
        }

        public override OffhireDto MapToModel(Offhire entity)
        {
            var dto = base.MapToModel(entity);

            dto.IntroducerType = (CharteringPartyType)(int)entity.IntroducerType;

            dto.Vessel = this.vesselDtoMapper.MapToModel(entity.VesselInCompany);
            dto.Vessel.TankDtos = this.tankDtoMapper.MapToModel(entity.VesselInCompany.Tanks).ToList();
            dto.OffhireLocation = this.activityLocationDtoMapper.MapToModel(entity.OffhireLocation);

            dto.Introducer = this.companyDtoMapper.MapToModel(entity.Introducer);
            dto.OffhireDetails = new ObservableCollection<OffhireDetailDto>(offhireDetailMapper.MapToModel(entity.OffhireDetails));

            dto.VoucherCurrency = this.currencyDtoMapper.MapToModel(entity.VoucherCurrency);
            dto.VoucherDate = entity.VoucherDate;

            if (entity.Voyage != null)
                dto.Voyage = this.voyageDtoMapper.MapToModel(entity.Voyage);

            var lastWorkflowLog = entity.ApproveWorkflows.LastOrDefault();

            if (lastWorkflowLog != null)
            {
                dto.UserInCharge = userDtoMapper.MapToModel(lastWorkflowLog.ActorUser);
                dto.CurrentState = lastWorkflowLog.CurrentWorkflowStep.CurrentWorkflowStage.ToString();
            }

            return dto;
        }

        public OffhireDto MapToModel(Offhire entity, Action<Offhire, OffhireDto> action)
        {
            var dto = this.MapToModel(entity);

            if (action != null) action(entity, dto);

            return dto;
        }

        public IEnumerable<OffhireDto> MapToModel(IEnumerable<Offhire> entities, Action<Offhire, OffhireDto> action)
        {
            var result = entities.Select(e => this.MapToModel(e, action));
            return result;
        }
    }
}