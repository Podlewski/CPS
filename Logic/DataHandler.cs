using System;
using System.Collections.Generic;
using System.IO;
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

        public bool IsValid(DataHandler data)
        {
            if (!data.Frequency.Equals(Frequency))
                return false;
            if (!data.StartTime.Equals(StartTime))
                return false;
            if (data.Samples.Count != Samples.Count)
                return false;
            return true;
        }

        public void LoadFromFile(string filePath)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
            {
                Samples = new List<double>();
                StartTime = reader.ReadDouble();
                Frequency = reader.ReadDouble();
                Type = reader.ReadByte();

                int length = reader.ReadInt32();
                for (int i = 0; i < length; i++)
                {

                    Samples.Add(reader.ReadDouble());
                }
                CalculateSamplesX();
            }
        }

        public void SaveToFile(string filePath)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Create(filePath)))
            {
                writer.Write(StartTime);
                writer.Write(Frequency);
                writer.Write(Type);
                writer.Write(Samples.Count);
                foreach (double sample in Samples)
                {
                    writer.Write(sample);
                }
            }
            string newPath = Path.ChangeExtension(filePath, ".txt");

            using (StreamWriter writer = new StreamWriter(newPath))
            {
                writer.WriteLine("Start Time: " + StartTime);
                writer.WriteLine("Frequency: " + Frequency);
                writer.WriteLine("Type: " + Type);
                writer.WriteLine("Number of samples: " + Samples.Count);
                for (int i = 0; i < Samples.Count; i++)
                {
                    writer.WriteLine(i + 1 + ". " + Samples[i]);
                }
            }
        }

        public List<(double, double, int)> GetDataForHistogram(int count)
        {
            List<(double, double, int)> result = new List<(double, double, int)>(count);
            List<double> pointsY;
            if (FromSamples)
                pointsY = Samples;
            else
                pointsY = PointsY;

            double max = pointsY.Max();
            double min = pointsY.Min();

            double range = max - min;
            double interval = range / count;
            for (int i = 0; i < count - 1; i++)
            {
                int points = pointsY.Count(n => n >= min + (interval * i) && n < min + (interval * (i + 1)));
                result.Add((Math.Round(min + (interval * i), 2), Math.Round(min + (interval * (i + 1)), 2), points));
            }
            int lastPoints = pointsY.Count(n => n >= min + (interval * (count - 1)) && n <= min + (interval * count));
            result.Add((Math.Round(min + (interval * (count - 1)), 2), Math.Round(min + (interval * count), 2), lastPoints));

            return result;
        }
    }
}
