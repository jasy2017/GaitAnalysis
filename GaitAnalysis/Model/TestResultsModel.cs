using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GaitAnalysis.Model
{
    public class TestResultsModel
    {
        public class TestResults
        {
            [JsonProperty("Idtestresult")]
            public int Idtestresult { get; set; }

            [JsonProperty("IdTest")]
            public int idTest { get; set; }

            [JsonProperty("caderaX")]
            public string caderaX { get; set; }

            [JsonProperty("caderaY")]
            public string caderaY { get; set; }

            [JsonProperty("rodillaX")]
            public string rodillaX { get; set; }

            [JsonProperty("rodillaY")]
            public string rodillaY { get; set; }

            [JsonProperty("tobilloX")]
            public string tobilloX { get; set; }

            [JsonProperty("tobilloY")]
            public string tobilloY { get; set; }

            //[JsonProperty("antepieX")]
            //public string AntepieX { get; set; }

            //[JsonProperty("antepieY")]
            //public string AntepieY { get; set; }

        }
    }
}
