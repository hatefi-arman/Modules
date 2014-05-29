using System;
using System.IO;
using System.Windows;
using MITD.Core;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.StorageSpace.Presentation.Contracts.SL.Controllers;
namespace MITD.Main.Presentation.Logic.SL.Infrastructure
{
    public partial class StorageSpaceController : ApplicationController, IStorageSpaceController, IApplicationController
    {
        #region ctor

        public UserDto CurrentUser { get; set; }

        public StorageSpaceController(IViewManager viewManager, IEventPublisher eventPublisher,
                                   IDeploymentManagement deploymentManagement)
            : base(viewManager, eventPublisher, deploymentManagement)
        {


            CurrentUser = new UserDto();
            CurrentUser.Id = 1;
            // CurrentUser.Name = "Ali";
            CurrentUser.Code = "X100";
            CurrentUser.CompanyDto = new CompanyDto { Id = 1 };  //SAPID = 1, HAFIZ = 2, IMSENGCO = 3, IRISL = 4 
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

        public IViewManager ViewManager { get; set; }

        public IEventPublisher EventPublisher { get; set; }

        public IDeploymentManagement DeploymentManagement { get; set; }

        #endregion

        public void HandleException(Exception exp)
        {
            var exceptionMessageDto =
                Newtonsoft.Json.JsonConvert.DeserializeObject<ExceptionMessageDto>(exp.Data["error"].ToString());
            viewManager.ShowMessage(exceptionMessageDto.Message, this);
        }


        public void GetRemoteInstance<T>(Action<T, Exception> action) where T : class
        {
            deploymentManagement.AddModule(typeof(T),
                res => action(ServiceLocator.Current.GetInstance(typeof(T)) as T, null),
                exp => action(null, exp));
        }
    }
    public partial class FuelController : ApplicationController, IFuelController, IApplicationController
    {
        #region ctor

        public FuelController(IViewManager viewManager, IEventPublisher eventPublisher,
                                   IDeploymentManagement deploymentManagement)
            : base(viewManager, eventPublisher, deploymentManagement)
        {


            CurrentUser = new UserDto();
            CurrentUser.Id = 1;
            //CurrentUser.Name = "Ali";
            CurrentUser.Code = "X100";
            CurrentUser.CompanyDto = new CompanyDto { Id = 11, Name = "SAPID" };
            //CurrentUser.CompanyDto = new CompanyDto { Id = 10, Name = "IRISL" };

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
    }
}