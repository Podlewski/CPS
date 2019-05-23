using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;

using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;

using Logic;

namespace ViewModel
{
    public class AntennaViewModel : BaseViewModel
    {

        public ICommand CountCommand { get; set; }

        private Antenna antenna;

        public SeriesCollection Chart1 { get; set; }
        public SeriesCollection Chart2 { get; set; }
        public SeriesCollection Chart3 { get; set; }

        public int NumberOfMeasurement { get; set; }
        public int BasicSignals { get; set; }
        public double TimeUnit { get; set; }
        public double RealSpeed { get; set; }
        public double AbstractSpeed { get; set; }
        public double SignalPeriod { get; set; }
        public double SamplingFrequency { get; set; }
        public double BuffersLength { get; set; }
        public double ReportingPeriod { get; set; }

        public List<double> OriginalList { get; set; }
        public List<double> CountedList { get; set; }
        public List<double> DiffrenceList { get; set; }

        public AntennaViewModel()
        {
            CountCommand = new RelayCommand(Count);

            OriginalList = new List<double>();
            CountedList = new List<double>();
            DiffrenceList = new List<double>();

            NumberOfMeasurement = 10;
            TimeUnit = 10;
            RealSpeed = 10;
            AbstractSpeed = 3000;
            SignalPeriod = 1;
            BasicSignals = 2;
            SamplingFrequency = 100;
            BuffersLength = 500;
            ReportingPeriod = 2;
        }

        public void Count()
        {
            antenna = new Antenna()
            {
                NumberOfMeasurement = NumberOfMeasurement,
                TimeUnit = TimeUnit,
                RealSpeed = RealSpeed,
                AbstractSpeed = AbstractSpeed,
                SignalPeriod = SignalPeriod,
                BasicSignals = BasicSignals,
                SamplingFrequency = SamplingFrequency,
                BuffersLength = BuffersLength,
                ReportingPeriod = ReportingPeriod
            };

            OriginalList = antenna.GetOriginalDistance();
            CountedList = antenna.CountDistances();
            DiffrenceList = antenna.CountDiffrence(OriginalList, CountedList);

            DrawCharts(antenna);

            OnPropertyChanged(nameof(OriginalList));
            OnPropertyChanged(nameof(CountedList));
            OnPropertyChanged(nameof(DiffrenceList));
        }

        private void DrawCharts(Antenna antenna)
        {
            var mapper = Mappers.Xy<Point>()
                .X(value => value.X)
                .Y(value => value.Y);

            ChartValues<Point> values1 = new ChartValues<Point>();
            ChartValues<Point> values2 = new ChartValues<Point>();
            ChartValues<Point> values3 = new ChartValues<Point>();

            for (int i = 0; i < antenna.ProbingSignal.Count; i++)
                values1.Add(new Point(i, antenna.ProbingSignal[i]));

            for (int i = 0; i < antenna.FeedbackSignal.Count; i++)
                values2.Add(new Point(i, antenna.FeedbackSignal[i]));

            for (int i = 0; i < antenna.CorrelationSamples.Count; i++)
                values3.Add(new Point(i, antenna.CorrelationSamples[i]));

            Chart1 = new SeriesCollection(mapper)
            {
                new LineSeries()
                {
                    PointGeometry = null,
                    StrokeThickness = 2,
                    Values = values1,
                    Fill = Brushes.Transparent,
                    Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#26A0DA"))
                }
            };

            Chart2 = new SeriesCollection(mapper)
            {
                new LineSeries()
                {
                    PointGeometry = null,
                    StrokeThickness = 2,
                    Values = values2,
                    Fill = Brushes.Transparent,
                    Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#26A0DA"))
                }
            };

            Chart3 = new SeriesCollection(mapper)
            {
                new LineSeries()
                {
                    PointGeometry = null,
                    StrokeThickness = 2,
                    Values = values3,
                    Fill = Brushes.Transparent,
                    Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#26A0DA"))
                }
            };

            OnPropertyChanged(nameof(Chart1));
            OnPropertyChanged(nameof(Chart2));
            OnPropertyChanged(nameof(Chart3));
        }
    }
}