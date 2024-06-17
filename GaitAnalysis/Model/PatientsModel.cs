using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GaitAnalysis.Model
{
    public class PatientsModel
    {
        public class Patient
        {
            [JsonProperty("id")]
            public int Id { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("lastname")]
            public string Lastname { get; set; }
            [JsonProperty("age")]
            public int Age { get; set; }
            [JsonProperty("weight")]
            public float Weight { get; set; }
            [JsonProperty("height")]
            public float Height { get; set; }  
        }
    }
}
