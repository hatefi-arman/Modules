using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MITD.Presentation;
using MITD.Presentation.UI;
using MITD.Fuel.Presentation.FuelApp.Logic.SL.ViewModels;
using MITD.Fuel.Presentation.Contracts.SL.Views;

namespace MITD.Fuel.Presentation.FuelApp.UI.SL.Views
{
    public partial class FuelReportListView : ViewBase, IFuelReportListView
    {
        public FuelReportListView()
        {
            InitializeComponent();
        }

        public FuelReportListView(FuelReportListVM vm)
            : this()
        {

            ViewModel = vm;
            //ViewModel.View = this;
            uxFuelReportDetailListView.ViewModel = vm.FuelReportDetailListVm;
            uxFuelReportDetailListView.ViewModel.View = uxFuelReportDetailListView;

        }
    }
}
