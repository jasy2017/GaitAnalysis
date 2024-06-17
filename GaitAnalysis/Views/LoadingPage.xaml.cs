using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GaitAnalysis.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GaitAnalysis.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPage : ContentPage
    {
        public LoadingPage()
        {
            InitializeComponent();
            BindingContext = new VMpageAddPatients(Navigation);
        }
    }
}