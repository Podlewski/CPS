using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View
{
    public class DataHandler
    {
        private List<double> _samples;

        public double StartTime { get; set; }
        public double Frequency { get; set; }
        public byte Type { get; set; }

        public List<double> SamplesX { get; set; }
        public List<double> Samples
        {
            get => _samples;
            set
            {
                _samples = value;
                CalculateSamplesX();
            }
        }

        public List<double> PointsX { get; set; }
        public List<double> PointsY { get; set; }

        public bool FromSamples { get; set; }

        public void CalculateSamplesX()
        {
            List<double> points = new List<double>();
            for (int i = 0; i < Samples.Count; i++)
            {
                points.Add(StartTime + i / Frequency);
            }

            SamplesX = points;
        }

        public bool HasData()
        {
            if (FromSamples)
            {
                if (Samples == null || Samples.Count == 0)
                    return false;
                return true;
            }
            if (PointsX == null || PointsX.Count == 0)
                return false;
            if (PointsY == null || PointsY.Count == 0)
                return false;
            return true;

        }
    }
}
