using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymulatorPoziomuCieczy.Model.Symulacja
{
    public static class SzumGaussowski
    {
        public static float DodajSzumyGaussowski(float poziomCieczy, double odchylenieStandardowe)
        {
            Random rand = new Random();
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double randNormal = 0 + odchylenieStandardowe * randStdNormal;
            return poziomCieczy + (float)randNormal;
        }
    }
}
