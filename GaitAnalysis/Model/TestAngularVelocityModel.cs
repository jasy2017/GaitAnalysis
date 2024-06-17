using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GaitAnalysis.Model
{
    public class TestAngularVelocityModel
    {
        public class TestAngularVelocity
        {
            [JsonProperty("IdMainTestAngularVelocityModel")]
            public int IdMainTestAngularVelocityModel { get; set; }

            [JsonProperty("idTestAngularVelocity")]
            public int idTestAngularVelocity { get; set; }

            [JsonProperty("caderaAngularVelocity")]
            public string caderaAngularVelocity { get; set; }

            [JsonProperty("rodillaAngularVelocity")]
            public string rodillaAngularVelocity { get; set; }

            [JsonProperty("tobilloAngularVelocity")]
            public string tobilloAngularVelocity { get; set; }

        }
    }
}
