using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GaitAnalysis.Model
{
    public class TestDisplacementModel
    {
        public class TestDisplacement
        {
            [JsonProperty("idMainTestDisplacementModel")]
            public int idMainTestDisplacementModel { get; set; }

            [JsonProperty("idTestDisplacement")]
            public int idTestDisplacement { get; set; }

            [JsonProperty("caderaDisplacement")]
            public string caderaDisplacement { get; set; }

            [JsonProperty("rodillaDisplacement")]
            public string rodillaDisplacement { get; set; }

            [JsonProperty("tobilloDisplacement")]
            public string tobilloDisplacement { get; set; }

        }
    }

}
