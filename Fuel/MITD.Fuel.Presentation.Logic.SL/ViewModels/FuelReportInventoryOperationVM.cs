using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Logic.SL.ViewModels;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Fuel.Presentation.Contracts.SL.Events;
using MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper;
using MITD.Fuel.Presentation.Contracts.SL.Views;

namespace MITD.Fuel.Presentation.FuelApp.Logic.SL.ViewModels
{
    public class FuelReportInventoryOperationVM : WorkspaceViewModel
    {
        #region properties

        private IFuelController mainController;

        private FuelReportInventoryOperationDto _entity;
        public FuelReportInventoryOperationDto Entity
        {
            get { return _entity; }
            private  set
            {
                this.SetField(p => p.Entity, ref _entity, value);
            }
        }

        #endregion

        #region ctor

        public FuelReportInventoryOperationVM()
        {
            Entity = new FuelReportInventoryOperationDto() { Id = -1 };
        }

        public FuelReportInventoryOperationVM(IFuelController mainController)
            : this()
        {
            this.mainController = mainController;

            this.RequestClose += FuelReportVM_RequestClose;
        }

        #endregion

        #region methods

        
        public void SetMainController(IFuelController fuelController)
        {
            this.mainController = fuelController;
        }

        void FuelReportVM_RequestClose(object sender, EventArgs e)
        {
            this.mainController.Close(this);
        }

        public void SetEntity(FuelReportInventoryOperationDto entity)
        {
            this.Entity = entity;
        }

        #endregion



    }
}
