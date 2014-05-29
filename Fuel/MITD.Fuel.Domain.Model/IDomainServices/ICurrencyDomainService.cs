#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Domain.Model;
using MITD.Fuel.Domain.Model.DomainObjects;

#endregion

namespace MITD.Fuel.Domain.Model.IDomainServices
{
    public interface ICurrencyDomainService : IDomainService<Currency>
    {
        //Comment for check in.
        decimal ConvertPrice(decimal sourceValue, Currency sourceCurrency, Currency destinationCurrency, DateTime dateTime);
        decimal ConvertPrice(decimal sourceValue, long sourceCurrencyId, long destinationCurrencyId, DateTime dateTime);

        Currency GetMainCurrency();

        decimal GetCurrencyValueInMainCurrency(Currency currency, decimal valueInCurrency, DateTime date);
        decimal GetCurrencyToMainCurrencyRate(long currency, DateTime date);
    }
}