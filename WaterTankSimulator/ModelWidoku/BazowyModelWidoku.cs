using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymulatorPoziomuCieczy.ModelWidoku
{
    public class BazowyModelWidoku : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void onPropertyChanged(string nazwaWlasnosci)
        {
            if (onPropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(nazwaWlasnosci));
        }
    }
}
