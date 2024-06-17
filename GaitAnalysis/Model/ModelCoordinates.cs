using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GaitAnalysis.Model
{
    public class ModelCoordinates
    {
        public class Puntos
        {
            public double HipX { get; set; }
            public double HipY { get; set; }
            public double KneeX { get; set; }
            public double KneeY { get; set; }
            public double AnkleX { get; set; }
            public double AnkleY { get; set; }

        }

    }

}
