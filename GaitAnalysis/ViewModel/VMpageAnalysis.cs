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
using static GaitAnalysis.Model.TestModel;
using static GaitAnalysis.Model.TestResultsModel;
using System.Collections.ObjectModel;
using System.Linq;
using GaitAnalysis.Views;
using GaitAnalysis.Model;


namespace GaitAnalysis.ViewModel
{
    public class VMpageAnalysis : BaseViewModel
    {
        #region Variables
        public Patient _newPatient; //Variable para la clase Patient
        public Patient _pickPatient; //Variable para la clase Patient
        public Test _newTest; //Variable para la clase Test
        public TestResults _newTestResult; //Variable para la clase TestResuult
        public ObservableCollection<Patient> GetListPatients { get; set; } //Lista de clases de pacientes
        public ObservableCollection<Test> GetListTest { get; set; } //Lista de clases de test
        public ObservableCollection<TestResults> GetListTestResult { get; set; } //Lista de clases de test
        #endregion
        #region Constructor
        public VMpageAnalysis(INavigation navigation)
        {
            Navigation = navigation;
            Patient = new Patient();
            PickPatients = new Patient();
            TestPatient = new Test();
            TestResult = new TestResults();
            GetListPatients = new ObservableCollection<Patient>();
            GetListTest = new ObservableCollection<Test>();
            GetListTestResult = new ObservableCollection<TestResults>();
            MessagingCenter.Subscribe<VMpageAddPatients>(this, "UpdateList", (sender) =>
            {
                ListPatients();
            });
            ListPatients();
            ListTest();
            //ListTestResults();
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
        //paciente escogido por combobox
        public Patient PickPatients
        {
            get { return _pickPatient; }
            set
            {
                SetValue(ref _pickPatient, value);
            }
        }

        public Test TestPatient
        {
            get { return _newTest; }
            set
            {
                SetProperty(ref _newTest, value);
            }



        }
        public TestResults TestResult
        {
            get { return _newTestResult; }
            set
            {
                SetProperty(ref _newTestResult, value);
            }
        }
        #endregion
        #region Procesos
        public async Task ListPatients()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/patients";
                var response = await client.GetAsync(apiUrl);
                Console.WriteLine($"JSON Patients: {response}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"JSON Patients: {json}");
                    var patientSQL = JsonConvert.DeserializeObject<ObservableCollection<Patient>>(json);
                    
                    GetListPatients.Clear();
                    foreach (var patient in patientSQL)
                    {
                        GetListPatients.Add(patient);
                    }
                }
            }
        }
        public async void ListTest()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/test";
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);
                    var testSQL = JsonConvert.DeserializeObject<List<Test>>(json);
                    GetListTest.Clear();
                    foreach (var test in testSQL)
                    {
                        GetListTest.Add(test);
                    }
                }
            }
        }
        /*public async void ListTestResults()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "http://192.168.18.47:8000/testresult";
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var testresultSQL = JsonConvert.DeserializeObject<List<TestResults>>(json);
                    GetListTestResult.Clear();
                    foreach (var testresult in testresultSQL)
                    {
                        GetListTestResult.Add(testresult);
                    }
                }
            }
        }*/
        private async Task UploadVideo(MediaFile video)
        {
            IsLoading = true;
            try
            {

                using (var videoStream = video.GetStream())
                {
                    var memoryStream = new MemoryStream();
                    await videoStream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;//posiciona el puntero al inicio; ya que, después de la copia se encuentra al final

                    using (HttpClient client = new HttpClient())
                    {
                        string apiUrl = $"{ApiConfig.BaseUrl}/uploadvideo/{PickPatients.Id}";
                        var content = new MultipartFormDataContent();//datos binarios que se envían a través de http
                        //Console.WriteLine($"ID: {PickPatients.Id}");
                        content.Add(new StreamContent(memoryStream), "file", $"video_{PickPatients.Name}_.mp4");                              //permite enviar contenidoo binario en http
                        var response = await client.PostAsync(apiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Video uploaded successfully");
                            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Success", "Video uploaded successfully", "OK");

                            ListTest();//requiere actualizar la lista de test para que se muestre el nuevo video subido
                        }
                        else
                        {
                            Console.WriteLine("Failed to upload video");
                            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", "Failed to upload video", "OK");

                        }
                    }
                    memoryStream.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }
        public async Task RecordVideo()
        {
            StoreVideoOptions mediaOptions = new StoreVideoOptions()
            {
                DefaultCamera = CameraDevice.Rear,
                SaveToAlbum = true,
                CompressionQuality = 92,
                Quality = VideoQuality.High
            };
            var video = await Plugin.Media.CrossMedia.Current.TakeVideoAsync(mediaOptions);
            if (video != null)
            {
                await UploadVideo(video);
            }
            else
            {
                Console.WriteLine("No se ha grabado el video");
            }
        }
        public async Task PickVideoFromGallery()
        {
            if (!Plugin.Media.CrossMedia.Current.IsPickVideoSupported)
            {
                Console.WriteLine("La selección de video no está soportada");
                return;
            }
            var video = await Plugin.Media.CrossMedia.Current.PickVideoAsync();
            if (video != null)
            {
                await UploadVideo(video);
            }
            else
            {
                Console.WriteLine("No se ha seleccionado ningún video");
            }
        }
        public async Task ShowVideoOptions()
        {
            // Verifica si se ha seleccionado un paciente
            if (PickPatients == null || PickPatients.Id == 0)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Advertencia", "Por favor seleccione un paciente antes de continuar.", "OK");
                return;
            }
            string action = await Xamarin.Forms.Application.Current.MainPage.DisplayActionSheet("Video", "Cancel", null, "Grabar video", "Seleccionar video de la galería");
            switch (action)
            {
                case "Grabar video":
                    await RecordVideo();
                    break;
                case "Seleccionar video de la galería":
                    await PickVideoFromGallery();
                    break;
            }
        }

        
        public async Task<int> GetLastTestId() //debe ser asíncrono porque necesita esperar a que se ejecute la consulta
        {
            using (HttpClient client = new HttpClient())
            {
                string apiurl = $"{ApiConfig.BaseUrl}/test/{PickPatients.Id}";
                var response = await client.GetAsync(apiurl);
                var json = await response.Content.ReadAsStringAsync();
                var IdTest = int.Parse(json);
                return IdTest;
            }

        }
        public async Task<Test> GetPathVideo() //obtiene la clase que contiene el path del video 
        {
            //var idTest= GetLastTestId();
            Task<int> idTest = GetLastTestId();
            int idtest = await idTest;
            Test table_test = null;
            //foreach (var test in GetListTest)
            //{
            //    Console.WriteLine($"wasdcsdscwscvcswsc IdTest: {test.IdTest}, IdTest: {test.IdPatients}, PathVideo: {test.VideoPath}, ..."); 
            //}
            foreach (var test in GetListTest)
            {
                if (test.IdTest == idtest  )
                {
                    table_test= test;
                    break;
                }
            };
            return table_test;
        }
        public async Task GaitAnalysis()
        {
            // Verifica si se ha seleccionado un paciente
            if (PickPatients == null || PickPatients.Id == 0)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Advertencia", "Por favor seleccione un paciente antes de continuar.", "OK");
                return;
            }
            // Mostrar la pantalla de carga
            var loadingPage = new LoadingPage();
            await Navigation.PushAsync(loadingPage);

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    TestPatient = await GetPathVideo();
                    if (TestPatient == null)
                    {
                        Console.WriteLine("no encuentra último ID test");
                        await Navigation.PopAsync(); // Cerrar la pantalla de carga
                        return;
                    }

                    var json = JsonConvert.SerializeObject(TestPatient);
                    Console.WriteLine(json);
                    var clase = new StringContent(json, Encoding.UTF8, "application/json");
                    string apiurl = $"{ApiConfig.BaseUrl}/testresult/{TestPatient.IdTest}/{PickPatients.Weight}";
                    var response = await client.PostAsync(apiurl, clase);
                    if (response.IsSuccessStatusCode)
                    {
                        //await Navigation.PopAsync();
                        // Navegar a la página de resultados
                        await Navigation.PushAsync(new PageAnalysisResults(TestPatient, PickPatients));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    // Manejo de errores
                }
                //finally
                //{
                //    // Cerrar la pantalla de carga
                //    await Navigation.PopAsync();
                //}
            }
        }
        public async Task Prueba()
        {
           
            var loadingPage = new LoadingPage();
            await Navigation.PushAsync(loadingPage);
            //await Navigation.PopAsync();
   
        }
        public ICommand PruebaCommand => new Command(async () => await Prueba());
        #endregion
        #region Comandos
        public ICommand GaitAnalysisCommand => new Command(async () => await GaitAnalysis());
        public ICommand ShowVideoOptionsCommand => new Command(async () => await ShowVideoOptions());

        #endregion
    }
}
