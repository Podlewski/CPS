using LiveCharts;
using LiveCharts.Configurations;
using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace View.ViewModels
{
    class GraphViewModel
    {
        #region Properties
        public List<string> Signals { get; set; }
        public string SelectedSignal { get; set; }
        public Func<double, double> Function { get; set; }
        public DataHandler Data { get; set; }
        #endregion

        #region Comands
        public ICommand PlotCommand { get; set; }
        #endregion

        #region Factors
        public double A_Amplitude { get; set; }
        public double T1_StartTime { get; set; }
        public double D_DurationOfTheSignal { get; set; }
        public double T_BasicPeroid { get; set; }
        public double Kw_DutyCycle { get; set; }
        #endregion

        #region ChartVars
        public SeriesCollection ChartSeries { get; set; }
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

            SelectedSignal = Signals[0];
        }

        public void Plot()
        {
            Generator generator = new Generator()
            {
                A = A_Amplitude,
                T1 = T1_StartTime,
                T = T_BasicPeroid,
                Kw = Kw_DutyCycle,
            };

            Function = generator.SIn;

            List<double> pointsX = new List<double>();
            List<double> pointsY = new List<double>();
            List<double> samples = new List<double>();

            for (double i = T1_StartTime; i < T1_StartTime + D_DurationOfTheSignal; i += 1 / Fp)
            {
                samples.Add(Function(i));
            }
            for (double i = T1_StartTime; i < T1_StartTime + D_DurationOfTheSignal; i += D_DurationOfTheSignal / 5000)
            {
                pointsX.Add(i);
                pointsY.Add(Function(i));
            }

            Data.Samples = samples;
            Data.Frequency = Fp;
            Data.StartTime = T1_StartTime;

            LoadData(pointsX, pointsY, false);
            DrawChart();
        }

        public void LoadData(DataHandler data)
        {
            Data = data;
        }

        public void LoadData(List<double> x, List<double> y, bool fromSamples)
        {
            if (fromSamples)
            {
                Data.FromSamples = true;
                Data.Samples = y;
            }
            else
            {
                Data.FromSamples = false;
                Data.PointsX = x;
                Data.PointsY = y;
            }
        }

        public void DrawChart()
        {
            if (Data.HasData())
            {
                var mapper = Mappers.Xy<PointXY>()
                    .X(value => value.X)
                    .Y(value => value.Y);
                ChartValues<PointXY> values = new ChartValues<PointXY>();
                List<double> pointsX;
                List<double> pointsY;
                if (Data.FromSamples)
                {
                    pointsX = Data.SamplesX;
                    pointsY = Data.Samples;
                }
                else
                {
                    pointsX = Data.PointsX;
                    pointsY = Data.PointsY;
                }
                for (int i = 0; i < pointsX.Count; i++)

                {
                    values.Add(new PointXY(pointsX[i], pointsY[i]));
                }

                /*if (IsScattered || Data.FromSamples)
                {
                    ChartSeries = new SeriesCollection(mapper)
                    {
                        new ScatterSeries()
                        {
                            PointGeometry = new EllipseGeometry(),
                            StrokeThickness = 5,
                            Values = values
                        }
                    };
                }

                else
                {
                    ChartSeries = new SeriesCollection(mapper)
                    {
                        new LineSeries()
                        {
                            LineSmoothness = 0,
                            StrokeThickness = 0.5,
                            Fill = Brushes.Transparent,
                            PointGeometry = null,
                            Values = values
                        }
                    };
                }*/
            }
        }
    }
}