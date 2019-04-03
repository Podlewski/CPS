using System;
using System.Collections.Generic;
using System.Linq;

using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;

using Logic;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace ViewModel
{
    public class TabViewModel : BaseViewModel
    {
        #region Properties

        public string TabName { get; set; }

        public SignalData SignalData { get; set; }
        public SeriesCollection Chart { get; set; }
        public bool IsScattered { get; set; }
        public SeriesCollection Histogram { get; set; }

        public int HistogramStep { get; set; }
        public string[] Labels { get; set; }

        public ICommand SaveCharts { get; set; }

        #region ChartData

        public double AverageValue { get; set; }
        public double AverageAbsValue { get; set; }
        public double RootMeanSquare { get; set; }
        public double Variance { get; set; }
        public double AveragePower { get; set; }

        #endregion

        #region Slider

        public int Slider { get; set; }
        public int SliderValue
        {
            get => Slider;
            set
            {
                Slider = value;
                LoadHistogram(Slider);
            } 
        }

        #endregion

        #endregion

        public TabViewModel(int tabNumber)
        {
            TabName = "Karta " + tabNumber;

            SignalData = new SignalData();

            SliderValue = 10;
        }

        public void DrawCharts()
        {
            var mapper = Mappers.Xy<Logic.Point>()
                .X(value => value.X)
                .Y(value => value.Y);

            ChartValues<Logic.Point> values = new ChartValues<Logic.Point>();

            for (int i = 0; i < SignalData.SamplesX.Count; i++)
                values.Add(new Logic.Point(SignalData.SamplesX[i], SignalData.SamplesY[i]));


            if (IsScattered)
            {
                Chart = new SeriesCollection(mapper)
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
                Chart = new SeriesCollection(mapper)
                {
                    new LineSeries()
                    {
                        PointGeometry = null,
                        Values = values
                    }
                };
            }

            var histogramResults = SignalData.GetDataForHistogram(SliderValue);
            HistogramStep = 1;
            Histogram = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Values = new ChartValues<int> (histogramResults.Select(n=>n.Item3)),
                        ColumnPadding = 0,
                        CacheMode = new BitmapCache()
                    }
                };
            Labels = histogramResults.Select(n => n.Item1 + " do " + n.Item2).ToArray();

            OnPropertyChanged(nameof(Histogram));
            OnPropertyChanged(nameof(Chart));
        }


        public override string ToString()
        {
            return TabName;
        }

        public void CalculateSignalInfo(double t1 = 0, double t2 = 0, bool isDiscrete = false, bool fromSamples = false)
        {
            List<double> points;

            points = SignalData.SamplesY;

            AverageValue = Operations.Average(points, t1, t2, isDiscrete);
            AverageAbsValue = Operations.AbsAverage(points, t1, t2, isDiscrete);
            RootMeanSquare = Operations.RootMeanSquare(points, t1, t2, isDiscrete);
            Variance = Operations.Variance(points, t1, t2, isDiscrete);
            AveragePower = Operations.AveragePower(points, t1, t2, isDiscrete);

            OnPropertyChanged(nameof(AverageValue));
            OnPropertyChanged(nameof(AverageAbsValue));
            OnPropertyChanged(nameof(RootMeanSquare));
            OnPropertyChanged(nameof(Variance));
            OnPropertyChanged(nameof(AveragePower));
        }

        public void LoadData(SignalData data)
        {
            SignalData = data;
        }

        public void LoadData(List<double> x, List<double> y, bool fromSamples)
        {
            //if (fromSamples)
            //{
            //    SignalData.UsesSamples = true;
            //    SignalData.SamplesY = y;
            //}

            //else
            //{
            //    SignalData.UsesSamples = false;
            //    SignalData.PointsX = x;
            //    SignalData.PointsY = y;
            //}
        }

        public void LoadHistogram(int c)
        {
            if (SignalData.IsEmpty())
            {
                var histogramResults = SignalData.GetDataForHistogram(c);
                HistogramStep = (int)Math.Ceiling(c / 20.0);
                Histogram = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Values = new ChartValues<int> (histogramResults.Select(n=>n.Item3)),
                        ColumnPadding = 0,
                        CacheMode = new BitmapCache()
                    }
                };
                Labels = histogramResults.Select(n => n.Item1 + " to " + n.Item2).ToArray();

                OnPropertyChanged(nameof(Histogram));
            }
        }

        #region Save Charts

        public void SaveChartsToFile()
        {
            var chart = new LiveCharts.Wpf.CartesianChart()
            {
                Background = new SolidColorBrush(Colors.White),
                DisableAnimations = true,
                Width = 1920,
                Height = 1080,
                DataTooltip = null,
                Hoverable = false,

            };

            var histogram = new LiveCharts.Wpf.CartesianChart()
            {
                Background = new SolidColorBrush(Colors.White),
                DisableAnimations = true,
                Width = 1920,
                Height = 1080,
                DataTooltip = null,
                Hoverable = false,
            };

            var mapper = Mappers.Xy<Logic.Point>()
                .X(value => value.X)
                .Y(value => value.Y);

            ChartValues<Logic.Point> values = new ChartValues<Logic.Point>();

            for (int i = 0; i < SignalData.SamplesX.Count; i++)
                values.Add(new Logic.Point(SignalData.SamplesX[i], SignalData.SamplesY[i]));

            if (IsScattered)
            {
                Chart = new SeriesCollection(mapper)
                {
                    new LineSeries
                    {
                        PointGeometry = null,
                        Values = values
                    }
                };
            }

            else
            {
                chart.Series = new SeriesCollection(mapper)
                    {
                        new LineSeries()
                        {
                            LineSmoothness = 0,
                            StrokeThickness = 1,
                            Fill = Brushes.Transparent,
                            PointGeometry = null,
                            Values = values,
                        }
                    };
            }

            OnPropertyChanged(nameof(Chart));

            var histogramResults = SignalData.GetDataForHistogram(SliderValue);

            histogram.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = new ChartValues<int> (histogramResults.Select(n=>n.Item3)),
                    ColumnPadding = 0,
                }
            };

            chart.AxisX = new AxesCollection() { new Axis() { FontSize = 20, Title = "t[s]" } };
            chart.AxisY = new AxesCollection() { new Axis() { FontSize = 20, Title = "A" } };

            histogram.AxisY = new AxesCollection() { new Axis() { FontSize = 20, } };
            histogram.AxisX = new AxesCollection() { new Axis() { Title = "Interval", FontSize = 20, Labels = histogramResults.Select(n => n.Item1 + " to " + n.Item2).ToArray(), LabelsRotation = 60, Separator = new LiveCharts.Wpf.Separator() { Step = (int)Math.Ceiling(SliderValue / 20.0) } } };

            var viewbox = new Viewbox();
            viewbox.Child = chart;
            viewbox.Measure(chart.RenderSize);
            viewbox.Arrange(new Rect(new System.Windows.Point(0, 0), chart.RenderSize));
            chart.Update(true, true); 
            viewbox.UpdateLayout();

            var histViewbox = new Viewbox();
            histViewbox.Child = histogram;
            histViewbox.Measure(histogram.RenderSize);
            histViewbox.Arrange(new Rect(new System.Windows.Point(0, 0), histogram.RenderSize));
            histogram.Update(true, true); 
            histViewbox.UpdateLayout();

            MessageBox.Show("Files saved", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void SaveDataToFile(string path)
        {
            SignalData.SaveToFile(path);
        }

        public void LoadDataFromFile(string path)
        {
            SignalData.LoadFromFile(path);
        }

        #endregion
    }
}
