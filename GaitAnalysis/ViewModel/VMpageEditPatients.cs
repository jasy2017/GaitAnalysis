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
    public class VMpageEditPatients : BaseViewModel
    {
        #region Variables
        public Patient _newPatient; //Variable para la clase Patient
        private int _patientId; //variable Int Idpaciente que recibe de VMpagePatients
        public ObservableCollection<Patient> GetListPatients { get; set; } //Lista de clases
        #endregion
        #region Constructor
        public VMpageEditPatients(INavigation navigation, int patientId)//recibe un parámetro de tipo int que es el ID que envía VMpagePatients
        {
            Navigation = navigation;
            Patient = new Patient();
            GetListPatients = new ObservableCollection<Patient>();
            _patientId = patientId;//guarda el ID del paciente
            LoadPatientDetails();
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
        private async void LoadPatientDetails()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/patients/{_patientId}";
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);
                    Patient = JsonConvert.DeserializeObject<Patient>(json);
                }
            }
        }
        private async Task EditPatient()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/patients/{_patientId}";
                var patient = new
                {
                   
                    //id = _patientId,
                    name = Patient.Name,
                    lastname = Patient.Lastname,
                    age = Patient.Age,
                    weight = Patient.Weight,
                    height = Patient.Height
                };
                var json = JsonConvert.SerializeObject(patient);
                Console.WriteLine(json);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PutAsync(apiUrl, data);
                await Navigation.PopAsync();
            }
            
        }
        private async Task CancelEdition()
        {
            await Navigation.PopAsync();
        }
        #endregion
        #region Comandos
        public ICommand CancelEditionCommand => new Command(async () => await CancelEdition());
        public ICommand EditPatientCommand => new Command(async () => await EditPatient());
        #endregion
    }
}

