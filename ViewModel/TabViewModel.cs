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

            //SaveCharts = new RelayCommand(SaveChartsToFile);

            SliderValue = 20;

            LoadHistogram(SliderValue);
        }

        public void DrawCharts()
        {
            var mapper = Mappers.Xy<Logic.Point>()
                .X(value => value.X)
                .Y(value => value.Y);

            ChartValues<Logic.Point> values = new ChartValues<Logic.Point>();

            if (SignalData.UsesSamples)
            {
                for (int i = 0; i < SignalData.SamplesX.Count; i++)
                    values.Add(new Logic.Point(SignalData.SamplesX[i], SignalData.SamplesY[i]));
            }
            else
            {
                for (int i = 0; i < SignalData.PointsX.Count; i++)
                    values.Add(new Logic.Point(SignalData.PointsX[i], SignalData.PointsY[i]));
            }


            if (IsScattered || SignalData.UsesSamples)
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
                        //LineSmoothness = 0,
                        //StrokeThickness = 0.5,
                        //Fill = Brushes.Transparent,
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
            Labels = histogramResults.Select(n => n.Item1 + " to " + n.Item2).ToArray();


            OnPropertyChanged(nameof(Chart));
        }


        public override string ToString()
        {
            return TabName;
        }

        public void CalculateSignalInfo(double t1 = 0, double t2 = 0, bool isDiscrete = false, bool fromSamples = false)
        {
            List<double> points;

            if (fromSamples)
                points = SignalData.SamplesX;

            else
                points = SignalData.PointsY;

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

            //MessageBox.Show(" points  " + str + " sampl " + fromSamples.ToString(), "Done", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void LoadData(SignalData data)
        {
            SignalData = data;
        }

        public void LoadData(List<double> x, List<double> y, bool fromSamples)
        {
            if (fromSamples)
            {
                SignalData.UsesSamples = true;
                SignalData.SamplesY = y;
            }

            else
            {
                SignalData.UsesSamples = false;
                SignalData.PointsX = x;
                SignalData.PointsY = y;
            }
        }

        //public void SaveDataToFile(string path)
        //{
        //    SignalData.SaveToFile(path);
        //}

        //public void LoadDataFromFile(string path)
        //{
        //    SignalData.FromSamples = true;
        //    SignalData.LoadFromFile(path);
        //}

        public void LoadHistogram(int c)
        {
            if (SignalData.HasData())
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

            }
        }

        //#region Save Charts

        //public void SaveChartsToFile()
        //{
        //    var chart = new LiveCharts.Wpf.CartesianChart()
        //    {
        //        Background = new SolidColorBrush(Colors.White),
        //        DisableAnimations = true,
        //        Width = 1920,
        //        Height = 1080,
        //        DataTooltip = null,
        //        Hoverable = false,

        //    };
        //    var histogram = new LiveCharts.Wpf.CartesianChart()
        //    {
        //        Background = new SolidColorBrush(Colors.White),
        //        DisableAnimations = true,
        //        Width = 1920,
        //        Height = 1080,
        //        DataTooltip = null,
        //        Hoverable = false,
        //    };
        //    var mapper = Mappers.Xy<Point>()
        //        .X(value => value.X)
        //        .Y(value => value.Y);

        //    ChartValues<Point> values = new ChartValues<Point>();

        //    for (int i = 0; i < PointsX.Count(); i++)
        //    {
        //        values.Add(new Point(PointsX[i], PointsY[i]));
        //    }

        //    Charts = new SeriesCollection(mapper)
        //    {
        //        new LineSeries
        //        {
        //            PointGeometry = null,
        //            Values = values
        //        }
        //    };

        //    OnPropertyChanged(nameof(Charts));
        //    else
        //    {
        //        chart.Series = new SeriesCollection(mapper)
        //            {
        //                new LineSeries()
        //                {
        //                    LineSmoothness = 0,
        //                    StrokeThickness = 1,
        //                    Fill = Brushes.Transparent,
        //                    PointGeometry = null,
        //                    Values = values,
        //                }
        //            };
        //    }


        //    var histogramResults = Data.GetDataForHistogram(SliderValue);

        //    //HistogramStep = (int)Math.Ceiling(SliderValue / 20.0);
        //    histogram.Series = new SeriesCollection
        //    {
        //        new ColumnSeries
        //        {
        //            Values = new ChartValues<int> (histogramResults.Select(n=>n.Item3)),
        //            ColumnPadding = 0,

        //        }
        //    };
        //    //Labels = histogramResults.Select(n => n.Item1 + " to " + n.Item2).ToArray();
        //    chart.AxisX = new AxesCollection() { new Axis() { FontSize = 20, Title = "t[s]" } };
        //    chart.AxisY = new AxesCollection() { new Axis() { FontSize = 20, Title = "A" } };

        //    histogram.AxisY = new AxesCollection() { new Axis() { FontSize = 20, } };
        //    histogram.AxisX = new AxesCollection() { new Axis() { Title = "Interval", FontSize = 20, Labels = histogramResults.Select(n => n.Item1 + " to " + n.Item2).ToArray(), LabelsRotation = 60, Separator = new LiveCharts.Wpf.Separator() { Step = (int)Math.Ceiling(SliderValue / 20.0) } } };

        //    var viewbox = new Viewbox();
        //    viewbox.Child = chart;
        //    viewbox.Measure(chart.RenderSize);
        //    viewbox.Arrange(new Rect(new Point(0, 0), chart.RenderSize));
        //    chart.Update(true, true); //force chart redraw
        //    viewbox.UpdateLayout();

        //    var histViewbox = new Viewbox();
        //    histViewbox.Child = histogram;
        //    histViewbox.Measure(histogram.RenderSize);
        //    histViewbox.Arrange(new Rect(new Point(0, 0), histogram.RenderSize));
        //    histogram.Update(true, true); //force chart redraw
        //    histViewbox.UpdateLayout();

        //    SaveToPng(chart, "../../../Data/chart.png");
        //    SaveToPng(histogram, "../../../Data/histogram.png");
        //    MessageBox.Show("Files saved", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
        //    //png file was created at the root directory.
        //}

        //private static void EncodeVisual(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        //{
        //    var bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
        //    bitmap.Render(visual);
        //    var frame = BitmapFrame.Create(bitmap);
        //    encoder.Frames.Add(frame);
        //    using (var stream = File.Create(fileName)) encoder.Save(stream);
        //}

        //#endregion
    }
}
