using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GaitAnalysis.Model
{
    public class TestAngularAccelerationModel
    {
        public class TestAngularAcceleration
        {
            [JsonProperty("IdMainTestAngularAccelerationModel")]
            public int IdMainTestAngularAccelerationModel { get; set; }

            [JsonProperty("idTestAngularAcceleration")]
            public int idTestAngularAcceleration { get; set; }

            [JsonProperty("caderaAngularAcceleration")]
            public string caderaAngularAcceleration { get; set; }

            [JsonProperty("rodillaAngularAcceleration")]
            public string rodillaAngularAcceleration { get; set; }

            [JsonProperty("tobilloAngularAcceleration")]
            public string tobilloAngularAcceleration { get; set; }

        }
    }
}
