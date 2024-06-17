using GaitAnalysis.Model;
using GaitAnalysis.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GaitAnalysis.Model.PatientsModel;
using static GaitAnalysis.Model.TestModel;

namespace GaitAnalysis.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageAnalysisResults : ContentPage
    {
        public PageAnalysisResults(Test TestPatient,Patient PatientPicked)
        {
            InitializeComponent();
            BindingContext = new VMpageAnalysisResults(Navigation, TestPatient, PatientPicked);
        }
    }
}