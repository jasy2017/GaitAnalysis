using GaitAnalysis.Model;
using GaitAnalysis.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static GaitAnalysis.Model.PatientsModel;
using static GaitAnalysis.Model.TestModel;
using static GaitAnalysis.Model.TestResultsModel;

namespace GaitAnalysis.ViewModel
{
    public class VMpageTestResults: BaseViewModel
    {
        #region Variables
        private int _testpatientId; //variable Int Idpaciente que recibe de VMpagePatients
        public Patient _pickPatient; //Variable para la clase Patient
        public Test _test; //Variable para la clase Patient
        public ObservableCollection<Patient> GetListPatients { get; set; } //Lista de clases de pacientes
        public ObservableCollection<Test> GetListTest { get; set; } //Lista de clases de test
        public ObservableCollection<TestResults> GetListTestResult { get; set; } //Lista de clases de test
        #endregion
        #region Constructor
        public VMpageTestResults(INavigation navigation, int patientId)
        {
            Navigation = navigation;
            _testpatientId = patientId;
            PickPatients = new Patient();
            TestPatient = new Test();
            GetListTest = new ObservableCollection<Test>();
            ListTest();
            //ProcessAsync();
        }
        #endregion
        #region Objetos
        public Test TestPatient
        {
            get { return _test; }
            set
            {
                SetValue(ref _test, value);
            }
        }
        public Patient PickPatients
        {
            get { return _pickPatient; }
            set
            {
                SetValue(ref _pickPatient, value);
            }
        }
        #endregion
        #region Procesos

        public async void ListTest()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/test/list/{_testpatientId}";
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);
                    var testSQL = JsonConvert.DeserializeObject<List<Test>>(json);
                    foreach (var test in testSQL)
                    {
                        GetListTest.Add(test);
                    }
                }
            }
        }
        public async Task ComeBack()
        {
            await Navigation.PopAsync();
            //await Navigation.PushAsync(new PageAnalysisResults(TestPatient, PickPatients));
        }
        public async Task DeleteTest(int testid)
        {
            Console.WriteLine(testid);
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/test/{testid}";
                await client.DeleteAsync(apiUrl);

                foreach (var test in GetListTest)
                {
                    if (test.IdTest == testid)
                    {
                        GetListTest.Remove(test);
                        break;
                    }
                }
            }
        }
        public async Task NavigateToAnlysisResults(int testid)
        {
            foreach (var test in GetListTest)
            {
                if (test.IdTest == testid)
                {
                    TestPatient = test;
                    break;
                }
            }
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/patients/{_testpatientId}";
                var response=await client.GetAsync(apiUrl);
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine(json);
                PickPatients = JsonConvert.DeserializeObject<Patient>(json);
            };
            await Navigation.PushAsync(new PageAnalysisResults(TestPatient, PickPatients));//enviar test y paciente
        }
        
        #endregion
        #region Comandos
        public ICommand ComeBackCommand => new Command(async () => await ComeBack());

        public ICommand DeleteTestCommand => new Command<int>(async (testid) => await DeleteTest(testid));
        public ICommand NavigateToAnlysisResultsCommand => new Command<int>(async (testid) => await NavigateToAnlysisResults(testid));
        #endregion
    }
}
