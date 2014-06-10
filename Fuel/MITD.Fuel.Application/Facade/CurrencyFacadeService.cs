#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Castle.Core;
using MITD.Fuel.Application.Facade.Contracts.Mappers;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Presentation.Contracts;
using MITD.Services.Facade;

#endregion

namespace MITD.Fuel.Application.Facade
{
    [Interceptor(typeof(SecurityInterception))]
    public class CurrencyFacadeService : ICurrencyFacadeService
    {
        #region props

        private readonly ICurrencyDomainService currencyDomainService;
        private readonly ICurrencyToCurrencyDtoMapper _mapper;

        #endregion

        #region ctor

        public CurrencyFacadeService(
              ICurrencyDomainService currencyDomainService,
            ICurrencyToCurrencyDtoMapper currencyToCurrencyDtoMapper

                                                 )
        {
            this.currencyDomainService = currencyDomainService;
            this._mapper = currencyToCurrencyDtoMapper;
        }

        #endregion

        #region methods

        public IEnumerable<CurrencyDto> GetAll()
        {
            var entities = currencyDomainService.GetAll();
            var dtos = _mapper.MapToModel(entities).ToList();

            return dtos;
        }

        public CurrencyDto GetById(int id)
        {
            var dto = _mapper.MapEntityToDto(currencyDomainService.Get(id));
            dto.CurrencyToMainCurrencyRate = currencyDomainService.GetCurrencyToMainCurrencyRate(id, DateTime.Now);
            return dto;
        }

        public decimal GetCurrencyValueInMainCurrency(long currencyId, decimal value, DateTime dateTime)
        {
            var currency = currencyDomainService.Get(currencyId);

            var result = currencyDomainService.GetCurrencyValueInMainCurrency(currency, value, dateTime);
            return result;
        }

        public decimal ConvertPrice(decimal value, long sourceCurrencyId, long destinationCurrencyId, DateTime dateTime)
        {
            var sourceCurrency = currencyDomainService.Get(sourceCurrencyId);
            var destinationCurrency = currencyDomainService.Get(destinationCurrencyId);

            var result = currencyDomainService.ConvertPrice(value, sourceCurrency, destinationCurrency, dateTime);

            return result;
        }

        public decimal ConvertPriceToMainCurrency(decimal value, long sourceCurrencyId, DateTime dateTime)
        {
            var sourceCurrency = currencyDomainService.Get(sourceCurrencyId);
            var destinationCurrency = currencyDomainService.GetMainCurrency();

            var result = currencyDomainService.ConvertPrice(value, sourceCurrency, destinationCurrency, dateTime);

            return result;
        }

        #endregion

    }
}