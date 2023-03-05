using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Windows.Threading;
using System.Threading;
using SymulatorPoziomuCieczy.Model.Komunikacja;
using SymulatorPoziomuCieczy.Model.Symulacja;
using SymulatorPoziomuCieczy.ModelWidoku;

namespace SymulatorPoziomuCieczy.Aplikacja
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }
        private void UstawModelWidoku(object sender, RoutedEventArgs e)
        {
            OknoUstawieńModelu oknoUstawieńModelu = new OknoUstawieńModelu();
            oknoUstawieńModelu.DataContext=this.DataContext;
            oknoUstawieńModelu.Show();
        }

        private void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}

