using MITD.Core;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Fuel.Presentation.Contracts.SL.Views;
using MITD.Fuel.Presentation.Logic.SL.ViewModels;
using MITD.Presentation;

namespace MITD.Fuel.Presentation.Logic.SL.Controllers
{
    public class OffhireController : BaseController, IOffhireController
    {
        public OffhireController(IViewManager viewManager, IEventPublisher eventPublisher, IDeploymentManagement deploymentManagement)
            : base(viewManager, eventPublisher, deploymentManagement)
        {

        }

        public void ShowList()
        {
            var view = this.ViewManager.ShowInTabControl<IOffhireListView>();
            (view.ViewModel as OffhireListVM).Load();
        }

        public void ShowOffhireManagementSystemList()
        {
            var view = this.ViewManager.ShowInDialog<IOffhireManagementSystemListView>();
            (view.ViewModel as OffhireManagementSystemListVM).Load();
        }

        public void Import(long referenceNumber)
        {
            var view = this.ViewManager.ShowInDialog<IOffhireView>();
            (view.ViewModel as OffhireVM).Load(referenceNumber);
        }

        public void Edit(OffhireDto offhireDto)
        {
            var view = this.ViewManager.ShowInDialog<IOffhireView>();
            (view.ViewModel as OffhireVM).Edit(offhireDto);
        }

        public void EditOffhireDetail(OffhireDto offhireDto, OffhireDetailDto offhireDetailDto)
        {
            var view = this.ViewManager.ShowInDialog<IOffhireDetailView>();
            (view.ViewModel as OffhireDetailVM).Edit(offhireDto, offhireDetailDto);
        }
    }
}
