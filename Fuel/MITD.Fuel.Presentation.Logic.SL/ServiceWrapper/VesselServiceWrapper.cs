using System;
using System.Collections.Generic;
using MITD.Fuel.Presentation.Contracts.SL.Infrastructure;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper;
using MITD.Presentation.Contracts;
using System.IO;

namespace MITD.Fuel.Presentation.Logic.SL.ServiceWrapper
{
    public class VesselServiceWrapper : IVesselServiceWrapper
    {
        #region fields

        private string vesselAddressFormatString = Path.Combine(ApiConfig.HostAddress, "apiarea/Fuel/Vessel/{0}");

        #endregion

        #region methods

        public void GetAll(Action<List<VesselDto>, Exception> action)
        {
            var url = string.Format(vesselAddressFormatString, string.Empty);

            WebClientHelper.Get<List<VesselDto>>(new Uri(url, UriKind.Absolute),
                                                                    (res, exp) => action(res, exp),
                                                                    WebClientHelper.MessageFormat.Json
                );
        }

        public void GetAll(Action<PageResultDto<VesselDto>, Exception> action, string methodName, int pageSize, int pageIndex)
        {
            var url = string.Format(vesselAddressFormatString, string.Empty)
                + "?PageSize=" + pageSize + "&PageIndex=" + pageIndex;

            WebClientHelper.Get<PageResultDto<VesselDto>>(new Uri(url, UriKind.Absolute),
                                                                    (res, exp) => action(res, exp),
                                                                    WebClientHelper.MessageFormat.Json
                );
        }

        public void GetById(Action<VesselDto, Exception> action, int id, bool includeCompany = true, bool includeTanks = false)
        {
            var url = string.Format(vesselAddressFormatString, id);

            url = url + "?includeCompany=" + includeCompany +
                "&includeTanks=" + includeTanks;

            WebClientHelper.Get<VesselDto>(new Uri(url, UriKind.Absolute),
                                                     action,
                                                     WebClientHelper.MessageFormat.Json);
        }

        public void GetPagedDataByFilter(Action<PageResultDto<VesselDto>, Exception> action, long companyId, int? pageSize, int? pageIndex)
        {
            var url = string.Format(vesselAddressFormatString, string.Empty);

            url += "?companyId=" + companyId;

            if (pageSize.HasValue && pageIndex.HasValue)
            {
                url += "&pageSize" + pageSize + "&pageIndex" + pageIndex;
            }

            WebClientHelper.Get(new Uri(url, UriKind.Absolute), action, WebClientHelper.MessageFormat.Json);
        }

        //public void Add(Action<VesselDto, Exception> action, VesselDto ent)
        //{
        //    var url = vesselAddressFormatString;// string.Concat(baseAddress, "/Post");
        //    WebClientHelper.Post<VesselDto, VesselDto>(new Uri(url, UriKind.Absolute),
        //                                                                   (res, exp) => action(res, exp), ent,
        //                                                                   WebClientHelper.MessageFormat.Json);
        //}

        //public void Update(Action<VesselDto, Exception> action, VesselDto ent)
        //{
        //    //var url = string.Concat(baseAddress, "/Put?Id=",ent.Id);
        //    var url = string.Concat(vesselAddressFormatString, ent.Id);
        //    WebClientHelper.Put<VesselDto, VesselDto>(new Uri(url, UriKind.Absolute),
        //                                                                  (res, exp) => action(res, exp), ent,
        //                                                                  WebClientHelper.MessageFormat.Json);
        //}

        //public void Delete(Action<string, Exception> action, int id)
        //{
        //    //var url = string.Concat(baseAddress, "/DeleteById?id=", id);
        //    var url = string.Concat(vesselAddressFormatString, id);
        //    WebClientHelper.Delete(new Uri(url, UriKind.Absolute), (res, exp) => action(res, exp));
        //}

        #endregion

    }
}
