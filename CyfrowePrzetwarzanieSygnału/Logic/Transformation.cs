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
                throw new ArgumentException();

            for (int i = 0; i < points.Count; i++)
            {
                Complex complex = 0;

                for (int j = 0; j < points.Count; j++)
                    complex += new Complex(points[j].Real, points[j].Imaginary) * CoreFactor(i, j, points.Count);

                result.Add(complex / points.Count);
            }

            return result;
        }

        public static List<double> DiscreteFourierBackwardTransformation(List<Complex> points)
        {
            List<double> result = new List<double>();

            if ((points.Count != 0) && ((points.Count & (points.Count - 1)) != 0))
                throw new ArgumentException();

            for (int i = 0; i < points.Count; i++)
            {
                double real = 0;

                for (int j = 0; j < points.Count; j++)
                    real += (points[j] * ReverseCoreFactor(i, j, points.Count)).Real;

                result.Add(real);
            }

            return result;
        }

        private static Complex CoreFactor(int m, int n, int N)
        {
            return Complex.Exp(new Complex(0, -2 * Math.PI * m * n / N));
        }

        private static Complex ReverseCoreFactor(int m, int n, int N)
        {
            return Complex.Exp(new Complex(0, 2 * Math.PI * m * n / N));
        }

        #endregion

        #region Szybka Transformacja Fouriera

        public static List<Complex> FastFourierTransformation(List<double> realPoints)
        {
            return null;
        }

        public static List<double> FastFourierBackwardTransformation(List<Complex> points)
        {
            return null;
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

            List<double> xh = Operations.ConvoluteSignals(points, H).Take(points.Count).ToList();
            List<double> xg = Operations.ConvoluteSignals(points, G).Take(points.Count).ToList();

            List<double> xhHalf = new List<double>();
            List<double> xgHalf = new List<double>();

            for (int i = 0; i < xh.Count; i++)
            {
                if (i % 2 == 0)
                {
                    xhHalf.Add(xh[i]);
                }
                else
                {
                    xgHalf.Add(xg[i]);
                }
            }
            for (int i = 0; i < xgHalf.Count; i++)
            {
                result.Add(new Complex(xhHalf[i], xgHalf[i]));
            }

            return result;
        }

        public static List<double> WaveletBackwardTransformation(List<Complex> points)
        {
            var HRevesed = new List<double>(H);
            HRevesed.Reverse();
            var GReversed = new List<double>(G);
            GReversed.Reverse();
            List<double> xh = new List<double>();
            List<double> xg = new List<double>();
            for (int i = 0; i < points.Count; i++)
            {
                xh.Add(points[i].Real);
                xh.Add(0);
                xg.Add(0);

                xg.Add(points[i].Imaginary);
            }
            List<double> xhC = Operations.ConvoluteSignals(xh, HRevesed).Take(xh.Count).ToList();
            List<double> xgC = Operations.ConvoluteSignals(xg, GReversed).Take(xg.Count).ToList();
            return Operations.AddSignals(xhC, xgC);
        }

        #endregion

        public static List<Complex> RealToComplex(List<double> real)
        {
            List<Complex> result = new List<Complex>();

            foreach (double number in real)
                result.Add(new Complex(number, 0));

            return result;
        }
    }
}
