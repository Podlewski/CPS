using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;

using Logic;

namespace ViewModel
{
    public class TabViewModel : BaseViewModel
    {
        #region Properties

        public string TabName { get; set; }

        public SeriesCollection Chart { get; set; }
        public List<double> PointsX { get; set; }
        public List<double> PointsY { get; set; }
        public SeriesCollection Histogram { get; set; }
        public bool IsScattered { get; set; }
        public int AAA { get; set; }

        #endregion

        public TabViewModel(int tabNumber)
        {
            TabName = "Karta " + tabNumber;
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

            Chart = new SeriesCollection(mapper)
            {
                new LineSeries
                {
                    PointGeometry = null,
                    Values = values
                }
            };

            AAA = 3;

            OnPropertyChanged(nameof(AAA));
            OnPropertyChanged(nameof(Chart));
        }

        public override string ToString()
        {
            return TabName;
        }

    }
}
