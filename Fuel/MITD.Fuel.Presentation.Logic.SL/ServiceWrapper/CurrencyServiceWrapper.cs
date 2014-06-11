#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.SL.Infrastructure;
using MITD.Fuel.Presentation.Logic.SL.Infrastructure;
using MITD.Presentation;
using MITD.Presentation.Contracts;
using ApiServiceAddressHelper = MITD.Fuel.Presentation.Logic.SL.Infrastructure.ApiServiceAddressHelper;

#endregion

namespace MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper
{
    public class CurrencyServiceWrapper : ICurrencyServiceWrapper
    {
        private readonly string currencyAddressFormatString;


        public CurrencyServiceWrapper()
        {
            currencyAddressFormatString = Path.Combine(ApiConfig.HostAddress, "apiarea/Fuel/Currency/{0}");
        }

        #region ICurrencyServiceWrapper Members

        public void GetById(Action<CurrencyDto, Exception> action, long id)
        {
            var url = string.Format(currencyAddressFormatString, id);

            WebClientHelper.Get(new Uri(url, UriKind.Absolute), action, WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        public void GetAllCurrency(Action<List<CurrencyDto>, Exception> action)
        {
            var url = string.Format(currencyAddressFormatString, string.Empty);

            WebClientHelper.Get(new Uri(url, UriKind.Absolute), action, WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        //decimal GetCurrencyValueInMainCurrency(long currencyId, decimal value);
        public void GetCurrencyValueInMainCurrency(Action<decimal, Exception> action, long sourceCurrencyId, decimal value)
        {
            string url = string.Format(currencyAddressFormatString, string.Empty);
            var sb = new StringBuilder(url);
            sb.Append(string.Concat("?sourceCurrencyId=", sourceCurrencyId));
            sb.Append(string.Concat("&value=", value));

            WebClientHelper.Get(new Uri(sb.ToString(), UriKind.Absolute), action, WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        public void GetCurrencyValueInMainCurrency(Action<decimal, Exception> action, long sourceCurrencyId, decimal value, DateTime dateTime)
        {
            string url = string.Format(currencyAddressFormatString, string.Empty);
            var sb = new StringBuilder(url);
            sb.Append(string.Concat("?sourceCurrencyId=", sourceCurrencyId));
            sb.Append(string.Concat("&value=", value));
            sb.Append(string.Concat("&dateTime=", HttpUtil.DateTimeToString(dateTime)));

            WebClientHelper.Get(ApiServiceAddressHelper.BuildUri(sb), action, WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        public void ConvertPrice(Action<decimal, Exception> action, decimal value, long sourceCurrencyId, long destinationCurrencyId, DateTime dateTime)
        {
            string url = string.Format(currencyAddressFormatString, string.Empty);
            var sb = new StringBuilder(url);
            sb.Append(string.Concat("?value=", value));
            sb.Append(string.Concat("&sourceCurrencyId=", sourceCurrencyId));
            sb.Append(string.Concat("&destinationCurrencyId=", destinationCurrencyId));
            sb.Append(string.Concat("&dateTime=", HttpUtil.DateTimeToString(dateTime)));

            WebClientHelper.Get(ApiServiceAddressHelper.BuildUri(sb), action, WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        #endregion
    }
}