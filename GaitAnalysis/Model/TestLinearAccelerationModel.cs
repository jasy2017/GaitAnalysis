using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GaitAnalysis.Model
{
    public class TestLinearAccelerationModel
    {
        public class TestLinearAcceleration
        {
            [JsonProperty("IdMainTestLinearAccelerationModel")]
            public int IdMainTestLinearAccelerationModel { get; set; }

            [JsonProperty("idTestLinearAcceleration")]
            public int idTestLinearAcceleration { get; set; }

            [JsonProperty("caderaLinearAcceleration")]
            public string caderaLinearAcceleration { get; set; }

            [JsonProperty("rodillaLinearAcceleration")]
            public string rodillaLinearAcceleration { get; set; }

            [JsonProperty("tobilloLinearAcceleration")]
            public string tobilloLinearAcceleration { get; set; }

        }
    }
}
