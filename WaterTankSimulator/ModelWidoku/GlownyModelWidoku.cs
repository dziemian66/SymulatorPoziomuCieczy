using SymulatorPoziomuCieczy.Model.Komunikacja;
using SymulatorPoziomuCieczy.Model.Symulacja;
using SymulatorPoziomuCieczy.Model.Wykresy;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Timers;

namespace SymulatorPoziomuCieczy.ModelWidoku
{
    public class GlownyModelWidoku : BazowyModelWidoku
    {
        public GlownyModelWidoku()
        {
            //Utwórz czasomierz widoku
            czasomierzWidoku = new System.Timers.Timer();

            //Wybór czasu odswiezania widoku
            czasomierzWidoku.Interval = 100;

            //Podłącz zdarzenie Elapsed do czasomierz.
            czasomierzWidoku.Elapsed += OnTimedEvent;

            // Niech czasomierz uruchamia się ponownie w sposób automatyczny
            czasomierzWidoku.AutoReset = true;

            // Włącz czasomierz
            czasomierzWidoku.Enabled = true;
        }

        #region Pola ogólne
        private Timer czasomierzWidoku;
        private Komunikator komunikator = new Komunikator();
        private Symulator symulator = new Symulator();
        private GeneratorWykresow generatorWykresow = new GeneratorWykresow();

        private string digitalIn;
        private string digitalOut;
        #endregion
        #region Inicjalizacja pól i interfejsów komunikatora  
        private string wybranyPort;
        private string wybranaPredkosc;
        private Visibility lampkaStanuKomunikacji = Visibility.Hidden; 
        //private SolidColorBrush kolorPrzyciskuRozpoczeciaKomunikacji = new SolidColorBrush(Color.FromArgb(100, 102, 247, 91));

        private ICommand znajdzPorty = null;
        private ICommand rozpocznijKomunikacje = null;
        private ICommand zakonczKomunikacje = null;
        #endregion
        #region Inicjalizacja pól i interfejsów symulatora    
        private Visibility plywakPolozenie1 = Visibility.Visible;
        private Visibility plywakPolozenie2 = Visibility.Hidden;
        private Visibility plywakPolozenie3 = Visibility.Hidden;
        private Visibility lampka1 = Visibility.Hidden;
        private Visibility lampka2 = Visibility.Hidden;
        private Visibility pompaNalewajaca1 = Visibility.Visible;
        private Visibility pompaNalewajaca2 = Visibility.Hidden;
        private Visibility pompaWylewajaca1 = Visibility.Visible;
        private Visibility pompaWylewajaca2 = Visibility.Hidden;
        private Visibility przycisk1 = Visibility.Visible;
        private Visibility przycisk2 = Visibility.Visible;
        private Visibility przyciskBezpieczenstwa;
        private Visibility lampkaStanuSymulacji = Visibility.Hidden;

        private double wodaNalewanaWysokoscPix = 0;
        private double wodaNalewanaSzerokoscPix = 0;
        private double wodaWylewanaPix = 0;
        private double wodaWZbiornikuPix = 0;

        private int ruchPompy = 0;
        private string przeplyw;
        private string odczytanyPoziomCieczy;
        private string iloscWodyWZbiorniku;

        private ICommand startSymulacji = null;
        private ICommand stopSymulacji = null;
        private ICommand resetSymulacji = null;
        private ICommand wcisnijprzycisk1 = null;
        private ICommand wcisnijprzycisk2 = null;
        private ICommand wcisnijprzyciskBezpieczenstwa = null;
        private ICommand okreslModel = null;
        private ICommand wygenerujCharakterystykeNalewania = null;
        private ICommand wybierzAI1 = null;
        private ICommand wybierzAI2 = null;
        private ICommand wybierzAO1 = null;
        private ICommand wybierzAO2 = null;
        #endregion

        #region Właściwości ogólne aplikacji
        public int AnalogIn1
        {
            get
            {
                return komunikator.AnalogIn1;
            }
            set
            {
                komunikator.AnalogIn1 = value;
                onPropertyChanged(nameof(AnalogIn1));
            }
        }

        public int AnalogIn2
        {
            get
            {
                return komunikator.AnalogIn2;
            }
            set
            {
                komunikator.AnalogIn2 = value;
                onPropertyChanged(nameof(AnalogIn2));
            }
        }

        public int AnalogOut1
        {
            get
            {
                return komunikator.AnalogOut1;
            }
            set
            {
                komunikator.AnalogOut1 = value;
                onPropertyChanged(nameof(AnalogOut1));
            }
        }

        public int AnalogOut2
        {
            get
            {
                return komunikator.AnalogOut2;
            }
            set
            {
                komunikator.AnalogOut2 = value;
                onPropertyChanged(nameof(AnalogOut2));
            }
        }

        public string DigitalIn
        {
            get
            {
                return digitalIn;
            }
            set
            {
                digitalIn = value;
                onPropertyChanged(nameof(DigitalIn));
            }
        }

        public string DigitalOut
        {
            get
            {
                return digitalOut;
            }
            set
            {
                digitalOut = value;
                onPropertyChanged(nameof(DigitalOut));
            }
        }
        #endregion
        #region Właściwości komunikatora
        public string[] ListaPortow
        {
            get
            {
                return komunikator.ListaPortow;
            }
            set
            {
                komunikator.ListaPortow = value;
                onPropertyChanged(nameof(ListaPortow));
            }
        }

        public string WybranyPort
        {
            get 
            { 
                return wybranyPort; 
            }
            set
            {
                wybranyPort = value;
                onPropertyChanged(nameof(WybranyPort));
            }
        }

        public string WybranaPredkosc
        {
            get
            {
                return wybranaPredkosc;
            }
            set
            {
                    wybranaPredkosc = value.Remove(0, 38); //Usun niepotrzebne wartosci stringa
                onPropertyChanged(nameof(WybranaPredkosc));
            }
        }

        public Visibility LampkaStanuKomunikacji
        {
            get
            {
                return lampkaStanuKomunikacji;
            }
            set
            {
                lampkaStanuKomunikacji = value;
                onPropertyChanged(nameof(LampkaStanuKomunikacji));
            }
        }
        #endregion
        #region Właściwości symulatora
        public float MaksymalnyPoziomCieczy
        {
            get
            {
                return symulator.MaksymalnyPoziomCieczy;
            }
            set
            {
                symulator.MaksymalnyPoziomCieczy = value;
                onPropertyChanged(nameof(MaksymalnyPoziomCieczy));
            }
        }
        public float MinimalnyPoziomCieczy
        {
            get
            {
                return symulator.MinimalnyPoziomCieczy;
            }
            set
            {
                symulator.MinimalnyPoziomCieczy = value;
                onPropertyChanged(nameof(MinimalnyPoziomCieczy));
            }
        }
        public float PoczatkowyPoziomCieczy
        {
            get
            {
                return symulator.PoczatkowyPoziomCieczy;
            }
            set
            {
                symulator.PoczatkowyPoziomCieczy = value;
                onPropertyChanged(nameof(PoczatkowyPoziomCieczy));
            }
        }
        public float PromienZbiornika
        {
            get
            {
                return symulator.PromienZbiornika;
            }
            set
            {
                symulator.PromienZbiornika = value;
                onPropertyChanged(nameof(PromienZbiornika));
            }
        }
        public float WyplywCieczy
        {
            get
            {
                return symulator.WyplywCieczy;
            }
            set
            {
                symulator.WyplywCieczy = value;
                onPropertyChanged(nameof(WyplywCieczy));
            }
        }
        public bool WybranoAI1
        {
            get
            {
                return symulator.WybranoAI1;
            }
            set
            {
                symulator.WybranoAI1 = value;
                onPropertyChanged(nameof(WybranoAI1));
            }
        }
        public bool WybranoAI2
        {
            get
            {
                return symulator.WybranoAI2;
            }
            set
            {
                symulator.WybranoAI2 = value;
                onPropertyChanged(nameof(WybranoAI2));
            }
        }
        public bool WybranoAO1
        {
            get
            {
                return symulator.WybranoAO1;
            }
            set
            {
                symulator.WybranoAO1 = value;
                onPropertyChanged(nameof(WybranoAO1));
            }
        }
        public bool WybranoAO2
        {
            get
            {
                return symulator.WybranoAO2;
            }
            set
            {
                symulator.WybranoAO2 = value;
                onPropertyChanged(nameof(WybranoAO2));
            }
        }
        public int OpoznienieNalewania
        {
            get
            {
                return symulator.ZwrocWartoscOpoznienia();
            }
            set
            {    
                symulator.ZmienWartoscOpoznienia(value);
                onPropertyChanged(nameof(OpoznienieNalewania));
            }
        }
        public double OdchylenieStandardoweSzumu
        {
            get
            {
                return symulator.OdchylenieStandardoweSzumu;
            }
            set
            {
                symulator.OdchylenieStandardoweSzumu = value;
                onPropertyChanged(nameof(OdchylenieStandardoweSzumu));
            }
        }

        public float ParametrACharakterystyki
        {
            get
            {
                return symulator.ParametrA;
            }
            set
            {
                symulator.ParametrA = value;
                onPropertyChanged(nameof(ParametrACharakterystyki));
            }
        }
        public float ParametrBCharakterystyki
        {
            get
            {
                return symulator.ParametrB;
            }
            set
            {
                symulator.ParametrB = value;
                onPropertyChanged(nameof(ParametrBCharakterystyki));
            }
        }
        public float ParametrCCharakterystyki
        {
            get
            {
                return symulator.ParametrC;
            }
            set
            {
                symulator.ParametrC = value;
                onPropertyChanged(nameof(ParametrCCharakterystyki));
            }
        }
        public Visibility LampkaStanuSymulacji
        {
            get
            {
                return lampkaStanuSymulacji;
            }
            set
            {
                lampkaStanuSymulacji = value;
                onPropertyChanged(nameof(LampkaStanuSymulacji));
            }
        }
        public Visibility PlywakPolozenie1
        {
            get
            {
                return plywakPolozenie1;
            }
            set
            {
                plywakPolozenie1 = value;
                onPropertyChanged(nameof(PlywakPolozenie1));
            }
        }
        public Visibility PlywakPolozenie2
        {
            get
            {
                return plywakPolozenie2;
            }
            set
            {
                plywakPolozenie2 = value;
                onPropertyChanged(nameof(PlywakPolozenie2));
            }
        }
        public Visibility PlywakPolozenie3
        {
            get
            {
                return plywakPolozenie3;
            }
            set
            {
                plywakPolozenie3 = value;
                onPropertyChanged(nameof(PlywakPolozenie3));
            }
        }
        public Visibility Lampka1
        {
            get
            {
                return lampka1;
            }
            set
            {
                lampka1 = value;
                onPropertyChanged(nameof(Lampka1));
            }
        }
        public Visibility Lampka2
        {
            get
            {
                return lampka2;
            }
            set
            {
                lampka2 = value;
                onPropertyChanged(nameof(Lampka2));
            }
        }

        public Visibility PompaNalewajaca1
        {
            get
            {
                return pompaNalewajaca1;
            }
            set
            {
                pompaNalewajaca1 = value;
                onPropertyChanged(nameof(PompaNalewajaca1));
            }
        }
        public Visibility PompaNalewajaca2
        {
            get
            {
                return pompaNalewajaca2;
            }
            set
            {
                pompaNalewajaca2 = value;
                onPropertyChanged(nameof(PompaNalewajaca2));
            }
        }

        public Visibility PompaWylewajaca1
        {
            get
            {
                return pompaWylewajaca1;
            }
            set
            {
                pompaWylewajaca1 = value;
                onPropertyChanged(nameof(PompaWylewajaca1));
            }
        }
        public Visibility PompaWylewajaca2
        {
            get
            {
                return pompaWylewajaca2;
            }
            set
            {
                pompaWylewajaca2 = value;
                onPropertyChanged(nameof(PompaWylewajaca2));
            }
        }

        public Visibility Przycisk1
        {
            get
            {
                return przycisk1;
            }
            set
            {
                przycisk1 = value;
                onPropertyChanged(nameof(Przycisk1));
            }
        }
        public Visibility Przycisk2
        {
            get
            {
                return przycisk2;
            }
            set
            {
                przycisk2 = value;
                onPropertyChanged(nameof(Przycisk2));
            }
        }
        public Visibility PrzyciskBezpieczenstwa
        {
            get
            {
                return przyciskBezpieczenstwa;
            }
            set
            {
                przyciskBezpieczenstwa = value;
                onPropertyChanged(nameof(PrzyciskBezpieczenstwa));
            }
        }
        public string Przeplyw
        {
            get
            {
                return przeplyw;
            }
            set
            {
                przeplyw = value;
                onPropertyChanged(nameof(Przeplyw));
            }
        }
        public string OdczytanyPoziomCieczy
        {
            get
            {
                return odczytanyPoziomCieczy;
            }
            set
            {
                odczytanyPoziomCieczy = value;
                onPropertyChanged(nameof(OdczytanyPoziomCieczy));
            }
        }

        public string IloscWodyWZbiorniku
        {
            get
            {
                return iloscWodyWZbiorniku;
            }
            set
            {
                iloscWodyWZbiorniku = value;
                onPropertyChanged(nameof(IloscWodyWZbiorniku));
            }
        }

        public double WodaNalewanaWysokoscPix
        {
            get
            {
                return wodaNalewanaWysokoscPix;
            }
            set
            {
                wodaNalewanaWysokoscPix = value;
                onPropertyChanged(nameof(WodaNalewanaWysokoscPix));
            }
        }
        public double WodaNalewanaSzerokoscPix
        {
            get
            {
                return wodaNalewanaSzerokoscPix;
            }
            set
            {
                wodaNalewanaSzerokoscPix = value;
                onPropertyChanged(nameof(WodaNalewanaSzerokoscPix));
            }
        }
        public double WodaWylewanaPix
        {
            get
            {
                return wodaWylewanaPix;
            }
            set
            {
                wodaWylewanaPix = value;
                onPropertyChanged(nameof(WodaWylewanaPix));
            }
        }
        public double WodaWZbiornikuPix
        {
            get
            {
                return wodaWZbiornikuPix;
            }
            set
            {
                wodaWZbiornikuPix = value;
                onPropertyChanged(nameof(WodaWZbiornikuPix));
            }
        }
    
        #endregion
        #region Właściwości wykresów
        private int licznikWykresu = 1;
        public SeriesCollection PoziomCieczyWykres
        {
            get
            {
                return generatorWykresow.PoziomCieczyWykres;
            }
            set
            {
                generatorWykresow.PoziomCieczyWykres = value;
                onPropertyChanged(nameof(PoziomCieczyWykres));
            }
        }
        public SeriesCollection NalewanaCieczWykres
        {
            get
            {
                return generatorWykresow.NalewanaCieczWykres;
            }
            set
            {
                generatorWykresow.NalewanaCieczWykres = value;
                onPropertyChanged(nameof(NalewanaCieczWykres));
            }
        }
        public SeriesCollection CharakterystykaNalewania
        {
            get
            {
                return generatorWykresow.CharakterystykaNalewania;
            }
            set
            {
                generatorWykresow.CharakterystykaNalewania = value;
                onPropertyChanged(nameof(CharakterystykaNalewania));
            }
        }

        public Func<double, string> FormatOsiYNalewanie
        {
            get
            {
                return generatorWykresow.FormatOsiYNalewanie;
            }
            set
            {
                generatorWykresow.FormatOsiYNalewanie = value;
                onPropertyChanged(nameof(FormatOsiYNalewanie));
            }
        }
        public Func<double, string> FormatOsiYCharakterystyka
        {
            get
            {
                return generatorWykresow.FormatOsiYCharakterystyka;
            }
            set
            {
                generatorWykresow.FormatOsiYCharakterystyka = value;
                onPropertyChanged(nameof(FormatOsiYCharakterystyka));
            }
        }
        public Func<double, string> FormatOsiYPoziomCieczy
        {
            get
            {
                return generatorWykresow.FormatOsiYPoziomCieczy;
            }
            set
            {
                generatorWykresow.FormatOsiYPoziomCieczy = value;
                onPropertyChanged(nameof(FormatOsiYPoziomCieczy));
            }
        }

        #endregion

        #region Interfejsy komunikatora
        public ICommand ZnajdzPorty
        {
            get
            {
                if (znajdzPorty == null) znajdzPorty = new RelayCommand(
                    (object o) =>
                    {
                        komunikator.ZnajdzPorty();
                        if(ListaPortow.Length > 0) //Sprawdz czy znaleziono jakiś port
                        {
                            wybranyPort = ListaPortow[0]; // Wybierz pierwszy port z list jako domyślny
                            onPropertyChanged(nameof(WybranyPort)); 
                        }
                        onPropertyChanged(nameof(ListaPortow));
                    },
                    (object o) =>
                    {
                        return true;
                    });
                return znajdzPorty; 
            }
        }

        public ICommand RozpocznijKomunikacje
        {
            get
            {
                if (rozpocznijKomunikacje == null) rozpocznijKomunikacje = new RelayCommand(
                     (object o) =>
                     {
                         if(!komunikator.CzyRozpoczeto())
                         {                           
                             komunikator.RozpocznijKomunikacje(wybranyPort, wybranaPredkosc);
                             lampkaStanuKomunikacji = Visibility.Visible;
                             onPropertyChanged(nameof(LampkaStanuKomunikacji));
                             
                             // KolorPrzyciskuZakonczeniaKomunikacji = new SolidColorBrush(Color.FromArgb(100, 237, 11, 21));

                         }
                         
                     },
                     (object o) =>
                     {
                         if(wybranyPort != null && wybranaPredkosc != null)
                         {
                             return true;
                         }
                         else
                         //MessageBox.Show("Wybierz port, prędkość transmisji oraz sprawdź połączenie!");
                         return false;
                     });
                return rozpocznijKomunikacje;
            }
        }

        public ICommand ZakonczKomunikacje
        {
            get
            {
                if (zakonczKomunikacje == null) zakonczKomunikacje = new RelayCommand(
                     (object o) =>
                     {
                             komunikator.ZakonaczKomunikacje();
                             LampkaStanuKomunikacji = Visibility.Hidden;
                             symulator.SymulacjaTrwa = false;
                             LampkaStanuSymulacji = Visibility.Hidden;
                     },
                     (object o) =>
                     {                       
                             return komunikator.CzyRozpoczeto();              
                     });
                return zakonczKomunikacje;
            }
        }
        #endregion
        #region Interfejsy symulatora
        public ICommand StartSymulacji
        {
            get
            {
                if (startSymulacji == null) startSymulacji = new RelayCommand(
                    (object o) =>
                    {
                        symulator.SymulacjaTrwa = true;
                        LampkaStanuSymulacji = Visibility.Visible;                    
                    },
                    (object o) =>
                    {
                        return komunikator.CzyRozpoczeto();
                    });
                return startSymulacji;
            }
        }
        public ICommand StopSymulacji
        {
            get
            {
                if (stopSymulacji == null) stopSymulacji = new RelayCommand(
                    (object o) =>
                    {
                        symulator.SymulacjaTrwa = false;
                        LampkaStanuSymulacji = Visibility.Hidden;                      
                    },
                    (object o) =>
                    {
                        return symulator.SymulacjaTrwa;
                    });
                return stopSymulacji;
            }
        }
        public ICommand ResetSymulacji
        {
            get
            {
                if (resetSymulacji == null) resetSymulacji = new RelayCommand(
                    (object o) =>
                    {
                        symulator.ZawartoscCieczyWZbiorniku = symulator.PoczatkowyPoziomCieczy * symulator.PromienZbiornika * symulator.PromienZbiornika * 3.14159265f / 1000;
                        generatorWykresow.WyczyscWykres();
                    },
                    (object o) =>
                    {
                        return !symulator.SymulacjaTrwa;
                    });
                return resetSymulacji;
            }
        }


        public ICommand Wcisnijprzycisk1
        {
            get
            {
                if (wcisnijprzycisk1 == null) wcisnijprzycisk1 = new RelayCommand(
                    (object o) =>
                    {
                        if (!symulator.Przyciski[1])
                        {
                            symulator.Przyciski[1] = true;
                            Przycisk1 = Visibility.Hidden;
                        }
                        else
                        {
                            symulator.Przyciski[1] = false;
                            Przycisk1 = Visibility.Visible;
                        }
                    },
                    (object o) =>
                    {
                        return symulator.SymulacjaTrwa;
                    });
                return wcisnijprzycisk1;
            }
        }
        public ICommand Wcisnijprzycisk2
        {
            get
            {
                if (wcisnijprzycisk2 == null) wcisnijprzycisk2 = new RelayCommand(
                    (object o) =>
                    {
                        if (!symulator.Przyciski[0])
                        {
                            symulator.Przyciski[0] = true;
                            Przycisk2 = Visibility.Hidden;
                        }
                        else
                        {
                            symulator.Przyciski[0] = false;
                            Przycisk2 = Visibility.Visible;
                        }
                    },
                    (object o) =>
                    {
                        return symulator.SymulacjaTrwa;
                    });
                return wcisnijprzycisk2;
            }
        }
        public ICommand WcisnijprzyciskBezpieczenstwa
        {
            get
            {
                if (wcisnijprzyciskBezpieczenstwa == null) wcisnijprzyciskBezpieczenstwa = new RelayCommand(
                    (object o) =>
                    {
                        if(symulator.PrzyciskBezpieczenstwa)
                        {
                            symulator.PrzyciskBezpieczenstwa = false;
                            PrzyciskBezpieczenstwa = Visibility.Hidden;
                        }
                        else
                        {
                            symulator.PrzyciskBezpieczenstwa = true;
                            PrzyciskBezpieczenstwa = Visibility.Visible;
                        }
                        onPropertyChanged(nameof(Przycisk1));
                        onPropertyChanged(nameof(Przycisk2));
                    },
                    (object o) =>
                    {
                        return symulator.SymulacjaTrwa;
                    });
                return wcisnijprzyciskBezpieczenstwa;
            }
        }
        public ICommand OkreslModel
        {
            get
            {
                if (okreslModel == null) okreslModel = new RelayCommand(
                    (object o) =>
                    {
                        OknoUstawieńModelu oknoUstawieńModelu = new OknoUstawieńModelu();
                        oknoUstawieńModelu.Show();
                    },
                    (object o) =>
                    {
                        return true;
                    });
                return okreslModel;
            }
        }
        public ICommand WygenerujCharakterystykeNalewania
        {
            get
            {
                if (wygenerujCharakterystykeNalewania == null) wygenerujCharakterystykeNalewania = new RelayCommand(
                    (object o) =>
                    {
                        generatorWykresow.GenerujWykresNalewania(symulator.ParametrA, symulator.ParametrB, symulator.ParametrC);
                    },
                    (object o) =>
                    {
                        return true;
                    });
                return wygenerujCharakterystykeNalewania;
            }
        }
        public ICommand WybierzAI1
        {
            get
            {
                if (wybierzAI1 == null) wybierzAI1 = new RelayCommand(
                    (object o) =>
                    {
                        symulator.WybranoAI1 = true;
                        symulator.WybranoAI2 = false;
                    },
                    (object o) =>
                    {
                        return true;
                    });
                return wybierzAI1;
            }
        }
        public ICommand WybierzAI2
        {
            get
            {
                if (wybierzAI2 == null) wybierzAI2 = new RelayCommand(
                    (object o) =>
                    {
                        symulator.WybranoAI1 = false;
                        symulator.WybranoAI2 = true;
                    },
                    (object o) =>
                    {
                        return true;
                    });
                return wybierzAI2;
            }
        }
        public ICommand WybierzAO1
        {
            get
            {
                if (wybierzAO1 == null) wybierzAO1 = new RelayCommand(
                    (object o) =>
                    {
                        symulator.WybranoAO1 = true;
                        symulator.WybranoAO2 = false;
                    },
                    (object o) =>
                    {
                        return true;
                    });
                return wybierzAO1;
            }
        }
        public ICommand WybierzAO2
        {
            get
            {
                if (wybierzAO2 == null) wybierzAO2 = new RelayCommand(
                    (object o) =>
                    {
                        symulator.WybranoAO1 = false;
                        symulator.WybranoAO2 = true;
                    },
                    (object o) =>
                    {
                        return true;
                    });
                return wybierzAO2;
            }
        }
        #endregion

        #region Metody symulatora
        private double ZmienStanWodyNalewanejWysokosc()
        {
            return (1 - symulator.PoziomCieczy / symulator.MaksymalnyPoziomCieczy) * 386.0 + 69.0;
        }
        private double ZmienStanWodyNalewanejSzerokosc()
        {
            return symulator.NalewanaCiecz / symulator.MaksymalnaNalewanaCiecz * 12.0;
        }
        private double ZmienStanWodyWylewanej()
        {
            if (symulator.PompaWylewajaca && symulator.PoziomCieczy > symulator.MinimalnyPoziomCieczy && symulator.WyplywCieczy > 0)
            {
                return 5.0;
            }
            else
            {
                return 0.0;
            }          
        }
        private double ZmienStanWodyWZbiorniku()
        {
            double stanWodyWPixelach = 0;
            stanWodyWPixelach = symulator.PoziomCieczy / symulator.MaksymalnyPoziomCieczy * 387.0;
            return stanWodyWPixelach;
        }
        private void ZmienStanLampek()
        {
            if(symulator.Lampki[0])
            {
                Lampka1 = Visibility.Visible;
            }
            else
            {
                Lampka1 = Visibility.Hidden;
            }
            if (symulator.Lampki[1])
            {
                Lampka2 = Visibility.Visible;
            }
            else
            {
                Lampka2 = Visibility.Hidden;
            }
        }

        private void ZmienPolozeniePlywaka()
        {
            if (symulator.Plywak[0])
            {
                PlywakPolozenie1 = Visibility.Visible;
            }
            else
            {
                PlywakPolozenie1 = Visibility.Hidden;
            }
            if (symulator.Plywak[1])
            {
                PlywakPolozenie2 = Visibility.Visible;
            }
            else
            {
                PlywakPolozenie2 = Visibility.Hidden;
            }
            if (symulator.Plywak[2])
            {
                PlywakPolozenie3 = Visibility.Visible;
            }
            else
            {
                PlywakPolozenie3 = Visibility.Hidden;
            }
        }

        private void RuchPomp()
        {
            if(symulator.SymulacjaTrwa && symulator.PompaNalewajaca && symulator.PrzyciskBezpieczenstwa)
            {
                if (ruchPompy < 1) 
                {
                    PompaNalewajaca1 = Visibility.Visible;
                    PompaNalewajaca2 = Visibility.Hidden;
                    ruchPompy++;

                }
                else if (ruchPompy < 2)
                {
                    PompaNalewajaca1 = Visibility.Hidden;
                    PompaNalewajaca2 = Visibility.Visible;         
                    ruchPompy++;
                }
                else
                {
                    ruchPompy = 0;
                }
            }
            if (symulator.SymulacjaTrwa && symulator.PompaWylewajaca && symulator.PrzyciskBezpieczenstwa)
            {
                if (ruchPompy < 1)
                {
                    PompaWylewajaca1 = Visibility.Visible;
                    PompaWylewajaca2 = Visibility.Hidden;
                    ruchPompy++;

                }
                else if (ruchPompy < 2)
                {
                    PompaWylewajaca1 = Visibility.Hidden;
                    PompaWylewajaca2 = Visibility.Visible;
                    ruchPompy++;
                }
                else
                {
                    ruchPompy = 0;
                }
            }
        }
        #endregion
        #region Metody ogólne aplikacji

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            AnalogIn1 = komunikator.AnalogIn1;
            symulator.AnalogIn1 = komunikator.AnalogIn1;

            AnalogIn2 = komunikator.AnalogIn2;
            symulator.AnalogIn2 = komunikator.AnalogIn2;

            AnalogOut1 = symulator.AnalogOut1;
            komunikator.AnalogOut1 = symulator.AnalogOut1;

            AnalogOut2 = symulator.AnalogOut2;
            komunikator.AnalogOut2 = symulator.AnalogOut2;

            komunikator.DigitalIn = symulator.DigitalIn;
            DigitalIn = komunikator.DigitalInChar[0].ToString() + komunikator.DigitalInChar[1].ToString() + komunikator.DigitalInChar[2].ToString() + komunikator.DigitalInChar[3].ToString();

            symulator.DigitalOut = komunikator.DigitalOut;
            DigitalOut = komunikator.DigitalOutChar[0].ToString() + komunikator.DigitalOutChar[1].ToString() + komunikator.DigitalOutChar[2].ToString() + komunikator.DigitalOutChar[3].ToString();

            Przeplyw = (symulator.NalewanaCiecz).ToString();
            OdczytanyPoziomCieczy = symulator.OdczytanyPoziomCieczy.ToString();
            IloscWodyWZbiorniku = symulator.ZawartoscCieczyWZbiorniku.ToString();

            ZmienPolozeniePlywaka();
            ZmienStanLampek();
            RuchPomp();
            WodaNalewanaWysokoscPix = ZmienStanWodyNalewanejWysokosc();
            WodaNalewanaSzerokoscPix = ZmienStanWodyNalewanejSzerokosc();
            WodaWylewanaPix = ZmienStanWodyWylewanej();
            WodaWZbiornikuPix = ZmienStanWodyWZbiorniku();

            if (symulator.SymulacjaTrwa && licznikWykresu >= 10) // Odswiezaj wykres co sekunde (100ms x 10)
            {
                generatorWykresow.DodajWartosci(symulator.OdczytanyPoziomCieczy, (double)symulator.NalewanaCiecz);
                licznikWykresu = 0;
            }
            if (symulator.SymulacjaTrwa)
            {
                licznikWykresu++;
            }
        }
       
        #endregion
    }
}
