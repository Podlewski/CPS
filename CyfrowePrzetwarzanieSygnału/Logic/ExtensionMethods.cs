﻿using System;
using System.Collections.Generic;
using System.Numerics;

namespace Logic
{
    public static class ExtensionMethods
    {
        public static Func<double, double> SelectGenerator(this Generator generator, string signal)
        {
            switch (signal.Substring(0, 2))
            {
                case "01":
                    return generator.UniformDistribution; 
                case "02":
                    return generator.GaussianNoise; 
                case "03":
                    return generator.SinusoidalSignal; 
                case "04":
                    return generator.HalfWaveRectifierSignal;
                case "05":
                    return generator.FullWaveRectifierSignal;
                case "06":
                    return generator.RectangularSignal; 
                case "07":
                    return generator.RectangularSymmetricalSignal; 
                case "08":
                    return generator.TriangularSignal;
                case "09":
                    return generator.UnitStepFunction; 
                case "10":
                    return generator.UnitImpulse; 
                case "11":
                    return generator.ImpulseNoise;
                case "12":
                    return generator.CustomSinusoidalSignal;
                default:
                    return null;
            }
        }
        public static bool IsGenerationScattered(this string signal)
        {
            if (signal.Substring(0, 2) == "10" || signal.Substring(0, 2) == "11")
                return true;

            return false;
        }

        public static List<double> SignalOperation(this string operation, List<double> firstSamples, List<double> secondSamples)
        {
            switch (operation.Substring(0, 1))
            {
                case "1":
                    return Operations.AddSignals(firstSamples, secondSamples);
                case "2":
                    return Operations.SubtractSignals(firstSamples, secondSamples);
                case "3":
                    return Operations.MultiplySignals(firstSamples, secondSamples);
                case "4":
                    return Operations.DivideSignals(firstSamples, secondSamples);
                case "5":
                    return Operations.ConvoluteSignals(firstSamples, secondSamples);
                case "6":
                    return Operations.DirectlyCorelateSignals(firstSamples, secondSamples);
                case "7":
                    return Operations.IndirectlyCorelateSignals(firstSamples, secondSamples);
                default:
                    return null;
            }
        }

        public static List<double> FilterOperation(this string operation, int M, double F0, double Fp)
        {
            switch (operation.Substring(0, 1))
            {
                case "1":
                    return Operations.LowPassFilter(M, F0, Fp);
                case "2":
                    return Operations.BandPassFilter(M, F0, Fp);
                case "3":
                    return Operations.HighPassFilter(M, F0, Fp);
                default:
                    return null;
            }
        }

        public static List<double> WindowOperation(this string operation, List<double> filterFactors, int M)
        {
            switch (operation.Substring(0, 1))
            {
                case "1":
                    return Operations.RectangularWindow(filterFactors, M);
                case "2":
                    return Operations.HanningWindow(filterFactors, M);
                case "3":
                    return Operations.HammingWindow(filterFactors, M);
                case "4":
                    return Operations.BlackmanWindow(filterFactors, M);
                default:
                    return null;
            }
        }

        public static List<Complex> TransformOperation(this string operation, List<double> samples)
        {
            switch (operation.Substring(0, 1))
            {
                case "1":
                    return Transformation.DiscreteFourierTransformation(samples);
                case "2":
                    return Transformation.FastFourierTransformation(samples); 
                case "3":
                    return Transformation.WaveletTransformation(samples);
                default:
                    return null;
            }
        }

        public static List<double> TransformBackwardOperation(this string operation, List<Complex> samples)
        {
            switch (operation.Substring(0, 1))
            {
                case "1":
                    return Transformation.DiscreteFourierBackwardTransformation(samples);
                case "2":
                    return Transformation.FastFourierBackwardTransformation(samples);
                case "3":
                    return Transformation.WaveletBackwardTransformation(samples);
                default:
                    return null;
            }
        }

    }
}
