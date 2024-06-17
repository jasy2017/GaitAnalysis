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
    public partial class PageAddPatients : ContentPage
    {
        public PageAddPatients()
        {
            InitializeComponent();
            // Comunicación entre la vista y el ViewModel
            BindingContext = new VMpageAddPatients(Navigation);
        }
       
    }
}