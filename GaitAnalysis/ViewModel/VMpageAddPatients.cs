using Plugin.Media.Abstractions;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using static GaitAnalysis.Model.PatientsModel;
using System.Collections.ObjectModel;
using System.Linq;
using GaitAnalysis.Views;
using GaitAnalysis.Model;


namespace GaitAnalysis.ViewModel
{
    public class VMpageAddPatients : BaseViewModel
    {
        #region Variables
        public Patient _newPatient; //Variable para la clase Patient

        public ObservableCollection<Patient> GetListPatients { get; set; } //Lista de clases
        #endregion
        #region Constructor
        public VMpageAddPatients(INavigation navigation)
        {
            Navigation = navigation;
            Patient = new Patient();
            GetListPatients = new ObservableCollection<Patient>();
            ListPatients();
        }
        #endregion
        #region Objetos
        public Patient Patient
        {
            get { return _newPatient; }
            set
            {
                SetValue(ref _newPatient, value);
            }
        }
        #endregion
        #region Procesos
        public async Task AddPatient()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/patients";
                var patientJson = JsonConvert.SerializeObject(Patient);
                Console.WriteLine(patientJson);
                var content = new StringContent(patientJson, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);
                

            }
            MessagingCenter.Send(this, "UpdateList");
            await Navigation.PopAsync();
        }
        public async void ListPatients()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/patients";
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var patientSQL = JsonConvert.DeserializeObject<ObservableCollection<Patient>>(json);
                    GetListPatients.Clear();
                    foreach (var patient in patientSQL)
                    {
                        GetListPatients.Add(patient);
                    }
                }
            }
        }
        public async Task CancelAddPatient() //Vuelve a la página anterior
        {
            await Navigation.PopAsync();
        }
        #endregion
        #region Comandos
        public ICommand AddPatientCommand => new Command(async () => await AddPatient());
        public ICommand CancelAddPatientCommand => new Command(async () => await CancelAddPatient());
        #endregion
    }
}