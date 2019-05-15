using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public static class Operations
    {
        public static List<double> AddSignals(List<double> signal1, List<double> signal2)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < signal1.Count; i++)
            {
                result.Add(signal1[i] + signal2[i]);
            }

            return result;
        }


        public static List<double> SubtractSignals(List<double> signal1, List<double> signal2)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < signal1.Count; i++)
            {
                result.Add(signal1[i] - signal2[i]);
            }

            return result;
        }

        public static List<double> MultiplySignals(List<double> signal1, List<double> signal2)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < signal1.Count; i++)
            {
                result.Add(signal1[i] * signal2[i]);
            }

            return result;
        }

        public static List<double> DivideSignals(List<double> signal1, List<double> signal2)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < signal1.Count; i++)
            {
                if (signal2[i] != 0)
                    result.Add(signal1[i] / signal2[i]);
            }

            return result;
        }

        public static double Average(List<double> samples, double t1 = 0, double t2 = 0, bool isDiscrete = true)
        {
            double result;

            if (isDiscrete)
            {
                result = 1.0 / samples.Count * Sum(samples);
            }
            else
            {
                result = 1 / (t2 - t1) * Integral(Math.Abs((t2 - t1) / samples.Count), samples);
            }

            return Math.Round(result, 4, MidpointRounding.AwayFromZero);
        }

        public static double Variance(List<double> samples, double t1 = 0, double t2 = 0, bool isDiscrete = true)
        {
            double result;

            if (isDiscrete)
            {
                result = 1.0 / samples.Count * Sum(samples, d => Math.Pow(d - Average(samples, isDiscrete: true), 2));
            }
            else
            {
                result = 1 / (t2 - t1) * Integral(Math.Abs((t2 - t1) / samples.Count), samples, d => Math.Pow(d - Average(samples, t1, t2), 2));
            }

            return Math.Round(result, 2, MidpointRounding.AwayFromZero);
        }
        public static double AbsAverage(List<double> samples, double t1 = 0, double t2 = 0, bool isDiscrete = true)
        {
            double result;

            if (isDiscrete)
            {
                result = 1.0 / samples.Count * Sum(samples, Math.Abs);
            }
            else
            {
                result = 1 / (t2 - t1) * Integral(Math.Abs((t2 - t1) / samples.Count), samples, Math.Abs);
            }

            return Math.Round(result, 4, MidpointRounding.AwayFromZero);
        }

        public static double AveragePower(List<double> samples, double t1 = 0, double t2 = 0, bool isDiscrete = true)
        {
            double result;

            if (isDiscrete)
            {
                result = 1.0 / samples.Count * Sum(samples, d => d * d);
            }
            else
            {
                result = 1 / (t2 - t1) * Integral(Math.Abs((t2 - t1) / samples.Count), samples, d => d * d);
            }

            return Math.Round(result, 4, MidpointRounding.AwayFromZero);
        }

        public static double RootMeanSquare(List<double> samples, double t1 = 0, double t2 = 0, bool isDiscrete = true)
        {
            double result;

            if (isDiscrete)
            {
                result = Math.Sqrt(AveragePower(samples, isDiscrete: true));
            }
            else
            {
                result = Math.Sqrt(AveragePower(samples, t1, t2));
            }

            return Math.Round(result, 4, MidpointRounding.AwayFromZero);
        }

        private static double Integral(double dx, List<double> samples, Func<double, double> additionalFunc = null)
        {
            double integral = 0;
            foreach (var sample in samples)
            {
                if (additionalFunc != null)
                    integral += additionalFunc(sample);
                else
                    integral += sample;
            }

            integral *= dx;

            return integral;
        }

        private static double Sum(List<double> samples, Func<double, double> additionalFunc = null)
        {
            double sum = 0;
            foreach (var sample in samples)
            {
                if (additionalFunc != null)
                    sum += additionalFunc(sample);
                else
                    sum += sample;
            }

            return sum;
        }

        public static double MeanSquaredError(List<double> orignalSignal, List<double> sampledSignal)
        {
            List<double> quantizedSignal = QuantizedSignal(orignalSignal.Count(), sampledSignal);

            int N = sampledSignal.Count;
            double fraction = 1.0 / N;
            double sum = 0;

            for (int i = 0; i < N; i++)
            {
                sum += Math.Pow((orignalSignal[i] - sampledSignal[i]), 2);
            }

            double result = fraction * sum;

            return Math.Round(result, 4, MidpointRounding.AwayFromZero); 
        }

        public static double SignalToNoiseRatio(List<double> orignalSignal, List<double> sampledSignal)
        {
            List<double> quantizedSignal = QuantizedSignal(orignalSignal.Count(), sampledSignal);

            double numerator = 0;
            double denominator = 0;
            int N = sampledSignal.Count;

            for (int i = 0; i < N; i++)
            {
                numerator += Math.Pow(orignalSignal[i], 2);
            }

            for (int i = 0; i < N; i++)
            {
                denominator += Math.Pow(orignalSignal[i] - sampledSignal[i], 2);
            }

            double result = 10 * Math.Log10(numerator / denominator);

            return Math.Round(result, 4, MidpointRounding.AwayFromZero);
        }

        public static double PeakSignalToNoiseRatio(List<double> orignalSignal, List<double> sampledSignal)
        {
            List<double> quantizedSignal = QuantizedSignal(orignalSignal.Count(), sampledSignal);

            double mse = MeanSquaredError(orignalSignal, sampledSignal);
            double numerator = sampledSignal.Max();

            double result = 10 * Math.Log10(numerator / mse);

            return Math.Round(result, 4, MidpointRounding.AwayFromZero);
        }

        public static double MaximumDifference(List<double> orignalSignal, List<double> sampledSignal)
        {
            List<double> quantizedSignal = QuantizedSignal(orignalSignal.Count(), sampledSignal);

            int N = sampledSignal.Count;
            List<double> differences = new List<double>(N);

            for (int i = 0; i < N; i++)
            {
                differences.Add(Math.Abs(orignalSignal[i] - sampledSignal[i]));
            }

            double result = differences.Max();

            return Math.Round(result, 4, MidpointRounding.AwayFromZero);
        }

        public static double EffectiveNumberOfBits(List<double> orignalSignal, List<double> sampledSignal)
        {
            List<double> quantizedSignal = QuantizedSignal(orignalSignal.Count(), sampledSignal);

            double snr =  SignalToNoiseRatio(orignalSignal, sampledSignal);

            double result = (snr - 1.76) / 6.02;

            return Math.Round(result, 4, MidpointRounding.AwayFromZero);
        }

        private static List<double> QuantizedSignal(int orignalSignalCount, List<double> sampledSignal)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < sampledSignal.Count(); i++)
            {
                for (int j = 0; j < (int)orignalSignalCount / sampledSignal.Count(); j++)
                    result.Add(sampledSignal[i]);
            }

            return result;
        }

        public static List<double> ConvoluteSignals(List<double> signal1, List<double> signal2)
        {
            var result = new List<double>();

            for (int i = 0; i < signal1.Count + signal2.Count - 1; i++)
            {
                double sum = 0;
                for (int j = 0; j < signal1.Count; j++)
                {
                    if (i - j < 0 || i - j >= signal2.Count)
                        continue;

                    sum += signal1[j] * signal2[i - j];
                }
                result.Add(sum);
            }

            return result;
        }

        public static List<double> CorelateSignals(List<double> signal1, List<double> signal2)
        {
            List<double> reversedSignal = signal1.ToList();
            reversedSignal.Reverse();

            return ConvoluteSignals(reversedSignal, signal2);
        }

        #region Filtry

        public static List<double> LowPassFilter(int M, double K)
        {
            List<double> factors = new List<double>();
            int center = (M - 1) / 2;

            for (int i = 1; i <= M; i++)
            {
                double factor;
                if (i == center)
                {
                    factor = 2.0 / K;
                }
                else
                {
                    factor = Math.Sin(2 * Math.PI * (i - center) / K) / (Math.PI * (i - center));
                }

                factors.Add(factor);
            }

            return factors;
        }

        public static List<double> MidPassFilter(int M, double K)
        {
            List<double> lowPassFactors = LowPassFilter(M, K);
            List<double> factors = new List<double>();

            for (int i = 0; i < lowPassFactors.Count; i++)
            {
                factors.Add(lowPassFactors[i] * 2 * Math.Sin(Math.PI * i / 2.0));
            }

            return factors;
        }

        public static List<double> HighPassFilter(int M, double K)
        {
            List<double> lowPassFactors = LowPassFilter(M, K);
            List<double> factors = new List<double>();

            for (int i = 0; i < lowPassFactors.Count; i++)
            {
                factors.Add(lowPassFactors[i] * Math.Pow(-1.0, i));
            }

            return factors;
        }

        public static List<double> RectangularWindow(List<double> filterFactors, int M)
        {
            return filterFactors;
        }

        public static List<double> HammingWindow(List<double> filterFactors, int M)
        {
            List<double> factors = new List<double>();

            for (int i = 0; i < filterFactors.Count; i++)
            {
                double windowFactor = 0.53836 - (0.46164 * Math.Cos(2 * Math.PI * i / M));
                factors.Add(windowFactor * filterFactors[i]);
            }

            return factors;
        }

        public static List<double> HanningWindow(List<double> filterFactors, int M)
        {
            List<double> factors = new List<double>();

            for (int i = 0; i < filterFactors.Count; i++)
            {
                double windowFactor = 0.5 - (0.5 * Math.Cos(2 * Math.PI * i / M));
                factors.Add(windowFactor * filterFactors[i]);
            }

            return factors;
        }

        public static List<double> BlackmanWindow(List<double> filterFactors, int M)
        {
            List<double> factors = new List<double>();

            for (int i = 0; i < filterFactors.Count; i++)
            {
                double windowFactor = 0.42 - (0.5 * Math.Cos(2 * Math.PI * i / M)) + (0.08 * Math.Cos(4 * Math.PI * i / M));
                factors.Add(windowFactor * filterFactors[i]);
            }

            return factors;
        }

        public static List<double> CreateFilterSignal(int M, double K, Func<int, double, List<double>> filterFunction, Func<List<double>, int, List<double>> windowFunction)
        {
            return windowFunction(filterFunction(M, K), M);
        }

        #endregion


    }
}
