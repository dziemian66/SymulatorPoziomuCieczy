using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading;
using System.Timers;

using SymulatorPoziomuCieczy;



namespace SymulatorPoziomuCieczy.Model.Komunikacja
{
    public class Komunikator
    {
        //Serial 
        SerialPort sp = new SerialPort(); 
        public string[] ListaPortow { get; set; }
        public int AnalogIn1 { get; set; } = 0;
        public int AnalogIn2 { get; set; } = 0;
        public int AnalogOut1 { get; set; } = 0;
        public int AnalogOut2 { get; set; } = 0;

        public bool[] DigitalIn { get; set; } = new bool[4];
        public char[] DigitalInChar { get; set; } = new char[4] { '0', '0', '0', '0' };
        public bool[] DigitalOut { get; set; } = new bool[4];
        public char[] DigitalOutChar { get; set; } = new char[4] { '0', '0', '0', '0' };

        private System.Timers.Timer aTimer;
        //private DispatcherTimer czasomierzKomunikatora;
        private string odebraneDane;
        private string ramkaDanych = "";
        private string daneWyslane = "";
        private string wyslanaSumaKontrolnaString;

        private int resztaRamki = 0;
        private int poczatekRamki = 0; //Znajdź początek ramki
        private int koniecRamki = 0; //Znajdź koniec ramki
        private int odebranaSumaKontrolna;
        private int obliczonaSumaKontrolna;
        private int wyslanaSumaKontrolna;

        private int analogIn1PrzedSprawdzeniem = 0;
        private int analogIn2PrzedSprawdzeniem = 0;

        private string analogOut1String;
        private string analogOut2String;
        private static int analogOut1Kopia = 0;
        private static int analogOut2Kopia = 0;

        public Komunikator()
        {
            ListaPortow = SerialPort.GetPortNames();

            //Utwórz czasomierz komunikatora
            aTimer = new System.Timers.Timer();
            //Wybór czasu komunikacji
            aTimer.Interval = 100;

            //Podłącz zdarzenie Elapsed do czasomierz 
            aTimer.Elapsed += OnTimedEvent;

            // Niech czasomierz uruchamia się ponownie w sposób automatyczny
            aTimer.AutoReset = true;

            //Włącz czasomierz
            aTimer.Enabled = true;

        }
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (CzyRozpoczeto())
            {
                WyslijDane();
            }
        }


        public void ZnajdzPorty()
        {
            ListaPortow = SerialPort.GetPortNames();
        }

        public void RozpocznijKomunikacje(string wybranyPort, string wybranaPredkosc)
        {  
            sp.PortName = wybranyPort;
            sp.BaudRate = Convert.ToInt32(wybranaPredkosc);
            sp.Open();
            //Wywołanie funkcji wyłącznie, gdy port szeregowy ma dane do odczytu
            sp.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(OdbierzDane);
        }

        public void ZakonaczKomunikacje()
        {
            sp.Close();
        }


        private delegate void UpdateUiTextDelegate(string text);
        private void OdbierzDane(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            odebraneDane = sp.ReadExisting();
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(OdczytajOdebraneDane), odebraneDane);
        }

        private void OdczytajOdebraneDane(string text)
        {
            ramkaDanych = ramkaDanych + text;
            if (ramkaDanych.Length >= (19 + resztaRamki))
            {
                while (true)
                {
                    //Znajdź początek i koniec ramki
                    if (ramkaDanych[poczatekRamki] != ':') poczatekRamki++;
                    if (ramkaDanych[koniecRamki] == '\n') break;
                    else if ((ramkaDanych.Length - 1) > koniecRamki) koniecRamki++;
                }

                if (poczatekRamki < koniecRamki)
                {
                    //Przetwarzanie ramki danych z STM
                    analogIn1PrzedSprawdzeniem = (ramkaDanych[poczatekRamki + 1] - 48) * 1000 + (ramkaDanych[poczatekRamki + 2] - 48) * 100 + (ramkaDanych[poczatekRamki + 3] - 48) * 10 + (ramkaDanych[poczatekRamki + 4] - 48);
                    analogIn2PrzedSprawdzeniem = (ramkaDanych[poczatekRamki + 5] - 48) * 1000 + (ramkaDanych[poczatekRamki + 6] - 48) * 100 + (ramkaDanych[poczatekRamki + 7] - 48) * 10 + (ramkaDanych[poczatekRamki + 8] - 48);
                    DigitalInChar[0] = ramkaDanych[poczatekRamki + 9];
                    DigitalInChar[1] = ramkaDanych[poczatekRamki + 10];
                    DigitalInChar[2] = ramkaDanych[poczatekRamki + 11];
                    DigitalInChar[3] = ramkaDanych[poczatekRamki + 12];

                    //Odbierana i obliczana suma kontrolna
                    obliczonaSumaKontrolna = analogIn1PrzedSprawdzeniem + analogIn2PrzedSprawdzeniem + (DigitalInChar[0] - 48) + (DigitalInChar[1] - 48) + (DigitalInChar[2] - 48) + (DigitalInChar[3] - 48);
                    odebranaSumaKontrolna = (ramkaDanych[poczatekRamki + 13] - 48) * 1000 + (ramkaDanych[poczatekRamki + 14] - 48) * 100 + (ramkaDanych[poczatekRamki + 15] - 48) * 10 + (ramkaDanych[poczatekRamki + 16] - 48);
                    //Jeśli otrzymane dane są zgodne z sumą kontrolną to przypisz dane
                    if (obliczonaSumaKontrolna == odebranaSumaKontrolna && ramkaDanych[poczatekRamki + 17] == '\r' && ramkaDanych[poczatekRamki + 18] == '\n')
                    {
                        if (DigitalInChar[0] == '1')
                        {
                            DigitalIn[0] = true;
                        }
                        else
                        {
                            DigitalIn[0] = false;
                        }

                        if (DigitalInChar[1] == '1')
                        {
                            DigitalIn[1] = true;
                        }
                        else
                        {
                            DigitalIn[1] = false;
                        }

                        if (DigitalInChar[2] == '1')
                        {
                            DigitalIn[2] = true;
                        }
                        else
                        {
                            DigitalIn[2] = false;
                        }

                        if (DigitalInChar[3] == '1')
                        {
                            DigitalIn[3] = true;
                        }
                        else
                        {
                            DigitalIn[3] = false;
                        }
                        
                        AnalogIn1 = analogIn1PrzedSprawdzeniem;
                        AnalogIn2 = analogIn2PrzedSprawdzeniem;
                        ramkaDanych = "";
                        poczatekRamki = 0;
                        koniecRamki = 0;
                        resztaRamki = 0;
                    }
                }
                //Pobierz więcej danych jeśli ramka nie jest pełna
                else if ((koniecRamki + 1) == poczatekRamki)
                {
                    if ((ramkaDanych.Length - poczatekRamki) < 20)
                        resztaRamki = 20 - (ramkaDanych.Length - poczatekRamki);
                    else
                    {
                        resztaRamki = ramkaDanych.Length - poczatekRamki;
                        koniecRamki = poczatekRamki;
                    }
                    return;
                }
            }
        }

        public void WyslijDane()
        {
            analogIn2PrzedSprawdzeniem = AnalogOut1;
            analogOut1Kopia = AnalogOut1;
            analogOut2Kopia = AnalogOut2;

            #region Wysyłanie danych
            //Uzupełnij zerami
            if (AnalogOut1 < 10) analogOut1String = "000" + analogOut1Kopia.ToString();
            else if (AnalogOut1 < 100) analogOut1String = "00" + analogOut1Kopia.ToString();
            else if (AnalogOut1 < 1000) analogOut1String = "0" + analogOut1Kopia.ToString();
            else if (AnalogOut1 > 9999) analogOut1String = "9999";
            else analogOut1String = analogOut1Kopia.ToString();

            if (AnalogOut2 < 10) analogOut2String = "000" + analogOut2Kopia.ToString();
            else if (AnalogOut2 < 100) analogOut2String = "00" + analogOut2Kopia.ToString();
            else if (AnalogOut2 < 1000) analogOut2String = "0" + analogOut2Kopia.ToString();
            else if (AnalogOut2 > 9999) analogOut2String = "9999";
            else analogOut2String = analogOut2Kopia.ToString();

            //Wysylana suma kontrolna
            wyslanaSumaKontrolna = analogOut1Kopia + analogOut2Kopia + (DigitalOutChar[0] - 48) + (DigitalOutChar[1] - 48) + (DigitalOutChar[2] - 48) + (DigitalOutChar[3] - 48);
            if (wyslanaSumaKontrolna < 10) wyslanaSumaKontrolnaString = "000" + wyslanaSumaKontrolna.ToString();
            else if (wyslanaSumaKontrolna < 100) wyslanaSumaKontrolnaString = "00" + wyslanaSumaKontrolna.ToString();
            else if (wyslanaSumaKontrolna < 1000) wyslanaSumaKontrolnaString = "0" + wyslanaSumaKontrolna.ToString();
            else wyslanaSumaKontrolnaString = wyslanaSumaKontrolna.ToString();
            //Zmien stan wyjsc cyfrowych na tekst;
            if(DigitalOut[0])
            {
                DigitalOutChar[0] = '1';
            }
            else
            {
                DigitalOutChar[0] = '0';
            }
               
            if (DigitalOut[1])
            {
                DigitalOutChar[1] = '1';
            }
            else
            {
                DigitalOutChar[1] = '0';
            }

            if (DigitalOut[2])
            {
                DigitalOutChar[2] = '1';
            }
            else
            {
                DigitalOutChar[2] = '0';
            }

            if (DigitalOut[3])
            {
                DigitalOutChar[3] = '1';
            }
            else
            {
                DigitalOutChar[3] = '0';
            }

            //Wysyłana paczka danych
            daneWyslane = ":" + analogOut1String + analogOut2String + DigitalOutChar[0] + DigitalOutChar[1] + DigitalOutChar[2] + DigitalOutChar[3] + wyslanaSumaKontrolnaString;
            //Wysłanie danych do STM
            if (sp.IsOpen)
                sp.Write(daneWyslane);
            #endregion
        }

        public bool CzyRozpoczeto()
        {
            return sp.IsOpen;
        }

    }
}
