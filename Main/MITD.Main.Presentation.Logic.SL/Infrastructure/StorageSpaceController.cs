using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using MITD.Core;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.SL.Infrastructure;
using MITD.Main.Presentation.Logic.SL.ServiceWrapper;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using Newtonsoft.Json.Linq;

namespace MITD.Main.Presentation.Logic.SL.Infrastructure
{
    //public partial class StorageSpaceController : ApplicationController, IFuelController, IApplicationController
    //{
    //    #region ctor


    //    public StorageSpaceController(
    //        IViewManager viewManager,
    //        IEventPublisher eventPublisher,
    //        IDeploymentManagement deploymentManagement, IUserServiceWrapper userService, IUserProvider userProvider)
    //        : base(viewManager, eventPublisher, deploymentManagement)
    //    {


    //        CurrentUser = new UserDto();

    //        this.userService = userService;
    //        this.userProvider = userProvider;


    //        //viewManager
    //        if (viewManager == null)
    //            throw new Exception("ViewManager can not be null");

    //        this.ViewManager = viewManager;
    //        //eventPublisher
    //        if (eventPublisher == null)
    //            throw new Exception("eventPublisher can not be null");

    //        this.EventPublisher = eventPublisher;

    //        //deploymentManagement
    //        if (deploymentManagement == null)
    //            throw new Exception("deploymentManagement can not be null");

    //        this.DeploymentManagement = deploymentManagement;



    //    }

    //    #endregion

    //    #region props
    //    public UserDto CurrentUser { get; set; }

    //    private IUserServiceWrapper userService;

    //    private IUserProvider userProvider;
    //    public IViewManager ViewManager { get; set; }

    //    public IEventPublisher EventPublisher { get; set; }

    //    public IDeploymentManagement DeploymentManagement { get; set; }

    //    public UserStateDTO LoggedInUserState { get; set; }

    //    public UserStateDTO CurrentUserState { get; set; }

    //    #endregion

    //    public void HandleException(Exception exp)
    //    {
    //        var exceptionMessageDto =
    //            Newtonsoft.Json.JsonConvert.DeserializeObject<ExceptionMessageDto>(exp.Data["error"].ToString());
    //        viewManager.ShowMessage(exceptionMessageDto.Message, this);
    //    }


    //    public void GetRemoteInstance<T>(Action<T, Exception> action) where T : class
    //    {
    //        deploymentManagement.AddModule(typeof(T),
    //            res => action(ServiceLocator.Current.GetInstance(typeof(T)) as T, null),
    //            exp => action(null, exp));
    //    }


    //    public void Login(Action action)
    //    {
    //        ShowBusyIndicator("در حال ورود به سامانه...");
    //        this.userService.GetToken((res, exp) => BeginInvokeOnDispatcher(() =>
    //        {
    //            if (exp == null)
    //            {
    //                userProvider.SamlToken = res;
    //                getSessionToken(res, action);
    //            }
    //            else
    //            {
    //                HandleException(exp);
    //            }
    //        }));
    //    }

    //    private void getSessionToken(string token, Action action, string newCurrentWorkListUser = "")
    //    {
    //        userService.GetSessionToken((res, exp) => BeginInvokeOnDispatcher(() =>
    //        {
    //            if (exp == null)
    //            {
    //                var json = JObject.Parse(res);
    //                var sessionToken = json["access_token"].ToString();
    //                var expiresIn = int.Parse(json["expires_in"].ToString());
    //                var expiration = DateTime.UtcNow.AddSeconds(expiresIn);
    //                userProvider.Token = sessionToken;
    //                getLogonUser();
    //                action();
    //            }
    //            else
    //            {
    //                HideBusyIndicator();
    //                HandleException(exp);
    //            }
    //        }), token, newCurrentWorkListUser);
    //    }


    //    private void getLogonUser()
    //    {
    //        userService.GetLogonUser((res, exp) =>
    //        {
    //            if (exp == null)
    //            {
    //                BeginInvokeOnDispatcher(() =>
    //                {
    //                    CurrentUserState = res;
    //                    LoggedInUserState = res;
    //                    Publish(new MainWindowUpdateArgs());

    //                });
    //                //createCustomFieldEntityList();
    //            }
    //            else
    //            {
    //                HideBusyIndicator();
    //                HandleException(exp);
    //            }
    //        });
    //    }



    //    public UserDto GetCurrentUser()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    public partial class FuelController : ApplicationController, IFuelController, IApplicationController
    {
        #region ctor

     
        public FuelController(
            IViewManager viewManager,
            IEventPublisher eventPublisher,
            IDeploymentManagement deploymentManagement, IUserServiceWrapper userService, IUserProvider userProvider)
            : base(viewManager, eventPublisher, deploymentManagement)
        {

            //TODO: All the User DTO Codes must be removed. <A.H>
            CurrentUser = new UserDto();
            CurrentUser.Id = 1101;
            CurrentUser.CompanyDto = new CompanyDto() {Id = 11, Code = "SAPID", Name = "SAPID"};

            this.userService = userService;
            this.userProvider = userProvider;

            //viewManager
            if (viewManager == null)
                throw new Exception("ViewManager can not be null");

            this.ViewManager = viewManager;
            //eventPublisher
            if (eventPublisher == null)
                throw new Exception("eventPublisher can not be null");

            this.EventPublisher = eventPublisher;

            //deploymentManagement
            if (deploymentManagement == null)
                throw new Exception("deploymentManagement can not be null");

            this.DeploymentManagement = deploymentManagement;



        }

        #endregion

        #region props

        public UserDto CurrentUser { get; set; }

        private IUserServiceWrapper userService;

        private IUserProvider userProvider;
        public UserStateDTO LoggedInUserState { get; set; }

        public UserStateDTO CurrentUserState { get; set; }
        public IViewManager ViewManager { get; set; }

        public IEventPublisher EventPublisher { get; set; }

        public IDeploymentManagement DeploymentManagement { get; set; }

        #endregion

        public UserDto GetCurrentUser()
        {
            return CurrentUser;
        }



        public void HandleException(Exception exp)
        {
            //TODO: Fake handle exception
            viewManager.ShowMessage(exp.ToString(), this);
            return;

            var exceptionMessageDto =
                Newtonsoft.Json.JsonConvert.DeserializeObject<ExceptionMessageDto>(exp.Data["error"].ToString());
            viewManager.ShowMessage(exceptionMessageDto.Message, this);
        }


        public void GetRemoteInstance<T>(Action<T, Exception> action) where T : class
        {
            deploymentManagement.AddModule(typeof(T),
                res =>
                {
                    action(ServiceLocator.Current.GetInstance(typeof(T)) as T, null);
                },
                exp => { action(null, exp); });
        }

        public void Login(Action action)
        {
            ShowBusyIndicator("در حال ورود به سامانه...");
            this.userService.GetToken((res, exp) => BeginInvokeOnDispatcher(() =>
            {
                if (exp == null)
                {
                    userProvider.SamlToken = res;
                    getSessionToken(res, action);
                }
                else
                {
                    HandleException(exp);
                }
            }));
        }

        private void getSessionToken(string token, Action action, string newCurrentWorkListUser = "")
        {
            userService.GetSessionToken((res, exp) => BeginInvokeOnDispatcher(() =>
            {
                if (exp == null)
                {
                    var json = JObject.Parse(res);
                    var sessionToken = json["access_token"].ToString();
                    var expiresIn = int.Parse(json["expires_in"].ToString());
                    var expiration = DateTime.UtcNow.AddSeconds(expiresIn);
                    userProvider.Token = sessionToken;
                    ApiConfig.Headers = ApiConfig.CreateHeaderDic(userProvider.Token);
                    getLogonUser();
                    action();
                }
                else
                {
                    HideBusyIndicator();
                    HandleException(exp);
                }
            }), token, newCurrentWorkListUser);
        }


        private void getLogonUser()
        {
            userService.GetLogonUser((res, exp) =>
            {
                if (exp == null)
                {
                    BeginInvokeOnDispatcher(() =>
                    {
                        CurrentUserState = res;
                        LoggedInUserState = res;
                        Publish(new MainWindowUpdateArgs());

                    });
                    //createCustomFieldEntityList();
                }
                else
                {
                    HideBusyIndicator();
                    HandleException(exp);
                }
            });
        }
    }
}