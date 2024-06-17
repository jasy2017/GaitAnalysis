using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GaitAnalysis.Model
{
    public class TestModel
    {
        public class Test
        {   //La deserealización de json tiene prioridad sobre los nombbres de los atributos y no sobre 
            //la asignación de nombres que se le asigna con JsonProperty
            //[JsonProperty("IdTest")]//debe ser el mismo nombre json con el que envía la api
                                    //test_item = {
                                    //              "IdTest": row[0],
                                    //              "IdPatients": row[1],
                                    //              "VideoPath": row[2],
                                    //              "Date": row[3]
                                    //             }
        public int IdTest { get; set; }

            //[JsonProperty("idPatients")]
            public int IdPatients { get; set; }

            //[JsonProperty("videoPath")]
            public string VideoPath { get; set; }
            
            //[JsonProperty("date")]
            public string Date { get; set; }
            
        }


    }
}
