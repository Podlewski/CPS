using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.ViewModels
{
    class GraphViewModel
    {
        #region Properties
        public List<string> Signals { get; set; }
        public string SelectedSignal { get; set; }
        #endregion


        public GraphViewModel()
        {
            Signals = new List<string>()
            {
                "01) Szum o rozkładzie jednostajnym",
                "02) Szum Gaussowski",
                "03) Sygnał sinusoidalny",
                "04) Sygnał sinusoidalny wyprostowany jednopołówkowo",
                "05) Sygnał sinusoidalny wyprostowany dwupołówkowo",
                "06) Sygnał prostokątny",
                "07) Sygnał prostokątny symetryczny",
                "08) Sygnał trójkątny",
                "09) Skok jednostkowy",
                "10) Impuls jednostkowy",
                "11) Szum impulsowy"
            };

            SelectedSignal = Signals[2];
        }
    }
}
