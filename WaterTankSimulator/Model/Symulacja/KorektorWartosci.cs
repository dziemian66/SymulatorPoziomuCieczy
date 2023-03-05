using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymulatorPoziomuCieczy.Model.Symulacja
{
    public static class KorektorWartosci
    {
        public static float KonwertujAnalogInNaWartoscProcentowa(int analogIn)
        {
            return (float)analogIn / 4095 * 100;
        }
        public static float KorektaWejsciaAnalogowego1(float analogIn)
        {
            return 1.009f * analogIn - 0.0552f;
        }
        public static float KorektaWejsciaAnalogowego2(float analogIn)
        {
            return 1.012f * analogIn - 0.0767f;
        }
        public static int KorektaWyjsciaAnalogowego1(float analogOut)
        {
            return (int)(1.0002f * analogOut - 0.005f);
        }
        public static int KorektaWyjsciaAnalogowego2(float analogOut)
        {
            return (int)(1.0011f * analogOut - 0.007f);
        }
    }
}
