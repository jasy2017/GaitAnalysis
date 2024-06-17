using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GaitAnalysis.Model
{
    public class TestMomentModel
    {
        public class TestMoments
        {
            [JsonProperty("IdMainTestMoment")]
            public int IdMainTestAngleMoment { get; set; }

            [JsonProperty("idTestMoment")]
            public int idTestMoment { get; set; }

            [JsonProperty("caderaMomentMrc")]
            public string caderaMomentMrc { get; set; }

            [JsonProperty("rodillaMomentMtr")]
            public string rodillaMomentMtr { get; set; }

            [JsonProperty("tobilloMomentMmt")]
            public string tobilloMomentMmt { get; set; }

        }
    }
}
