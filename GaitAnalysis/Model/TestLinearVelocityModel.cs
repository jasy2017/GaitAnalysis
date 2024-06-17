using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GaitAnalysis.Model
{
    public class TestLinearVelocityModel
    {
        public class TestLinearVelocity
        {
            [JsonProperty("IdMainTestLinearVelocityModel")]
            public int IdMainTestLinearVelocityModel { get; set; }

            [JsonProperty("idTestLinearVelocity")]
            public int idTestLinearVelocity { get; set; }

            [JsonProperty("caderaLinearVelocity")]
            public string caderaLinearVelocity { get; set; }

            [JsonProperty("rodillaLinearVelocity")]
            public string rodillaLinearVelocity { get; set; }

            [JsonProperty("tobilloLinearVelocity")]
            public string tobilloLinearVelocity { get; set; }

        }
    }
}
