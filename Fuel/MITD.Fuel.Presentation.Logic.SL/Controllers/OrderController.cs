using System.Collections.Generic;
using MITD.Core;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Fuel.Presentation.Contracts.SL.Views;
using MITD.Fuel.Presentation.Logic.SL.ViewModels;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.DTOs;

namespace MITD.Fuel.Presentation.Logic.SL.Controllers
{
    public class OrderController :BaseController, IOrderController
    {
        public OrderController(IViewManager viewManager, IEventPublisher eventPublisher, IDeploymentManagement deploymentManagement) : base(viewManager, eventPublisher, deploymentManagement)
        {
        }

        public void Add(List<CompanyDto> dtos, List<VesselDto> vesselDtos)
        {
            var view = ViewManager.ShowInDialog<IOrderView>();
            (view.ViewModel as OrderVM).NewOrder(dtos, vesselDtos);
        }

        public void Edit(OrderDto dto, List<CompanyDto> dtos,List<VesselDto> vesselDtos)
        {
            var view = ViewManager.ShowInDialog<IOrderView>();
            (view.ViewModel as OrderVM).Load(dto, dtos, vesselDtos);
        }

        public void ShowList()
        {
           
            
            var view = this.ViewManager.ShowInTabControl<IOrderListView>();
            (view.ViewModel as OrderListVM).Load();
        }

    }
}
