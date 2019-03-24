using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public static class Operations
    {
        public static List<double> AddSignals(List<double> signalX, List<double> signalY)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < signalX.Count; i++)
            {
                result.Add(signalX[i] + signalY[i]);
            }

            return result;
        }


        public static List<double> SubtractSignals(List<double> signalX, List<double> signalY)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < signalX.Count; i++)
            {
                result.Add(signalX[i] - signalY[i]);
            }

            return result;
        }

        public static List<double> MultiplySignals(List<double> signalX, List<double> signalY)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < signalX.Count; i++)
            {
                result.Add(signalX[i] * signalY[i]);
            }

            return result;
        }

        public static List<double> DivideSignals(List<double> signalX, List<double> signalY)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < signalX.Count; i++)
            {
                result.Add(signalX[i] / signalY[i]);
            }

            return result;
        }

        public static double Average(List<double> samples, double t1 = 0, double t2 = 0, bool isDiscrete = false)
        {
            if (isDiscrete)
            {
                return 1.0 / samples.Count * Sum(samples);
            }

            return 1 / (t2 - t1) * Integral(Math.Abs((t2 - t1) / samples.Count), samples);
        }

        public static double Variance(List<double> samples, double t1 = 0, double t2 = 0, bool isDiscrete = false)
        {
            if (isDiscrete)
            {
                return 1.0 / samples.Count * Sum(samples, d => Math.Pow(d - Average(samples, isDiscrete: true), 2));
            }

            return 1 / (t2 - t1) * Integral(Math.Abs((t2 - t1) / samples.Count), samples, d => Math.Pow(d - Average(samples, t1, t2), 2));

        }
        public static double AbsAverage(List<double> samples, double t1 = 0, double t2 = 0, bool isDiscrete = false)
        {
            if (isDiscrete)
            {
                return 1.0 / samples.Count * Sum(samples, Math.Abs);
            }

            return 1 / (t2 - t1) * Integral(Math.Abs((t2 - t1) / samples.Count), samples, Math.Abs);
        }

        public static double AveragePower(List<double> samples, double t1 = 0, double t2 = 0, bool isDiscrete = false)
        {
            if (isDiscrete)
            {
                return 1.0 / samples.Count * Sum(samples, d => d * d);
            }

            return 1 / (t2 - t1) * Integral(Math.Abs((t2 - t1) / samples.Count), samples, d => d * d);
        }

        public static double RootMeanSquare(List<double> samples, double t1 = 0, double t2 = 0, bool isDiscrete = false)
        {
            if (isDiscrete)
            {
                return Math.Sqrt(AveragePower(samples, isDiscrete: true));
            }

            return Math.Sqrt(AveragePower(samples, t1, t2));
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
