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
using MITD.Fuel.Presentation.Contracts.SL.Views;
using MITD.Fuel.Presentation.Logic.SL.ViewModels;
using MITD.Presentation.UI;
using MITD.Presentation.UI.SL;

namespace MITD.Fuel.Presentation.UI.SL.Views
{
    public partial class OrderListView : ViewBase, IOrderListView
    {


        public OrderListView()
        {
            InitializeComponent();
        }
        public OrderListView(OrderListVM vm)
            : this()
        {

            ViewModel = vm;
            ViewModel.View = this;
            uxOrderItemListView.ViewModel = vm.OrderItemListVM;
            uxOrderItemListView.ViewModel.View = uxOrderItemListView;

        }

    }
}
