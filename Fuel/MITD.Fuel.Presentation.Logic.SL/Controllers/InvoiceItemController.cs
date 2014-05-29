using System.Collections.Generic;
using MITD.Core;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Fuel.Presentation.Contracts.SL.Views;
using MITD.Fuel.Presentation.Logic.SL.ViewModels;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.DTOs;

namespace MITD.Fuel.Presentation.Logic.SL.Controllers
{
    public class InvoiceItemController :BaseController, IInvoiceItemController
    {
        public InvoiceItemController(IViewManager viewManager, IEventPublisher eventPublisher, IDeploymentManagement deploymentManagement) : 
            base(viewManager, eventPublisher, deploymentManagement)
        {
        }

      

//        public void Edit(InvoiceItemDto dto, DivisionMethodEnum divisionMethod)
//        {
//            var view = ViewManager.ShowInDialog<IInvoiceItemView>();
//           (view.ViewModel as InvoiceItemVM).Load(dto,divisionMethod, TODO);
//        }

//        public void ShowList()
//        {
//          
//            var view =this.ViewManager.ShowInTabControl<IInvoiceItemListView>();
//           
//              
//                  (view.ViewModel as InvoiceItemListVM).Load();
//           
//        }

    }
}
