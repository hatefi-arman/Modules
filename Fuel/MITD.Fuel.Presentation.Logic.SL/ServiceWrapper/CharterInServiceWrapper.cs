﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.SL.Infrastructure;
using MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper;
using MITD.Presentation;
using MITD.Presentation.Contracts;

namespace MITD.Fuel.Presentation.Logic.SL.ServiceWrapper
{
    public class CharterInServiceWrapper : ICharterInServiceWrapper
    {

        #region Field

        private string hostCharterAddressController;
        private string hostCharterItemAddressController;

        private string hostCharterVesselAddressController;

        private string goodAddressController;

        private string currencyAddressFormatString;

        #endregion


        #region Method

        #region Ctor

        public CharterInServiceWrapper()
        {
            hostCharterAddressController = ApiConfig.HostAddress + "apiarea/Fuel/Charter/{0}";
            hostCharterItemAddressController = ApiConfig.HostAddress + "apiarea/Fuel/Charter/{0}/charterItem/{1}";

            hostCharterVesselAddressController = ApiConfig.HostAddress + "apiarea/Fuel/CharterVessel/{0}";

            goodAddressController = ApiConfig.HostAddress + "apiarea/Fuel/Good/{0}";

            currencyAddressFormatString = ApiConfig.HostAddress + "apiarea/Fuel/Currency/{0}";
        }
        #endregion


        public void GetByFilter(Action<PageResultDto<CharterDto>, Exception> action, long companyId, int pageIndex, int pageSize)
        {

             var uri = String.Format(hostCharterAddressController, string.Empty);
            StringBuilder stringBuilder = new StringBuilder(uri);
            stringBuilder.Append("?companyId=" + companyId);
            stringBuilder.Append("&pageIndex=" + pageIndex);
            stringBuilder.Append("&pageSize=" + pageSize);
            stringBuilder.Append("&charterType=" + CharterType.In);

            WebClientHelper.Get<PageResultDto<CharterDto>>(new Uri(stringBuilder.ToString(), UriKind.Absolute), action, WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        public void GetById(Action<CharterDto, Exception> action,CharterStateTypeEnum charterStateTypeEnum, long id)
        {
            string uri = string.Format(hostCharterAddressController,id);
            uri = String.Concat(uri, "?charterType=" + CharterType.In + "&charterStateTypeEnum=" + charterStateTypeEnum);
            WebClientHelper.Get<CharterDto>(new Uri(uri, UriKind.Absolute), action, WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        public void Add(Action<CharterDto, Exception> action, CharterDto charterDto)
        {
            var uri = String.Concat(hostCharterAddressController, "?charterType=" + CharterType.In);
            WebClientHelper.Post<CharterDto, CharterDto>(new Uri(uri, UriKind.Absolute), action, charterDto, WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        public void Update(Action<CharterDto, Exception> action, long id, CharterDto charterDto)
        {

            string uri = String.Concat(String.Format(hostCharterAddressController, id), "?charterType=" + CharterType.In);
            WebClientHelper.Put<CharterDto, CharterDto>(new Uri(uri, UriKind.Absolute), action, charterDto, WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        public void Delete(Action<string, Exception> action, long id)
        {
            string uri = String.Concat(String.Format(hostCharterAddressController, id), "?charterType=" + CharterType.In);
            WebClientHelper.Delete(new Uri(uri, UriKind.Absolute), action);
        }


        public void GetAllOwner(Action<PageResultDto<CompanyDto>, Exception> action)
        {
            throw new NotImplementedException();
        }

        public void GetAllIdelVessels(Action<PageResultDto<VesselDto>, Exception> action, long companyId)
        {

            var uri  = String.Format(hostCharterVesselAddressController, string.Empty);
            uri = String.Concat(uri, "?charterType=" + CharterType.In + "&companyId=" + companyId);
            WebClientHelper.Get<PageResultDto<VesselDto>>(new Uri(uri, UriKind.Absolute), action, WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        public void GetByIdVessel(Action<VesselDto, Exception> action, long id)
        {

            string uri = String.Concat(String.Format(hostCharterVesselAddressController, id), "?flag=true&charterType=" + CharterType.In);
            WebClientHelper.Get<VesselDto>(new Uri(uri, UriKind.Absolute), action, WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }


        public void GetItems(Action<PageResultDto<CharterItemDto>, Exception> action, CharterStateTypeEnum stateTypeEnum, long chrterId, int pageIndex, int pageSize)
        {
            string uri = String.Concat(String.Format(hostCharterItemAddressController, chrterId, string.Empty), "?charterType=" + CharterType.In);
            WebClientHelper.Get<PageResultDto<CharterItemDto>>(new Uri(uri, UriKind.Absolute), action, WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        public void GetItemById(Action<CharterItemDto, Exception> action, CharterStateTypeEnum stateTypeEnum, long id, long charerItemId)
        {
            string uri = String.Concat(String.Format(hostCharterItemAddressController, id, charerItemId), "?charterType=" + CharterType.In);
            WebClientHelper.Get<CharterItemDto>(new Uri(uri, UriKind.Absolute), action, WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        public void AddItem(Action<CharterItemDto, Exception> action, CharterStateTypeEnum stateTypeEnum, CharterItemDto charterDto)
        {

            var uri = String.Format(hostCharterItemAddressController, charterDto.CharterId, string.Empty);
            uri = String.Concat(uri, "?charterType=" + CharterType.In);
            WebClientHelper.Post<CharterItemDto, CharterItemDto>(new Uri(uri, UriKind.Absolute), action, charterDto, WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        public void UpdateItem(Action<CharterItemDto, Exception> action, long id, long charterItemId, CharterStateTypeEnum stateTypeEnum, CharterItemDto charterDto)
        {
            string uri = String.Concat(String.Format(hostCharterItemAddressController, id, charterItemId), "?charterType=" + CharterType.In);
            WebClientHelper.Put<CharterItemDto, CharterItemDto>(new Uri(uri, UriKind.Absolute), action, charterDto, WebClientHelper.MessageFormat.Json,ApiConfig.Headers);

        }

        public void DeleteItem(Action<string, Exception> action, CharterStateTypeEnum stateTypeEnum, long id, long charterItemId)
        {
            string uri = String.Concat(String.Format(hostCharterItemAddressController, id, charterItemId), "?charterType=" + CharterType.In);
            WebClientHelper.Delete(new Uri(uri, UriKind.Absolute), action);
        }



        public void GetAllGoods(Action<List<GoodDto>, Exception> action, long companyId)
        {
            var url = string.Format(goodAddressController, companyId);

            WebClientHelper.Get<List<GoodDto>>(new Uri(url, UriKind.Absolute), action, WebClientHelper.MessageFormat.Json,ApiConfig.Headers);


        }


        public void GetAllCurrencies(Action<List<CurrencyDto>, Exception> action)
        {

            WebClientHelper.Get<List<CurrencyDto>>(new Uri(string.Format(currencyAddressFormatString, string.Empty), UriKind.Absolute), action, WebClientHelper.MessageFormat.Json,ApiConfig.Headers);


        }



        #endregion
    }
}
