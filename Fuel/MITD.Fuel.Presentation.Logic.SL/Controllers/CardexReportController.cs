using System;
using System.Windows;
using System.Windows.Browser;
using MITD.Core;
using MITD.Fuel.Presentation.FuelApp.Logic.SL.ViewModels;
using MITD.Fuel.Presentation.Logic.SL.Controllers;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Fuel.Presentation.Contracts.SL.Views;

namespace MITD.Fuel.Presentation.FuelApp.Logic.SL.Controllers
{
    public class CardexReportController : BaseController, ICardexReportController
    {

        public CardexReportController(IViewManager viewManager, IEventPublisher eventPublisher, IDeploymentManagement deploymentManagement)
            : base(viewManager, eventPublisher, deploymentManagement)
        {

        }

        public void ShowReport()
        {
            var windowOption = new HtmlPopupWindowOptions
            {
                Location = false,
                Menubar = false,
                Toolbar = false,
                Directories = false,
                Resizeable = true,
                Scrollbars = true,
                Status = false,
                Width = 1000,
                Height = 1000,
            };

            var hostingSiteBaseAddress = Application.Current.Host.Source.AbsoluteUri.Replace(Application.Current.Host.Source.AbsolutePath, string.Empty);

            var reportViewerPageUri = new Uri(hostingSiteBaseAddress + "/Reports/ReportViewer.aspx?ItemPath=MiniStock_Cardex", UriKind.Absolute);

            //System.Windows.MessageBox.Show(hostingSiteBaseAddress + "/Reports/ReportViewer.aspx?ItemPath=" + "MiniStock_Cardex");

            System.Windows.Browser.HtmlPage.PopupWindow(reportViewerPageUri, "_blank", windowOption);
        }
    }
}
