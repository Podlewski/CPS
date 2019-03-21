using System;

namespace Logic
{
    public class Generator
    {
        private Random Random = new Random(); // potrzebne?

        #region Factors
        
        public double A { get; set; }     // Amplitude
        public double T1 { get; set; }    // Start Time
        public double T { get; set; }     // Basic Peroid
        public double Kw { get; set; }    // Duty Cycle
        #endregion

        // 03) Sygnał sinusoidalny
        public double SinusoidalSignal(double time)
        {
            return A * Math.Sin ((2 * Math.PI / T) * (time - T1));
        }

        // 04) Sygnał sinusoidalny wyprostowany jednopołówkowo
        public double HalfWaveRectifierSignal(double time)
        {
            return 0.5 * A * (Math.Sin((2 * Math.PI / T) * (time - T1)) +
                   Math.Abs (Math.Sin((2 * Math.PI / T) * (time - T1))));
        }

        // 05) Sygnał sinusoidalny wyprostowany dwupołówkowo
        public double FullWaveRectifierSignal(double time)
        {
            return A * Math.Abs (Math.Sin((2 * Math.PI / T) * (time - T1)));
        }
    }
}
