using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using System;

using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;

using Logic;

namespace ViewModel
{
    public class MainViewModel : BaseViewModel
    {

        #region Commands

        public ICommand AddTabCommand { get; set; }
        public ICommand DrawChartCommand { get; set; }
        public ICommand ComputeCommand { get; set; }

        #endregion

        #region Properties

        public ObservableCollection<TabViewModel> TabList { get; set; }
        public TabViewModel SelectedTab { get; set; }
        public TabViewModel FirstOperationTab { get; set; }
        public TabViewModel SecondOperationTab { get; set; }


        public List<string> SignalList { get; set; }
        public string SelectedSignal { get; set; }

        public List<string> OperationList { get; set; }
        public string SelectedOperation { get; set; }

        #region Factors

        public double A_Amplitude { get; set; }
        public double T1_StartTime { get; set; }
        public double D_DurationOfTheSignal { get; set; }
        public double T_BasicPeroid { get; set; }
        public double Kw_DutyCycle { get; set; }
        public double Ts_TimeStep { get; set; }
        public double P_Probability { get; set; }

        #endregion
        
        #endregion

        public MainViewModel()
        {
            TabList = new ObservableCollection<TabViewModel>();
            AddTab();
            SelectedTab = TabList[0];
            FirstOperationTab = TabList[0];
            SecondOperationTab = TabList[0];

            SignalList = new List<string>()
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
            SelectedSignal = SignalList[2];

            OperationList = new List<string>()
            {
                "1) Dodawanie",
                "2) Odejmowanie",
                "3) Mnożenie",
                "4) Dzielenie",
            };
            SelectedOperation = OperationList[0];

            AddTabCommand = new RelayCommand(AddTab);
            DrawChartCommand = new RelayCommand(Plot);
        }

        public void AddTab()
        {
            TabList.Add(new TabViewModel(TabList.Count));
        }

        public void Plot()
        {
            Generator generator = new Generator()
            {
                A = A_Amplitude,
                T1 = T1_StartTime,
                T = T_BasicPeroid,
                Kw = Kw_DutyCycle,
                Ts = Ts_TimeStep,
                P = P_Probability
            };

            SelectedTab.PointsX = new List<double>();
            SelectedTab.PointsY = new List<double>();

            for (double i = T1_StartTime; i < T1_StartTime + D_DurationOfTheSignal; i += D_DurationOfTheSignal / 500)
            {
                SelectedTab.PointsX.Add(i);
                SelectedTab.PointsY.Add(generator.GenerateSignal(SelectedSignal, i));
            }

            SelectedTab.DrawChart();
        }


    }
}