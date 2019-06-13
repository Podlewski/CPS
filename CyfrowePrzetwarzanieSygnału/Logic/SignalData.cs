using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Logic
{
    public class SignalData
    {
        public byte Type { get; set; }

        public double StartTime { get; set; }
        public int Sampling { get; set; }
        public int ConversionSampling { get; set; }
        public List<double> SamplesX { get; set; }
        public List<double> SamplesY { get; set; }
        public List<double> ConversionSamplesX { get; set; }
        public List<double> ConversionSamplesY { get; set; }
        public List<double> QuantizationSamplesY { get; set; }
        public List<double> ReconstructionSamplesY { get; set; }
        public List<double> ReconstructionSamplesX { get; set; }
        public List<Complex> ComplexSamples { get; set; }
        public bool IsDiscrete { get; set; }

        public SignalData()
        {
            Initialize();
        }

        public SignalData(double startTime)
        {
            Initialize();
            StartTime = startTime;
        }

        public SignalData(double startTime, int frequency, int quantizationFrequency)
        {
            Initialize();
            StartTime = startTime;
            Sampling = frequency;
            ConversionSampling = quantizationFrequency;
        }

        private void Initialize()
        {
            IsDiscrete = true;
            SamplesX = new List<double>();
            SamplesY = new List<double>();
            ConversionSamplesX = new List<double>();
            ConversionSamplesY = new List<double>();
            QuantizationSamplesY = new List<double>();
            ReconstructionSamplesX = new List<double>();
            ReconstructionSamplesY = new List<double>();
        }

        public bool IsNotEmpty()
        {
            if (ConversionSamplesX == null || ConversionSamplesX.Count == 0)
                return false;

            if (ConversionSamplesY == null || ConversionSamplesY.Count == 0)
                return false;

            return true;
        }

        public bool IsInvalid(SignalData data, string message)
        {
            message = "Błąd: te sygnały nie pasują do siebie: ";

            if (!data.StartTime.Equals(StartTime))
            {
                message += "inne czasy rozpoczęcia.";
                return true;
            }

            if (data.ConversionSamplesX.Count != ConversionSamplesX.Count)
            {
                message += "niezgodna liczba próbek.";
                return true;
            }

            if (!data.Sampling.Equals(Sampling))
            {
                message += "niezgodna częstotliwość próbkowania.";
                return true;
            }

            return false;
        }

        public List<(double, double, int)> GetDataForHistogram(int count)
        {
            List<(double, double, int)> result = new List<(double, double, int)>(count);

            double max = ConversionSamplesY.Max();
            double min = ConversionSamplesY.Min();

            double range = max - min;
            double interval = range / count;
            for (int i = 0; i < count - 1; i++)
            {
                int points = SamplesY.Count(n => n >= min + (interval * i) && n < min + (interval * (i + 1)));
                result.Add((Math.Round(min + (interval * i), 2), Math.Round(min + (interval * (i + 1)), 2), points));
            }
            int lastPoints = SamplesY.Count(n => n >= min + (interval * (count - 1)) && n <= min + (interval * count));
            result.Add((Math.Round(min + (interval * (count - 1)), 2), Math.Round(min + (interval * count), 2), lastPoints));

            return result;
        }

        public void LoadFromFile(string filePath)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
            {
                SamplesY = new List<double>();
                StartTime = reader.ReadDouble();
                Sampling = reader.ReadInt32();
                Type = reader.ReadByte();

                int length = reader.ReadInt32();
                for (int i = 0; i < length; i++)
                {

                    SamplesY.Add(reader.ReadDouble());
                }

                List<double> points = new List<double>();
                for (int i = 0; i < SamplesY.Count; i++)
                {
                    points.Add(StartTime + (i / Sampling));
                }

                SamplesX = points;
            }
        }

        public void SaveToFile(string filePath)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Create(filePath)))
            {
                writer.Write(StartTime);
                writer.Write(Sampling);
                writer.Write(Type);
                writer.Write(SamplesX.Count);
                foreach (double sample in SamplesY)
                {
                    writer.Write(sample);
                }
            }
            string newPath = Path.ChangeExtension(filePath, ".txt");

            using (StreamWriter writer = new StreamWriter(newPath))
            {
                writer.WriteLine("Start Time: " + StartTime);
                writer.WriteLine("Frequency: " + Sampling);
                writer.WriteLine("Type: " + Type);
                writer.WriteLine("Number of samples: " + SamplesX.Count);
                for (int i = 0; i < SamplesX.Count; i++)
                {
                    writer.WriteLine(i + 1 + ". " + SamplesY[i]);
                }
            }
        }
    }
}
