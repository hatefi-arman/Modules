using System;
using System.Collections.Generic;
using System.Text;
using MITD.Fuel.Presentation.Contracts.SL.Infrastructure;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper;
using MITD.Presentation.Contracts;
using System.IO;

namespace MITD.Fuel.Presentation.FuelApp.Logic.SL.ServiceWrapper
{
    public class VoyageServiceWrapper : IVoyageServiceWrapper
    {
        private readonly string voyageAddressFormatString;
        private readonly string voyageLogAddressFormatString;

        public VoyageServiceWrapper()
        {
            voyageAddressFormatString = Path.Combine(ApiConfig.HostAddress, "apiarea/Fuel/Voyage/{0}");
            voyageLogAddressFormatString = string.Concat(voyageAddressFormatString, "/Log");
        }


        public void GetById(Action<VoyageDto, Exception> action, long id)
        {
            var url = string.Format(voyageAddressFormatString, id);

            WebClientHelper.Get(new Uri(url, UriKind.Absolute), action, WebClientHelper.MessageFormat.Json);
        }

        public void GetAll(Action<PageResultDto<VoyageDto>, Exception> action, int? pageSize, int? pageIndex)
        {
            var url = string.Format(voyageAddressFormatString, string.Empty);

            var sbUrl = new StringBuilder(url);

            if (pageSize.HasValue && pageIndex.HasValue)
            {
                sbUrl.Append(string.Concat("?pageSize=", pageSize));
                sbUrl.Append(string.Concat("&pageIndex=", pageIndex));
            }

            WebClientHelper.Get(new Uri(sbUrl.ToString(), UriKind.Absolute), action, WebClientHelper.MessageFormat.Json);
        }

        public void GetByFilter(Action<PageResultDto<VoyageDto>, Exception> action, long companyId, long vesselId, int? pageSize, int? pageIndex)
        {
            var url = string.Format(voyageAddressFormatString, string.Empty);

            var sbUrl = new StringBuilder(url);
            sbUrl.Append(string.Concat("?companyId=", companyId));
            sbUrl.Append(string.Concat("&vesselId=", vesselId));

            if (pageSize.HasValue && pageIndex.HasValue)
            {
                sbUrl.Append(string.Concat("&pageSize=", pageSize));
                sbUrl.Append(string.Concat("&pageIndex=", pageIndex));
            }

            WebClientHelper.Get(new Uri(sbUrl.ToString(), UriKind.Absolute), action, WebClientHelper.MessageFormat.Json);
        }

        public void GetChenageHistory(Action<PageResultDto<VoyageLogDto>, Exception> action, long voyageId, int pageSize, int pageIndex)
        {
            var url = string.Format(voyageLogAddressFormatString, voyageId);

            var sbUrl = new StringBuilder(url);
            sbUrl.Append(string.Concat("?pageSize=", pageSize));
            sbUrl.Append(string.Concat("&pageIndex=", pageIndex));


            WebClientHelper.Get(new Uri(sbUrl.ToString(), UriKind.Absolute), action, WebClientHelper.MessageFormat.Json);
        }
    }
}
