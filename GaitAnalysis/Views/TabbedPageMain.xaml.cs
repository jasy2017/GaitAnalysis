﻿using GaitAnalysis.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GaitAnalysis.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedPageMain : TabbedPage
    {
        public TabbedPageMain()
        {
            InitializeComponent();
            BindingContext = new VMpageAnalysis(Navigation);
        }
    }
}