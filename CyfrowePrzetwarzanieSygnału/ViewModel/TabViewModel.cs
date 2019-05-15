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
using System.Diagnostics;

namespace ViewModel
{
    public class TabViewModel : BaseViewModel
    {
        #region Properties

        public string TabName { get; set; }

        public SeriesCollection Chart { get; set; }
        public bool IsScattered { get; set; }

        public SeriesCollection SamplingChart { get; set; }
        public SeriesCollection QuantizationChart { get; set; }
        public SeriesCollection InterpolationChart { get; set; }
        public SeriesCollection ReconstructionChart { get; set; }

        public SeriesCollection Histogram { get; set; }
        public SignalData SignalData { get; set; }

        public int HistogramStep { get; set; }
        public string[] Labels { get; set; }

        public ICommand SaveCharts { get; set; }

        #region ChartData

        public double AverageValue { get; set; }
        public double AverageAbsValue { get; set; }
        public double RootMeanSquare { get; set; }
        public double Variance { get; set; }
        public double AveragePower { get; set; }

        public double MeanSquaredErrorValue { get; set; }
        public double SignalToNoiseRatioValue { get; set; }
        public double PeakSignalToNoiseRatioValue { get; set; }
        public double MaximumDifferenceValue { get; set; }
        public double EffectiveNumberOfBitsValue { get; set; }

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

        public void DrawCharts(bool reconstruction)
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
                        Values = values,
                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#26A0DA"))
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
                        StrokeThickness = 2,
                        Values = values,
                        Fill = Brushes.Transparent,
                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#26A0DA"))
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
                    CacheMode = new BitmapCache(),
                    Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#26A0DA"))
                }
            };
            Labels = histogramResults.Select(n => n.Item1 + " do " + n.Item2).ToArray();

            OnPropertyChanged(nameof(Chart));
            OnPropertyChanged(nameof(Histogram));

            if (reconstruction)
            {
                ChartValues<Logic.Point> samplingValues = new ChartValues<Logic.Point>();
                ChartValues<Logic.Point> quantizationValues = new ChartValues<Logic.Point>();
                ChartValues<Logic.Point> reconstructionValues = new ChartValues<Logic.Point>();

                for (int i = 0; i < SignalData.ConversionSamplesX.Count; i++)
                    samplingValues.Add(new Logic.Point(SignalData.ConversionSamplesX[i], SignalData.ConversionSamplesY[i]));

                for (int i = 0; i < SignalData.ConversionSamplesX.Count; i++)
                    quantizationValues.Add(new Logic.Point(SignalData.ConversionSamplesX[i], SignalData.QuantizationSamplesY[i]));

                for (int i = 0; i < SignalData.ReconstructionSamplesX.Count; i++)
                    reconstructionValues.Add(new Logic.Point(SignalData.ReconstructionSamplesX[i], SignalData.ReconstructionSamplesY[i]));

                SamplingChart = new SeriesCollection(mapper)
                {
                    new LineSeries()
                    {
                        PointGeometry = null,
                        StrokeThickness = 3,
                        Values = values,
                        Fill = Brushes.Transparent,
                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#34414F"))
                    },
                    new ScatterSeries()
                    {
                        PointGeometry = new EllipseGeometry(),
                        StrokeThickness = 5,
                        Values = samplingValues,
                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#26A0DA"))
                    }
                };

                QuantizationChart = new SeriesCollection(mapper)
                {
                    new LineSeries()
                    {
                        PointGeometry = null,
                        StrokeThickness = 3,
                        Values = values,
                        Fill = Brushes.Transparent,
                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#34414F"))
                    },
                    new StepLineSeries()
                    {
                        PointGeometry = null,
                        StrokeThickness = 3,
                        Values = quantizationValues,
                        Fill = Brushes.Transparent,
                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#26A0DA"))
                    }
                };

                InterpolationChart = new SeriesCollection(mapper)
                {
                    new LineSeries()
                    {
                        PointGeometry = null,
                        StrokeThickness = 3,
                        Values = values,
                        Fill = Brushes.Transparent,
                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#34414F"))
                    },
                    new LineSeries()
                    {
                        PointGeometry = null,
                        StrokeThickness = 3,
                        LineSmoothness = 0,
                        Values = quantizationValues,
                        Fill = Brushes.Transparent,
                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#26A0DA"))
                    }
                };

                ReconstructionChart = new SeriesCollection(mapper)
                {
                    new LineSeries()
                    {
                        PointGeometry = null,
                        StrokeThickness = 3,
                        Values = values,
                        Fill = Brushes.Transparent,
                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#34414F"))
                    },
                    new LineSeries()
                    {
                        PointGeometry = null,
                        StrokeThickness = 3,
                        Values = reconstructionValues,
                        Fill = Brushes.Transparent,
                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#26A0DA"))
                    }
                };

                OnPropertyChanged(nameof(SamplingChart));
                OnPropertyChanged(nameof(QuantizationChart));
                OnPropertyChanged(nameof(InterpolationChart));
                OnPropertyChanged(nameof(ReconstructionChart));
            }
        }


        public override string ToString()
        {
            return TabName;
        }

        public void CalculateSignalInfo(double t1 = 0, double t2 = 0, bool isDiscrete = false)
        {
            List<double> points = SignalData.SamplesY;

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

        public void CalculateReconstructionInfo()
        {
            List<double> points = SignalData.SamplesY;
            List<double> reconstructionPoints = SignalData.ReconstructionSamplesY;

            MeanSquaredErrorValue = Operations.MeanSquaredError(points, reconstructionPoints);
            SignalToNoiseRatioValue = Operations.SignalToNoiseRatio(points, reconstructionPoints);
            PeakSignalToNoiseRatioValue = Operations.PeakSignalToNoiseRatio(points, reconstructionPoints);
            MaximumDifferenceValue = Operations.MaximumDifference(points, reconstructionPoints);
            EffectiveNumberOfBitsValue = Operations.EffectiveNumberOfBits(points, reconstructionPoints);

            OnPropertyChanged(nameof(MeanSquaredErrorValue));
            OnPropertyChanged(nameof(SignalToNoiseRatioValue));
            OnPropertyChanged(nameof(PeakSignalToNoiseRatioValue));
            OnPropertyChanged(nameof(MaximumDifferenceValue));
            OnPropertyChanged(nameof(EffectiveNumberOfBitsValue));
        }

        public void LoadData(SignalData data)
        {
            SignalData = data;
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
                        CacheMode = new BitmapCache(),
                        Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#26A0DA"))
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
