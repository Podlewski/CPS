﻿using System;

namespace Logic
{
    public class Generator
    {
        public Random random = new Random();

        #region Factors
        
        public double A { get; set; }     // Amplitude
        public double T1 { get; set; }    // Start Time
        public double T { get; set; }     // Basic Peroid
        public double Kw { get; set; }    // Duty Cycle
        public double Ts { get; set; }    // ??
        public double P { get; set; }     // Probability
        #endregion

        // 01) Szum o rozkładzie jednostajnym
        public double UniformDistribution(double a)
        {
            return random.NextDouble() * 2 * A - A;
        }

        // 02) Szum Gaussowski
        public double GaussianNoise(double time)
        {
            //double mean = 2 * Amplitude;
            double stdDev = A / 3;

            //nuget version - simpler
            //Normal normalDist = new Normal(mean, stdDev);
            //return normalDist.Sample();

            double u1 = 1.0 - random.NextDouble(); //to avoid log(0)=Inf
            double u2 = 1.0 - random.NextDouble();
            double normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return normal * stdDev;
        }

        // 03) Sygnał sinusoidalny
        public double SinusoidalSignal(double time)
        {
            return A * Math.Sin ((2 * Math.PI / T) * (time - T1));
        }

        // 04) Sygnał sinusoidalny wyprostowany jednopołówkowo
        public double HalfWaveRectifierSignal(double time)
        {
            return 0.5 * A * (Math.Sin((2 * Math.PI / T) * (time - T1)) + Math.Abs (Math.Sin((2 * Math.PI / T) * (time - T1))));
        }

        // 05) Sygnał sinusoidalny wyprostowany dwupołówkowo
        public double FullWaveRectifierSignal(double time)
        {
            return A * Math.Abs (Math.Sin((2 * Math.PI / T) * (time - T1)));
        }

        // 06) Sygnał prostokątny
        public double RectangularSignal(double time)
        {
            int k = (int)((time / T) - (T1 / T));
            if (time >= (k * T + T1) && time < (Kw * T + k * T + T1))
            {
                return A;
            }

            return 0;
        }


        // 07) Sygnał prostokątny symetryczny
        public double rectangularSymmetricalSignal(double time)
        {
            int k = (int)((time / T) - (T1 / T));
            if (time >= k * T + T1 && time < Kw * T + k * T + T1)
            {
                return A;
            }

            return -A;
        }

        // 08) Sygnał trójkątny
        public double TriangularSignal(double time)
        {
            int k = (int)((time / T) - (T1 / T));
            if (time >= k * T + T1 && time < Kw * T + k * T + T1)
            {
                return (A / (Kw * T)) * (time - k * T - T1);
            }

            return -A / (T * (1 - Kw)) * (time - k * T - T1) + (A / (1 - Kw));
        }

        // 09) Skok jednostkowy
        public double UnitStepFunction(double time)
        {
            if (time > Ts)
            {
                return A;
            }

            if (time.Equals(Ts))
            {
                return 0.5 * A;
            }

            return 0;
        }

        // 10) Impuls jednostkowy - Kronecker delta
        public double UnitImpulse(double time)
        {
            if (time >= 0)
            {
                return 1;   
            }

            return 0;
        }

        // 11) Szum impulsowy
        public double ImpulseNoise(double time = 0)
        {
            double temp = random.NextDouble();
            if (P > temp)
            {
                return A;
            }

            return 0;
        }

    }
}
