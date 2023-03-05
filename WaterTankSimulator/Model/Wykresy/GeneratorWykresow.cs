using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;


namespace SymulatorPoziomuCieczy.Model.Wykresy
{
    public class GeneratorWykresow : UserControl
    {
        private int licznikWartosci = 1;
        

        public SeriesCollection PoziomCieczyWykres { get; set; }
        public SeriesCollection NalewanaCieczWykres { get; set; }
        public SeriesCollection CharakterystykaNalewania { get; set; }

        public Func<double, string> FormatOsiYPoziomCieczy { get; set; }
        public Func<double, string> FormatOsiYNalewanie { get; set; }
        public Func<double, string> FormatOsiYCharakterystyka { get; set; }
        public GeneratorWykresow()
        {

            PoziomCieczyWykres = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservableValue>
                    {
                        new ObservableValue(0),

                    },
                    PointGeometrySize = 0,
                    StrokeThickness = 2,     
                    Fill = Brushes.Transparent


                },
            };
            NalewanaCieczWykres = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservableValue>
                    {
                     new ObservableValue(0),
                       
                    },
                    PointGeometrySize = 0,
                    StrokeThickness = 2,
                    Stroke = Brushes.Red,
                    Fill = Brushes.Transparent

                },
            };
            CharakterystykaNalewania = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservableValue>
                    {
                     new ObservableValue(0),

                    },
                    PointGeometrySize = 0,
                    StrokeThickness = 2,
                    Stroke = Brushes.LightSlateGray,
                    Fill = Brushes.Transparent
                    
                },
            };
            FormatOsiYNalewanie = value => value.ToString("000.00");
            FormatOsiYPoziomCieczy = value => value.ToString("00.00");
            FormatOsiYCharakterystyka = value => value.ToString("00.00");
            DataContext = this;
        }

        public void DodajWartosci(double poziomCieczy, double nalewanaCiecz)
        {
            poziomCieczy = Math.Round(poziomCieczy,4);
            nalewanaCiecz = Math.Round(nalewanaCiecz, 2);
            
            if(licznikWartosci >= 100)
            {
                PoziomCieczyWykres[0].Values.RemoveAt(0);
                NalewanaCieczWykres[0].Values.RemoveAt(0);
            }

            foreach (var series in PoziomCieczyWykres)
            {
                series.Values.Add(new ObservableValue(poziomCieczy));

            }
            foreach (var series in NalewanaCieczWykres)
            {
                series.Values.Add(new ObservableValue(nalewanaCiecz));

            }
            licznikWartosci++;
        }
        public void GenerujWykresNalewania(float ParametrA, float ParametrB, float ParametrC)
        {
            float y = 0;
            CharakterystykaNalewania[0].Values.Clear();
            for (int i = 0; i < 101; i++)
            {
                y = ParametrA * i * i * i + ParametrB * i * i + ParametrC * i;
                if(y < 0)
                {
                    y = 0;
                }
                foreach (var series in CharakterystykaNalewania)
                {
                    series.Values.Add(new ObservableValue(y));
                }
            }
        }

        public void WyczyscWykres()
        {
            PoziomCieczyWykres[0].Values.Clear();
            NalewanaCieczWykres[0].Values.Clear();
            licznikWartosci = 0;
        }
    }
}
