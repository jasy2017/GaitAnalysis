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
using static GaitAnalysis.Model.TestDisplacementModel;
using static GaitAnalysis.Model.TestAngleModel;
using static GaitAnalysis.Model.TestLinearAccelerationModel;
using static GaitAnalysis.Model.TestLinearVelocityModel;
using static GaitAnalysis.Model.TestAngularAccelerationModel;
using static GaitAnalysis.Model.TestAngularVelocityModel;
using static GaitAnalysis.Model.TestMomentModel;
using static GaitAnalysis.Model.ModelCoordinates;
using static GaitAnalysis.Model.ModelAngles;
using static Xamarin.Essentials.Permissions;
using System.Linq;
using Syncfusion.SfChart.XForms;
using GaitAnalysis.Model;//para la conversión de string a lista de int

namespace GaitAnalysis.ViewModel
{
    public class VMpageAnalysisResults : BaseViewModel
    {
        #region Variables
        public Patient _patientPicked; //Variable para la clase Patient
        public Test _testPatient; //Variable para la clase Test
        public TestResults _newTestResult; //Variable para la clase TestResuult
        private int _idTestResults; //variable Int Idpaciente que recibe de VMpagePatients
        public Puntos _dataPoint; //Variable para la clase DataPoint

        public ObservableCollection<Patient> GetListPatients { get; set; } //Lista de clases de pacientes
        public ObservableCollection<Puntos> CaderaPoints { get; set; }
        public ObservableCollection<Angles> HipAngles { get; set; }
        public ObservableCollection<Angles> NormalKneeAngles { get; set; }
        public ObservableCollection<Angles> KneeAngles { get; set; }
        public ObservableCollection<Angles> AnkleAngles { get; set; }

        public ObservableCollection<Angles> HipLinearVelocity { get; set; }
        public ObservableCollection<Angles> KneeLinearVelocity { get; set; }
        public ObservableCollection<Angles> AnkleLinearVelocity { get; set; }
        public ObservableCollection<Angles> HipDisplacement { get; set; }
        public ObservableCollection<Angles> KneeDisplacement { get; set; }
        public ObservableCollection<Angles> AnkleDisplacement { get; set; }
        public ObservableCollection<Angles> HipLinearAcceleration { get; set; }
        public ObservableCollection<Angles> KneeLinearAcceleration { get; set; }
        public ObservableCollection<Angles> AnkleLinearAcceleration { get; set; }
        public ObservableCollection<Angles> HipAngularVelocity { get; set; }
        public ObservableCollection<Angles> KneeAngularVelocity { get; set; }
        public ObservableCollection<Angles> AnkleAngularVelocity { get; set; }
        public ObservableCollection<Angles> HipAngularAcceleration { get; set; }
        public ObservableCollection<Angles> KneeAngularAcceleration { get; set; }
        public ObservableCollection<Angles> AnkleAngularAcceleration { get; set; }
        public ObservableCollection<Angles> HipMoment { get; set; }
        public ObservableCollection<Angles> KneeMoment { get; set; }
        public ObservableCollection<Angles> AnkleMoment { get; set; }

        public ObservableCollection<Test> GetListTest { get; set; } //Lista de clases de test
        public ObservableCollection<TestResults> GetListTestResult { get; set; } //Lista de clases de test
        public ObservableCollection<TestResults> GetListTestAngle { get; set; }
        #endregion
        #region Constructor
        public VMpageAnalysisResults(INavigation navigation, Test TestPatient, Patient PatientPicked)//recibe un parámetro de tipo int que es el ID que envía VMpagePatients
        {
            Navigation = navigation;
            Patient = new Patient();
            _patientPicked = PatientPicked;//Guarda la clase del paciente seleccionado
            _testPatient = TestPatient;//Guarda la clase del test del usuario 
            TestResult = new TestResults();
            GetListPatients = new ObservableCollection<Patient>();
            CaderaPoints = new ObservableCollection<Puntos>();
            HipAngles = new ObservableCollection<Angles>();
            KneeAngles = new ObservableCollection<Angles>();
            AnkleAngles = new ObservableCollection<Angles>();
            NormalKneeAngles = new ObservableCollection<Angles>();

            HipDisplacement = new ObservableCollection<Angles>();
            KneeDisplacement = new ObservableCollection<Angles>();
            AnkleDisplacement = new ObservableCollection<Angles>();
            HipLinearVelocity = new ObservableCollection<Angles>();
            KneeLinearVelocity = new ObservableCollection<Angles>();
            AnkleLinearVelocity = new ObservableCollection<Angles>();
            HipLinearAcceleration = new ObservableCollection<Angles>();
            KneeLinearAcceleration = new ObservableCollection<Angles>();
            AnkleLinearAcceleration = new ObservableCollection<Angles>();
            HipAngularVelocity = new ObservableCollection<Angles>();
            KneeAngularVelocity = new ObservableCollection<Angles>();
            AnkleAngularVelocity = new ObservableCollection<Angles>();
            HipAngularAcceleration = new ObservableCollection<Angles>();
            KneeAngularAcceleration = new ObservableCollection<Angles>();
            AnkleAngularAcceleration = new ObservableCollection<Angles>();
            HipMoment = new ObservableCollection<Angles>();
            KneeMoment = new ObservableCollection<Angles>();
            AnkleMoment = new ObservableCollection<Angles>();
            //Prueba();
            //ToGraphCoordinates();
            //ToGraphicAngles();
            //ToGraphicAngularAccelaration();
            //ToGraphicAngularVelocity();
            //ToGraphicLinearAcceleration();
            //ToGraphicLinearVelocity();
            //ToGraphicMoments();
            //NormalAngles();
            InitializeData();
        }
        private async void InitializeData()
        {
            try
            {
                await ToGraphCoordinates();
                await ToGraphicAngles();
                await ToGraphicAngularAccelaration();
                await ToGraphicAngularVelocity();
                await ToGraphicLinearAcceleration();
                await ToGraphicLinearVelocity();
                await ToGraphicMoments();
                await ToGraphicDisplacement();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex.Message}");
            }
        }

        #endregion
        #region Objetos
        public Patient Patient
        {
            get { return _patientPicked; }
            set
            {
                SetValue(ref _patientPicked, value);
            }
        }
        public Test Test
        {
            get { return _testPatient; }
            set
            {
                SetValue(ref _testPatient, value);
            }
        }
        /*public DataPoint DataPoint
        {
            get { return _dataPoint; }
            set
            {
                SetValue(ref _dataPoint, value);
            }
        }*/
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
        /*public void Prueba() 
        {
            CaderaPoints = new ObservableCollection<Puntos>()
            {
                new Puntos {  Target = 500, Sale = 600 },
                new Puntos {  Target = 600, Sale = 750 },
                new Puntos {  Target = 700, Sale = 800 },
                new Puntos {  Target = 800, Sale = 900 },
                new Puntos {  Target = 900, Sale = 950 },
                new Puntos {  Target = 1000, Sale = 1100 }
            };
            //DataPoint datapoint = new DataPoint() { Xvalue = 5.7, Yvalue = 7.2 };
            //CaderaPoints.Add(datapoint);

            //DataPoint datapoint2 = new DataPoint() { Xvalue = 7.2, Yvalue = 2.2 };
            //CaderaPoints.Add(datapoint2);
        }*/
        public async Task ComeBackMain()
        {
            await Navigation.PopAsync();
            //await Navigation.PushAsync(new PageAnalysisResults(TestPatient, PickPatients));
        }
        public async Task ToGraphCoordinates()
        {
            //Console.WriteLine(Patient.Name);
            //Console.WriteLine(Test.IdTest);
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/testresult/{Test.IdTest}";
                var response = await client.GetAsync(apiUrl);
                var json = await response.Content.ReadAsStringAsync();
                var testResults = JsonConvert.DeserializeObject<TestResults>(json);
                Console.WriteLine($"Coordenadas: {testResults.caderaY}");

                List<double> ConvertToDoubleList(string jsonArray)
                {
                    try
                    {
                        return jsonArray.TrimStart('[').TrimEnd(']')
                                        .Split(',')
                                        .Select(x => double.Parse(x.Trim()))
                                        .ToList();
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine($"Error de formato en ConvertToDoubleList: {ex.Message}");
                        return new List<double>();
                    }
                }

                List<double> CADERAX = ConvertToDoubleList(testResults.caderaX);
                List<double> CADERAY = ConvertToDoubleList(testResults.caderaY);
                List<double> RODILLAX = ConvertToDoubleList(testResults.rodillaX);
                List<double> RODILLAY = ConvertToDoubleList(testResults.rodillaY);
                List<double> TOBILLOX = ConvertToDoubleList(testResults.tobilloX);
                List<double> TOBILLOY = ConvertToDoubleList(testResults.tobilloY);
                

                for (int i = 0; i < CADERAX.Count; i++)
                {
                    CaderaPoints.Add(new Puntos
                    { HipX = CADERAX[i],
                        HipY = CADERAY[i],
                        KneeX = RODILLAX[i],
                        KneeY = RODILLAY[i],
                        AnkleX = TOBILLOX[i],
                        AnkleY = TOBILLOY[i],
                    });
                }
                //Para reorganizar los puntos multiplico por negativo el eje Y y después sumo el doble

            }
        }
        public async Task ToGraphicDisplacement()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/testdisplacement/{Test.IdTest}";
                var response = await client.GetAsync(apiUrl);
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Desplazamiento: {json}");

                try
                {
                    var testDisplacement = JsonConvert.DeserializeObject<TestDisplacement>(json);

                    // Helper method to convert JSON array strings to List<double>
                    List<double> ConvertToDoubleList(string jsonArray)
                    {
                        try
                        {
                            return jsonArray.TrimStart('[').TrimEnd(']')
                                            .Split(',')
                                            .Select(x => double.Parse(x.Trim()))
                                            .ToList();
                        }
                        catch (FormatException ex)
                        {
                            Console.WriteLine($"Error de formato en ConvertToDoubleList: {ex.Message}");
                            return new List<double>();
                        }
                    }

                    List<double> desplazamientoCADERA = ConvertToDoubleList(testDisplacement.caderaDisplacement);

                    List<double> desplazamientoRODILLA = ConvertToDoubleList(testDisplacement.rodillaDisplacement);

                    List<double> desplazamientoTOBILLO = ConvertToDoubleList(testDisplacement.tobilloDisplacement);


                    for (int i = 0; i < desplazamientoCADERA.Count; i++)
                    {

                        HipDisplacement.Add(new Angles
                        {
                            Angle = desplazamientoCADERA[i],
                            Time = i,
                        });
                        KneeDisplacement.Add(new Angles
                        {
                            Angle = desplazamientoRODILLA[i],
                            Time = i,
                        });
                        AnkleDisplacement.Add(new Angles
                        {
                            Angle = desplazamientoTOBILLO[i],
                            Time = i,
                        });
                    }
                    foreach (var angle in AnkleAngles)
                    {
                        Console.WriteLine($"Angle: {angle.Angle}, Time: {angle.Time}");

                    }
                    if (HipDisplacement.Count == 0)
                    {

                        Console.WriteLine("La lista HipAngles está vacía");
                    }
                    else
                    {
                        Console.WriteLine($"La lista HipAngles tiene {HipDisplacement.Count} elementos");
                        Console.WriteLine($"El tipo de valor de Angle es: {HipDisplacement[0].Angle.GetType()}");
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Ángulos: {ex.Message}");
                }
            }
        }
        public async Task ToGraphicAngles()
        {
            List<double> CaderaNormal = new List<double>()
            {
                23.64678048150219, 25.22122666689114, 25.776082608939454, 25.692207065871465, 25.239783155740135, 24.60289666945694, 23.899925285664764, 23.200283473740242, 22.538015266664388, 21.92267769259817, 21.347912036303967, 20.798057634344936, 20.253123652594134, 19.692398083038462, 19.096939807253264, 18.45116893242021, 17.743742571752158, 16.967877610262576, 16.12125875436547, 15.20564904446013, 14.226301011657519, 13.19124956512804, 12.11055246044257, 10.995530653062659, 9.858048894278959, 8.709866482493474, 7.562079010258913, 6.424664160413812, 5.306138010883785, 4.213322778461055, 3.1512224403442275, 2.1229990394107148, 1.1300397160790254, 0.17210245435085625, -0.7524728421253499, -1.6465021193073426, -2.5136582372670375, -3.3581496026907267, -4.184390084352422, -4.996673041765337, -5.798861157097932, -6.594102400102237, -7.38458094528125, -8.171310240045251, -8.95397373098411, -9.730817046672911, -10.498593737949754, -11.252565027056413, -11.986552449799994, -12.693040813692626, -13.36332756703397, -13.987713499921888, -14.555728693882521, -15.056386820354994, -15.478460266616274, -15.810768152273504, -16.042469091255803, -16.163350556930297, -16.164106917143563, -16.036598617302765, -15.774085592908428, -15.371428776346399, -14.825254510042043, -14.1340777702517, -13.29838131997871, -12.320649220054655, -11.205354504739248, -9.958902239771596, -8.58953059042856, -7.107173895485182, -5.523293027252464, -3.8506794718022377, -2.1032405377187864, -0.29577384325704514, 1.556260315586698, 3.4369600094891126, 5.3301877335025125, 7.219753864104511, 9.089571234485629, 10.923776500417004, 12.706816619578833, 14.423502194635864, 16.059033717815986, 17.599011988780234, 19.029450248098144, 20.336812968901224, 21.508114875921244, 22.531123714113885, 23.394721671784474, 24.08949328228801, 24.60862219406424, 24.94919552443603, 25.114032715065452, 25.114176006408734, 24.97220196848271, 24.726538092664867, 24.43699539437145, 24.191757432934182, 24.116098259832025, 24.3831367
            };
            List<double> RodillaNormal = new List<double>()
            {
                1.9449522404684085, 2.1979994978715194, 2.35468059363947, 2.4231572127588503, 2.4260111352634066, 2.3937979460613246, 2.3600141189602066, 2.357285453321138, 2.4146016670000474, 2.5554378455353, 2.7966185378335933, 3.147794438548315, 3.611414965765155, 4.183092582315188, 4.852266491126683, 5.603084304183708, 6.415430593084897, 7.266040758233677, 8.129647541436787, 8.980115709942636, 9.791528007016415, 10.539192420850624, 11.20054717214134, 11.755945617612758, 12.189308499098017, 12.4886356805979, 12.646373728344477, 12.659639414220441, 12.53030249192104, 12.264933928761193, 11.874628187000203, 11.374710173170028, 10.784339119188145, 10.126022954721966, 9.425057692451942, 8.708907003885846, 8.006537522729223, 7.347725507687235, 6.762350338721694, 6.279689935037835, 5.927732585043996, 5.732518892968171, 5.7175265874738255, 5.903109828451937, 6.306003406337139, 6.938900871036855, 7.8101141766317905, 8.923320900708978, 10.277403510756292, 11.866383523425354, 13.679451754883088, 15.701094208162486, 17.911311502911857, 20.28592814608114, 22.79698638022291, 25.413217851457254, 28.100584925429104, 30.82288216452988, 33.542387280356365, 36.220549807154654, 38.81870482135773, 41.298798275994855, 43.62410994117596, 45.75995955994141, 47.67438165703745, 49.338754492564085, 50.72836894747262, 51.82292367887689, 52.60693370454909, 53.070040682529054, 53.207214557598135, 53.01883796593668, 52.51066683627927, 51.69366301440101, 50.58369748120081, 49.201125846646015, 47.5702412955269, 45.71861404954008, 43.67633070649086, 41.47515153421084, 39.147608946561974, 36.72607598409779, 34.24183967472825, 31.724220672073407, 29.199787572960254, 26.69172181231584, 24.219397035034206, 21.79824536150464, 19.439992007494467, 17.153349300898995, 14.945271268240598, 12.822880653279613, 10.796191489064146, 8.881762183394454, 7.107426506021355, 5.518262893774153, 4.183976126872758, 3.207879685394982, 2.7376819785336313, 2.97829516
            };
            List<double> TobilloNormal = new List<double>()
            {
                14.229449680061128, 14.092195924564697, 13.812375657009602, 13.464768334544692, 13.097901679622515, 12.741453821744932, 12.411950014274487, 12.117058693892465, 11.858751996506589, 11.635559305776587, 11.444109362555883, 11.28012694937218, 11.139023073944292, 11.016194059528663, 10.907123296995854, 10.807360975670468, 10.712440461938973, 10.617775698978027, 10.518571806027955, 10.409770382811365, 10.286042378650626, 10.14183404996479, 9.971465692081983, 9.769278274119989, 9.529819640231073, 9.248059553638944, 8.919621340290808, 8.541017193723269, 8.10987413243513, 7.625138229426744, 7.087245720483031, 6.498251065008423, 5.861903816987541, 5.183668100971863, 4.4706806833399835, 3.7316458354986306, 2.9766674423712143, 2.2170210285814047, 1.464870492599351, 0.7329363243659, 0.03412386613852014, -0.6188782478882136, -1.2140180303872175, -1.7403059936152179, -2.188233736969261, -2.5501717076146324, -2.8207325233960407, -2.9970865592696274, -3.0792171424749117, -3.070103710126725, -2.9758226193833313, -2.805556966105124, -2.571508741336652, -2.288708907806276, -1.9747234896037558, -1.649256494391432, -1.3336533965215924, -1.0503119510941819, -0.8220102376389011, -0.671164988260097, -0.6190363819854188, -0.684898516359306, -0.8851976299588902, -1.2327227674577816, -1.7358158720672012, -2.397650172495114, -3.215607108650355, -4.18078281822701, -5.277655280637825, -6.483942478525181, -7.77068027803479, -9.102546028454869, -10.438450017184879, -11.732411758247133, -12.934731508652762, -13.993459257604103, -14.856153574736966, -15.471910986242287, -15.79363281696889, -15.780480532862635, -15.400452376386747, -14.632993337495792, -13.471527069070108, -11.925772057618824, -10.023675011731362, -7.812761841037313, -5.360670570982345, -2.7545908719880767, -0.09929136870514021, 2.4866316758735874, 4.876702553012686, 6.945185773267218, 8.578912869361712, 9.692918030468455, 10.250555486175896, 10.288848223147344, 9.949899970745705, 9.51929068756331, 9.472470277895216, 10.53026623
            };
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/testangle/{Test.IdTest}";
                var response = await client.GetAsync(apiUrl);
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Angulos: {json}");

                try
                {
                    var testAngles = JsonConvert.DeserializeObject<TestAngle>(json);
                    Console.WriteLine($"asdasdsasdasdasd: {testAngles}");
                    Console.WriteLine($"mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm: {testAngles.anguloCadera}");

                    // Helper method to convert JSON array strings to List<double>
                    List<double> ConvertToDoubleList(string jsonArray)
                    {
                        try
                        {
                            return jsonArray.TrimStart('[').TrimEnd(']')
                                            .Split(',')
                                            .Select(x => double.Parse(x.Trim()))
                                            .ToList();
                        }
                        catch (FormatException ex)
                        {
                            Console.WriteLine($"Error de formato en ConvertToDoubleList: {ex.Message}");
                            return new List<double>();
                        }
                    }

                    List<double> anguloCADERA = ConvertToDoubleList(testAngles.anguloCadera);
                    
                    List<double> anguloRODILLA = ConvertToDoubleList(testAngles.anguloRodilla);
                    
                    List<double> anguloTOBILLO = ConvertToDoubleList(testAngles.anguloTobillo);
                    

                    for (int i = 0; i < anguloCADERA.Count; i++)
                    {

                        HipAngles.Add(new Angles
                        {
                            Angle = anguloCADERA[i],
                            Time = i,
                        });
                        KneeAngles.Add(new Angles
                        {
                            Angle = anguloRODILLA[i],
                            Time = i,
                        });
                        AnkleAngles.Add(new Angles
                        {
                            Angle = anguloTOBILLO[i],
                            Time = i,
                        });
                    }
                    foreach (var angle in AnkleAngles)
                    {
                        Console.WriteLine($"Angle: {angle.Angle}, Time: {angle.Time}");

                    }
                    if (HipAngles.Count == 0)
                    {
                        
                        Console.WriteLine("La lista HipAngles está vacía");
                    }
                    else
                    {
                        Console.WriteLine($"La lista HipAngles tiene {HipAngles.Count} elementos");
                        Console.WriteLine($"El tipo de valor de Angle es: {HipAngles[0].Angle.GetType()}");
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Ángulos: {ex.Message}");
                }
            }
        }
        public async Task ToGraphicLinearVelocity()
        {

            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/testlinearvelocity/{Test.IdTest}"; //Modificar
                var response = await client.GetAsync(apiUrl);
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Velocidadaes Lineales: {json}");
                try 
                { 

                    var LinearVelocity = JsonConvert.DeserializeObject<TestLinearVelocity>(json);

                    LinearVelocity.caderaLinearVelocity = LinearVelocity.caderaLinearVelocity.TrimStart('[').TrimEnd(']');
                    List<double> LinearVelocityCADERA = LinearVelocity.caderaLinearVelocity.Split(',').Select(double.Parse).ToList();


                    LinearVelocity.rodillaLinearVelocity = LinearVelocity.rodillaLinearVelocity.TrimStart('[').TrimEnd(']');
                    List<double> LinearVelocityRODILLA = LinearVelocity.rodillaLinearVelocity.Split(',').Select(double.Parse).ToList();


                    LinearVelocity.tobilloLinearVelocity = LinearVelocity.tobilloLinearVelocity.TrimStart('[').TrimEnd(']');
                    List<double> LinearVelocityTOBILLO = LinearVelocity.tobilloLinearVelocity.Split(',').Select(double.Parse).ToList();

                    for (int i = 0; i < LinearVelocityCADERA.Count; i++)
                    {
                        HipLinearVelocity.Add(new Angles //revisar si acepta este modelo
                        {
                            Angle = LinearVelocityCADERA[i],
                            Time = i,
                        });
                        KneeLinearVelocity.Add(new Angles
                        {
                            Angle = LinearVelocityRODILLA[i],
                            Time = i,
                        });
                        AnkleLinearVelocity.Add(new Angles
                        {
                            Angle = LinearVelocityTOBILLO[i],
                            Time = i,
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Velocida Lineal: {ex.Message}");
                }
            }
        }
        public async Task ToGraphicLinearAcceleration()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/testlinearacceleration/{Test.IdTest}"; //Modificar
                var response = await client.GetAsync(apiUrl);
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Aceleraciones lineales: {json}");
                try 
                {
                    var LinearAcceleration = JsonConvert.DeserializeObject<TestLinearAcceleration>(json);

                    LinearAcceleration.caderaLinearAcceleration = LinearAcceleration.caderaLinearAcceleration.TrimStart('[').TrimEnd(']');
                    List<double> LinearAccelerationCADERA = LinearAcceleration.caderaLinearAcceleration.Split(',').Select(double.Parse).ToList();


                    LinearAcceleration.rodillaLinearAcceleration = LinearAcceleration.rodillaLinearAcceleration.TrimStart('[').TrimEnd(']');
                    List<double> LinearAccelerationRODILLA = LinearAcceleration.rodillaLinearAcceleration.Split(',').Select(double.Parse).ToList();


                    LinearAcceleration.tobilloLinearAcceleration = LinearAcceleration.tobilloLinearAcceleration.TrimStart('[').TrimEnd(']');
                    List<double> LinearAccelerationTOBILLO = LinearAcceleration.tobilloLinearAcceleration.Split(',').Select(double.Parse).ToList();

                    for (int i = 0; i < LinearAccelerationCADERA.Count; i++)
                    {
                        HipLinearAcceleration.Add(new Angles
                        {
                            Angle = LinearAccelerationCADERA[i],
                            Time = i,
                        });
                        KneeLinearAcceleration.Add(new Angles
                        {
                            Angle = LinearAccelerationRODILLA[i],
                            Time = i,
                        });
                        AnkleLinearAcceleration.Add(new Angles
                        {
                            Angle = LinearAccelerationTOBILLO[i],
                            Time = i,
                        });

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Aceleracion lineal: {ex.Message}");
                }
            }
        }
        public async Task ToGraphicAngularVelocity()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/testangularvelocity/{Test.IdTest}"; //Modificar
                var response = await client.GetAsync(apiUrl);
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Velocidades angulares: {json}");
                try 
                {
                    var AngularVelocity = JsonConvert.DeserializeObject<TestAngularVelocity>(json);

                    AngularVelocity.caderaAngularVelocity = AngularVelocity.caderaAngularVelocity.TrimStart('[').TrimEnd(']');
                    List<double> AngularVelocityCADERA = AngularVelocity.caderaAngularVelocity.Split(',').Select(double.Parse).ToList();


                    AngularVelocity.rodillaAngularVelocity = AngularVelocity.rodillaAngularVelocity.TrimStart('[').TrimEnd(']');
                    List<double> AngularVelocityRODILLA = AngularVelocity.rodillaAngularVelocity.Split(',').Select(double.Parse).ToList();


                    AngularVelocity.tobilloAngularVelocity = AngularVelocity.tobilloAngularVelocity.TrimStart('[').TrimEnd(']');
                    List<double> AngularVelocityTOBILLO = AngularVelocity.tobilloAngularVelocity.Split(',').Select(double.Parse).ToList();

                    for (int i = 0; i < AngularVelocityCADERA.Count; i++)
                    {
                        HipAngularVelocity.Add(new Angles
                        {
                            Angle = AngularVelocityCADERA[i],
                            Time = i,
                        });
                        KneeAngularVelocity.Add(new Angles
                        {
                            Angle = AngularVelocityRODILLA[i],
                            Time = i,
                        });
                        AnkleAngularVelocity.Add(new Angles
                        {
                            Angle = AngularVelocityTOBILLO[i],
                            Time = i,
                        });

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Velocidad Angular: {ex.Message}");
                }
            }
        }
        public async Task ToGraphicAngularAccelaration()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/testangularacceleration/{Test.IdTest}"; //Modificar
                var response = await client.GetAsync(apiUrl);
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Aceleraciones angulares: {json}");
                try 
                { 
                    var AngularAcceleration = JsonConvert.DeserializeObject<TestAngularAcceleration>(json);

                    AngularAcceleration.caderaAngularAcceleration = AngularAcceleration.caderaAngularAcceleration.TrimStart('[').TrimEnd(']');
                    List<double> AngularAccelerationCADERA = AngularAcceleration.caderaAngularAcceleration.Split(',').Select(double.Parse).ToList();


                    AngularAcceleration.rodillaAngularAcceleration = AngularAcceleration.rodillaAngularAcceleration.TrimStart('[').TrimEnd(']');
                    List<double> AngularAccelerationRODILLA = AngularAcceleration.rodillaAngularAcceleration.Split(',').Select(double.Parse).ToList();


                    AngularAcceleration.tobilloAngularAcceleration = AngularAcceleration.tobilloAngularAcceleration.TrimStart('[').TrimEnd(']');
                    List<double> AngularAccelerationTOBILLO = AngularAcceleration.tobilloAngularAcceleration.Split(',').Select(double.Parse).ToList();

                    for (int i = 0; i < AngularAccelerationCADERA.Count; i++)
                    {
                        HipAngularAcceleration.Add(new Angles
                        {
                            Angle = AngularAccelerationCADERA[i],
                            Time = i,
                        });
                        KneeAngularAcceleration.Add(new Angles
                        {
                            Angle = AngularAccelerationRODILLA[i],
                            Time = i,
                        });
                        AnkleAngularAcceleration.Add(new Angles
                        {
                            Angle = AngularAccelerationTOBILLO[i],
                            Time = i,
                        });

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Angular Aceleracion: {ex.Message}");
                }

            }
        }
        public async Task ToGraphicMoments()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{ApiConfig.BaseUrl}/testmoments/{Test.IdTest}"; //Modificar
                var response = await client.GetAsync(apiUrl);
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Momentos: {json}");
                try 
                { 
                    var Moments = JsonConvert.DeserializeObject<TestMoments>(json);

                    Moments.caderaMomentMrc = Moments.caderaMomentMrc.TrimStart('[').TrimEnd(']');
                    List<double> LinearVelocityCADERA = Moments.caderaMomentMrc.Split(',').Select(double.Parse).ToList();


                    Moments.rodillaMomentMtr = Moments.rodillaMomentMtr.TrimStart('[').TrimEnd(']');
                    List<double> LinearVelocityRODILLA = Moments.rodillaMomentMtr.Split(',').Select(double.Parse).ToList();


                    Moments.tobilloMomentMmt = Moments.tobilloMomentMmt.TrimStart('[').TrimEnd(']');
                    List<double> LinearVelocityTOBILLO = Moments.tobilloMomentMmt.Split(',').Select(double.Parse).ToList();

                    for (int i = 0; i < LinearVelocityCADERA.Count; i++)
                    {
                        HipMoment.Add(new Angles
                        {
                            Angle = LinearVelocityCADERA[i],
                            Time = i,
                        });
                        KneeMoment.Add(new Angles
                        {
                            Angle = LinearVelocityRODILLA[i],
                            Time = i,
                        });
                        AnkleMoment.Add(new Angles
                        {
                            Angle = LinearVelocityTOBILLO[i],
                            Time = i,
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Momentos: {ex.Message}");
                }
            }
        }
        //public async void NormalAngles()
        //{


        //    //for (int i = 0; i < CaderaNormal.Count; i++)
        //    //{
        //    //    NormalKneeAngles.Add(new Angles
        //    //    {
        //    //        Angle = CaderaNormal[i],
        //    //        Time = i,
        //    //    });
        //    //}
        //}
        //ObservableCollection<int> listaDeNumeros = new ObservableCollection<int>(testResults.caderaX.Split(',').Select(int.Parse));


        #endregion
        #region Comandos
        public ICommand ComeBackMainCommand => new Command(async () => await ComeBackMain());
        //public ICommand LlegaCommand => new Command(async () => await Llega());
        //public ICommand EditPatientCommand => new Command(async () => await EditPatient());
        #endregion
    }
}
