using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class SignalData
    {
        public byte Type { get; set; }

        public double StartTime { get; set; }
        public double Sampling { get; set; }

        public List<double> PointsX { get; set; }
        public List<double> PointsY { get; set; }
        public List<double> SamplesX { get; set; }
        public List<double> SamplesY { get; set; }

        public bool UsesSamples { get; set; }


        public SignalData()
        {
            Initialize();
        }

        public SignalData(double startTime, double sampling)
        {
            Initialize();
            StartTime = startTime;
            Sampling = sampling;
        }

        private void Initialize()
        {
            PointsX = new List<double>();
            PointsY = new List<double>();
            SamplesX = new List<double>();
            SamplesY = new List<double>();
        }

        public bool IsEmpty()
        {
            if (UsesSamples)
            {
                if (SamplesX == null || SamplesX.Count == 0)
                    return false;

                if (SamplesY == null || SamplesY.Count == 0)
                    return false;

                return true;
            }

            if (PointsX == null || PointsX.Count == 0)
                return false;

            if (PointsY == null || PointsY.Count == 0)
                return false;

            return true;
        }

        public bool IsInvalid(SignalData data)
        {
            if (!data.StartTime.Equals(StartTime))
                return true;

            if (data.SamplesX.Count != SamplesX.Count)
                return true;

            if (!data.Sampling.Equals(Sampling))
                return true;

            return false;
        }

        //public void LoadFromFile(string filePath)
        //{
        //    using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
        //    {
        //        Samples = new List<double>();
        //        StartTime = reader.ReadDouble();
        //        Sampling = reader.ReadDouble();
        //        Type = reader.ReadByte();

        //        int length = reader.ReadInt32();
        //        for (int i = 0; i < length; i++)
        //        {

        //            Samples.Add(reader.ReadDouble());
        //        }
        //        CalculateSamplesX();
        //    }
        //}

        //public void SaveToFile(string filePath)
        //{
        //    using (BinaryWriter writer = new BinaryWriter(File.Create(filePath)))
        //    {
        //        writer.Write(StartTime);
        //        writer.Write(Sampling);
        //        writer.Write(Type);
        //        writer.Write(Samples.Count);
        //        foreach (double sample in Samples)
        //        {
        //            writer.Write(sample);
        //        }
        //    }
        //    string newPath = Path.ChangeExtension(filePath, ".txt");

        //    using (StreamWriter writer = new StreamWriter(newPath))
        //    {
        //        writer.WriteLine("Start Time: " + StartTime);
        //        writer.WriteLine("Frequency: " + Sampling);
        //        writer.WriteLine("Type: " + Type);
        //        writer.WriteLine("Number of samples: " + Samples.Count);
        //        for (int i = 0; i < Samples.Count; i++)
        //        {
        //            writer.WriteLine(i + 1 + ". " + Samples[i]);
        //        }
        //    }
        //}

        public List<(double, double, int)> GetDataForHistogram(int count)
        {
            List<(double, double, int)> result = new List<(double, double, int)>(count);
            List<double> pointsY;
            if (UsesSamples)
            {
                pointsY = SamplesY;
            }
            else
            {
                pointsY = PointsY;
            }

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
