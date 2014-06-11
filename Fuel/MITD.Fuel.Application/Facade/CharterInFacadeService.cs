using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy.Serialization;
using MITD.Domain.Repository;
using MITD.Fuel.Application.Facade.Contracts.Mappers;
using MITD.Fuel.Application.Service.Contracts;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Repositories;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Presentation.Contracts;
using MITD.Services.Facade;

namespace MITD.Fuel.Application.Facade
{
    [Interceptor(typeof(SecurityInterception))]
    public class CharterInFacadeService : ICharterInFacadeService,IFacadeService
    {

        #region Prop
        static ObservableCollection<CharterDto> CharterDtos;

        private ICharterInApplicationService _charterInApplicationService;
        private ICharterInRepository _charterInRepository;
        private ICharterInToDtoMapper _charterInToDtoMapper;
        private ICharterItemToDtoMapper _charterItemToDtoMapper;
        private IInventoryOperationToInventoryOperationDtoMapper _inventoryOperationDtoMapper;
        private IEntityConfigurator<Charter> _configurator;
        #endregion


        #region Ctor

        public CharterInFacadeService(

         ICharterInApplicationService charterInApplicationService,
         ICharterInRepository charterInRepository,
         ICharterInToDtoMapper charterInToDtoMapper,
         ICharterItemToDtoMapper charterItemToDtoMapper,
            IInventoryOperationToInventoryOperationDtoMapper inventoryOperationDtoMapper,
            IEntityConfigurator<Charter> configurator 
)
        {
            _charterInApplicationService = charterInApplicationService;
            _charterInRepository = charterInRepository;
            _charterInToDtoMapper = charterInToDtoMapper;
            _charterItemToDtoMapper = charterItemToDtoMapper;
            _inventoryOperationDtoMapper = inventoryOperationDtoMapper;
            _configurator = configurator;

        }

        #endregion


        #region Method

        public PageResultDto<CharterDto> GetAll(long companyId, int pageIndex, int pageSize)
        {
            var data = _charterInRepository.GetByFilter(companyId, pageSize, pageIndex);
            var res= _charterInToDtoMapper.MapToDtoModels(data);

            res.Result.ToList().ForEach(c=>c.EndDateStr=SetEndDate(c.Id));

            return res;
        }


        string SetEndDate(long startId)
        {
            var end = _charterInRepository.GetCharterEnd(startId);
            return (end!=null && end.CurrentState == States.Submitted) ? end.ActionDate.ToShortDateString() : "-------";
        }

        public CharterDto GetCharterEnd(long startId)
        {
            var entity = _charterInRepository.GetCharterEnd(startId);
            var res = new CharterDto();

            if (entity != null)
            {
                res = _charterInToDtoMapper.MapToDtoModel(entity);

                if (entity.CharterItems.Count > 0)
                {
                    var items = _charterItemToDtoMapper.MapToDtoModels(
                        new PageResult<CharterItem>() {Result = entity.CharterItems});
                    items.Result.ToList().ForEach(c => res.CharterItems.Add(c));
                }

                if (entity.InventoryOperationItems.Count > 0)
                {
                    var invOperts = _charterInToDtoMapper.MapToInvDtoModels(
                        new PageResult<InventoryOperation>() {Result = entity.InventoryOperationItems});
                    invOperts.Result.ToList().ForEach(c => res.InventoryOperationDtos.Add(c));
                }
                return res;
            }
            else
            {
                return null;
            }
            
        }


        public CharterDto GetById(long id)
        {
            var entity = _charterInRepository.GetCharterStartById(id);
         
            var res = _charterInToDtoMapper.MapToDtoModel(entity);
            if (entity.CharterItems.Count > 0)
            {
                var items = _charterItemToDtoMapper.MapToDtoModels(
               new PageResult<CharterItem>() { Result = entity.CharterItems });
                items.Result.ToList().ForEach(c => res.CharterItems.Add(c));
            }

            if (entity.InventoryOperationItems.Count > 0)
            {
                var invOperts = _charterInToDtoMapper.MapToInvDtoModels(
                new PageResult<InventoryOperation>() { Result = entity.InventoryOperationItems });
                invOperts.Result.ToList().ForEach(c => res.InventoryOperationDtos.Add(c));
            }
            return res;
        }

        public void Add(CharterDto data)
        {
           
         
            
            if (data.CharterStateType == CharterStateTypeEnum.Start)
            {
                _charterInApplicationService.AddStart(data.Charterer.Id, data.Owner.Id, data.Vessel.Id,
                    data.Currency.Id, data.StartDate, _charterInToDtoMapper.DtoToOfHreConvertor(data.OffHirePricingType));
            }
            else
            {
                _charterInApplicationService.AddEnd(data.Charterer.Id, data.Owner.Id, data.Vessel.Id,
                      data.Currency.Id, data.EndDate, _charterInToDtoMapper.CharterEndTypeEnumConvertor(data.CharterEndType), _charterInToDtoMapper.DtoToOfHreConvertor(data.OffHirePricingType));
            }


        }

        public void Update(CharterDto data)
        {
            if (data.CharterStateType == CharterStateTypeEnum.Start)
            {
                _charterInApplicationService.UpdateStart(data.Id, data.Charterer.Id, data.Owner.Id, data.Vessel.Id,
                    data.Currency.Id, data.StartDate, _charterInToDtoMapper.DtoToOfHreConvertor(data.OffHirePricingType));
            }
            else
            {
                _charterInApplicationService.UpdateEnd(data.Id, data.Charterer.Id, data.Owner.Id, data.Vessel.Id,
                      data.Currency.Id, data.EndDate, _charterInToDtoMapper.CharterEndTypeEnumConvertor(data.CharterEndType));
            }
        }

        public void Delete(long id)
        {
            _charterInApplicationService.Delete(id);
        }

        public PageResultDto<CharterItemDto> GetAllItem(long charterId, int pageIndex, int pageSize)
        {
            var res = new PageResultDto<CharterItemDto>();// { Result =   _charterInToDtoMapper.MapToDtoModel(_charterInRepository.GetCharterStartById(charterId)); };
            return res;
        }

        public CharterItemDto GetItemById(long id, long charterItemId)
        {
            var entity = _charterInRepository.GetById(id).CharterItems;
            return _charterItemToDtoMapper.MapToDtoModel(entity.Single(c => c.Id == charterItemId));
        }

        public void AddItem(CharterItemDto dto)
        {
            _charterInApplicationService.AddItem(dto.Id, dto.CharterId, dto.Rob, dto.Fee, dto.FeeOffhire, dto.Good.Id, dto.TankDto.Id, dto.Good.Unit.Id);


        }

        public void UpdateItem(CharterItemDto dto)
        {
            _charterInApplicationService.UpdateItem(dto.Id, dto.CharterId, dto.Rob, dto.Fee, dto.FeeOffhire, dto.Good.Id, dto.TankDto.Id, dto.Good.Unit.Id);
        }

        public void DeleteItem(long id, long charterItemId)
        {
            _charterInApplicationService.DeleteItem(charterItemId, id);
        }
        #endregion






    }
}
