using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Timers;


namespace SymulatorPoziomuCieczy.Model.Symulacja
{
    public class Symulator
    {
        public bool SymulacjaTrwa { get; set; } = false;
        public bool[] Przyciski { get; set; } = new bool[2];
        public bool PrzyciskBezpieczenstwa { get; set; } = true;
        public bool[] Lampki { get; set; } = new bool[2];
        public bool PompaNalewajaca { get; set; } // true - pompa nalewająca jest załączona
        public bool PompaWylewajaca { get; set; } // true - pompa wylewajaca jest załączona
        public float PromienZbiornika { get; set; } = 0f; //cm
        public float MaksymalnyPoziomCieczy { get; set; } = 0f;
        public float MinimalnyPoziomCieczy { get; set; } = 0f;
        public float PoczatkowyPoziomCieczy { get; set; } = 0f;
        public float WyplywCieczy { get; set; } = 0f;

        public float PoziomCieczy { get; set; }
        public float OdczytanyPoziomCieczy { get; set; }
        public float NalewanaCiecz { get; set; } //Nalewana ciecz w l/s
        public float ZawartoscCieczyWZbiorniku { get; set; } //Ilosc cieczy w litrach
        public bool[] Plywak { get; set; } = new bool[3] {true, false, false}; // Trzy położenia pływaka
        public float OtwarcieZaworuNalewajacego { get; set; }
        public float MaksymalnaNalewanaCiecz { get; set; }


        public int AnalogIn1 { get; set; } = 0;
        public int AnalogIn2 { get; set; } = 0;
        public int AnalogOut1 { get; set; } = 0;
        public int AnalogOut2 { get; set; } = 0;
        public bool[] DigitalIn { get; set; } = new bool[4];
        public bool[] DigitalOut { get; set; } = new bool[4];

        //Parametry charakterystyki nalewania a*x^3+b*x^2+c*x
        public float ParametrA { get; set; } = 0f;
        public float ParametrB { get; set; } = 0f;
        public float ParametrC { get; set; } = 0f;

        //Wybierz sygnały sterujący oraz wielkość regulowan
        public bool WybranoAI1 { get; set; } = false;
        public bool WybranoAI2 { get; set; } = true;
        public bool WybranoAO1 { get; set; } = true;
        public bool WybranoAO2 { get; set; } = false;
        public double OdchylenieStandardoweSzumu { get; set; } = 0;
        public static int Opoznienie { get; set; } = 0; //x * 100 ms
       

        private float analogIn1Przekonwertowany;
        private float analogIn2Przekonwertowany;

        private static int licznikopoznienia = Opoznienie; 
        private float[] magazynSygnalow = new float[Opoznienie];
        private float otwarcieZaworuBezOpoznienia;

        private Timer czasomierzSymulatora;

        public Symulator()
        {
            //Utwórz czasomierz symulatora
            czasomierzSymulatora = new System.Timers.Timer();
            //Wybór czasu symulacji
            czasomierzSymulatora.Interval = 100; //ms

            //Podłącz zdarzenie Elapsed do czasomierz 
            czasomierzSymulatora.Elapsed += OnTimedEvent;

            // Niech czasomierz uruchamia się ponownie w sposób automatyczny
            czasomierzSymulatora.AutoReset = true;

            // Włącz czasomierz
            czasomierzSymulatora.Enabled = true;

            ZawartoscCieczyWZbiorniku = PoczatkowyPoziomCieczy * PromienZbiornika * PromienZbiornika * 3.14159265f / 1000;
        }

        public void ZmienWartoscOpoznienia(int opoznienie)
        {
            Opoznienie = opoznienie;
            float[] magazynSygnalow = new float[Opoznienie];
        }
        public int ZwrocWartoscOpoznienia()
        {
            return Opoznienie;
        }

        public void SprawdzPlywak()
        {
            if (PoziomCieczy >= 0.5 * MaksymalnyPoziomCieczy)
            {
                Plywak[0] = false;
                Plywak[1] = false;
                Plywak[2] = true;

                DigitalOut[3] = true;
            }
            else if (PoziomCieczy >= 0.48 * MaksymalnyPoziomCieczy && PoziomCieczy < 0.50 * MaksymalnyPoziomCieczy)
            {
                Plywak[0] = false;
                Plywak[1] = true;
                Plywak[2] = false;

                DigitalOut[3] = false;       
            }
            else
            {
                Plywak[0] = true;
                Plywak[1] = false;
                Plywak[2] = false;

                DigitalOut[3] = false;
            }
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            SprawdzPlywak();

            DigitalOut[2] = PrzyciskBezpieczenstwa;
            if (!PrzyciskBezpieczenstwa)
            {
                DigitalOut[0] = false;
                DigitalOut[1] = false;
                DigitalOut[2] = false;
                DigitalOut[3] = false;

                Lampki[0] = false;
                Lampki[1] = false;
                PompaWylewajaca = false;
                PompaNalewajaca = false;

                AnalogIn2 = 0;

            }
            if (SymulacjaTrwa)
            {
                if (!PompaNalewajaca)
                {
                    AnalogIn2 = 0;
                }

                Lampki[0] = DigitalIn[0];
                Lampki[1] = DigitalIn[1];
                PompaWylewajaca = DigitalIn[2];
                PompaNalewajaca = DigitalIn[3];


                DigitalOut[0] = Przyciski[0];
                DigitalOut[1] = Przyciski[1];

                analogIn1Przekonwertowany = KorektorWartosci.KonwertujAnalogInNaWartoscProcentowa(AnalogIn1);
                analogIn1Przekonwertowany = KorektorWartosci.KorektaWejsciaAnalogowego1(analogIn1Przekonwertowany);
                analogIn2Przekonwertowany = KorektorWartosci.KonwertujAnalogInNaWartoscProcentowa(AnalogIn2);
                analogIn2Przekonwertowany = KorektorWartosci.KorektaWejsciaAnalogowego2(analogIn2Przekonwertowany);


                if (WybranoAI1)
                {
                    otwarcieZaworuBezOpoznienia = analogIn1Przekonwertowany;
                }
                else if (WybranoAI2)
                {
                    otwarcieZaworuBezOpoznienia = analogIn2Przekonwertowany;
                }

                //Wprowadzenie opoznienia
                do
                {
                    if (licznikopoznienia > 1)
                    {
                        magazynSygnalow[licznikopoznienia - 1] = magazynSygnalow[licznikopoznienia - 2];
                        licznikopoznienia--;
                    }
                    else
                    {
                        magazynSygnalow[0] = otwarcieZaworuBezOpoznienia;
                        licznikopoznienia = Opoznienie;
                        break;
                    }
                }
                while (licznikopoznienia == Opoznienie);

                OtwarcieZaworuNalewajacego = magazynSygnalow[Opoznienie - 1];

                NalewanaCiecz = LicznikSymulatora.ObliczNalewanaCieczWMiliLitrach(OtwarcieZaworuNalewajacego,ParametrA,ParametrB,ParametrC, PompaNalewajaca);
                ZawartoscCieczyWZbiorniku = LicznikSymulatora.ObliczIloscCieczyWLitrach(ZawartoscCieczyWZbiorniku, NalewanaCiecz, PompaWylewajaca, WyplywCieczy);
                ZawartoscCieczyWZbiorniku = KontrolerLimitow.SprawdzLimitDolny(ZawartoscCieczyWZbiorniku, PromienZbiornika, MinimalnyPoziomCieczy);
                ZawartoscCieczyWZbiorniku = KontrolerLimitow.SprawdzLimitGorny(ZawartoscCieczyWZbiorniku, PromienZbiornika, MaksymalnyPoziomCieczy);
                PoziomCieczy = LicznikSymulatora.ObliczPoziomCieczy(ZawartoscCieczyWZbiorniku, PromienZbiornika);
                OdczytanyPoziomCieczy = SzumGaussowski.DodajSzumyGaussowski(PoziomCieczy, OdchylenieStandardoweSzumu);

                if(WybranoAO1)
                {
                    AnalogOut1 = LicznikSymulatora.ZmienPoziomCieczyNaWartoscAnalogowaWyjsciowa(OdczytanyPoziomCieczy);
                    AnalogOut1 = KorektorWartosci.KorektaWyjsciaAnalogowego1(AnalogOut1);
                }
                else if (WybranoAO2)
                {
                    AnalogOut2 = LicznikSymulatora.ZmienPoziomCieczyNaWartoscAnalogowaWyjsciowa(OdczytanyPoziomCieczy);
                    AnalogOut2 = KorektorWartosci.KorektaWyjsciaAnalogowego2(AnalogOut2);
                }
                MaksymalnaNalewanaCiecz = LicznikSymulatora.ObliczMaksymalneNalewanie(ParametrA, ParametrB, ParametrC);
            }
        }
       
    }

}
