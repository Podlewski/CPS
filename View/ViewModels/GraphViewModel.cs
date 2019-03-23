using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;

using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;

using Logic;

namespace View.ViewModels
{
    class GraphViewModel : BaseViewModel
    {
        public ICommand DrawChartCommand { get; set; }

        #region Chart
        public SeriesCollection Chart { get; set; }
        List<double> pointsX;
        List<double> pointsY;
        #endregion

        #region Properties
        public List<string> SignalList { get; set; }
        public string SelectedSignal { get; set; }
        #endregion

        #region Factors
        public double A_Amplitude { get; set; }
        public double T1_StartTime { get; set; }
        public double D_DurationOfTheSignal { get; set; }
        public double T_BasicPeroid { get; set; }
        public double Kw_DutyCycle { get; set; }
        public double Ts_Ts { get; set; }
        public double P_Probability { get; set; }

        #endregion

        public GraphViewModel()
        {
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

            SetStartingChart();
            DrawChartCommand = new RelayCommand(Plot);
        }

        public void Plot()
        {
            Console.WriteLine(SelectedSignal);

            Generator generator = new Generator()
            {
                A = A_Amplitude,
                T1 = T1_StartTime,
                T = T_BasicPeroid,
                Kw = Kw_DutyCycle,
                Ts = Ts_Ts,
                P = P_Probability
            };

            pointsX = new List<double>();
            pointsY = new List<double>();

            for (double i = T1_StartTime; i < T1_StartTime + D_DurationOfTheSignal; i += D_DurationOfTheSignal / 500)
            {
                pointsX.Add(i);
                pointsY.Add(generator.GenerateSignal(SelectedSignal, i));
            }

            DrawChart();
        }

        public void DrawChart()
        {
            var mapper = Mappers.Xy<Point>()
                .X(value => value.X)
                .Y(value => value.Y);

            ChartValues<Point> values = new ChartValues<Point>();

            for(int i = 0; i < pointsX.Count(); i++)
            {
                values.Add(new Point(pointsX[i], pointsY[i]));
            }

            Chart = new SeriesCollection(mapper)
            {
                new LineSeries
                {
                    PointGeometry = null,
                    Values = values
                }
            };

            OnPropertyChanged(nameof(Chart));
        }

        private void SetStartingChart()
        {
            A_Amplitude = 1;
            T1_StartTime = -1 * Math.PI;
            D_DurationOfTheSignal = 2 * Math.PI;
            T_BasicPeroid = Math.PI;

            SelectedSignal = SignalList[2];
            Plot();
        }
    }
}