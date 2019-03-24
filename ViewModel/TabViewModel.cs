using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;

using Logic;
using View;
using System.Windows.Input;

namespace ViewModel
{
    public class TabViewModel : BaseViewModel
    {
        #region Properties

        public string TabName { get; set; }

        public DataHandler Data { get; set; }
        public int Slider;

        public SeriesCollection Charts { get; set; }
        public List<double> PointsX { get; set; }
        public List<double> PointsY { get; set; }
        public SeriesCollection Histograms { get; set; }
        public bool IsScattered { get; set; }

        public ICommand Histogram { get; set; }
        public ICommand SaveCharts { get; set; }

        #region ChartData

        public double AverageValue { get; set; }
        public double AverageAbsValue { get; set; }
        public double RootMeanSquare { get; set; }
        public double Variance { get; set; }
        public double AveragePower { get; set; }

        #endregion

        #endregion

        public TabViewModel(int tabNumber)
        {
            TabName = "Karta " + tabNumber;
            Histogram = new RelayCommand<int>(LoadHistogram);
            SaveCharts = new RelayCommand(SaveChartsToFile);
            SliderValue = 15;
        }

        public void DrawChart()
        {
            var mapper = Mappers.Xy<Point>()
                .X(value => value.X)
                .Y(value => value.Y);

            ChartValues<Point> values = new ChartValues<Point>();

            for (int i = 0; i < PointsX.Count(); i++)
            {
                values.Add(new Point(PointsX[i], PointsY[i]));
            }

            Charts = new SeriesCollection(mapper)
            {
                new LineSeries
                {
                    PointGeometry = null,
                    Values = values
                }
            };

            OnPropertyChanged(nameof(Charts));
        }

        public int SliderValue
        {
            get => Slider;
            set
            {
                Slider = value;
                LoadHistogram(Slider);
            }
        }

        public override string ToString()
        {
            return TabName;
        }

        public void CalculateSignalInfo(double t1 = 0, double t2 = 0, bool isDiscrete = false, bool fromSamples = false)
        {
            List<double> points;

            if (fromSamples)
                points = Data.Samples;
            else
                points = Data.PointsY;

            AverageValue = Operations.Average(points, t1, t2, isDiscrete);
            AverageAbsValue = Operations.AbsAverage(points, t1, t2, isDiscrete);
            RootMeanSquare = Operations.RootMeanSquare(points, t1, t2, isDiscrete); 
            Variance = Operations.Variance(points, t1, t2, isDiscrete);
            AveragePower = Operations.AveragePower(points, t1, t2, isDiscrete);
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

        public void SaveDataToFile(string path)
        {
            Data.SaveToFile(path);
        }

        public void LoadDataFromFile(string path)
        {
            Data.FromSamples = true;
            Data.LoadFromFile(path);
        }

    }
}
