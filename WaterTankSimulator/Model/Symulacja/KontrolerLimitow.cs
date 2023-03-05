using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymulatorPoziomuCieczy.Model.Symulacja
{
    public static class KontrolerLimitow
    {
        public static float SprawdzLimitGorny(float iloscCieczyWZbiorniku, float promienZbiornika, float maksymalnyPoziom)
        {
            if (((float)iloscCieczyWZbiorniku / (promienZbiornika * 0.1f * promienZbiornika * 0.1f * 3.14159265359f)) > maksymalnyPoziom * 0.1f)
            {
                return promienZbiornika * 0.1f * promienZbiornika * 0.1f * 3.14159265359f * maksymalnyPoziom * 0.1f;
            }
            else
            {
                return iloscCieczyWZbiorniku;
            }
        }
        public static float SprawdzLimitDolny(float iloscCieczyWZbiorniku, float promienZbiornika, float minimalnyPoziom)
        {
            if ((float)iloscCieczyWZbiorniku / (promienZbiornika * 0.1 * promienZbiornika * 0.1 * 3.14159265359f) < minimalnyPoziom * 0.1)
            {
                return promienZbiornika * 0.1f * promienZbiornika * 0.1f * 3.14159265359f * minimalnyPoziom * 0.1f;
            }
            else
            {
                return iloscCieczyWZbiorniku;
            }
        }
    }
}
