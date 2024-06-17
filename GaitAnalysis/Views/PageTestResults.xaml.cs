using GaitAnalysis.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GaitAnalysis.Model.PatientsModel;

namespace GaitAnalysis.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PageTestResults : ContentPage
    {
        public PageTestResults (int Idpatient)
        {
			InitializeComponent ();
            BindingContext = new VMpageTestResults(Navigation, Idpatient);
        }
	}
}