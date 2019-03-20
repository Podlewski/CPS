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
        public double A { get; set; }
        public double T1 { get; set; }
        public double Ts { get; set; }
        public double D { get; set; }
        public double T { get; set; }
        public double Kw { get; set; }
        public double F { get; set; }
        public double N { get; set; }
        public double N1 { get; set; }
        public double Ns { get; set; }
        public double P { get; set; }
        public double Fp { get; set; }
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

            SelectedSignal = Signals[2];
        }

        public void Plot()
        {
            Generator generator = new Generator()
            {
                Amplitude = A,
                FillFactor = Kw,
                Period = T,
                StartTime = T1,
                JumpTime = Ts,
                JumpN = Ns
            };

            Function = generator.GenerateSinusoidalSignal;

            List<double> pointsX = new List<double>();
            List<double> pointsY = new List<double>();
            List<double> samples = new List<double>();

            for (double i = T1; i < T1 + D; i += 1 / Fp)
            {
                samples.Add(Function(i));
            }
            for (double i = T1; i < T1 + D; i += D / 5000)
            {
                pointsX.Add(i);
                pointsY.Add(Function(i));
            }

            Data.Samples = samples;
            Data.Frequency = Fp;
            Data.StartTime = T1;

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