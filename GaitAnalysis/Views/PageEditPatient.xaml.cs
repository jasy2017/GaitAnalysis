using GaitAnalysis.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static GaitAnalysis.Model.PatientsModel;

namespace GaitAnalysis.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PageEditPatient : ContentPage
	{
		public PageEditPatient (int Idpatient)
		{
			InitializeComponent ();
            BindingContext = new VMpageEditPatients(Navigation, Idpatient);
        }

    }
}