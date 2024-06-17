using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GaitAnalysis.Model
{
    public class TestAngleModel
    {
        public class TestAngle
        {
            [JsonProperty("IdMainTestAngle")]
            public int IdMainTestAngle { get; set; }

            [JsonProperty("idTestAngle")]
            public int idTestAngle { get; set; }

            [JsonProperty("anguloCadera")]
            public string anguloCadera { get; set; }

            [JsonProperty("anguloRodilla")]
            public string anguloRodilla { get; set; }

            [JsonProperty("anguloTobillo")]
            public string anguloTobillo { get; set; }

        }
    }
}
