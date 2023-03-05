using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymulatorPoziomuCieczy.Model.Symulacja
{
    public static class LicznikSymulatora
    {
        public static float ObliczNalewanaCieczWMiliLitrach( float zaworNalewajacy, float parametrA, float parametrB, float parametrC, bool pompaNalewajaca )
        {

            float nalewanaCiecz = 0;

            if(zaworNalewajacy == 0 || pompaNalewajaca == false)
            {
                nalewanaCiecz = 0;
            }
            else
            {
                nalewanaCiecz = parametrA * zaworNalewajacy * zaworNalewajacy * zaworNalewajacy +
                               parametrB * zaworNalewajacy * zaworNalewajacy +
                               parametrC * zaworNalewajacy;
            }
            if(nalewanaCiecz > 0)
            {
                return nalewanaCiecz;
            }
            else
            {
                return 0;
            }
           
        }
        public static float ObliczMaksymalneNalewanie(float parametrA, float parametrB, float parametrC )
        {
            return parametrA * 1000000 + parametrB * 10000 + parametrC * 100;
        }
        public static float ObliczIloscCieczyWLitrach(float aktualnaIloscCieczyWZbiorniku, float nalewanaCiecz, bool zaworGrawitacyjny, float wyplywCieczy)
        {
            float obliczonaCiecz = 0;
            float czasProbkowania = 0.1f;
            float zmienNaLitry = 0.001f;

            if (!zaworGrawitacyjny)
            {
                wyplywCieczy = 0;
            }

            obliczonaCiecz = aktualnaIloscCieczyWZbiorniku + nalewanaCiecz * zmienNaLitry * czasProbkowania - wyplywCieczy * zmienNaLitry * czasProbkowania;

            return obliczonaCiecz;
        }

        public static float ObliczPoziomCieczy(float iloscCieczyWZbiorniku, float promienZbiornika)
        {
            return ((float)iloscCieczyWZbiorniku * 1000/ (promienZbiornika * promienZbiornika * 3.14159265359f)); //Oblicz poziom cieczy i zamien na cm
        }


        public static int ZmienPoziomCieczyNaWartoscAnalogowaWyjsciowa(float poziomCieczy)
        {
            const int K1 = -3;
            const int K2 = 100;
            const int HI_LIM = 4095;
            const int LO_LIM = 0;

            return (int)((poziomCieczy - K1) / (K2 - K1) * (HI_LIM - LO_LIM) + LO_LIM);
        }



    }
}
