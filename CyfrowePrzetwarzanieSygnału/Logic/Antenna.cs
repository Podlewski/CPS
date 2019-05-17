using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class Antenna
    {
        private Random Random { get; set; }

        public int Signals { get; set; }
        public double BeginningDistance { get; set; }
        public double TimeUnit { get; set; }
        public double RealSpeed { get; set; }
        public double AbstractSpeed { get; set; }
        public double SignalPeriod { get; set; }
        public double SamplingFrequency { get; set; }
        public double BuffersLength { get; set; }
        public double ReportingPeriod { get; set; }

        public Antenna()
        {
            Random = new Random();
        }

        public List<double> GetOriginalDistance()
        {
            List<double> result = new List<double>();

            for (double i = 0.0; i < 10.0 * ReportingPeriod; i += ReportingPeriod)
                result.Add(BeginningDistance + i * RealSpeed);

            return result;
        }

        public List<double> CountDiffrence(List<double> originalDistance, List<double> countedDistance)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < originalDistance.Count; i++)
                result.Add(Math.Round(Math.Abs(originalDistance[i] - countedDistance[i]), 6));

            return result;
        }

        public List<double> CountDistances()
        {
            List<double> result = new List<double>();
            List<double> amplitues = new List<double>();
            List<double> periods = new List<double>();

            for (int i = 0; i < Signals; i++)
            {
                amplitues.Add(Random.NextDouble() * 50.0 + 1.0);
                periods.Add(Random.NextDouble() * (SignalPeriod - 1e-10) + 1e-10);
            }

            double duration = BuffersLength / SamplingFrequency;

            for (double i = 0.0; i < 10.0 * ReportingPeriod; i += ReportingPeriod)
            {
                double currentDistance = BeginningDistance + i * RealSpeed;
                double propagationTime = 2 * currentDistance / AbstractSpeed;

                List<double> probingSignal = CreateSignal(amplitues, periods, i - duration, duration, SamplingFrequency);
                List<double> feedbackSignal = CreateSignal(amplitues, periods, i - propagationTime, duration, SamplingFrequency);

                List<double> correlationSamples = Operations.IndirectlyCorelateSignals(probingSignal, feedbackSignal);

                result.Add(CalculateDistance(correlationSamples, SamplingFrequency, AbstractSpeed));    
            }

            return result;
        }

        private static double CalculateDistance(List<double> correlation, double frequency, double abstractSpeed)
        {
            var rightHalf = correlation.Skip(correlation.Count / 2).ToList();
            var maxSample = rightHalf.IndexOf(rightHalf.Max());
            var tDelay = maxSample / frequency;

            return Math.Round(((tDelay * abstractSpeed) / 2), 6);
        }


        private List<double> CreateSignal(List<double> amplitudes, List<double> periods, double startTime, double duration, double frequency)
        {
            List<double> samples = new List<double>();

            for (int i = 1; i < amplitudes.Count; i++ )
            {
                List<double> newSamples = new List<double>();

                for (decimal j = (decimal)startTime; j < (decimal)(startTime + duration); j += 1 / (decimal)frequency)
                {
                    newSamples.Add(AntenaSinusoidalSignal(amplitudes[i], periods[i], (double)j));
                }

                if (samples.Count != 0)
                    samples = Operations.AddSignals(newSamples, samples);
                else
                    samples = newSamples;
            }

            return samples;
        }

        private double AntenaSinusoidalSignal(double A, double T, double t)
        {
            return A * Math.Sin((2 * Math.PI / T) * t);
        }
    }
}
