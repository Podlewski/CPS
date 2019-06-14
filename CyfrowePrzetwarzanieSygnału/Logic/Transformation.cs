using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace Logic
{
    public static class Transformation
    {
        #region Dyskretna Transformacja Fouriera

        public static List<Complex> DiscreteFourierTransformation(List<double> realPoints)
        {
            List<Complex> points = RealToComplex(realPoints);
            List<Complex> result = new List<Complex>();

            if ((points.Count != 0) && ((points.Count & (points.Count - 1)) != 0))
                throw new ArgumentException("Liczba próbek musi być potęgą dwójki");

            for (int i = 0; i < points.Count; i++)
            {
                Complex complex = 0;

                for (int j = 0; j < points.Count; j++)
                    complex += new Complex(points[j].Real, points[j].Imaginary) * TwiddleFactor(i, j, points.Count);

                result.Add(complex / points.Count);
            }

            return result;
        }

        public static List<double> DiscreteFourierBackwardTransformation(List<Complex> points)
        {
            List<double> result = new List<double>();

            if ((points.Count != 0) && ((points.Count & (points.Count - 1)) != 0))
                throw new ArgumentException("Liczba próbek musi być potęgą dwójki");

            for (int i = 0; i < points.Count; i++)
            {
                double real = 0;

                for (int j = 0; j < points.Count; j++)
                    real += (points[j] * ReversalTwiddleFactor(i, j, points.Count)).Real;

                result.Add(real);
            }

            return result;
        }

        #endregion

        #region Szybka Transformacja Fouriera

        public static List<Complex> FastFourierTransformation(List<double> realPoints)
        {
            List<Complex> points = RealToComplex(realPoints);

            List<Complex> transformed = SwitchSamples(points);

            return transformed.Select(c => c / points.Count).ToList();
        }

        public static List<double> FastFourierBackwardTransformation(List<Complex> points)
        {
            List<Complex> transformed = SwitchSamples(points, true);

            return transformed.Select(c => c.Real).ToList();
        }

        public static List<Complex> SwitchSamples(List<Complex> points, bool reverse = false)
        {
            if (points.Count < 2)
                return points;

            List<Complex> odd = new List<Complex>();
            List<Complex> even = new List<Complex>();

            for (int i = 0; i < points.Count / 2; i++)
            {
                even.Add(points[i * 2]);
                odd.Add(points[i * 2 + 1]);
            }

            var result = Connect(SwitchSamples(even, reverse), SwitchSamples(odd, reverse), reverse);

            return result;
        }

        private static List<Complex> Connect(List<Complex> evenPoints, List<Complex> oddPoints, bool reverse)
        {
            List<Complex> result = new List<Complex>();
            List<Complex> resultRight = new List<Complex>();

            for (int i = 0; i < oddPoints.Count; i++)
            {
                if (!reverse)
                {
                    result.Add(evenPoints[i] + TwiddleFactor(i, 1, oddPoints.Count * 2) * oddPoints[i]);
                    resultRight.Add(evenPoints[i] - TwiddleFactor(i, 1, oddPoints.Count * 2) * oddPoints[i]);
                }
                else
                {
                    result.Add(evenPoints[i] + ReversalTwiddleFactor(i, 1, oddPoints.Count * 2) * oddPoints[i]);
                    resultRight.Add(evenPoints[i] - ReversalTwiddleFactor(i, 1, oddPoints.Count * 2) * oddPoints[i]);
                }
            }

            result.AddRange(resultRight);

            return result;
        }

        #endregion

        #region Transformacja Falkowa

        private static List<double> H = new List<double>
        {
            0.32580343,
            1.01094572,
            0.8922014,
            -0.03957503,
            -0.26450717,
            0.0436163,
            0.0465036,
            -0.01498699
        };

        private static List<double> G = new List<double>
        {
            H[7],
            -H[6],
            H[5],
            -H[4],
            H[3],
            -H[2],
            H[1],
            -H[0],
        };


        public static List<Complex> WaveletTransformation(List<double> points)
        {
            List<Complex> result = new List<Complex>();

            List<double> hSamples = Operations.ConvoluteSignals(points, H).Take(points.Count).ToList();
            List<double> gSamples = Operations.ConvoluteSignals(points, G).Take(points.Count).ToList();

            List<double> hHalf = new List<double>();
            List<double> gHalf = new List<double>();

            for (int i = 0; i < hSamples.Count; i++)
            {
                if (i % 2 == 0)
                    hHalf.Add(hSamples[i]);

                else
                    gHalf.Add(gSamples[i]);
            }

            for (int i = 0; i < gHalf.Count; i++)
                result.Add(new Complex(hHalf[i], gHalf[i]));

            return result;
        }

        public static List<double> WaveletBackwardTransformation(List<Complex> points)
        {
            List<double> hRevesed = new List<double>(H);
            List<double> gReversed = new List<double>(G);

            hRevesed.Reverse();
            gReversed.Reverse();

            List<double> hSamples = new List<double>();
            List<double> gSamples = new List<double>();

            for (int i = 0; i < points.Count; i++)
            {
                hSamples.Add(points[i].Real);
                hSamples.Add(0);

                gSamples.Add(0);
                gSamples.Add(points[i].Imaginary);
            }

            List<double> hResult = Operations.ConvoluteSignals(hSamples, hRevesed).Take(hSamples.Count).ToList();
            List<double> gResult = Operations.ConvoluteSignals(gSamples, gReversed).Take(gSamples.Count).ToList();

            return Operations.AddSignals(hResult, gResult);
        }

        #endregion

        public static List<Complex> RealToComplex(List<double> real)
        {
            List<Complex> result = new List<Complex>();

            foreach (double number in real)
                result.Add(new Complex(number, 0));

            return result;
        }

        private static Complex TwiddleFactor(int m, int n, int N)
        {
            return Complex.Exp(new Complex(0, -2 * Math.PI * m * n / N));
        }

        private static Complex ReversalTwiddleFactor(int m, int n, int N)
        {
            return Complex.Exp(new Complex(0, 2 * Math.PI * m * n / N));
        }
    }
}
