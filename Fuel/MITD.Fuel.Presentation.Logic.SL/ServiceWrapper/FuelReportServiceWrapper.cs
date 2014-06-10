using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using MITD.Fuel.Presentation.Contracts.SL.Infrastructure;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper;
using MITD.Fuel.Presentation.Logic.SL.Infrastructure;
using MITD.Presentation.Contracts;
using System.IO;

namespace MITD.Fuel.Presentation.FuelApp.Logic.SL.ServiceWrapper
{
    public class FuelReportServiceWrapper : IFuelReportServiceWrapper
    {
        #region fields

        private string fuelReportAddressFormatString;

        private string fuelReportDetailAddressFormatString;

        private string currencyAddressFormatString;

        #endregion

        #region methods

        public FuelReportServiceWrapper()
        {
            fuelReportAddressFormatString = Path.Combine(ApiConfig.HostAddress, "apiarea/Fuel/FuelReport/{0}");

            fuelReportDetailAddressFormatString = string.Concat(fuelReportAddressFormatString, "/Detail/{1}");

            currencyAddressFormatString = Path.Combine(ApiConfig.HostAddress, "apiarea/Fuel/Currency/{0}");
        }

        public void GetByFilter(Action<PageResultDto<FuelReportDto>, Exception> action, long companyId, long vesselId, int pageSize, int pageIndex)
        {
            var url = string.Format(fuelReportAddressFormatString, string.Empty);

            var sbUrl = new StringBuilder(url);
            sbUrl.Append(string.Concat("?companyId=", companyId));
            sbUrl.Append(string.Concat("&vesselId=", vesselId));
            sbUrl.Append(string.Concat("&pageSize=", pageSize));
            sbUrl.Append(string.Concat("&pageIndex=", pageIndex));

            WebClientHelper.Get<PageResultDto<FuelReportDto>>(new Uri(sbUrl.ToString(), UriKind.Absolute),
                                                                    (res, exp) => action(res, exp),
                                                                    WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        public void GetAll(Action<PageResultDto<FuelReportDto>, Exception> action, string methodName, int pageSize,
                           int pageIndex)
        {
            var url = string.Format(fuelReportAddressFormatString, string.Empty)
                + "?PageSize=" + pageSize + "&PageIndex=" + pageIndex;
            WebClientHelper.Get<PageResultDto<FuelReportDto>>(new Uri(url, UriKind.Absolute),
                                                                    (res, exp) => action(res, exp),
                                                                    WebClientHelper.MessageFormat.Json,ApiConfig.Headers
                );
        }

        public void GetById(Action<FuelReportDto, Exception> action, long id)
        {
            var url = string.Format(fuelReportAddressFormatString, id);
            WebClientHelper.Get<FuelReportDto>(new Uri(url, UriKind.Absolute),
                                                     (res, exp) => action(res, exp),
                                                     WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        public void Add(Action<FuelReportDto, Exception> action, FuelReportDto ent)
        {
            var url = string.Format(fuelReportAddressFormatString, string.Empty);

            WebClientHelper.Post<FuelReportDto, FuelReportDto>(new Uri(url, UriKind.Absolute),
                                                                           (res, exp) => action(res, exp), ent,
                                                                           WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        public void Update(Action<FuelReportDto, Exception> action, FuelReportDto ent)
        {
            var url = string.Format(fuelReportAddressFormatString, ent.Id);
         
            WebClientHelper.Put<FuelReportDto, FuelReportDto>(new Uri(url, UriKind.Absolute),
                                                                          (res, exp) => action(res, exp), ent,
                                                                          WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        public void Delete(Action<string, Exception> action, long id)
        {
            var url = string.Format(fuelReportAddressFormatString, id);
            WebClientHelper.Delete(new Uri(url, UriKind.Absolute), (res, exp) => action(res, exp));
        }

        public void UpdateFuelReportDetail(Action<FuelReportDetailDto, Exception> action, FuelReportDetailDto ent)
        {
            var url = string.Format(fuelReportDetailAddressFormatString, ent.FuelReportId, ent.Id);

            WebClientHelper.Put<FuelReportDetailDto, FuelReportDetailDto>(new Uri(url, UriKind.Absolute),
                                                                          (res, exp) => action(res, exp), ent,
                                                                          WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        public void GetAllCurrency(Action<List<CurrencyDto>, Exception> action)
        {
            var url = string.Format(currencyAddressFormatString, string.Empty);
            WebClientHelper.Get<List<CurrencyDto>>(new Uri(url, UriKind.Absolute),
                                                                    (res, exp) => action(res, exp),
                                                                    WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        #endregion
    }
}