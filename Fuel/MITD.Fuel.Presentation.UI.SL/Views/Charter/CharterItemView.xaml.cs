﻿using System;
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

namespace MITD.Fuel.Presentation.UI.SL.Views
{
    public partial class CharterItemView : ViewBase, ICharterItemView
    {
        public CharterItemView()
        {
            InitializeComponent();
        }

        public CharterItemView(CharterItemVM vm):this()
        {
            this.ViewModel = vm;
            vm.View = this;


        }

    }
}
