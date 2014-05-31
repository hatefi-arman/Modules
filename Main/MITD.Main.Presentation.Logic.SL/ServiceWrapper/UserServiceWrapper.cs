﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using MITD.Core;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.SL.Infrastructure;
using System.IO;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Presentation.Contracts;
using MITD.Fuel.Presentation.Contracts;

namespace MITD.Main.Presentation.Logic.SL.ServiceWrapper
{
    public partial class UserServiceWrapper : IUserServiceWrapper
    {
        private readonly IUserProvider userProvider;
        private readonly string baseAddressUsers = Path.Combine(ApiConfig.HostAddress, "Users");
        private readonly string baseAddressUserGroups = Path.Combine(ApiConfig.HostAddress, "UserGroups");
        private readonly string baseAddressActionTypes = Path.Combine(ApiConfig.HostAddress, "ActionTypes");

       

        public UserServiceWrapper(IUserProvider userProvider)
        {
            this.userProvider = userProvider;
        }

        public void GetAllUsers(Action<PageResultDto<UserDTOWithActions>, Exception> action, int pageSize, int pageIndex, Dictionary<string, string> sortBy, UserCriteria criteria)
        {
            var url = baseAddressUsers + "?PageSize=" + pageSize + "&PageIndex=" + pageIndex + getFilterUser(criteria);
            if (sortBy.Count > 0)
                url += "&SortBy=" + QueryConditionHelper.GetSortByQueryString(sortBy);
            WebClientHelper.Get(new Uri(url, UriKind.Absolute), action, MITD.Presentation.WebClientHelper.MessageFormat.Json, ApiConfig.CreateHeaderDic(userProvider.Token));

        }

        public void GetAllUsers(Action<List<UserDto>, Exception> action)
        {
            var url = baseAddressUsers;
            WebClientHelper.Get(new Uri(url, UriKind.Absolute), action, MITD.Presentation.WebClientHelper.MessageFormat.Json, ApiConfig.CreateHeaderDic(userProvider.Token));
        }

        private string getFilterUser(UserCriteria userCriteria)
        {
            if (userCriteria == null)
                return string.Empty;

            var qs = string.Empty;

            if (!string.IsNullOrEmpty(userCriteria.Fname))
                qs += "FirstName:" + userCriteria.Fname + ";";

            if (!string.IsNullOrEmpty(userCriteria.Lname))
                qs += "LastName:" + userCriteria.Lname + ";";

            if (!string.IsNullOrEmpty(userCriteria.PartyName))
                qs += "PartyName:" + userCriteria.PartyName + ";";

            if (string.IsNullOrEmpty(qs))
                return string.Empty;

            return "&Filter=" + qs;

        }

        public void GetUser(Action<UserDto, Exception> action, string username)
        {
            var url = string.Format(baseAddressUsers + "?partyName=" + username);
            WebClientHelper.Get(new Uri(url, UriKind.Absolute), action, MITD.Presentation.WebClientHelper.MessageFormat.Json, ApiConfig.CreateHeaderDic(userProvider.Token));
        }

        public void AddUser(Action<UserDto, Exception> action, UserDto userDto)
        {
            var url = string.Format(baseAddressUsers);
            WebClientHelper.Post(new Uri(url, UriKind.Absolute), action, userDto, MITD.Presentation.WebClientHelper.MessageFormat.Json, ApiConfig.CreateHeaderDic(userProvider.Token));
        }

        public void UpdateUser(Action<UserDto, Exception> action, UserDto userDto)
        {
            var url = string.Format(baseAddressUsers + "?partyname=" + userDto.PartyName);
            WebClientHelper.Put(new Uri(url, UriKind.Absolute), action, userDto, MITD.Presentation.WebClientHelper.MessageFormat.Json, ApiConfig.CreateHeaderDic(userProvider.Token));
        }

        public void DeleteUser(Action<string, Exception> action, string username)
        {
            var url = string.Format(baseAddressUsers + "?partyName=" + username);
            WebClientHelper.Delete(new Uri(url, UriKind.Absolute), action, ApiConfig.CreateHeaderDic(userProvider.Token));
        }

        public void GetAllUserGroups(Action<List<UserGroupDtoWithActions>, Exception> action)
        {
            WebClientHelper.Get(new Uri(baseAddressUserGroups, UriKind.Absolute), action, MITD.Presentation.WebClientHelper.MessageFormat.Json, ApiConfig.CreateHeaderDic(userProvider.Token));
        }

        public void GetUserGroup(Action<UserGroupDto, Exception> action, string groupName)
        {
            var url = string.Format(baseAddressUserGroups + "?partyName=" + groupName);
            WebClientHelper.Get(new Uri(url, UriKind.Absolute), action, MITD.Presentation.WebClientHelper.MessageFormat.Json, ApiConfig.CreateHeaderDic(userProvider.Token));
        }

        public void AddUserGroup(Action<UserGroupDto, Exception> action, UserGroupDto userGroupDto)
        {
            var url = string.Format(baseAddressUserGroups);
            WebClientHelper.Post(new Uri(url, UriKind.Absolute), action, userGroupDto, MITD.Presentation.WebClientHelper.MessageFormat.Json, ApiConfig.CreateHeaderDic(userProvider.Token));
        }

        public void UpdateUserGroup(Action<UserGroupDto, Exception> action, UserGroupDto userGroupDto)
        {
            var url = string.Format(baseAddressUserGroups + "?partyname=" + userGroupDto.PartyName);
            WebClientHelper.Put(new Uri(url, UriKind.Absolute), action, userGroupDto, MITD.Presentation.WebClientHelper.MessageFormat.Json, ApiConfig.CreateHeaderDic(userProvider.Token));
        }

        public void DeleteUserGroup(Action<string, Exception> action, string groupName)
        {
            var url = string.Format(baseAddressUserGroups + "?partyName=" + groupName);
            WebClientHelper.Delete(new Uri(url, UriKind.Absolute), action, ApiConfig.CreateHeaderDic(userProvider.Token));
        }

        public void GetToken(Action<string, Exception> action, string userName, string password)
        {
            var url = string.Format(baseAddressUsers + "?userName=" + userName + "&password=" + password);
            WebClientHelper.GetString(new Uri(url, UriKind.Absolute), action);
        }

        public void GetSessionToken(Action<string, Exception> action, string token, string newCurrentWorkListUser)
        {
           /// var url = string.Format(ApiConfig.HostAddress + "/token");
            var url = string.Format("http://localhost:1890/api" + "/token");
            if (!string.IsNullOrWhiteSpace(newCurrentWorkListUser))
                url = url + "?CurrentWorkListUserName=" + newCurrentWorkListUser;
            WebClientHelper.GetString(new Uri(url, UriKind.Absolute), action,
                new Dictionary<string, string> { { "SilverLight", "1" }, { "Authorization", "SAML " + token } });
        }

        public void GetToken(Action<string, Exception> action)
        {
            var x = "/Security";
            var url = string.Format(x);
            var client = new WebClient();

            client.DownloadStringCompleted += (s, a) =>
            {
                try
                {
                    action(a.Result, a.Error);
                }
                catch (Exception ex)
                {
                    
                    throw;
                }
                
            };
            client.DownloadStringAsync(new Uri(url, UriKind.Relative));
        }

        public void GetLogonUser(Action<UserStateDTO, Exception> action)
        {
            var url = string.Format(baseAddressUsers);
            WebClientHelper.Get(new Uri(url, UriKind.Absolute), action, MITD.Presentation.WebClientHelper.MessageFormat.Json, ApiConfig.CreateHeaderDic(userProvider.Token));
        }

        public void LogoutUser(Action<string, Exception> action)
        {
            var url = string.Format(baseAddressUsers);
            WebClientHelper.Delete(new Uri(url, UriKind.Absolute), action, ApiConfig.CreateHeaderDic(userProvider.Token));
        }

        public void GetAllUserGroupsDescriptions(Action<List<UserGroupDescriptionDto>, Exception> action)
        {
            WebClientHelper.Get(new Uri(baseAddressUserGroups, UriKind.Absolute), action, MITD.Presentation.WebClientHelper.MessageFormat.Json, ApiConfig.CreateHeaderDic(userProvider.Token));
        }

        public void GetAllActionTypes(Action<List<ActionTypeDto>, Exception> action)
        {
            WebClientHelper.Get(new Uri(baseAddressActionTypes, UriKind.Absolute), action, MITD.Presentation.WebClientHelper.MessageFormat.Json, ApiConfig.CreateHeaderDic(userProvider.Token));
        }

        public void ChangeCurrentWorkListUserName(Action<string, Exception> action, string logonUserName, string currentPermittedUser)
        {
            var url = string.Format(baseAddressUsers + "/" + logonUserName + "/CurrentPermittedUser");
            WebClientHelper.Put(new Uri(url, UriKind.Absolute), action, currentPermittedUser, MITD.Presentation.WebClientHelper.MessageFormat.Json, ApiConfig.CreateHeaderDic(userProvider.Token));
        }

        public void GetAllUserDescriptions(Action<PageResultDto<UserDescriptionDTO>, Exception> action, int pageSize, int pageIndex, Dictionary<string, string> sortBy, UserCriteria criteria)
        {
            var url = baseAddressUsers + "?PageSize=" + pageSize + "&PageIndex=" + pageIndex + getFilterUser(criteria);
            if (sortBy.Count > 0)
                url += "&SortBy=" + QueryConditionHelper.GetSortByQueryString(sortBy);
            WebClientHelper.Get(new Uri(url, UriKind.Absolute), action, MITD.Presentation.WebClientHelper.MessageFormat.Json, ApiConfig.CreateHeaderDic(userProvider.Token));

        }
    }
}
