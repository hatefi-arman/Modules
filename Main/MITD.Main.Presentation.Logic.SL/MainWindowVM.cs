using System;
using System.Collections.ObjectModel;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Main.Presentation.Logic.SL.Infrastructure;

namespace MITD.Main.Presentation.Logic.SL
{
    public class MainWindowVM : WorkspaceViewModel
    {
        #region Fields
        ReadOnlyObservableCollection<CommandViewModel> commands;
        IFuelController controller;
        public UserStateDTO UserState { get; set; }



        #endregion // Fields

        #region Constructor

        public MainWindowVM()
        {
            DisplayName = "سامانه";

        }


        public MainWindowVM(IFuelController controller)//, IProductServiceWrapper service)
        {
            DisplayName = "سامانه";
            this.controller = controller;
            UserState = controller.CurrentUserState;
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
