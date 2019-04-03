using System;
using System.Collections.Generic;

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

            return Math.Round(result, 2, MidpointRounding.AwayFromZero);
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

            return Math.Round(result, 2, MidpointRounding.AwayFromZero);
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

            return Math.Round(result, 2, MidpointRounding.AwayFromZero);
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

            return Math.Round(result, 2, MidpointRounding.AwayFromZero);
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
    }
}
