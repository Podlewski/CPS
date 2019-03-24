namespace Logic
{
    public static class ExtensionMethods
    {
        public static double GenerateSignal(this Generator generator, string signal, double time)
        {
            switch (signal.Substring(0, 2))
            {
                case "01":
                    return generator.UniformDistribution(time); 
                case "02":
                    return generator.GaussianNoise(time); 
                case "03":
                    return generator.SinusoidalSignal(time); 
                case "04":
                    return generator.HalfWaveRectifierSignal(time);
                case "05":
                    return generator.FullWaveRectifierSignal(time);
                case "06":
                    return generator.RectangularSignal(time); 
                case "07":
                    return generator.RectangularSymmetricalSignal(time); 
                case "08":
                    return generator.TriangularSignal(time);
                case "09":
                    return generator.UnitStepFunction(time); 
                case "10":
                    return generator.UnitImpulse(time); 
                case "11":
                    return generator.ImpulseNoise(time); 
                default:
                    return generator.SinusoidalSignal(time);
            }
        }

    }
}
