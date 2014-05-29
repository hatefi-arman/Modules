using System;
using System.Collections;
using System.Collections.Generic;
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
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.SL;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Main.Presentation.Logic.SL.Infrastructure;
using MITD.StorageSpace.Presentation.Contracts.SL.Controllers;

namespace MITD.Main.Presentation.Logic.SL
{
    public class MainWindowVM : WorkspaceViewModel
    {
        #region Fields
        ReadOnlyObservableCollection<CommandViewModel> commands;
        IStorageSpaceController controller;
        //  private IProductServiceWrapper service;
        #endregion // Fields

        #region Constructor

        public MainWindowVM()
        {
            DisplayName = "سامانه";
        }


        public MainWindowVM(IStorageSpaceController controller)//, IProductServiceWrapper service)
        {
            DisplayName = "سامانه";
            this.controller = controller;
            //this.service = service;
        }
        #endregion

        #region Commands
        public ReadOnlyObservableCollection<CommandViewModel> Commands
        {
            get
            {
                if (commands == null)
                {

                    commands = CreatMenu(); // new ReadOnlyObservableCollection<CommandViewModel>(createCommands());
                }
                return commands;
            }
        }

        public ReadOnlyObservableCollection<CommandViewModel> CreatMenu()
        {
            MenuHelper menuHelper = new MenuHelper(controller);
            /*menuHelper.AddMenuItem(new MenuItem<IStorageSpaceTypeController>("انواع انبار", "ShowList"));
            menuHelper.AddMenuItem(new MenuItem<IGoodTypeController>("انواع کالا", "ShowList"));
            menuHelper.AddMenuItem(new MenuItem<IUnitController>("واحدها", "ShowUnits"));
            menuHelper.AddMenuItem(new MenuItem<IEffectiveFactorController>("عوامل موثر", "ShowList"));
            menuHelper.AddMenuItem(new MenuItem<IGoodController>("کالاها", "ShowGoods"));
            menuHelper.AddMenuItem(new MenuItem<IReceiptTypeController>("انواع رسید", "ShowReceiptType"));
            menuHelper.AddMenuItem(new MenuItem<IBrandController>("برندها", "ShowList"));
            menuHelper.AddMenuItem(new MenuItem<IIssueTypeController>("انواع حواله", "ShowIssueType"));*/
            menuHelper.AddMenuItem(new MenuItem<ICharterController>("Charter In", "ShowCharterInList"));
            menuHelper.AddMenuItem(new MenuItem<IOrderController>("سفارشات", "ShowList"));
            menuHelper.AddMenuItem(new MenuItem<IFuelReportController>("گزارش سوخت", "ShowList"));
            menuHelper.AddMenuItem(new MenuItem<IInvoiceController>("صورت حساب", "ShowList"));
            menuHelper.AddMenuItem(new MenuItem<IVoyageController>("گزارش سفرها", "Show"));
            menuHelper.AddMenuItem(new MenuItem<IOffhireController>("Offhire", "ShowList"));
            menuHelper.AddMenuItem(new MenuItem<IScrapController>("Scrap", "ShowList"));
            menuHelper.AddMenuItem(new MenuItem<ICharterController>("Charter Out", "ShowCharterOutList"));
            return menuHelper.ExcuteMenu();
        }



        #endregion // Commands

        public DateTime TimeToLogOut
        {
            get { return DateTime.Now; }
        }

    }
}
