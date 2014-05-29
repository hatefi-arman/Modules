using MITD.Core;
using MITD.Fuel.Presentation.FuelApp.Logic.SL.ViewModels;
using MITD.Fuel.Presentation.Logic.SL.Controllers;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Fuel.Presentation.Contracts.SL.Views;

namespace MITD.Fuel.Presentation.FuelApp.Logic.SL.Controllers
{
    public class FuelReportController : BaseController, IFuelReportController
    {

        public FuelReportController(IViewManager viewManager, IEventPublisher eventPublisher, IDeploymentManagement deploymentManagement)
            : base(viewManager, eventPublisher, deploymentManagement)
        {

        }

        public void Add()
        {
            // ViewManager.ShowInTabControl<IFuelReportView>();
            ViewManager.ShowInDialog<IFuelReportView>();
        }

        public void Edit(FuelReportDto dto)
        {

            var view = ViewManager.ShowInDialog<IFuelReportView>();
            (view.ViewModel as FuelReportVM).Load(dto);
        }

        public void ShowList()
        {
            var view = this.ViewManager.ShowInTabControl<IFuelReportListView>();
            (view.ViewModel as FuelReportListVM).Load();
        }

    }
}
