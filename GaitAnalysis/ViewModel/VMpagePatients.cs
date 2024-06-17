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
using System.Collections.Specialized;
using GaitAnalysis.Model;


namespace GaitAnalysis.ViewModel
{
    public class VMpagePatients : BaseViewModel
    {
        #region Variables
        public Patient _newPatient; //Variable para la clase Patient
        private string _searchText; //Variable para la búsqueda de pacientes
        public ObservableCollection<Patient> GetListPatients { get; set; } //Lista de clases
        public bool IsSearching { get; set; } //Variable para evaluar si se está buscando un paciente
        #endregion
        #region Constructor
        public VMpagePatients(INavigation navigation)
        {
            Navigation = navigation;
            Patient = new Patient();
            GetListPatients = new ObservableCollection<Patient>();
            //Actualizar la vista de la lista de pacientes
            MessagingCenter.Subscribe<VMpageAddPatients>(this, "UpdateList", (sender) =>
            {
                 ListPatients();
            });
            ListPatients();
        }
        #endregion
        #region Objetos
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                SetValue(ref _searchText, value);
                IsSearching = !string.IsNullOrEmpty(value); //Evalúa si el campo de búsqueda está vacío
                if (!IsSearching)
                {
                    ListPatients();//Si el campo de búsqueda está vacío, se listan todos los pacientes
                }
            }
        }
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

        public async Task ListPatients()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string apiUrl = $"{ApiConfig.BaseUrl}/patients";
                    Console.WriteLine(apiUrl);
                    var response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine(json);
                        var patientSQL = JsonConvert.DeserializeObject<ObservableCollection<Patient>>(json);
                        GetListPatients.Clear();
                        foreach (var patient in patientSQL)
                        {
                            GetListPatients.Add(patient);
                        
                        }
                    }
                    else Console.WriteLine("Mal");
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                    // Aquí puedes manejar el error, mostrar un mensaje al usuario, etc.
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    // Manejo general de errores
                }
            }
            Console.WriteLine("*********************************************************");
        }
        /*bool isAlreadyInList = false;
                        foreach (Patient consultPatient in GetListPatients)
                        {
                            if (patient.Id == consultPatient.Id)
                            {
                                isAlreadyInList = true;
                                break;
                            }
                        }
                        if (!isAlreadyInList)
                        {
                            GetListPatients.Add(patient);
                        }*/
        private async Task SearchPatients()
        {
            try
            { 
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/patients";
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    var patients = JsonConvert.DeserializeObject<List<Patient>>(json);
                    GetListPatients.Clear();//Se borra todo lo que contiene la lista para después llenarla con los datos de la búsqueda
                    foreach (var classpatient in patients)
                    {
                        if (classpatient.Name.Contains(SearchText))
                        {
                            GetListPatients.Add(classpatient);
                        }
                    }
                }

            }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                // Aquí puedes manejar el error, mostrar un mensaje al usuario, etc.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                // Manejo general de errores
            }
        }
        public async Task DeletePatient(int patientId)
        {
            try { 
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/patients/{patientId}";
                await client.DeleteAsync(apiUrl);

                //Patient patientToRemove = null; //Se inicializa una variable del tipo clase Patient en null
                foreach (var patient in GetListPatients)
                {
                    if (patient.Id == patientId)
                    {
                        //patientToRemove = patient;
                        GetListPatients.Remove(patient);
                        break;
                    }
                }
            }
        }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                    // Aquí puedes manejar el error, mostrar un mensaje al usuario, etc.
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    // Manejo general de errores
                }
        }
        public async Task NavigateToEdit(int patientIdNave)
        {
            await Navigation.PushAsync(new PageEditPatient(patientIdNave));
        }
        
        public async Task NavigateToAdd()
        {
            await Navigation.PushAsync(new PageAddPatients());
        }
        public async Task NavigateToTest( int patientIdTest)
        {
            await Navigation.PushAsync(new PageTestResults(patientIdTest));
        }
        #endregion
        #region Comandos
        public ICommand NavigateToTestCommand => new Command<int>(async (patientIdTest) => await NavigateToTest(patientIdTest));
        public ICommand NavigateToEditCommand => new Command<int>(async (patientIdNave) => await NavigateToEdit(patientIdNave));
        
        public ICommand NavigateToAddCommand => new Command(async () => await NavigateToAdd());
        public ICommand DeletePatientCommand => new Command<int>(async (patientId) => await DeletePatient(patientId));
        public ICommand SearchCommand => new Command(async () => await SearchPatients());
        #endregion
    }
}

