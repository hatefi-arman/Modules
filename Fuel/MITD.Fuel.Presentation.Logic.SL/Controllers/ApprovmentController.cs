using MITD.Core;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Fuel.Presentation.Contracts.SL.Views;
using MITD.Fuel.Presentation.Logic.SL.ViewModels;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.DTOs;

namespace MITD.Fuel.Presentation.Logic.SL.Controllers
{
    public class ApprovmentController :BaseController, IApprovmentController
    {
        public ApprovmentController(IViewManager viewManager, IEventPublisher eventPublisher, IDeploymentManagement deploymentManagement) : base(viewManager, eventPublisher, deploymentManagement)
        {
        }

        //public void Add()
        //{
        //    ViewManager.ShowInTabControl<IApprovmentView>();
        //}

        //public void Edit(ApprovmentDto dto)
        //{
        //    var view = ViewManager.ShowInTabControl<IApprovmentView>();
        //    (view.ViewModel as ApprovmentVM).Load(dto);
        //}

        //public void ShowList()
        //{
        //    var view =this.ViewManager.ShowInTabControl<IApprovmentListView>();
        //    //(view.ViewModel as ApprovmentListVM).Load();
        //}

    }
}
