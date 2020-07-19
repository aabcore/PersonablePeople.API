using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace PersonablePeople.API
{
    public class Randomization
    {
        public static Normal PredictionRand = new Normal(0.4, 0.21);

        public static double RandomPredictionScore() => PredictionRand.Sample();
    }
}
