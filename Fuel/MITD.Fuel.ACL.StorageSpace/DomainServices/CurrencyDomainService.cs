using System;
using System.Collections.Generic;
using System.Linq;
using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.FakeDomainServices;
using MITD.Fuel.Domain.Model.IDomainServices;

namespace MITD.Fuel.ACL.StorageSpace.DomainServices
{
    public class CurrencyDomainService : ICurrencyDomainService
    {
        //TODO: (A.H) Added for Fake data fetch;
        private readonly IRepository<Currency> currencyFakeRepository;

        public CurrencyDomainService(IRepository<Currency> currencyFakeRepository)
        {
            //TODO: (A.H) Added for Fake data fetch;
            this.currencyFakeRepository = currencyFakeRepository;
        }

        public Currency Get(long id)
        {
            return currencyFakeRepository.Single(c => c.Id == id);
        }


        public List<Currency> GetAll()
        {
            return currencyFakeRepository.GetAll().ToList();
            /*
                 var data = Adapter.Get(id);
            if (data == null)
                throw new ObjectNotFound("EnterpriseParty");
            return data;
             */
        }

        public List<Currency> Get(List<long> IDs)
        {
            return GetAll().Where(c => IDs.Contains(c.Id)).ToList();
        }

        public decimal ConvertPrice(decimal sourceValue, Currency sourceCurrency, Currency destinationCurrency, DateTime dateTime)
        {
            //return sourceValue;

            var mainCurrency = GetMainCurrency();

            if (sourceCurrency.Id == mainCurrency.Id)
            {
                var rateToMainCurrency = GetCurrencyToMainCurrencyRate(destinationCurrency.Id, dateTime);

                var result = sourceValue / rateToMainCurrency;
                return decimal.Round(result, 2, MidpointRounding.AwayFromZero);
                //return result;
            }
            else if (destinationCurrency.Id == mainCurrency.Id)
            {
                var rateToMainCurrency = GetCurrencyToMainCurrencyRate(sourceCurrency.Id, dateTime);

                var result = sourceValue * rateToMainCurrency;
                return decimal.Round(result, 0, MidpointRounding.AwayFromZero);
                //return result;
            }
            else
            {
                var rateFromSourceToMainCurrency = GetCurrencyToMainCurrencyRate(sourceCurrency.Id, dateTime);
                var rateFromDestinationToMainCurrency = GetCurrencyToMainCurrencyRate(destinationCurrency.Id, dateTime);

                var result = sourceValue * rateFromSourceToMainCurrency / rateFromDestinationToMainCurrency;
                return decimal.Round(result, 2, MidpointRounding.AwayFromZero);
                //return result;
            }
        }

        public decimal ConvertPrice(decimal sourceValue, long sourceCurrencyId, long destinationCurrencyId, DateTime dateTime)
        {
            //return sourceValue;

            var mainCurrency = GetMainCurrency();

            if (sourceCurrencyId == mainCurrency.Id)
            {
                var rateToMainCurrency = GetCurrencyToMainCurrencyRate(destinationCurrencyId, dateTime);

                var result = sourceValue / rateToMainCurrency;
                return decimal.Round(result, 2, MidpointRounding.AwayFromZero);
                //return result;
            }
            else if (destinationCurrencyId == mainCurrency.Id)
            {
                var rateToMainCurrency = GetCurrencyToMainCurrencyRate(sourceCurrencyId, dateTime);

                var result = sourceValue * rateToMainCurrency;
                return decimal.Round(result, 0, MidpointRounding.AwayFromZero);
                //return result;
            }
            else
            {
                var rateFromSourceToMainCurrency = GetCurrencyToMainCurrencyRate(sourceCurrencyId, dateTime);
                var rateFromDestinationToMainCurrency = GetCurrencyToMainCurrencyRate(destinationCurrencyId, dateTime);

                var result = sourceValue * rateFromSourceToMainCurrency / rateFromDestinationToMainCurrency;
                return decimal.Round(result, 2, MidpointRounding.AwayFromZero);
                //return result;
            }
        }

        public Currency GetMainCurrency()
        {
            var rialCurrency = this.currencyFakeRepository.First(c => c.Abbreviation.ToUpper() == "IRR");

            if (rialCurrency == null)
                throw new ObjectNotFound("MainCurrency");

            return rialCurrency;
        }

        public decimal GetCurrencyValueInMainCurrency(Currency currency, decimal valueInCurrency, DateTime date)
        {
            var rate = GetCurrencyToMainCurrencyRate(currency.Id, date);
            return CalculateCurrencyToMainCurrencyWithRate(valueInCurrency, rate);
        }

        public decimal GetCurrencyToMainCurrencyRate(long currencyId, DateTime date)
        {
            //Dollar to MainCurrency 
            var currency = currencyFakeRepository.Single(c => c.Id == currencyId);
            if (currency.Abbreviation.ToLower() == "usd")
                return 12650;
            else if (currency.Abbreviation.ToLower() == "eur")
                return 30000;
            else
                return 1;
        }

        private decimal CalculateCurrencyToMainCurrencyWithRate(decimal valueInCurrency, decimal rate)
        {
            var result = valueInCurrency * rate;
            return decimal.Round(result, 2, MidpointRounding.AwayFromZero);
        }
    }
}
